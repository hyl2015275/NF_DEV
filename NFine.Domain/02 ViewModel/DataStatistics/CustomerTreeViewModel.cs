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
    public class CustomerTreeViewModel
    {
        public string text { get; set; }
        public string href { get; set; }
        public List<char> tags { get; set; }
        public List<CustomerTreeViewModel> nodes { get; set; }
    }
}