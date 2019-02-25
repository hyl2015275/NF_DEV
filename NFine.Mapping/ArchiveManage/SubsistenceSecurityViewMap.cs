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
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Mapping.ArchiveManage
{
    public class SubsistenceSecurityViewMap : EntityTypeConfiguration<SubsistenceSecurityViewModel>
    {
        public SubsistenceSecurityViewMap()
        {
            this.ToTable("View_Arc_SubsistenceSecurity");
            this.HasKey(t => t.F_Id);
        }
    }
}