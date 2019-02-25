using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NFine.Code
{
	public static class StringHelper
	{
		#region MD5 加密
		/// <summary>
		/// MD5 加密（不可逆）
		/// </summary>
		/// <param name="str">待加密的字符串</param>
		/// <returns>加密后的字符串</returns>
		public static string EncryptMD5(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] t = md5.ComputeHash(Encoding.Default.GetBytes(str));
			StringBuilder sb = new StringBuilder(32);
			for (int i = 0; i < t.Length; i++)
			{
				sb.Append(t[i].ToString("x").PadLeft(2, '0'));
			}

			return sb.ToString();
		}
		/// <summary>
		/// MD5 加密（不可逆）
		/// </summary>
		/// <param name="str">待加密的字符串</param>
		/// <param name="charset">编码</param>
		/// <returns>加密后的字符串</returns>
		public static string EncryptMD5(this string str, string charset)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] t = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(str));
			StringBuilder sb = new StringBuilder(32);
			for (int i = 0; i < t.Length; i++)
			{
				sb.Append(t[i].ToString("x").PadLeft(2, '0'));
			}

			return sb.ToString();
		}
		#endregion

		#region SHA1 加密
		/// <summary>
		/// SHA1 加密（不可逆）
		/// </summary>
		/// <param name="str">待加密的字符串</param>
		/// <returns>加密后的字符串</returns>
		public static string EncryptSHA1(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			byte[] StrRes = Encoding.Default.GetBytes(str);
			HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
			StrRes = iSHA.ComputeHash(StrRes);
			StringBuilder EnText = new StringBuilder();
			foreach (byte iByte in StrRes)
			{
				EnText.AppendFormat("{0:x2}", iByte);
			}
			return EnText.ToString();
		}
		#endregion

		#region DES 加密
		/// <summary>
		/// DES 加密(可逆)
		/// </summary>
		/// <param name="str">待加密的字符串</param>
		/// <param name="key">解密所需8位密钥</param>
		/// <returns>加密后的字符串</returns>
		public static string EncryptDES(string str, string key)
		{
			if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(key))
			{
				return string.Empty;
			}
			byte[] rgbKey = Encoding.UTF8.GetBytes(key);
			byte[] rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
			byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
			DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
			MemoryStream mStream = new MemoryStream();
			CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
			cStream.Write(inputByteArray, 0, inputByteArray.Length);
			cStream.FlushFinalBlock();
			return Convert.ToBase64String(mStream.ToArray());
		}
		#endregion

		#region DES 解密
		/// <summary>
		/// DES 解密
		/// </summary>
		/// <param name="str">待解密的字符串</param>
		/// <param name="key">解密所需8位密钥</param>
		/// <returns>解密后的字符串</returns>
		public static string DecryptDES(string str, string key)
		{
			if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(key))
			{
				return string.Empty;
			}
			byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
			byte[] rgbKey = Encoding.UTF8.GetBytes(key);
			byte[] rgbIV = Keys;
			byte[] inputByteArray = Convert.FromBase64String(str);
			DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
			MemoryStream mStream = new MemoryStream();
			CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
			cStream.Write(inputByteArray, 0, inputByteArray.Length);
			cStream.FlushFinalBlock();
			return Encoding.UTF8.GetString(mStream.ToArray());
		}
		#endregion

		#region AES 加密
		/// <summary>
		/// AES 加密
		/// </summary>
		/// <param name="str">待加密的字符串</param>
		/// <param name="key">解密所需32位密钥</param>
		/// <returns>加密后的字符串</returns>
		public static String EncryptAES(string str, string key)
		{
			if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(key))
			{
				return string.Empty;
			}

			Byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
			Byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(str);

			RijndaelManaged rDel = new RijndaelManaged();
			rDel.Key = keyArray;
			rDel.Mode = CipherMode.ECB;
			rDel.Padding = PaddingMode.PKCS7;

			ICryptoTransform cTransform = rDel.CreateEncryptor();
			Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}
		#endregion

		#region AES 解密
		/// <summary>
		/// AES 解密
		/// </summary>
		/// <param name="str">待解密的字符串</param>
		/// <param name="key">解密所需32位密钥</param>
		/// <returns>解密后的字符串</returns>
		public static String DecryptAES(string str, string key)
		{
			if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(key))
			{
				return string.Empty;
			}
			Byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
			Byte[] toEncryptArray = Convert.FromBase64String(str);

			RijndaelManaged rDel = new RijndaelManaged();
			rDel.Key = keyArray;
			rDel.Mode = CipherMode.ECB;
			rDel.Padding = PaddingMode.PKCS7;

			ICryptoTransform cTransform = rDel.CreateDecryptor();
			Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

			return UTF8Encoding.UTF8.GetString(resultArray);
		}
		#endregion

		#region Base64 编码
		/// <summary>
		/// Base64 编码
		/// </summary>
		/// <param name="str">待编码的字符串</param>
		/// <returns>编码后的字符串</returns>
		public static string ToBase64(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
		}
		#endregion

		#region  Base64 解码
		/// <summary>
		/// Base64 解码
		/// </summary>
		/// <param name="str">待解码的字符串</param>
		/// <returns>解码后的字符串</returns>
		public static string DeBase64(this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(Convert.FromBase64String(str));
		}
		#endregion

		#region GetDefaultValue
		/// <summary>
		/// 得到默认值
		/// </summary>
		/// <param name="value">类型</param>
		/// <returns>默认值</returns>
		public static string GetDefaultValue(this string value)
		{
			return string.Empty;
		}
		#endregion

		#region Like
		/// <summary>
		/// Like
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="like">匹配字符串</param>
		/// <returns>结果</returns>
		public static bool Like(this string str, string like)
		{
			if (!string.IsNullOrWhiteSpace(str)
				&& !string.IsNullOrWhiteSpace(like))
			{
				if (str.IndexOf(like) != -1)
				{
					return true;
				}
				else
				{
					if (like.Length > 0)
					{
						foreach (var _char in like)
						{
							if (str.IndexOf(_char) != -1)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		#endregion

		#region 过滤
		/// <summary>
		///   过滤
		/// </summary>
		/// <param name="str">需要过滤的字符串</param>
		/// <param name="html">过滤HTML</param>
		/// <param name="sql">过滤与数据库相关的词</param>
		/// <param name="special">过滤的字符</param>
		/// <returns>过滤结果</returns>
		public static string Filter(this string str, bool html, bool sql, bool special)
		{
			if (str == null)
			{
				return string.Empty;
			}
			else
			{
				//过滤HTML
				if (html)
				{
					//过滤脚本,样式
					str = Regex.Replace(str, "(<script)[\\s\\S]*?(</script>)|(<style)[\\s\\S]*?(</style>)", " ", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"-->", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"<!--.*", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, @"&#(\d+);", "", RegexOptions.IgnoreCase);
				}
				//过滤与数据库相关的词
				if (sql)
				{
					str = Regex.Replace(str, "xp_cmdshell", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "select", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "insert", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "delete from", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "count''", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "drop table", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "truncate", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "asc", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "mid", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "char", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "xp_cmdshell", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "exec master", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "net localgroup administrators", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "and", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "net user", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "or", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "net", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "\\*", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "\\-", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "delete", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "drop", "", RegexOptions.IgnoreCase);
					str = Regex.Replace(str, "script", "", RegexOptions.IgnoreCase);
				}
				//过滤的字符
				if (special)
				{
					str = str.Replace("<", "");
					str = str.Replace(">", "");
					str = str.Replace("*", "");
					str = str.Replace("-", "");
					str = str.Replace("?", "");
					str = str.Replace(",", "");
					str = str.Replace("/", "");
					str = str.Replace(";", "");
					str = str.Replace("*/", "");
					str = str.Replace("\r\n", "");
				}
				return str;
			}
		}
		#endregion

		public static byte[] GetBytes(this string str)
		{
			return Encoding.Default.GetBytes(str);
		}
		public static byte[] GetBytes(this string str, Encoding encoding)
		{
			return encoding.GetBytes(str);
		}
        public static string ConvertToChinese(double dou)
        {
            string s =
               dou.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s,
                @"((?<=-|^)[^\-1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))
                |((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            string RMB = "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰";
            string r = Regex.Replace(d, ".", m => RMB[m.Value[0] - '-'].ToString());
            if (r.EndsWith("元"))
                r = r + "整";
            return r;
        }
	}
}