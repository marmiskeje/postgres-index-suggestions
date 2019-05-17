Web.Controllers.AnalysisWorkloadAnalysisDetailSelectEnvController = function ($scope, $rootScope, $window, settingsService, notificationsService, $state, $stateParams) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOAD_ANALYSIS_DETAIL_SELECT_ENV.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_DETAIL, { workloadAnalysis: $stateParams.workloadAnalysis, data: $stateParams.data, selectedEnvironmentID: $stateParams.currentEnvironmentID });
    };
    $scope.actions.selectEnv = function (env) {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_DETAIL, { workloadAnalysis: $stateParams.workloadAnalysis, data: $stateParams.data, selectedEnvironmentID: env.environmentID });
    };
    $scope.actions.getPercentage = function (val) {
        return (val * 100.0).toFixed(2) + " %";
    }
    $scope.viewModel = new Object();
    $scope.viewModel.environments = [];
    for (var envID in $stateParams.data.indicesEnvironments) {
        var env = $stateParams.data.indicesEnvironments[envID];
        var indexNames = [];
        var improvementRatio = 0.0;
        for (var indexID in env.indices) {
            var indexExtended = $stateParams.data.indices[indexID];
            var index = env.indices[indexID];
            improvementRatio += index.improvementRatio;
            indexNames.push({ "long": indexExtended.name, "short": indexExtended.name.substring(0, 60)});
        }
        
        $scope.viewModel.environments.push({ indexNames: indexNames, environmentID: envID, improvementRatio: $scope.actions.getPercentage(improvementRatio)});
    }
}

angular.module('WebApp').controller('AnalysisWorkloadAnalysisDetailSelectEnvController', ['$scope', '$rootScope', '$window', 'settingsService', 'notificationsService', '$state', '$stateParams', Web.Controllers.AnalysisWorkloadAnalysisDetailSelectEnvController]);