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
    public enum ReadUnusualEnum
    {
        /// <summary>
        /// 走数异常
        /// </summary>
        Read = 0,
        /// <summary>
        /// 未抄表
        /// </summary>
        Not = 1,
        /// <summary>
        /// 读数为零
        /// </summary>
        Zero = 2,
        /// <summary>
        /// 读数负值
        /// </summary>
        Negative = 3
    }
}
