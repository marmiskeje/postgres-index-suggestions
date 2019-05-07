using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class StatsOverviewData
    {
        public List<ChartDataItem<string, long>> MostExecutedStatements { get; set; }
        public List<ChartDataItem<string, double>> MostSlowestStatements { get; set; }
        public List<ChartDataItem<string, long>> MostAliveRelations { get; set; }
    }
}
