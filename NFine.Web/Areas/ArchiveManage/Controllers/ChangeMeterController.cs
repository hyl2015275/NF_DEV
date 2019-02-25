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
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;

namespace NFine.Web.Areas.ArchiveManage.Controllers
{
    public class ChangeMeterController : ControllerBase
    {
        private readonly ChangeMeterApp _changeMeterApp = new ChangeMeterApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _changeMeterApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="keyValue">编号</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _changeMeterApp.GetFormByMeterKey(keyValue);
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ChangeMeterEntity changeMeterEntity, string F_MeterCode)
        {
            _changeMeterApp.SubmitForm(changeMeterEntity, F_MeterCode);
            return Success("操作成功。");
        }
    }
}
