Web.ViewModels.StatsIndicesViewModel = function () {
    this.isLoading = false;
    this.isValid = false;
    this.dateFrom = null;
    this.dateTo = null;
    this.currentRelation = null;
    this.currentIndex = null;
    this.allRelations = [];
    this.allIndices = [];
    this.data = null;
    this.graph = null;
    
    this.validate = function () {
        this.isValid = this.dateFrom != null && this.dateTo != null && this.currentRelation != null && this.currentIndex != null;
        return this.isValid;
    }
}