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
using NFine.Application.DataStatistics;
using NFine.Code;

namespace NFine.Web.Areas.DataStatistics.Controllers
{
    public class ReportStatisticsController : ControllerBase
    {
        private readonly ReportStatisticsApp _ReportStatisticsApp = new ReportStatisticsApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _ReportStatisticsApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="queryJson">查询条件</param>
        [HttpGet]
        public virtual void DownLoad(string queryJson)
        {
            queryJson = HttpUtility.UrlDecode(queryJson, System.Text.Encoding.UTF8);
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            _ReportStatisticsApp.DownLoad(queryJson, companyId);
        }
        [HttpGet]
        public ActionResult SettingForm()
        {
            return View();
        }
    }
}