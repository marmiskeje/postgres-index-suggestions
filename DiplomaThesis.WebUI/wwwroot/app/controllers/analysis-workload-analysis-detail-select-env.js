Web.Controllers.AnalysisWorkloadAnalysisDetailSelectEnvController = function ($scope, $rootScope, $window, settingsService, notificationsService, $state, $stateParams) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOAD_ANALYSIS_DETAIL_SELECT_ENV.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_DETAIL, { workloadAnalysis: $stateParams.workloadAnalysis, data: $stateParams.data, selectedEnvironmentID: $stateParams.currentEnvironmentID });
    };
    $scope.actions.selectEnv = function (env) {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_DETAIL, { workloadAnalysis: $stateParams.workloadAnalysis, data: $stateParams.data, selectedEnvironmentID: env.environmentID });
    };
    $scope.viewModel = new Object();
    $scope.viewModel.environments = [];
    for (var envID in $stateParams.data.indicesEnvironments) {
        var env = $stateParams.data.indicesEnvironments[envID];
        var indexNames = [];
        for (var indexID in env.indices) {
            var indexExtended = $stateParams.data.indices[indexID];
            indexNames.push(indexExtended.name);
        }
        
        $scope.viewModel.environments.push({ indexNames: indexNames, environmentID: envID});
    }
}

angular.module('WebApp').controller('AnalysisWorkloadAnalysisDetailSelectEnvController', ['$scope', '$rootScope', '$window', 'settingsService', 'notificationsService', '$state', '$stateParams', Web.Controllers.AnalysisWorkloadAnalysisDetailSelectEnvController]);