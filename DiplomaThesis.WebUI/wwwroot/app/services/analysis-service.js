﻿Web.Services.AnalysisService = function ($http) {
    this.getUnusedObjects = function (request, onFinally) {
        $http.post('api/analysis/unused-objects', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getWorkloads = function (request, onFinally) {
        $http.post('api/analysis/workloads', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.createWorkload = function (workload, onFinally) {
        $http.post('api/analysis/workload-create', workload).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.deleteWorkload = function (request, onFinally) {
        $http.put('api/analysis/workload-delete', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getWorkloadAnalyses = function (request, onFinally) {
        $http.post('api/analysis/workload-analyses', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.createWorkloadAnalysis = function (workloadAnalysis, onFinally) {
        $http.post('api/analysis/workload-analysis-create', workloadAnalysis).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getWorkloadAnalysisDetail = function (request, onFinally) {
        $http.post('api/analysis/workload-analysis-detail', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getWorkloadAnalysisDetailForEnv = function (request, onFinally) {
        $http.post('api/analysis/workload-analysis-detail-env', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
    this.getWorkloadAnalysisDetailForBestEnv = function (request, onFinally) {
        $http.post('api/analysis/workload-analysis-detail-env-best', request).then(function (response) {
            onFinally(response);
        }, function (errorResponse) {
            onFinally(errorResponse);
        });
    }
}
Web.App.service('analysisService', ['$http', Web.Services.AnalysisService]);