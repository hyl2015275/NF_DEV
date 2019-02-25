/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System;
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.IRepository.SystemManage;
using NFine.Repository.SystemManage;
using System.Collections.Generic;
using System.Linq;
using NFine.Domain.IRepository.DataStatistics;
using NFine.Domain.ViewModel.DataStatistics;
using NFine.Repository.DataStatistics;

namespace NFine.Application.DataStatistics
{
    public class MoneyAlarmApp
    {
        private readonly IMoneyAlarmRepository _service = new MoneyAlarmRepository();
        private readonly IMoneyAlarmViewRepository _viewService = new MoneyAlarmViewRepository();
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询条件</param>
        /// <param name="companyId">公司编号</param>
        /// <returns></returns>
        public List<MoneyAlarmViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<MoneyAlarmViewModel>();
            var queryParam = queryJson.ToJObject();
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
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.FindList(expression, pagination);
        }
        /// <summary>
        /// 获取表单
        /// </summary>
        /// <param name="keyValue">表单编号</param>
        /// <returns></returns>
        public MoneyAlarmEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }
        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="keyValue">表单编号</param>
        public void DeleteForm(string keyValue)
        {
            _service.Delete(t => t.F_Id == keyValue);
        }
        /// <summary>
        /// 表单提交
        /// </summary>
        /// <param name="moneyAlarmEntity">保单对象</param>
        /// <param name="keyValue">表单编号</param>
        public void SubmitForm(MoneyAlarmEntity moneyAlarmEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                moneyAlarmEntity.Modify(keyValue);
                _service.Update(moneyAlarmEntity);
            }
            else
            {
                moneyAlarmEntity.Create();
                _service.Insert(moneyAlarmEntity);
            }
        }
        /// <summary>
        /// 是否继续发送短信报警
        /// </summary>
        /// <param name="archiveId">设备编号</param>
        /// <returns></returns>
        public bool IsCanSend(string archiveId)
        {
            var list = _service.IQueryable(x => x.F_ArchiveId == archiveId && x.F_SureState == false);
            return list == null || list.Count() < 2;
        }
        /// <summary>
        /// 充值确认
        /// </summary>
        /// <param name="archiveId">设备编号</param>
        public void SureAlarm(string archiveId)
        {
            var list = _service.IQueryable(x => x.F_ArchiveId == archiveId && x.F_SureState == false).ToList();
            if (!list.Any()) return;
            foreach (var item in list)
            {
                item.F_SureState = true;
                _service.Update(item);
            }
        }
    }
}