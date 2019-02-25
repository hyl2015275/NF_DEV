/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.IRepository.SystemManage;
using NFine.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NFine.Application.SystemManage
{
    public class UserApp
    {
        private readonly IUserRepository _service = new UserRepository();
        private readonly UserLogOnApp _userLogOnApp = new UserLogOnApp();

        public List<UserEntity> GetList(Pagination pagination, string keyword, string companyId = "")
        {
            var expression = ExtLinq.True<UserEntity>();
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OrganizeId == companyId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_Account.Contains(keyword) || t.F_RealName.Contains(keyword) || t.F_MobilePhone.Contains(keyword));
            }
            expression = expression.And(t => t.F_AllowDelete != false);
            return _service.FindList(expression, pagination);
        }
        public UserEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }
        public UserEntity GetFormByAccount(string account)
        {
            return _service.FindEntity(x => x.F_Account == account && x.F_DeleteMark != true);
        }

        public UserEntity GetDefaultUser(string companyId)
        {
            return _service.FindEntity(x => x.F_OrganizeId == companyId && x.F_AllowDelete == false);
        }
        public void DeleteForm(string keyValue)
        {
            var user = GetForm(keyValue);
            if (user.F_AllowDelete == false)
                throw new Exception("此用户不允许删除。");
            _service.DeleteForm(keyValue);
        }
        public void SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue)
        {
            CheckEntity(userEntity, keyValue);
            if (!string.IsNullOrEmpty(keyValue))
            {
                userEntity.Modify(keyValue);
            }
            else
            {
                userEntity.Create();
            }
            _service.SubmitForm(userEntity, userLogOnEntity, keyValue);
        }
        public void UpdateForm(UserEntity userEntity)
        {
            _service.Update(userEntity);
        }
        public UserEntity CheckLogin(string username, string password)
        {
            UserEntity userEntity = _service.FindEntity(t => t.F_Account == username);
            if (userEntity != null)
            {
                if (userEntity.F_EnabledMark == true)
                {
                    UserLogOnEntity userLogOnEntity = _userLogOnApp.GetForm(userEntity.F_Id);
                    string dbPassword = Md5.md5(DESEncrypt.Encrypt(password.ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower();
                    if (dbPassword == userLogOnEntity.F_UserPassword)
                    {
                        DateTime lastVisitTime = DateTime.Now;
                        var logOnCount = (userLogOnEntity.F_LogOnCount).ToInt() + 1;
                        if (userLogOnEntity.F_LastVisitTime != null)
                        {
                            userLogOnEntity.F_PreviousVisitTime = userLogOnEntity.F_LastVisitTime.ToDate();
                        }
                        userLogOnEntity.F_FirstVisitTime = userLogOnEntity.F_FirstVisitTime ?? DateTime.Now;
                        userLogOnEntity.F_LastVisitTime = lastVisitTime;
                        userLogOnEntity.F_LogOnCount = logOnCount;
                        _userLogOnApp.UpdateForm(userLogOnEntity);
                        return userEntity;
                    }
                    else
                    {
                        throw new Exception("密码不正确，请重新输入");
                    }
                }
                else
                {
                    throw new Exception("账户被系统锁定,请联系管理员");
                }
            }
            else
            {
                throw new Exception("账户不存在，请重新输入");
            }
        }

        public void CheckEntity(UserEntity userEntity, string keyValue)
        {
            if (string.IsNullOrEmpty(userEntity.F_Account))
                throw new Exception("账户不允许为空");
            if (_service.IQueryable(t => t.F_Account.Equals(userEntity.F_Account) && t.F_DeleteMark != true && t.F_Id != keyValue).Any())
                throw new Exception("账户" + userEntity.F_Account + "已存在。");
        }
    }
}