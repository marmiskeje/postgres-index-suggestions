using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.CommandProcessing
{
    public class ActionDelegateCommand : ChainableCommand
    {
        private readonly Action action;
        public ActionDelegateCommand(Action action)
        {
            this.action = action;
        }
        protected override void OnExecute()
        {
            action();
        }
    }
}
