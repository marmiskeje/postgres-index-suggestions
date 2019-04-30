using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.CommandProcessing
{
    public class HandleExceptionCommand : ChainableCommand
    {
        private readonly Action<Exception> onExceptionAction;
        private readonly Action finallyAction;
        public HandleExceptionCommand(Action<Exception> onExceptionAction = null, Action finallyAction = null)
        {
            this.onExceptionAction = onExceptionAction;
            this.finallyAction = finallyAction;
        }
        protected override void OnExecute()
        {
            try
            {
                var successor = Successor;
                SetSuccessor(null);
                if (successor != null)
                {
                    successor.Execute();
                }
            }
            catch (Exception ex)
            {
                onExceptionAction?.Invoke(ex);
            }
            finally
            {
                finallyAction?.Invoke();
            }
        }
    }
}
