/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NFine.Application;
using NFine.Application.SystemSecurity;
using NFine.Domain.Entity.SystemSecurity;


namespace NFine.Web.Areas.SystemManage.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly UserApp _userApp = new UserApp();
        private readonly UserLogOnApp _userLogOnApp = new UserLogOnApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _userApp.GetList(pagination, keyword, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _userApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue)
        {
            userEntity.F_IsAdministrator = OperatorProvider.Provider.GetCurrent().IsSystem;//平台用户创建用户为平台用户，客户用户创建用户为客户用户
            _userApp.SubmitForm(userEntity, userLogOnEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAuthorize]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            _userApp.DeleteForm(keyValue);
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
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DisabledAccount(string keyValue)
        {
            var userEntity = new UserEntity
            {
                F_Id = keyValue,
                F_EnabledMark = false
            };
            _userApp.UpdateForm(userEntity);
            return Success("账户禁用成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult EnabledAccount(string keyValue)
        {
            var userEntity = new UserEntity
            {
                F_Id = keyValue,
                F_EnabledMark = true
            };
            _userApp.UpdateForm(userEntity);
            return Success("账户启用成功。");
        }
        [HttpGet]
        public ActionResult Info()
        {
            return View();
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult GetInfo()
        {
            var keyValue = OperatorProvider.Provider.GetCurrent().UserId;
            var data = _userApp.GetForm(keyValue);
            if (data != null && data.F_OrganizeId != null)
            {
                var organize = new OrganizeApp().GetForm(data.F_OrganizeId);
                data.F_OrganizeId = organize == null ? "无" : organize.F_FullName;
            }
            else
            {
                if (data != null) data.F_OrganizeId = "无";
            }
            if (data != null && data.F_DepartmentId != null)
            {
                var department = new OrganizeApp().GetForm(data.F_DepartmentId);
                data.F_DepartmentId = department == null ? "无" : department.F_FullName;
            }
            else
            {
                if (data != null) data.F_DepartmentId = "无";
            }
            if (data != null && data.F_RoleId != null)
            {
                var role = new RoleApp().GetForm(data.F_RoleId);
                data.F_RoleId = role == null ? "无" : role.F_FullName;
            }
            else
            {
                if (data != null) data.F_RoleId = "无";
            }
            if (data != null && data.F_DutyId != null)
            {
                var duty = new DutyApp().GetForm(data.F_DutyId);
                data.F_DutyId = duty == null ? "无" : duty.F_FullName;
            }
            else
            {
                if (data != null) data.F_DutyId = "无";
            }
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult GetLogInfo()
        {
            var userId = OperatorProvider.Provider.GetCurrent().UserId;
            var logInfo = _userLogOnApp.GetFormByUserId(userId);
            return Content(new
            {
                logInfo.F_FirstVisitTime,
                logInfo.F_LastVisitTime,
                logInfo.F_ChangePasswordDate,
                logInfo.F_PreviousVisitTime,
                logInfo.F_LogOnCount,
            }.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult ChangePassword(string F_OldPassword, string F_NewPassword, string F_SurePassword)
        {
            var userId = OperatorProvider.Provider.GetCurrent().UserId;
            _userLogOnApp.RevisePassword(F_OldPassword, F_NewPassword, userId);
            return Success("修改密码成功。");
        }
    }
}