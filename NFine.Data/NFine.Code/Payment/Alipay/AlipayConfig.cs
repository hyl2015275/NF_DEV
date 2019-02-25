using System.Web;
using System.Xml;

namespace NFine.Code.Payment.Alipay
{
    sealed class AlipayConfig
    {
        #region 字段
        static readonly string partner = string.Empty;
        static string key = string.Empty;
        static string input_charset = string.Empty;
        static string sign_type = string.Empty;
        static string notify_url = string.Empty;
        static string return_url = string.Empty;
        static string seller_email = string.Empty;
        static string show_url = string.Empty;
        static string gateway_new = string.Empty;
        static string https_veryfy_url = string.Empty;
        #endregion

        static AlipayConfig()
        {
            var xml = new XmlDocument();
            xml.Load(HttpContext.Current.Server.MapPath("/App_Data/xml/payment.xml"));
            var selectSingleNode = xml.SelectSingleNode("paymentMethod");
            if (selectSingleNode == null) return;
            var root = selectSingleNode.ChildNodes;
            foreach (XmlNode xn in root)
            {
                var xe = (XmlElement)xn;
                if (xe.GetAttribute("id") != "alipay") continue;
                partner = xe.GetAttribute("partner");
                key = xe.GetAttribute("key");
                input_charset = xe.GetAttribute("input_charset");
                sign_type = xe.GetAttribute("sign_type");
                notify_url = xe.GetAttribute("notify_url");
                return_url = xe.GetAttribute("return_url");
                seller_email = xe.GetAttribute("seller_email");
                show_url = xe.GetAttribute("show_url");
                gateway_new = xe.GetAttribute("gateway_new");
                https_veryfy_url = xe.GetAttribute("https_veryfy_url");
                break;
            }
        }
        internal static string GatewayNew
        {
            get
            {
                return gateway_new;
            }
        }
        internal static string Partner
        {
            get
            {
                return partner;
            }
        }
        internal static string InputCharset
        {
            get
            {
                return input_charset;
            }
        }
        internal static string SignType
        {
            get
            {
                return sign_type;
            }
        }
        internal static string Key
        {
            get
            {
                return key;
            }
        }
        internal static string HttpsVeryfyUrl
        {
            get
            {
                return https_veryfy_url;
            }
        }
        internal static string SellerEmail
        {
            get
            {
                return seller_email;
            }
        }
        internal static string NotifyUrl
        {
            get
            {
                return notify_url;
            }
        }
        internal static string ReturnUrl
        {
            get
            {
                return return_url;
            }
        }
        internal static string ShowUrl
        {
            get
            {
                return show_url;
            }
        }
    }
}