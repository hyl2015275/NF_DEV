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
using NFine.Domain.Entity.ArchiveManage;

namespace NFine.Mapping.ArchiveManage
{
    public class MeterChargingMap : EntityTypeConfiguration<MeterChargingEntity>
    {
        public MeterChargingMap()
        {
            this.ToTable("Arc_MeterCharging");
            this.HasKey(t => t.F_ArchiveId);
        }
    }
}
