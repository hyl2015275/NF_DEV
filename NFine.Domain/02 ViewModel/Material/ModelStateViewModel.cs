using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.ViewModel.Material
{
    public class ModelStateViewModel
    {
        public int OnlineNumber { get; set; }//上限次数
        public string OnlineTime { get; set; }//上线时间
        public string IMSI { get; set; }//模组号
        public int Signal { get; set; }//信号强度
        public decimal Dosage { get; set; }//底数
        public string MeterCode { get; set; }//表号
    }
}
