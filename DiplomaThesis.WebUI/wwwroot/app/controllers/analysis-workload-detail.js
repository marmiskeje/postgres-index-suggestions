Web.Controllers.AnalysisWorkloadDetailController = function ($scope, $rootScope, $http, uiGridConstants, $stateParams, $window, $translate) {
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
    $scope.viewModel = new Object();
    $scope.viewModel.allRelations = [];
    for (var i = 0; i < 100; i++) {
        var toAdd = new Object();
        toAdd.Id = i;
        toAdd.FullName = "schema.relation" + i;
        $scope.viewModel.allRelations.push(toAdd);
    }
    $scope.actions.searchRelation = function (str) {
        var lowerStr = str.toLowerCase();
        return $scope.viewModel.allRelations.filter(x => {
            return x.FullName.toLowerCase().indexOf(lowerStr) >= 0 && !$scope.viewModel.Workload.ForbiddenRelations.includes(x);
        });
    };
    $scope.viewModel.DayNames = moment.weekdays(true);
    $scope.viewModel.ForbiddenDateTimeSlotDay = $scope.viewModel.DayNames[0];
    $scope.viewModel.ForbiddenDateTimeSlotStartTime = null;
    $scope.viewModel.ForbiddenRelationSearch = null;
    $scope.viewModel.ForbiddenRelationToAdd = null;
    $scope.viewModel.Workload = new Object();
    $scope.viewModel.Workload.Name = "";
    $scope.viewModel.Workload.ForbiddenRelations = [];
    $scope.viewModel.Workload.ForbiddenUsers = [];
    $scope.viewModel.Workload.ForbiddenApplications = [];
    $scope.viewModel.Workload.Tresholds = new Object();
    $scope.viewModel.Workload.Tresholds.MinDuration = 0;
    $scope.viewModel.Workload.Tresholds.MinExectutionCount = 0;
    $scope.viewModel.Workload.ForbiddenDateTimeSlots = [];
    var slot = new Object();
    slot.DayName = 'Monday';
    slot.StartTime = moment().format("LTS");
    slot.EndTime = moment().format("LTS");
    $scope.viewModel.Workload.ForbiddenDateTimeSlots.push(slot);
    slot = new Object();
    slot.DayName = 'Monday2';
    slot.StartTime = moment().format("LTS");
    slot.EndTime = moment().format("LTS");
    $scope.viewModel.Workload.ForbiddenDateTimeSlots.push(slot);
    if ($stateParams.workload) {
        $rootScope.pageSubtitle = $translate.instant('ANALYSIS_WORKLOAD_DETAIL.VIEW') + " " + $stateParams.workload.Name;
        $scope.viewModel.isReadonly = true;
        $scope.viewModel.Workload = $stateParams.workload;
    } else {
        $rootScope.pageSubtitle = $translate.instant('ANALYSIS_WORKLOAD_DETAIL.CREATE_NEW');
        $scope.viewModel.isReadonly = false;
    }
    $scope.pageSubtitle = $rootScope.pageSubtitle;
    
    $scope.viewModel.isValid = true;
    
}



angular.module('WebApp').controller('AnalysisWorkloadDetailController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$stateParams', '$window', '$translate', Web.Controllers.AnalysisWorkloadDetailController]);