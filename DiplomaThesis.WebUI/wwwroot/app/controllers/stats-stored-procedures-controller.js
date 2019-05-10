Web.Controllers.StatsStoredProceduresController = function ($scope, $rootScope, $http, uiGridConstants, $state, statisticsService, drawingService, notificationsService) {
    $rootScope.pageSubtitle = 'STATS_STORED_PROCEDURES.PAGE_SUBTITLE';
    $scope.actions.drawChart = function (graphData) {
        var data = {
            labels: [],
            datasets: []
        };
        var callsCountDataset = {
            label: 'Calls count',
            data: [],
            fill: false
        };
        var totalDurationDataset = {
            label: 'Total duration (ms)',
            data: [],
            fill: false
        };
        var selfDurationDataset = {
            label: 'Self duration (ms)',
            data: [],
            fill: false
        };
        for (var i = 0; i < graphData.length; i++) {
            data.labels.push(graphData[i].date);
            callsCountDataset.data.push(graphData[i].callsCount);
            totalDurationDataset.data.push(graphData[i].totalDurationInMs);
            selfDurationDataset.data.push(graphData[i].selfDurationInMs);
            //selfDurationDataset.data.push(moment.duration(graphData[i].selfDuration).asMilliseconds());
        }
        data.datasets.push(callsCountDataset);
        data.datasets.push(totalDurationDataset);
        data.datasets.push(selfDurationDataset);
        $scope.viewModel.graph = drawingService.drawTimeLineGraph('div-stats-stored-procedures', '', data);
    }
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.StatsStoredProcedureRequest();
            request.storedProcedureID = $scope.viewModel.currentStoredProcedure.id;
            request.filter.dateFrom = moment($scope.viewModel.dateFrom).format('YYYY-MM-DDTHH:mm:ss');
            request.filter.dateTo = moment($scope.viewModel.dateTo).format('YYYY-MM-DDTHH:mm:ss');
            statisticsService.getStoredProcedureStats(request, function (response) {
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
        if ($scope.viewModel.graph != null) {
            $scope.viewModel.graph.destroy();
            $scope.viewModel.graph = null;
        }
    }
    $scope.actions.refreshData = function (enforceLoading) {
        var loadData = new Promise(function (resolve, reject) {
            if ($scope.viewModel.validate()) {
                if (enforceLoading || $scope.viewModel.data == null) {
                    $scope.actions.loadData().then(function (result) {
                        resolve(result);
                    });
                }
                else {
                    resolve($scope.viewModel.data)
                }
            }
            else {
                resolve(null);
            }
        });
        loadData.then(function (data) {
            $scope.actions.clearData();
            if (data != null) {
                $scope.viewModel.data = data;
                $scope.actions.drawChart(data.totalStoredProcedureStatistics);
            }
        });
    }
    $scope.actions.updateAvailableProcedures = function () {
        $scope.viewModel.allStoredProcedures = [];
        $scope.viewModel.currentStoredProcedure = null;
        if ($rootScope.viewModel.currentDatabase.id in $rootScope.viewModel.databaseStoredProcedures) {
            $scope.viewModel.allStoredProcedures = $rootScope.viewModel.databaseStoredProcedures[$rootScope.viewModel.currentDatabase.id];
            if ($scope.viewModel.allStoredProcedures.length > 0) {
                $scope.viewModel.currentStoredProcedure = $scope.viewModel.allStoredProcedures[0];
            }
        }
    }
    $rootScope.$on('onDatabaseChanged', function () {
        $scope.actions.updateAvailableProcedures();
        if ($state.is(Web.Constants.StateNames.STATS_STORED_PROCEDURES)) {
            $scope.actions.refreshData(true);
        }
        else {
            $scope.actions.clearData();
        }
    })

    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.StatsStoredProceduresViewModel();
        $scope.viewModel.dateFrom = moment().startOf('day');
        $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
        $scope.actions.updateAvailableProcedures();
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
    $scope.actions.refreshData(false);
}

angular.module('WebApp').controller('StatsStoredProceduresController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', 'statisticsService', 'drawingService', 'notificationsService', Web.Controllers.StatsStoredProceduresController]);