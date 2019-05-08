Web.ViewModels.StatsOverviewViewModel = function () {
    this.isLoading = false;
    this.selectedPeriod = 1;
    this.data = null;
    this.dataLoadedDate = null;
    this.mostExecutedStatementsChart = null;
    this.slowestStatementsChart = null;
    this.mostAliveRelationsChart = null;
    this.lastSelectedDatabaseID = null;
}