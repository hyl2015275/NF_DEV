using System.Web.Http;
using NFine.Application.MeterReading;
using NFine.Code.Security;
using NFine.Domain.Mbus;
using NFine.Library;
using NFine.Library.Analysis;
using NFine.Library.DefaultImplements;

namespace NFine.Web.Controllers
{
    public class TelecomController : ApiController
    {
        [HttpPost]
        public bool Submit(DeviceData data)
        {
            if (data == null) return false;
            if (data.service == null || data.service.data == null || string.IsNullOrEmpty(data.service.data.rawData))
                return false;
            var rawData = data.service.data.rawData;
            byte[] content = Base64.DecodeBase64ToArry(rawData);
            var message = new BaseDataBuffer().TryReadMessage(content);
            var isChecked = Check.IsCheck(message.RawData);
            if (isChecked == false) return false;
            var result = Parse.ParaseCommand(message as BaseMessage);
            if (result == null) return false;
            switch (result.Code)
            {
                case (int)ResultTypeEnum.Read:
                    {
                        if (result.Record != null)
                            new ReadRecordApp().ServiceSubmit(result.Record);
                    }
                    break;
            }
            return true;
        }
    }
}