using DiplomaThesis.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging;

namespace DiplomaThesis.DAL
{
    internal partial class IndexSuggestionsContext : DbContext
    {
        private readonly string providerName = null;
        private readonly string connectionString = null;
        private static readonly LoggerFactory loggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });

        public DbSet<SettingProperty> SettingProperties { get; set; }
        public DbSet<Workload> Workloads { get; set; }
        public DbSet<NormalizedStatement> NormalizedStatements { get; set; }
        public DbSet<NormalizedStatementStatistics> NormalizedStatementStatistics { get; set; }
        public DbSet<NormalizedStatementIndexStatistics> NormalizedStatementIndexStatistics { get; set; }
        public DbSet<NormalizedStatementRelationStatistics> NormalizedStatementRelationStatistics { get; set; }
        public DbSet<TotalRelationStatistics> TotalRelationStatistics { get; set; }
        public DbSet<TotalIndexStatistics> TotalIndexStatistics { get; set; }
        public DbSet<TotalStoredProcedureStatistics> TotalStoredProcedureStatistics { get; set; }
        public DbSet<TotalViewStatistics> TotalViewStatistics { get; set; }
        public DbSet<WorkloadAnalysis> WorkloadAnalyses { get; set; }
        public DbSet<VirtualEnvironment> VirtualEnvironments { get; set; }
        public DbSet<PossibleIndex> PossibleIndices { get; set; }
        public DbSet<VirtualEnvironmentPossibleIndex> VirtualEnvironmentPossibleIndices { get; set; }
        public DbSet<VirtualEnvironmentStatementEvaluation> VirtualEnvironmentStatementEvaluations { get; set; }
        public DbSet<ExecutionPlan> ExecutionPlans { get; set; }
        public DbSet<WorkloadAnalysisRealStatementEvaluation> WorkloadAnalysisRealStatementEvaluations { get; set; }
        public DbSet<VirtualEnvironmentPossibleCoveringIndex> VirtualEnvironmentPossibleCoveringIndices { get; set; }
        public DbSet<VirtualEnvironmentPossibleHPartitioning> VirtualEnvironmentPossibleHPartitionings { get; set; }
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
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }
#endif
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
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.Relational().ColumnType = "decimal(18, 6)";
            }
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NormalizedStatement>().HasIndex(x => x.StatementFingerprint).IsUnique();
            modelBuilder.Entity<NormalizedStatement>().HasIndex(x => x.CommandType).HasFilter("CommandType IS NOT NULL");
            modelBuilder.Entity<NormalizedStatementStatistics>().HasIndex(x => new { x.DatabaseID, x.NormalizedStatementID, x.UserName, x.ApplicationName, x.Date }).IsUnique();
            modelBuilder.Entity<NormalizedStatementStatistics>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedStatementStatistics);
            modelBuilder.Entity<NormalizedStatementIndexStatistics>().HasIndex(x => new { x.DatabaseID, x.NormalizedStatementID, x.IndexID, x.Date }).IsUnique();
            modelBuilder.Entity<NormalizedStatementIndexStatistics>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedStatementIndexStatistics);
            modelBuilder.Entity<NormalizedStatementRelationStatistics>().HasIndex(x => new { x.DatabaseID, x.NormalizedStatementID, x.RelationID, x.Date }).IsUnique();
            modelBuilder.Entity<NormalizedStatementRelationStatistics>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedStatementRelationStatistics);
            modelBuilder.Entity<TotalRelationStatistics>().HasIndex(x => new { x.DatabaseID, x.RelationID, x.Date }).IsUnique();
            modelBuilder.Entity<TotalRelationStatistics>().HasIndex(x => new { x.DatabaseID, x.Date });
            modelBuilder.Entity<TotalRelationStatistics>().HasIndex(x => x.RelationID);
            modelBuilder.Entity<TotalIndexStatistics>().HasIndex(x => new { x.DatabaseID, x.RelationID, x.IndexID, x.Date }).IsUnique();
            modelBuilder.Entity<TotalIndexStatistics>().HasIndex(x => x.IndexID);
            modelBuilder.Entity<TotalStoredProcedureStatistics>().HasIndex(x => new { x.DatabaseID, x.ProcedureID, x.Date }).IsUnique();
            modelBuilder.Entity<TotalStoredProcedureStatistics>().HasIndex(x => x.ProcedureID);
            modelBuilder.Entity<TotalViewStatistics>().HasIndex(x => new { x.DatabaseID, x.ViewID, x.Date }).IsUnique();
            modelBuilder.Entity<TotalViewStatistics>().HasIndex(x => x.ViewID);
            modelBuilder.Entity<SettingProperty>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<WorkloadAnalysis>().HasOne(x => x.Workload).WithMany(x => x.WorkloadAnalyses);
            modelBuilder.Entity<WorkloadAnalysis>().HasIndex(x => new { x.PeriodFromDate, x.PeriodToDate });
            modelBuilder.Entity<WorkloadAnalysis>().HasIndex(x => x.State);
            modelBuilder.Entity<VirtualEnvironment>().HasOne(x => x.WorkloadAnalysis).WithMany(x => x.VirtualEnvironments);
            modelBuilder.Entity<VirtualEnvironment>().HasIndex(x => x.Type);
            modelBuilder.Entity<VirtualEnvironmentPossibleIndex>().HasKey(x => new { x.PossibleIndexID, x.VirtualEnvironemntID });
            modelBuilder.Entity<VirtualEnvironmentPossibleIndex>().HasOne(x => x.PossibleIndex).WithMany(x => x.VirtualEnvironmentPossibleIndices).HasForeignKey(x => x.PossibleIndexID);
            modelBuilder.Entity<VirtualEnvironmentPossibleIndex>().HasOne(x => x.VirtualEnvironment).WithMany(x => x.VirtualEnvironmentPossibleIndices).HasForeignKey(x => x.VirtualEnvironemntID);
            modelBuilder.Entity<VirtualEnvironmentStatementEvaluation>().HasKey(x => new { x.ExecutionPlanID, x.NormalizedStatementID, x.VirtualEnvironmentID });
            modelBuilder.Entity<VirtualEnvironmentStatementEvaluation>().HasOne(x => x.ExecutionPlan).WithMany(x => x.VirtualEnvironmentStatementEvaluations).HasForeignKey(x => x.ExecutionPlanID);
            modelBuilder.Entity<VirtualEnvironmentStatementEvaluation>().HasOne(x => x.NormalizedStatement).WithMany(x => x.VirtualEnvironmentStatementEvaluations).HasForeignKey(x => x.NormalizedStatementID);
            modelBuilder.Entity<VirtualEnvironmentStatementEvaluation>().HasOne(x => x.VirtualEnvironment).WithMany(x => x.VirtualEnvironmentStatementEvaluations).HasForeignKey(x => x.VirtualEnvironmentID);
            modelBuilder.Entity<WorkloadAnalysisRealStatementEvaluation>().HasKey(x => new { x.ExecutionPlanID, x.NormalizedStatementID, x.WorkloadAnalysisID });
            modelBuilder.Entity<WorkloadAnalysisRealStatementEvaluation>().HasOne(x => x.ExecutionPlan).WithMany(x => x.WorkloadAnalysisRealStatementEvaluations).HasForeignKey(x => x.ExecutionPlanID);
            modelBuilder.Entity<WorkloadAnalysisRealStatementEvaluation>().HasOne(x => x.NormalizedStatement).WithMany(x => x.WorkloadAnalysisRealStatementEvaluations).HasForeignKey(x => x.NormalizedStatementID);
            modelBuilder.Entity<WorkloadAnalysisRealStatementEvaluation>().HasOne(x => x.WorkloadAnalysis).WithMany(x => x.WorkloadAnalysisRealStatementEvaluations).HasForeignKey(x => x.WorkloadAnalysisID);
            modelBuilder.Entity<VirtualEnvironmentPossibleCoveringIndex>().HasKey(x => new { x.NormalizedStatementID, x.PossibleIndexID, x.VirtualEnvironmentID });
            modelBuilder.Entity<VirtualEnvironmentPossibleCoveringIndex>().HasOne(x => x.NormalizedStatement).WithMany(x => x.VirtualEnvironmentPossibleCoveringIndices).HasForeignKey(x => x.NormalizedStatementID);
            modelBuilder.Entity<VirtualEnvironmentPossibleCoveringIndex>().HasOne(x => x.PossibleIndex).WithMany(x => x.VirtualEnvironmentPossibleCoveringIndices).HasForeignKey(x => x.PossibleIndexID);
            modelBuilder.Entity<VirtualEnvironmentPossibleCoveringIndex>().HasOne(x => x.VirtualEnvironment).WithMany(x => x.VirtualEnvironmentPossibleCoveringIndices).HasForeignKey(x => x.VirtualEnvironmentID);
            modelBuilder.Entity<VirtualEnvironmentPossibleHPartitioning>().HasOne(x => x.VirtualEnvironment).WithMany(x => x.VirtualEnvironmentPossibleHPartitionings).HasForeignKey(x => x.VirtualEnvironmentID);

            IndexSuggestionsContextSeedDataUtility.SeedData(modelBuilder);
        }
    }
}
