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
using System.Data.Entity.ModelConfiguration;
using NFine.Domain.Entity.PaymentManage;

namespace NFine.Mapping.PaymentManage
{
    public class ChargeRecordMap : EntityTypeConfiguration<ChargeRecordEntity>
    {
        public ChargeRecordMap()
        {
            this.ToTable("Pay_ChargeRecord");
            this.HasKey(t => t.F_ReadId);
        }
    }
}