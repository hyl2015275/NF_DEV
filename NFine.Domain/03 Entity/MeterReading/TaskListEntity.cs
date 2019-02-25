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

namespace NFine.Domain.Entity.MeterReading
{
    public class TaskListEntity : IEntity<TaskListEntity>, ICreationAudited, IOwnAudited, IDeleteAudited, IModificationAudited
    {
        public string F_Id { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_OwnerId { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string F_TaskName { get; set; }
        /// <summary>
        /// 表类型
        /// </summary>
        public string F_MeterType { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public TimeSpan? F_ExecuteTime { get; set; }
        /// <summary>
        /// 每天循环执行
        /// </summary>
        public bool F_LoopMark { get; set; }
        /// <summary>
        /// 执行次数
        /// </summary>
        public int F_ExecuteNumber { get; set; }
        /// <summary>
        /// 设备数
        /// </summary>
        public int F_DeviceNumber { get; set; }
        /// <summary>
        /// 成功数
        /// </summary>
        public int F_SuccessNumber { get; set; }
        /// <summary>
        /// 失败数
        /// </summary>
        public int F_FailNumber { get; set; }
        /// <summary>
        /// 进度
        /// </summary>
        public string F_Speed { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
    }
}