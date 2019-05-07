Web.Controllers.StatsRelationsController = function ($scope, $rootScope, $http, uiGridConstants, $state, statisticsService, drawingService) {
    $rootScope.pageSubtitle = 'STATS_RELATIONS.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showStatementDetail = function (statementId) {
        $state.go(Web.Constants.StateNames.STATS_STATEMENT_DETAIL, { statementId: statementId, dateFrom: $scope.viewModel.dateFrom, dateTo: $scope.viewModel.dateTo });
    };
    var paginationOptions = {
        pageNumber: 1,
        pageSize: 25,
        sort: null
    };
    $scope.gridStatsRelationsStatements = {
        enablePaginationControls: false,
        paginationPageSize: paginationOptions.pageSize,
        useExternalPagination: true,
        useExternalSorting: true,
        columnDefs: [
            {
                name: 'Statement', displayName: 'Statement',
                cellTemplate: '<div class="ui-grid-cell-contents">' +
                    '  <a href="javascript:void(0)" ng-click="grid.appScope.actions.showStatementDetail(row.entity.Id)" >{{row.entity.Statement}}</a>' +
                    '</div>',
                field: 'Statement'
            },
            { name: 'SeqScansCount', displayName: 'Sequential scans count', maxWidth: 210, field: 'SeqScansCount' },
            { name: 'MinCost', displayName: 'Min seq scan cost', maxWidth: 180, field: 'MinCost' },
            { name: 'MaxCost', displayName: 'Max seq scan cost', maxWidth: 180, field: 'MaxCost' },
            { name: 'AvgCost', displayName: 'Avg seq scan cost', maxWidth: 180, field: 'AvgCost' }
        ],
        onRegisterApi: function (gridApi){
            $scope.gridApi = gridApi;
            $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if (sortColumns.length == 0) {
                    paginationOptions.sort = null;
                } else {
                    paginationOptions.sort = sortColumns[0].sort.direction;
                }
                getPage();
            });
            gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                paginationOptions.pageNumber = newPage;
                paginationOptions.pageSize = pageSize;
                getPage();
            });
        }
    };
    var getPage = function () {
        var data = [];
        for (var i = 0; i < 100; i++) {
            var toAdd = new Object();
            toAdd.Id = i;
            toAdd.Statement = "Select * from test" + i;
            toAdd.SeqScansCount = i;
            toAdd.MinCost = 1.2;
            toAdd.MaxCost = 5.6;
            toAdd.AvgCost = 3.1;
            data.push(toAdd);
        }
        $scope.gridStatsRelationsStatements.totalItems = 100;
        var firstRow = (paginationOptions.pageNumber - 1) * paginationOptions.pageSize;
        $scope.gridStatsRelationsStatements.data = data.slice(firstRow, firstRow + paginationOptions.pageSize);
    }
    getPage();
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
            label: 'Index tuple retch count',
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
        if ($scope.viewModel.graph != null) {
            $scope.viewModel.graph.destroy();
            $scope.viewModel.graph = null;
        }
        $scope.viewModel.graph = drawingService.drawTimeLineGraph('div-stats-relations', '', data);
    }
    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.StatsRelationsViewModel();
        $scope.viewModel.dateFrom = moment().startOf('day');
        $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
        if ($rootScope.viewModel.currentDatabase.id in $rootScope.viewModel.databaseRelations) {
            $scope.viewModel.allRelations = $rootScope.viewModel.databaseRelations[$rootScope.viewModel.currentDatabase.id];
            if ($scope.viewModel.allRelations.length > 0) {
                $scope.viewModel.currentRelation = $scope.viewModel.allRelations[0];
            }
        }
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
    $scope.viewModel.validate();
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.StatsRelationTotalRequest();
            request.relationID = $scope.viewModel.currentRelation.id;
            request.filter.dateFrom = $scope.viewModel.dateFrom;
            request.filter.dateTo = $scope.viewModel.dateTo;
            statisticsService.getRelationTotalStats(request, function (response) {
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
    $scope.actions.refreshData = function () {
        if ($scope.viewModel.validate()) {
            $scope.actions.loadData().then(function (data) {
                if (data != null) {
                    $scope.viewModel.graphData = data;
                    $scope.actions.drawChart(data);
                }
            });
        }
        else {
            if ($scope.viewModel.graph != null) {
                $scope.viewModel.graph.destroy();
            }
            $scope.gridStatsRelationsStatements.data = [];
            $scope.gridStatsRelationsStatements.totalItems = 0;
        }
    }
    $rootScope.$on('onDatabaseChanged', function () {
        $scope.viewModel.allRelations = [];
        $scope.viewModel.currentRelation = null;
        if ($rootScope.viewModel.currentDatabase.id in $rootScope.viewModel.databaseRelations) {
            $scope.viewModel.allRelations = $rootScope.viewModel.databaseRelations[$rootScope.viewModel.currentDatabase.id];
            if ($scope.viewModel.allRelations.length > 0) {
                $scope.viewModel.currentRelation = $scope.viewModel.allRelations[0];
            }
        }
        $scope.viewModel.validate();
        $scope.actions.refreshData();
    })
    $scope.actions.refreshData();
}



angular.module('WebApp').controller('StatsRelationsController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', 'statisticsService', 'drawingService', Web.Controllers.StatsRelationsController]);