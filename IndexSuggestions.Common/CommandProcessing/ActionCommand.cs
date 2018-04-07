using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.CommandProcessing
{
    public class ActionCommand : ChainableCommand
    {
        private readonly Action action;
        public ActionCommand(Action action)
        {
            this.action = action;
        }
        protected override void OnExecute()
        {
            action();
        }
    }
}
