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
using NFine.Domain;
using NFine.Domain.Entity.ArchiveManage;

namespace NFine.Domain.Entity.ArchiveManage
{
    public class PriceEntity : IEntity<MeterEntity>, ICreationAudited, IDeleteAudited, IModificationAudited, IOwnAudited
    {
        public string F_Id { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_OwnerId { get; set; }
        public string F_PriceName { get; set; }
        public string F_PriceType { get; set; }
        public string F_UnitPrice { get; set; }
        public bool? F_EnabledMark { get; set; }
        public string F_Description { get; set; }
        public DateTime F_StartTime { get; set; }
        public int? F_Cycle { get; set; }
    }
}