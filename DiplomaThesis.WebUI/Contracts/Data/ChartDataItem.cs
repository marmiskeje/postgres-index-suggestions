using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class ChartDataItem<TIndependentValue, TDependentValue>
    {
        public TIndependentValue IndependentValue { get; set; }
        public TDependentValue DependentValue { get; set; }
    }
}
