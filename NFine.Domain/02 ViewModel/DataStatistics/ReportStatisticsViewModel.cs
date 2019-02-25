/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.ViewModel.DataStatistics
{
    public class ReportStatisticsViewModel
    {
        public Guid F_Id { get; set; }
        public string F_MeterType { get; set; }
        public string F_ReportCycle { get; set; }
        public int F_CountRecord { get; set; }
        public int F_CountDevice { get; set; }
        public decimal F_SumDosage { get; set; }
        public decimal F_SumBill { get; set; }
        public decimal F_SumPay { get; set; }
        public string F_OwnerId { get; set; }
    }
}
