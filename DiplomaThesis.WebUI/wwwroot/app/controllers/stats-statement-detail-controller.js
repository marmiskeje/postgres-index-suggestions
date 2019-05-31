Web.Controllers.StatsStatementDetailController = function ($scope, $rootScope, $http, uiGridConstants, $state, statisticsService, drawingService, notificationsService, $window, $stateParams) {
    $rootScope.pageSubtitle = 'STATS_RELATIONS.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
    $scope.gridStatsSlowestStatements = {
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Statement', displayName: 'Statement', field: 'representativeStatement'
            },
            {
                name: 'Username', displayName: 'Username', field: 'userName'
            },
            {
                name: 'Application name', displayName: 'Application name', field: 'applicationName'
            },
            {
                name: 'Date', displayName: 'Date', field: 'date'
            },
            { name: 'Duration', displayName: 'Duration', field: 'maxDuration' },
            { name: 'Cost', displayName: 'Cost', field: 'maxTotalCost' },
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridStatsSlowestStatementsApi = gridApi;
        }
    };
    $scope.actions.drahGraphExecTimeline = function (graphData) {
        var data = {
            labels: [],
            datasets: []
        };
        var executionsCountDataset = {
            label: 'Executions count',
            data: [],
            fill: false
        };
        for (var i = 0; i < graphData.length; i++) {
            data.labels.push(graphData[i].date);
            executionsCountDataset.data.push(graphData[i].totalExecutionsCount);
        }
        data.datasets.push(executionsCountDataset);
        $scope.viewModel.graph = drawingService.drawTimeLineGraph('div-stats-statement-exec-timeline', '', data);
    }
    $scope.actions.drawGraphDurationTimeline = function (graphData) {
        var data = {
            labels: [],
            datasets: []
        };
        var minDurationDataset = {
            label: 'Min duration (ms)',
            data: [],
            fill: false
        };
        var maxDurationDataset = {
            label: 'Max duration (ms)',
            data: [],
            fill: false
        };
        var avgDurationDataset = {
            label: 'Avg duration (ms)',
            data: [],
            fill: false
        };
        for (var i = 0; i < graphData.length; i++) {
            data.labels.push(graphData[i].date);
            minDurationDataset.data.push(moment.duration(graphData[i].minDuration).asMilliseconds());
            maxDurationDataset.data.push(moment.duration(graphData[i].maxDuration).asMilliseconds());
            avgDurationDataset.data.push(moment.duration(graphData[i].avgDuration).asMilliseconds());
        }
        data.datasets.push(minDurationDataset);
        data.datasets.push(maxDurationDataset);
        data.datasets.push(avgDurationDataset);
        $scope.viewModel.graph = drawingService.drawTimeLineGraph('div-stats-statement-duration-timeline', '', data);
    }
    $scope.actions.drawGraphRelationsUsage = function (graphData) {
        var data = {
            datasets: []
        };
        var datasets = {};
        for (var i = 0; i < graphData.length; i++) {
            var item = graphData[i];
            if (!(item.relationID in datasets)) {
                datasets[item.relationID] = {
                    label: (item.relationID in $rootScope.viewModel.allRelations ? $rootScope.viewModel.allRelations[item.relationID].fullName : item.relationID),
                    data: [],
                    fill: false
                }
            }
            datasets[item.relationID].data.push({ x: item.date, y: item[$scope.viewModel.selectedRelationsUsage] });
        }
        for (var key in datasets) {
            data.datasets.push(datasets[key]);
        }
        if ($scope.viewModel.graphRelationsUsage != null) {
            $scope.viewModel.graphRelationsUsage.destroy();
            $scope.viewModel.graphRelationsUsage = null;
        }
        $scope.viewModel.graphDurationTimeline = drawingService.drawTimeLineGraph('div-stats-statement-relations', '', data);
    }
    $scope.actions.drawGraphIndicesUsage = function (graphData) {
        var data = {
            datasets: []
        };
        var datasets = {};
        for (var i = 0; i < graphData.length; i++) {
            var item = graphData[i];
            if (!(item.indexID in datasets)) {
                datasets[item.indexID] = {
                    label: (item.indexID in $rootScope.viewModel.allIndices ? $rootScope.viewModel.allIndices[item.indexID].fullName : item.indexID),
                    data: [],
                    fill: false
                }
            }
            datasets[item.indexID].data.push({ x: item.date, y: item[$scope.viewModel.selectedIndicesUsage] });
        }
        for (var key in datasets) {
            data.datasets.push(datasets[key]);
        }
        if ($scope.viewModel.graphIndicesUsage != null) {
            $scope.viewModel.graphIndicesUsage.destroy();
            $scope.viewModel.graphIndicesUsage = null;
        }
        $scope.viewModel.graphIndicesUsage = drawingService.drawTimeLineGraph('div-stats-statement-indices', '', data);
    }
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.StatsStatementRequest();
            request.statementID = $scope.viewModel.statement.normalizedStatementID;
            request.filter.dateFrom = moment($scope.viewModel.dateFrom).format('YYYY-MM-DDTHH:mm:ss');
            request.filter.dateTo = moment($scope.viewModel.dateTo).format('YYYY-MM-DDTHH:mm:ss');
            statisticsService.getStatementStats(request, function (response) {
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
        if ($scope.viewModel.graphExecTimeline != null) {
            $scope.viewModel.graphExecTimeline.destroy();
            $scope.viewModel.graphExecTimeline = null;
        }
        if ($scope.viewModel.graphDurationTimeline != null) {
            $scope.viewModel.graphDurationTimeline.destroy();
            $scope.viewModel.graphDurationTimeline = null;
        }
        if ($scope.viewModel.graphRelationsUsage != null) {
            $scope.viewModel.graphRelationsUsage.destroy();
            $scope.viewModel.graphRelationsUsage = null;
        }
        if ($scope.viewModel.graphIndicesUsage != null) {
            $scope.viewModel.graphIndicesUsage.destroy();
            $scope.viewModel.graphIndicesUsage = null;
        }
        $scope.gridStatsSlowestStatements.data = [];
        $scope.gridStatsSlowestStatementsApi.core.queueRefresh();
        $scope.gridStatsSlowestStatementsApi.core.handleWindowResize();
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
                $scope.actions.drahGraphExecTimeline(data.statementTimeline);
                $scope.actions.drawGraphDurationTimeline(data.statementTimeline);
                $scope.actions.drawGraphRelationsUsage(data.statementRelationStatistics);
                $scope.actions.drawGraphIndicesUsage(data.statementIndexStatistics);
                $scope.gridStatsSlowestStatements.data = data.slowestRepresentatives;
                $scope.gridStatsSlowestStatementsApi.core.queueRefresh();
                $scope.gridStatsSlowestStatementsApi.core.handleWindowResize();
            }
        });
    }

    $scope.viewModel = new Web.ViewModels.StatsStatementDetailViewModel();
    if ($stateParams.dateFrom && $stateParams.dateTo) {
        $scope.viewModel.statement = $stateParams.statement;
        $scope.viewModel.dateFrom = $stateParams.dateFrom;
        $scope.viewModel.dateTo = $stateParams.dateTo;
    }
    else {
        $scope.viewModel.dateFrom = moment().startOf('day').add(Web.Constants.Defaults.DATE_PERIOD_ADD_FROM_DAYS, 'days');
        $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
    }
    $scope.actions.refreshData(true);
}



angular.module('WebApp').controller('StatsStatementDetailController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', 'statisticsService', 'drawingService', 'notificationsService', '$window', '$stateParams', Web.Controllers.StatsStatementDetailController]);