﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.CommandProcessing
{
    public static class ExtensionsIExecutableCommand
    {
        public static IChainableCommand AsChainableCommand(this IExecutableCommand command)
        {
            return new ActionCommand(() => { command?.Execute(); return true; });
        }
    }
}
