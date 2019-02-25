﻿/*******************************************************************************
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
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.IRepository.PaymentManage;
using NFine.Domain.ViewModel.PaymentManage;

namespace NFine.Repository.PaymentManage
{
    public class ChargeRecordRepository : RepositoryBase<ChargeRecordEntity>, IChargeRecordRepository
    {
        public void SubmitForm(ChargeRecordEntity chargeRecordEntity, ReadTaskEntity readTaskEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Insert(chargeRecordEntity);
                if (readTaskEntity != null)
                    db.Insert(readTaskEntity);
                db.Commit();
            }
        }
    }
    public class ChargeRecordViewRepository : RepositoryBase<ChargeRecordViewModel>, IChargeRecordViewRepository
    {

    }
}
