using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.Logging;
using DiplomaThesis.DBMS.Contracts;

namespace DiplomaThesis.Collector
{
    internal class DatabaseDependencyHierarchyProvider : IDatabaseDependencyHierarchyProvider
    {
        private readonly ILog log;
        private readonly IDatabaseDependencyHierarchyBuilder builder;
        private readonly IDatabasesRepository databasesRepository;
        private readonly Timer timer;
        private bool isDisposed = false;
       
        private ConcurrentDictionary<uint, IDatabaseDependencyHierarchy> dependencyHierarchyFilteredForViewPerDatabase = new ConcurrentDictionary<uint, IDatabaseDependencyHierarchy>();
        public DatabaseDependencyHierarchyProvider(ILog log, IDatabaseDependencyHierarchyBuilder builder, IDatabasesRepository databasesRepository)
        {
            this.log = log;
            this.builder = builder;
            this.databasesRepository = databasesRepository;
            this.timer = new Timer(Timer_Job, null, TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60));
            Timer_Job(null);
        }

        private void Timer_Job(object obj)
        {
            try
            {
                var databases = databasesRepository.GetAll();
                foreach (var d in databases)
                {
                    try
                    {
                        using (var scope = new DatabaseScope(d.Name))
                        {
                            var data = builder.Build();
                            data.StoredProcedures.IntersectWith(data.StoredProcedures.Where(x => ContainsView(x)));
                            data.Views.IntersectWith(data.Views.Where(x => ContainsView(x)));
                            dependencyHierarchyFilteredForViewPerDatabase.AddOrUpdate(d.ID, data, (k, v) => data);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Write(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Write(ex);
            }
        }

        private bool ContainsView(IDependencyHierarchyObject obj)
        {
            if (obj.ObjectType == DependencyHierarchyObjectType.View)
            {
                return true;
            }
            foreach (var d in obj.Dependencies)
            {
                if (ContainsView(d))
                {
                    return true;
                }
            }
            return false;
        }

        public IDatabaseDependencyHierarchy ProvideForDatabaseFilteredForViewOccurence(uint databaseId)
        {
            if (dependencyHierarchyFilteredForViewPerDatabase.TryGetValue(databaseId, out var result))
            {
                return result;
            }
            return null;
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                }
                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DatabaseDependencyHierarchyProvider()
        {
            Dispose(false);
        }
    }
}
