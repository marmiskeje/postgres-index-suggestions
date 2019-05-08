Web.Controllers.StatsOverviewController = function ($scope, $rootScope, $state, statisticsService, notificationsService, drawingService) {
    $rootScope.pageSubtitle = 'STATS_OVERVIEW.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.drawMostExecutedStatementsChart = function (graphData) {
        var data = {
            labels: [],
            datasets: []
        };
        var dataset = {
            label: 'Execution count',
            data: [],
            fill: false,
            backgroundColor: ["#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8"]
        };
        for (var i = 0; i < graphData.length; i++) {
            data.labels.push(graphData[i].independentValue.substring(0, 100));
            dataset.data.push(graphData[i].dependentValue);
        }
        data.datasets.push(dataset);
        $scope.viewModel.mostExecutedStatementsChart = drawingService.drawBarGraph('div-stats-most-executed-statements', 'TOP 10 most frequently executed statements', data);
    }
    $scope.actions.drawSlowestStatementsChart = function (graphData) {
        var data = {
            labels: [],
            datasets: []
        };
        var dataset = {
            label: 'Max duration (s)',
            data: [],
            fill: false,
            backgroundColor: ["#7075bf", "#7075bf", "#7075bf", "#7075bf", "#7075bf", "#7075bf", "#7075bf", "#7075bf", "#7075bf", "#7075bf", "#7075bf"]
        };
        for (var i = 0; i < graphData.length; i++) {
            data.labels.push(graphData[i].independentValue.substring(0, 100));
            dataset.data.push(graphData[i].dependentValue / 1000);
        }
        data.datasets.push(dataset);
        $scope.viewModel.slowestStatementsChart = drawingService.drawBarGraph('div-stats-slowest-statements', 'TOP 10 slowest statements', data);
    }
    $scope.actions.drawMostAliveRelationsChart = function (graphData) {
        var data = {
            labels: [],
            datasets: []
        };
        var dataset = {
            label: 'Liveness index',
            data: [],
            fill: false,
            backgroundColor: ["#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8"]
        };
        for (var i = 0; i < graphData.length; i++) {
            data.labels.push(graphData[i].independentValue.substring(0, 100));
            dataset.data.push(graphData[i].dependentValue);
        }
        data.datasets.push(dataset);
        $scope.viewModel.mostAliveRelationsChart = drawingService.drawBarGraph('div-stats-most-alive-relations', 'TOP 10 alive relations', data);
    }
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.StatsOverviewRequest();
            request.databaseID = $rootScope.viewModel.currentDatabase.id;
            var dateTo = moment();
            var dateFrom = moment(dateTo).add(-1 * $scope.viewModel.selectedPeriod, 'hours');
            request.filter.dateFrom = dateFrom;
            request.filter.dateTo = dateTo;
            statisticsService.getOverview(request, function (response) {
                $scope.viewModel.isLoading = false;
                if (response.data == null) {
                    notificationsService.showError("An error occured during data loading.", response.status, response.statusText);
                    resolve(null);
                }
                else if (response.data.isSuccess && response.data.data != null) {
                    var result = response.data.data;
                    resolve(result);
                }
                else {
                    notificationsService.showError("An error occured during data loading.", 500, response.data.errorMessage);
                    resolve(null);
                }
            });
        });
    };
    $scope.actions.clearData = function () {
        $scope.viewModel.data = null;
        if ($scope.viewModel.mostAliveRelationsChart != null) {
            $scope.viewModel.mostAliveRelationsChart.destroy();
            $scope.viewModel.mostAliveRelationsChart = null;
        }
        if ($scope.viewModel.slowestStatementsChart != null) {
            $scope.viewModel.slowestStatementsChart.destroy();
            $scope.viewModel.slowestStatementsChart = null;
        }
        if ($scope.viewModel.mostExecutedStatementsChart != null) {
            $scope.viewModel.mostExecutedStatementsChart.destroy();
            $scope.viewModel.mostExecutedStatementsChart = null;
        }
    }
    $scope.actions.refreshData = function (loadingEnforced) {
        var loadData = new Promise(function (resolve, reject) {
            if (loadingEnforced || $scope.viewModel.data == null || moment.duration(moment().diff($scope.viewModel.dataLoadedDate)).asMinutes() > 1) {
                $scope.actions.loadData().then(function (result) {
                    resolve(result);
                });
            }
            else {
                resolve($scope.viewModel.data);
            }
        });
        loadData.then(function (data) {
            $scope.actions.clearData();
            if (data != null) {
                $scope.viewModel.data = data;
                $scope.viewModel.dataLoadedDate = moment();
                $scope.actions.drawMostExecutedStatementsChart(data.mostExecutedStatements);
                $scope.actions.drawSlowestStatementsChart(data.mostSlowestStatements);
                $scope.actions.drawMostAliveRelationsChart(data.mostAliveRelations);
            }
        });
    }
    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.StatsOverviewViewModel();
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
    $scope.actions.refreshData(false);
    $rootScope.$on('onDatabaseChanged', function () {
        if ($state.is(Web.Constants.StateNames.STATS_OVERVIEW)) {
            $scope.actions.refreshData(true);
        }
        else {
            $scope.actions.clearData();
        }
    })
}

angular.module('WebApp').controller('StatsOverviewController', ['$scope', '$rootScope', '$state', 'statisticsService', 'notificationsService', 'drawingService', Web.Controllers.StatsOverviewController]);