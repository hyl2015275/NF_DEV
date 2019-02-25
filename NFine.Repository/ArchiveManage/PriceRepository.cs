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
using NFine.Code;
using NFine.Data;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;

namespace NFine.Repository.ArchiveManage
{
    public class PriceRepository : RepositoryBase<PriceEntity>, IPriceRepository
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Delete<PriceEntity>(t => t.F_Id == keyValue);
                db.Delete<PriceDetailsEntity>(t => t.F_PriceId == keyValue);
                db.Delete<PriceBaseEntity>(t => t.F_PriceId == keyValue);
                db.Commit();
            }
        }
        public void SubmitForm(PriceEntity priceEntity, List<PriceBaseEntity> peiceBaseEntitys, List<PriceDetailsEntity> priceDetailEntitys, string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                priceEntity.F_EnabledMark = true;
                priceEntity.F_DeleteMark = false;
                if (!string.IsNullOrEmpty(keyValue))
                {
                    priceEntity.Modify(keyValue);
                    db.Update(priceEntity);
                }
                else
                {
                    priceEntity.Create();
                    db.Insert(priceEntity);
                }
                foreach (var item in priceDetailEntitys)
                {
                    item.F_Id = Common.GuId();
                    item.F_PriceId = priceEntity.F_Id;
                }
                foreach (var item in peiceBaseEntitys)
                {
                    item.F_Id = Common.GuId();
                    item.F_PriceId = priceEntity.F_Id;
                }
                db.Delete<PriceDetailsEntity>(t => t.F_PriceId == priceEntity.F_Id);
                db.Insert(priceDetailEntitys);
                db.Delete<PriceBaseEntity>(t => t.F_PriceId == priceEntity.F_Id);
                db.Insert(peiceBaseEntitys);
                db.Commit();
            }
        }
    }
}
