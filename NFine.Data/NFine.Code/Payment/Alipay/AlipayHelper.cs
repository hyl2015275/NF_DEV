using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace NFine.Code.Payment.Alipay
{
    public sealed class AlipayHelper
    {
        /// <summary>
        /// 建立请求，以表单HTML形式构造
        /// </summary>
        /// <param name="outTradeNo">订单编号</param>
        /// <param name="totalFee">价格</param>
        /// <returns>提交表单HTML文本</returns>
        public static string Pay(string outTradeNo, double totalFee)
        {
            return Pay("get", outTradeNo, "Billion智能仪表管理平台", totalFee, "Billion智能仪表管理平台");
        }

        /// <summary>
        /// 建立请求，以表单HTML形式构造
        /// </summary>
        /// <param name="outTradeNo">订单编号</param>
        /// <param name="subject">标题</param>
        /// <param name="totalFee">价格</param>
        /// <param name="body">描述</param>
        /// <returns>提交表单HTML文本</returns>
        public static string Pay(string outTradeNo, string subject, double totalFee, string body)
        {
            return Pay("get", outTradeNo, subject, totalFee, body);
        }

        /// <summary>
        /// 建立请求，以表单HTML形式构造
        /// </summary>
        /// <param name="strMethod">提交方式（get，post）</param>
        /// <param name="out_trade_no">订单编号</param>
        /// <param name="subject">标题</param>
        /// <param name="total_fee">价格</param>
        /// <param name="body">描述</param>
        /// <param name="body">物流类型 EXPRESS（快递）、POST（平邮）、EMS（EMS）</param>
        /// <param name="body">物流费用</param>
        /// <returns>提交表单HTML文本</returns>
        public static string Pay(string strMethod, string out_trade_no, string subject, double total_fee, string body)
        {
            SortedDictionary<string, string> _para = new SortedDictionary<string, string>();
            _para.Add("partner", AlipayConfig.Partner);
            _para.Add("seller_email", AlipayConfig.SellerEmail);
            _para.Add("_input_charset", AlipayConfig.InputCharset);
            _para.Add("service", "create_direct_pay_by_user");
            _para.Add("payment_type", "1");
            _para.Add("notify_url", AlipayConfig.NotifyUrl);
            _para.Add("return_url", AlipayConfig.ReturnUrl);
            _para.Add("out_trade_no", out_trade_no);
            _para.Add("subject", subject);
            _para.Add("total_fee", total_fee.ToString());
            _para.Add("body", body);
            _para.Add("show_url", AlipayConfig.ShowUrl);
            _para.Add("anti_phishing_key", Query_timestamp());
            _para.Add("exter_invoke_ip", HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString());
            //待请求参数数组
            Dictionary<string, string> dicPara = new Dictionary<string, string>();
            dicPara = BuildRequestPara(_para);

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + AlipayConfig.GatewayNew + "input_charset="
                + AlipayConfig.InputCharset + "' method='" + strMethod.ToLower().Trim() + "'>");

            foreach (KeyValuePair<string, string> temp in dicPara)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='付款' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }

        /// <summary>
        /// 即时到账批量申请退款
        /// </summary>
        /// <param name="batch_no">批次号，必填，格式：当天日期[8位]+序列号[3至24位]，如：201603081000001</param>
        /// <param name="batch_num">//退款笔数，必填，参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔</param>
        /// <param name="detail_data">退款详细数据，必填，格式（支付宝交易号^退款金额^备注），多笔请用#隔开</param>
        /// <returns></returns>
        public static string Refund(string batch_no, string batch_num, string detail_data)
        {
            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("service", "refund_fastpay_by_platform_pwd");
            sParaTemp.Add("partner", AlipayConfig.Partner);
            sParaTemp.Add("_input_charset", AlipayConfig.InputCharset);
            sParaTemp.Add("notify_url", AlipayConfig.NotifyUrl);
            sParaTemp.Add("seller_user_id", AlipayConfig.Partner);
            sParaTemp.Add("refund_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sParaTemp.Add("batch_no", batch_no);
            sParaTemp.Add("batch_num", batch_num);
            sParaTemp.Add("detail_data", detail_data);

            //待请求参数数组
            Dictionary<string, string> dicPara = new Dictionary<string, string>();
            dicPara = BuildRequestPara(sParaTemp);

            //请求地址
            string GATEWAY_NEW = "https://mapi.alipay.com/gateway.do?";
            //请求方法
            string strMethod = "get";

            //拼接成html页面
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + GATEWAY_NEW + "_input_charset=" + AlipayConfig.InputCharset + "' method='" + strMethod.ToLower().Trim() + "'>");
            foreach (KeyValuePair<string, string> temp in dicPara)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }
            sbHtml.Append("<input type='submit' value='退款' style='display:none;'></form>");
            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            //返回
            return sbHtml.ToString();
        }

        /// <summary>
        /// 生成请求时的签名
        /// </summary>
        /// <param name="sPara">请求给支付宝的参数数组</param>
        /// <returns>签名结果</returns>
        private static string BuildRequestMysign(Dictionary<string, string> sPara)
        {
            //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            string prestr = AlipayCore.CreateLinkString(sPara);

            //把最终的字符串签名，获得签名结果
            string mysign = "";
            switch (AlipayConfig.SignType)
            {
                case "MD5":
                    mysign = AlipayMD5.Sign(prestr, AlipayConfig.Key, AlipayConfig.InputCharset);
                    break;
                default:
                    mysign = "";
                    break;
            }

            return mysign;
        }

        /// <summary>
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <returns>要请求的参数数组</returns>
        private static Dictionary<string, string> BuildRequestPara(SortedDictionary<string, string> sParaTemp)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();
            //签名结果
            string mysign = "";

            //过滤签名参数数组
            sPara = AlipayCore.FilterPara(sParaTemp);

            //获得签名结果
            mysign = BuildRequestMysign(sPara);

            //签名结果与签名方式加入请求提交参数组中
            sPara.Add("sign", mysign);
            sPara.Add("sign_type", AlipayConfig.SignType);

            return sPara;
        }

        /// <summary>
        /// 生成要请求给支付宝的参数数组
        /// </summary>
        /// <param name="sParaTemp">请求前的参数数组</param>
        /// <param name="code">字符编码</param>
        /// <returns>要请求的参数数组字符串</returns>
        private static string BuildRequestParaToString(SortedDictionary<string, string> sParaTemp, Encoding code)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();
            sPara = BuildRequestPara(sParaTemp);

            //把参数组中所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
            string strRequestData = AlipayCore.CreateLinkStringUrlencode(sPara, code);

            return strRequestData;
        }

        /// <summary>
        /// 用于防钓鱼，调用接口query_timestamp来获取时间戳的处理函数
        /// 注意：远程解析XML出错，与IIS服务器配置有关
        /// </summary>
        /// <returns>时间戳字符串</returns>
        private static string Query_timestamp()
        {
            string url = AlipayConfig.GatewayNew + "service=query_timestamp&partner=" + AlipayConfig.Partner + "&input_charset=" + AlipayConfig.InputCharset;
            string encrypt_key = "";

            XmlTextReader Reader = new XmlTextReader(url);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Reader);

            encrypt_key = xmlDoc.SelectSingleNode("/alipay/response/timestamp/encrypt_key").InnerText;

            return encrypt_key;
        }
    }
}