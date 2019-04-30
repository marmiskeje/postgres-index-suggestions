using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.CommandProcessing
{
    /// <summary>
    /// Returns whether successor call should be allowed.
    /// </summary>
    public class ActionCommand : ChainableCommand
    {
        private readonly Func<bool> action;
        public ActionCommand(Func<bool> action)
        {
            this.action = action;
        }
        protected override void OnExecute()
        {
            IsEnabledSuccessorCall = action();
        }
    }
}
