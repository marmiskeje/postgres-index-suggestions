Web.Controllers.StatsOverviewController = function ($scope, $rootScope) {
    $rootScope.pageSubtitle = 'STATS_OVERVIEW.PAGE_SUBTITLE';
    $scope.viewModel = new Object();
    $scope.viewModel.selectedPeriod = 1;
    this.drawMostExecutedChart = function () {
        var ctx = document.getElementById('div-stats-most-executed-statements').getContext('2d');
        var chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['select * from test where x = 0...', 'select * from test2', 'select * from test3', 'select * from test4', 'select * from test5', 'select * from test6', 'select * from test7', '8', '9', '10'],
                datasets: [{
                    label: 'Execution count',
                    data: [
                        10, 30, 5, 3, 10, 100, 55, 13, 24, 28
                    ],
                    fill: false,
                    //backgroundColor: ["#e41a1c", "#377eb8", "#4daf4a", "#984ea3", "#ff7f00", "#efef33", "#a65628", "#f781bf", "#999999", "#7075bf"]
                    backgroundColor: ["#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8"]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    colorschemes: {
                        scheme: 'brewer.Set1-9'//'brewer.Paired12'
                    }
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            min: 0
                        }
                    }]
                },
                title: {
                    display: true,
                    text: 'TOP 10 most frequently executed statements',
                    fontSize: 16
                }
            }
        });
    }
    this.drawSlowestChart = function () {
        var ctx = document.getElementById('div-stats-slowest-statements').getContext('2d');
        var chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['select * from test where x = 0...', 'select * from test2', 'select * from test3', 'select * from test4', 'select * from test5', 'select * from test6', 'select * from test7', '8', '9', '10'],
                datasets: [{
                    label: 'Max duration (s)',
                    data: [
                        10, 30, 5, 3, 10, 100, 55, 13, 24, 28
                    ],
                    fill: false,
                    backgroundColor: ["#7075bf","#7075bf","#7075bf","#7075bf","#7075bf","#7075bf","#7075bf","#7075bf","#7075bf","#7075bf","#7075bf"]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    colorschemes: {
                        scheme: 'brewer.Set1-9'//'brewer.Paired12'
                    }
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            min: 0
                        }
                    }]
                },
                title: {
                    display: true,
                    text: 'TOP 10 slowest statements',
                    fontSize: 16
                }
            }
        });
    }
    this.drawMostAliveChart = function () {
        var ctx = document.getElementById('div-stats-most-alive-relations').getContext('2d');
        var chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['test', 'test', 'test', 'test', 'test', 'test', '7', '8', '9', '10'],
                datasets: [{
                    label: 'Liveness index',
                    data: [
                        10, 30, 5, 3, 10, 100, 55, 13, 24, 28
                    ],
                    fill: false,
                    backgroundColor: ["#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8", "#377eb8"]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    colorschemes: {
                        scheme: 'brewer.Set1-9'//'brewer.Paired12'
                    }
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            min: 0
                        }
                    }]
                },
                title: {
                    display: true,
                    text: 'TOP 10 alive relations',
                    fontSize: 16
                }
            }
        });
    }
    this.drawMostExecutedChart();
    this.drawSlowestChart();
    this.drawMostAliveChart();
}

angular.module('WebApp').controller('StatsOverviewController', ['$scope', '$rootScope', Web.Controllers.StatsOverviewController]);