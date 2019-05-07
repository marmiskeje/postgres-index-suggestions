Web.Services.NotificationsService = function ($mdToast) {
    this.hideDelay = 3000;
    this.showError = function (friendlyMessage, statusCode, serverMessage) {
        var text = friendlyMessage;
        if (statusCode) {
            text += ' Status code: ' + statusCode + ' (' + serverMessage + ').';
        }
        $mdToast.show(
            $mdToast.simple()
            .textContent(text)
            .hideDelay(this.hideDelay)
        );
    }
}
Web.App.service('notificationsService', ['$mdToast', Web.Services.NotificationsService]);