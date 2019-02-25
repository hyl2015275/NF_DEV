/*******************************************************************************
 * Copyright © 2018 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Entity.ArchiveManage
{
    public class SubsistenceSecurityEntity : IEntity<SubsistenceSecurityEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string F_Id { get; set; }
        public string F_ArchiveId { get; set; }
        public string F_PriceId { get; set; }
        public DateTime? F_StartTime { get; set; }
        public DateTime? F_EndTime { get; set; }
        public int? F_EnjoyTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
    }
}