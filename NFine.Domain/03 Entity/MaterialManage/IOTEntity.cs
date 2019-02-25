/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;

namespace NFine.Domain.Entity.MaterialManage
{
    public class IOTEntity : IEntity<IOTEntity>
    {
        public int M_ID { get; set; }
        public string M_IMEI { get; set; }
        public string M_SN { get; set; }
        public string M_IMSI { get; set; }
        public string M_ICCID { get; set; }
        public string M_CardID { get; set; }
        public string M_HardwareName { get; set; }
        public string M_ArtNo { get; set; }
        public string M_MeterTypeName { get; set; }
        public string M_CustomerName { get; set; }
        public string M_Location { get; set; }
        public string M_Balance { get; set; }
        public string M_Remarks { get; set; }
        public string M_Batch { get; set; }
        public DateTime? M_PurchaseTime { get; set; }
        public DateTime? M_OpenAccountTime { get; set; }
        public string M_OpenAccountUserName { get; set; }
    }
}
