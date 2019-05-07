Web.Controllers.AnalysisWorkloadsController = function ($scope, $rootScope, $state) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOADS.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.showWorkloadDetail = function (workload) {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_DETAIL, { workload: workload });
    };
    $scope.viewModel = new Object();
    $scope.viewModel.isValid = true;
    $scope.viewModel.isLoading = false;
    $scope.gridAnalysisWorkloads = {
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Name', displayName: 'Name', field: 'Name', enableSorting: false,
                cellTemplate: '<div class="ui-grid-cell-contents">' +
                    '  <a href="javascript:void(0)" ng-click="grid.appScope.actions.showWorkloadDetail(row.entity)" >{{row.entity.Name}}</a>' +
                    '</div>'
            },
            { name: 'CreatedDate', displayName: 'Created date', field: 'CreatedDate', enableSorting: false },
            { name: 'ForbiddenUsers', displayName: 'Forbidden users', field: 'ForbiddenUsers', enableSorting: false, cellFilter: 'stringArrayFilter' },
            { name: 'ForbiddenRelations', displayName: 'Forbidden relations', field: 'ForbiddenRelations', enableSorting: false, cellFilter: 'relationArrayFilter' },
            { name: 'ForbiddenApplications', displayName: 'Forbidden applications', field: 'ForbiddenApplications', enableSorting: false, cellFilter: 'stringArrayFilter' },
            {
                name: 'Actions', displayName: '', field: 'Actions', enableSorting: false, maxWidth: 50, enableHiding: false,
                cellTemplate: '<md-button style="padding: 0; margin: 0; min-height: inherit; min-width: inherit"><i class="material-icons md-18">delete_forever</i></md-button>'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };
    var getPage = function () {
        var data = [];
        for (var i = 0; i < 100; i++) {
            var toAdd = new Object();
            toAdd.Id = i;
            toAdd.Name = "Test" + i;
            toAdd.CreatedDate = moment().format("YYYY-MM-DD HH:mm:ss");
            toAdd.ForbiddenUsers = [];
            toAdd.ForbiddenUsers.push("User1");
            toAdd.ForbiddenUsers.push("USer2");
            toAdd.ForbiddenApplications = "Test, test";
            toAdd.ForbiddenRelations = [];
            for (var y = 0; y < 3; y++) {
                var r = new Object();
                r.Id = y;
                r.FullName = "schema.relation" + y;
                toAdd.ForbiddenRelations.push(r);
            }
            toAdd.ForbiddenApplications = [];
            toAdd.ForbiddenApplications.push("App1");
            toAdd.ForbiddenApplications.push("App2");
            data.push(toAdd);
        }
        $scope.gridAnalysisWorkloads.totalItems = 100;
        $scope.gridAnalysisWorkloads.data = data;
    }
    getPage();
}

angular.module('WebApp').controller('AnalysisWorkloadsController', ['$scope', '$rootScope', '$state', Web.Controllers.AnalysisWorkloadsController])
    .filter('stringArrayFilter', function () {
        return function (myArray) {
            return myArray.join(', ');
        };
    }).filter('relationArrayFilter', function () {
        return function (myArray) {
            var result = [];
            myArray.forEach(x => {
                result.push(x.FullName);
            });
            return result.join(', ');
        };
    });