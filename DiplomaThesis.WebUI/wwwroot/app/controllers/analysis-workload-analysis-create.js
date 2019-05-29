Web.Controllers.AnalysisWorkloadAnalysisCreateController = function ($scope, $rootScope, $window, uiGridConstants, $state, $stateParams, analysisService, drawingService, notificationsService, $translate, $mdDialog) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOAD_ANALYSIS_CREATE.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
    $scope.actions.createWorkloadAnalysis = function () {
        var workloadAnalysis = $scope.viewModel.workloadAnalysis;
        workloadAnalysis.periodFromDate = new moment(workloadAnalysis.periodFromDate).format('YYYY-MM-DDTHH:mm:ss');
        workloadAnalysis.periodToDate = new moment(workloadAnalysis.periodToDate).format('YYYY-MM-DDTHH:mm:ss');
        workloadAnalysis.relationReplacements = {};
        for (var i = 0; i < $scope.viewModel.relationReplacements.length; i++) {
            var r = $scope.viewModel.relationReplacements[i];
            workloadAnalysis.relationReplacements[r.sourceRelation.id] = r.targetRelation.id;
        }
        $scope.viewModel.isLoading = true;
        analysisService.createWorkloadAnalysis(workloadAnalysis, function (response) {
            $scope.viewModel.isLoading = false;
            if (response.data == null) {
                notificationsService.showError("An error occured during creation.", response.status, response.statusText);
            }
            else if (response.data.isSuccess) {
                notificationsService.showMessage("Workload analysis was succesfully created.");
                $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSES, { enforceLoading: true });
            }
            else {
                notificationsService.showError("An error occured during creation.", 500, response.data.errorMessage);
            }
        });
    };
    $scope.actions.addRelationReplacement = function () {
        $scope.viewModel.relationReplacements.push({ "sourceRelation": $scope.viewModel.sourceRelation, "targetRelation": $scope.viewModel.targetRelation });
        $scope.viewModel.sourceRelation = null;
        $scope.viewModel.targetRelation = null;
    };
    $scope.actions.removeRelationReplacement = function (r) {
        $scope.viewModel.relationReplacements = $scope.viewModel.relationReplacements.filter(function (item) {
            return item !== r
        });
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
    $scope.actions.loadWorkloads = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Web.Data.AnalysisWorkloadsRequest();
            request.databaseID = $rootScope.viewModel.currentDatabase.id;
            request.filter.dateFrom = moment('0001-01-01').format('YYYY-MM-DDTHH:mm:ss');
            request.filter.dateTo = moment().format('YYYY-MM-DDTHH:mm:ss');
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
    $scope.viewModel = new Web.ViewModels.AnalysisWorkloadAnalysisCreateViewModel();
    $scope.actions.loadWorkloads().then(function (workloads) {
        $scope.viewModel.workloadAnalysis = new Object();
        $scope.viewModel.workloadAnalysis.periodFromDate = moment().startOf('day').add(Web.Constants.Defaults.DATE_PERIOD_ADD_FROM_DAYS, 'days');
        $scope.viewModel.workloadAnalysis.periodToDate = moment().startOf('day').add(1, 'days');
        $scope.viewModel.allAvailableRelations = $rootScope.viewModel.databaseRelations[$rootScope.viewModel.currentDatabase.id];
        
        $scope.viewModel.allWorkloads = workloads;
        $scope.viewModel.workloadAnalysis.workloadID = 0;
        if ($scope.viewModel.allWorkloads.length > 0) {
            $scope.viewModel.workloadAnalysis.workloadID = $scope.viewModel.allWorkloads[0].id;
        }
        $scope.viewModel.workloadAnalysis.relationReplacements = [];
        $scope.viewModel.sourceRelation = null;
        $scope.viewModel.targetRelation = null;
        $scope.viewModel.validate();
        $scope.viewModel.validateReplacement();
        $scope.$apply();
    });
}



angular.module('WebApp').controller('AnalysisWorkloadAnalysisCreateController', ['$scope', '$rootScope', '$window', 'uiGridConstants', '$state', '$stateParams', 'analysisService', 'drawingService', 'notificationsService', '$translate', '$mdDialog', Web.Controllers.AnalysisWorkloadAnalysisCreateController]);