/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Data;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.IRepository.MeterReading;

namespace NFine.Repository.MeterReading
{
    public class TaskListRepository : RepositoryBase<TaskListEntity>, ITaskListRepository
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Delete<TaskListEntity>(t => t.F_Id == keyValue);
                db.Delete<ReadTaskEntity>(t => t.F_Param == keyValue);
                db.Commit();
            }
        }
        public void SubmitForm(TaskListEntity taskListEntity, List<ReadTaskEntity> readTaskList)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {

                db.Insert(taskListEntity);
                db.Insert(readTaskList);
                db.Commit();
            }
        }
    }
}