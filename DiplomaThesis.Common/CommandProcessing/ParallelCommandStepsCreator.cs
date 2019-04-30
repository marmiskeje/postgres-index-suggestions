using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaThesis.Common.CommandProcessing
{
    public class ParallelCommandStepsCreator
    {
        private readonly int maxDegreeOfParallelism;
        private readonly List<IExecutableCommand> parallelSteps = new List<IExecutableCommand>();

        #region private class ParallelCommand : ChainableCommand
        private class ParallelCommand : ChainableCommand
        {
            private readonly IEnumerable<IExecutableCommand> parallelSteps;
            private readonly int maxDegreeOfParallelism;
            public ParallelCommand(IEnumerable<IExecutableCommand> parallelSteps, int maxDegreeOfParallelism)
            {
                this.parallelSteps = parallelSteps;
                this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            }

            protected override void OnExecute()
            {
                ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism };
                Parallel.ForEach(parallelSteps, options, x => x.Execute());
            }
        }
        #endregion

        public ParallelCommandStepsCreator() : this(Environment.ProcessorCount)
        {

        }

        public ParallelCommandStepsCreator(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
            {
                maxDegreeOfParallelism = 1;
            }
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }
        public void AddParallelStep(params IChainableCommand[] multileCommandsAsSingleStep)
        {
            CommandChainCreator chain = new CommandChainCreator();
            foreach (var c in multileCommandsAsSingleStep)
            {
                chain.Add(c);
            }
            parallelSteps.Add(chain.AsChainableCommand());
        }

        public IChainableCommand CreateParallelCommand()
        {
            return new ParallelCommand(parallelSteps, maxDegreeOfParallelism);
        }
    }
}
