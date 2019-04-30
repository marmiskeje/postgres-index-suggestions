using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class EnqueueCommand : ChainableCommand
    {
        private readonly ICommandProcessingQueue<IExecutableCommand> queue;
        public EnqueueCommand(ICommandProcessingQueue<IExecutableCommand> queue)
        {
            this.queue = queue;
        }
        protected override void OnExecute()
        {
            if (Successor != null && IsEnabledSuccessorCall)
            {
                var toEnqueue = Successor;
                queue.Enqueue(toEnqueue);
                SetSuccessor(null);
            }
        }
    }
}
