using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Domain.ViewModel.ArchiveManage;
using NFine.Repository.ArchiveManage;

namespace NFine.Application.ArchiveManage
{
    public class ChannelMeterApp
    {
        private readonly IChannelMeterRepository _service = new ChannelMeterRepository();
        private readonly IChannelMeterViewRepository _viewService = new ChannelMeterViewRepository();
        private readonly ChannelApp _channelApp = new ChannelApp();
        private readonly MeterApp _meterApp = new MeterApp();
        public List<ChannelMeterViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<ChannelMeterViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号/表计编码/客户名称
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            if (!queryParam["channelId"].IsEmpty())
            {
                var channelId = queryParam["channelId"].ToString();
                switch (channelId)
                {
                    case "00000000_0000_0000_0000_000000000000":
                        break;
                    case "ffffffff_ffff_ffff_ffff_ffffffffffff":
                        var channelMeters = _viewService.IQueryable().Select(x => x.F_ArchiveId).ToList();
                        return _meterApp.GetList(pagination, queryJson, companyId).Where(x => !channelMeters.Contains(x.F_Id) && x.F_DeleteMark != true).Select(x => new ChannelMeterViewModel
                        {
                            F_ArchiveId = x.F_Id,
                            F_ChannelId = "",
                            F_Factor = x.F_Factor,
                            F_UserCard = x.F_UserCard,
                            F_CustomerAddress = x.F_CustomerAddress,
                            F_CustomerName = x.F_CustomerName,
                            F_MeterCode = x.F_MeterCode,
                            F_MeterType = x.F_MeterType,
                            F_MobilePhone = x.F_MobilePhone,
                            F_DeleteMark = x.F_DeleteMark,
                            F_OwnerId = x.F_OwnerId,
                        }).ToList();
                    default:
                        //设备编号
                        expression = expression.And(t => t.F_ChannelId == channelId);
                        break;
                }
            }
            expression = expression.And(t => t.F_DeleteMark != true);
            return _viewService.FindList(expression, pagination).ToList();
        }
        public List<ChannelMeterViewModel> GetList(string companyId = "")
        {
            return !string.IsNullOrEmpty(companyId) ? _viewService.IQueryable(x => x.F_OwnerId == companyId).ToList() : _viewService.IQueryable().ToList();
        }
        public List<string> GetMeterListByChannel(string content)
        {
            var channel = _channelApp.GetFormByContent(content);
            if (channel != null)
            {
                return _viewService.IQueryable(x => x.F_ChannelId == channel.F_Id).Select(x => x.F_MeterCode).ToList();
            }
            else
            {
                return null;
            }

        }
        public ChannelMeterViewModel GetForm(string keyValue)
        {
            return _viewService.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            _service.Delete(x => x.F_Id == keyValue);
        }
        public void SubmitForm(ChannelMeterEntity channeMeterEntity, string keyValue)
        {
            CheckEntity(channeMeterEntity, keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                channeMeterEntity.Create();
                _service.Insert(channeMeterEntity);
            }
            else
            {
                channeMeterEntity.Modify(keyValue);
                _service.Update(channeMeterEntity);
            }
        }
        public void CheckEntity(ChannelMeterEntity meterEntity, string keyValue)
        {
            if (string.IsNullOrEmpty(meterEntity.F_ArchiveId))
                throw new Exception("表计不允许为空");
            if (string.IsNullOrEmpty(meterEntity.F_ChannelId))
                throw new Exception("设备不允许空");
            if (
                _service.IQueryable(
                    x => x.F_ArchiveId == meterEntity.F_ArchiveId && x.F_Id != keyValue).Any())
                throw new Exception("表计已存在");
        }
        public bool UpLoad(Stream stream, string companyId)
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var count = 2;
            var ds = NPOIExcel.ImportExceltoDs(stream);
            if (ds == null) return false;
            var dt = ds.Tables[0];
            if (dt == null || dt.Rows.Count <= 0) return false;
            foreach (DataRow item in dt.Rows)
            {
                try
                {
                    var row = item;
                    if (string.IsNullOrEmpty(row[0] as string) || string.IsNullOrEmpty(row[1] as string)) continue;
                    var channel = new ChannelApp().GetFormByName((string)row[0]);
                    if (channel == null) throw new Exception("当前设备不存在");
                    var meter = _meterApp.GetFormByMeterCode((string)row[1], companyId);
                    if (meter == null) throw new Exception("当前表计不存在");
                    var channeMeterEntity = new ChannelMeterEntity
                    {
                        F_ArchiveId = meter.F_Id,
                        F_ChannelId = channel.F_Id,
                    };
                    var channelMeter = _viewService.FindEntity(x => x.F_ArchiveId == meter.F_Id && x.F_DeleteMark != true);
                    SubmitForm(channeMeterEntity, channelMeter == null ? "" : channelMeter.F_Id);
                    count++;
                }
                catch (Exception ex)
                {
                    throw new Exception("表格行" + count + "数据不正确," + ex.Message);
                }
            }
            return count > 2;
        }

    }
}