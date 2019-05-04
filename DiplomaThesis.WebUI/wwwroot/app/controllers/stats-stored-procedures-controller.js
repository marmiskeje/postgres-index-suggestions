Web.Controllers.StatsStoredProceduresController = function ($scope, $rootScope, $http, uiGridConstants) {
    $rootScope.pageSubtitle = 'STATS_STORED_PROCEDURES.PAGE_SUBTITLE';
    $scope.viewModel = new Object();
    $scope.viewModel.isValid = true;
    $scope.viewModel.isLoading = false;
    $scope.dateFrom = moment().startOf('day');
    $scope.dateTo = moment().startOf('day').add(1, 'days');
    
    this.chart = null;
    this.drawChart = function() {
        var ctx = document.getElementById('div-stats-stored-procedures').getContext('2d');
        this.chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [new Date(2018, 1, 1), new Date(2018, 1, 2), new Date(2018, 1, 5)],
                datasets: [{
                    label: 'Calls count',
                    data: [
                        10, 30, 5,
                    ],
                    fill: false,
                }, {
                    label: 'Total duration (ms)',
                    fill: false,
                    data: [
                        500, 30, 5
                    ],
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
    this.drawChart();
}



angular.module('WebApp').controller('StatsStoredProceduresController', ['$scope', '$rootScope', '$http', 'uiGridConstants', Web.Controllers.StatsStoredProceduresController]);