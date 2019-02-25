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
using NFine.Data.Extensions;

namespace NFine.Domain.Entity.ArchiveManage
{
    public class PriceBaseEntity
    {
        public string F_Id { get; set; }
        public string F_PriceId { get; set; }
        public string F_PriceName { get; set; }
        [DecimalPrecision(18, 3)]
        public decimal F_PriceValue { get; set; }
        public int? F_SortNumber { get; set; }
    }
}
