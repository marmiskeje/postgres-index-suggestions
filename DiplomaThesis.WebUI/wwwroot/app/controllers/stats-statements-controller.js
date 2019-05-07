Web.Controllers.StatsStatementsController = function ($scope, $rootScope, $http, uiGridConstants, $state) {
    $rootScope.pageSubtitle = 'STATS_STATEMENTS.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showStatementDetail = function (statementId) {
        $state.go(Web.Constants.StateNames.STATS_STATEMENT_DETAIL, { statementId: statementId, dateFrom: $scope.viewModel.dateFrom, dateTo: $scope.viewModel.dateTo });
    };
    $scope.viewModel = new Object();
    $scope.viewModel.isValid = true;
    $scope.viewModel.isLoading = false;
    $scope.viewModel.allCommandTypeFilters = [];
    var filter = new Object();
    filter.id = 0;
    filter.name = "Any";
    $scope.viewModel.allCommandTypeFilters.push(filter);
    $scope.viewModel.selectedCommandTypeFilter = filter;
    $scope.viewModel.dateFrom = moment().startOf('day');
    $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
    var paginationOptions = {
        pageNumber: 1,
        pageSize: 25,
        sort: null
    };
    $scope.gridStatsStatements = {
        enablePaginationControls: false,
        paginationPageSize: paginationOptions.pageSize,
        useExternalPagination: true,
        useExternalSorting: true,
        columnDefs: [
            { name: 'Command type', maxWidth: 150, field: 'CommandType' },
            {
                name: 'Statement',
                cellTemplate: '<div class="ui-grid-cell-contents">' +
                    '  <a href="javascript:void(0)" ng-click="grid.appScope.actions.showStatementDetail(row.entity.Id)" >{{row.entity.Statement}}</a>' +
                    '</div>',
                field: 'Statement'
            },
            { displayName: 'Total executions count', maxWidth: 220, name: 'TotalExecutionsCount', field: 'TotalExecutionsCount' },
            { displayName: 'Min duration (s)', maxWidth: 170, field: 'MinDuration', name: 'MinDuration' },
            { displayName: 'Max duration (s)', maxWidth: 170, field: 'MaxDuration', name: 'MaxDuration' },
            { displayName: 'Avg duration (s)', maxWidth: 170, field: 'AvgDuration', name: 'AvgDuration' },
            { displayName: 'Min cost', maxWidth: 100, field: 'MinCost', name: 'MinCost' },
            { displayName: 'Max cost', maxWidth: 100, field: 'MaxCost', name: 'MaxCost' },
            { displayName: 'Avg cost', maxWidth: 100, field: 'AvgCost', name: 'AvgCost' }
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
            toAdd.CommandType = "SELECT";
            toAdd.Statement = "Select * from test" + i;
            toAdd.TotalExecutionsCount = i;
            toAdd.MinDuration = 1.2;
            toAdd.MaxDuration = 5.6;
            toAdd.AvgDuration = 3.1;
            toAdd.MinCost = 1.2;
            toAdd.MaxCost = 5.6;
            toAdd.AvgCost = 3.1;
            data.push(toAdd);
        }
        $scope.gridStatsStatements.totalItems = 100;
        var firstRow = (paginationOptions.pageNumber - 1) * paginationOptions.pageSize;
        $scope.gridStatsStatements.data = data.slice(firstRow, firstRow + paginationOptions.pageSize);
    }
    getPage();
}



angular.module('WebApp').controller('StatsStatementsController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', Web.Controllers.StatsStatementsController]);