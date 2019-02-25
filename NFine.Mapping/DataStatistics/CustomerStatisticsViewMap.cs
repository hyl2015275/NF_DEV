/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System.Data.Entity.ModelConfiguration;
using NFine.Domain.ViewModel.DataStatistics;

namespace NFine.Mapping.DataStatistics
{
    public class CustomerStatisticsViewMap : EntityTypeConfiguration<CustomerStatisticsViewModel>
    {
        public CustomerStatisticsViewMap()
        {
            this.ToTable("Ala_CustomerStatistics");
            this.HasKey(t => t.F_Id);
        }
    }
}