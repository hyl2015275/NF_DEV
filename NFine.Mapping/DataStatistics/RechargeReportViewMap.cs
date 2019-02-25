/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System.Data.Entity.ModelConfiguration;
using NFine.Domain.ViewModel.DataStatistics;
using NFine.Domain.ViewModel.PaymentManage;

namespace NFine.Mapping.DataStatistics
{
    public class RechargeReportViewMap : EntityTypeConfiguration<RechargeReportViewModel>
    {
        public RechargeReportViewMap()
        {
            this.ToTable("View_Pay_PayReport");
            this.HasKey(t => t.F_Id);
        }
    }
}