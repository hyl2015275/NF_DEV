using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Entity.ArchiveManage
{
    public class ChannelMeterEntity : IEntity<ChannelMeterEntity>, ICreationAudited,IModificationAudited
    {
        public string F_Id { get; set; }
        public string F_ChannelId { get; set; }
        public string F_ArchiveId { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
    }
}