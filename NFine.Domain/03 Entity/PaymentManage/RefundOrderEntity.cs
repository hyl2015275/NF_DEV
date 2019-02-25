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
using NFine.Domain.Entity.SystemManage;

namespace NFine.Domain.Entity.PaymentManage
{
    public class RefundOrderEntity : IEntity<OrganizeEntity>, ICreationAudited, IDeleteAudited, IModificationAudited, IOwnAudited
    {
        public string F_Id { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_OwnerId { get; set; }
        public string F_ArchiveId { get; set; }
        public decimal? F_Money { get; set; }
        public decimal? F_Balance { get; set; }
        public string F_OrderNumber { get; set; }
        public string F_CreatorUserName { get; set; }
        public DateTime? F_RefundTime { get; set; }
    }
}