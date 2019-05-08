Web.Services.AnalysisService = function ($http) {
    this.getUnusedObjects = function (request, onFinally) {
        $http.post('api/analysis/unused-objects', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
}
Web.App.service('analysisService', ['$http', Web.Services.AnalysisService]);