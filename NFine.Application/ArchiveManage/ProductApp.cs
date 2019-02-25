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
    public class ProductApp
    {
        private readonly IProductRepository _service = new ProductRepository();

        public List<ProductEntity> GetList(Pagination pagination, string queryJson)
        {
            var expression = ExtLinq.True<ProductEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //通道名称
                expression = expression.And(t => t.F_ProductName.Contains(keyword));
            }
            expression = expression.And(t => t.F_DeleteMark != true);
            return _service.FindList(expression, pagination).ToList();
        }
        public ProductEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            var channel = GetForm(keyValue);
            _service.Delete(t => t.F_Id == keyValue);
        }
        public void SubmitForm(ProductEntity productEntity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                productEntity.Create();
                _service.Insert(productEntity);
            }
            else
            {
                productEntity.Modify(keyValue);
                _service.Update(productEntity);
            }
        }
    }
}