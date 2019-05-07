﻿Web.Services.EntitiesService = function ($http) {
    this.getAllDatabases = function (onFinally) {
        $http.get('api/entities/databases').then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getAllRelations = function (onFinally) {
        $http.get('api/entities/relations').then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
}
Web.App.service('entitiesService', ['$http', Web.Services.EntitiesService]);