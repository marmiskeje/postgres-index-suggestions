Web.Controllers.StatsRelationsController = function ($scope, $rootScope, $http, uiGridConstants, $state, statisticsService, drawingService, notificationsService) {
    $rootScope.pageSubtitle = 'STATS_RELATIONS.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showStatementDetail = function (statement) {
        $state.go(Web.Constants.StateNames.STATS_STATEMENT_DETAIL, { statement: statement, dateFrom: $scope.viewModel.dateFrom, dateTo: $scope.viewModel.dateTo });
    };
    $scope.gridStatsRelationsStatements = {
        paginationPageSize: 25,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Statement', displayName: 'Statement',
                cellTemplate: '<div class="ui-grid-cell-contents">' +
                    '  <a href="javascript:void(0)" ng-click="grid.appScope.actions.showStatementDetail(row.entity)" >{{row.entity.normalizedStatement}}</a>' +
                    '</div>',
                field: 'normalizedStatement'
            },
            { name: 'SeqScansCount', displayName: 'Seq scans count', maxWidth: 210, field: 'seqScansCount' },
            { name: 'MinCost', displayName: 'Min seq scan cost', maxWidth: 180, field: 'minCost' },
            { name: 'MaxCost', displayName: 'Max seq scan cost', maxWidth: 180, field: 'maxCost' },
            { name: 'AvgCost', displayName: 'Avg seq scan cost', maxWidth: 180, field: 'avgCost' }
        ],
        onRegisterApi: function (gridApi){
            $scope.gridStatsRelationsStatementsApi = gridApi;
            $scope.gridStatsRelationsStatementsApi.core.queueRefresh();
            $scope.gridStatsRelationsStatementsApi.core.handleWindowResize();
        }
    };
    $scope.actions.drawChart = function (graphData) {
        var data = {
            labels: [],
            datasets: []
        };
        var seqScanCountDataset = {
            label: 'Seq scans count',
            data: [],
            fill: false
        };
        var seqTupleReadCountDataset = {
            label: 'Seq tuple read count',
            data: [],
            fill: false
        };
        var indexScanCountDataset = {
            label: 'Index scans count',
            data: [],
            fill: false
        };
        var indexTupleFetchCountDataset = {
            label: 'Index tuple fetch count',
            data: [],
            fill: false
        };
        var tupleInsertedCountDataset = {
            label: '# Tuple inserted',
            data: [],
            fill: false
        };
        var tupleUpdatedCountDataset = {
            label: '# Tuple updated',
            data: [],
            fill: false
        };
        var tupleDeletedCountDataset = {
            label: '# Tuple deleted',
            data: [],
            fill: false
        };
        for (var i = 0; i < graphData.length; i++) {
            data.labels.push(graphData[i].date);
            seqScanCountDataset.data.push(graphData[i].seqScanCount);
            seqTupleReadCountDataset.data.push(graphData[i].seqTupleReadCount);
            indexScanCountDataset.data.push(graphData[i].indexScanCount);
            indexTupleFetchCountDataset.data.push(graphData[i].indexTupleFetchCount);
            tupleInsertedCountDataset.data.push(graphData[i].tupleInsertCount);
            tupleUpdatedCountDataset.data.push(graphData[i].tupleUpdateCount);
            tupleDeletedCountDataset.data.push(graphData[i].tupleDeleteCount);
        }
        data.datasets.push(seqScanCountDataset);
        data.datasets.push(seqTupleReadCountDataset);
        data.datasets.push(indexScanCountDataset);
        data.datasets.push(indexTupleFetchCountDataset);
        data.datasets.push(tupleInsertedCountDataset);
        data.datasets.push(tupleUpdatedCountDataset);
        data.datasets.push(tupleDeletedCountDataset);
        $scope.viewModel.graph = drawingService.drawTimeLineGraph('div-stats-relations', '', data);
    }
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.StatsRelationRequest();
            request.relationID = $scope.viewModel.currentRelation.id;
            request.filter.dateFrom = moment($scope.viewModel.dateFrom).format('YYYY-MM-DDTHH:mm:ss');
            request.filter.dateTo = moment($scope.viewModel.dateTo).format('YYYY-MM-DDTHH:mm:ss');
            statisticsService.getRelationStats(request, function (response) {
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
        $scope.gridStatsRelationsStatements.data = [];
        $scope.gridStatsRelationsStatementsApi.core.queueRefresh();
        $scope.gridStatsRelationsStatementsApi.core.handleWindowResize();
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
                $scope.actions.drawChart(data.totalRelationStatistics);
                $scope.gridStatsRelationsStatements.data = data.relationSummaryStatementStatistics;
                $scope.gridStatsRelationsStatementsApi.core.queueRefresh();
                $scope.gridStatsRelationsStatementsApi.core.handleWindowResize();
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
    $rootScope.$on('onDatabaseChanged', function () {
        $scope.actions.updateAvailableRelations();
        if ($state.is(Web.Constants.StateNames.STATS_RELATIONS)) {
            $scope.actions.refreshData(true);
        }
        else {
            $scope.actions.clearData();
        }
    })

    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.StatsRelationsViewModel();
        $scope.viewModel.dateFrom = moment().startOf('day').add(Web.Constants.Defaults.DATE_PERIOD_ADD_FROM_DAYS, 'days');
        $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
        $scope.actions.updateAvailableRelations();
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
    $scope.actions.refreshData(false);
}



angular.module('WebApp').controller('StatsRelationsController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', 'statisticsService', 'drawingService', 'notificationsService', Web.Controllers.StatsRelationsController]);