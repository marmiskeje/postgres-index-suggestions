using IndexSuggestions.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Postgres
{
    internal class IndexSuggestionsContext : DbContext
    {
        public DbSet<NormalizedStatement> NormalizedStatements { get; set; }
        public DbSet<Index> Indices { get; set; }
        public DbSet<NormalizedStatementIndexUsage> NormalizedStatementIndexUsages { get; set; }
        public DbSet<SettingProperty> SettingProperties { get; set; }

        public IndexSuggestionsContext() : base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password = root;ApplicationName=IndexSuggestions");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NormalizedStatement>().HasIndex(x => x.Statement).IsUnique();
            modelBuilder.Entity<NormalizedStatementIndexUsage>().HasOne(x => x.Index).WithMany(x => x.NormalizedStatementIndexUsages);
            modelBuilder.Entity<NormalizedStatementIndexUsage>().HasOne(x => x.NormalizedStatement).WithMany(x => x.NormalizedStatementIndexUsages);
            modelBuilder.Entity<SettingProperty>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<SettingProperty>().SeedData(new SettingProperty() { ID = 1, Key = SettingPropertyKeys.LAST_PROCESSED_LOG_ENTRY_TIMESTAMP });
        }
    }
}
