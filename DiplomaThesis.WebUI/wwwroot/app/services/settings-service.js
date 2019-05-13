Web.Services.SettingsService = function ($http) {
    this.getConfiguration = function (onFinally) {
        $http.get('api/settings/configuration').then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.updateConfiguration = function (configuration, onFinally) {
        $http.post('api/settings/update-configuration', configuration).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
}
Web.App.service('settingsService', ['$http', Web.Services.SettingsService]);