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
    public class PriceDetailsApp
    {
        private readonly IPriceDetailsRepository _service = new PriceDetailsRepository();

        public List<PriceDetailsEntity> GetList(string keyValue)
        {
            return _service.IQueryable(x => x.F_PriceId == keyValue).ToList();
        }
    }
}
