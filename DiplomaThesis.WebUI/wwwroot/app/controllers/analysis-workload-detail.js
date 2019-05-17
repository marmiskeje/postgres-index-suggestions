Web.Controllers.AnalysisWorkloadDetailController = function ($scope, $rootScope, $window, uiGridConstants, $state, $stateParams, analysisService, drawingService, notificationsService, $translate) {
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
    $scope.actions.createWorkload = function () {
        var workload = $scope.viewModel.workload;
        workload.definition.forbiddenRelations = [];
        for (var i = 0; i < $scope.viewModel.forbiddenRelations.length; i++) {
            workload.definition.forbiddenRelations.push($scope.viewModel.forbiddenRelations[i].id);
        }
        $scope.viewModel.isLoading = true;
        analysisService.createWorkload(workload, function (response) {
            $scope.viewModel.isLoading = false;
            if (response.data == null) {
                notificationsService.showError("An error occured during creation.", response.status, response.statusText);
            }
            else if (response.data.isSuccess) {
                notificationsService.showMessage("Workload " + workload.name + " was succesfully created.");
                $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOADS, { enforceLoading: true });
            }
            else {
                notificationsService.showError("An error occured during creation.", 500, response.data.errorMessage);
            }
        });
    };
    $scope.actions.searchRelation = function (str) {
        var lowerStr = str.toLowerCase();
        if ($rootScope.viewModel.currentDatabase.id in $rootScope.viewModel.databaseRelations) {
            return $rootScope.viewModel.databaseRelations[$rootScope.viewModel.currentDatabase.id].filter(x => {
                return x.fullName.toLowerCase().indexOf(lowerStr) >= 0 && !$scope.viewModel.forbiddenRelations.includes(x);
            });
        }
        return [];
    };
    $scope.actions.removeForbiddenDateTimeSlot = function (slot) {
        $scope.viewModel.workload.definition.forbiddenDateTimeSlots = $scope.viewModel.workload.definition.forbiddenDateTimeSlots.filter(function (item) {
            return item !== slot
        })
    }
    $scope.actions.addForbiddenDateTimeSlot = function () {
        var toAdd = new Web.Data.WorkloadTimeSlotData();
        toAdd.dayOfWeek = $scope.viewModel.forbiddenDateTimeSlotDay.id;
        toAdd.startTime = moment($scope.viewModel.forbiddenDateTimeSlotStartTime).format('HH:mm:ss');
        toAdd.endTime = moment($scope.viewModel.forbiddenDateTimeSlotEndTime).format('HH:mm:ss');
        $scope.viewModel.workload.definition.forbiddenDateTimeSlots.push(toAdd);
        $scope.viewModel.forbiddenDateTimeSlotStartTime = null;
        $scope.viewModel.forbiddenDateTimeSlotEndTime = null;
    };
    $scope.viewModel = new Web.ViewModels.AnalysisWorkloadDetailViewModel();
    $scope.viewModel.days = [];
    $scope.viewModel.days.push({ id: 0, name: 'Sunday' });
    $scope.viewModel.days.push({ id: 1, name: 'Monday' });
    $scope.viewModel.days.push({ id: 2, name: 'Thuesday' });
    $scope.viewModel.days.push({ id: 3, name: 'Wednesday' });
    $scope.viewModel.days.push({ id: 4, name: 'Thursday' });
    $scope.viewModel.days.push({ id: 5, name: 'Friday' });
    $scope.viewModel.days.push({ id: 6, name: 'Saturday' });
    $scope.viewModel.forbiddenDateTimeSlotDay = $scope.viewModel.days[0];
    if ($stateParams.workload) {
        $rootScope.pageSubtitle = $translate.instant('ANALYSIS_WORKLOAD_DETAIL.VIEW') + " " + $stateParams.workload.Name;
        $scope.viewModel.isReadonly = true;
        $scope.viewModel.workload = $stateParams.workload;
        if ($scope.viewModel.workload.definition == null) {
            $scope.viewModel.workload.definition = new Web.Data.WorkloadDefinitionData();
        }
        $scope.viewModel.forbiddenRelations = [];
        for (var i = 0; i < $scope.viewModel.workload.definition.forbiddenRelations.length; i++) {
            var relationID = $scope.viewModel.workload.definition.forbiddenRelations[i];
            if (relationID in $rootScope.viewModel.allRelations) {
                $scope.viewModel.forbiddenRelations.push($rootScope.viewModel.allRelations[relationID]);
            }
            else {
                $scope.viewModel.forbiddenRelations.push({ id: relationID, fullName: relationID});
            }
        }
    } else {
        $rootScope.pageSubtitle = $translate.instant('ANALYSIS_WORKLOAD_DETAIL.CREATE_NEW');
        $scope.viewModel.isReadonly = false;
        $scope.viewModel.workload = new Web.Data.WorkloadData();
        $scope.viewModel.workload.databaseID = $rootScope.viewModel.currentDatabase.id;
        /*
         * implict forbidden relations - system ones
         */
        if ($rootScope.viewModel.currentDatabase.id in $rootScope.viewModel.databaseRelations) {
            for (var i = 0; i < $rootScope.viewModel.databaseRelations[$rootScope.viewModel.currentDatabase.id].length; i++) {
                var relation = $rootScope.viewModel.databaseRelations[$rootScope.viewModel.currentDatabase.id][i];
                if (relation.schemaName == "information_schema" || relation.schemaName == "pg_catalog") {
                    $scope.viewModel.forbiddenRelations.push(relation);
                }
            }
        }
        $scope.viewModel.validate();
    }
    $scope.pageSubtitle = $rootScope.pageSubtitle;
    
}

angular.module('WebApp').controller('AnalysisWorkloadDetailController', ['$scope', '$rootScope', '$window', 'uiGridConstants', '$state', '$stateParams', 'analysisService', 'drawingService', 'notificationsService', '$translate', Web.Controllers.AnalysisWorkloadDetailController]);