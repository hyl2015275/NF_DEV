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
using NFine.Domain.Entity.ArchiveManage;

namespace NFine.Domain.ViewModel.ArchiveManage
{
    public class ChangeMeterViewModel : ChangeMeterEntity
    {
        public string F_MeterCode { get; set; }
        public string F_MeterType { get; set; }
        public string F_Factor { get; set; }
        public string F_MeterNumber { get; set; }
        public string F_CustomerName { get; set; }
        public string F_CustomerAddress { get; set; }
        public string F_MobilePhone { get; set; }
        public int? F_State { get; set; }
        public string F_UserCard { get; set; }
        public string F_OwnerId { get; set; }
    }
}