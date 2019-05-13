Web.ViewModels.RootViewModel = function () {
    this.isLoading = true;
    this.initializationProgress = 0;
    this.currentDatabase = null;
    this.allDatabases = [];
    this.databaseRelations = {};
    this.allRelations = {};
    this.relationIndices = {};
    this.allIndices = {};
    this.databaseStoredProcedures = {};
}