using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.ViewModel;

namespace NFine.Web.Areas.SystemManage.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly OrganizeApp _organizeApp = new OrganizeApp();
        private readonly UserApp _userApp = new UserApp();
        private readonly RoleApp _roleApp = new RoleApp();
        private readonly UserLogOnApp _userLogOnApp = new UserLogOnApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            var company = _organizeApp.GetList(pagination, keyword);
            var customers = company.Where(x => x.F_Id != OperatorProvider.Provider.GetCurrent().CompanyId && x.F_DeleteMark != true).Select(item => new CustomerViewModel
            {
                CompanyInfo = item,
                DefaultUser = _userApp.GetDefaultUser(item.F_Id),
                DefaultRole = _roleApp.GetDefaultCompanyRole(item.F_Id)
            }).ToList();
            return Content(customers.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(OrganizeEntity organizeEntity, string F_Account, string permissionIds, string keyValue)
        {
            var roleKey = "";
            var userKey = "";
            organizeEntity.F_CategoryId = "Company";
            organizeEntity.F_ParentId = "0";
            organizeEntity.F_Layers = 1;
            organizeEntity.F_DeleteMark = false;
            organizeEntity.F_AllowDelete = false;
            _organizeApp.SubmitForm(organizeEntity, keyValue);
            var roleEntity = new RoleEntity
            {
                F_Category = 0,
                F_FullName = "系统管理员",
                F_OrganizeId = organizeEntity.F_Id,
                F_AllowDelete = false,
            };
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleKey = _roleApp.GetDefaultCompanyRole(keyValue).F_Id;
            }
            _roleApp.SubmitForm(roleEntity, permissionIds.Split(','), roleKey);
            if (!string.IsNullOrEmpty(keyValue))
            {
                userKey = _userApp.GetDefaultUser(keyValue).F_Id;
            }
            var userEntity = new UserEntity
            {
                F_DeleteMark = false,
                F_EnabledMark = true,
                F_Description = "系统内置账户",
                F_IsAdministrator = false,
                F_MobilePhone = organizeEntity.F_MobilePhone,
                F_NickName = organizeEntity.F_ManagerId,
                F_RealName = organizeEntity.F_ManagerId,
                F_Email = organizeEntity.F_Email,
                F_ManagerId = organizeEntity.F_ManagerId,
                F_Account = F_Account,
                F_OrganizeId = organizeEntity.F_Id,
                F_AllowDelete = false,
                F_RoleId = roleEntity.F_Id,
            };
            _userApp.SubmitForm(userEntity, new UserLogOnEntity { F_UserPassword = F_Account }, userKey);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult EnabledAccount(string keyValue)
        {
            var userEntity = new OrganizeEntity
            {
                F_Id = keyValue,
                F_EnabledMark = true
            };
            _organizeApp.UpdateForm(userEntity);
            return Success("客户启用成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DisabledAccount(string keyValue)
        {
            var userEntity = new OrganizeEntity
            {
                F_Id = keyValue,
                F_EnabledMark = false
            };
            _organizeApp.UpdateForm(userEntity);
            return Success("客户禁用成功。");
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _organizeApp.GetForm(keyValue);
            return Content(new CustomerViewModel
            {
                CompanyInfo = data,
                DefaultUser = _userApp.GetDefaultUser(data.F_Id),
                DefaultRole = _roleApp.GetDefaultCompanyRole(data.F_Id)
            }.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            _organizeApp.DeleteForm(keyValue, true);
            return Success("删除成功。");
        }
        [HttpGet]
        public ActionResult RevisePassword()
        {
            return View();
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRevisePassword(string userPassword, string keyValue)
        {
            _userLogOnApp.RevisePassword(userPassword, keyValue);
            return Success("重置密码成功。");
        }
    }
}