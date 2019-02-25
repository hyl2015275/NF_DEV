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

namespace NFine.Mapping.MeterReading
{
    public class TaskListMap : EntityTypeConfiguration<TaskListEntity>
    {
        public TaskListMap()
        {
            this.ToTable("Mer_TaskList");
            this.HasKey(t => t.F_Id);
        }
    }
}