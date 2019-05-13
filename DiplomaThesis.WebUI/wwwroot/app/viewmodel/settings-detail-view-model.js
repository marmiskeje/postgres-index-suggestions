Web.ViewModels.SettingsDetailViewModel = function () {
    this.isLoading = false;
    this.isUpdating = false;
    this.isValid = false;
    this.configuration = null;

    this.validate = function () {
        this.isValid = this.configuration != null;
        return this.isValid;
    }
}