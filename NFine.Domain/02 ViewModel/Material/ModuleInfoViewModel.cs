using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.ViewModel.Material
{
    public class ModuleInfoViewModel
    {
        public string ICCID { get; set; }
        public string IMSI { get; set; }
        public string SN { get; set; }
        public string IMEI { get; set; }
        public string Batch { get; set; }
        public string MeterTypeName { get; set; }
        public DateTime? OpenAccountTime { get; set; }
    }
}