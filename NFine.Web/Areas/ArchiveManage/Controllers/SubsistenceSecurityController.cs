/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.ArchiveManage;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;

namespace NFine.Web.Areas.ArchiveManage.Controllers
{
    public class SubsistenceSecurityController : ControllerBase
    {
        private readonly SubsistenceSecurityApp _subsistenceSecurityApp = new SubsistenceSecurityApp();
        private readonly UserApp _userApp = new UserApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _subsistenceSecurityApp.GetList(pagination, queryJson, companyId),
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
            var data = _subsistenceSecurityApp.GetForm(keyValue);
            data.F_CreatorUserId = _userApp.GetForm(data.F_CreatorUserId) == null
                ? data.F_CreatorUserId
                : _userApp.GetForm(data.F_CreatorUserId).F_NickName;
            data.F_LastModifyUserId = _userApp.GetForm(data.F_LastModifyUserId) == null
                ? data.F_LastModifyUserId
                : _userApp.GetForm(data.F_LastModifyUserId).F_NickName;
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(SubsistenceSecurityEntity subsistenceSecurityEntity, string keyValue)
        {
            subsistenceSecurityEntity.F_EndTime = subsistenceSecurityEntity.F_StartTime == null
               ? DateTime.MinValue
               : ((DateTime)subsistenceSecurityEntity.F_StartTime).AddMonths(subsistenceSecurityEntity.F_EnjoyTime ?? 0);
            _subsistenceSecurityApp.SubmitForm(subsistenceSecurityEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            _subsistenceSecurityApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}