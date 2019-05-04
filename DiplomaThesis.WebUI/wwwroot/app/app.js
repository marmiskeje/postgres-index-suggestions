﻿var Web = Web || {};
Web.createNamespace = function (namespace) {
    var parts = namespace.split(".");
    var parent = Web;

    if (parts[0] === "Web") {
        parts = parts.slice(1);
    }

    for (var i = 0; i < parts.length; i++) {
        var partname = parts[i];
        if (typeof parent[partname] === "undefined") {
            parent[partname] = {};
        }
        parent = parent[partname];
    }
    return parent;
};
Web.createNamespace("Web.Constants");
Web.createNamespace("Web.Controllers");
Web.createNamespace("Web.Data");
Web.createNamespace("Web.Directives");
Web.createNamespace("Web.Services");
Web.createNamespace("Web.ViewModels");

Web.App = angular.module('WebApp', ['ui.router', 'ngCookies', 'ngSanitize', 'pascalprecht.translate', 'LocalStorageModule', 'ngMaterial', 'ngMessages', 'ngMaterialDatePicker', 'ngTouch', 'ui.grid', 'ui.grid.pagination', 'ui.grid.resizeColumns']);

Web.App.config(['$stateProvider', '$urlRouterProvider', '$translateProvider', '$httpProvider', '$mdThemingProvider', function ($stateProvider, $urlRouterProvider, $translateProvider, $httpProvider, $mdThemingProvider) {
    $urlRouterProvider.otherwise(Web.Constants.StateNames.IMPLICIT);
    
    $stateProvider.state(Web.Constants.StateNames.STATS_OVERVIEW, {
        url: "/stats-overview",
        controller: "StatsOverviewController",
        templateUrl: "/app/views/stats-overview.html",
    });
    $stateProvider.state(Web.Constants.StateNames.STATS_RELATIONS, {
        url: "/stats-relations",
        controller: "StatsRelationsController",
        templateUrl: "/app/views/stats-relations.html",
    });
    $stateProvider.state(Web.Constants.StateNames.STATS_INDICES, {
        url: "/stats-indices",
        controller: "StatsIndicesController",
        templateUrl: "/app/views/stats-indices.html",
    });
    $stateProvider.state(Web.Constants.StateNames.STATS_STORED_PROCEDURES, {
        url: "/stats-stored-procedures",
        controller: "StatsStoredProceduresController",
        templateUrl: "/app/views/stats-stored-procedures.html",
    });
    $stateProvider.state(Web.Constants.StateNames.STATS_STATEMENTS, {
        url: "/stats-statements",
        controller: "StatsStatementsController",
        templateUrl: "/app/views/stats-statements.html",
    });
    $stateProvider.state(Web.Constants.StateNames.STATS_STATEMENT_DETAIL, {
        url: "/stats-statement-detail",
        controller: "StatsStatementDetailController",
        templateUrl: "/app/views/stats-statement-detail.html",
        params: { statementId: null, dateFrom: null, dateTo: null}
    });
    $stateProvider.state(Web.Constants.StateNames.ANALYSIS_WORKLOADS, {
        url: "/analysis-workloads",
        controller: "AnalysisWorkloadsController",
        templateUrl: "/app/views/analysis-workloads.html",
    });
    $stateProvider.state(Web.Constants.StateNames.ANALYSIS_WORKLOAD_DETAIL, {
        url: "/analysis-workload-detail",
        controller: "AnalysisWorkloadDetailController",
        templateUrl: "/app/views/analysis-workload-detail.html",
        params: { workload: null }
    });
    $stateProvider.state(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSES, {
        url: "/analysis-workload-analyses",
        controller: "AnalysisWorkloadAnalysesController",
        templateUrl: "/app/views/analysis-workload-analyses.html"
    });
    $stateProvider.state(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_CREATE, {
        url: "/analysis-workload-analysis-create",
        controller: "AnalysisWorkloadAnalysisCreateController",
        templateUrl: "/app/views/analysis-workload-analysis-create.html"
    });
    $stateProvider.state(Web.Constants.StateNames.ANALYSIS_UNUSED_DB_OBJECTS, {
        url: "/analysis-unused-db-objects",
        controller: "AnalysisUnusedDbObjectsController",
        templateUrl: "/app/views/analysis-unused-db-objects.html",
    });
    
    $translateProvider.useStaticFilesLoader({
        prefix: 'app/lang/locale-',
        suffix: '.json'
    });
    $translateProvider.preferredLanguage('en').useSanitizeValueStrategy('sanitizeParameters');

    $mdThemingProvider.theme('default')
        .primaryPalette('blue', { 'default': '700' })
        .accentPalette('pink')
        .warnPalette('red')
        .backgroundPalette('grey');
}]);

Web.App.run(['$rootScope', '$state', '$translate', function ($rootScope, $state, $translate) {
    $rootScope.services = new Object();
    $rootScope.services.state = $state;
    $rootScope.actions = new Object();;
    $rootScope.viewModel = Object();
    var db = new Object();
    db.id = 1;
    db.name = "TestDatabase";
    $rootScope.viewModel.allDatabases = [];
    $rootScope.viewModel.allDatabases.push(db);
    var db2 = new Object();
    db2.id = 2;
    db2.name = "TestDatabase2";
    $rootScope.viewModel.allDatabases.push(db2);
    $rootScope.viewModel.currentDatabase = db;
}]);