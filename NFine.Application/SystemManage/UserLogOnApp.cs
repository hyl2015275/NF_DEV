/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System;
using System.Linq;
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.IRepository.SystemManage;
using NFine.Repository.SystemManage;

namespace NFine.Application.SystemManage
{
    public class UserLogOnApp
    {
        private IUserLogOnRepository service = new UserLogOnRepository();

        public UserLogOnEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public UserLogOnEntity GetFormByUserId(string userId)
        {
            var userLogOnEntity = service.IQueryable(x => x.F_UserId == userId).FirstOrDefault();
            return userLogOnEntity;
        }

        public void UpdateForm(UserLogOnEntity userLogOnEntity)
        {
            service.Update(userLogOnEntity);
        }
        public void RevisePassword(string userPassword, string keyValue)
        {
            var userLogOnEntity = new UserLogOnEntity
            {
                F_Id = keyValue,
                F_UserSecretkey = Md5.md5(Common.CreateNo(), 16).ToLower()
            };
            userLogOnEntity.F_UserPassword = Md5.md5(DESEncrypt.Encrypt(Md5.md5(userPassword, 32).ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower();
            service.Update(userLogOnEntity);
        }

        public void RevisePassword(string oldPassword, string newPassword, string userId)
        {
            var userLogOnEntity = service.IQueryable(x => x.F_UserId == userId).FirstOrDefault();
            if (userLogOnEntity == null)
                throw new Exception("用户不存在");
            if (userLogOnEntity.F_UserPassword != Md5.md5(DESEncrypt.Encrypt(Md5.md5(oldPassword, 32).ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower())
                throw new Exception("旧密码不正确");
            userLogOnEntity.F_ChangePasswordDate = DateTime.Now;
            userLogOnEntity.F_UserSecretkey = Md5.md5(Common.CreateNo(), 16).ToLower();
            userLogOnEntity.F_UserPassword = Md5.md5(DESEncrypt.Encrypt(Md5.md5(newPassword, 32).ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower();
            service.Update(userLogOnEntity);
        }
    }
}
