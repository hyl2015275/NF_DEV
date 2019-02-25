using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Repository.ArchiveManage;

namespace NFine.Application.ArchiveManage
{
    public class PriceBaseApp
    {
        private readonly IPriceBaseRepository _service = new PriceBaseRepository();

        public List<PriceBaseEntity> GetList(string keyValue)
        {
            return _service.IQueryable(x => x.F_PriceId == keyValue).OrderBy(x => x.F_SortNumber).ToList();
        }
    }
}
