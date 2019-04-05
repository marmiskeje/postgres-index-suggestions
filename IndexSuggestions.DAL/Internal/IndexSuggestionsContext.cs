using IndexSuggestions.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class IndexSuggestionsContext : DbContext
    {
        private readonly string providerName = null;
        private readonly string connectionString = null;
        
        public DbSet<SettingProperty> SettingProperties { get; set; }
        public DbSet<Workload> Workloads { get; set; }
        public DbSet<NormalizedStatement> NormalizedStatements { get; set; }
        public DbSet<NormalizedWorkloadStatement> NormalizedWorkloadStatements { get; set; }
        public DbSet<NormalizedStatementStatistics> NormalizedStatementStatistics { get; set; }
        public DbSet<NormalizedStatementIndexStatistics> NormalizedStatementIndexStatistics { get; set; }
        public DbSet<NormalizedStatementRelationStatistics> NormalizedStatementRelationStatistics { get; set; }
        public DbSet<TotalRelationStatistics> TotalRelationStatistics { get; set; }
        public DbSet<TotalIndexStatistics> TotalIndexStatistics { get; set; }
        public DbSet<TotalStoredProcedureStatistics> TotalStoredProcedureStatistics { get; set; }
        public DbSet<TotalViewStatistics> TotalViewStatistics { get; set; }

        public IndexSuggestionsContext(string providerName, string connectionString) : base()
        {
            this.providerName = providerName;
            this.connectionString = connectionString;
            var connectionStringSplit = connectionString.Split(";");
            bool containsAppName = false;
            foreach (var keyValue in connectionStringSplit)
            {
                var keyValueSplit = keyValue.Split("=");
                if (keyValueSplit[0].Trim().Replace(" ", "").ToLower() == "applicationname" && keyValueSplit[1].Trim() == "IndexSuggestions")
                {
                    containsAppName = true;
                    break;
                }
            }
            if (!containsAppName)
            {
                throw new ArgumentException("ConnectionString must contain ApplicationName=IndexSuggestions!"); // otherwise infinite log processing may occur
            }
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            switch (providerName?.ToLower())
            {
                case "mariadb":
                case "mysql":
                    optionsBuilder.UseMySql(connectionString);
                    break;
                case "npgsql":
                    optionsBuilder.UseNpgsql(connectionString);
                    break;
                case "sqlserver":
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                default:
                    throw new Exception($"Unknown provider {providerName}!");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NormalizedStatement>().HasIndex(x => x.StatementFingerprint).IsUnique();
            modelBuilder.Entity<NormalizedStatement>().HasIndex(x => x.CommandType).HasFilter("CommandType IS NOT NULL");
            
            modelBuilder.Entity<NormalizedWorkloadStatement>().HasOne(x => x.Workload).WithMany(x => x.NormalizedWorkloadStatements);
            modelBuilder.Entity<NormalizedWorkloadStatement>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedWorkloadStatements);
            modelBuilder.Entity<NormalizedStatementStatistics>().HasIndex(x => new { x.DatabaseID, x.NormalizedStatementID, x.UserName, x.ApplicationName, x.Date }).IsUnique();
            modelBuilder.Entity<NormalizedStatementStatistics>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedStatementStatistics);
            modelBuilder.Entity<NormalizedStatementIndexStatistics>().HasIndex(x => new { x.DatabaseID, x.NormalizedStatementID, x.IndexID, x.Date }).IsUnique();
            modelBuilder.Entity<NormalizedStatementIndexStatistics>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedStatementIndexStatistics);
            modelBuilder.Entity<NormalizedStatementRelationStatistics>().HasIndex(x => new { x.DatabaseID, x.NormalizedStatementID, x.RelationID, x.Date }).IsUnique();
            modelBuilder.Entity<NormalizedStatementRelationStatistics>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedStatementRelationStatistics);
            modelBuilder.Entity<TotalRelationStatistics>().HasIndex(x => new { x.DatabaseID, x.RelationID, x.Date }).IsUnique();
            modelBuilder.Entity<TotalRelationStatistics>().HasIndex(x => x.RelationID);
            modelBuilder.Entity<TotalIndexStatistics>().HasIndex(x => new { x.DatabaseID, x.RelationID, x.IndexID, x.Date }).IsUnique();
            modelBuilder.Entity<TotalIndexStatistics>().HasIndex(x => x.IndexID);
            modelBuilder.Entity<TotalStoredProcedureStatistics>().HasIndex(x => new { x.DatabaseID, x.ProcedureID, x.Date }).IsUnique();
            modelBuilder.Entity<TotalStoredProcedureStatistics>().HasIndex(x => x.ProcedureID);
            modelBuilder.Entity<TotalViewStatistics>().HasIndex(x => new { x.DatabaseID, x.ViewID, x.Date }).IsUnique();
            modelBuilder.Entity<TotalViewStatistics>().HasIndex(x => x.ViewID);
            modelBuilder.Entity<SettingProperty>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<SettingProperty>().SeedData(new SettingProperty() { ID = 1, Key = SettingPropertyKeys.LAST_PROCESSED_LOG_ENTRY_TIMESTAMP });
            modelBuilder.Entity<SettingProperty>().SeedData(new SettingProperty() { ID = 2, Key = SettingPropertyKeys.ACTIVE_WORKLOAD, IntValue = 1 });
            var workload = new Workload()
            {
                ID = 1,
                Definition = new WorkloadDefinition()
                {
                    DatabaseName = "test",
                    Applications = new WorkloadPropertyValuesDefinition<string>() { RestrictionType = WorkloadPropertyRestrictionType.Disallowed },
                    DateTimeSlots = new WorkloadPropertyValuesDefinition<WorkloadDateTimeSlot>() { RestrictionType = WorkloadPropertyRestrictionType.Disallowed },
                    QueryThresholds = new WorkloadQueryThresholds(),
                    Relations = new WorkloadPropertyValuesDefinition<WorkloadRelation>() { RestrictionType = WorkloadPropertyRestrictionType.Disallowed },
                    Users = new WorkloadPropertyValuesDefinition<string>() { RestrictionType = WorkloadPropertyRestrictionType.Disallowed }
                }
            };
            workload.DefinitionData = JsonSerializationUtility.Serialize(workload.Definition);
            modelBuilder.Entity<Workload>().SeedData(workload);
        }
    }
}
