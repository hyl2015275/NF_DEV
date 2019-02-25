using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NFine.Domain.ViewModel.ArchiveManage
{
    public class MeterCustomParameter
    {
        public string LINENO { get; set; }//通道号
        public string AreaID { get; set; }//区域号
        public string InTabManIDs { get; set; }//抄表员
        public string BusinessID { get; set; }//营业所号
        public string BookID { get; set; }//册本号
    }
}