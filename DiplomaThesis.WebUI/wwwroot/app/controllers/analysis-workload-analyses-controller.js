Web.Controllers.AnalysisWorkloadAnalysesController = function ($scope, $rootScope, $http, uiGridConstants, $state, $stateParams, analysisService, drawingService, notificationsService) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOAD_ANALYSES.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showAnalysisDetail = function (workloadAnalysis) {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_DETAIL, { workloadAnalysis: workloadAnalysis });
    };
    $scope.actions.showCreateWindow = function () {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_CREATE);
    };
    $scope.gridAnalyses = {
        paginationPageSize: 25,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            { name: 'CreatedDate', displayName: 'Created date', maxWidth: 200, field: 'createdDateStr' },
            { name: 'Workload', displayName: 'Workload', field: 'workload.name' },
            { name: 'Period', displayName: 'For period', field: 'period' },
            { name: 'State', displayName: 'State', field: 'stateStr' },
            { name: 'StartDate', displayName: 'Start date', maxWidth: 200, field: 'startDateStr' },
            { name: 'EndDate', displayName: 'End date', maxWidth: 200, field: 'endDateStr' },
            {
                name: 'Actions', displayName: '', field: 'Actions', enableSorting: false, maxWidth: 50, enableHiding: false,
                cellTemplate: '<md-button style="padding: 0; margin: 0; min-height: inherit; min-width: inherit"' +
                    'ng-hide="row.entity.state != 2"' +
                    'ng-click="grid.appScope.actions.showAnalysisDetail(row.entity)"><md-tooltip>Show detail</md-tooltip><i class="material-icons md-18">zoom_in</i></md-button>'
            }
        ],
        onRegisterApi: function (gridApi){
            $scope.gridAnalysesApi = gridApi;
            $scope.gridAnalysesApi.core.queueRefresh();
            $scope.gridAnalysesApi.core.handleWindowResize();
        }
    };
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.AnalysisWorkloadAnalysesRequest();
            request.databaseID = $rootScope.viewModel.currentDatabase.id;
            request.filter.dateFrom = moment($scope.viewModel.dateFrom).format('YYYY-MM-DDTHH:mm:ss');
            request.filter.dateTo = moment($scope.viewModel.dateTo).format('YYYY-MM-DDTHH:mm:ss');
            analysisService.getWorkloadAnalyses(request, function (response) {
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
        $scope.gridAnalyses.data = [];
        $scope.gridAnalysesApi.core.queueRefresh();
        $scope.gridAnalysesApi.core.handleWindowResize();
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
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    item.period = moment(item.periodFromDate).format("YYYY-MM-DD HH:mm") + " - " + moment(item.periodToDate).format("YYYY-MM-DD HH:mm");
                    item.createdDateStr = moment(item.createdDate).format("YYYY-MM-DD HH:mm:ss");
                    item.startDateStr = "";
                    if (item.startDate != null) {
                        item.startDateStr = moment(item.startDate).format("YYYY-MM-DD HH:mm:ss");
                    }
                    item.endDateStr = "";
                    if (item.endDate != null) {
                        item.endDateStr = moment(item.endDate).format("YYYY-MM-DD HH:mm:ss");
                    }
                    switch (item.state) {
                        case 0:
                            item.stateStr = "Created";
                            break;
                        case 1:
                            item.stateStr = "In progress";
                            break;
                        case 2:
                            item.stateStr = "Finished";
                            break;
                        case 3:
                            item.stateStr = "Error (" + (item.errorMessage == null ? "Unknown" : item.errorMessage) + ")";
                            break;
                        default:
                            item.stateStr = "Unknown";
                            break;
                    }
                }
                $scope.viewModel.data = data;
                $scope.gridAnalyses.data = data;
                $scope.gridAnalysesApi.core.queueRefresh();
                $scope.gridAnalysesApi.core.handleWindowResize();
            }
        });
    }
    $rootScope.$on('onDatabaseChanged', function () {
        if ($state.is(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSES)) {
            $scope.actions.refreshData(true);
        }
        else {
            $scope.actions.clearData();
        }
    })

    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.AnalysisWorkloadAnalysesViewModel();
        $scope.viewModel.dateFrom = moment().startOf('day').add(Web.Constants.Defaults.DATE_PERIOD_ADD_FROM_DAYS, 'days');
        $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
    var enforceLoading = false;
    if ($stateParams.enforceLoading) {
        enforceLoading = true;
    }
    $scope.actions.refreshData(enforceLoading);
}



angular.module('WebApp').controller('AnalysisWorkloadAnalysesController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', '$stateParams', 'analysisService', 'drawingService', 'notificationsService', Web.Controllers.AnalysisWorkloadAnalysesController]);