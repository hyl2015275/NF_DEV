using System;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using System.IO;
using System.Web;

namespace NFine.Code
{
    public sealed class NVelocityHelper
    {
        #region 配置参数
        private VelocityEngine ve = null;
        private VelocityContext vc = null;
        #endregion

        #region NVelocity
        /// <summary>
        /// NVelocity
        /// </summary>
        public NVelocityHelper()
        {
            //创建VelocityEngine实例对象
            ve = new VelocityEngine();
            //初始化VelocityEngine
            ExtendedProperties eps = new ExtendedProperties();
            //编码
            eps.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
            eps.AddProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");
            eps.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");
            eps.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, HttpContext.Current.Server.MapPath(@"~/"));
            ve.Init(eps);

            //整合模板
            vc = new VelocityContext();
        }
        /// <summary>
        /// NVelocity
        /// </summary>
        /// <param name="path">path</param>
        public NVelocityHelper(string path)
        {
            //创建VelocityEngine实例对象
            ve = new VelocityEngine();
            //初始化VelocityEngine
            ExtendedProperties eps = new ExtendedProperties();
            //编码
            eps.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
            eps.AddProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");
            eps.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");
            eps.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, HttpContext.Current.Server.MapPath(path));
            ve.Init(eps);

            //整合模板
            vc = new VelocityContext();
        }
        #endregion

        #region Put
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void Put(string key, object value)
        {
            vc.Put(key, value);
        }
        #endregion

        #region Display
        /// <summary>
        /// Display
        /// </summary>
        /// <param name="path">path</param>
        public void Display(string path)
        {
            //加载模板
            Template vm = ve.GetTemplate(path);
            //写入
            using (StringWriter sw = new StringWriter())
            {
                vm.Merge(vc, sw);
                //输出
                HttpContext.Current.Response.Write(sw.ToString());
            }
        }
        #endregion

        #region Write
        /// <summary>
        /// Write
        /// </summary>
        /// <param name="path">path</param>
        public string Write(string path)
        {
            //加载模板
            Template vm = ve.GetTemplate(path);
            //写入
            using (StringWriter sw = new StringWriter())
            {
                vm.Merge(vc, sw);
                //输出
                return sw.ToString();
            }
        }
        #endregion

        #region Save
        /// <summary>
        /// Display
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="savePath">savePath</param>
        public void Save(string path, string savePath)
        {
            //加载模板
            Template vm = ve.GetTemplate(path);
            //写入
            using (StringWriter sw = new StringWriter())
            {
                vm.Merge(vc, sw);
                //保存文件
                string str = sw.ToString().Replace("\\t", "    ");
                WriteFile(str, savePath);
            }
        }
        public static bool WriteFile(string str, string path)
        {
            try
            {
                FileHelper.CreateFile(path);
                //写入文件
                StreamWriter sw = new StreamWriter(path);
                sw.Write(str);
                sw.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
