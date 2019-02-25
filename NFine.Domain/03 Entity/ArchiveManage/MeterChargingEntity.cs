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
using NFine.Data.Extensions;

namespace NFine.Domain.Entity.ArchiveManage
{
    public class MeterChargingEntity : IEntity<MeterChargingEntity>
    {
        public string F_ArchiveId { get; set; }
        public string F_MeterModel { get; set; }
        public string F_PriceModel { get; set; }
        public decimal? F_StartValue { get; set; }
        [DecimalPrecision]
        public decimal? F_Balance { get; set; }
        public decimal? F_AlarmAmount { get; set; }
        public decimal? F_CloseAmount { get; set; }
        public bool? F_EnableAlarm { get; set; }
        public bool? F_EnableClose { get; set; }
        public bool? F_State { get; set; }
    }
}