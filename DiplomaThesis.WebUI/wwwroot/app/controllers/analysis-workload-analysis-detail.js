Web.Controllers.AnalysisWorkloadAnalysisDetailController = function ($scope, $rootScope, $state, $stateParams, $window, analysisService, notificationsService, $mdDialog) {
    $rootScope.pageSubtitle = 'ANALYSIS_WORKLOAD_ANALYSIS_DETAIL.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSES);
    };
    $scope.actions.showHtmlContentDialog = function (ev, title, content) {
        $mdDialog.show(
            $mdDialog.alert()
                .parent(angular.element(document.querySelector('#pageBody')))
                .clickOutsideToClose(true)
                .title(title)
                .htmlContent(content)
                .ariaLabel('showHtmlContentDialog')
                .ok('Close')
                .targetEvent(ev)
        );
    };
    $scope.actions.onChangedHP = function () {
        if ($scope.viewModel.data.currentHP != null) {
            $scope.actions.loadEnvironmentData($scope.viewModel.data.currentHP.environmentID).then(function (data) {
                var statementsData = [];
                for (var statementID in data.evaluatedStatements) {
                    var evaluation = data.evaluatedStatements[statementID];
                    var statementData = data.statements[statementID];
                    var toAdd = new Object();
                    toAdd.statement = statementData.statement;
                    toAdd.totalExecutionsCount = statementData.totalExecutionsCount;
                    toAdd.realTotalCost = statementData.totalCost;
                    toAdd.potentialTotalCost = evaluation.totalCost;
                    toAdd.localImprovementRatio = $scope.actions.getPercentage(evaluation.localImprovementRatio);
                    toAdd.globalImprovementRatio = $scope.actions.getPercentage(evaluation.globalImprovementRatio);
                    statementsData.push(toAdd);
                }
                $scope.gridAnalysisHPStatements.data = statementsData;
                $scope.gridAnalysisHPStatementsApi.core.queueRefresh();
                $scope.gridAnalysisHPStatementsApi.core.handleWindowResize();
            });
        }
    }
    $scope.actions.showSelectEnvironmentDialog = function () {
        $state.go(Web.Constants.StateNames.ANALYSIS_WORKLOAD_ANALYSIS_DETAIL_SELECT_ENV, { workloadAnalysis: $scope.viewModel.workloadAnalysis, data: $scope.viewModel.data, currentEnvironmentID: $scope.viewModel.selectedIndicesEnvID });
    };
    $scope.actions.findBestEnvironment = function () {
        $scope.viewModel.selectedIndicesEnvID = -1;
        $scope.actions.refreshIndicesEnvironmentData($scope.viewModel.selectedIndicesEnvID);
    };
    $scope.gridAnalysisIndicesAffectedStatements = {
        enableFiltering: true,
        minRowsToShow: 10,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [],
        onRegisterApi: function (gridApi) {
            $scope.gridAnalysisIndicesAffectedStatementsApi = gridApi;
        }
    };
    $scope.gridAnalysisHPStatements = {
        enableFiltering: true,
        minRowsToShow: 8,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Statement', displayName: 'Statement', field: 'statement', minWidth: 200
            },
            {
                name: 'ExecutionCount', displayName: 'Executions count', field: 'totalExecutionsCount', maxWidth: 160, enableFiltering: false
            },
            {
                name: 'RealTotalCost', displayName: 'Real total cost', field: 'realTotalCost', maxWidth: 160, enableFiltering: false
            },
            {
                name: 'PotentialTotalCost', displayName: 'Potential total cost', field: 'potentialTotalCost', maxWidth: 170, enableFiltering: false
            },
            {
                name: 'LocalImprovement', displayName: 'Local % ↑', field: 'localImprovementRatio', maxWidth: 100, headerTooltip: '% improvement of statement execution price', enableFiltering: false
            },
            {
                name: 'GlobalImprovement', displayName: 'Global % ↑', field: 'globalImprovementRatio', maxWidth: 100, headerTooltip: '% improvement towards others', enableFiltering: false
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridAnalysisHPStatementsApi = gridApi;
        }
    };
    $scope.gridAnalysisIndicesPossibleIndices = {
        minRowsToShow: 5,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Relation', displayName: 'Relation', field: 'relationName'
            },
            {
                name: 'Name', displayName: 'Name', field: 'name'
            },
            {
                name: 'Size', displayName: 'Potential size', field: 'size'
            },
            {
                name: 'GlobalImpact', displayName: 'Global % impact', field: 'improvementRatio', headerTooltip: '% impact considering ALL statements', maxWidth: 180
            },
            {
                name: 'Possibilities', displayName: '', field: 'Possibilities', maxWidth: 100, enableSorting: false, enableHiding: false,
                cellTemplate: '<div class="ui-grid-cell-contents" style="text-align: center">' +
                    '<md-button ng-click="grid.appScope.actions.showHtmlContentDialog($event, \'Index CREATE definition\', row.entity.createDefinition)" style="padding: 0; margin: 0; min-height: inherit; min-width: inherit">' +
                    '<md-tooltip>Show CREATE definition</md-tooltip><i class="material-icons md-18">subject</i></md-button>' +
                    '<md-button ng-click="grid.appScope.actions.showHtmlContentDialog($event, \'Possible index filters\', row.entity.filtersContent)" ng-hide="!row.entity.hasFilters" style="margin-left: 7px;margin-right: 0;margin-top: 0;margin-bottom: 0; padding: 0; min-height: inherit; min-width: inherit">' +
                    '<md-tooltip>Show possible filters</md-tooltip><i class="material-icons md-18">filter_list</i></md-button>'
                    + '</div>'
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridAnalysisIndicesPossibleIndicesApi = gridApi;
        }
    };
    $scope.gridAnalysisIndicesNotAffectedStatements = {
        enableFiltering: true,
        minRowsToShow: 10,
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Statement', displayName: 'Statement', field: 'statement', minWidth: 200
            },
            {
                name: 'ExecutionCount', displayName: 'Executions count', field: 'totalExecutionsCount', maxWidth: 160, enableFiltering: false
            },
            {
                name: 'RealTotalCost', displayName: 'Real total cost', field: 'realTotalCost', maxWidth: 160, enableFiltering: false
            }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridAnalysisIndicesNotAffectedStatementsApi = gridApi;
        }
    }
    $scope.actions.loadMainData = function () {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoading = true;
            var request = new Object();
            request.workloadAnalysisID = $scope.viewModel.workloadAnalysis.id;
            analysisService.getWorkloadAnalysisDetail(request, function (response) {
                $scope.viewModel.isLoading = false;
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
            });
        });
    };
    $scope.actions.refreshMainData = function () {
        var loadData = new Promise(function (resolve, reject) {
            if ($scope.viewModel.data != null) {
                resolve($scope.viewModel.data);
            }
            else {
                $scope.actions.loadMainData().then(function (result) {
                    resolve(result);
                });
            }
        });
        loadData.then(function (data) {
            if (data != null) {
                $scope.viewModel.data = data;
                $scope.actions.refreshIndicesEnvironmentData($scope.viewModel.selectedIndicesEnvID);
                if ($scope.viewModel.data.isHPartitioningInitialized == null) {
                    var allHPs = [];
                    for (var envID in $scope.viewModel.data.hPartitioningsEnvironments) {
                        var env = $scope.viewModel.data.hPartitioningsEnvironments[envID];
                        for (var pID in env.hPartitionings) {
                            var p = env.hPartitionings[pID];
                            p.environmentID = envID;
                            var relation = null;
                            if (p.relationID in $rootScope.viewModel.allRelations) {
                                relation = $rootScope.viewModel.allRelations[p.relationID];
                            }
                            else {
                                relation = { fullName: p.relationID };
                            }
                            p.partitioningStatementStr = relation.fullName + " " + p.partitioningStatement;
                            p.partitionStatementsStr = p.partitionStatements.join("\r\n");
                            p.impactStr = $scope.actions.getPercentage(p.improvementRatio);
                            allHPs.push(p);
                            break;
                        }
                    }
                    $scope.viewModel.data.allHPs = allHPs;
                    $scope.viewModel.data.currentHP = null;
                    if ($scope.viewModel.data.allHPs.length > 0) {
                        $scope.viewModel.data.currentHP = $scope.viewModel.data.allHPs[0];
                    }
                    $scope.actions.onChangedHP();
                }
            }
        });
    }
    $scope.actions.loadEnvironmentData = function (environmentID) {
        return new Promise(function (resolve, reject) {
            $scope.viewModel.isLoadingEnv = true;
            var request = new Object();
            request.workloadAnalysisID = $scope.viewModel.workloadAnalysis.id;
            request.environmentID = environmentID;
            var usedCall = analysisService.getWorkloadAnalysisDetailForEnv;
            if (environmentID == null ||environmentID == -1) {
                usedCall = analysisService.getWorkloadAnalysisDetailForBestEnv;
            }
            usedCall(request, function (response) {
                $scope.viewModel.isLoadingEnv = false;
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
            });
        });
    };
    $scope.actions.refreshIndicesEnvironmentData = function (environmentID) {
        var loadData = new Promise(function (resolve, reject) {
            $scope.actions.loadEnvironmentData(environmentID).then(function (result) {
                resolve(result);
            });
        });
        loadData.then(function (data) {
            if (data != null) {
                $scope.viewModel.indicesEnvData = data;
                if (data.environmentID in $scope.viewModel.data.indicesEnvironments) {
                    $scope.viewModel.selectedIndicesEnv = $scope.viewModel.data.indicesEnvironments[data.environmentID];
                    console.log("Loaded env: " + data.environmentID);
                    var indicesData = [];
                    for (var indexID in $scope.viewModel.selectedIndicesEnv.indices) {
                        var index = $scope.viewModel.selectedIndicesEnv.indices[indexID];
                        var indexExtended = $scope.viewModel.data.indices[indexID];
                        var relationName = "Unknown";
                        if (indexExtended.relationID in $rootScope.viewModel.allRelations) {
                            relationName = $rootScope.viewModel.allRelations[indexExtended.relationID].fullName;
                        }
                        var toAdd = new Object();
                        toAdd.relationName = relationName;
                        toAdd.name = indexExtended.name;
                        toAdd.size = $scope.actions.getSizeString(indexExtended.size);
                        toAdd.improvementRatio = $scope.actions.getPercentage(index.improvementRatio);
                        toAdd.createDefinition = indexExtended.createDefinition;
                        toAdd.hasFilters = Object.keys(indexExtended.filters).length > 0;
                        if (Object.keys(indexExtended.filters).length > 0) {
                            toAdd.filtersContent = "<table><tr><th>Filter</th><th>Size</th></tr>";
                            for (var filter in indexExtended.filters) {
                                toAdd.filtersContent += "<tr><td>" + filter + "</td><td>" + $scope.actions.getSizeString(indexExtended.filters[filter]) + "</td></tr>";
                            }
                            toAdd.filtersContent += "</table>";
                        }
                        indicesData.push(toAdd);
                    }
                    $scope.gridAnalysisIndicesPossibleIndices.data = indicesData;
                    $scope.gridAnalysisIndicesPossibleIndicesApi.core.queueRefresh();
                    $scope.gridAnalysisIndicesPossibleIndicesApi.core.handleWindowResize();
                    var notAffectedStatementsData = [];
                    var affectedStatementsData = [];
                    var affectedStatementsColumns = [];
                    affectedStatementsColumns.push({
                        name: 'Statement', displayName: 'Statement', field: 'statement', minWidth: 250
                    });
                    for (var indexID in $scope.viewModel.selectedIndicesEnv.indices) {
                        var indexExtended = $scope.viewModel.data.indices[indexID];
                        affectedStatementsColumns.push({
                            name: 'Index' + indexID, displayName: indexExtended.name, field: 'index' + indexID, minWidth: 160, enableFiltering: false,
                            headerTooltip: indexExtended.name,
                            cellTemplate: '<div class="ui-grid-cell-contents" style="text-align: center">' +
                                '<md-checkbox aria-label="is-used" disabled ng-checked="{{row.entity.index' + indexID + '.isUsed}}">{{row.entity.index' + indexID + '.isCovering ? "COVERING!" : ""}}</md-checkbox>' +
                                '<md-tooltip md-direction="top">{{row.entity.index' + indexID + '.isUsed ? "Used" : "Not used"}}</md-tooltip></div>'
                        });
                    }
                    affectedStatementsColumns.push({
                        name: 'ExecutionCount', displayName: 'Executions count', field: 'totalExecutionsCount', maxWidth: 160, enableFiltering: false
                    });
                    affectedStatementsColumns.push({
                        name: 'RealTotalCost', displayName: 'Real total cost', field: 'realTotalCost', maxWidth: 160, enableFiltering: false
                    });
                    affectedStatementsColumns.push({
                        name: 'PotentialTotalCost', displayName: 'Potential total cost', field: 'potentialTotalCost', maxWidth: 170, enableFiltering: false
                    });
                    affectedStatementsColumns.push({
                        name: 'LocalImprovement', displayName: 'Local % ↑', field: 'localImprovementRatio', maxWidth: 100, headerTooltip: '% improvement of statement execution price', enableFiltering: false
                    });
                    affectedStatementsColumns.push({
                        name: 'GlobalImprovement', displayName: 'Global % ↑', field: 'globalImprovementRatio', maxWidth: 100, headerTooltip: '% improvement towards others', enableFiltering: false
                    });
                    for (var statementID in $scope.viewModel.indicesEnvData.statements) {
                        var statementData = $scope.viewModel.indicesEnvData.statements[statementID];
                        var toAdd = new Object();
                        toAdd.statement = statementData.statement;
                        toAdd.totalExecutionsCount = statementData.totalExecutionsCount;
                        toAdd.realTotalCost = statementData.totalCost;
                        if (statementID in $scope.viewModel.indicesEnvData.evaluatedStatements && $scope.viewModel.indicesEnvData.evaluatedStatements[statementID].affectingIndices.length > 0) {
                            var evaluation = $scope.viewModel.indicesEnvData.evaluatedStatements[statementID];
                            toAdd.potentialTotalCost = evaluation.totalCost;
                            toAdd.localImprovementRatio = $scope.actions.getPercentage(evaluation.localImprovementRatio);
                            toAdd.globalImprovementRatio = $scope.actions.getPercentage(evaluation.globalImprovementRatio);
                            for (var indexID in $scope.viewModel.selectedIndicesEnv.indices) {
                                toAdd["index" + indexID] = {};
                                toAdd["index" + indexID].isUsed = evaluation.usedIndices.includes(indexID - 0);
                                toAdd["index" + indexID].isCovering = evaluation.coveringIndices.includes(indexID - 0);
                            }
                            affectedStatementsData.push(toAdd);
                        } else {
                            notAffectedStatementsData.push(toAdd);
                        }
                    }
                    $scope.gridAnalysisIndicesAffectedStatements.columnDefs = affectedStatementsColumns;
                    $scope.gridAnalysisIndicesAffectedStatements.data = affectedStatementsData;
                    $scope.gridAnalysisIndicesAffectedStatementsApi.core.queueRefresh();
                    $scope.gridAnalysisIndicesAffectedStatementsApi.core.handleWindowResize();
                    $scope.gridAnalysisIndicesNotAffectedStatements.data = notAffectedStatementsData;
                    $scope.gridAnalysisIndicesNotAffectedStatementsApi.core.queueRefresh();
                    $scope.gridAnalysisIndicesNotAffectedStatementsApi.core.handleWindowResize();
                }
            }
        });
    };
    $scope.actions.getPercentage = function (val) {
        return (val * 100.0).toFixed(2) + " %";
    }
    $scope.actions.getSizeString = function (size) {
        var unit = "B";
        var newSize = size;
        if (newSize > 1024) {
            newSize = newSize / 1024.0;
            unit = "KB";
        }
        if (newSize > 1024) {
            newSize = newSize / 1024.0;
            unit = "MB";
        }
        if (newSize > 1024) {
            newSize = newSize / 1024.0;
            unit = "GB";
        }
        if (newSize > 1024) {
            newSize = newSize / 1024.0;
            unit = "TB";
        }
        return newSize.toFixed(2) + " " + unit;
    }
    $scope.viewModel = new Web.ViewModels.AnalysisWorkloadAnalysisDetailViewModel();
    $scope.viewModel.workloadAnalysis = $stateParams.workloadAnalysis;
    $scope.viewModel.data = $stateParams.data;
    $scope.viewModel.selectedIndicesEnvID = $stateParams.selectedEnvironmentID;
    $scope.actions.refreshMainData();
}

angular.module('WebApp').controller('AnalysisWorkloadAnalysisDetailController', ['$scope', '$rootScope', '$state', '$stateParams', '$window', 'analysisService', 'notificationsService', '$mdDialog', Web.Controllers.AnalysisWorkloadAnalysisDetailController]);