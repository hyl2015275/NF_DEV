using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NFine.Application
{
    public class MeterRecord
    {
        public string MeterCode { get; set; }
        public string Value { get; set; }
        public DateTime ReadingTime { get; set; }
        public string TaskType { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
        /// <summary>
        /// 结算数据
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Settlement { get; set; }
    }
}