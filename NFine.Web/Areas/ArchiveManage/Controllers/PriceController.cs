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
    public class PriceController : ControllerBase
    {
        private readonly PriceApp _priceApp = new PriceApp();
        private readonly PriceBaseApp _priceBaseApp = new PriceBaseApp();
        private readonly PriceDetailsApp _priceDetailsApp = new PriceDetailsApp();
        private readonly UserApp _userApp = new UserApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _priceApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJson(bool isStatrt)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = _priceApp.GetList(isStatrt, companyId);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(PriceEntity priceEntity, string keyValue)
        {
            var keys = Request.Form.AllKeys.OrderBy(x => x).ToList();
            List<PriceBaseEntity> bases;
            var details = (from item in keys
                           where item.Contains("F_DetailName")
                           let index = int.Parse(item.Replace("F_DetailName", ""))
                           select new PriceDetailsEntity
                           {
                               F_DetailName = Request.Form[item],
                               F_Price = decimal.Parse(!string.IsNullOrEmpty(Request.Form["F_Price" + index]) ? Request.Form["F_Price" + index] : "0"),
                               F_SortNumber = index
                           }).Where(x => !string.IsNullOrEmpty(x.F_DetailName)).ToList();
            if (priceEntity.F_PriceType == "1")
            {
                bases = new List<PriceBaseEntity>()
                {
                  new PriceBaseEntity
                  {
                        F_PriceName =Request.Form["F_PriceName"+0],
                        F_PriceValue = decimal.Parse(!string.IsNullOrEmpty(Request.Form["F_PriceValue" + 0]) ? Request.Form["F_PriceValue" + 0] : "0"),
                        F_SortNumber = 0
                  }
                };
            }
            else
            {
                bases = (from item in keys
                         where item.Contains("F_PriceName") && item != "F_PriceName" && item != "F_PriceName0"
                         let index = int.Parse(item.Replace("F_PriceName", ""))
                         select new PriceBaseEntity
                         {
                             F_PriceName = Request.Form[item],
                             F_PriceValue = decimal.Parse(!string.IsNullOrEmpty(Request.Form["F_PriceValue" + index]) ? Request.Form["F_PriceValue" + index] : "0"),
                             F_SortNumber = index
                         }).Where(x => !string.IsNullOrEmpty(x.F_PriceName)).ToList();
            }
            priceEntity.F_StartTime = priceEntity.F_StartTime == DateTime.MinValue ? DateTime.Now : priceEntity.F_StartTime;
            priceEntity.F_Cycle = priceEntity.F_Cycle ?? 0;
            _priceApp.SubmitForm(priceEntity, bases, details, keyValue);
            return Success("操作成功。");
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _priceApp.GetForm(keyValue);
            data.F_CreatorUserId = _userApp.GetForm(data.F_CreatorUserId) == null
                ? data.F_CreatorUserId
                : _userApp.GetForm(data.F_CreatorUserId).F_NickName;
            data.F_LastModifyUserId = _userApp.GetForm(data.F_LastModifyUserId) == null
                ? data.F_LastModifyUserId
                : _userApp.GetForm(data.F_LastModifyUserId).F_NickName;
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetDetailsListJson(string keyValue)
        {
            var data = _priceDetailsApp.GetList(keyValue);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetBaseListJson(string keyValue)
        {
            var data = _priceBaseApp.GetList(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAuthorize]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            _priceApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}