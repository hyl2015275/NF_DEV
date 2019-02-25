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
    public class ReadRecordEntity : IEntity<ReadRecordEntity>
    {
        public string F_Id { get; set; }
        public string F_ArchiveId { get; set; }
        public decimal? F_ThisDosage { get; set; }
        public decimal? F_TotalDosage { get; set; }
        public string F_TaskId { get; set; }
        public string F_ReadType { get; set; }
        public DateTime? F_ReadTime { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_Imsi { get; set; }
        public string F_Details { get; set; }
        public string F_Settlement { get; set; }
    }
}