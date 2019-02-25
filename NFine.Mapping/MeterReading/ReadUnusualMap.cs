/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.ViewModel.MeterReading;

namespace NFine.Mapping.MeterReading
{
    public class ReadUnusualMap : EntityTypeConfiguration<ReadUnusualEntity>
    {
        public ReadUnusualMap()
        {
            this.ToTable("Mer_ReadUnusual");
            this.HasKey(t => t.F_Id);
        }
    }
}
