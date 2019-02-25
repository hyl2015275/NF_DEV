/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System;
using System.Configuration;
using System.Web;
using System.Xml;

namespace NFine.Code
{
    public class Configs
    {
        /// <summary>
        /// 根据Key取Value值
        /// </summary>
        /// <param name="key"></param>
        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString().Trim();
        }
        /// <summary>
        /// 根据Key修改Value
        /// </summary>
        /// <param name="key">要修改的Key</param>
        /// <param name="value">要修改为的值</param>
        public static void SetValue(string key, string value)
        {
            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.Load(HttpContext.Current.Server.MapPath("~/Configs/system.config"));
            System.Xml.XmlNode xNode;
            System.Xml.XmlElement xElem1;
            System.Xml.XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");

            xElem1 = (System.Xml.XmlElement)xNode.SelectSingleNode("//add[@key='" + key + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", value);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", key);
                xElem2.SetAttribute("value", value);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(HttpContext.Current.Server.MapPath("~/Configs/system.config"));
        }
        #region 得到自定义的webconfig文件配置节点的值
        /// <summary>
        /// 得到自定义的webconfig文件配置节点的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        #endregion

        #region 得到数据库连接字符串
        /// <summary>
        /// 得到数据库连接字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            if (GetAppSetting("EncryptStrConn").Trim() == "Encrypt")
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString.DeBase64();
            }
            else
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
        }
        #endregion

        #region 修改config文件
        /// <summary>
        /// 修改config文件(AppSetting节点)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">要修改成的值</param>
        public static void UpdateAppSetting(string key, string value)
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径 
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Web.config";
            doc.Load(strFileName);
            //找出名称为“add”的所有元素 
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性 
                XmlAttribute _key = nodes[i].Attributes["key"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素 
                if (_key != null)
                {
                    if (_key.Value == key)
                    {
                        //对目标元素中的第二个属性赋值 
                        _key = nodes[i].Attributes["value"];
                        _key.Value = value;
                        break;
                    }
                }
            }
            //保存上面的修改 
            doc.Save(strFileName);
        }

        /// <summary>
        /// 修改config文件(ConnectionString节点)
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">要修改成的值</param>
        public static void UpdateConnectionString(string name, string value)
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径 
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Web.config";
            doc.Load(strFileName);
            //找出名称为“add”的所有元素 
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性 
                XmlAttribute _name = nodes[i].Attributes["name"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素 
                if (_name != null)
                {
                    if (_name.Value == name)
                    {
                        //对目标元素中的第二个属性赋值 
                        _name = nodes[i].Attributes["connectionString"];
                        _name.Value = value;
                        break;
                    }
                }
            }
            //保存上面的修改 
            doc.Save(strFileName);
        }
        #endregion

        #region 生成ConnectionString
        /// <summary>
        /// 生成ConnectionString(sql server 身份验证)
        /// </summary>
        /// <param name="source">服务器名称</param>
        /// <param name="id">登录名</param>
        /// <param name="password">密码</param>
        /// <returns>数据库连接字符串</returns>
        public static string GeneratConnectionString(string source, string id, string password)
        {
            return "Data Source=" + source + ";Persist Security Info=True;User ID=" + id + ";Password=" + password;
        }

        /// <summary>
        /// 生成ConnectionString(sql server 身份验证)
        /// </summary>
        /// <param name="source">服务器名称</param>
        /// <param name="database">数据库名称</param>
        /// <param name="id">登录名</param>
        /// <param name="password">密码</param>
        /// <returns>数据库连接字符串</returns>
        public static string GeneratConnectionString(string source, string database, string id, string password)
        {
            return "Data Source=" + source + ";Persist Security Info=True;Initial Catalog=" + database + ";User ID=" + id + ";Password=" + password;
        }

        /// <summary>
        /// 生成ConnectionString(windows 身份验证)
        /// </summary>
        /// <param name="source">服务器名称</param>
        /// <returns>数据库连接字符串</returns>
        public static string GeneratConnectionString(string source)
        {
            return "Data Source=" + source + ";Integrated Security=True";
        }

        /// <summary>
        /// 生成ConnectionString(windows 身份验证)
        /// </summary>
        /// <param name="source">服务器名称</param>
        /// <param name="database">数据库名称</param>
        /// <returns>数据库连接字符串</returns>
        public static string GeneratConnectionString(string source, string database)
        {
            return "Data Source=" + source + ";Initial Catalog=" + database + ";Integrated Security=True";
        }
        #endregion
    }
}
