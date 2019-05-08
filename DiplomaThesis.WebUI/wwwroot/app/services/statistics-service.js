Web.Services.StatisticsService = function ($http) {
    this.getOverview = function (request, onFinally) {
        $http.post('api/stats/overview', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }

    this.getRelationStats = function (request, onFinally) {
        $http.post('api/stats/relation', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getIndexStats = function (request, onFinally) {
        $http.post('api/stats/index', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getStoredProcedureStats = function (request, onFinally) {
        $http.post('api/stats/stored-procedure', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getStatementsStats = function (request, onFinally) {
        $http.post('api/stats/statements', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getStatementStats = function (request, onFinally) {
        $http.post('api/stats/statement', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
}
Web.App.service('statisticsService', ['$http', Web.Services.StatisticsService]);