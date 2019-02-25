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
using NFine.Application.MeterReading;
using NFine.Code;
using NFine.Domain.Entity.MeterReading;

namespace NFine.Web.Areas.MeterReading.Controllers
{
    public class TaskListController : ControllerBase
    {
        private readonly TaskListApp _taskListApp = new TaskListApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _taskListApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCustomTask()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _taskListApp.GetList(companyId),
            };
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TaskListEntity taskListEntity)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            _taskListApp.SubmitForm(taskListEntity, companyId);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitCustomTaskForm(TaskListEntity taskListEntity, string keyValue)
        {
            _taskListApp.SubmitCustomTaskForm(taskListEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult CustomTask()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult CustomTaskForm()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _taskListApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            _taskListApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCustomTaskForm(string keyValue)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            _taskListApp.DeleteCustomTaskForm(keyValue, companyId);
            return Success("删除成功。");
        }
    }
}