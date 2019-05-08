Web.ViewModels.StatsStoredProceduresViewModel = function () {
    this.isLoading = false;
    this.isValid = false;
    this.dateFrom = null;
    this.dateTo = null;
    this.currentStoredProcedure = null;
    this.allStoredProcedures = [];
    this.data = null;
    this.graph = null;
    
    this.validate = function () {
        this.isValid = this.dateFrom != null && this.dateTo != null && this.currentStoredProcedure != null;
        return this.isValid;
    }
}