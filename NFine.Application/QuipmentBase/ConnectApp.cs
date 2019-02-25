/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System.Collections.Generic;
using System.Linq;
using NFine.Domain.Entity.QuipmentBase;
using NFine.Domain.IRepository.QuipmentBase;
using NFine.Repository.QuipmentBase;

namespace NFine.Application.QuipmentBase
{
    public class ConnectApp
    {
        private readonly IConnectRepository _service = new ConnectRepository();
        public void SubmitForm(ConnectEntity readTaskEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                _service.Update(readTaskEntity);
            }
            else
            {
                _service.Insert(readTaskEntity);
            }
        }
        public ConnectEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }

        public ConnectEntity GetFormByIp(string ip)
        {
            return _service.IQueryable(x => x.Ip == ip).FirstOrDefault();
        }

        public List<ConnectEntity> GetLongMeterList()
        {
            return _service.IQueryable(x => x.IsLong).ToList();
        }

        public void DeleteForm(string keyValue)
        {
            _service.Delete(t => t.DeviceId == keyValue);
        }
        public void DeleteFormByMeterCode(string imsi)
        {
            _service.Delete(t => t.MeterCode == imsi);
        }
    }
}