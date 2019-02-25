/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System;
using NFine.Application.MaterialManage;
using NFine.Code;
using NFine.Domain.Entity.MaterialManage;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NFine.Web.Areas.MaterialManage.Controllers
{
    public class IOTController : ControllerBase
    {
        private readonly IOTApp _iotApp = new IOTApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var data = new
            {
                rows = _iotApp.GetList(pagination, queryJson),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult Import()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult Export()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult UpLoad()
        {
            try
            {
                var file = Request.Files[0];
                if (file != null) _iotApp.UpLoad(file.InputStream);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        public virtual void DownLoad()
        {
            _iotApp.DownLoad();
        }
    }
}