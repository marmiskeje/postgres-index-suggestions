Web.Controllers.StatsStatementsController = function ($scope, $rootScope, $http, uiGridConstants, $state, statisticsService, drawingService, notificationsService) {
    $rootScope.pageSubtitle = 'STATS_STATEMENTS.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showStatementDetail = function (statement) {
        $state.go(Web.Constants.StateNames.STATS_STATEMENT_DETAIL, { statement: statement, dateFrom: $scope.viewModel.dateFrom, dateTo: $scope.viewModel.dateTo });
    };
    $scope.gridStatsStatements = {
        enablePaginationControls: false,
        paginationPageSize: 25,
        columnDefs: [
            { name: 'Command type', maxWidth: 150, field: 'commandTypeStr' },
            {
                name: 'Statement',
                cellTemplate: '<div class="ui-grid-cell-contents">' +
                    '  <a href="javascript:void(0)" ng-click="grid.appScope.actions.showStatementDetail(row.entity)" >{{row.entity.statement}}</a>' +
                    '</div>',
                field: 'statement'
            },
            { displayName: 'Total executions count', maxWidth: 220, name: 'totalExecutionsCount', field: 'totalExecutionsCount' },
            { displayName: 'Min duration (ms)', maxWidth: 170, field: 'minDurationStr', name: 'minDuration' },
            { displayName: 'Max duration (ms)', maxWidth: 170, field: 'maxDurationStr', name: 'maxDuration' },
            { displayName: 'Avg duration (ms)', maxWidth: 170, field: 'avgDurationStr', name: 'avgDuration' },
            { displayName: 'Min cost', maxWidth: 100, field: 'minTotalCost', name: 'minTotalCost' },
            { displayName: 'Max cost', maxWidth: 100, field: 'minTotalCost', name: 'maxTotalCost' },
            { displayName: 'Avg cost', maxWidth: 100, field: 'minTotalCost', name: 'avgTotalCost' }
        ],
        onRegisterApi: function (gridApi){
            $scope.gridStatsStatementsApi = gridApi;
            $scope.gridStatsStatementsApi.core.queueRefresh();
            $scope.gridStatsStatementsApi.core.handleWindowResize();
        }
    };
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.StatsStatementsRequest();
            request.databaseID = $rootScope.viewModel.currentDatabase.id;
            request.filter.dateFrom = moment($scope.viewModel.dateFrom).format('YYYY-MM-DDTHH:mm:ss');
            request.filter.dateTo = moment($scope.viewModel.dateTo).format('YYYY-MM-DDTHH:mm:ss');
            request.filter.commandType = $scope.viewModel.currentCommandType;
            statisticsService.getStatementsStats(request, function (response) {
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
        $scope.gridStatsStatements.data = [];
        $scope.gridStatsStatementsApi.core.queueRefresh();
        $scope.gridStatsStatementsApi.core.handleWindowResize();
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
                for (var i = 0; i < data.summaryNormalizedStatementStatistics.length; i++) {
                    var item = data.summaryNormalizedStatementStatistics[i];
                    item.minDurationStr = moment.duration(item.minDuration).asMilliseconds();
                    item.maxDurationStr = moment.duration(item.maxDuration).asMilliseconds();
                    item.avgDurationStr = moment.duration(item.avgDuration).asMilliseconds();
                    if (item.commandType != null) {
                        switch (item.commandType) {
                            case 1:
                                item.commandTypeStr = "Select";
                                break;
                            case 2:
                                item.commandTypeStr = "Insert";
                                break;
                            case 3:
                                item.commandTypeStr = "Update";
                                break;
                            case 4:
                                item.commandTypeStr = "Delete";
                                break;
                            case 5:
                                item.commandTypeStr = "Utility";
                                break;
                        }
                    }
                }
                $scope.gridStatsStatements.data = data.summaryNormalizedStatementStatistics;
                $scope.gridStatsStatementsApi.core.queueRefresh();
                $scope.gridStatsStatementsApi.core.handleWindowResize();
            }
        });
    }
    $rootScope.$on('onDatabaseChanged', function () {
        if ($state.is(Web.Constants.StateNames.STATS_STATEMENTS)) {
            $scope.actions.refreshData(true);
        }
        else {
            $scope.actions.clearData();
        }
    })

    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.StatsStatementsViewModel();
        $scope.viewModel.dateFrom = moment().startOf('day');
        $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
    $scope.actions.refreshData(false);
}



angular.module('WebApp').controller('StatsStatementsController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', 'statisticsService', 'drawingService', 'notificationsService', Web.Controllers.StatsStatementsController]);