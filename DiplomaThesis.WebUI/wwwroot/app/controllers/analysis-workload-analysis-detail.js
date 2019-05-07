Web.Controllers.AnalysisWorkloadAnalysisDetailController = function ($scope, $rootScope, $stateParams, $window) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOAD_ANALYSIS_DETAIL.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
    $scope.viewModel = new Object();
    $scope.viewModel.WorkloadAnalysis = $stateParams.workloadAnalysis;
    $scope.gridAnalysisIndicesAffectedStatements = {
        minRowsToShow: 10,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Statement', displayName: 'Statement', field: 'Statement', minWidth: 200
            },
            {
                name: 'relation.IndexName0', displayName: 'relation.IndexName0', field: 'IsUsed', headerTooltip: 'Name of possible index',
                cellTemplate: '<div class="ui-grid-cell-contents" style="text-align: center">' +
                    '<md-checkbox aria-label="is-used" disabled ng-checked="{{row.entity.PossibleIndices[0].IsUsed}}">{{row.entity.PossibleIndices[0].IsCovering ? "COVERING!" : ""}}</md-checkbox>' +
                    '<md-tooltip md-direction="top">{{row.entity.PossibleIndices[0].IsUsed ? "Used" : "Not used"}}</md-tooltip></div>'
            },
            {
                name: 'relation.IndexName1', displayName: 'relation.IndexName1', field: 'IsUsed', headerTooltip: 'Name of possible index',
                cellTemplate: '<div class="ui-grid-cell-contents" style="text-align: center">' +
                    '<md-checkbox aria-label="is-used" disabled ng-checked="{{row.entity.PossibleIndices[0].IsUsed}}"></md-checkbox>' +
                    '<md-tooltip md-direction="top">{{row.entity.PossibleIndices[0].IsUsed ? "Used" : "Not used"}}</md-tooltip></div>'
            },
            {
                name: 'ExecutionCount', displayName: 'Execution count', field: 'ExecutionCount', maxWidth: 160
            },
            {
                name: 'RealTotalCost', displayName: 'Real total cost', field: 'RealTotalCost', maxWidth: 160
            },
            {
                name: 'PotentialTotalCost', displayName: 'Potential total cost', field: 'PotentialTotalCost', maxWidth: 170
            },
            {
                name: 'LocalImprovement', displayName: 'Local % ↑', field: 'LocalImprovement', maxWidth: 100, headerTooltip: '% improvement of statement execution price'
            },
            {
                name: 'GlobalImprovement', displayName: 'Global % ↑', field: 'GlobalImprovement', maxWidth: 100, headerTooltip: '% improvement towards others'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };
    $scope.gridAnalysisIndicesPossibleIndices = {
        minRowsToShow: 5,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Relation', displayName: 'Relation', field: 'RelationName'
            },
            {
                name: 'Name', displayName: 'Name', field: 'Name'
            },
            {
                name: 'Size', displayName: 'Potential size', field: 'Size'
            },
            {
                name: 'GlobalImpact', displayName: 'Global % impact', field: 'GlobalImpact', headerTooltip: '% impact considering ALL statements (not only selects)', maxWidth: 180
            },
            {
                name: 'Possibilities', displayName: '', field: 'Possibilities', maxWidth: 100, enableSorting: false, enableHiding: false,
                cellTemplate: '<div class="ui-grid-cell-contents" style="text-align: center">' +
                    '<md-button style="padding: 0; margin: 0; min-height: inherit; min-width: inherit">' +
                    '<md-tooltip>Show CREATE definition</md-tooltip><i class="material-icons md-18">subject</i></md-button>' +
                    '<md-button ng-hide="row.entity.Filters.length == 0" style="margin-left: 7px;margin-right: 0;margin-top: 0;margin-bottom: 0; padding: 0; min-height: inherit; min-width: inherit">' +
                    '<md-tooltip>Show possible filters</md-tooltip><i class="material-icons md-18">filter_list</i></md-button>'
                    + '</div>'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };
    var getPage = function () {
        var indicesAffectedStatements = [];
        for (var i = 0; i < 30; i++) {
            var toAdd = new Object();
            toAdd.Statement = "Select * from relation.Test" + i;
            toAdd.Name = "TestName" + i;
            toAdd.PossibleIndices = [];
            var ind = new Object();
            ind.Name = "relation.IndexName" + i;
            ind.IsUsed = i % 2 == 0;
            ind.IsCovering = i % 5 == 0;
            toAdd.PossibleIndices.push(ind);
            indicesAffectedStatements.push(toAdd);
        }
        $scope.gridAnalysisIndicesAffectedStatements.totalItems = 100;
        $scope.gridAnalysisIndicesAffectedStatements.data = indicesAffectedStatements;

        var indicesPossibleIndices = [];
        for (var i = 0; i < 6; i++) {
            var toAdd = new Object();
            toAdd.RelationName = "relation";
            toAdd.Name = "relation.PossibleIndex" + i;
            toAdd.Size = "456 MB";
            toAdd.CreateDefinition = "Create ...";
            toAdd.Filters = [];
            if (i % 3 == 0) {
                var filter = new Object();
                filter.Filter = "WHERE x = 1";
                filter.Size = "45 MB";
                toAdd.Filters.push(filter);
            }
            indicesPossibleIndices.push(toAdd);
        }
        $scope.gridAnalysisIndicesPossibleIndices.totalItems = 100;
        $scope.gridAnalysisIndicesPossibleIndices.data = indicesPossibleIndices;
    }
    getPage();
}

angular.module('WebApp').controller('AnalysisWorkloadAnalysisDetailController', ['$scope', '$rootScope', '$stateParams', '$window', Web.Controllers.AnalysisWorkloadAnalysisDetailController]);