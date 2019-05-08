Web.Controllers.AnalysisUnusedDbObjectsController = function ($scope, $rootScope, $http, uiGridConstants, $state, analysisService, drawingService, notificationsService) {
    $rootScope.pageSubtitle = 'ANALYSIS_UNUSED_DB_OBJECTS.PAGE_SUBTITLE';
    $scope.gridAnalysisUnusedDbObjects = {
        paginationPageSize: 25,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Schema', displayName: 'Schema', field: 'schemaName'
            },
            { name: 'Name', displayName: 'Name', field: 'name' },
            { name: 'ObjectType', displayName: 'Object type', field: 'objectType', enableSorting: false },
            {
                name: 'LastUsage', displayName: 'Last known usage', field: 'lastKnownUsageDateStr',
                cellTemplate: '<div ng-class="{\'md-warn\': row.entity.isUnused }" class="ui-grid-cell-contents">' +
                    '<span>{{row.entity.lastKnownUsageDateStr}}</span>'
                    + '<md-tooltip>{{row.entity.isUnused == null ? "Not enough statistics for evaluation" : (row.entity.isUnused ? "Unused" : "Used")}}</md-tooltip>'
                    + '</div>'
            }
        ],
        onRegisterApi: function (gridApi){
            $scope.gridAnalysisUnusedDbObjectsApi = gridApi;
            $scope.gridAnalysisUnusedDbObjectsApi.core.queueRefresh();
            $scope.gridAnalysisUnusedDbObjectsApi.core.handleWindowResize();
        }
    };
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.AnalysisUnusedObjectsRequest();
            request.databaseID = $rootScope.viewModel.currentDatabase.id;
            analysisService.getUnusedObjects(request, function (response) {
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
        $scope.gridAnalysisUnusedDbObjects.totalItems = 0;
        $scope.gridAnalysisUnusedDbObjects.data = [];
        $scope.gridAnalysisUnusedDbObjectsApi.core.queueRefresh();
        $scope.gridAnalysisUnusedDbObjectsApi.core.handleWindowResize();
    }
    $scope.actions.refreshData = function (enforceLoading) {
        var loadData = new Promise(function (resolve, reject) {
            if (enforceLoading || $scope.viewModel.data == null) {
                $scope.actions.loadData().then(function (result) {
                    resolve(result);
                });
            }
            else {
                resolve($scope.viewModel.data)
            }
        });
        loadData.then(function (data) {
            $scope.actions.clearData();
            if (data != null) {
                $scope.viewModel.data = data;
                $scope.gridAnalysisUnusedDbObjects.data = data.objects;
                for (var i = 0; i < $scope.gridAnalysisUnusedDbObjects.data.length; i++) {
                    if ($scope.gridAnalysisUnusedDbObjects.data[i].lastKnownUsageDate != null) {
                        $scope.gridAnalysisUnusedDbObjects.data[i].lastKnownUsageDateStr = moment().format("YYYY-MM-DD HH:mm:ss");
                    }
                    else {
                        $scope.gridAnalysisUnusedDbObjects.data[i].lastKnownUsageDateStr = "Unknown";
                    }
                }
                $scope.gridAnalysisUnusedDbObjects.totalItems = $scope.gridAnalysisUnusedDbObjects.data.length;
                $scope.gridAnalysisUnusedDbObjectsApi.core.queueRefresh();
                $scope.gridAnalysisUnusedDbObjectsApi.core.handleWindowResize();
            }
        });
    }
    $rootScope.$on('onDatabaseChanged', function () {
        $scope.actions.clearData();
    })

    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.AnalysisUnusedObjectsViewModel();
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
}



angular.module('WebApp').controller('AnalysisUnusedDbObjectsController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$state', 'analysisService', 'drawingService', 'notificationsService', Web.Controllers.AnalysisUnusedDbObjectsController]);