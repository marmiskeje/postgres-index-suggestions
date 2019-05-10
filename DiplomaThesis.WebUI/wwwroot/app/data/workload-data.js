Web.Data.WorkloadData = function () {
    this.name = null;
    this.databaseID = null;
    this.definition = new Web.Data.WorkloadDefinitionData();
}

Web.Data.WorkloadDefinitionData = function () {
    this.forbiddenUsers = [];
    this.forbiddenRelations = [];
    this.forbiddenApplications = [];
    this.forbiddenDateTimeSlots = [];
    this.statementMinDurationInMs = 0;
    this.statementMinExectutionCount = 0;
}

Web.Data.WorkloadTimeSlotData = function () {
    this.startTime = null;
    this.endTime = null;
    this.dayOfWeek = null;
}