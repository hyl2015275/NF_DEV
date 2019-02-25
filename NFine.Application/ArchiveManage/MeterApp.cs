/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Repository.ArchiveManage;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Application.ArchiveManage
{
    public class MeterApp
    {
        private readonly IMeterRepository _service = new MeterRepository();
        private readonly IMeterViewRepository _viewService = new MeterViewRepository();
        private readonly PriceApp _priceApp = new PriceApp();
        public List<MeterViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<MeterViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["model"].IsEmpty())
            {
                var model = queryParam["model"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterModel == model);
            }
            if (!queryParam["factor"].IsEmpty())
            {
                var factor = queryParam["factor"].ToString();
                //生产厂商
                expression = expression.And(t => t.F_Factor == factor);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名、安装地址、身份证号
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword) || t.F_IDNumber.Contains(keyword));
            }
            if (!queryParam["balance"].IsEmpty())
            {
                var balance = decimal.Parse(queryParam["balance"].ToString());
                //余额小于等于
                expression = expression.And(t => t.F_Balance <= balance);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            expression = expression.And(t => t.F_DeleteMark != true);
            try
            {
                var list = _viewService.FindList(expression, pagination).ToList();
                foreach (var item in list)
                {
                    item.F_PriceModel = _priceApp.GetForm(item.F_PriceModel).F_PriceName;
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public List<MeterViewModel> GetListByMeterType(List<string> meterTypes, string ownerId = "")
        {
            return string.IsNullOrEmpty(ownerId) ?
                _viewService.IQueryable(x => x.F_DeleteMark != true && meterTypes.Contains(x.F_MeterType) && x.F_State == true).ToList() :
                _viewService.IQueryable(x => x.F_DeleteMark != true && meterTypes.Contains(x.F_MeterType) && x.F_OwnerId == ownerId && x.F_State == true).ToList();
        }
        public List<MeterViewModel> QueryMeter(string q, int limit, string companyId, List<string> channelMeter = null)
        {
            if (channelMeter == null)
            {
                return
                    _viewService.IQueryable(
                        x =>
                            x.F_DeleteMark != true && x.F_State == true && x.F_OwnerId == companyId &&
                            (x.F_CustomerName.Contains(q) || x.F_MeterCode.Contains(q) || x.F_MeterCode.Contains(q) ||
                             x.F_IDNumber.Contains(q) || x.F_CustomerAddress.Contains(q))).Take(limit).ToList();
            }
            else
            {
                return
                   _viewService.IQueryable(
                       x =>
                           x.F_DeleteMark != true && x.F_State == true && x.F_OwnerId == companyId && !channelMeter.Contains(x.F_Id) &&
                           (x.F_CustomerName.Contains(q) || x.F_MeterCode.Contains(q) || x.F_MeterCode.Contains(q) ||
                            x.F_IDNumber.Contains(q) || x.F_CustomerAddress.Contains(q))).Take(limit).ToList();
            }
        }

        public List<MeterViewModel> GetAllList(string companyId, string customerName = "")
        {
            return string.IsNullOrEmpty(customerName)
                ? _viewService.IQueryable(x => x.F_OwnerId == companyId && x.F_DeleteMark != true && x.F_State == true)
                    .ToList()
                : _viewService.IQueryable(
                        x =>
                            x.F_OwnerId == companyId && x.F_DeleteMark != true && x.F_State == true &&
                            x.F_CustomerName.Contains(customerName))
                    .ToList();
        }

        public MeterEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }

        public MeterViewModel GetFormByUserCard(string userCard, string companyId)
        {
            return _viewService.FindEntity(x => x.F_UserCard == userCard && x.F_DeleteMark != true && x.F_State == true && x.F_OwnerId == companyId);
        }

        public MeterViewModel GetFormByMeterCode(string meterCode, string companyId = "")
        {
            return !string.IsNullOrEmpty(companyId) ? _viewService.FindEntity(x => x.F_MeterCode == meterCode && x.F_DeleteMark != true && x.F_State == true && x.F_OwnerId == companyId) : _viewService.FindEntity(x => x.F_MeterCode == meterCode && x.F_DeleteMark != true && x.F_State == true);
        }

        public MeterViewModel GetFormByMeterNumber(string meterNumber, string companyId)
        {
            return _viewService.FindEntity(x => x.F_MeterNumber == meterNumber && x.F_DeleteMark != true && x.F_State == true && x.F_OwnerId == companyId);
        }

        public void DeleteForm(string keyValue)
        {
            var meterEntity = GetForm(keyValue);
            meterEntity.Remove();
            _service.Update(meterEntity);
        }
        public void SubmitForm(MeterEntity meterEntity, MeterChargingEntity meterChargingEntity, string keyValue)
        {
            CheckEntity(meterEntity, keyValue);
            meterEntity.F_MeterNumber = meterEntity.F_MeterNumber ?? BitConverter.ToUInt16(Guid.Parse(meterEntity.F_OwnerId).ToByteArray().Take(2).ToArray(), 0) + "|" + (_service.IQueryable().Count() + 1);
            meterChargingEntity.F_CloseAmount = meterChargingEntity.F_CloseAmount ?? 0;
            meterChargingEntity.F_AlarmAmount = meterChargingEntity.F_AlarmAmount ?? 0;
            meterChargingEntity.F_Balance = meterChargingEntity.F_Balance ?? 0;
            meterChargingEntity.F_EnableAlarm = meterChargingEntity.F_EnableAlarm ?? false;
            meterChargingEntity.F_EnableClose = meterChargingEntity.F_EnableClose ?? false;
            meterChargingEntity.F_StartValue = meterChargingEntity.F_StartValue ?? 0;
            meterChargingEntity.F_State = meterChargingEntity.F_State ?? true;
            _service.SubmitForm(meterEntity, meterChargingEntity, keyValue);
        }
        public void CheckEntity(MeterEntity meterEntity, string keyValue)
        {
            var ownerId = meterEntity.F_OwnerId;
            if (string.IsNullOrEmpty(meterEntity.F_MeterCode))
                throw new Exception("表计编码不允许为空");
            if (string.IsNullOrEmpty(meterEntity.F_UserCard))
                throw new Exception("用户卡号不允许为空");
            if (_service.IQueryable(x => x.F_MeterCode == meterEntity.F_MeterCode && x.F_Id != keyValue && x.F_DeleteMark != true).Any())
                throw new Exception("表计编码" + meterEntity.F_MeterCode + "已存在");
            if (_service.IQueryable(x => x.F_UserCard == meterEntity.F_UserCard && x.F_OwnerId == ownerId && x.F_Id != keyValue && x.F_DeleteMark != true).Any())
                throw new Exception("用户卡号" + meterEntity.F_UserCard + "已存在");
        }
        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="queryJson"></param>
        /// <param name="companyId"></param>
        public void DownLoad(string queryJson, string companyId)
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var expression = ExtLinq.True<MeterViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["model"].IsEmpty())
            {
                var model = queryParam["model"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterModel == model);
            }
            if (!queryParam["factor"].IsEmpty())
            {
                var factor = queryParam["factor"].ToString();
                //生产厂商
                expression = expression.And(t => t.F_Factor == factor);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword) || t.F_IDNumber.Contains(keyword));
            }
            if (!queryParam["balance"].IsEmpty())
            {
                var balance = decimal.Parse(queryParam["balance"].ToString());
                //余额小于等于
                expression = expression.And(t => t.F_Balance <= balance);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            expression = expression.And(t => t.F_DeleteMark != true);
            var meterList = _viewService.IQueryable(expression).OrderByDescending(x => x.F_CreatorTime);
            if (companyId == "030eca51-483a-47b6-ad46-b6164278ebcb")
            {
                var dt = new DataTable();
                var now = DateTime.Now;
                dt.Columns.AddRange(new[]
                {
                    new DataColumn("UserID"),
                    new DataColumn("UserName"),
                    new DataColumn("Address"),
                    new DataColumn("Tel"),
                    new DataColumn("WaterTagID"),
                    new DataColumn("LINENO"),
                    new DataColumn("AreaID"),
                    new DataColumn("InTabManID"),
                    new DataColumn("AMTabNum"),
                    new DataColumn("NMTabNum"),
                    new DataColumn("WaterMete"),
                    new DataColumn("WaterPrice"),
                    new DataColumn("CBtDate"),
                });
                foreach (var en in meterList)
                {
                    var row = dt.NewRow();
                    try
                    {
                        var customPara = en.F_Description.ToObject<MeterCustomParameter>();
                        row[5] = customPara == null ? "" : customPara.LINENO;
                        row[6] = customPara == null ? "" : customPara.AreaID;
                        row[7] = customPara == null ? "" : customPara.InTabManIDs;
                    }
                    catch
                    {
                        
                    }
                    row[0] = en.F_UserCard;
                    row[1] = en.F_CustomerName;
                    row[2] = en.F_CustomerAddress;
                    row[3] = en.F_MobilePhone;
                    row[4] = en.F_MeterCode;
                    row[8] = null;
                    row[9] = null;
                    row[10] = null;
                    row[11] = _priceApp.GetForm(en.F_PriceModel).F_PriceName; ;
                    row[12] = now.ToString("yyyy-MM-dd");
                    dt.Rows.Add(row);
                }
                var fileName = "11" + now.ToString("yyyyMM") + "X.xls";
                NPOIExcel.ExportByWeb(dt, fileName, fileName);
            }
            else
            {
                var dt = new DataTable();
                dt.Columns.AddRange(new[]
                 {
                new DataColumn("表计类型"),
                new DataColumn("计费方式"),
                new DataColumn("执行价格"),
                new DataColumn("生产厂商"),
                new DataColumn("用户卡号"),
                new DataColumn("表计编码"),
                new DataColumn("当前余额"),
                new DataColumn("客户名称"),
                new DataColumn("身份证号"),
                new DataColumn("联系方式"),
                new DataColumn("安装地址"),
                 });
                foreach (var en in meterList)
                {
                    var row = dt.NewRow();
                    if (dataItems != null)
                    {
                        row[0] = ((Dictionary<string, string>)dataItems["DeviceType"])[en.F_MeterType];
                        row[1] = ((Dictionary<string, string>)dataItems["MeterModel"])[en.F_MeterModel];
                    }
                    else
                    {
                        row[0] = "";
                        row[1] = "";
                    }
                    row[2] = _priceApp.GetForm(en.F_PriceModel).F_PriceName;
                    row[3] = en.F_Factor;
                    row[4] = en.F_UserCard;
                    row[5] = en.F_MeterCode;
                    row[6] = en.F_Balance;
                    row[7] = en.F_CustomerName;
                    row[8] = en.F_IDNumber;
                    row[9] = en.F_MobilePhone;
                    row[10] = en.F_CustomerAddress;
                    dt.Rows.Add(row);
                }
                NPOIExcel.ExportByWeb(dt, "客户档案", "客户档案.xls");
            }
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
                    var price = new PriceEntity();
                    if (companyId == "030eca51-483a-47b6-ad46-b6164278ebcb")
                    {
                        var metercode = row[4] as string;
                        if (string.IsNullOrEmpty(metercode)) continue;
                        price = _priceApp.GetDefault(companyId);
                        var customPara = new MeterCustomParameter
                        {
                            LINENO = "",
                            BusinessID = row[5] as string,
                            AreaID = row[6] as string,
                            BookID = row[7] as string,
                            InTabManIDs = row[8] as string,
                        };
                        var meterEntity = new MeterEntity();
                        var meterChargingEntity = new MeterChargingEntity();
                        var meter = _service.FindEntity(x => x.F_MeterCode == metercode && x.F_DeleteMark != true && x.F_OwnerId == companyId);
                        if (meter != null)
                        {
                            var meterCharging = new MeterChargingApp().GetForm(meter.F_Id);
                            meterEntity = meter;
                            meterChargingEntity = meterCharging;
                        }
                        meterEntity.F_MeterType = "WaterMeter";
                        meterEntity.F_Factor = "贝林电子";
                        meterEntity.F_MeterName = row[1] as string;
                        meterEntity.F_MeterRate = 1;
                        meterEntity.F_UserCard = (string)row[0];
                        meterEntity.F_MeterCode = (string)row[4];
                        meterEntity.F_CustomerName = (string)row[1];
                        meterEntity.F_IDNumber = row[0] as string;
                        meterEntity.F_MobilePhone = row[3] as string;
                        meterEntity.F_CustomerAddress = row[2] as string;
                        meterEntity.F_Description = customPara.ToJson();
                        meterEntity.F_OwnerId = companyId;
                        meterChargingEntity.F_PriceModel = price.F_Id;
                        meterChargingEntity.F_MeterModel = "1";
                        SubmitForm(meterEntity, meterChargingEntity, meter == null ? "" : meter.F_Id);
                        count++;
                    }
                    else
                    {
                        var metercode = row[7] as string;
                        if (string.IsNullOrEmpty(metercode)) continue;
                        var priceName = row[2] as string;
                        price = _priceApp.GetForm(companyId, priceName);
                        if (price == null) throw new Exception("当前执行价格不存在");
                        double rate;
                        var meterEntity = new MeterEntity();
                        var meterChargingEntity = new MeterChargingEntity();
                        var meter = _service.FindEntity(x => x.F_MeterCode == metercode && x.F_DeleteMark != true && x.F_OwnerId == companyId);
                        if (meter != null)
                        {
                            var meterCharging = new MeterChargingApp().GetForm(meter.F_Id);
                            meterEntity = meter;
                            meterChargingEntity = meterCharging;
                        }
                        meterEntity.F_MeterType = ((Dictionary<string, string>)dataItems["DeviceType"]).First(x => x.Value == (string)row[0]).Key;
                        meterEntity.F_Factor = (string)row[3];
                        meterEntity.F_MeterName = row[4] as string;
                        meterEntity.F_MeterRate = double.TryParse(row[5] as string, out rate) ? rate : 1;
                        meterEntity.F_UserCard = (string)row[6];
                        meterEntity.F_MeterCode = (string)row[7];
                        meterEntity.F_CustomerName = (string)row[8];
                        meterEntity.F_IDNumber = row[9] as string;
                        meterEntity.F_MobilePhone = (string)row[10];
                        meterEntity.F_CustomerAddress = row[11] as string;
                        meterEntity.F_Description = row[12] as string;
                        meterEntity.F_OwnerId = companyId;
                        meterChargingEntity.F_PriceModel = price.F_Id;
                        meterChargingEntity.F_MeterModel = ((Dictionary<string, string>)dataItems["MeterModel"]).First(x => x.Value == (string)row[1]).Key;
                        SubmitForm(meterEntity, meterChargingEntity, meter == null ? "" : meter.F_Id);
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("表格行" + count + "数据不正确," + ex.Message);
                }
            }
            return count > 2;
        }
        /// <summary>
        /// 获取设备数量
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public int GetCountTotal(string companyId)
        {
            return _service.IQueryable(x => x.F_OwnerId == companyId).Count(x => x.F_DeleteMark != true);
        }

        public void InsertTestMeter(string meterCode)
        {
            var meterEntity = new MeterEntity
            {
                F_MeterType = "WaterMeter",
                F_Factor = "贝林电子",
                F_MeterRate = 1,
                F_MeterCode = meterCode,
                F_OwnerId = "b566c9fd-cc74-4280-95e5-b513b86f365b",
                F_UserCard = meterCode
            };
            var meterChargingEntity = new MeterChargingEntity
            {
                F_PriceModel = "95a3d254-af0b-4424-894c-3de53c85bdfb",
                F_MeterModel = "1",
                F_Balance = 0,
                F_StartValue = 0,
                F_CloseAmount = 0,
                F_AlarmAmount = 0,
                F_EnableAlarm = false,
                F_EnableClose = false,
                F_State = true
            };
            SubmitForm(meterEntity, meterChargingEntity, "");
        }
    }
}