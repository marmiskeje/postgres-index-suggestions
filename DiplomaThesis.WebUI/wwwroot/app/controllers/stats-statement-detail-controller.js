Web.Controllers.StatsStatementDetailController = function ($scope, $rootScope, $http, uiGridConstants, $stateParams, $window) {
    $rootScope.pageSubtitle = 'STATS_RELATIONS.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
    $scope.viewModel = new Object();
    $scope.viewModel.isValid = true;
    $scope.viewModel.isLoading = false;
    $scope.viewModel.selectedRelationsUsageFilter = 0;
    $scope.viewModel.selectedIndicesUsageFilter = 0;
    if ($stateParams.dateFrom && $stateParams.dateTo) {
        $scope.viewModel.dateFrom = $stateParams.dateFrom;
        $scope.viewModel.dateTo = $stateParams.dateTo;
    }
    else {
        $scope.viewModel.dateFrom = moment().startOf('day');
        $scope.viewModel.dateTo = moment().startOf('day').add(1, 'days');
    }
    $scope.gridStatsSlowestStatements = {
        enablePaginationControls: false,
        useExternalPagination: false,
        useExternalSorting: false,
        columnDefs: [
            {
                name: 'Statement', displayName: 'Statement', enableSorting: false, field: 'Statement', enableHiding: false
            },
            { name: 'MaxDuration', displayName: 'Max duration (s)', maxWidth: 210, field: 'MaxDuration', enableSorting: false, enableHiding: false }
        ],
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
        }
    };
    var getPage = function () {
        var data = [];
        for (var i = 0; i < 10; i++) {
            var toAdd = new Object();
            toAdd.Id = i;
            toAdd.Statement = "Select * from test" + i;
            toAdd.MaxDuration = i * 2;
            data.push(toAdd);
        }
        $scope.gridStatsSlowestStatements.totalItems = 100;
        $scope.gridStatsSlowestStatements.data = data;
    }
    getPage();
    this.drawExecutionTimelineChart = function () {
        var ctx = document.getElementById('div-stats-statement-exec-timeline').getContext('2d');
        var chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [new Date(2018, 1, 1), new Date(2018, 1, 2), new Date(2018, 1, 5)],
                datasets: [{
                    label: 'Executions count',
                    data: [
                        10, 30, 5,
                    ],
                    fill: false,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        type: 'time',
                        time: {
                            minUnit: 'minute',
                            displayFormats: {
                                minute: 'MMM D h:mm a',
                                hour: 'MMM D h:mm a'
                            }
                        },
                        ticks: {
                            autoSkip: true,
                            maxRotation: 75,
                            minRotation: 45
                        }
                    }]
                },
                plugins: {
                    colorschemes: {
                        scheme: 'brewer.Set1-9'//'brewer.Paired12'
                    }
                }
            }
        });
    }
    this.drawExecutionDurationChart = function () {
        var ctx = document.getElementById('div-stats-statement-duration-timeline').getContext('2d');
        var chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [new Date(2018, 1, 1), new Date(2018, 1, 2), new Date(2018, 1, 5)],
                datasets: [{
                    label: 'Min duration (s)',
                    data: [
                        10, 30, 5,
                    ],
                    fill: false,
                }, {
                        label: 'Max duration (s)',
                        data: [
                            20, 40, 10,
                        ],
                        fill: false,
                    },
                    {
                        label: 'Avg duration (s)',
                        data: [
                            15, 33.3, 7.7,
                        ],
                        fill: false,
                    }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        type: 'time',
                        time: {
                            minUnit: 'minute',
                            displayFormats: {
                                minute: 'MMM D h:mm a',
                                hour: 'MMM D h:mm a'
                            }
                        }, 
                        ticks: {
                            autoSkip: true,
                            maxRotation: 75,
                            minRotation: 45
                        }
                    }]
                },
                plugins: {
                    colorschemes: {
                        scheme: 'brewer.Set1-9'//'brewer.Paired12'
                    }
                }
            }
        });
    }
    this.drawRelationsUsageChart = function () {
        var ctx = document.getElementById('div-stats-statement-relations').getContext('2d');
        var chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [new Date(2018, 1, 1), new Date(2018, 1, 2), new Date(2018, 1, 5)],
                datasets: [{
                    label: 'test.relation',
                    data: [
                        10, 30, 5,
                    ],
                    fill: false,
                }, {
                    label: 'test.relation2',
                    data: [
                        20, 40, 10,
                    ],
                    fill: false,
                },
                {
                    label: 'test.relation3',
                    data: [
                        15, 33.3, 7.7,
                    ],
                    fill: false,
                }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        type: 'time',
                        time: {
                            minUnit: 'minute',
                            displayFormats: {
                                minute: 'MMM D h:mm a',
                                hour: 'MMM D h:mm a'
                            }
                        },
                        ticks: {
                            autoSkip: true,
                            maxRotation: 75,
                            minRotation: 45
                        }
                    }]
                },
                plugins: {
                    colorschemes: {
                        scheme: 'brewer.Set1-9'//'brewer.Paired12'
                    }
                }
            }
        });
    }
    this.drawIndicesUsageChart = function () {
        var ctx = document.getElementById('div-stats-statement-indices').getContext('2d');
        var chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [new Date(2018, 1, 1), new Date(2018, 1, 2), new Date(2018, 1, 5)],
                datasets: [{
                    label: 'idx_test',
                    data: [
                        10, 30, 5,
                    ],
                    fill: false,
                }, {
                        label: 'idx_test2',
                    data: [
                        20, 40, 10,
                    ],
                    fill: false,
                },
                {
                    label: 'idx_test3',
                    data: [
                        15, 33.3, 7.7,
                    ],
                    fill: false,
                }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        type: 'time',
                        time: {
                            minUnit: 'minute',
                            displayFormats: {
                                minute: 'MMM D h:mm a',
                                hour: 'MMM D h:mm a'
                            }
                        },
                        ticks: {
                            autoSkip: true,
                            maxRotation: 75,
                            minRotation: 45
                        }
                    }]
                },
                plugins: {
                    colorschemes: {
                        scheme: 'brewer.Set1-9'//'brewer.Paired12'
                    }
                }
            }
        });
    }
    this.drawExecutionTimelineChart();
    this.drawExecutionDurationChart();
    this.drawRelationsUsageChart();
    this.drawIndicesUsageChart();
}



angular.module('WebApp').controller('StatsStatementDetailController', ['$scope', '$rootScope', '$http', 'uiGridConstants', '$stateParams', '$window', Web.Controllers.StatsStatementDetailController]);