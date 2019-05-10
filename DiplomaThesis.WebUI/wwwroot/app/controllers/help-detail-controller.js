Web.Controllers.HelpDetailController = function ($scope, $rootScope) {
    $rootScope.pageSubtitle = 'HELP_DETAIL.PAGE_SUBTITLE';
    $scope.actions = new Object();
    $scope.actions.goBack = function () {
        $window.history.back();
    };
}

angular.module('WebApp').controller('HelpDetailController', ['$scope', '$rootScope', Web.Controllers.HelpDetailController]);