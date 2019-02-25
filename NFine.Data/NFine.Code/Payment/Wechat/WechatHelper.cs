using Senparc.Weixin.MP.TenPayLibV3;
using System.Web;
using System.Xml;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Senparc.Weixin.MP.Containers;

namespace NFine.Code.Payment.Wechat
{
    public class WechatHelper
    {
        public static string Pay(string orderNumber, int totalFee, string openid)
        {
            //读取配置文件
            var merchantId = string.Empty;
            var appId = string.Empty;
            var key = string.Empty;
            var appSecret = string.Empty;
            //接收微信支付异步通知回调地址
            var notifyUrl = string.Empty;
            var payUrl = string.Empty;

            var xd = new XmlDocument();
            xd.Load(HttpContext.Current.Server.MapPath("/app_data/xml/payment.xml"));
            var selectSingleNode = xd.SelectSingleNode("paymentMethod");
            if (selectSingleNode != null)
            {
                var xnl = selectSingleNode.ChildNodes;
                foreach (XmlNode xn in xnl)
                {
                    var xe = (XmlElement)xn;
                    if (xe.GetAttribute("id") != "wechat") continue;
                    merchantId = xe.GetAttribute("merchant_id");
                    appId = xe.GetAttribute("app_id");
                    appSecret = xe.GetAttribute("app_secret");
                    key = xe.GetAttribute("key");
                    notifyUrl = xe.GetAttribute("notify_url");
                    payUrl = xe.GetAttribute("pay_url") + orderNumber.ToString();
                    break;
                }
            }
            var timestamp = Senparc.Weixin.MP.TenPayLib.TenPayUtil.GetTimestamp();
            var nonceStr = Senparc.Weixin.MP.TenPayLib.TenPayUtil.GetNoncestr();
            JsApiTicketContainer.Register(appId, appSecret);
            var jsapiTicket = JsApiTicketContainer.GetJsApiTicket(appId);
            var signConfig = new Senparc.Weixin.MP.TenPayLib.RequestHandler(null);
            signConfig.SetParameter("jsapi_ticket", jsapiTicket);
            signConfig.SetParameter("noncestr", nonceStr);
            signConfig.SetParameter("timestamp", timestamp);
            signConfig.SetParameter("url", payUrl);
            var signature = signConfig.CreateSHA1Sign();
            //支付
            //商品或支付单简要描述
            var body = "Billion智能仪表管理系统";
            //商户系统内部的订单号，32个字符内，可包含字母，其他说明见商户订单号
            //APP和网页支付提交用户端IP，Native支付填调用微信支付API的机器IP
            var spbillCreateIp = HttpContext.Current.Request.ServerVariables.Get("Remote_Addr");

            //JSAPI,NATIVE,APP,WAP
            var trade_type = "JSAPI";
            //创建支付应答对象
            var panSigig = new RequestHandler(null);
            //初始化
            panSigig.Init();
            //设置package订单参数
            panSigig.SetParameter("appid", appId);
            panSigig.SetParameter("mch_id", merchantId);
            panSigig.SetParameter("nonce_str", nonceStr);
            panSigig.SetParameter("body", body);
            panSigig.SetParameter("out_trade_no", orderNumber);
            panSigig.SetParameter("total_fee", totalFee.ToString());
            panSigig.SetParameter("spbill_create_ip", spbillCreateIp);
            panSigig.SetParameter("notify_url", notifyUrl);
            panSigig.SetParameter("trade_type", trade_type);
            panSigig.SetParameter("openid", openid);

            var sign = panSigig.CreateMd5Sign("key", key);

            panSigig.SetParameter("sign", sign);

            var data = panSigig.ParseXML();

            var result = TenPayV3.Unifiedorder(data);

            var res = System.Xml.Linq.XDocument.Parse(result);
            var xElement = res.Element("xml");
            if (xElement == null) return "";
            var element = xElement.Element("prepay_id");
            if (element == null) return "";
            var prepayId = element.Value;
            var paysignReqHandler = new RequestHandler(null);
            paysignReqHandler.Init();

            //设置支付参数
            paysignReqHandler.SetParameter("appId", appId);
            paysignReqHandler.SetParameter("timeStamp", timestamp);
            paysignReqHandler.SetParameter("nonceStr", nonceStr);
            paysignReqHandler.SetParameter("package", string.Format("prepay_id={0}", prepayId));
            paysignReqHandler.SetParameter("signType", "MD5");
            var paysign = paysignReqHandler.CreateMd5Sign("key", key);
            paysignReqHandler.SetParameter("paysign", paysign);
            NVelocityHelper nh = new NVelocityHelper();
            nh.Put("app_id", appId);
            nh.Put("timestamp", timestamp);
            nh.Put("nonceStr", nonceStr);
            nh.Put("signature", signature);
            nh.Put("package", string.Format("prepay_id={0}", prepayId));
            nh.Put("signType", "MD5");
            nh.Put("paySign", paysign);
            nh.Put("orderid", orderNumber);
            return nh.Write("app_data/template/wechat");
        }

        /// <summary>
        /// 微信退款申请
        /// </summary>
        /// <param name="orderNumber">商户订单号</param>
        /// <param name="refundNo">商户退款单号，商户系统内部唯一，同一退款单号多请求只退一笔</param>
        /// <param name="totalFee">总金额</param>
        /// <param name="refundFee">退款金额</param>
        /// <returns>list[0]=SUCCESS/FAIL，list[1]是具体描述信息</returns>
        public static List<string> Refund(string orderNumber, string refundNo, int totalFee, int refundFee)
        {
            //读取配置文件
            var merchantId = string.Empty;
            var appId = string.Empty;
            var key = string.Empty;
            var certPath = string.Empty;
            var certKey = string.Empty;
            //接收微信支付异步通知回调地址

            var xd = new XmlDocument();
            xd.Load(HttpContext.Current.Server.MapPath("/app_data/xml/payment.xml"));
            var selectSingleNode = xd.SelectSingleNode("paymentMethod");
            if (selectSingleNode != null)
            {
                var xnl = selectSingleNode.ChildNodes;
                foreach (XmlNode xn in xnl)
                {
                    var xe = (XmlElement)xn;
                    if (xe.GetAttribute("id") != "wechat") continue;
                    merchantId = xe.GetAttribute("merchant_id");
                    appId = xe.GetAttribute("app_id");
                    key = xe.GetAttribute("key");
                    certKey = xe.GetAttribute("cert_key");
                    certPath = xe.GetAttribute("cert_path");
                    break;
                }
            }
            var nonceStr = TenPayV3Util.GetNoncestr();
            var packageReqHandler = new RequestHandler(null);
            //设置package订单参数
            packageReqHandler.SetParameter("appid", appId);               //公众账号ID
            packageReqHandler.SetParameter("mch_id", merchantId);         //商户号
            packageReqHandler.SetParameter("nonce_str", nonceStr);         //随机字符串            
            packageReqHandler.SetParameter("out_trade_no", orderNumber);   //填入商户订单号
            packageReqHandler.SetParameter("out_refund_no", refundNo);     //填入退款订单号
            packageReqHandler.SetParameter("total_fee", totalFee.ToString());       //填入总金额
            packageReqHandler.SetParameter("refund_fee", refundFee.ToString());     //填入退款金额
            packageReqHandler.SetParameter("op_user_id", merchantId);     //操作员Id，默认就是商户号
            var sign = packageReqHandler.CreateMd5Sign("key", key);//签名
            packageReqHandler.SetParameter("sign", sign);

            //退款需要post的数据
            var data = packageReqHandler.ParseXML();

            //退款接口地址
            var url = "https://api.mch.weixin.qq.com/secapi/pay/refund";

            //本地或者服务器的证书位置（证书在微信支付申请成功发来的通知邮件中）
            var cert = certPath;

            //私钥（在安装证书时设置）
            var password = certKey;

            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            //调用证书
            var cer = new X509Certificate2(cert, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

            #region 发起post请求
            var webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.ClientCertificates.Add(cer);
            webrequest.Method = "post";

            var postdatabyte = Encoding.UTF8.GetBytes(data);
            webrequest.ContentLength = postdatabyte.Length;
            var stream = webrequest.GetRequestStream();
            stream.Write(postdatabyte, 0, postdatabyte.Length);
            stream.Close();

            var httpWebResponse = (HttpWebResponse)webrequest.GetResponse();
            var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            var responseContent = streamReader.ReadToEnd();
            #endregion

            var res = System.Xml.Linq.XDocument.Parse(responseContent);
            var xElement = res.Element("xml");
            if (xElement == null) return null;
            var element = xElement.Element("return_code");
            if (element == null) return null;
            var returnCode = element.Value;
            var o = xElement.Element("return_msg");
            if (o == null) return null;
            var returnMsg = o.Value;
            var list = new List<string> { returnCode, returnMsg };
            return list;
        }

        /// <summary>
        /// 查询订单退款
        /// </summary>
        /// <param name="orderNumber">订单号</param>
        /// <returns></returns>
        public string RefundQuery(string orderNumber)
        {
            //读取配置文件
            var merchantId = string.Empty;
            var appId = string.Empty;
            var key = string.Empty;
            //接收微信支付异步通知回调地址

            var xd = new XmlDocument();
            xd.Load(HttpContext.Current.Server.MapPath("/app_data/xml/payment.xml"));
            var selectSingleNode = xd.SelectSingleNode("paymentMethod");
            if (selectSingleNode != null)
            {
                var xnl = selectSingleNode.ChildNodes;
                foreach (XmlNode xn in xnl)
                {
                    var xe = (XmlElement)xn;
                    if (xe.GetAttribute("id") != "wechat") continue;
                    merchantId = xe.GetAttribute("merchant_id");
                    appId = xe.GetAttribute("app_id");
                    key = xe.GetAttribute("key");
                    break;
                }
            }
            var nonceStr = TenPayV3Util.GetNoncestr();
            var packageReqHandler = new RequestHandler(null);
            packageReqHandler.SetKey(key);
            packageReqHandler.SetParameter("appid", appId);
            packageReqHandler.SetParameter("mch_id", merchantId);
            packageReqHandler.SetParameter("nonce_str", nonceStr);
            packageReqHandler.SetParameter("out_trade_no", orderNumber);//商户系统内部的订单号

            var querysign = packageReqHandler.CreateMd5Sign("key", key);

            packageReqHandler.SetParameter("sign", querysign);                        //签名

            var data = packageReqHandler.ParseXML();

            var result = TenPayV3.RefundQuery(data);
            var res = System.Xml.Linq.XDocument.Parse(result);

            var xElement = res.Element("xml");
            if (xElement == null) return "";
            var element = xElement.Element("return_code");
            if (element == null) return "";
            var returnCode = element.Value;
            return returnCode;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}
