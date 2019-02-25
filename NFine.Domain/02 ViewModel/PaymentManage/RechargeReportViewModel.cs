using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.ViewModel.PaymentManage
{
    public class RechargeReportViewModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid F_Id { get; set; }
        /// <summary>
        /// 购买日期
        /// </summary>
        public string F_PurchaseDate { get; set; }
        /// <summary>
        /// 购买月份
        /// </summary>
        public string F_Month { get; set; }
        /// <summary>
        /// 购买人次
        /// </summary>
        public int F_PurchaseNumber { get; set; }
        /// <summary>
        /// 累计数量
        /// </summary>
        public decimal F_AccumulativeTotal { get; set; }
        /// <summary>
        /// 累计金额
        /// </summary>
        public decimal F_AccumulativeAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 所有者
        /// </summary>
        public string F_OwnerId { get; set; }
    }
}
