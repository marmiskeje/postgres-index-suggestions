using IndexSuggestions.DAL.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndexSuggestions.IndexAnalysis
{
    public class IndexDesigner
    {
        private enum IndexStructureType
        {
            BTree = 0
        }
        #region private class IndexDefinition
        private class IndexDefinition
        {
            public IndexStructureType StructureType { get; set; }
            public IndexRelation Relation { get; set; }
            public IList<IndexAttribute> Attributes { get; private set; }
            public ISet<string> UsableInQueries { get; private set; } // fingerprints
            public ISet<string> CoveringInQueries { get; private set; } // fingerprints
            public ISet<string> ActuallyUsedInQueries { get; private set; } // fingerprints
            public IndexDefinition()
            {
                Attributes = new List<IndexAttribute>();
                UsableInQueries = new HashSet<string>();
                CoveringInQueries = new HashSet<string>();
                ActuallyUsedInQueries = new HashSet<string>();
            }

            public void MergeWith(IndexDefinition source, bool ignoreNonEquality = false)
            {
                if (!ignoreNonEquality && !Equals(source))
                {
                    throw new ArgumentException("Merge is not possible. Source is not the same index!");
                }
                UsableInQueries.AddRange(source.UsableInQueries);
                CoveringInQueries.AddRange(source.CoveringInQueries);
                ActuallyUsedInQueries.AddRange(source.ActuallyUsedInQueries);
            }

            private string CreateIdentificationString()
            {
                return $"{StructureType}_{Relation?.ID.ToString() ?? "UnknownRelation"}_{String.Join("_", Attributes.Select(x => x.Name))}";
            }

            public override int GetHashCode()
            {
                return CreateIdentificationString().GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is IndexDefinition))
                {
                    return false;
                }
                IndexDefinition tmp = (IndexDefinition)obj;
                return String.Equals(CreateIdentificationString(), tmp.CreateIdentificationString());
            }

            public IndexDefinition DeepCopy()
            {
                IndexDefinition result = new IndexDefinition();
                result.StructureType = StructureType;
                result.Relation = new IndexRelation() { ID = Relation.ID, Name = Relation.Name, DatabaseName = Relation.DatabaseName, SchemaName = Relation.SchemaName };
                foreach (var a in Attributes)
                {
                    result.Attributes.Add(new IndexAttribute() { Name = a.Name, Relation = result.Relation });
                }
                result.UsableInQueries.AddRange(UsableInQueries);
                result.CoveringInQueries.AddRange(CoveringInQueries);
                result.ActuallyUsedInQueries.AddRange(ActuallyUsedInQueries);
                return result;
            }

            public override string ToString()
            {
                return CreateIdentificationString();
            }
        }
        #endregion

        #region private class IndexRelation
        private class IndexRelation
        {
            public uint ID { get; set; }
            public string Name { get; set; }
            public string SchemaName { get; set; }
            public string DatabaseName { get; set; }
            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is IndexRelation))
                {
                    return false;
                }
                IndexRelation tmp = (IndexRelation)obj;
                return ID.Equals(tmp.ID);
            }
        } 
        #endregion

        #region private class IndexAttribute
        private class IndexAttribute : IEquatable<IndexAttribute>
        {
            public IndexRelation Relation { get; set; }
            public string Name { get; set; }
            private string CreateIdentificationString()
            {
                return $"{Relation?.ID.ToString() ?? "UnknownRelation"}_{Name}";
            }
            public override int GetHashCode()
            {
                return CreateIdentificationString().GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is IndexAttribute))
                {
                    return false;
                }
                return Equals((IndexAttribute)obj);
            }

            public bool Equals(IndexAttribute tmp)
            {
                if (tmp == null)
                {
                    return false;
                }
                return String.Equals(CreateIdentificationString(), tmp.CreateIdentificationString());
            }
        }
        #endregion

        #region private class ExecutionPlan
        private class ExecutionPlan
        {
            public string PlainString { get; private set; }
            public JObject Json { get; private set; }
            public decimal TotalCost { get; private set; }

            public ExecutionPlan(string json)
            {
                PlainString = json;
                Json = JObject.Parse("{ \"RESULT_PLAN\":" + json + "}");
                var rootPlan = Json.SelectTokens("RESULT_PLAN[0].Plan").First();
                TotalCost = rootPlan.SelectToken("['Total Cost']").Value<decimal>();
            }
        }
        #endregion

        private class VirtualIndexData
        {
            public DBMS.Contracts.IVirtualIndex VirtualIndex { get; private set; }
            public IndexDefinition Definition { get; private set; }
            public string VirtualIndexName { get; private set; }
            public VirtualIndexData(DBMS.Contracts.IVirtualIndex virtualIndex, IndexDefinition definition)
            {
                VirtualIndex = virtualIndex;
                Definition = definition;
                VirtualIndexName = definition.ToString();
            }
        }
        private const int MAX_INDEX_TUPLE_LENGTH = 6;
        private readonly IRepositoriesFactory dalRepositories;
        private readonly DBMS.Contracts.IRepositoriesFactory dbmsRepositories;
        public IndexDesigner(IRepositoriesFactory dalRepositories, DBMS.Contracts.IRepositoriesFactory dbmsRepositories)
        {
            this.dalRepositories = dalRepositories;
            this.dbmsRepositories = dbmsRepositories;
        }
        public Task Run(long workloadId)
        {
            return Task.Run(() =>
            {
                NormalizedWorkloadStatementFilter filter = new NormalizedWorkloadStatementFilter();
                filter.CommandType = StatementQueryCommandType.Select;
                filter.MinExecutionsCount = 0; // todo from workload definition
                var workloadSelectStatements = dalRepositories.GetNormalizedWorkloadStatementsRepository().GetAllByWorkloadId(workloadId, filter);
                Dictionary<IndexRelation, ISet<IndexDefinition>> possibleIndices = new Dictionary<IndexRelation, ISet<IndexDefinition>>();
                var relationsRepository = dbmsRepositories.GetRelationsRepository();
                var attributesRepository = dbmsRepositories.GetRelationAttributesRepository();
                foreach (var statement in workloadSelectStatements)
                {
                    foreach (var query in statement.NormalizedStatement.StatementDefinition.IndependentQueries)
                    {
                        var whereAttributes = new HashSet<IndexAttribute>();
                        var joinAttributes = new HashSet<IndexAttribute>();
                        var havingAttributes = new HashSet<IndexAttribute>();
                        var orderByAttributes = new HashSet<IndexAttribute>();
                        var groupByAttributes = new HashSet<IndexAttribute>();
                        var projectionAttributes = new HashSet<IndexAttribute>();
                        FillAllAttributesFromExpressions(relationsRepository, attributesRepository, query.WhereExpressions, whereAttributes);
                        FillAllAttributesFromExpressions(relationsRepository, attributesRepository, query.JoinExpressions, joinAttributes);
                        FillAllAttributesFromExpressions(relationsRepository, attributesRepository, query.HavingExpressions, havingAttributes);
                        FillAllAttributesFromExpressions(relationsRepository, attributesRepository, query.OrderByExpressions, orderByAttributes);
                        FillAllAttributesFromExpressions(relationsRepository, attributesRepository, query.GroupByExpressions, groupByAttributes);
                        foreach (var t in query.ProjectionAttributes)
                        {
                            var attribute = attributesRepository.Get(t.RelationID, t.AttributeNumber);
                            var relation = relationsRepository.Get(t.RelationID);
                            projectionAttributes.Add(new IndexAttribute() { Relation = new IndexRelation() { ID = t.RelationID, Name = relation.Name, DatabaseName = relation.DatabaseName, SchemaName = relation.SchemaName }, Name = attribute.Name });
                        }
                        GenerateIndices(whereAttributes, joinAttributes, havingAttributes, orderByAttributes, groupByAttributes, projectionAttributes, possibleIndices, statement.NormalizedStatement.StatementFingerprint);
                    }
                }
                // odstran vsetky uz existujuce indexy
                var existingIndices = dbmsRepositories.GetIndicesRepository().GetAll();
                HashSet<IndexDefinition> existingIndicesSet = new HashSet<IndexDefinition>();
                Dictionary<uint, IndexRelation> relationsCache = new Dictionary<uint, IndexRelation>();
                foreach (var i in existingIndices)
                {
                    if (!relationsCache.ContainsKey(i.RelationID))
                    {
                        relationsCache.Add(i.RelationID, new IndexRelation() { ID = i.RelationID, DatabaseName = i.DatabaseName, Name = i.Name, SchemaName = i.SchemaName });
                    }
                    var toAdd = new IndexDefinition()
                    {
                        Relation = relationsCache[i.RelationID],
                    };
                    foreach (var attrName in i.AttributesNames)
                    {
                        toAdd.Attributes.Add(new IndexAttribute() { Name = attrName, Relation = toAdd.Relation });
                    }
                    existingIndicesSet.Add(toAdd);
                }
                // vypis existujucich, ktore maju budu zmazane:
                // vypis vsetkych moznych indexov
                foreach (var kv in possibleIndices)
                {
                    var relation = kv.Key;
                    var relationIndices = kv.Value.Intersect(existingIndicesSet);
                    Console.WriteLine($"Removed existing indices for: {relation.DatabaseName}.{relation.SchemaName}.{relation.Name}");
                    foreach (var item in relationIndices)
                    {
                        Console.WriteLine(item.ToString());
                    }
                }
                Console.WriteLine("#######################################");
                // zmazanie existujucich
                foreach (var kv in possibleIndices)
                {
                    var relationIndices = kv.Value;
                    relationIndices.ExceptWith(existingIndicesSet);
                }
                // vypis vsetkych moznych indexov
                foreach (var kv in possibleIndices)
                {
                    var relation = kv.Key;
                    var relationIndices = kv.Value;
                    Console.WriteLine($"Relation possible indices for: {relation.DatabaseName}.{relation.SchemaName}.{relation.Name}");
                    foreach (var item in relationIndices)
                    {
                        Console.WriteLine(item.ToString());
                    }
                }
                Console.WriteLine("#######################################");
                // ziskaj si aktualne exekucne plany bez hypo indexov
                var explainRepository = dbmsRepositories.GetExplainRepository();
                Dictionary<string, ExecutionPlan> originalQueryPlans = new Dictionary<string, ExecutionPlan>();
                Dictionary<string, NormalizedWorkloadStatement> workloadStatementsByFingerprint = workloadSelectStatements.ToDictionary(x => x.NormalizedStatement.StatementFingerprint);
                foreach (var statement in workloadSelectStatements)
                {
                    var explainResult = explainRepository.Eplain(statement.RepresentativeStatement);
                    originalQueryPlans.Add(statement.NormalizedStatement.StatementFingerprint, new ExecutionPlan(explainResult.PlanJson));
                }
                // Pre kazde normalizovane query bude znamy prinos kazdeho (relevantneho*) indexu (tj. kazdy index vytvorime samostatne a zistime prinos pre kazde query).
                // *relevantny index je taky, ktory je nad relaciou, ktora je v query a atributmi, kde aspon jeden je v query
                // Toto nie je dobre. Ak je v query WHERE A = 1 OR B = 1 a my mame index iba pre A, moze sa pouzit radsej sekvencny prechod!!
                var virtualIndicesRepository = dbmsRepositories.GetVirtualIndicesRepository();
                foreach (var kv in possibleIndices)
                {
                    var relation = kv.Key;
                    var relationPossibleIndices = kv.Value;
                    foreach (var i in relationPossibleIndices)
                    {
                        // create index
                        var virtualIndex = virtualIndicesRepository.Create(new DBMS.Contracts.VirtualIndexDefinition()
                        {
                            CreateStatement = $"CREATE INDEX ON {relation.SchemaName}.{relation.Name} ({String.Join(",", i.Attributes.Select(x => x.Name))})"
                        });

                        try
                        {
                            // zisti, ci bude vobec pouzity a ci zlepsia cenu
                            foreach (var queryFingerprint in i.UsableInQueries)
                            {
                                var statement = workloadStatementsByFingerprint[queryFingerprint];
                                var explainResult = explainRepository.Eplain(statement.RepresentativeStatement);
                                var plan = new ExecutionPlan(explainResult.PlanJson);
                                var originalPlan = originalQueryPlans[queryFingerprint];
                                if (plan.TotalCost < originalPlan.TotalCost && plan.PlainString.Contains(virtualIndex.Name))
                                {
                                    i.ActuallyUsedInQueries.Add(queryFingerprint);
                                }
                            }
                        }
                        catch (Exception) // index creation can fail, e.g. because size
                        {
                            // todo log
                        }
                        // delete index
                        virtualIndicesRepository.DestroyAll();
                    }
                }
                // odstran indexy, ktore neboli vobec pouzite
                foreach (var kv in possibleIndices)
                {
                    kv.Value.RemoveWhere(x => x.ActuallyUsedInQueries.Count == 0);
                }
                // zresetujeme pouzitie
                foreach (var kv in possibleIndices)
                {
                    foreach (var i in kv.Value)
                    {
                        i.ActuallyUsedInQueries.Clear();
                    }
                }
                // Skusime urobit behove prostredia, kedy mame vsetky 1prvkove indexy, 2prvkove,.. az n
                Dictionary<string, VirtualIndexData> usedVirtualIndices = new Dictionary<string, VirtualIndexData>();
                foreach (var kv in possibleIndices)
                {
                    var relation = kv.Key;
                    foreach (var g in kv.Value.GroupBy(x => x.Attributes.Count))
                    {
                        var relationPossibleIndices = g;
                        List<VirtualIndexData> virtualIndices = new List<VirtualIndexData>();
                        foreach (var i in relationPossibleIndices)
                        {
                            // create index
                            var virtualIndex = virtualIndicesRepository.Create(new DBMS.Contracts.VirtualIndexDefinition()
                            {
                                CreateStatement = $"CREATE INDEX ON {relation.SchemaName}.{relation.Name} ({String.Join(",", i.Attributes.Select(x => x.Name))})"
                            });
                            virtualIndices.Add(new VirtualIndexData(virtualIndex, i));
                        }
                        // zisti, ktore boli pouzite a zlepsia cenu
                        foreach (var i in relationPossibleIndices)
                        {
                            foreach (var queryFingerprint in i.UsableInQueries)
                            {
                                var statement = workloadStatementsByFingerprint[queryFingerprint];
                                var explainResult = explainRepository.Eplain(statement.RepresentativeStatement);
                                var plan = new ExecutionPlan(explainResult.PlanJson);
                                var originalPlan = originalQueryPlans[queryFingerprint];
                                if (plan.TotalCost < originalPlan.TotalCost)
                                {
                                    foreach (var vi in virtualIndices)
                                    {
                                        if (plan.PlainString.Contains(vi.VirtualIndex.Name))
                                        {
                                            if (!usedVirtualIndices.ContainsKey(vi.VirtualIndexName))
                                            {
                                                usedVirtualIndices.Add(vi.VirtualIndexName, vi);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // zrus vsetky virtualne indexy
                virtualIndicesRepository.DestroyAll();
                // Tie indexy, ktore boli vobec pouzite budu potom sluzit ako behove prostredie pre test queries, tj. vsetky budu vytvorene sucasne a nad nimi budu spustane queries.
                // Takto si pre kazde query ziskame najlepsie indexy(AAA).
                foreach (var kv in possibleIndices)
                {
                    var relation = kv.Key;
                    var relationPossibleIndices = kv.Value;
                    List<VirtualIndexData> virtualIndices = new List<VirtualIndexData>();
                    foreach (var i in relationPossibleIndices)
                    {
                        // create index
                        var virtualIndex = virtualIndicesRepository.Create(new DBMS.Contracts.VirtualIndexDefinition()
                        {
                            CreateStatement = $"CREATE INDEX ON {relation.SchemaName}.{relation.Name} ({String.Join(",", i.Attributes.Select(x => x.Name))})"
                        });
                        virtualIndices.Add(new VirtualIndexData(virtualIndex, i));
                    }
                    // zisti, ktore boli pouzite a zlepsia cenu
                    foreach (var i in relationPossibleIndices)
                    {
                        foreach (var queryFingerprint in i.UsableInQueries)
                        {
                            var statement = workloadStatementsByFingerprint[queryFingerprint];
                            var explainResult = explainRepository.Eplain(statement.RepresentativeStatement);
                            var plan = new ExecutionPlan(explainResult.PlanJson);
                            var originalPlan = originalQueryPlans[queryFingerprint];
                            if (plan.TotalCost < originalPlan.TotalCost)
                            {
                                foreach (var vi in virtualIndices)
                                {
                                    if (plan.PlainString.Contains(vi.VirtualIndex.Name))
                                    {
                                        if (!usedVirtualIndices.ContainsKey(vi.VirtualIndexName))
                                        {
                                            usedVirtualIndices.Add(vi.VirtualIndexName, vi);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // zrus vsetky virtualne indexy
                    virtualIndicesRepository.DestroyAll();
                   
                }
                // a potom finale: Vytvorime vsetky 1-n prvkove kombinacie z najlepsich indexov a to budu finalne behove prostredia:
                // vypis vsetkych moznych indexov
                foreach (var g in usedVirtualIndices.Values.GroupBy(x => x.Definition.Relation))
                {
                    var relation = g.Key;
                    Console.WriteLine("========================================================");
                    Console.WriteLine($"Best indices for: {relation.DatabaseName}.{relation.SchemaName}.{relation.Name}");
                    foreach (var vi in g)
                    {
                        Console.WriteLine(vi.VirtualIndexName);
                    }
                    Console.WriteLine("========================================================");
                }
            });
        }

        private class IndexGenerationData
        {
            public Dictionary<IndexRelation, IndexGenerationRelationData> Relations { get; set; }
            public IndexGenerationData()
            {
                Relations = new Dictionary<IndexRelation, IndexGenerationRelationData>();
            }
        }

        private class IndexGenerationRelationData
        {
            public ISet<IndexAttribute> Attributes { get; set; }
            public ISet<IndexDefinition> AttributesPermutations { get; set; }
            public ISet<IndexDefinition> AttributesTillNowPermutations { get; set; }
            public IndexGenerationRelationData()
            {
                Attributes = new HashSet<IndexAttribute>();
                AttributesPermutations = new HashSet<IndexDefinition>();
                AttributesTillNowPermutations = new HashSet<IndexDefinition>();
            }
        }

        // generujeme tak, ze vygenerujeme permutacie pre where + join, doplnime group by, doplnime having a order by
        // a nakoniec pre covering indexy - doplnime jednu variaciu projection atributov v poradi podla ich unikatnosti
        private void GenerateIndices(ISet<IndexAttribute> whereAttributes, ISet<IndexAttribute> joinAttributes, ISet<IndexAttribute> havingAttributes, ISet<IndexAttribute> orderByAttributes, ISet<IndexAttribute> groupByAttributes, ISet<IndexAttribute> projectionAttributes,
                                     Dictionary<IndexRelation, ISet<IndexDefinition>> possibleIndices, string queryFingerprint)
        {
            var allSets = new List<ISet<IndexAttribute>>(new[] { whereAttributes.Union(joinAttributes).ToHashSet(), groupByAttributes, havingAttributes, orderByAttributes });
            var setsGenerationData = new List<IndexGenerationData>();
            for (int i = 0; i < allSets.Count; i++)
            {
                var generationData = new IndexGenerationData();
                var set = allSets[i];
                var attributesByRelation = set.GroupBy(x => x.Relation);
                foreach (var g in attributesByRelation)
                {
                    var relation = g.Key;
                    var attributes = g.ToHashSet();
                    var relationData = new IndexGenerationRelationData() { Attributes = attributes };
                    if (!possibleIndices.ContainsKey(relation))
                    {
                        possibleIndices.Add(relation, new HashSet<IndexDefinition>());
                    }
                    relationData.AttributesPermutations = GeneratePermutations(relation, attributes, queryFingerprint);
                    var attributesToConcat = new HashSet<IndexAttribute>(attributes);
                    var previousGenerations = setsGenerationData.Take(i + 1 -1);
                    foreach (var previous in previousGenerations)
                    {
                        if (previous.Relations.ContainsKey(relation))
                        {
                            attributesToConcat.ExceptWith(previous.Relations[relation].Attributes);
                        }
                    }
                    // odstran vsetky atributy, ktore su v predchadzajucich a ak nieco zostane, tak to !dopln! vsetky ich permutacie k predchadzajucim
                    if (i == 0)
                    {
                        relationData.AttributesTillNowPermutations = relationData.AttributesPermutations;
                    }
                    else
                    {
                        var previousAttributesPermutations = setsGenerationData[i - 1].Relations[relation].AttributesTillNowPermutations;
                        var attributesToConcatPermutations = GeneratePermutations(relation, attributesToConcat, queryFingerprint);
                        relationData.AttributesTillNowPermutations = ConcatPermutations(previousAttributesPermutations, attributesToConcatPermutations);
                    }
                    generationData.Relations.Add(relation, relationData);
                }
                // take previous generation and add relations with empty data, we need to ensure, that previous always generation exists
                if (i > 0)
                {
                    foreach (var previous in setsGenerationData[i-1].Relations)
                    {
                        var relation = previous.Key;
                        var data = previous.Value;
                        if (!generationData.Relations.ContainsKey(relation))
                        {
                            generationData.Relations.Add(relation, new IndexGenerationRelationData() { AttributesTillNowPermutations = data.AttributesTillNowPermutations });
                        }
                    }
                }
                setsGenerationData.Add(generationData);
            }
            foreach (var setData in setsGenerationData)
            {
                foreach (var kv in setData.Relations)
                {
                    var relation = kv.Key;
                    var relationData = kv.Value;
                    foreach (var i in relationData.AttributesPermutations)
                    {
                        if (possibleIndices[relation].Contains(i))
                        {
                            IndexDefinition existing = null;
                            if (possibleIndices[relation].TryGetValue(i, out existing))
                            {
                                existing.MergeWith(i);
                            }
                        }
                        else
                        {
                            possibleIndices[relation].Add(i);
                        }
                    }
                    foreach (var i in relationData.AttributesTillNowPermutations)
                    {
                        if (possibleIndices[relation].Contains(i))
                        {
                            IndexDefinition existing = null;
                            if (possibleIndices[relation].TryGetValue(i, out existing))
                            {
                                existing.MergeWith(i);
                            }
                        }
                        else
                        {
                            possibleIndices[relation].Add(i);
                        }
                    }
                }
            }
        }

        private ISet<IndexDefinition> ConcatPermutations(ISet<IndexDefinition> first, ISet<IndexDefinition> second)
        {
            if (first.Count == 0)
            {
                return second;
            }
            else if (second.Count == 0)
            {
                return first;
            }
            ISet<IndexDefinition> result = new HashSet<IndexDefinition>();
            foreach (var f in first)
            {
                foreach (var s in second)
                {
                    var newDefinition = f.DeepCopy();
                    newDefinition.Attributes.AddRange(s.Attributes);
                    newDefinition.MergeWith(s, true);
                    result.Add(newDefinition);
                }
            }
            return result;
        }

        private ISet<IndexDefinition> GeneratePermutations(IndexRelation relation, ISet<IndexAttribute> attributes, string queryFingerprint)
        {
            ISet<IndexDefinition> result = new HashSet<IndexDefinition>();
            for (int i = 1; i <= Math.Min(MAX_INDEX_TUPLE_LENGTH, attributes.Count); i++)
            {
                var permutations = PermuteUtils.Permute(attributes, i);
                foreach (var p in permutations)
                {
                    var indexToAdd = new IndexDefinition();
                    indexToAdd.Relation = relation;
                    indexToAdd.Attributes.AddRange(p);
                    indexToAdd.UsableInQueries.Add(queryFingerprint);
                    result.Add(indexToAdd);
                }
            }
            return result;
        }

        private void FillAllAttributesFromExpressions(DBMS.Contracts.IRelationsRepository relationsRepository, DBMS.Contracts.IRelationAttributesRepository attributesRepository,
                                                      IEnumerable<StatementQueryExpression> expressions, ISet<IndexAttribute> attributes)
        {
            foreach (var e in expressions)
            {
                if (e is StatementQueryAttributeExpression)
                {
                    var t = (StatementQueryAttributeExpression)e;
                    var attribute = attributesRepository.Get(t.RelationID, t.AttributeNumber);
                    var relation = relationsRepository.Get(t.RelationID);
                    attributes.Add(new IndexAttribute() { Relation = new IndexRelation() { ID = t.RelationID, Name = relation.Name, DatabaseName = relation.DatabaseName, SchemaName = relation.SchemaName }, Name = attribute.Name });
                }
                else if (e is StatementQueryBooleanExpression)
                {
                    var t = (StatementQueryBooleanExpression)e;
                    FillAllAttributesFromExpressions(relationsRepository, attributesRepository, t.Arguments, attributes);
                }
                else if (e is StatementQueryFunctionExpression)
                {
                    var t = (StatementQueryFunctionExpression)e;
                    FillAllAttributesFromExpressions(relationsRepository, attributesRepository, t.Arguments, attributes);
                }
                else if (e is StatementQueryNullTestExpression)
                {
                    var t = (StatementQueryNullTestExpression)e;
                    FillAllAttributesFromExpressions(relationsRepository, attributesRepository, new[] { t.Argument }, attributes);
                }
                else if (e is StatementQueryOperatorExpression)
                {
                    var t = (StatementQueryOperatorExpression)e;
                    // get operator, if for btree usage
                    if (true)
                    {
                        FillAllAttributesFromExpressions(relationsRepository, attributesRepository, t.Arguments, attributes);
                    }
                }
            }
        }
    }

    public class PermuteUtils
    {
        // Returns an enumeration of enumerators, one for each permutation
        // of the input.
        public static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> list, int count)
        {
            if (count == 0)
            {
                yield return new T[0];
            }
            else
            {
                int startingElementIndex = 0;
                foreach (T startingElement in list)
                {
                    IEnumerable<T> remainingItems = AllExcept(list, startingElementIndex);

                    foreach (IEnumerable<T> permutationOfRemainder in Permute(remainingItems, count - 1))
                    {
                        yield return Concat<T>(
                            new T[] { startingElement },
                            permutationOfRemainder);
                    }
                    startingElementIndex += 1;
                }
            }
        }

        // Enumerates over contents of both lists.
        private static IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (T item in a) { yield return item; }
            foreach (T item in b) { yield return item; }
        }

        // Enumerates over all items in the input, skipping over the item
        // with the specified offset.
        private static IEnumerable<T> AllExcept<T>(IEnumerable<T> input, int indexToSkip)
        {
            int index = 0;
            foreach (T item in input)
            {
                if (index != indexToSkip) yield return item;
                index += 1;
            }
        }
    }
}
