/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.IRepository.DataStatistics;
using NFine.Domain.ViewModel.DataStatistics;
using NFine.Repository.DataStatistics;

namespace NFine.Application.DataStatistics
{
    public class NightReadApp
    {
        private readonly INightReadRepository _viewService = new NightReadRepository();

        public List<NightReadViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<NightReadViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_ReadTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_ReadTime < endtime);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.FindList(expression, pagination);
        }
        public void DownLoad(string queryJson, string companyId)
        {
            var expression = ExtLinq.True<NightReadViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_ReadTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_ReadTime < endtime);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            var recordList = _viewService.IQueryable(expression).OrderByDescending(x => x.F_CreatorTime);
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("用户卡号"),
                new DataColumn("表计编码"),
                new DataColumn("夜间用量"),
                new DataColumn("累计用量"),
                new DataColumn("客户名称"),
                new DataColumn("联系方式"),
                new DataColumn("安装地址"),
                new DataColumn("抄表时间"),
            });
            foreach (var en in recordList)
            {
                var row = dt.NewRow();
                row[0] = en.F_UserCard;
                row[1] = en.F_MeterCode;
                row[2] = en.F_ThisDosage;
                row[3] = en.F_TotalDosage;
                row[4] = en.F_CustomerName;
                row[5] = en.F_MobilePhone;
                row[6] = en.F_CustomerAddress;
                row[7] = en.F_ReadTime;
                dt.Rows.Add(row);
            }
            NPOIExcel.ExportByWeb(dt, "夜间用电", "夜间用电.xls");
        }

    }
}
