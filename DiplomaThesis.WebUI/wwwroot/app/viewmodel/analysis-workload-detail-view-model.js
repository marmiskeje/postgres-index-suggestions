Web.ViewModels.AnalysisWorkloadDetailViewModel = function () {
    this.isLoading = false;
    this.isValid = false;
    this.isReadonly = false;
    this.workload = null;
    this.forbiddenRelations = [];
    this.forbiddenRelationToAdd = null;
    this.forbiddenRelationSearch = null;
    this.isValidForbiddenDateTimeSlot = false;
    this.forbiddenDateTimeSlotDay = null;
    this.forbiddenDateTimeSlotStartTime = null;
    this.forbiddenDateTimeSlotEndTime = null;
    this.days = [];
    
    this.validate = function () {
        this.isValid = this.workload != null && this.workload.name != null && this.workload.databaseID > 0
            && this.workload.definition != null && this.workload.definition.statementMinDurationInMs >= 0 && this.workload.definition.statementMinExectutionCount >= 0;
        return this.isValid;
    }

    this.validateForbiddenSlot = function () {
        this.isValidForbiddenDateTimeSlot = this.forbiddenDateTimeSlotDay != null && this.forbiddenDateTimeSlotStartTime != null && this.forbiddenDateTimeSlotEndTime != null
            && this.forbiddenDateTimeSlotStartTime < this.forbiddenDateTimeSlotEndTime;
        return this.isValidForbiddenDateTimeSlot;
    }
}