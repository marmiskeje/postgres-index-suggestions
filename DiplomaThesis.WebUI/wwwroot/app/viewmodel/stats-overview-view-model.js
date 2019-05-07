Web.ViewModels.StatsOverviewViewModel = function () {
    this.isLoading = true;
    this.selectedPeriod = 1;
    this.graphData = null;
    this.graphDataLoadedDate = null;
    this.mostExecutedStatementsChart = null;
    this.slowestStatementsChart = null;
    this.mostAliveRelationsChart = null;
}