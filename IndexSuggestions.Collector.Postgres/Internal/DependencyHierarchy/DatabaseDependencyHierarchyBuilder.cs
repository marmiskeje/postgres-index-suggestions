using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IndexSuggestions.Collector.Postgres
{
    public class DatabaseDependencyHierarchyBuilder : IDatabaseDependencyHierarchyBuilder
    {
        private readonly ILog log;
        private readonly IViewsRepository viewsRepository;
        private readonly IDatabaseDependencyObjectsRepository dependenciesRepository;
        private readonly IStoredProceduresRepository proceduresRepository;
        public DatabaseDependencyHierarchyBuilder(ILog log, IRepositoriesFactory repositories)
        {
            this.log = log;
            this.viewsRepository = repositories.GetViewsRepository();
            this.dependenciesRepository = repositories.GetDatabaseDependencyObjectsRepository();
            this.proceduresRepository = repositories.GetStoredProceduresRepository();
        }
        public IDatabaseDependencyHierarchy Build()
        {
            DatabaseDependencyHierarchy hierarchy = new DatabaseDependencyHierarchy();
            var allViews = viewsRepository.GetAll();
            var allProcedures = proceduresRepository.GetAll().Where(x => x.SchemaName == "test");
            foreach (var view in allViews)
            {
                var toAdd = BuildDependencyObject(allViews, allProcedures, new HashSet<IDependencyHierarchyObject>(),
                                    new HashSet<IDependencyHierarchyObject>(), CreateDependencyHierarchyObject(view));
                if (toAdd != null)
                {
                    hierarchy.Views.Add(toAdd); 
                }
            }
            foreach (var proc in allProcedures)
            {
                var toAdd = BuildDependencyObject(allViews, allProcedures, new HashSet<IDependencyHierarchyObject>(),
                                    new HashSet<IDependencyHierarchyObject>(), CreateDependencyHierarchyObject(proc)) as IStoredProcedureHierarchyObject;
                if (toAdd != null)
                {
                    hierarchy.StoredProcedures.Add(toAdd);
                }
            }
            return hierarchy;
        }

        private IDependencyHierarchyObject BuildDependencyObject(IEnumerable<IView> allLoadedViews, IEnumerable<IStoredProcedure> allLoadedProcedures,
                                                                 HashSet<IDependencyHierarchyObject> alreadyBuiltObjects,
                                                                 HashSet<IDependencyHierarchyObject> parents, IDependencyHierarchyObject obj)
        {
            IDependencyHierarchyObject result = obj;
            if (alreadyBuiltObjects.TryGetValue(obj, out var existing))
            {
                result = existing;
            }
            else
            {
                if (!parents.TryGetValue(obj, out var existingParent))
                {
                    var directDependencies = GetDirectDependencies(allLoadedViews, allLoadedProcedures, obj);
                    foreach (var dep in directDependencies)
                    {
                        var newDependency = CreateDependencyHierarchyObject(dep);
                        if (alreadyBuiltObjects.TryGetValue(newDependency, out var existingDependency))
                        {
                            obj.Dependencies.Add(existingDependency);
                        }
                        else
                        {
                            var newParents = new HashSet<IDependencyHierarchyObject>(parents);
                            newParents.Add(obj);
                            var builtDependency = BuildDependencyObject(allLoadedViews, allLoadedProcedures, alreadyBuiltObjects, newParents, newDependency);
                            if (builtDependency != null)
                            {
                                obj.Dependencies.Add(builtDependency); 
                            }
                        }
                    }
                }
                else
                {
                    log.Write(SeverityType.Warning, $"Cycle detected when building dependency with id: {obj.ID}.");
                    result = null;
                }
            }
            if (result != null)
            {
                alreadyBuiltObjects.Add(result);
            }
            return result;
        }

        private IEnumerable<IDatabaseDependencyObject> GetDirectDependencies(IEnumerable<IView> allLoadedViews, IEnumerable<IStoredProcedure> allLoadedProcedures,
                                                                             IDependencyHierarchyObject obj)
        {
            switch (obj)
            {
                case DependencyHierarchyObject<IView> view:
                    return dependenciesRepository.GetDependenciesForDatabaseObject(obj.ID);
                case DependencyHierarchyObject<IStoredProcedure> proc:
                    {
                        var definition = proc.DatabaseDependencyObject.Definition.ToLower();
                        List<IDatabaseDependencyObject> result = new List<IDatabaseDependencyObject>();
                        foreach (var v in allLoadedViews)
                        {
                            // from viewname or join viewname
                            var regex = new Regex("[from|join] " + v.SchemaName.ToLower() + @"\." + v.Name.ToLower());
                            if (regex.IsMatch(definition))
                            {
                                result.Add(v);
                            }
                        }
                        foreach (var p in allLoadedProcedures)
                        {
                            // functionname(,,,) where ,,, is count of arguments - 1
                            Regex regex = null;
                            if (p.ArgumentsCount == 0)
                            {
                                regex = new Regex(p.SchemaName.ToLower() + @"\." + p.Name.ToLower() + @"\(\)");
                            }
                            else
                            {
                                regex = new Regex(p.SchemaName.ToLower() + @"\." + p.Name.ToLower() + @"\(([^,)]*,){" + (p.ArgumentsCount - 1).ToString() + @"}[^,)]+\)");
                            }
                            if (regex.IsMatch(definition))
                            {
                                result.Add(p);
                            }
                        }
                        return result;
                    }
            }
            return new HashSet<IDatabaseDependencyObject>(0);
        }

        private IDependencyHierarchyObject CreateDependencyHierarchyObject(IDatabaseDependencyObject obj)
        {
            switch (obj)
            {
                case IView view:
                    return new DependencyHierarchyObject<IView>()
                    {
                        DatabaseID = view.DatabaseID,
                        ID = view.ID,
                        Name = view.Name,
                        ObjectType = DependencyHierarchyObjectType.View,
                        SchemaName = view.SchemaName,
                        DatabaseDependencyObject = view
                    };
                case IStoredProcedure procedure:
                    return new StoredProcedureHierarchyObject<IStoredProcedure>()
                    {
                        DatabaseID = procedure.DatabaseID,
                        ID = procedure.ID,
                        Name = procedure.Name,
                        ObjectType = DependencyHierarchyObjectType.StoredProcedure,
                        SchemaName = procedure.SchemaName,
                        ArgumentsCount = procedure.ArgumentsCount,
                        DatabaseDependencyObject = procedure
                    };
            }
            throw new NotSupportedException($"obj type {obj?.GetType()} is not supported");
        }
    }
}
