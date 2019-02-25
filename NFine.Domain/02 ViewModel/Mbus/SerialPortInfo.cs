using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NFine.Domain.MBus.Models
{
    public class SerialPortInfo
    {
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public bool IsConnect { get; set; }
    }
}