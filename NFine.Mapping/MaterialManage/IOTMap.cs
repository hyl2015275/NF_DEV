/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Domain.Entity.MaterialManage;
using System.Data.Entity.ModelConfiguration;

namespace NFine.Mapping.MaterialManage
{
    public class IOTMap : EntityTypeConfiguration<IOTEntity>
    {
        public IOTMap()
        {
            this.ToTable("Mat_IOT");
            this.HasKey(t => t.M_ID);
        }
    }
}
