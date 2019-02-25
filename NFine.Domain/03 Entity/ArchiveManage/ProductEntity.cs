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
    public class ProductEntity : IEntity<ProductEntity>, ICreationAudited, IDeleteAudited, IModificationAudited, IOwnAudited
    {
        public string F_Id { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        /// <summary>
        /// 设备厂商
        /// </summary>
        public string F_OwnerId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string F_ProductName { get; set; }
        /// <summary>
        /// 表计类型
        /// </summary>
        public string F_MeterType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Details { get; set; }
    }
}