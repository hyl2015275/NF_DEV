using System.Web.Http;
using NFine.Application.MeterReading;
using NFine.Domain.MBus.Models;
using NFine.Library;
using NFine.Library.Analysis;
using NFine.Library.DefaultImplements;
using NFine.Web.App_Start.Handler;

namespace NFine.Web.Controllers
{
    public class MBusController : ControllerMBus
    {
        [HttpPost]
        public bool Submit(CommandResult cmd)
        {
            byte[] content;
            if (!Util.ByteArrayFromHexString(cmd.Data, out content)) return false;
            var isChecked = Check.IsCheck(content);
            if (isChecked == false) return false;
            var result = Parse.ParaseCommand(new BaseMessage(content.Length, content, ProtocolEnum.X188));
            if (result == null) return false;
            switch (result.Code)
            {
                case (int)ResultTypeEnum.Iimplement:
                    if (!string.IsNullOrEmpty(result.DeviceId))
                        new ReadTaskApp().SetConfirmationTask(result.DeviceId);
                    break;
                case (int)ResultTypeEnum.Read:
                    {
                        if (result.Record != null)
                            new ReadRecordApp().ServiceSubmit(result.Record);
                    }
                    break;
            }
            return true;
        }

        [HttpPost]
        public bool Register([FromBody]PortSetting portSetting)
        {
            return true;
        }
    }
}