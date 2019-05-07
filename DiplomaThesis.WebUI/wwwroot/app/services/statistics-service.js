Web.Services.StatisticsService = function ($http) {
    this.getOverview = function (request, onFinally) {
        $http.post('api/stats/overview', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }

    this.getRelationTotalStats = function (request, onFinally) {
        $http.post('api/stats/relation-total', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
}
Web.App.service('statisticsService', ['$http', Web.Services.StatisticsService]);