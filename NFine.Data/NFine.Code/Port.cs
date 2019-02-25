using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Code
{
    public class Port
    {
        #region 检测端口号
        public bool CheckPort(string tempPort)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo("netstat", "-an")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true
                }
            };
            p.Start();
            string result = p.StandardOutput.ReadToEnd().ToLower();//最后都转换成小写字母
            var addressList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
            var ipList = new List<string> {"127.0.0.1", "0.0.0.0"};
            ipList.AddRange(addressList.Select(t => t.ToString()));
            var use = ipList.Any(t => result.IndexOf("tcp    " + t + ":" + tempPort, StringComparison.Ordinal) >= 0 || result.IndexOf("udp    " + t + ":" + tempPort) >= 0);
            p.Close();
            return use;
        }
        #endregion
    }
}
