using System;
using System.Collections.Generic;
using System.Linq;
using NFine.Application.ArchiveManage;
using NFine.Code;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Enum;
using NFine.Domain.IRepository.MeterReading;
using NFine.Domain.ViewModel.ArchiveManage;
using NFine.Repository.MeterReading;

namespace NFine.Application.MeterReading
{
    public class TaskListApp
    {
        public delegate string MethodCaller(string command, string url, int spaceInt);//定义个代理 
        private readonly ITaskListRepository _service = new TaskListRepository();
        private readonly ReadTaskApp _readTaskApp = new ReadTaskApp();
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly ChannelApp _channelApp = new ChannelApp();
        public List<TaskListEntity> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<TaskListEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //任务名称
                expression = expression.And(t => t.F_TaskName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            expression = expression.And(t => t.F_DeleteMark != true && t.F_LoopMark != true);
            var list = _service.FindList(expression, pagination);
            foreach (var item in list)
            {
                var tasks = _readTaskApp.GetActiveTaskByTaskId(item.F_Id);
                item.F_SuccessNumber = tasks.Count(x => x.F_State == (int)TaskStateEnum.Finish);
                item.F_FailNumber = tasks.Count(x => x.F_State == (int)TaskStateEnum.Error);
                item.F_Speed = tasks.Count(x => x.F_State != (int)TaskStateEnum.Wait) + "/" + item.F_DeviceNumber;
            }
            return list;
        }

        public List<TaskListEntity> GetList(string companyId = "", bool loopMark = true)
        {
            return string.IsNullOrEmpty(companyId) ? _service.IQueryable(x => x.F_DeleteMark != true && x.F_LoopMark == loopMark).ToList() : _service.IQueryable(x => x.F_DeleteMark != true && x.F_LoopMark == loopMark && x.F_OwnerId == companyId).ToList();
        }

        public void SubmitForm(TaskListEntity taskListEntity, string companyId)
        {
            var tasks = new List<ReadTaskEntity>();
            var meterTypes = new List<string>
            {
                taskListEntity.F_MeterType
            };
            var meters = _meterApp.GetListByMeterType(meterTypes, companyId);
            var taskMeter = new List<MeterViewModel>();
            taskListEntity.F_DeviceNumber = meters.Count;
            taskListEntity.F_LoopMark = false;
            taskListEntity.Create();
            if (meters.Count <= 0) return;
            foreach (var item in meters)
            {
                var channel = _channelApp.GetFormByArchiveId(item.F_Id);
                if (channel != null)
                {
                    var readTaskEntity = new ReadTaskEntity
                    {
                        F_MeterType = item.F_MeterType,
                        F_CreatorTime = DateTime.Now,
                        F_State = (int)TaskStateEnum.Wait,
                        F_Factor = item.F_Factor,
                        F_MeterCode = item.F_MeterCode,
                        F_Id = Common.GuId(),
                        F_WorkId = taskListEntity.F_Id,
                        F_Param = channel.F_Id,
                        F_TaskType = (int)TaskTypeEnum.Read,
                        F_ChannelType = (int)ChannelTypeEnum.Mbus
                    };
                    tasks.Add(readTaskEntity);
                }
                else if (meterTypes.Contains(MeterTypeEnum.WattMeter.ToString()))
                {
                    taskMeter.Add(item);
                }
            }
            tasks.AddRange(new ReadTaskApp().WattReadWithOther(taskMeter, taskListEntity.F_Id, TaskTypeEnum.Read));
            _service.SubmitForm(taskListEntity, tasks);
        }

        public void SubmitCustomTaskForm(TaskListEntity taskListEntity, string keyValue)
        {
            taskListEntity.F_LoopMark = true;
            if (!string.IsNullOrEmpty(keyValue))
            {
                taskListEntity.Modify(keyValue);
                _service.Update(taskListEntity);
            }
            else
            {
                taskListEntity.Create();
                _service.Insert(taskListEntity);
            }
        }
        public TaskListEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }

        public void DeleteForm(string keyValue)
        {
            _service.DeleteForm(keyValue);
        }

        public void DeleteCustomTaskForm(string keyValue, string companyId)
        {
            var taskListNumber = _service.IQueryable(x => x.F_OwnerId == companyId && x.F_DeleteMark != true && x.F_LoopMark).Count();
            if (taskListNumber <= 1)
            {
                throw new Exception("删除失败！必须保证有一项存在。");
            }
            _service.Delete(t => t.F_Id == keyValue);
        }
    }
}