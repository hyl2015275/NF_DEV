/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;

namespace NFine.Domain.Entity.SystemManage
{
    public class MoneyAlarmEntity : IEntity<MoneyAlarmEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string F_Id { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public string F_ArchiveId { get; set; }
        public decimal F_AlarmMonry { get; set; }
        public bool F_AlarmState { get; set; }
        public bool F_SureState { get; set; }
        public string F_ErrorMsg { get; set; }
    }
}