using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NFine.Web.Models
{
    public class RelevanceParameter
    {
        public string IMSI { get; set; }//模块号
        public string MeterCode { get; set; }//表号
        public bool Relevance { get; set; }//关联/解绑
    }
}