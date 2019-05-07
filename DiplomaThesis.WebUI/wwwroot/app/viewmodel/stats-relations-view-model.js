Web.ViewModels.StatsRelationsViewModel = function () {
    this.isLoading = true;
    this.isValid = false;
    this.dateFrom = null;
    this.dateTo = null;
    this.currentRelation = null;
    this.allRelations = [];
    this.graphData = null;
    this.graph = null;
    
    this.validate = function () {
        this.isValid = this.dateFrom != null && this.dateTo != null && this.currentRelation != null;
        return this.isValid;
    }
}