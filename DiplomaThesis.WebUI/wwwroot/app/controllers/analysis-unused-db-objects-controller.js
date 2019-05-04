Web.Controllers.AnalysisUnusedDbObjectsController = function ($scope, $rootScope, $http, uiGridConstants) {
    $rootScope.pageSubtitle = 'ANALYSIS_UNUSED_DB_OBJECTS.PAGE_SUBTITLE';
    $scope.viewModel = new Object();
    $scope.viewModel.isValid = true;
    $scope.viewModel.isLoading = false;
    $scope.gridAnalysisUnusedDbObjects = {
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Schema', displayName: 'Schema', field: 'SchemaName'
            },
            { name: 'Name', displayName: 'Name', field: 'Name' },
            { name: 'ObjectType', displayName: 'Object type', field: 'ObjectType', enableSorting: false },
            {
                name: 'LastUsage', displayName: 'Last usage', field: 'LastUsage', cellTemplate: '<div ng-class="{\'md-warn\': row.entity.IsUnused }" class="ui-grid-cell-contents">' +
                    '{{row.entity.LastUsage}}' +
                    '</div>', }
        ],
        onRegisterApi: function (gridApi){
            $scope.gridApi = gridApi;
        }
    };
    var getPage = function () {
        var data = [];
        for (var i = 0; i < 100; i++) {
            var toAdd = new Object();
            toAdd.Id = i;
            toAdd.SchemaName = "Test" + i;
            toAdd.Name = "TestName" + i;
            toAdd.ObjectType = "Relation";
            toAdd.LastUsage = moment().format("YYYY-MM-DD HH:mm:ss");
            toAdd.IsUnused = i % 2 == 0;
            data.push(toAdd);
        }
        $scope.gridAnalysisUnusedDbObjects.totalItems = 100;
        $scope.gridAnalysisUnusedDbObjects.data = data;
    }
    getPage();
}



angular.module('WebApp').controller('AnalysisUnusedDbObjectsController', ['$scope', '$rootScope', '$http', 'uiGridConstants', Web.Controllers.AnalysisUnusedDbObjectsController]);