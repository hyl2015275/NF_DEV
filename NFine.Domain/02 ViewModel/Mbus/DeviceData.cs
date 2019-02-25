using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Mbus
{
    public class DeviceData
    {
        public string notifyType { get; set; }
        public string requestId { get; set; }
        public string timestamp { get; set; }
        public string deviceId { get; set; }
        public string gatewayId { get; set; }
        public Service service { get; set; }
        public class Service
        {
            public string serviceType { get; set; }
            public string serviceId { get; set; }
            public string eventTime { get; set; }
            public Data data { get; set; }
        }
        public class Data
        {
            public string rawData { get; set; }
        }
    }
}
