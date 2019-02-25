/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Code;
using System;
using System.Configuration;
using System.IO;

namespace NFine.Domain
{
    public class IEntity<TEntity>
    {
        public void Create()
        {
            var entity = this as ICreationAudited;
            var entityOwn = this as IOwnAudited;
            entity.F_Id = Common.GuId();
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo != null)
            {
                if (entityOwn != null) entityOwn.F_OwnerId = LoginInfo.CompanyId;
                entity.F_CreatorUserId = LoginInfo.UserId;
            }
            entity.F_CreatorTime = DateTime.Now;
        }
        public void Modify(string keyValue)
        {
            var entity = this as IModificationAudited;
            entity.F_Id = keyValue;
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo != null)
            {
                entity.F_LastModifyUserId = LoginInfo.UserId;
            }
            entity.F_LastModifyTime = DateTime.Now;
        }
        public void Remove()
        {
            var entity = this as IDeleteAudited;
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo != null)
            {
                entity.F_DeleteUserId = LoginInfo.UserId;
            }
            entity.F_DeleteTime = DateTime.Now;
            entity.F_DeleteMark = true;
        }
       
    }
}
