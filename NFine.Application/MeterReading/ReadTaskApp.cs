/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NFine.Application.ArchiveManage;
using NFine.Application.PaymentManage;
using NFine.Application.QuipmentBase;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Enum;
using NFine.Domain.IRepository.MeterReading;
using NFine.Domain.ViewModel.ArchiveManage;
using NFine.Domain.ViewModel.MeterReading;
using NFine.Repository.MeterReading;

namespace NFine.Application.MeterReading
{
    public class ReadTaskApp
    {
        private readonly IReadTaskRepository _service = new ReadTaskRepository();
        private readonly IReadTaskViewRepository _viewService = new ReadTaskViewRepository();
        private readonly PayOrderApp _payOrderApp = new PayOrderApp();
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly ConnectApp _conectApp = new ConnectApp();
        private readonly ChannelMeterApp _channelMeterApp = new ChannelMeterApp();
        /// <summary>
        /// 提交表单
        /// </summary>
        /// <param name="readTaskEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(ReadTaskEntity readTaskEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                _service.Update(readTaskEntity);
            }
            else
            {
                var meter = _meterApp.GetFormByMeterCode(readTaskEntity.F_MeterCode);
                if (meter != null)
                {
                    var channel = new ChannelApp().GetFormByArchiveId(meter.F_Id);
                    if (channel != null)
                    {
                        readTaskEntity.F_Param = channel.F_Id;
                        readTaskEntity.F_ChannelType = (int)ChannelTypeEnum.Mbus;
                    }
                }
                readTaskEntity.F_Id = Common.GuId();
                readTaskEntity.F_CreatorTime = DateTime.Now;
                _service.Insert(readTaskEntity);
            }
        }
        /// <summary>
        /// 新增任务
        /// </summary>
        /// <param name="connect"></param>
        public void Insert(List<ReadTaskEntity> connect)
        {
            _service.Insert(connect);
        }
        /// <summary>
        /// 获取子任务
        /// </summary>
        /// <param name="taskId">任务编号</param>
        /// <returns></returns>
        public List<ReadTaskEntity> GetSubtaskList(string taskId)
        {
            return _service.IQueryable(x => x.F_WorkId == taskId && x.F_State == 1).ToList();
        }
        /// <summary>
        /// 获取同类型任务列表
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public List<ReadTaskEntity> GetList(int taskType)
        {
            return _service.IQueryable(x => x.F_TaskType == taskType && x.F_State < 1 && x.F_State > -5).ToList();
        }
        /// <summary>
        /// 获取表任务列表
        /// </summary>
        /// <param name="meterCode"></param>
        /// <returns></returns>
        public List<ReadTaskEntity> GetListByMeterCode(string meterCode, bool withoutRead = false)
        {
            if (withoutRead)
                return _service.IQueryable(x => x.F_MeterCode == meterCode && x.F_State < 1 && x.F_State > -5 && x.F_TaskType != 0 && x.F_TaskType != 1).ToList();
            else
                return _service.IQueryable(x => x.F_MeterCode == meterCode && x.F_State < 1 && x.F_State > -5).ToList();
        }
        public ReadTaskEntity GetTaskByDtuId(string dutId)
        {
            var meters = _channelMeterApp.GetMeterListByChannel(dutId);
            if (meters != null && meters.Count > 0)
            {
                var lastTime = DateTime.Now.AddDays(-1);//一天内的命令
                var task = _service.FindEntity(x => x.F_CreatorTime > lastTime && meters.Contains(x.F_MeterCode) && x.F_State < 1 && x.F_State > -5);
                if (task != null)
                {
                    task.F_State = task.F_State - 1;
                    _service.Update(task);
                    return task;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(string keyValue)
        {
            _service.Delete(t => t.F_Id == keyValue);
        }
        /// <summary>
        /// 获取表单
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<ReadTaskViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<ReadTaskViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["task"].IsEmpty())
            {
                var task = int.Parse(queryParam["task"].ToString());
                //任务类型
                expression = expression.And(t => t.F_TaskType == task);
            }
            else
            {
                expression = expression.And(t => t.F_TaskType != 0 && t.F_TaskType != 1);
            }
            if (!queryParam["result"].IsEmpty())
            {
                var result = int.Parse(queryParam["result"].ToString());
                expression = expression.And(t => t.F_State == result);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //表计编码
                expression = expression.And(t => t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            expression = expression.And(t => t.F_WorkId == null && t.F_DeleteMark != true);
            return _viewService.FindList(expression, pagination);
        }
        /// <summary>
        /// 获取电表任务
        /// </summary>
        /// <returns></returns>
        public List<ReadTaskEntity> GetTimeTask()
        {
            var wattMeter = MeterTypeEnum.WattMeter.ToString();
            var longMeterList = _conectApp.GetLongMeterList().Select(x => x.MeterCode);
            return _service.IQueryable(x => x.F_MeterType == wattMeter && longMeterList.Contains(x.F_MeterCode) && x.F_State < 1 && x.F_State > -5).OrderBy(x => x.F_CreatorTime).ToList();
        }

        public ReadTaskEntity GetTaskByChannelType(int channelType)
        {
            return
                _service.IQueryable(x => x.F_ChannelType == channelType && x.F_State < 1 && x.F_State > -5).FirstOrDefault();
        }

        /// <summary>
        /// 获取任务抄表列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public List<ReadTaskEntity> GetActiveTaskByTaskId(string taskId)
        {
            return _service.IQueryable(x => x.F_WorkId == taskId).ToList();
        }
        /// <summary>
        /// 获取设备未完成任务
        /// </summary>
        /// <param name="deviceId">设备编号</param>
        /// <param name="taskType">任务类型</param>
        /// <returns></returns>
        public ReadTaskEntity GetFistTask(string deviceId, string taskType = "")
        {
            //一个月内的任务
            var startTime = DateTime.Now.AddMonths(-1);
            var endTime = DateTime.Now;
            const int state = (int)TaskStateEnum.Wait;
            if (!string.IsNullOrEmpty(taskType))
            {
                var type = 0;
                foreach (TaskTypeEnum suit in Enum.GetValues(typeof(TaskTypeEnum)))
                {
                    if (suit.ToString() == taskType)
                    {
                        type = (int)suit;
                    }
                }
                return _service.IQueryable(x => x.F_CreatorTime > startTime && x.F_CreatorTime < endTime && x.F_MeterCode == deviceId && x.F_TaskType == type && x.F_State == state)
                    .OrderBy(x => x.F_CreatorTime)
                    .FirstOrDefault();
            }
            const int read = (int)TaskTypeEnum.Read;
            const int active = (int)TaskTypeEnum.Task;
            const int module = (int)TaskTypeEnum.ModuleAddress;
            return
                _service.IQueryable(x => x.F_CreatorTime > startTime && (x.F_MeterCode == deviceId || (x.F_Param == deviceId && x.F_TaskType == module)) && x.F_TaskType != read && x.F_TaskType != active && x.F_State == state)
                    .OrderBy(x => x.F_CreatorTime)
                    .FirstOrDefault();
        }
        public ReadTaskEntity GetLastEntity(int taskType, string meterCode)
        {
            //一个月内的任务
            var startTime = DateTime.Now.AddMonths(-1);
            return
                _service.IQueryable(
                        x => x.F_TaskType == taskType && x.F_State < 1 && x.F_State > -5 && x.F_MeterCode == meterCode && x.F_CreatorTime >= startTime)
                    .OrderByDescending(x => x.F_CreatorTime)
                    .FirstOrDefault();
        }
        /// <summary>
        /// 任务回调
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="record"></param>
        public void SetConfirmationTask(string deviceId, MeterRecord record = null, bool flag = true)
        {
            var taskType = record == null ? null : record.TaskType;
            var fistTask = GetFistTask(deviceId, taskType);
            if (fistTask == null) return;
            if (flag)
            {
                fistTask.F_State = (int)TaskStateEnum.Finish;

            }
            else
            {
                fistTask.F_State = (int)TaskStateEnum.Error;
            }
            fistTask.F_ExecuteTime = DateTime.Now;
            fistTask.F_Value = record == null ? null : record.Value;
            switch (fistTask.F_TaskType)
            {
                case (int)TaskTypeEnum.Pay:
                    {
                        var payOrder = _payOrderApp.FindEntity(deviceId);
                        if (payOrder != null)
                        {
                            payOrder.F_State = ((int)PayStateEnum.Finish).ToString();
                            _service.SubmitRechargeMoney(fistTask, payOrder);
                        }
                    }
                    break;
                case (int)TaskTypeEnum.SetBaseDosage:
                    {
                        var changeMeter = new ChangeMeterApp().GetForm(fistTask.F_WorkId);
                        if (fistTask.F_WorkId != "xxxxxxxx-xxxx-xxxx-xxxx-000000000005" && changeMeter != null)
                        {
                            _service.SubmitChangeMeter(fistTask, changeMeter);
                        }
                        else
                        {
                            _service.Update(fistTask);
                        }
                    }
                    break;
                default:
                    {
                        _service.Update(fistTask);
                    }
                    break;
            }
        }

        public void SetPassThroughTask(string deviceId, string cmd)
        {
            var fistTask = GetFistTask(deviceId);
            if (fistTask == null) return;
            fistTask.F_ExecuteTime = DateTime.Now;
            fistTask.F_Value = cmd;
            fistTask.F_State = (int)TaskStateEnum.Finish;
            _service.Update(fistTask);
        }
        /// <summary>
        /// 电表抄表超时
        /// </summary>
        public void OverTime()
        {
            var log = LogFactory.GetLogger("OverTimeTask");
            var activeTasks = GetTimeTask();
            foreach (var item in activeTasks)
            {
                var stratTime = item.F_CreatorTime ?? DateTime.MinValue;
                if (stratTime.AddMinutes(5) <= DateTime.Now)
                {
                    item.F_State = -5;
                    item.F_ExecuteTime = DateTime.Now;
                    _service.Update(item);
                }
                log.Error(item.F_Id + "任务超时");
            }
        }
        /// <summary>
        /// 转发数据
        /// </summary>
        /// <param name="readList">抄表记录集合</param>
        public void ForwardData(List<ReadRecordViewModel> readList)
        {
            foreach (var item in readList)
            {
                new ForwardTaskApp().SubmitForm(new ForwardTaskEntity
                {
                    F_Id = item.F_Id,
                    F_Status = new ForwardTaskApp().PostData(item),
                    F_CreatorTime = DateTime.Now
                });
            }
        }
        /// <summary>
        /// 电表抄表
        /// </summary>
        /// <param name="meters">电表集合</param>
        /// <param name="connectId">任务编号</param>
        /// <param name="taskType">任务类型</param>
        /// <returns></returns>
        public List<ReadTaskEntity> WattReadWithOther(List<MeterViewModel> meters, string connectId, TaskTypeEnum taskType)
        {
            var taskWithOther = new List<ReadTaskEntity>();
            var tasks = meters.Select(item => new ReadTaskEntity
            {
                F_Id = Common.GuId(),
                F_WorkId = connectId,
                F_CreatorTime = DateTime.Now,
                F_Factor = item.F_Factor,
                F_MeterCode = item.F_MeterCode,
                F_State = (int)TaskStateEnum.Wait,
                F_MeterType = MeterTypeEnum.WattMeter.ToString(),
                F_TaskType = (int)taskType,
            }).ToList();
            taskWithOther.AddRange(tasks);
            foreach (var item in tasks)
            {
                taskWithOther.AddRange(new List<ReadTaskEntity>
                        {
                            new ReadTaskEntity//抄电压
                            {
                                F_Id = Guid.NewGuid().ToString(),
                                F_Factor = item.F_Factor,
                                F_CreatorTime = DateTime.Now,
                                F_MeterCode = item.F_MeterCode,
                                F_MeterType = item.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_TaskType = (int)TaskTypeEnum.ThreeVoltage,
                                F_WorkId=item.F_Id
                            },
                            new ReadTaskEntity//抄电流
                            {
                                F_Id = Guid.NewGuid().ToString(),
                                F_Factor = item.F_Factor,
                                F_CreatorTime = DateTime.Now,
                                F_MeterCode = item.F_MeterCode,
                                F_MeterType = item.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_TaskType = (int)TaskTypeEnum.ThreeCurrent,
                                F_WorkId=item.F_Id
                            },
                            new ReadTaskEntity//抄运行状态
                            {
                                F_Id = Guid.NewGuid().ToString(),
                                F_Factor = item.F_Factor,
                                F_CreatorTime = DateTime.Now,
                                F_MeterCode = item.F_MeterCode,
                                F_MeterType = item.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_TaskType =(int)TaskTypeEnum.RunningState,
                                F_WorkId=item.F_Id
                            },
                             new ReadTaskEntity//抄尖时段电能
                            {
                                F_Id = Guid.NewGuid().ToString(),
                                F_Factor = item.F_Factor,
                                F_CreatorTime = DateTime.Now,
                                F_MeterCode = item.F_MeterCode,
                                F_MeterType = item.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_TaskType =(int)TaskTypeEnum.TipEnergy,
                                F_WorkId=item.F_Id
                            },
                             new ReadTaskEntity//抄峰时段电能
                            {
                                F_Id = Guid.NewGuid().ToString(),
                                F_Factor = item.F_Factor,
                                F_CreatorTime = DateTime.Now,
                                F_MeterCode = item.F_MeterCode,
                                F_MeterType = item.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_TaskType =(int)TaskTypeEnum.PeakEnergy,
                                F_WorkId=item.F_Id
                            },
                             new ReadTaskEntity//抄平时段电能
                            {
                                F_Id = Guid.NewGuid().ToString(),
                                F_Factor = item.F_Factor,
                                F_CreatorTime = DateTime.Now,
                                F_MeterCode = item.F_MeterCode,
                                F_MeterType = item.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_TaskType =(int)TaskTypeEnum.FlatEnergy,
                                F_WorkId=item.F_Id
                            },
                             new ReadTaskEntity//抄谷时段电能
                            {
                                F_Id = Guid.NewGuid().ToString(),
                                F_Factor = item.F_Factor,
                                F_CreatorTime = DateTime.Now,
                                F_MeterCode = item.F_MeterCode,
                                F_MeterType = item.F_MeterType,
                                F_State = (int)TaskStateEnum.Wait,
                                F_TaskType =(int)TaskTypeEnum.ValleyEnergy,
                                F_WorkId=item.F_Id
                            },
                        });

            }
            return taskWithOther;
        }

        public bool RelevanceMeterCode(string meterCode, string imsi)
        {
            _meterApp.InsertTestMeter(meterCode);
            var task = new ReadTaskEntity()
            {
                F_Id = Common.GuId(),
                F_MeterCode = imsi,
                F_MeterType = "WaterMeter",
                F_Factor = "贝林电子",
                F_TaskType = (int)TaskTypeEnum.ModuleAddress,
                F_State = 0,
                F_Param = meterCode,
                F_WorkId = "xxxxxxxx-xxxx-xxxx-xxxx-000000001102",
                F_CreatorTime = DateTime.Now
            };
            return _service.Insert(task) > 0;
        }
        public bool RelieveMeterCode(string meterId, string imsi)
        {
            var taskType = (int)TaskTypeEnum.ModuleAddress;
            _meterApp.DeleteForm(meterId);
            return _service.Delete(x => x.F_MeterCode == imsi && x.F_TaskType == taskType && x.F_State == 0) > 0;
        }
        public bool ClearDosage(string imsi)
        {
            var task = new ReadTaskEntity()
            {
                F_Id = Common.GuId(),
                F_MeterCode = imsi,
                F_MeterType = "WaterMeter",
                F_Factor = "贝林电子",
                F_TaskType = (int)TaskTypeEnum.SetBaseDosage,
                F_State = 0,
                F_Param = "0",
                F_WorkId = "xxxxxxxx-xxxx-xxxx-xxxx-000000000005",
                F_CreatorTime = DateTime.Now
            };
            return _service.Insert(task) > 0;
        }
    }
}