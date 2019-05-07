using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    public class DatabaseSystemInfo : IDatabaseSystemInfo
    {
        private readonly Lazy<bool> supportsIncludeInIndices;
        private readonly Lazy<bool> supportsHashHpartitioning;

        public DatabaseSystemInfo()
        {
            supportsIncludeInIndices = new Lazy<bool>(() => Convert.ToInt64(VersionNumberString) >= 110000);
            supportsHashHpartitioning = new Lazy<bool>(() => Convert.ToInt64(VersionNumberString) >= 110000);
        }
        [Column("version_number_string")]
        public string VersionNumberString { get; set; }

        public bool SupportsIncludeInIndices
        {
            get { return supportsIncludeInIndices.Value; }
        }

        public bool SupportsHashHPartitioning
        {
            get { return supportsHashHpartitioning.Value; }
        }
    }
}
