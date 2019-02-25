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
    public enum PayStateEnum
    {
        /// <summary>
        /// 未支付
        /// </summary>
        Build = 0,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish = 1,
        /// <summary>
        /// 未远传
        /// </summary>
        Wait = -1,
        /// <summary>
        /// 未圈存
        /// </summary>
        Write = -2
    }
}