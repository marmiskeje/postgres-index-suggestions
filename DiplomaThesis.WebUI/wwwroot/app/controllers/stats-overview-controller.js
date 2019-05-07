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
        if ($scope.viewModel.mostExecutedStatementsChart != null) {
            $scope.viewModel.mostExecutedStatementsChart.destroy();
            $scope.viewModel.mostExecutedStatementsChart = null;
        }
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
        if ($scope.viewModel.slowestStatementsChart != null) {
            $scope.viewModel.slowestStatementsChart.destroy();
            $scope.viewModel.slowestStatementsChart = null;
        }
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
        if ($scope.viewModel.mostAliveRelationsChart != null) {
            $scope.viewModel.mostAliveRelationsChart.destroy();
            $scope.viewModel.mostAliveRelationsChart = null;
        }
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
                    notificationsService.showError("An error occured during data loading.", errorResponse.status, errorResponse.statusText);
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
    $scope.actions.refreshData = function (loadingEnforced) {
        var loadData = new Promise(function (resolve, reject) {
            if (loadingEnforced || $scope.viewModel.graphData == null || moment.duration(moment().diff($scope.viewModel.graphDataLoadedDate)).asMinutes() > 1) {
                $scope.actions.loadData().then(function (result) {
                    resolve(result);
                });
            }
            else {
                resolve($scope.viewModel.graphData);
            }
        });
        loadData.then(function (data) {
            if (data != null) {
                $scope.viewModel.graphData = data;
                $scope.viewModel.graphDataLoadedDate = moment();
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
        $scope.actions.refreshData(true);
    })
}

angular.module('WebApp').controller('StatsOverviewController', ['$scope', '$rootScope', '$state', 'statisticsService', 'notificationsService', 'drawingService', Web.Controllers.StatsOverviewController]);