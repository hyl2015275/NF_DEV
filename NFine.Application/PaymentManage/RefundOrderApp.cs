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
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.IRepository.PaymentManage;
using NFine.Domain.ViewModel.PaymentManage;
using NFine.Repository.PaymentManage;

namespace NFine.Application.PaymentManage
{
    public class RefundOrderApp
    {
        private readonly IRefundOrderRepository _service = new RefundOrderRepository();
        private readonly IRefundOrderViewRepository _viewService = new RefundOrderViewRepository();
        public List<RefundOrderViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<RefundOrderViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_RefundTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_RefundTime < endtime);
            }
            if (!queryParam["price"].IsEmpty())
            {
                var price = queryParam["price"].ToString();
                //计费方式
                expression = expression.And(t => t.F_PriceModel == price);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            return _viewService.FindList(expression, pagination);
        }
        public decimal GetSum(string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<RefundOrderViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_RefundTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_RefundTime < endtime);
            }
            if (!queryParam["price"].IsEmpty())
            {
                var price = queryParam["price"].ToString();
                //计费方式
                expression = expression.And(t => t.F_PriceModel == price);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            return _viewService.IQueryable(expression).Sum(x => x.F_Money ?? 0);
        }
        public void SubmitForm(RefundOrderEntity payOrderEntity, MeterChargingEntity meterChargingEntity)
        {
            payOrderEntity.Create();
            _service.SubmitForm(payOrderEntity, meterChargingEntity);
        }

        public RefundOrderViewModel GetForm(string keyValue)
        {
            return _viewService.FindEntity(keyValue);
        }
        public void DownLoad(string queryJson, string companyId)
        {
            var expression = ExtLinq.True<RefundOrderViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_RefundTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_RefundTime < endtime);
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
                new DataColumn("订单号"),
                new DataColumn("用户卡号"),
                new DataColumn("表计编码"),
                new DataColumn("退费金额"),
                new DataColumn("本次余额"),
                new DataColumn("客户名称"),
                new DataColumn("经办人"),
                new DataColumn("退费时间"),
            });
            foreach (var en in recordList)
            {
                var row = dt.NewRow();
                row[0] = en.F_OrderNumber;
                row[1] = en.F_UserCard;
                row[2] = en.F_MeterCode;
                row[3] = en.F_Money;
                row[4] = en.F_Balance;
                row[5] = en.F_CustomerName;
                row[6] = en.F_CreatorUserName;
                row[7] = en.F_RefundTime;
                dt.Rows.Add(row);
            }
            NPOIExcel.ExportByWeb(dt, "退费记录", "退费记录.xls");
        }
    }
}