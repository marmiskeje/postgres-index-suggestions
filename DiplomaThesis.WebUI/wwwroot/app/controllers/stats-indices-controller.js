Web.Controllers.StatsIndicesController = function ($scope, $rootScope, $http, uiGridConstants, $state, statisticsService, drawingService, notificationsService) {
    $rootScope.pageSubtitle = 'STATS_INDICES.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showStatementDetail = function (statementId) {
        $state.go(Web.Constants.StateNames.STATS_STATEMENT_DETAIL, { statementId: statementId, dateFrom: $scope.viewModel.dateFrom, dateTo: $scope.viewModel.dateTo });
    };
    $scope.gridStatsIndicesStatements = {
        paginationPageSize: 25,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Statement', displayName: 'Statement',
                cellTemplate: '<div class="ui-grid-cell-contents">' +
                    '  <a href="javascript:void(0)" ng-click="grid.appScope.actions.showStatementDetail(row.entity.normalizedStatementID)" >{{row.entity.normalizedStatement}}</a>' +
                    '</div>',
                field: 'normalizedStatement'
            },
            { name: 'IndexScansCount', displayName: 'Idx scans count', maxWidth: 180, field: 'indexScansCount' },
            { name: 'MinCost', displayName: 'Min idx scan cost', maxWidth: 180, field: 'minCost' },
            { name: 'MaxCost', displayName: 'Max idx scan cost', maxWidth: 180, field: 'maxCost' },
            { name: 'AvgCost', displayName: 'Avg idx scan cost', maxWidth: 180, field: 'avgCost' }
        ],
        onRegisterApi: function (gridApi){
            $scope.gridStatsIndicesStatementsApi = gridApi;
            $scope.gridStatsIndicesStatementsApi.core.queueRefresh();
            $scope.gridStatsIndicesStatementsApi.core.handleWindowResize();
        }
    };
    $scope.actions.drawChart = function (graphData) {
        var data = {
            labels: [],
            datasets: []
        };
        var indexScanCountDataset = {
            label: 'Index scans count',
            data: [],
            fill: false
        };
        var indexTupleReadCountDataset = {
            label: 'Index tuple read count',
            data: [],
            fill: false
        };
        var indexTupleFetchCountDataset = {
            label: 'Index tuple fetch count',
            data: [],
            fill: false
        };
        for (var i = 0; i < graphData.length; i++) {
            data.labels.push(graphData[i].date);
            indexScanCountDataset.data.push(graphData[i].indexScanCount);
            indexTupleReadCountDataset.data.push(graphData[i].indexTupleReadCount);
            indexTupleFetchCountDataset.data.push(graphData[i].indexTupleFetchCount);
        }
        data.datasets.push(indexScanCountDataset);
        data.datasets.push(indexTupleReadCountDataset);
        data.datasets.push(indexTupleFetchCountDataset);
        $scope.viewModel.graph = drawingService.drawTimeLineGraph('div-stats-indices', '', data);
    }
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.StatsIndexRequest();
            request.indexID = $scope.viewModel.currentIndex.id;
            request.filter.dateFrom = moment($scope.viewModel.dateFrom).format('YYYY-MM-DDTHH:mm:ss');
            request.filter.dateTo = moment($scope.viewModel.dateTo).format('YYYY-MM-DDTHH:mm:ss');
            statisticsService.getIndexStats(request, function (response) {
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
        $scope.gridStatsIndicesStatements.totalItems = 0;
        $scope.gridStatsIndicesStatements.data = [];
        $scope.gridStatsIndicesStatementsApi.core.queueRefresh();
        $scope.gridStatsIndicesStatementsApi.core.handleWindowResize();
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
                $scope.actions.drawChart(data.totalIndexStatistics);
                $scope.gridStatsIndicesStatements.data = data.indexSummaryStatementStatistics;
                $scope.gridStatsIndicesStatements.totalItems = $scope.gridStatsIndicesStatements.data.length;
                $scope.gridStatsIndicesStatementsApi.core.queueRefresh();
                $scope.gridStatsIndicesStatementsApi.core.handleWindowResize();
            }
        });
    }
    $scope.actions.updateAvailableRelations = function () {
        $scope.viewModel.allRelations = [];
        $scope.viewModel.currentRelation = null;
        if ($rootScope.viewModel.currentDatabase.id in $rootScope.viewModel.databaseRelations) {
            $scope.viewModel.allRelations = $rootScope.viewModel.databaseRelations[$rootScope.viewModel.currentDatabase.id];
            if ($scope.viewModel.allRelations.length > 0) {
                $scope.viewModel.currentRelation = $scope.viewModel.allRelations[0];
            }
        }
    }
    $scope.actions.onCurrentRelationChanged = function () {
        $scope.viewModel.allIndices = [];
        $scope.viewModel.currentIndex = null;
        if ($scope.viewModel.currentRelation != null && $scope.viewModel.currentRelation.id in $rootScope.viewModel.relationIndices) {
            $scope.viewModel.allIndices = $rootScope.viewModel.relationIndices[$scope.viewModel.currentRelation.id];
            if ($scope.viewModel.allIndices.length > 0) {
                $scope.viewModel.currentIndex = $scope.viewModel.allIndices[0];
            }
        }
        $scope.actions.refreshData(true);
    }
    $rootScope.$on('onDatabaseChanged', function () {
        $scope.actions.updateAvailableRelations();
        if ($state.is(Web.Constants.StateNames.STATS_INDICES)) {
            $scope.actions.refreshData(true);
        }
        else {
            $scope.actions.clearData();
        }
    })

    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.StatsIndicesViewModel();
        $scope.viewModel.dateFrom = moment().startOf('day');
        $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
        $scope.actions.updateAvailableRelations();
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
    $scope.actions.refreshData(false);
}



angular.module('WebApp').controller('StatsIndicesController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', 'statisticsService', 'drawingService', 'notificationsService', Web.Controllers.StatsIndicesController]);