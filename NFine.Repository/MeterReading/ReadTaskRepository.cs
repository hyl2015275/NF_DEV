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
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.IRepository.MeterReading;
using NFine.Domain.ViewModel.MeterReading;

namespace NFine.Repository.MeterReading
{
    public class ReadTaskRepository : RepositoryBase<ReadTaskEntity>, IReadTaskRepository
    {
        public void SubmitRechargeMoney(ReadTaskEntity readTaskEntity, PayOrderEntity payOrderEntity)
        {

            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Update(readTaskEntity);
                db.Update(payOrderEntity);
                db.Commit();
            }
        }

        public void SubmitChangeMeter(ReadTaskEntity readTaskEntity, ChangeMeterEntity changeMeterEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Update(readTaskEntity);
                db.ExecuteSqlCommand("update dbo.Mer_ReadRecord set F_ArchiveId='" + changeMeterEntity.F_ArchiveId + "' where F_ArchiveId='" + changeMeterEntity.F_OldArchiveId + "'");
                db.Commit();
            }
        }
    }
    public class ReadTaskViewRepository : RepositoryBase<ReadTaskViewModel>, IReadTaskViewRepository
    {

    }
}