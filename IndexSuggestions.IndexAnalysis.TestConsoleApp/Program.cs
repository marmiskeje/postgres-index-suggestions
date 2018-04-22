using System;

namespace IndexSuggestions.IndexAnalysis.TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var designer = new IndexDesigner(DAL.RepositoriesFactory.Instance, DBMS.Postgres.RepositoriesFactory.Instance);
            var task = designer.Run(1);
            task.Wait();
            Console.WriteLine("Done. Press any key to continue.");
            Console.ReadLine();
        }
    }
}
