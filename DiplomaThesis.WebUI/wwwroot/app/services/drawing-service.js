Web.Services.DrawingService = function () {
    this.drawBarGraph = function (graphElementId, title, data) {
        var elem = document.getElementById(graphElementId);
        if (elem != null) {
            var ctx = elem.getContext('2d');
            return new Chart(ctx, {
                type: 'bar',
                data: data,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        colorschemes: {
                            scheme: 'brewer.Set1-9'//'brewer.Paired12'
                        }
                    },
                    scales: {
                        yAxes: [{
                            ticks: {
                                min: 0
                            }
                        }]
                    },
                    title: {
                        display: true,
                        text: title,
                        fontSize: 16
                    },
                    tooltips: {
                        callbacks: {
                            title: function (tooltipItems, data) {
                                return data.labels[tooltipItems[0].index]
                            }
                        }
                    }
                }
            });
        }
    }
    this.drawTimeLineGraph = function (graphElementId, title, data) {
        var elem = document.getElementById(graphElementId);
        if (elem != null) {
            var ctx = elem.getContext('2d');
            return new Chart(ctx, {
                type: 'line',
                data: data,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        colorschemes: {
                            scheme: 'brewer.Set1-9'//'brewer.Paired12'
                        }
                    },
                    scales: {
                        yAxes: [{
                            ticks: {
                                min: 0
                            }
                        }],
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
                    title: {
                        display: true,
                        text: title,
                        fontSize: 16
                    },
                    tooltips: {
                        callbacks: {
                            title: function (tooltipItems, data) {
                                return data.labels[tooltipItems[0].index]
                            }
                        }
                    }
                }
            });
        }
    }
}
Web.App.service('drawingService', [Web.Services.DrawingService]);