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

namespace NFine.Domain.ViewModel.DataStatistics
{
    public class CustomerStatisticsViewModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public Guid F_Id { get; set; }
        /// <summary>
        /// 表类型
        /// </summary>
        public string F_MeterType { get; set; }
        /// <summary>
        /// 表计编码
        /// </summary>
        public string F_MeterCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string F_CustomerName { get; set; }
        /// <summary>
        /// 用电地址
        /// </summary>
        public string F_CustomerAddress { get; set; }
        /// <summary>
        /// 上月抄表
        /// </summary>
        public decimal F_LastMonthRecord { get; set; }
        /// <summary>
        /// 本月抄表
        /// </summary>
        public decimal F_ThisMonthRecord { get; set; }
        /// <summary>
        /// 本月余额
        /// </summary>
        public decimal F_ThisMonthBalance { get; set; }
        /// <summary>
        /// 本月用电
        /// </summary>
        public decimal F_ThisMonthDosage { get; set; }
        /// <summary>
        /// 倍率
        /// </summary>
        public double? F_MeterRate { get; set; }
        /// <summary>
        /// 执行价格
        /// </summary>
        public decimal? F_UnitPrice { get; set; }
        /// <summary>
        /// 本月电费
        /// </summary>
        public decimal F_ThisMonthBill { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string F_Month { get; set; }
        /// <summary>
        /// 所有者
        /// </summary>
        public string F_OwnerId { get; set; }
    }
}