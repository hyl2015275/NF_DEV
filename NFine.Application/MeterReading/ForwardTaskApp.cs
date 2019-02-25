using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Code;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.ViewModel.MeterReading;
using NFine.Repository.MeterReading;

namespace NFine.Application.MeterReading
{
    public class ForwardTaskApp
    {
        private readonly ForwardTaskRepository _service = new ForwardTaskRepository();

        public void SubmitForm(ForwardTaskEntity forwardList)
        {
            _service.Insert(forwardList);
        }

        public bool PostData(ReadRecordViewModel data)
        {
            var readTime = data.F_ReadTime == null ? "" : ((DateTime)data.F_ReadTime).ToString("yyyy-MM-dd HH:mm:ss");
            return HttpMethods.HttpGet("http://120.220.0.205:5080/Ammeter/upload/add?number=" + data.F_MeterCode + "&electricity=" + data.F_TotalDosage + "&time=" + readTime) == "true";
        }
    }
}
