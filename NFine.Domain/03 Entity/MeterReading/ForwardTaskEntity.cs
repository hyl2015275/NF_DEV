/*******************************************************************************
 * Copyright © 2018 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Entity.MeterReading
{
    public class ForwardTaskEntity
    {
        public string F_Id { get; set; }
        public bool F_Status { get; set; }
        public DateTime F_CreatorTime { get; set; }
    }
}