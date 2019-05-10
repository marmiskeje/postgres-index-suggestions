Web.ViewModels.AnalysisWorkloadsViewModel = function () {
    this.isLoading = false;
    this.isValid = false;
    this.dateFrom = null;
    this.dateTo = null;
    this.data = null;
    
    this.validate = function () {
        this.isValid = this.dateFrom != null && this.dateTo != null;
        return this.isValid;
    }
}