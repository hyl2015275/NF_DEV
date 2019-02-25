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

namespace NFine.Domain.Entity.ArchiveManage
{
    public class MeterEntity : IEntity<MeterEntity>, ICreationAudited, IDeleteAudited, IModificationAudited, IOwnAudited
    {
        public string F_Id { get; set; }
        public string F_MeterCode { get; set; }
        /// <summary>
        /// 表计名称
        /// </summary>
        public string F_MeterName { get; set; }
        /// <summary>
        /// 表计倍率
        /// </summary>
        public double? F_MeterRate { get; set; }
        public string F_MeterType { get; set; }
        public string F_Factor { get; set; }
        public string F_CustomerName { get; set; }
        public string F_CustomerAddress { get; set; }
        public string F_MobilePhone { get; set; }
        public string F_Description { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_OwnerId { get; set; }
        public string F_UserCard { get; set; }
        public string F_MeterNumber { get; set; }
        public string F_IDNumber { get; set; }
        public string F_GPS { get; set; }
        public string F_BiaoJing { get; set; }
        public string F_Sort { get; set; }
    }
}