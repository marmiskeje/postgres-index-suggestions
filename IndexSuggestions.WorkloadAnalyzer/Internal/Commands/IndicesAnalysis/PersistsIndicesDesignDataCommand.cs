using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class PersistsIndicesDesignDataCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IRepositoriesFactory dalRepositories;
        private readonly IIndexSqlCreateStatementGenerator sqlCreateStatementGenerator;
        public PersistsIndicesDesignDataCommand(WorkloadAnalysisContext context, IRepositoriesFactory dalRepositories, IIndexSqlCreateStatementGenerator sqlCreateStatementGenerator)
        {
            this.context = context;
            this.dalRepositories = dalRepositories;
            this.sqlCreateStatementGenerator = sqlCreateStatementGenerator;
        }
        protected override void OnExecute()
        {
            var designData = context.IndicesDesignData;
            var virtualEnvsRepository = dalRepositories.GetVirtualEnvironmentsRepository();
            var possibleIndicesRepository = dalRepositories.GetPossibleIndicesRepository();
            var virtualEnvPossibleIndicesRepository = dalRepositories.GetVirtualEnvironmentPossibleIndicesRepository();
            var virtualEnvStatementEvalsRepository = dalRepositories.GetVirtualEnvironmentStatementEvaluationsRepository();
            var executionPlansRepository = dalRepositories.GetExecutionPlansRepository();
            var analysisRealStatementEvaluationsRepository = dalRepositories.GetWorkloadAnalysisRealStatementEvaluationsRepository();
            var virtualEnvPossibleCoveringIndicesRepository = dalRepositories.GetVirtualEnvironmentPossibleCoveringIndicesRepository();
            Dictionary<IndexDefinition, PossibleIndex> createdIndices = new Dictionary<IndexDefinition, PossibleIndex>();
            using (var scope = new TransactionScope())
            {
                foreach (var kv in designData.RealExecutionPlansForStatements)
                {
                    var statementID = kv.Key;
                    var explainResult = kv.Value;
                    var createdPlan = Convert(statementID, explainResult);
                    executionPlansRepository.Create(createdPlan);
                    analysisRealStatementEvaluationsRepository.Create(Convert(context.WorkloadAnalysis.ID, statementID, createdPlan));
                }
                foreach (var env in designData.IndicesEnvironments)
                {
                    var createdEnvironment = Convert(env);
                    virtualEnvsRepository.Create(createdEnvironment);
                    foreach (var i in env.PossibleIndices.All)
                    {
                        if (!createdIndices.ContainsKey(i))
                        {
                            Dictionary<string, long> filters = null;
                            if (!designData.PossibleIndexFilters.TryGetValue(i, out filters))
                            {
                                filters = new Dictionary<string, long>();
                            }
                            var createdIndex = Convert(i, designData.PossibleIndexSizes[i], filters);
                            possibleIndicesRepository.Create(createdIndex);
                            createdIndices.Add(i, createdIndex);
                        }
                        virtualEnvPossibleIndicesRepository.Create(Convert(createdEnvironment, createdIndices[i]));
                    }
                    Dictionary<long, HashSet<long>> coveringIndicesPerStatement = new Dictionary<long, HashSet<long>>();
                    foreach (var kv in env.PossibleIndices.AllCoveringPerQuery)
                    {
                        var statementID = kv.Key.NormalizedStatementID;
                        var indices = kv.Value;
                        if (!coveringIndicesPerStatement.ContainsKey(statementID))
                        {
                            coveringIndicesPerStatement.Add(statementID, new HashSet<long>());
                        }
                        foreach (var index in indices)
                        {
                            var createdIndex = createdIndices[index];
                            if (!coveringIndicesPerStatement[statementID].Contains(createdIndex.ID))
                            {
                                virtualEnvPossibleCoveringIndicesRepository.Create(Convert(createdEnvironment, statementID, createdIndex));
                                coveringIndicesPerStatement[statementID].Add(createdIndex.ID);
                            }
                        }
                    }
                    foreach (var kv in env.PlansPerStatement)
                    {
                        var statementID = kv.Key;
                        var explainResult = kv.Value;
                        var createdPlan = Convert(statementID, explainResult);
                        executionPlansRepository.Create(createdPlan);
                        virtualEnvStatementEvalsRepository.Create(Convert(createdEnvironment, statementID, createdPlan));
                    }
                }
                scope.Complete();
            }
        }

        private VirtualEnvironmentPossibleCoveringIndex Convert(VirtualEnvironment env, long statementID, PossibleIndex createdIndex)
        {
            return new VirtualEnvironmentPossibleCoveringIndex() { VirtualEnvironmentID = env.ID, PossibleIndexID = createdIndex.ID, NormalizedStatementID = statementID };
        }

        private WorkloadAnalysisRealStatementEvaluation Convert(long workloadAnalysisID, long statementID, ExecutionPlan createdPlan)
        {
            return new WorkloadAnalysisRealStatementEvaluation() { ExecutionPlanID = createdPlan.ID, NormalizedStatementID = statementID, WorkloadAnalysisID = workloadAnalysisID };
        }

        private ExecutionPlan Convert(long statementID, DBMS.Contracts.IExplainResult explainResult)
        {
            return new ExecutionPlan() { Json = explainResult.PlanJson, TotalCost = explainResult.Plan.TotalCost };
        }

        private VirtualEnvironmentStatementEvaluation Convert(VirtualEnvironment env, long statementID, ExecutionPlan plan)
        {
            VirtualEnvironmentStatementEvaluation result = new VirtualEnvironmentStatementEvaluation();
            result.ExecutionPlanID = plan.ID;
            result.NormalizedStatementID = statementID;
            result.VirtualEnvironmentID = env.ID;
            return result;
        }

        private VirtualEnvironmentPossibleIndex Convert(VirtualEnvironment environment, PossibleIndex possibleIndex)
        {
            VirtualEnvironmentPossibleIndex result = new VirtualEnvironmentPossibleIndex();
            result.PossibleIndexID = possibleIndex.ID;
            result.VirtualEnvironemntID = environment.ID;
            return result;
        }

        private PossibleIndex Convert(IndexDefinition indexDefinition, long size, Dictionary<string, long> filters)
        {
            PossibleIndex result = new PossibleIndex();
            result.CreateDefinition = sqlCreateStatementGenerator.Generate(indexDefinition);
            result.FilterExpressions = new PossibleIndexFilterExpressionsData();
            foreach (var f in filters)
            {
                result.FilterExpressions.Expressions.Add(new PossibleIndexFilterExpression() { Expression = f.Key, Size = f.Value });
            }
            result.Name = indexDefinition.Name;
            result.Size = size;
            return result;
        }

        private VirtualEnvironment Convert(VirtualIndicesEnvironment env)
        {
            VirtualEnvironment result = new VirtualEnvironment();
            result.WorkloadAnalysisID = context.WorkloadAnalysis.ID;
            return result;
        }
    }
}
