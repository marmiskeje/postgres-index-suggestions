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
        public DbSet<NormalizedStatement> NormalizedStatements { get; set; }
        public DbSet<Index> Indices { get; set; }
        public DbSet<NormalizedStatementIndexUsage> NormalizedStatementIndexUsages { get; set; }
        public DbSet<SettingProperty> SettingProperties { get; set; }
        public DbSet<Workload> Workloads { get; set; }
        public DbSet<NormalizedWorkloadStatement> NormalizedWorkloadStatements { get; set; }

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
            modelBuilder.Entity<NormalizedStatementIndexUsage>().HasOne(x => x.Index).WithMany(x => x.NormalizedStatementIndexUsages);
            modelBuilder.Entity<NormalizedStatementIndexUsage>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedStatementIndexUsages);
            modelBuilder.Entity<NormalizedWorkloadStatement>().HasOne(x => x.Workload).WithMany(x => x.NormalizedWorkloadStatements);
            modelBuilder.Entity<NormalizedWorkloadStatement>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedWorkloadStatements);
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
