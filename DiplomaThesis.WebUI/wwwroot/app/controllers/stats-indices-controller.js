Web.Controllers.StatsIndicesController = function ($scope, $rootScope, $http, uiGridConstants, $state) {
    $rootScope.pageSubtitle = 'STATS_INDICES.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showStatementDetail = function (statementId) {
        $state.go(Web.Constants.StateNames.STATS_STATEMENT_DETAIL, { statementId: statementId, dateFrom: $scope.viewModel.dateFrom, dateTo: $scope.viewModel.dateTo });
    };
    $scope.viewModel = new Object();
    $scope.viewModel.isValid = true;
    $scope.viewModel.isLoading = false;
    $scope.viewModel.dateFrom = moment().startOf('day');
    $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
    var paginationOptions = {
        pageNumber: 1,
        pageSize: 25,
        sort: null
    };
    $scope.gridStatsIndicesStatements = {
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
            { name: 'IndexScansCount', displayName: 'Index scans count', maxWidth: 180, field: 'IndexScansCount' },
            { name: 'MinCost', displayName: 'Min idx scan cost', maxWidth: 180, field: 'MinCost' },
            { name: 'MaxCost', displayName: 'Max idx scan cost', maxWidth: 180, field: 'MaxCost' },
            { name: 'AvgCost', displayName: 'Avg idx scan cost', maxWidth: 180, field: 'AvgCost' }
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
            toAdd.IndexScansCount = i;
            toAdd.MinCost = 1.2;
            toAdd.MaxCost = 5.6;
            toAdd.AvgCost = 3.1;
            data.push(toAdd);
        }
        $scope.gridStatsIndicesStatements.totalItems = 100;
        var firstRow = (paginationOptions.pageNumber - 1) * paginationOptions.pageSize;
        $scope.gridStatsIndicesStatements.data = data.slice(firstRow, firstRow + paginationOptions.pageSize);
    }
    getPage();
    this.chart = null;
    this.drawChart = function() {
        var ctx = document.getElementById('div-stats-indices').getContext('2d');
        this.chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [new Date(2018, 1, 1), new Date(2018, 1, 2), new Date(2018, 1, 5)],
                datasets: [{
                    label: 'Index scans count',
                    data: [
                        10, 30, 5,
                    ],
                    fill: false,
                }, {
                    label: 'Index tuple reads count',
                    fill: false,
                    data: [
                        500, 30, 5
                    ],
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        type: 'time',
                        time: {
                            minUnit: 'minute',
                            displayFormats: {
                                minute: 'MMM D h:mm a',
                                hour: 'MMM D h:mm a'
                            }
                        }
                    }]
                },
                plugins: {
                    colorschemes: {
                        scheme: 'brewer.Set1-9'//'brewer.Paired12'
                    }
                }
            }
        });
    }
    this.drawChart();
}



angular.module('WebApp').controller('StatsIndicesController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', Web.Controllers.StatsIndicesController]);