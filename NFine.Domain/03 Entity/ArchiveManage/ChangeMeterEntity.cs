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
    public class ChangeMeterEntity : IEntity<ChangeMeterEntity>, ICreationAudited
    {
        public string F_Id { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 新标识
        /// </summary>
        public string F_ArchiveId { get; set; }
        /// <summary>
        /// 旧标识
        /// </summary>
        public string F_OldArchiveId { get; set; }
        /// <summary>
        /// 旧表计编码
        /// </summary>
        public string F_OldMeterCode { get; set; }
        /// <summary>
        /// 累计用量
        /// </summary>
        public decimal? F_TotalDosage { get; set; }
        /// <summary>
        /// 是否更新表底数
        /// </summary>
        public bool? F_AllowReplace { get; set; }
        /// <summary>
        /// 表底数
        /// </summary>
        public decimal? F_BaseDosage { get; set; }
        public string F_Description { get; set; }
    }
}