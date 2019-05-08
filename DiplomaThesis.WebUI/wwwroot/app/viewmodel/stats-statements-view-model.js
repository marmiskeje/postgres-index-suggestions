Web.ViewModels.StatsStatementsViewModel = function () {
    this.isLoading = false;
    this.isValid = false;
    this.dateFrom = null;
    this.dateTo = null;
    this.currentCommandType = 0;
    this.data = null;
    
    this.validate = function () {
        this.isValid = this.dateFrom != null && this.dateTo != null && this.currentCommandType != null;
        return this.isValid;
    }
}