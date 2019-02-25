using System.IO;
using System.Net;
using System.Text;

namespace NFine.Code
{
	public static class NetHelper
	{
		#region Get
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string Get(string url)
		{
			return Get(url, Encoding.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string Get(string url, Encoding encoding)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return string.Empty;
			}
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "get";
			var stream = request.GetResponse().GetResponseStream();
			using (var reader = new StreamReader(stream, encoding))
			{
				return reader.ReadToEnd();
			}
		}
		#endregion

		#region Post
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string Post(string url)
		{
			return Post(url, null, Encoding.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string Post(string url, Encoding encoding)
		{
			return Post(url, null, encoding);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string Post(string url, byte[] bytes)
		{
			return Post(url, bytes, Encoding.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="bytes"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string Post(string url, byte[] bytes, Encoding encoding)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return string.Empty;
			}
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "post";
			request.ContentType = "text/json; charset=utf-8";
			if (bytes.Length > 0)
			{
				request.ContentLength = bytes.Length;
				Stream writer = request.GetRequestStream();
				writer.Write(bytes, 0, bytes.Length);
				writer.Close();
			}
			var stream = request.GetResponse().GetResponseStream();
			using (var reader = new StreamReader(stream, encoding))
			{
				return reader.ReadToEnd();
			}
		}
		#endregion
	}
}
