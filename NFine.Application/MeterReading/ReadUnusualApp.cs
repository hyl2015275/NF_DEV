/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Data;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.IRepository.MeterReading;
using NFine.Domain.ViewModel.MeterReading;
using NFine.Repository.MeterReading;

namespace NFine.Application.MeterReading
{
    public class ReadUnusualApp
    {
        private readonly IReadUnusualRepository _service = new ReadUnusualRepository();
        private readonly IReadUnusualViewRepository _viewService = new ReadUnusualViewRepository();
        public List<ReadUnusualViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<ReadUnusualViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["factor"].IsEmpty())
            {
                var factor = queryParam["factor"].ToString();
                //生产厂商
                expression = expression.And(t => t.F_Factor == factor);
            }
            if (!queryParam["error"].IsEmpty())
            {
                var error = queryParam["error"].ToString();
                //异常类型
                expression = expression.And(t => t.F_ErrorType == error);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_CreatorTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_CreatorTime < endtime);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                var boolId = "\"BookID\":\"" + keyword + "\"";
                //用户卡号、表计编码、客户姓名、安装地址
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword) || t.F_Description.Contains(boolId));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.FindList(expression, pagination);
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        public void DownLoad(string queryJson, string companyId)
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var expression = ExtLinq.True<ReadUnusualViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["factor"].IsEmpty())
            {
                var factor = queryParam["factor"].ToString();
                //生产厂商
                expression = expression.And(t => t.F_Factor == factor);
            }
            if (!queryParam["error"].IsEmpty())
            {
                var error = queryParam["error"].ToString();
                //抄表类型
                expression = expression.And(t => t.F_ErrorType == error);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_CreatorTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_CreatorTime < endtime);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名、安装地址
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            var unusualList = _viewService.IQueryable(expression).OrderByDescending(x => x.F_CreatorTime);
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("表类型"),
                new DataColumn("异常类型"),
                new DataColumn("生产厂商"),
                new DataColumn("表计编码"),
                new DataColumn("当次用量"),
                new DataColumn("客户名称"),
                new DataColumn("联系方式"),
                new DataColumn("安装地址"),
                new DataColumn("异常时间")
            });
            foreach (var en in unusualList)
            {
                var row = dt.NewRow();
                if (dataItems != null)
                {
                    row[0] = ((Dictionary<string, string>)dataItems["DeviceType"])[en.F_MeterType];
                    row[1] = ((Dictionary<string, string>)dataItems["ErrorType"])[en.F_ErrorType];
                }
                else
                {
                    row[0] = "";
                    row[1] = "";
                }
                row[2] = en.F_Factor;
                row[3] = en.F_MeterCode;
                row[4] = en.F_Value;
                row[5] = en.F_CustomerName;
                row[6] = en.F_MobilePhone;
                row[7] = en.F_CustomerAddress;
                row[8] = en.F_CreatorTime;
                dt.Rows.Add(row);
            }
            NPOIExcel.ExportByWeb(dt, "异常记录", "异常记录.xls");
        }
        /// <summary>
        /// 获取异常数目
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public int GetCountTotal(string companyId)
        {
            if (companyId == "d3191d18-43f9-4547-b2dd-687113e30936")
            {
                var endTime = DateTime.Now.Date.AddDays(1);
                var startTime = DateTime.Now.AddDays(-7);
                var unusualList = _viewService.IQueryable(w => w.F_CreatorTime >= startTime && w.F_CreatorTime < endTime && w.F_OwnerId == companyId).GroupBy(x => x.F_MeterCode).Where(g => g.Count() > 7).Select(g => g.Key);
                return unusualList.Count();
            }
            else
            {
                var endTime = DateTime.Now.Date.AddDays(1);
                var startTime = DateTime.Now.Date;
                return _viewService.IQueryable(x => x.F_OwnerId == companyId && x.F_CreatorTime >= startTime && x.F_CreatorTime < endTime).Select(p => p.F_MeterCode).Distinct().Count();
            }
        }

        public void SubmitForm(ReadUnusualEntity readUnusualEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                _service.Update(readUnusualEntity);
            }
            else
            {
                _service.Insert(readUnusualEntity);
            }
        }

        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
            //    db.Update(readTaskEntity);
        //        db.ExecuteSqlCommand("update dbo.Mer_ReadRecord set F_ArchiveId='" + changeMeterEntity.F_ArchiveId + "' where F_ArchiveId='" + changeMeterEntity.F_OldArchiveId + "'");
                db.ExecuteSqlCommand("EXEC dbo.Pro_Mer_ReadUnusual @F_Id = N'"+keyValue+"'");
                db.Commit();
            }


        }

    }
}