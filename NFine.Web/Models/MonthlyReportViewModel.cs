using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NFine.Web.Models
{
    public class MonthlyReportViewModel
    {
        public decimal[] data { get; set; }
        public Record[] record { get; set; }
        public class Record
        {
            public int serial { get; set; }
            public string name { get; set; }
            public decimal sumcost { get; set; }
        }
    }
}