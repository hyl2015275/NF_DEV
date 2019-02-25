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

namespace NFine.Domain.Entity.DataStatistics
{
    public class ReportSettingEntity
    {
        public string F_Id { get; set; }//编号
        public string F_DayTime { get; set; }//日结时间
        public string F_WeekTime { get; set; }//周结时间
        public string F_MonthTime { get; set; }//月结时间
        public string F_YearTime { get; set; }//年结时间
        public string F_Description { get; set; }//备注
        public string F_OwnerId { get; set; }//所有者
    }
}