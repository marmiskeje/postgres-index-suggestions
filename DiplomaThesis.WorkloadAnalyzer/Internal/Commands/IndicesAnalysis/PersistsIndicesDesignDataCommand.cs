using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class PersistsIndicesDesignDataCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IRepositoriesFactory dalRepositories;
        private readonly DBMS.Contracts.IDbObjectDefinitionGenerator dbObjectDefinitionGenerator;
        public PersistsIndicesDesignDataCommand(WorkloadAnalysisContext context, IRepositoriesFactory dalRepositories, DBMS.Contracts.IDbObjectDefinitionGenerator dbObjectDefinitionGenerator)
        {
            this.context = context;
            this.dalRepositories = dalRepositories;
            this.dbObjectDefinitionGenerator = dbObjectDefinitionGenerator;
        }
        protected override void OnExecute()
        {
            var designData = context.IndicesDesignData;
            var virtualEnvsRepository = dalRepositories.GetVirtualEnvironmentsRepository();
            var possibleIndicesRepository = dalRepositories.GetPossibleIndicesRepository();
            var virtualEnvPossibleIndicesRepository = dalRepositories.GetVirtualEnvironmentPossibleIndicesRepository();
            var virtualEnvStatementEvalsRepository = dalRepositories.GetVirtualEnvironmentStatementEvaluationsRepository();
            var executionPlansRepository = dalRepositories.GetExecutionPlansRepository();
            var virtualEnvPossibleCoveringIndicesRepository = dalRepositories.GetVirtualEnvironmentPossibleCoveringIndicesRepository();
            Dictionary<DBMS.Contracts.IndexDefinition, PossibleIndex> createdIndices = new Dictionary<DBMS.Contracts.IndexDefinition, PossibleIndex>();
            using (var scope = new TransactionScope())
            {
                foreach (var env in designData.Environments)
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
                        virtualEnvPossibleIndicesRepository.Create(Convert(createdEnvironment, createdIndices[i], env.IndicesEvaluation[i]));
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
                    foreach (var kv in env.StatementsEvaluation)
                    {
                        var statementID = kv.Key;
                        var eval = kv.Value;
                        var createdPlan = Convert(statementID, eval.ExecutionPlan);
                        executionPlansRepository.Create(createdPlan);
                        virtualEnvStatementEvalsRepository.Create(Convert(createdEnvironment, statementID, createdPlan, eval));
                    }
                }
                scope.Complete();
            }
        }

        private VirtualEnvironmentPossibleCoveringIndex Convert(VirtualEnvironment env, long statementID, PossibleIndex createdIndex)
        {
            return new VirtualEnvironmentPossibleCoveringIndex()
            {
                VirtualEnvironmentID = env.ID,
                PossibleIndexID = createdIndex.ID,
                NormalizedStatementID = statementID,
            };
        }

        private ExecutionPlan Convert(long statementID, DBMS.Contracts.IExplainResult explainResult)
        {
            return new ExecutionPlan() { Json = explainResult.PlanJson, TotalCost = explainResult.Plan.TotalCost };
        }

        private DAL.Contracts.VirtualEnvironmentStatementEvaluation Convert(VirtualEnvironment env, long statementID, ExecutionPlan plan, VirtualEnvironmentStatementEvaluation eval)
        {
            DAL.Contracts.VirtualEnvironmentStatementEvaluation result = new DAL.Contracts.VirtualEnvironmentStatementEvaluation();
            result.ExecutionPlanID = plan.ID;
            result.NormalizedStatementID = statementID;
            result.VirtualEnvironmentID = env.ID;
            result.GlobalImprovementRatio = eval.GlobalImprovementRatio;
            result.LocalImprovementRatio = eval.LocalImprovementRatio;
            return result;
        }

        private VirtualEnvironmentPossibleIndex Convert(VirtualEnvironment environment, PossibleIndex possibleIndex, VirtualIndicesEnvironmentIndexEvaluation indexEval)
        {
            VirtualEnvironmentPossibleIndex result = new VirtualEnvironmentPossibleIndex();
            result.PossibleIndexID = possibleIndex.ID;
            result.VirtualEnvironemntID = environment.ID;
            result.ImprovementRatio = indexEval.ImprovementRatio;
            return result;
        }

        private PossibleIndex Convert(DBMS.Contracts.IndexDefinition indexDefinition, long size, Dictionary<string, long> filters)
        {
            PossibleIndex result = new PossibleIndex();
            result.CreateDefinition = dbObjectDefinitionGenerator.Generate(indexDefinition).CreateStatement;
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
            result.Type = VirtualEnvironmentType.Indices;
            return result;
        }
    }
}
