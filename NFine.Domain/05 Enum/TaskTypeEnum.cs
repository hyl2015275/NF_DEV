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

namespace NFine.Domain.Enum
{
    public enum TaskTypeEnum
    {
        /// <summary>
        ///自动抄表 
        /// </summary>
        Read = 0,
        /// <summary>
        /// 任务抄表
        /// </summary>
        Task = 1,
        /// <summary>
        /// 物联网充值
        /// </summary>
        Pay = 2,
        /// <summary>
        /// 开阀
        /// </summary>
        OpenValve = 3,
        /// <summary>
        /// 关阀
        /// </summary>
        CloseValve = 4,
        /// <summary>
        /// 设置表底数
        /// </summary>
        SetBaseDosage = 5,
        /// <summary>
        /// 设置表底数
        /// </summary>
        SetHeartbeat = 6,
        /// <summary>
        /// 设置上线类型
        /// </summary>
        SetOnlineSpan = 7,
        /// <summary>
        ///读三相电压 
        /// </summary>
        ThreeVoltage = 111,
        /// <summary>
        /// 读三相电流
        /// </summary>
        ThreeCurrent = 112,
        /// <summary>
        /// 运行状态
        /// </summary>
        RunningState = 113,
        /// <summary>
        /// 尖时段电能
        /// </summary>
        TipEnergy = 114,
        /// <summary>
        /// 峰时段电能
        /// </summary>
        PeakEnergy = 115,
        /// <summary>
        /// 平时段电能
        /// </summary>
        FlatEnergy = 116,
        /// <summary>
        /// 谷时段电能
        /// </summary>
        ValleyEnergy = 117,
        /// <summary>
        /// 设置跳闸
        /// </summary>
        TripSwitch = 121,
        /// <summary>
        /// 设置合闸
        /// </summary>
        CloseSwitch = 122,
        /// <summary>
        /// 设置抄表参数
        /// </summary>
        EnergyParameter = 123,
        /// <summary>
        /// 下线
        /// </summary>
        NotOnline = 1100,
        /// <summary>
        /// 修改服务器地址
        /// </summary>
        ServerAddress = 1101,
        /// <summary>
        /// 修改模组地址
        /// </summary>
        ModuleAddress = 1102,
        /// <summary>
        /// 下载表记地址
        /// </summary>
        SetMeterAddress = 2001,
        /// <summary>
        /// 立即抄表
        /// </summary>
        SetRead = 2002,
        /// <summary>
        /// 读取抄表数据
        /// </summary>
        GetRead = 2003,
        /// <summary>
        /// 设置集中器时间
        /// </summary>
        SetTime = 2011,
      //  SetControlOn=
    }
}