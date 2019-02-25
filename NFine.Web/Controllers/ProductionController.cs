using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NFine.Application.MaterialManage;
using NFine.Application.MeterReading;
using NFine.Domain.ViewModel.Material;
using NFine.Web.Models;
using NFine.Code;
using NFine.Application.ArchiveManage;

namespace NFine.Web.Controllers
{
    public class ProductionController : ApiController
    {
        private readonly IOTApp _iotApp = new IOTApp();
        private readonly ReadRecordApp _readRecordApp = new ReadRecordApp();
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly ReadTaskApp _readTaskApp = new ReadTaskApp();
        [HttpGet]
        public ModuleInfoViewModel GetModuleInfo(string imei)
        {
            var data = _iotApp.GetFormByImei(imei);
            if (data != null)
            {
                ModuleInfoViewModel moduleInfo = new ModuleInfoViewModel
                {
                    SN = data.M_SN,
                    Batch = data.M_Batch,
                    ICCID = data.M_ICCID,
                    IMEI = data.M_IMEI,
                    IMSI = data.M_IMSI,
                    MeterTypeName = data.M_MeterTypeName,
                    OpenAccountTime = data.M_OpenAccountTime
                };
                return moduleInfo;
            }
            else
            {
                throw new Exception("模组不存在！");
            }
        }
        [HttpGet]
        public ModelStateViewModel GetModuleState(string imsi, string scanTime = "")
        {
            var startTime = !string.IsNullOrEmpty(scanTime) ? DateTime.Parse(scanTime) : DateTime.MinValue;
            var endTime = DateTime.Now;
            var data = _readRecordApp.GetLastReadRecordByImsi(imsi, startTime, endTime);
            if (data != null)
            {
                ModelStateViewModel modelState = new ModelStateViewModel();
                var count = _readRecordApp.GetListByImsi(imsi, startTime).Count;
                var details = data.F_Details.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();
                var signal = int.Parse(details["信号强度"]);
                modelState = new ModelStateViewModel()
                {
                    IMSI = imsi,
                    Dosage = data.F_TotalDosage ?? 0,
                    OnlineTime = data.F_ReadTime == null ? "" : data.F_ReadTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    MeterCode = data.F_MeterCode,
                    Signal = signal,
                    OnlineNumber = count
                };
                return modelState;
            }
            else
            {
                throw new Exception("模组无信息！");
            }
        }
        [HttpPost]
        public bool RelevanceMeterCode([FromBody]RelevanceParameter para)
        {
            var meter = _meterApp.GetFormByMeterCode(para.MeterCode);
            if (meter == null)
            {
                if (para.Relevance == true)
                {
                    return _readTaskApp.RelevanceMeterCode(para.MeterCode, para.IMSI);
                }
                else
                {
                    throw new Exception("未找到表绑定记录！");
                }
            }
            else
            {
                if (para.Relevance == false)
                {
                    return _readTaskApp.RelieveMeterCode(meter.F_Id, para.IMSI);
                }
                else
                {
                    throw new Exception("表已存在绑定记录");
                }
            }
        }
        [HttpPost]
        public bool ClearDosage([FromBody]ClearParameter para)
        {
            return _readTaskApp.ClearDosage(para.IMSI);
        }
        [HttpPost]
        public bool ClearRecord([FromBody]ClearParameter para)
        {
            return _readRecordApp.ClearRecord(para.IMSI);
        }
    }
}