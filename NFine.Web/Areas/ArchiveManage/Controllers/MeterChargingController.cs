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

namespace NFine.Web.Areas.ArchiveManage.Controllers
{
    public class MeterChargingController : ControllerBase
    {
        private readonly MeterChargingApp _MeterChargingApp = new MeterChargingApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _MeterChargingApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
    }
}