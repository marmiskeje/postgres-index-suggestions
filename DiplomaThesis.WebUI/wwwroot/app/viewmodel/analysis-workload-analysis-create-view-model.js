Web.ViewModels.AnalysisWorkloadAnalysisCreateViewModel = function () {
    this.isLoading = false;
    this.isValid = false;
    this.workloadAnalysis = null;
    this.allWorkloads = [];
    this.relationReplacements = [];
    this.sourceRelation = null;
    this.targetRelation = null;
    this.allAvailableRelations = [];
    this.isValidReplacement = false;
    
    this.validate = function () {
        this.isValid = this.workloadAnalysis != null && this.workloadAnalysis.workloadID != null && this.workloadAnalysis.workloadID > 0 && this.workloadAnalysis.periodFromDate != null && this.workloadAnalysis.periodToDate != null;
        return this.isValid;
    }

    this.validateReplacement = function () {
        this.isValidReplacement = this.sourceRelation != null && this.targetRelation != null && this.sourceRelation.id != this.targetRelation.id
            && this.relationReplacements.filter(x => {
                return (x.sourceRelation.id == this.sourceRelation.id && x.targetRelation.id == this.targetRelation.id)
                    || (x.targetRelation.id == this.sourceRelation.id && x.sourceRelation.id == this.targetRelation.id);
            }).length == 0;
        return this.isValidReplacement;
    }
}