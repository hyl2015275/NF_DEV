using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.ViewModel.DataStatistics
{
    public class DosageStatisticsViewModel
    {
        public string F_Id { get; set; }
        public string F_MeterType { get; set; }
        public string F_UserCard { get; set; }
        public string F_MeterCode { get; set; }
        public string F_CustomerName { get; set; }
        public string F_CustomerAddress { get; set; }
        public string F_MobilePhone { get; set; }
        public decimal F_LastRecord { get; set; }
        public decimal F_ThisRecord { get; set; }
        public decimal F_ThisDosage { get; set; }
        public decimal F_ThisBill { get; set; }
        public decimal F_UnitPrice { get; set; }
        public string F_Description { get; set; }
    }
}
