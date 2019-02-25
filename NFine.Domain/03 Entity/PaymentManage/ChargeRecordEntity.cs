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

namespace NFine.Domain.Entity.PaymentManage
{
    public class ChargeRecordEntity : IEntity<ChargeRecordEntity>
    {
        public string F_ReadId { get; set; }
        [DecimalPrecision(18, 3)]
        public decimal F_UnitPrice { get; set; }
        [DecimalPrecision]
        public decimal F_ThisBill { get; set; }
        [DecimalPrecision]
        public decimal F_Balance { get; set; }
        public string F_Description { get; set; }
        public DateTime F_ChargeTime { get; set; }
    }
}