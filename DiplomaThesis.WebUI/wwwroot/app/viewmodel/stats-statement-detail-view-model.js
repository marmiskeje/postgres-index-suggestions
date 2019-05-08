Web.ViewModels.StatsStatementDetailViewModel = function () {
    this.isLoading = false;
    this.isValid = false;
    this.dateFrom = null;
    this.dateTo = null;
    this.data = null;
    this.statement = null;
    this.graphExecTimeline = null;
    this.graphDurationTimeline = null;
    this.graphRelationsUsage = null;
    this.graphIndicesUsage = null;
    this.selectedRelationsUsage = 'totalScansCount';
    this.selectedIndicesUsage = 'totalIndexScanCount';
    
    this.validate = function () {
        this.isValid = this.dateFrom != null && this.dateTo != null;
        return this.isValid;
    }
}