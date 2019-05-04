Web.Controllers.AnalysisWorkloadAnalysisCreateController = function ($scope, $rootScope, $http, uiGridConstants, $stateParams, $window, $translate, $mdDialog) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOAD_ANALYSIS_CREATE.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
    $scope.actions.showDataTransferScript = function (ev, replacement) {
        $mdDialog.show(
            $mdDialog.alert()
                .parent(angular.element(document.querySelector('#pageBody')))
                .clickOutsideToClose(true)
                .title('Data transfer script ' + replacement.SourceRelation.FullName + " => " + replacement.TargetRelation.FullName)
                .htmlContent('Select into ...<br />Test')
                .ariaLabel('showDataTransferScriptDialo')
                .ok('Close')
                .targetEvent(ev)
        );
    };
    $scope.viewModel = new Object();
    $scope.viewModel.dateFrom = moment().startOf('day');
    $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
    $scope.viewModel.allRelations = [];
    for (var i = 0; i < 100; i++) {
        var toAdd = new Object();
        toAdd.Id = i;
        toAdd.FullName = "schema.relation" + i;
        $scope.viewModel.allRelations.push(toAdd);
    }
    $scope.viewModel.allWorkloads = [];
    for (var i = 0; i < 5; i++) {
        var toAdd = new Object();
        toAdd.Id = i;
        toAdd.Name = "Workload" + i;
        $scope.viewModel.allWorkloads.push(toAdd);
    }
    $scope.viewModel.WorkloadAnalysis = new Object();
    $scope.viewModel.WorkloadAnalysis.Workload = new Object();
    $scope.viewModel.WorkloadAnalysis.Workload = $scope.viewModel.allWorkloads[0];
    $scope.viewModel.WorkloadAnalysis.RelationReplacements = [];
    for (var i = 0; i < 2; i++) {
        var toAdd = new Object();
        toAdd.SourceRelation = $scope.viewModel.allRelations[i];
        toAdd.TargetRelation = $scope.viewModel.allRelations[i + 1];
        $scope.viewModel.WorkloadAnalysis.RelationReplacements.push(toAdd);
    }
    $scope.viewModel.sourceRelation = null;
    $scope.viewModel.targetRelation = null;
}



angular.module('WebApp').controller('AnalysisWorkloadAnalysisCreateController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$stateParams', '$window', '$translate', '$mdDialog', Web.Controllers.AnalysisWorkloadAnalysisCreateController]);