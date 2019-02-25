/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Application.MeterReading;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Domain.ViewModel.ArchiveManage;
using NFine.Repository.ArchiveManage;

namespace NFine.Application.ArchiveManage
{
    public class SubsistenceSecurityApp
    {
        private readonly ISubsistenceSecurityRepository _service = new SubsistenceSecurityRepository();
        private readonly ISubsistenceSecurityViewRepository _viewService = new SubsistenceSecurityViewRepository();
        public void SubmitForm(SubsistenceSecurityEntity subsistenceSecurityEntity, string keyValue)
        {
            CheckEntity(subsistenceSecurityEntity, keyValue);
            if (!string.IsNullOrEmpty(keyValue))
            {
                subsistenceSecurityEntity.Modify(keyValue);
                _service.Update(subsistenceSecurityEntity);
            }
            else
            {
                subsistenceSecurityEntity.Create();
                _service.Insert(subsistenceSecurityEntity);
            }
        }
        public List<SubsistenceSecurityViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<SubsistenceSecurityViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名、身份证号
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_IDNumber.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            expression = expression.And(t => t.F_DeleteMark != true && t.F_MeterDeleteMark != true);
            return _viewService.FindList(expression, pagination);
        }
        public SubsistenceSecurityViewModel GetForm(string keyValue)
        {
            return _viewService.FindEntity(keyValue);
        }

        public SubsistenceSecurityEntity GetFormByArchiveId(string archiveId)
        {
            var now = DateTime.Now;
            return _service.FindEntity(x => x.F_ArchiveId == archiveId && x.F_DeleteMark != true && x.F_EndTime > now);
        }

        public void DeleteForm(string keyValue)
        {
            var subsistenceSecurityEntity = _service.FindEntity(keyValue);
            subsistenceSecurityEntity.Remove();
            _service.Update(subsistenceSecurityEntity);
        }
        public void CheckEntity(SubsistenceSecurityEntity subsistenceSecurityEntity, string keyValue)
        {
            if (string.IsNullOrEmpty(subsistenceSecurityEntity.F_PriceId))
                throw new Exception("执行价格不允许为空");
            if (string.IsNullOrEmpty(subsistenceSecurityEntity.F_ArchiveId))
                throw new Exception("用户卡号不允许为空");
            if (_service.IQueryable(x => x.F_ArchiveId == subsistenceSecurityEntity.F_ArchiveId && x.F_Id != keyValue && x.F_DeleteMark != true).Any())
                throw new Exception("低保用户已存在");
        }
    }
}