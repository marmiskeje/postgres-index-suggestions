Web.Controllers.SettingsDetailController = function ($scope, $rootScope, $window, settingsService, notificationsService, $state) {
    $rootScope.pageSubtitle = 'SETTINGS_DETAIL.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
    $scope.actions.saveData = function () {
        $scope.viewModel.isUpdating = true;
        settingsService.updateConfiguration($scope.viewModel.configuration, function (response) {
            try {
                if (response.data == null) {
                    notificationsService.showError("An error occured during settings update.", response.status, response.statusText);
                }
                else if (response.data.isSuccess) {
                    notificationsService.showMessage("Settings succesfully updated.");
                }
                else {
                    notificationsService.showError("An error occured during settings update.", 500, response.data.errorMessage);
                }
            } finally {
                $scope.viewModel.isUpdating = false;
            }
        });
    };
    $scope.actions.loadData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            settingsService.getConfiguration(function (response) {
                try {
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
                } finally {
                    $scope.viewModel.isLoading = false;
                }
            });
        });
    };
    $scope.actions.clearData = function () {
        $scope.viewModel.configuration = null;
    }
    $scope.actions.refreshData = function (enforceLoading) {
        var loadData = new Promise(function (resolve, reject) {
            if (enforceLoading || $scope.viewModel.data == null) {
                $scope.actions.loadData().then(function (result) {
                    resolve(result);
                });
            }
            else {
                resolve($scope.viewModel.data)
            }
        });
        loadData.then(function (data) {
            $scope.actions.clearData();
            if (data != null) {
                $scope.viewModel.configuration = data;
                var loadedCollectorDatabases = {};
                for (var i = 0; i < data.collector.databases.length; i++) {
                    var db = data.collector.databases[i];
                    loadedCollectorDatabases[db.databaseID] = db;
                }
                $scope.viewModel.configuration.collector.databases = [];
                for (var i = 0; i < $rootScope.viewModel.allDatabases.length; i++) {
                    var db = $rootScope.viewModel.allDatabases[i];
                    var dbToAdd = { "databaseID": db.id, "name": db.name, "isEnabledGeneralCollection": false, "isEnabledStatementCollection": false };
                    if (db.id in loadedCollectorDatabases) {
                        dbToAdd.isEnabledGeneralCollection = loadedCollectorDatabases[db.id].isEnabledGeneralCollection;
                        dbToAdd.isEnabledStatementCollection = loadedCollectorDatabases[db.id].isEnabledStatementCollection;
                    }
                    $scope.viewModel.configuration.collector.databases.push(dbToAdd);
                }
                try {
                    $scope.viewModel.validate();
                    $scope.$apply();
                } catch (e) {
                    console.log(e.message);
                }
            }
        });
    }

    if ($state.current.data == null) {
        $scope.viewModel = new Web.ViewModels.SettingsDetailViewModel();
        $state.current.data = $scope.viewModel;
    } else {
        $scope.viewModel = $state.current.data;
    }
    $scope.actions.refreshData(false);
}

angular.module('WebApp').controller('SettingsDetailController', ['$scope', '$rootScope', '$window', 'settingsService', 'notificationsService', '$state', Web.Controllers.SettingsDetailController]);