/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NFine.Application.ArchiveManage;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.Enum;
using NFine.Domain.ViewModel.ArchiveManage;
using NFine.Domain.ViewModel.DataStatistics;

namespace NFine.Web.Areas.ArchiveManage.Controllers
{
    public class MeterController : ControllerBase
    {
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly UserApp _userApp = new UserApp();
        private readonly MeterChargingApp _metertChargingApp = new MeterChargingApp();
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="pagination">分页排序</param>
        /// <param name="queryJson">查询条件</param>
        /// <returns>查询结果</returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _meterApp.GetList(pagination, queryJson, companyId).OrderBy(o=>Convert.ToInt32(o.F_Sort)),
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
            var data = _meterApp.GetForm(keyValue);
            data.F_CreatorUserId = _userApp.GetForm(data.F_CreatorUserId) == null
                ? data.F_CreatorUserId
                : _userApp.GetForm(data.F_CreatorUserId).F_NickName;
            data.F_LastModifyUserId = _userApp.GetForm(data.F_LastModifyUserId) == null
                ? data.F_LastModifyUserId
                : _userApp.GetForm(data.F_LastModifyUserId).F_NickName;
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="keyValue">用户卡号</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJsonByUserCard(string keyValue)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = _meterApp.GetFormByUserCard(keyValue, companyId);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="keyValue">表计编码</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJsonByMeterCode(string keyValue)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = _meterApp.GetFormByMeterCode(keyValue, companyId);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取详细信息
        /// </summary>
        /// <param name="keyValue">表计编号</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJsonByMeterNumber(string keyValue)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = _meterApp.GetFormByMeterNumber(keyValue, companyId);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="meterEntity">基本信息</param>
        /// <param name="meterChargingEntity">计费控制</param>
        /// <param name="keyValue">编号</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(MeterEntity meterEntity, MeterChargingEntity meterChargingEntity, string keyValue)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            meterEntity.F_OwnerId = companyId;
            _meterApp.SubmitForm(meterEntity, meterChargingEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAuthorize]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            _meterApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMeterCountByMeterType(string meterType)
        {
            var meterTypes = new List<string>
            {
                meterType
            };
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            return Content(_meterApp.GetListByMeterType(meterTypes, companyId).Count.ToString());
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
        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="queryJson">查询条件</param>
        [HttpGet]
        public virtual void DownLoad(string queryJson)
        {
            queryJson = HttpUtility.UrlDecode(queryJson, System.Text.Encoding.UTF8);
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            _meterApp.DownLoad(queryJson, companyId);
        }
        /// <summary>
        /// 数据导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult UpLoad()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var file = Request.Files[0];
            if (file != null) _meterApp.UpLoad(file.InputStream, companyId);
            return Success("操作成功。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DisabledMeter(string keyValue)
        {
            var metertChargingEntity = new MeterChargingEntity
            {
                F_ArchiveId = keyValue,
                F_State = false
            };
            _metertChargingApp.UpdateForm(metertChargingEntity);
            return Success("客户禁用成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult EnabledMeter(string keyValue)
        {
            var metertChargingEntity = new MeterChargingEntity
            {
                F_ArchiveId = keyValue,
                F_State = true
            };
            _metertChargingApp.UpdateForm(metertChargingEntity);
            return Success("客户启用成功。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CustomerTree(string customerName = "")
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var meterList = _meterApp.GetAllList(companyId, customerName).GroupBy(x => x.F_CustomerName).OrderBy(x => x.Key);
            var customerTree = meterList.Select(q => new CustomerTreeViewModel
            {
                text = q.Key,
                href = "",
                nodes = q.Select(t => new CustomerTreeViewModel
                {
                    text =
                        "[" + ((Dictionary<string, string>)dataItems["DeviceType"])[t.F_MeterType] + "]" +
                        t.F_MeterCode,
                    href = t.F_MeterCode
                }).ToList(),
            });
            return Content(customerTree.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult QueryComplete(string q, int limit)
        {
            string ret = "";
            if (q != "" && limit > 0)
            {
                var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
                //根据关键字搜索数据库或缓存，这个就比较简单不深入了
                List<MeterViewModel> list = _meterApp.QueryMeter(q, limit, companyId);
                if (list != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (MeterViewModel meter in list)
                    {
                        //以|分割的数据格式，可以是多个，这里是三个。当然也可以吧ID作为特殊的
                        sb.AppendLine(meter.F_CustomerName + "|" + meter.F_UserCard + "|" + meter.F_MeterCode + "|" + meter.F_CustomerAddress + "|" + meter.F_IDNumber);
                    }
                    ret = sb.ToString();
                }
            }
            return Content(ret);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult QueryCompleteUnassigned(string q, int limit)
        {
            var ret = "";
            if (q == "" || limit <= 0) return Content(ret);
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var channelMeter = new ChannelMeterApp().GetList(companyId).Select(x => x.F_ArchiveId).ToList();
            //根据关键字搜索数据库或缓存，这个就比较简单不深入了
            var list = _meterApp.QueryMeter(q, limit, companyId, channelMeter);
            if (list == null) return Content(ret);
            var sb = new StringBuilder();
            foreach (var meter in list)
            {
                //以|分割的数据格式，可以是多个，这里是三个。当然也可以吧ID作为特殊的
                sb.AppendLine(meter.F_CustomerName + "|" + meter.F_UserCard + "|" + meter.F_MeterCode + "|" + meter.F_CustomerAddress + "|" + meter.F_IDNumber);
            }
            ret = sb.ToString();
            return Content(ret);
        }
    }
}