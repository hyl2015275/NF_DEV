/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using NFine.Application.DataStatistics;
using NFine.Application.ArchiveManage;
using NFine.Application.PaymentManage;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.Entity.MaterialManage;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.Enum;
using NFine.Domain.IRepository.MeterReading;
using NFine.Domain.IRepository.PaymentManage;
using NFine.Domain.ViewModel.MeterReading;
using NFine.Repository.MeterReading;
using NFine.Repository.PaymentManage;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;

namespace NFine.Application.MeterReading
{
    public class ReadRecordApp
    {
        private readonly IReadRecordRepository _service = new ReadRecordRepository();
        private readonly IReadRecordViewRepository _viewService = new ReadRecordViewRepository();
        private readonly IUnusualViewRepository _unusualService = new UnusualViewRepository();
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly ReadTaskApp _readTaskApp = new ReadTaskApp();
        private readonly MeterChargingApp _meterChargingApp = new MeterChargingApp();
        private readonly PriceApp _priceApp = new PriceApp();
        private readonly PayOrderApp _payOrderApp = new PayOrderApp();
        private readonly MoneyAlarmApp _moneyAlarmApp = new MoneyAlarmApp();
        private readonly SubsistenceSecurityApp _subsistenceSecurityApp = new SubsistenceSecurityApp();
        private readonly ReadUnusualApp _readUnusualApp = new ReadUnusualApp();
        public List<ReadRecordViewModel> GetList(Pagination pagination, string queryJson, string companyId = "", string keyValue = "", string archiveId = "")
        {
            var expression = ExtLinq.True<ReadRecordViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["factor"].IsEmpty())
            {
                var factor = queryParam["factor"].ToString();
                //生产厂商
                expression = expression.And(t => t.F_Factor == factor);
            }
            if (!queryParam["read"].IsEmpty())
            {
                var read = queryParam["read"].ToString();
                //抄表类型
                expression = expression.And(t => t.F_ReadType == read);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_ReadTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_ReadTime < endtime);
            }
            if (!queryParam["minvalue"].IsEmpty())
            {
                var minvalue = queryParam["minvalue"].ToDecimal();
                //结束时间
                expression = expression.And(t => t.F_ThisDosage >= minvalue);
            }
            if (!queryParam["maxvalue"].IsEmpty())
            {
                var maxvalue = queryParam["maxvalue"].ToDecimal();
                //结束时间
                expression = expression.And(t => t.F_ThisDosage < maxvalue);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名、安装地址
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                var taskIds = _readTaskApp.GetActiveTaskByTaskId(keyValue).Select(x => x.F_Id);
                expression = expression.And(t => taskIds.Contains(t.F_TaskId));
            }
            if (!string.IsNullOrEmpty(archiveId))
            {
                expression = expression.And(t => t.F_ArchiveId == archiveId);
            }


            return _viewService.FindList(expression, pagination);
        }
        public List<UserInspectionViewModel> GetInspectionList(Pagination pagination, string queryJson, string companyId = "")
        {

            var expression = ExtLinq.True<ReadRecordViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_ReadTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_ReadTime < endtime);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            var readList = _viewService.IQueryable(expression);
            var groupList = readList.GroupBy(x => new { x.F_ArchiveId, x.F_MeterType, x.F_UserCard, x.F_MeterCode, x.F_CustomerName, x.F_CustomerAddress, x.F_MobilePhone, x.F_Description }).Select(x => new UserInspectionViewModel
            {
                F_ArchiveId = x.Key.F_ArchiveId,
                F_UserCard = x.Key.F_UserCard,
                F_MeterCode = x.Key.F_MeterCode,
                F_MeterType = x.Key.F_MeterType,
                F_CustomerName = x.Key.F_CustomerName,
                F_CustomerAddress = x.Key.F_CustomerAddress,
                F_Description = x.Key.F_Description,
                F_MobilePhone = x.Key.F_MobilePhone,
                F_SumDosage = x.Sum(g => g.F_ThisDosage)
            });
            if (!queryParam["minvalue"].IsEmpty())
            {
                var minvalue = queryParam["minvalue"].ToDecimal();
                //开始时间
                groupList = groupList.Where(x => x.F_SumDosage >= minvalue);
            }
            if (!queryParam["maxvalue"].IsEmpty())
            {
                var maxvalue = queryParam["maxvalue"].ToDecimal();
                //结束时间
                groupList = groupList.Where(x => x.F_SumDosage < maxvalue);
            }
            pagination.records = groupList.Count();
            groupList = groupList.OrderBy(x => x.F_UserCard).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return groupList.ToList();
        }
        public List<ReadRecordViewModel> GetListByImsi(string imsi, DateTime startTime)
        {
            return _viewService.IQueryable(x => x.F_Imsi == imsi && x.F_CreatorTime > startTime).ToList();
        }

        public List<ReadRecordViewModel> GetListBySQL(Pagination pagination, string queryJson, string companyId)
        {
            string sql = "  select x.* from View_Mer_ReadRecord x   full join dbo.Arc_Meter b on b.F_Id = x.F_ArchiveId  where F_ReadTime  = (select max(F_ReadTime) from View_Mer_ReadRecord y where x.F_MeterCode = y.F_MeterCode)  and x.F_OwnerId = '" + companyId + "' ";

            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                sql += " and x.F_MeterType='" + device + "'";
            }
            if (!queryParam["factor"].IsEmpty())
            {
                var factor = queryParam["factor"].ToString();
                //生产厂商
                sql += " and x.F_Factor='" + factor + "'";
            }
            if (!queryParam["read"].IsEmpty())
            {
                var read = queryParam["read"].ToString();
                //抄表类型

                sql += " and x.F_ReadType='" + read + "'";
            }
             
            if (!queryParam["minvalue"].IsEmpty())
            {
                var minvalue = queryParam["minvalue"].ToDecimal();
                //结束时间
                sql += " and x.F_ThisDosage>=" + minvalue + "";
            }
            if (!queryParam["maxvalue"].IsEmpty())
            {
                var maxvalue = queryParam["maxvalue"].ToDecimal();
                //结束时间

                sql += " and x.F_ThisDosage<" + maxvalue + "";
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名、安装地址
                sql += " and (  x.F_UserCard like '%" + keyword + "%' or   x.F_MeterCode like '%" + keyword + "%' or   x.F_CustomerName like '%" + keyword + "%'  or   x.F_CustomerAddress like '%" + keyword + "%'  )";
            }
            sql += " order by cast(x.F_Sort as int)";


           var reault = _viewService.FindList(sql).ToList();

            pagination.records = reault.Count;

            return reault.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
        }
        public void ServiceSubmit(MeterRecord record, string imsi = "")
        {
            decimal balance = 0;
            //正在换表不记录数
            if (_readTaskApp.GetLastEntity((int)TaskTypeEnum.SetBaseDosage, record.MeterCode) != null) return;
            //获取表信息
            var meter = _meterApp.GetFormByMeterCode(record.MeterCode);
            if (meter == null) return;
            //表计倍率处理
            if (meter.F_MeterRate != null)
            {
                record.Value = (double.Parse(record.Value) * (double)meter.F_MeterRate).ToString(CultureInfo.InvariantCulture);
            }
            const int read = (int)TaskTypeEnum.Read;
            const int task = (int)TaskTypeEnum.Task;
            var readTask = _readTaskApp.GetLastEntity(task, record.MeterCode) ?? _readTaskApp.GetLastEntity(read, record.MeterCode);
            var lastDosage = decimal.Parse(GetLastReadRecord(meter.F_Id, DateTime.Now));
            //本次用量
            var thisDosage = decimal.Parse(record.Value) - lastDosage;
            //过滤异常数据并加载到异常信息表内
            //if (lastDosage != 0 && (thisDosage > 2000 || thisDosage < -2000))
            if (1 > 1)
            {
                var readUnusualEntity = new ReadUnusualEntity()
                {
                    F_Id = Common.GuId(),
                    F_CreatorTime = DateTime.Now,
                    F_ErrorType = ((int)ReadUnusualEnum.Read).ToString(),
                    F_MeterCode = meter.F_MeterCode,
                    F_MeterType = meter.F_MeterType,
                    F_Description = meter.F_Description,
                    F_CustomerAddress = meter.F_CustomerAddress,
                    F_CustomerName = meter.F_CustomerName,
                    F_Factor = meter.F_Factor,
                    F_MobilePhone = meter.F_MobilePhone,
                    F_OwnerId = meter.F_OwnerId,
                    F_UserCard = meter.F_UserCard,
                    F_Value = thisDosage,
                };
                _readUnusualApp.SubmitForm(readUnusualEntity, "");
                return;
            }
            else
            {
                var readRecordEntity = new ReadRecordEntity
                {
                    F_TaskId = readTask == null ? "" : readTask.F_Id,
                    F_ReadTime = record.ReadingTime,
                    F_ArchiveId = meter.F_Id,
                    F_ReadType = readTask == null ? "-1" : readTask.F_TaskType.ToString(),
                    F_TotalDosage = decimal.Parse(record.Value),
                    F_CreatorTime = DateTime.Now,
                    F_Id = Common.GuId(),
                    F_ThisDosage = thisDosage,
                    F_Details = record.Data == null ? "" : record.Data.ToJson(),
                    F_Settlement = record.Settlement == null ? "" : record.Settlement.ToJson(),
                    F_Imsi = imsi
                };
                ReadTaskEntity controlTask = null;
                PayOrderEntity payOrderEntity = null;
                ChargeRecordEntity chargeRecordEntity = null;
                MoneyAlarmEntity moneyAlarmEntity = null;
                //获取计费信息
                var meterCharging = _meterChargingApp.GetForm(readRecordEntity.F_ArchiveId);
                var subsistenceCharging = _subsistenceSecurityApp.GetFormByArchiveId(readRecordEntity.F_ArchiveId);
                //0.判断表是否为IC卡预付费表
                //获取当前周期开始时间
                var cycleStartTime = _priceApp.GetCycleStartTime(subsistenceCharging != null && subsistenceCharging.F_PriceId != null ? subsistenceCharging.F_PriceId : meterCharging.F_PriceModel);
                //获取当前周期开始时间前一天最后一条抄表记录
                var cycleDosage = cycleStartTime == DateTime.MinValue
                    ? 0
                    : decimal.Parse(GetLastReadRecord(readRecordEntity.F_ArchiveId, cycleStartTime));
                //单位价格
                var uintPrice = _priceApp.GetPriceValue(subsistenceCharging != null && subsistenceCharging.F_PriceId != null ? subsistenceCharging.F_PriceId : meterCharging.F_PriceModel,
                    ((readRecordEntity.F_TotalDosage ?? 0) - cycleDosage).ToString(CultureInfo.InvariantCulture));
                if (meterCharging.F_MeterModel != "3")
                {
                    //扣费后的余额
                    balance = _meterChargingApp.UpdateBalance(readRecordEntity.F_ArchiveId, (readRecordEntity.F_ThisDosage ?? 0) * uintPrice);
                }
                else
                {
                    var count = _payOrderApp.GetPayOrderCount(record.MeterCode);
                    if (record.Data != null && record.Data.Keys.Contains("充值次数") && (count + 1).ToString() == record.Data["充值次数"])
                    {
                        payOrderEntity = _payOrderApp.GetLastPayOrder(record.MeterCode);
                        if (payOrderEntity != null)
                            payOrderEntity.F_State = "1";
                    }
                    if (record.Data != null && record.Data.Keys.Contains("余额"))
                    {
                        decimal money;
                        //上传的余额
                        balance = decimal.TryParse(record.Data["余额"], out money) ? _meterChargingApp.SetBalance(readRecordEntity.F_ArchiveId, money) : 0;
                    }
                }
                chargeRecordEntity = new ChargeRecordEntity
                {
                    F_ThisBill = (readRecordEntity.F_ThisDosage ?? 0) * uintPrice,
                    F_ReadId = readRecordEntity.F_Id,
                    F_UnitPrice = uintPrice,
                    F_Description = "",
                    F_Balance = balance,
                    F_ChargeTime = DateTime.Now
                };
                //1.设备是否阀控，是否报警
                if (meterCharging.F_EnableClose == true && balance <= meterCharging.F_CloseAmount)
                {
                    //2. 判断设备类型+是否可以阀控
                    if (meter.F_MeterType == MeterTypeEnum.WattMeter.ToString()) //电表关阀命令
                    {
                        controlTask = new ReadTaskEntity
                        {
                            F_MeterCode = meter.F_MeterCode,
                            F_Factor = meter.F_Factor,
                            F_MeterType = meter.F_MeterType,
                            F_State = (int)TaskStateEnum.Wait,
                            F_Id = Common.GuId(),
                            F_CreatorTime = DateTime.Now,
                            F_TaskType = (int)TaskTypeEnum.TripSwitch
                        };
                    }
                    else //水表、燃气表、热表关阀命令
                    {
                        //判断是否应急或关阀状态
                        if (!IsEmergency(readRecordEntity))
                        {
                            controlTask = new ReadTaskEntity
                            {
                                F_MeterCode = meter.F_MeterCode,
                                F_Factor = meter.F_Factor,
                                F_MeterType = meter.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_Id = Common.GuId(),
                                F_CreatorTime = DateTime.Now,
                                F_TaskType = (int)TaskTypeEnum.CloseValve
                            };
                        }
                    }
                }
                if (meterCharging.F_EnableClose == true && balance > meterCharging.F_CloseAmount)
                {
                    if (meter.F_MeterType == MeterTypeEnum.WattMeter.ToString()) //电表开阀命令
                    {
                        //电表充值自动开阀，不启用
                        //controlTask = new ReadTaskEntity
                        //{
                        //    F_MeterCode = meter.F_MeterCode,
                        //    F_Factor = meter.F_Factor,
                        //    F_MeterType = meter.F_MeterType,
                        //    F_State = (int)TaskStateEnum.Wait,
                        //    F_Id = Common.GuId(),
                        //    F_CreatorTime = DateTime.Now,
                        //    F_TaskType = (int)TaskTypeEnum.CloseSwitch
                        //};
                    }
                    else //水表、燃气表、热表开阀命令
                    {
                        //判断是否应急或关阀状态
                        if (IsEmergency(readRecordEntity))
                        {
                            controlTask = new ReadTaskEntity
                            {
                                F_MeterCode = meter.F_MeterCode,
                                F_Factor = meter.F_Factor,
                                F_MeterType = meter.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_Id = Common.GuId(),
                                F_CreatorTime = DateTime.Now,
                                F_TaskType = (int)TaskTypeEnum.OpenValve
                            };
                        }
                    }
                }
                if (meterCharging.F_EnableAlarm == true && balance <= meterCharging.F_AlarmAmount)
                {
                    if (_moneyAlarmApp.IsCanSend(meter.F_Id))//判断是否可报警
                    {
                        var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
                        var content = SmsHelper.UrlEncode("#name#=" + meter.F_CustomerName + "&#time#=" + DateTime.Now.ToString("yyyy年MM月dd日") + "&#meter#=" + meter.F_CustomerAddress + ((Dictionary<string, string>)dataItems["DeviceType"])[meter.F_MeterType] + "&#balance#=" + balance.ToString("#0.00"));
                        var content2 = SmsHelper.UrlEncode("#name#=" + meter.F_CustomerName + "&#time#=" + DateTime.Now.ToString("yyyy年MM月dd日") + "&#meter#=" + ((Dictionary<string, string>)dataItems["DeviceType"])[meter.F_MeterType] + "&#balance#=" + balance.ToString("#0.00"));
                        moneyAlarmEntity = new MoneyAlarmEntity
                        {
                            F_Id = Common.GuId(),
                            F_ArchiveId = meter.F_Id,
                            F_AlarmMonry = balance,
                            F_AlarmState = (meter.F_OwnerId == "ff92def0-dabe-4878-915b-1b8cd0560ce4" && SmsHelper.Send(meter.F_MobilePhone, "58698", content, "c9eb4d0f54ad274842ef562d79f67e59")) || (meter.F_OwnerId == "bd602a97-eb06-4a5e-9c36-30eb45bc717b" && SmsHelper.Send(meter.F_MobilePhone, "96473", content2, "f73188115023c7e37955b095fefe0bb7")), //发送短信
                            F_SureState = false,
                            F_CreatorTime = DateTime.Now,
                        };
                    }
                }
                else
                {
                    _moneyAlarmApp.SureAlarm(meter.F_Id);
                }
                //3.插入抄表信息、计费信息、阀控命令/报警命令
                _service.SubmitService(readRecordEntity, chargeRecordEntity, readTask, controlTask, payOrderEntity, moneyAlarmEntity);
            }
        }
        /// <summary>
        /// 获取上次抄表总用量
        /// </summary>
        /// <param name="archiveId">表计序号</param>
        /// <param name="endTime"></param>
        /// <returns>上次抄表总用量或表计默认值</returns>
        public string GetLastReadRecord(string archiveId, DateTime endTime)
        {
            var meter = _meterChargingApp.GetForm(archiveId);
            var initialVal = meter.F_StartValue ?? 0;
            var lastRecord = _service.IQueryable(x => x.F_ArchiveId == archiveId && x.F_CreatorTime < endTime).SortByDescending(x => x.F_CreatorTime).FirstOrDefault();
            return lastRecord == null ? initialVal.ToString(CultureInfo.InvariantCulture) : lastRecord.F_TotalDosage.ToString();
        }
        public ReadRecordViewModel GetLastReadRecordByMeterCode(string meterCode, DateTime endTime)
        {
            var lastRecord = _viewService.IQueryable(x => x.F_MeterCode == meterCode && x.F_ReadTime < endTime).SortByDescending(x => x.F_ReadTime).FirstOrDefault();
            return lastRecord;
        }
        public ReadRecordViewModel GetLastReadRecordByImsi(string imsi, DateTime startTime, DateTime endTime)
        {
            var lastRecord = _viewService.IQueryable(x => x.F_Imsi == imsi && x.F_CreatorTime >= startTime && x.F_ReadTime < endTime).SortByDescending(x => x.F_ReadTime).FirstOrDefault();
            return lastRecord;
        }

        /// <summary>
        /// 获取未计费抄表记录
        /// </summary>
        /// <returns></returns>
        public List<ReadRecordEntity> GetNonChargeList()
        {
            return _service.FindList("select * from dbo.Mer_ReadRecord where F_Id not in (select F_ReadId from Pay_ChargeRecord) and F_ReadTime<DATEADD(hh,-1,GETDATE())");
        }

        public List<UnusualViewModel> GetNonOnlineList(Pagination pagination = null, string queryJson = "", string companyId = "")
        {
            var expression = ExtLinq.True<UnusualViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
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
            expression = expression.And(t => t.F_DeleteMark != true);
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            if (!queryParam["begintime"].IsEmpty() && !queryParam["endtime"].IsEmpty())
            {
                var begintime = DateTime.Parse(queryParam["begintime"].ToString());
                var endtime = DateTime.Parse(queryParam["endtime"].ToString()).AddDays(1);
                var onlineList = _viewService.IQueryable(x => x.F_CreatorTime > begintime && x.F_CreatorTime < endtime && x.F_OwnerId == companyId).Select(x => x.F_ArchiveId).Distinct().ToList();
                expression = expression.And(t => !onlineList.Any(y => y == t.F_Id));
            }
            if (pagination == null)
                return _unusualService.IQueryable(expression).ToList();
            else
                return _unusualService.FindList(expression, pagination).ToList();
        }
        /// <summary>
        /// 获取未转发抄表记录
        /// </summary>
        /// <returns></returns>
        public List<ReadRecordViewModel> GetNonForwardList(string ownerId)
        {
            return _viewService.FindList("select * from dbo.View_Mer_ReadRecord where F_Id not in (select F_Id from Mer_ForwardTask) and F_ArchiveId in(select F_Id from Arc_Meter where F_OwnerId='" + ownerId + "')");
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        public List<ReadRecordViewModel> DownLoad(string queryJson, string companyId)
        {
            var expression = ExtLinq.True<ReadRecordViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["factor"].IsEmpty())
            {
                var factor = queryParam["factor"].ToString();
                //生产厂商
                expression = expression.And(t => t.F_Factor == factor);
            }
            if (!queryParam["read"].IsEmpty())
            {
                var read = queryParam["read"].ToString();
                //抄表类型
                expression = expression.And(t => t.F_ReadType == read);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_ReadTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_ReadTime < endtime);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名、安装地址
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            var recordList = _viewService.IQueryable(expression).OrderByDescending(x => x.F_CreatorTime).ToList();
            return recordList;
        }
        public decimal[][] GetMonthTotalByMeterType(string[] meterType, string companyId)
        {
            var datas = new decimal[meterType.Count()][];
            var now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, 1);
            var recordList = _viewService.IQueryable(x => x.F_OwnerId == companyId);
            for (int i = 0; i < meterType.Count(); i++)
            {
                var data = new decimal[12];
                var type = meterType[i];
                for (var j = 0; j < 12; j++)
                {
                    var start = now.AddMonths(-j);
                    var end = start.AddMonths(1);
                    data[11 - j] = recordList.Where(x => x.F_MeterType == type && x.F_ReadTime >= start && x.F_ReadTime < end).ToList().Select(x => Convert.ToDecimal(x.F_ThisDosage)).Sum(x => x);
                }
                datas[i] = data;
            }
            return datas;
        }
        public ReadRecordViewModel GetForm(string keyValue)
        {
            var readRecordEntity = _viewService.FindEntity(keyValue);
            var taskList = _readTaskApp.GetSubtaskList(readRecordEntity.F_TaskId);
            var details = readRecordEntity.F_Details.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            if (!taskList.Any()) return readRecordEntity;
            foreach (var item in taskList)
            {
                switch (item.F_TaskType)
                {
                    case 111:
                        details.Add("实时电压", item.F_Value);
                        break;
                    case 112:
                        details.Add("实时电流", item.F_Value);
                        break;
                    case 114:
                        details.Add("尖时段电能", item.F_Value);
                        break;
                    case 115:
                        details.Add("峰时段电能", item.F_Value);
                        break;
                    case 116:
                        details.Add("平时段电能", item.F_Value);
                        break;
                    case 117:
                        details.Add("谷时段电能", item.F_Value);
                        break;
                }
            }
            readRecordEntity.F_Details = details.ToJson();
            return readRecordEntity;
        }
        public bool IsEmergency(ReadRecordEntity lastRecord)
        {
            try
            {
                if (lastRecord == null) return false;
                var flag = false;
                var meterState = lastRecord.F_Details.ToObject<Dictionary<string, string>>();
                if (meterState.ContainsKey("应急标志"))
                {
                    flag = meterState["应急标志"] != "正常";
                }
                if (meterState.ContainsKey("阀门状态标志"))
                {
                    flag = meterState["阀门状态标志"] != "开阀";
                }
                if (meterState.ContainsKey("阀门状态"))
                {
                    flag = meterState["阀门状态"] != "开阀";
                }
                return flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<ReadRecordViewModel> GetMonthData(string meterCode, DateTime startTime)
        {
            var endTime = startTime.AddMonths(1);
            return
                _viewService.IQueryable(
                    x =>
                        x.F_MeterCode == meterCode && x.F_ReadTime >= startTime && x.F_ReadTime < endTime).ToList();
        }
        public bool ClearRecord(string imsi)
        {
            if (!string.IsNullOrEmpty(imsi) && imsi != "000000000000000")
                return _service.Delete(x => x.F_Imsi == imsi) > 0;
            else return false;
        }

        #region ==SQL语句查询抄表记录==
        public List<ReadRecordViewModel> GetListByT_SQL(Pagination pagination, string queryJson, string companyId = "", string keyValue = "", string archiveId = "")
        {
            #region 查询

            string sql = " 1=1 ";
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                sql += " and F_MeterType='" + device + "'";
            }
            if (!queryParam["factor"].IsEmpty())
            {
                var factor = queryParam["factor"].ToString();
                //生产厂商
                sql += " and F_Factor='" + factor + "'";
            }
            if (!queryParam["read"].IsEmpty())
            {
                var read = queryParam["read"].ToString();
                //抄表类型

                sql += " and F_ReadType='" + read + "'";
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间

                sql += " and F_ReadTime>='" + begintime.ToString("yyyy-MM-dd 00:00:00") + "'";
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                sql += " and F_ReadTime<'" + endtime + "'";
            }
            if (!queryParam["minvalue"].IsEmpty())
            {
                var minvalue = queryParam["minvalue"].ToDecimal();
                //结束时间
                sql += " and F_ThisDosage>=" + minvalue + "";
            }
            if (!queryParam["maxvalue"].IsEmpty())
            {
                var maxvalue = queryParam["maxvalue"].ToDecimal();
                //结束时间

                sql += " and F_ThisDosage<" + maxvalue + "";
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名、安装地址
                sql += " and (  F_UserCard like '%" + keyword + "%' or   F_MeterCode like '%" + keyword + "%' or   F_CustomerName like '%" + keyword + "%'  or   F_CustomerAddress like '%" + keyword + "%'  )";
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                sql += " and  F_OwnerId='" + companyId + "'";
            }
            if (!string.IsNullOrEmpty(archiveId))
            {
                sql += " and  F_ArchiveId='" + archiveId + "'";
            }

            #endregion

            int startIndex = (pagination.page - 1) * pagination.rows + 1;
            int endIndex = pagination.page * pagination.rows;

            int total = GetListByPageCount(sql, "", startIndex, endIndex);

            pagination.records = total;

            return GetListByPage(sql, pagination.sidx, startIndex, endIndex);
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public List<ReadRecordViewModel> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by CAST( T.F_Sort as INT) ");
            }
            strSql.Append(")AS Row, T.*  from View_Mer_ReadRecord T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            DataTable dt = Data.Extensions.DbHelper.QueryDB(strSql.ToString());

            List<ReadRecordViewModel> obj = new ModelConvertHelper<ReadRecordViewModel>().ConvertToModel(dt);

            return obj;
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public int GetListByPageCount(string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT 	top 1 TT.Row  FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by CAST( T.F_Sort as INT) ");
            }
            strSql.Append(")AS Row, T.*  from View_Mer_ReadRecord T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" ORDER BY TT.Row DESC");

            DataTable dt = Data.Extensions.DbHelper.QueryDB(strSql.ToString());
            int total = 0;
            if (dt.Rows.Count > 0)
            {
                total = Convert.ToInt32(dt.Rows[0][0]);
            }

            return total;
        }


        #endregion
    }

    /// <summary>
    /// 模型转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelConvertHelper<T> where T : new()
    {
        /// <summary>
        /// DataTable转泛型
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> ConvertToModel(DataTable dt)
        {
            // 定义集合    
            List<T> ts = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列    

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }
            return ts;
        }

    }
}