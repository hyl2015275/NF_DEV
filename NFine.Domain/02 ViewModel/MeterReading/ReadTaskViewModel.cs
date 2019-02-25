using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Domain.Entity.MeterReading;

namespace NFine.Domain.ViewModel.MeterReading
{
    public class ReadTaskViewModel
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string F_Id { get; set; }
        /// <summary>
        /// 表计编码
        /// </summary>
        public string F_MeterCode { get; set; }
        /// <summary>
        /// 表类型
        /// </summary>
        public string F_MeterType { get; set; }
        /// <summary>
        /// 厂商
        /// </summary>
        public string F_Factor { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public int F_TaskType { get; set; }
        /// <summary>
        /// 执行结果
        /// </summary>
        public int F_State { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string F_Param { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public string F_Value { get; set; }
        public string F_WorkId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? F_ExecuteTime { get; set; }
        /// <summary>
        /// 通道类型
        /// </summary>
        public int? F_ChannelType { get; set; }
        public string F_UserCard { get; set; }
        public string F_CustomerName { get; set; }
        public string F_CustomerAddress { get; set; }
        public string F_MobilePhone { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_OwnerId { get; set; }
    }
}