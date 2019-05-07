Web.Controllers.AnalysisWorkloadAnalysesController = function ($scope, $rootScope, $http, uiGridConstants, $state) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOAD_ANALYSES.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showAnalysisDetail = function (workloadAnalysis) {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_DETAIL, { workloadAnalysis: workloadAnalysis });
    };
    $scope.actions.showCreateWindow = function () {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_CREATE);
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
    $scope.gridAnalyses = {
        enablePaginationControls: false,
        paginationPageSize: paginationOptions.pageSize,
        useExternalPagination: true,
        useExternalSorting: true,
        columnDefs: [
            { name: 'CreatedDate', displayName: 'Created date', field: 'CreatedDate' },
            { name: 'Workload', displayName: 'Workload', field: 'Workload' },
            { name: 'Period', displayName: 'For period', field: 'Period' },
            { name: 'State', displayName: 'State', field: 'State' },
            { name: 'StartDate', displayName: 'Start date', field: 'StartDate' },
            { name: 'EndDate', displayName: 'End date', field: 'EndDate' },
            {
                name: 'Actions', displayName: '', field: 'Actions', enableSorting: false, maxWidth: 50, enableHiding: false,
                cellTemplate: '<md-button style="padding: 0; margin: 0; min-height: inherit; min-width: inherit"' +
                    'ng-click="grid.appScope.actions.showAnalysisDetail(row.entity)"><md-tooltip>Show detail</md-tooltip><i class="material-icons md-18">zoom_in</i></md-button>'
            }
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
            toAdd.CreatedDate = moment().format("YYYY-MM-DD HH:mm:ss");
            toAdd.Workload = "Test workload" + i;
            toAdd.Period = moment().format("YYYY-MM-DD HH:mm:ss") + " - " + moment().format("YYYY-MM-DD HH:mm:ss");
            toAdd.State = "Finished";
            toAdd.StartDate = moment().format("YYYY-MM-DD HH:mm:ss");
            toAdd.EndDate = moment().format("YYYY-MM-DD HH:mm:ss");
            data.push(toAdd);
        }
        $scope.gridAnalyses.totalItems = 100;
        var firstRow = (paginationOptions.pageNumber - 1) * paginationOptions.pageSize;
        $scope.gridAnalyses.data = data.slice(firstRow, firstRow + paginationOptions.pageSize);
    }
    getPage();
}



angular.module('WebApp').controller('AnalysisWorkloadAnalysesController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', Web.Controllers.AnalysisWorkloadAnalysesController]);