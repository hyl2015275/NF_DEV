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
    public class ChannelController : ControllerBase
    {
        private readonly ChannelApp _channelApp = new ChannelApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var data = new
            {
                rows = _channelApp.GetList(pagination, queryJson),
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
            var data = _channelApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ChannelEntity roleEntity, string keyValue)
        {
            _channelApp.SubmitForm(roleEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            _channelApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeJson()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = _channelApp.GetList(companyId);
            var treeList = new List<TreeViewModel>();
            const string topId = "00000000_0000_0000_0000_000000000000";
            treeList.Add(new TreeViewModel
            {
                id = topId,
                text = "所有设备",
                value = "all",
                isexpand = true,
                complete = true,
                hasChildren = data.Count > 0,
                parentId = "0",
                checkstate = 1
            });
            treeList.AddRange(data.Select(item => new TreeViewModel
            {
                id = item.F_Id,
                text = item.F_ChannelName,
                value = item.F_Id,
                isexpand = true,
                complete = true,
                hasChildren = false,
                parentId = topId
            }));
            treeList.Add(new TreeViewModel
            {
                id = "ffffffff_ffff_ffff_ffff_ffffffffffff",
                text = "未分配",
                value = "null",
                isexpand = true,
                complete = true,
                hasChildren = false,
                parentId = "0"
            });
            return Content(treeList.TreeViewJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJson()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = _channelApp.GetList(companyId);
            return Content(data.ToJson());
        }
    }
}