using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Entity.ArchiveManage
{
    public class ChannelEntity : IEntity<ChannelEntity>, ICreationAudited, IDeleteAudited, IModificationAudited, IOwnAudited
    {
        public string F_Id { get; set; }
        public string F_ChannelName { get; set; }
        public string F_ChannelType { get; set; }
        public string F_ChannelContent { get; set; }
        public string F_ChannelDetail { get; set; }
        public string F_CreatorUserId { get; set; }
        public bool? F_IsEnabled { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public string F_Description { get; set; }

        #region IOwnAudited 成员
        public string F_OwnerId { get; set; }
        #endregion
    }
}