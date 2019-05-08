Web.ViewModels.RootViewModel = function () {
    this.isLoading = true;
    this.currentDatabase = null;
    this.allDatabases = [];
    this.databaseRelations = {};
    this.allRelations = {};
    this.relationIndices = {};
    this.allIndices = {};
    this.databaseStoredProcedures = {};
}