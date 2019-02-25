using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NFine.Domain.MBus.Models
{
    public class PortSetting
    {
        public string ServerAddress { get; set; }
        public string Port { get; set; }
        public string ClientAddress { get; set; }
        public string IsDes { get; set; }
        public string UserKey { get; set; }
    }
}