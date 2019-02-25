using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NFine.Web.Models
{
    public class PayOrderReportViewModel
    {
        public int serial { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public decimal money { get; set; }
    }
}