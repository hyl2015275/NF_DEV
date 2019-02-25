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
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.IRepository.PaymentManage;
using NFine.Domain.ViewModel.PaymentManage;

namespace NFine.Repository.PaymentManage
{
    public class RefundOrderRepository : RepositoryBase<RefundOrderEntity>, IRefundOrderRepository
    {
        public void SubmitForm(RefundOrderEntity payOrderEntity, MeterChargingEntity meterChargingEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Insert(payOrderEntity);
                db.Update(meterChargingEntity);
                db.Commit();
            }
        }

    }
    public class RefundOrderViewRepository : RepositoryBase<RefundOrderViewModel>, IRefundOrderViewRepository
    {

    }
}