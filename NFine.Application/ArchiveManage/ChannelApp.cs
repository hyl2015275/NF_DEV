using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Repository.ArchiveManage;

namespace NFine.Application.ArchiveManage
{
    public class ChannelApp
    {
        private readonly IChannelRepository _service = new ChannelRepository();
        public List<ChannelEntity> GetList(Pagination pagination, string queryJson)
        {
            var expression = ExtLinq.True<ChannelEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //通道名称
                expression = expression.And(t => t.F_ChannelName.Contains(keyword));
            }
            expression = expression.And(t => t.F_DeleteMark != true);
            return _service.FindList(expression, pagination).ToList();
        }
        public List<ChannelEntity> GetList(string companyId = "")
        {
            var expression = ExtLinq.True<ChannelEntity>();
            expression = expression.And(t => t.F_DeleteMark != true);
            if (!string.IsNullOrEmpty(companyId))
                expression = expression.And(t => t.F_OwnerId == companyId);
            return _service.IQueryable(expression).ToList();
        }
        public ChannelEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }
        public ChannelEntity GetFormByContent(string content)
        {
            return _service.FindEntity(x => x.F_ChannelContent == content);
        }
        public ChannelEntity GetFormByArchiveId(string archiveId)
        {
            return _service.GetFormByArchiveId(archiveId);
        }
        public ChannelEntity GetFormByName(string name)
        {
            return _service.FindEntity(x => x.F_ChannelName == name);
        }
        public void DeleteForm(string keyValue)
        {
            var channelEntity = GetForm(keyValue);
            channelEntity.Remove();
            _service.Update(channelEntity);
        }
        public void SubmitForm(ChannelEntity channeEntity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                channeEntity.Create();
                _service.Insert(channeEntity);
            }
            else
            {
                channeEntity.Modify(keyValue);
                _service.Update(channeEntity);
            }
        }
    }
}