using System;
/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.MeterReading;
using NFine.Code;

namespace NFine.Web.Areas.MeterReading.Controllers
{
    public class UserInspectionController : ControllerBase
    {
        private readonly ReadRecordApp _readRecordApp = new ReadRecordApp();
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="pagination">分页排序</param>
        /// <param name="queryJson">查询条件</param>
        /// <param name="keyValue">任务编号</param>
        /// <returns>查询结果</returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson, string keyValue)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _readRecordApp.GetInspectionList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
    }
}