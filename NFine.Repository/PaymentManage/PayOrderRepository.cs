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
using NFine.Domain.IRepository.PaymentManage;
using NFine.Domain.ViewModel.PaymentManage;

namespace NFine.Repository.PaymentManage
{
    public class PayOrderRepository : RepositoryBase<PayOrderEntity>, IPayOrderRepository
    {
        public void SubmitForm(PayOrderEntity payOrderEntity, MeterChargingEntity meterChargingEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Insert(payOrderEntity);
                db.Update(meterChargingEntity);
                db.Commit();
            }
        }

        public void SubmitForm(PayOrderEntity payOrderEntity, MeterChargingEntity meterChargingEntity, ReadTaskEntity readTaskEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Insert(payOrderEntity);
                db.Update(meterChargingEntity);
                if (readTaskEntity != null)
                {
                    db.Insert(readTaskEntity);
                }
                db.Commit();
            }
        }

    }
    public class PayOrderViewRepository : RepositoryBase<PayOrderViewModel>, IPayOrderViewRepository
    {

    }
}
