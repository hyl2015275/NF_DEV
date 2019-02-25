﻿/*******************************************************************************
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
    public enum ReadTypeEnum
    {
        /// <summary>
        /// 定时抄表
        /// </summary>
        Time = 0,
        /// <summary>
        /// 任务抄表
        /// </summary>
        Task = 1,
        /// <summary>
        /// 主动上传
        /// </summary>
        Active = 2
    }
}
