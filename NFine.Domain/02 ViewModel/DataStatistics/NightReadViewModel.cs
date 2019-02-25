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

namespace NFine.Domain.ViewModel.DataStatistics
{
    public class NightReadViewModel
    {
        public string F_Id { get; set; }
        public string F_ArchiveId { get; set; }
        public decimal? F_ThisDosage { get; set; }
        public decimal? F_TotalDosage { get; set; }
        public string F_ReadType { get; set; }
        public DateTime? F_ReadTime { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_Details { get; set; }
        public string F_UserCard { get; set; }
        public string F_MeterCode { get; set; }
        public string F_MeterName { get; set; }
        public string F_MeterType { get; set; }
        public string F_Factor { get; set; }
        public string F_CustomerName { get; set; }
        public string F_CustomerAddress { get; set; }
        public string F_MobilePhone { get; set; }
        public string F_Description { get; set; }
        public string F_OwnerId { get; set; }
    }
}
