using DiplomaThesis.Collector.Tests.Scenarios;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DiplomaThesis.Collector.InfiniteTrafficApp
{
    class Program
    {
        private readonly static List<Action> actions = new List<Action>();
        private readonly static Random rnd = new Random();

        static void Main(string[] args)
        {
            var scenarios = new ScenariosStatements();
            actions.Add(scenarios.Scenario_Base1);
            actions.Add(scenarios.Scenario_Base2);
            actions.Add(scenarios.Scenario_Base3);
            actions.Add(scenarios.Scenario_Base4);
            actions.Add(scenarios.Scenario_Base5);
            actions.Add(scenarios.Scenario_Base6);
            actions.Add(scenarios.Scenario_Base7);
            actions.Add(scenarios.Scenario_Base8);
            Console.WriteLine("Infinite traffic is running. Press any key to exit...");
            using (var timer = new Timer(Timer_Elapsed, null, 0, 60000))
            {
                Console.ReadLine();
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private static void Timer_Elapsed(object obj)
        {
            try
            {
                Console.WriteLine("CYCLE " + DateTime.Now);
                var countOfActions = rnd.Next(1, actions.Count + 1);
                for (int i = 0; i < countOfActions; i++)
                {
                    var actionIndex = rnd.Next(0, actions.Count);
                    var execCount = rnd.Next(1, 6);
                    for (int y = 0; y < execCount; y++)
                    {
                        actions[actionIndex].Invoke();
                    }
                }
                Console.WriteLine("------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
