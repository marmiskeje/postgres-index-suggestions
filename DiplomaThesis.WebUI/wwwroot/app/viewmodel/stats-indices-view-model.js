Web.ViewModels.StatsRelationsViewModel = function () {
    this.isLoading = false;
    this.isValid = false;
    this.dateFrom = null;
    this.dateTo = null;
    this.currentRelation = null;
    this.allRelations = [];
    this.data = null;
    this.graph = null;
    
    this.validate = function () {
        this.isValid = this.dateFrom != null && this.dateTo != null && this.currentRelation != null;
        return this.isValid;
    }
}