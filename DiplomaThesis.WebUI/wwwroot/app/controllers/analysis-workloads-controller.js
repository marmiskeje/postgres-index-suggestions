Web.Controllers.AnalysisWorkloadsController = function ($scope, $rootScope, $http, uiGridConstants, $state, $stateParams, analysisService, drawingService, notificationsService) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOADS.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showWorkloadDetail = function (workload) {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_DETAIL, { workload: workload });
    };
    $scope.actions.deleteWorkload = function (workload) {
        if (confirm("Are you sure you want to delete workload " + workload.name + "?") == true) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.AnalysisDeleteWorkloadRequest();
            request.workloadID = workload.id;
            analysisService.deleteWorkload(request, function (response) {
                $scope.viewModel.isLoading = false;
                if (response.data == null) {
                    notificationsService.showError("An error occured during workload removing.", response.status, response.statusText);
                }
                else if (response.data.isSuccess) {
                    notificationsService.showMessage("Workload " + workload.name + " was removed.");
                    $scope.actions.refreshData(true);
                }
                else {
                    notificationsService.showError("An error occured during workload removing.", 500, response.data.errorMessage);
                }
            });
        }
    };
    $scope.gridAnalysisWorkloads = {
        paginationPageSize: 25,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Name', displayName: 'Name', field: 'name', enableSorting: false,
                cellTemplate: '<div class="ui-grid-cell-contents">' +
                    '  <a href="javascript:void(0)" ng-disabled="grid.appScope.isLoading" ng-click="grid.appScope.actions.showWorkloadDetail(row.entity)" >{{row.entity.name}}</a>' +
                    '</div>'
            },
            { name: 'CreatedDate', displayName: 'Created date', field: 'createdDateStr' },
            { name: 'ForbiddenUsers', displayName: 'Forbidden users', field: 'definition.forbiddenUsers', enableSorting: false, cellFilter: 'stringArrayFilter' },
            { name: 'ForbiddenRelations', displayName: 'Forbidden relations', field: 'definition.forbiddenRelations', enableSorting: false, cellFilter: 'relationArrayFilter' },
            { name: 'ForbiddenApplications', displayName: 'Forbidden applications', field: 'definition.forbiddenApplications', enableSorting: false, cellFilter: 'stringArrayFilter' },
            {
                name: 'Actions', displayName: '', field: 'Actions', enableSorting: false, maxWidth: 50, enableHiding: false,
                cellTemplate: '<md-button ng-disabled="grid.appScope.isLoading" ng-click="grid.appScope.actions.deleteWorkload(row.entity)" style="padding: 0; margin: 0; min-height: inherit; min-width: inherit">' +
                    '<i class="material-icons md-18">delete_forever</i>'
                    + '<md-tooltip>Remove</md-tooltip></md-button>'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridAnalysisWorkloadsApi = gridApi;
            $scope.gridAnalysisWorkloadsApi.core.queueRefresh();
            $scope.gridAnalysisWorkloadsApi.core.handleWindowResize();
        }
    };
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.AnalysisWorkloadsRequest();
            request.databaseID = $rootScope.viewModel.currentDatabase.id;
            request.filter.dateFrom = moment($scope.viewModel.dateFrom).format('YYYY-MM-DDTHH:mm:ss');
            request.filter.dateTo = moment($scope.viewModel.dateTo).format('YYYY-MM-DDTHH:mm:ss');
            analysisService.getWorkloads(request, function (response) {
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
        $scope.gridAnalysisWorkloads.data = [];
        $scope.gridAnalysisWorkloadsApi.core.queueRefresh();
        $scope.gridAnalysisWorkloadsApi.core.handleWindowResize();
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
                for (var i = 0; i < data.length; i++) {
                    data[i].createdDateStr = moment(data[i].createdDate).format("YYYY-MM-DD HH:mm:ss")
                }
                $scope.gridAnalysisWorkloads.data = data;
                $scope.gridAnalysisWorkloadsApi.core.queueRefresh();
                $scope.gridAnalysisWorkloadsApi.core.handleWindowResize();
            }
        });
    }
    $rootScope.$on('onDatabaseChanged', function () {
        if ($state.is(Web.Constants.StateNames.ANALYSIS_WORKLOADS)) {
            $scope.actions.refreshData(true);
        }
        else {
            $scope.actions.clearData();
        }
    })

    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.AnalysisWorkloadsViewModel();
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

angular.module('WebApp').controller('AnalysisWorkloadsController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', '$stateParams', 'analysisService', 'drawingService', 'notificationsService', Web.Controllers.AnalysisWorkloadsController])
    .filter('stringArrayFilter', function () {
        return function (stringArray) {
            return stringArray.join(', ');
        };
    }).filter('relationArrayFilter', function ($rootScope) {
        return function (relationIds) {
            var result = [];
            relationIds.forEach(relationID => {
                result.push(relationID in $rootScope.viewModel.allRelations ? $rootScope.viewModel.allRelations[relationID].fullName : relationID);
            });
            return result.join(', ');
        };
    });