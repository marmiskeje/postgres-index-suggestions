using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.CommandProcessing
{
    public abstract class ChainableCommand : IChainableCommand
    {
        protected bool IsEnabledSuccessorCall { get; set; }

        public ChainableCommand()
        {
            IsEnabledSuccessorCall = true;
        }

        public IExecutableCommand Successor { get; private set; }

        protected abstract void OnExecute();

        public virtual void Execute()
        {
            OnExecute();
            TryCallSuccessor();
        }

        protected void TryCallSuccessor()
        {
            if ((Successor != null) && (IsEnabledSuccessorCall == true))
            {
                Successor.Execute();
            }
        }

        public void SetSuccessor(IExecutableCommand successor)
        {
            this.Successor = successor;
        }

    }
}
