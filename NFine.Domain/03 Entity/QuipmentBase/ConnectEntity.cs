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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NFine.Domain.Entity.QuipmentBase
{
    public class ConnectEntity
    {
        public string DeviceId { get; set; }
        public string MeterCode { get; set; }
        public string Ip { get; set; }
        public DateTime Online { get; set; }
        public DateTime Time { get; set; }
        public string Span { get; set; }
        public string Details { get; set; }
        public bool IsLong { get; set; }
    }
}