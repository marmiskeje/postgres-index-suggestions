Web.Controllers.HelpDetailController = function ($scope, $rootScope, $window) {
    $rootScope.pageSubtitle = 'HELP_DETAIL.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
}

angular.module('WebApp').controller('HelpDetailController', ['$scope', '$rootScope', '$window', Web.Controllers.HelpDetailController]);