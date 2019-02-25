/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System;

namespace NFine.Code
{
    public class OperatorProvider
    {
        public static OperatorProvider Provider
        {
            get { return new OperatorProvider(); }
        }
        private string LoginUserKey = "billion_2018";
        private readonly string _loginProvider = Configs.GetValue("LoginProvider");

        public OperatorModel GetCurrent()
        {
            try
            {
                var operatorModel = _loginProvider == "Cookie"
               ? DESEncrypt.Decrypt(WebHelper.GetCookie(LoginUserKey)).ToObject<OperatorModel>()
               : DESEncrypt.Decrypt(WebHelper.GetSession(LoginUserKey)).ToObject<OperatorModel>();
                if (operatorModel == null)
                {
                    throw new Exception()
                    {
                        Source = "timeout"
                    };
                }
                return operatorModel;
            }
            catch
            {
                return null;
            }

        }
        public void AddCurrent(OperatorModel operatorModel)
        {
            if (_loginProvider == "Cookie")
            {
                WebHelper.WriteCookie(LoginUserKey, DESEncrypt.Encrypt(operatorModel.ToJson()), 60);
            }
            else
            {
                WebHelper.WriteSession(LoginUserKey, DESEncrypt.Encrypt(operatorModel.ToJson()));
            }
            WebHelper.WriteCookie("nfine_mac", Md5.md5(Net.GetMacByNetworkInterface().ToJson(), 32));
            WebHelper.WriteCookie("nfine_licence", Licence.GetLicence());
        }
        public void RemoveCurrent()
        {
            if (_loginProvider == "Cookie")
            {
                WebHelper.RemoveCookie(LoginUserKey.Trim());
            }
            else
            {
                WebHelper.RemoveSession(LoginUserKey.Trim());
            }
        }
    }
}
