﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class VirtualEnvironmentPossibleIndex
    {
        [Required]
        public long VirtualEnvironemntID { get; set; }
        public VirtualEnvironment VirtualEnvironment { get; set; }
        [Required]
        public long PossibleIndexID { get; set; }
        public PossibleIndex PossibleIndex { get; set; }
    }
}
