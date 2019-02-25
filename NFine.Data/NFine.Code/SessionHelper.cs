using System;
using System.Web;
using System.Web.SessionState;

namespace NFine.Code
{
    public sealed class SessionHelper : IRequiresSessionState
	{
		#region 写入Session
		/// <summary>
		/// 写入Session
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="value">值</param>
		/// <returns>是否成功</returns>
		public static void SetSession(string key, object value)
		{
			HttpContext.Current.Session[key] = value;
		}
        #endregion

        #region 读取Session
        /// <summary>
        /// 读取Session
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static object GetSession(string key)
		{
                if (HttpContext.Current.Session[key] == null)
                {
                    return null;
                }
                else
                {
                    return HttpContext.Current.Session[key];
                }
		}
		#endregion

		#region 清空Session
		/// <summary>
		/// 清空Session
		/// </summary>
		/// <param name="key">键</param>
		public static void ClearSession(string key)
		{
			HttpContext.Current.Session[key] = string.Empty;
			HttpContext.Current.Session.Remove(key);
		}

		/// <summary>
		/// 清空Session
		/// </summary>
		public static void ClearSession()
		{
			HttpContext.Current.Session.Abandon();
			HttpContext.Current.Session.Clear();
		}
		#endregion
	}
}
