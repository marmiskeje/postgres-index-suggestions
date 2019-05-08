Web.Controllers.RootController = function ($scope, $rootScope, $state, entitiesService, notificationsService) {
    $scope.actions.onDatabaseChanged = function () {
        $rootScope.$broadcast('onDatabaseChanged');
    }
    $scope.actions.loadDatabases = function () {
        return new Promise(function (resolve, reject) {
            entitiesService.getAllDatabases(function (response) {
                if (response.data == null) {
                    notificationsService.showError("An error occured during data loading.", response.status, response.statusText);
                    resolve(null);
                }
                else if (response.data.isSuccess && response.data.data != null) {
                    var databases = response.data.data;
                    resolve(databases);
                }
                else {
                    notificationsService.showError("An error occured during data loading.", 500, response.data.errorMessage);
                    resolve(null);
                }
            });
        });
    }
    $scope.actions.loadRelations = function () {
        return new Promise(function (resolve, reject) {
            entitiesService.getAllRelations(function (response) {
                if (response.data == null) {
                    notificationsService.showError("An error occured during data loading.", response.status, response.statusText);
                    resolve(null);
                }
                else if (response.data.isSuccess && response.data.data != null) {
                    var relationsPerDatabase = response.data.data;
                    resolve(relationsPerDatabase);
                }
                else {
                    notificationsService.showError("An error occured during data loading.", 500, response.data.errorMessage);
                    resolve(null);
                }
            });
        });
    }
    $scope.actions.loadIndices = function () {
        return new Promise(function (resolve, reject) {
            entitiesService.getAllIndices(function (response) {
                if (response.data == null) {
                    notificationsService.showError("An error occured during data loading.", response.status, response.statusText);
                    resolve(null);
                }
                else if (response.data.isSuccess && response.data.data != null) {
                    var indicesPerRelation = response.data.data;
                    resolve(indicesPerRelation);
                }
                else {
                    notificationsService.showError("An error occured during data loading.", 500, response.data.errorMessage);
                    resolve(null);
                }
            });
        });
    }
    $scope.actions.loadStoredProcedures = function () {
        return new Promise(function (resolve, reject) {
            entitiesService.getAllStoredProcedures(function (response) {
                if (response.data == null) {
                    notificationsService.showError("An error occured during data loading.", response.status, response.statusText);
                    resolve(null);
                }
                else if (response.data.isSuccess && response.data.data != null) {
                    var proceduresPerDatabase = response.data.data;
                    resolve(proceduresPerDatabase);
                }
                else {
                    notificationsService.showError("An error occured during data loading.", 500, response.data.errorMessage);
                    resolve(null);
                }
            });
        });
    }
    if ($state.current.data == null) {
        $state.current.data = $rootScope.viewModel;
        $scope.actions.loadDatabases().then(function (databases) {

            if (databases != null) {
                $rootScope.viewModel.allDatabases = [];
                for (var i = 0; i < databases.length; i++) {
                    $rootScope.viewModel.allDatabases.push(databases[i]);
                }
                if ($rootScope.viewModel.allDatabases.length > 0) {
                    $rootScope.viewModel.currentDatabase = $rootScope.viewModel.allDatabases[0];
                }

                $scope.actions.loadRelations().then(function (relationsPerDatabase) {
                    if (relationsPerDatabase) {
                        $rootScope.viewModel.databaseRelations = {};
                        for (var databaseId in relationsPerDatabase) {
                            $rootScope.viewModel.databaseRelations[databaseId] = [];
                            for (var i = 0; i < relationsPerDatabase[databaseId].length; i++) {
                                var relation = relationsPerDatabase[databaseId][i];
                                $rootScope.viewModel.databaseRelations[databaseId].push(relation);
                                $rootScope.viewModel.allRelations[relation.id] = relation;
                            }
                        }
                        $scope.actions.loadIndices().then(function (indicesPerRelation) {
                            if (indicesPerRelation) {
                                $rootScope.viewModel.relationIndices = {};
                                for (var relationId in indicesPerRelation) {
                                    $rootScope.viewModel.relationIndices[relationId] = [];
                                    for (var i = 0; i < indicesPerRelation[relationId].length; i++) {
                                        var index = indicesPerRelation[relationId][i];
                                        $rootScope.viewModel.relationIndices[relationId].push(index);
                                        $rootScope.viewModel.allIndices[index.id] = index;
                                    }
                                }
                                $scope.actions.loadStoredProcedures().then(function (proceduresPerDatabase) {
                                    if (proceduresPerDatabase) {
                                        $rootScope.viewModel.databaseStoredProcedures = {};
                                        for (var databaseId in proceduresPerDatabase) {
                                            $rootScope.viewModel.databaseStoredProcedures[databaseId] = [];
                                            for (var i = 0; i < proceduresPerDatabase[databaseId].length; i++) {
                                                $rootScope.viewModel.databaseStoredProcedures[databaseId].push(proceduresPerDatabase[databaseId][i]);
                                            }
                                        }
                                        $rootScope.viewModel.isLoading = false;
                                        $state.go(Web.Constants.StateNames.STATS_STATEMENTS);
                                    }
                                });
                            }
                        });
                    }
                });
            }
        });
    } else {
        $scope.viewModel = $state.current.data;
    }
}

angular.module('WebApp').controller('RootController', ['$scope', '$rootScope', '$state', 'entitiesService', 'notificationsService', Web.Controllers.RootController]);