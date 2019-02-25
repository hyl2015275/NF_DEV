﻿/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System.Data.Entity.ModelConfiguration;
using NFine.Domain.Entity.ArchiveManage;

namespace NFine.Mapping.ArchiveManage
{
    public class MeterMap : EntityTypeConfiguration<MeterEntity>
    {
        public MeterMap()
        {
            this.ToTable("Arc_Meter");
            this.HasKey(t => t.F_Id);
        }
    }
}