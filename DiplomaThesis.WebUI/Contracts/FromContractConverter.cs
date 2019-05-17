using DiplomaThesis.DAL.Contracts;
using System;
using DiplomaThesis.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DiplomaThesis.WebUI
{
    public partial class ContractConverter
    {
        internal WorkloadAnalysis CreateWorkloadAnalysis(CreateWorkloadAnalysisRequest source)
        {
            WorkloadAnalysis result = new WorkloadAnalysis();
            result.CreatedDate = DateTime.Now;
            result.PeriodFromDate = source.PeriodFromDate;
            result.PeriodToDate = source.PeriodToDate;
            result.RelationReplacements = new List<WorkloadAnalysisRelationReplacement>();
            if (source.RelationReplacements != null)
            {
                foreach (var kv in source.RelationReplacements)
                {
                    result.RelationReplacements.Add(new WorkloadAnalysisRelationReplacement() { SourceId = kv.Key, TargetId = kv.Value });
                }
            }
            result.State = WorkloadAnalysisStateType.Created;
            result.WorkloadID = source.WorkloadID;
            return result;
        }
        internal Workload CreateWorkload(WorkloadData source)
        {
            Workload result = new Workload();
            result.CreatedDate = DateTime.Now;
            result.DatabaseID = source.DatabaseID;
            result.Definition = new WorkloadDefinition();
            result.Definition.Applications = new WorkloadPropertyValuesDefinition<string>();
            result.Definition.DateTimeSlots = new WorkloadPropertyValuesDefinition<WorkloadDateTimeSlot>();
            result.Definition.QueryThresholds = new WorkloadQueryThresholds();
            result.Definition.Relations = new WorkloadPropertyValuesDefinition<uint>();
            result.Definition.Users = new WorkloadPropertyValuesDefinition<string>();
            if (source.Definition != null)
            {
                if (source.Definition.ForbiddenApplications != null)
                {
                    result.Definition.Applications.ForbiddenValues.AddRange(source.Definition.ForbiddenApplications);
                }
                if (source.Definition.ForbiddenDateTimeSlots != null)
                {
                    foreach (var slot in source.Definition.ForbiddenDateTimeSlots)
                    {
                        result.Definition.DateTimeSlots.ForbiddenValues.Add(Convert(slot));
                    }
                }
                if (source.Definition.ForbiddenRelations != null)
                {
                    result.Definition.Relations.ForbiddenValues.AddRange(source.Definition.ForbiddenRelations);
                }
                if (source.Definition.ForbiddenUsers != null)
                {
                    result.Definition.Users.ForbiddenValues.AddRange(source.Definition.ForbiddenUsers);
                }
                result.Definition.QueryThresholds.MinDuration = TimeSpan.FromMilliseconds(source.Definition.StatementMinDurationInMs);
                result.Definition.QueryThresholds.MinExectutionCount = source.Definition.StatementMinExectutionCount;
            }
            result.Name = source.Name;
            return result;
        }

        internal WorkloadAnalysisDetailForEnvData ConvertToWorkloadAnalysisDetailForEnv(VirtualEnvironment env, IEnumerable<WorkloadAnalysisRealStatementEvaluation> statements)
        {
            WorkloadAnalysisDetailForEnvData result = new WorkloadAnalysisDetailForEnvData();
            result.EnvironmentID = env.ID;
            foreach (var item in statements)
            {
                result.Statements.Add(item.NormalizedStatementID, Convert(item));
            }
            Dictionary<long, HashSet<long>> coveringIndicesPerStatement = new Dictionary<long, HashSet<long>>();
            foreach (var item in env.VirtualEnvironmentPossibleCoveringIndices)
            {
                if (!coveringIndicesPerStatement.ContainsKey(item.NormalizedStatementID))
                {
                    coveringIndicesPerStatement.Add(item.NormalizedStatementID, new HashSet<long>());
                }
                coveringIndicesPerStatement[item.NormalizedStatementID].Add(item.PossibleIndexID);
            }
            foreach (var item in env.VirtualEnvironmentStatementEvaluations)
            {
                result.EvaluatedStatements.Add(item.NormalizedStatementID, Convert(item, coveringIndicesPerStatement));
            }
            return result;
        }

        private WorkloadAnalysisDetailForEnvStatementEvaluationData Convert(VirtualEnvironmentStatementEvaluation source, Dictionary<long, HashSet<long>> coveringIndicesPerStatement)
        {
            WorkloadAnalysisDetailForEnvStatementEvaluationData result = new WorkloadAnalysisDetailForEnvStatementEvaluationData();
            if (source.AffectingIndices != null)
            {
                result.AffectingIndices.AddRange(source.AffectingIndices);
            }
            if (coveringIndicesPerStatement.ContainsKey(source.NormalizedStatementID))
            {
                result.CoveringIndices.AddRange(coveringIndicesPerStatement[source.NormalizedStatementID]);
            }
            result.GlobalImprovementRatio = source.GlobalImprovementRatio;
            result.LocalImprovementRatio = source.LocalImprovementRatio;
            result.TotalCost = source.ExecutionPlan.TotalCost;
            if (source.UsedIndices != null)
            {
                result.UsedIndices.AddRange(source.UsedIndices);
            }
            return result;
        }

        private WorkloadAnalysisDetailForEnvStatementData Convert(WorkloadAnalysisRealStatementEvaluation source)
        {
            WorkloadAnalysisDetailForEnvStatementData result = new WorkloadAnalysisDetailForEnvStatementData();
            result.Statement = source.NormalizedStatement.Statement;
            result.TotalCost = source.ExecutionPlan.TotalCost;
            result.TotalExecutionsCount = source.TotalExecutionsCount;
            return result;
        }

        internal WorkloadAnalysisDetailData ConvertToWorkloadAnalysisDetail(IEnumerable<VirtualEnvironment> indicesEnvs, IEnumerable<VirtualEnvironment> hPartsEnvs, IEnumerable<PossibleIndex> indices)
        {
            WorkloadAnalysisDetailData result = new WorkloadAnalysisDetailData();
            foreach (var e in indicesEnvs)
            {
                result.IndicesEnvironments.Add(e.ID, Convert(e));
            }
            foreach (var e in hPartsEnvs)
            {
                result.HPartitioningsEnvironments.Add(e.ID, Convert(e));
            }
            foreach (var item in indices)
            {
                result.Indices.Add(item.ID, Convert(item));
            }
            return result;
        }

        private WorkloadAnalysisEnvironmentIndexExtended Convert(PossibleIndex source)
        {
            WorkloadAnalysisEnvironmentIndexExtended result = new WorkloadAnalysisEnvironmentIndexExtended();
            result.CreateDefinition = source.CreateDefinition;
            if (source.FilterExpressions != null && source.FilterExpressions.Expressions != null)
            {
                foreach (var item in source.FilterExpressions.Expressions)
                {
                    result.Filters.Add(item.Expression, item.Size);
                }
            }
            result.ID = source.ID;
            result.Name = source.Name;
            result.RelationID = source.RelationID;
            result.Size = source.Size;
            return result;
        }

        private WorkloadAnalysisEnvironment Convert(VirtualEnvironment source)
        {
            WorkloadAnalysisEnvironment result = new WorkloadAnalysisEnvironment();
            result.ID = source.ID;
            if (source.VirtualEnvironmentPossibleHPartitionings != null)
            {
                foreach (var item in source.VirtualEnvironmentPossibleHPartitionings)
                {
                    result.HPartitionings.Add(item.ID, Convert(item));
                }
            }
            if (source.VirtualEnvironmentPossibleIndices != null)
            {
                foreach (var item in source.VirtualEnvironmentPossibleIndices)
                {
                    result.Indices.Add(item.PossibleIndexID, new WorkloadAnalysisEnvironmentIndex() { ID = item.PossibleIndexID, ImprovementRatio = item.ImprovementRatio });
                }
            }
            return result;
        }

        private WorkloadAnalysisEnvironmentHPartitioning Convert(VirtualEnvironmentPossibleHPartitioning source)
        {
            WorkloadAnalysisEnvironmentHPartitioning result = new WorkloadAnalysisEnvironmentHPartitioning();
            result.ID = source.ID;
            result.ImprovementRatio = source.ImprovementRatio;
            result.PartitioningStatement = source.PartitioningStatement;
            result.PartitionStatements.AddRange(source.PartitionStatements);
            result.RelationID = source.RelationID;
            return result;
        }

        internal WorkloadAnalysisData Convert(WorkloadAnalysis source)
        {
            WorkloadAnalysisData result = new WorkloadAnalysisData();
            result.CreatedDate = source.CreatedDate;
            result.EndDate = source.EndDate;
            result.ErrorMessage = source.ErrorMessage;
            result.ID = source.ID;
            result.PeriodFromDate = source.PeriodFromDate;
            result.PeriodToDate = source.PeriodToDate;
            result.StartDate = source.StartDate;
            result.State = (int)source.State;
            result.Workload = Convert(source.Workload);
            return result;
        }

        internal ConfigurationCollectorData Convert(CollectorConfiguration source)
        {
            ConfigurationCollectorData result = new ConfigurationCollectorData();
            if (source != null)
            {
                if (source.Databases != null)
                {
                    foreach (var d in source.Databases.Values)
                    {
                        result.Databases.Add(Convert(d));
                    }
                }
            }
            return result;
        }

        private ConfigurationCollectorDatabaseData Convert(CollectorDatabaseConfiguration source)
        {
            ConfigurationCollectorDatabaseData result = new ConfigurationCollectorDatabaseData();
            result.DatabaseID = source.DatabaseID;
            result.IsEnabledGeneralCollection = source.IsEnabledGeneralCollection;
            result.IsEnabledStatementCollection = source.IsEnabledStatementCollection;
            return result;
        }

        internal ConfigurationReportsData Convert(ReportingSettings source)
        {
            ConfigurationReportsData result = new ConfigurationReportsData();
            if (source != null)
            {
                result.EmailAddresses = String.Join(", ", source.Recipients);
            }
            return result;
        }

        internal ConfigurationSmtpData Convert(SmtpConfiguration source)
        {
            ConfigurationSmtpData result = new ConfigurationSmtpData();
            if (source != null)
            {
                result.Host = source.SmtpHost;
                result.Password = null;
                result.Port = source.SmtpPort;
                result.Username = source.SmtpUsername;
            }
            return result;
        }

        private WorkloadDateTimeSlot Convert(WorkloadDateTimeSlotData source)
        {
            WorkloadDateTimeSlot result = new WorkloadDateTimeSlot();
            result.DayOfWeek = source.DayOfWeek;
            result.EndTime = source.EndTime;
            result.StartTime = source.StartTime;
            return result;
        }
    }
}
