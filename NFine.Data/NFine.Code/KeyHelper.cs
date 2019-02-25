namespace NFine.Code
{
	public struct KeyHelper
	{
	    private static readonly string Cs = Configs.GetAppSetting("CookiesSalt");
		//用户信息
		public static string UserId = ("UserId" + Cs).EncryptMD5();
		public static string UserName = ("UserName" + Cs).EncryptMD5();
		public static string NickName = ("NickName" + Cs).EncryptMD5();
		public static string UserImg = ("UserImg" + Cs).EncryptMD5();
		public static string Email = ("Email" + Cs).EncryptMD5();
		public static string RoleId = ("RoleId" + Cs).EncryptMD5();
		public static string RoleName = ("RoleName" + Cs).EncryptMD5();
		public static string UserInFo = ("UserInFo" + Cs).EncryptMD5();
		public static string TypeId = ("TypeId" + Cs).EncryptMD5();
		public static string TypeName = ("TypeName" + Cs).EncryptMD5();
        public static string Mobile = ("Mobile" + Cs).EncryptMD5();
		//验证码
		public static string VCode = ("VCode" + Cs).EncryptMD5();
		//手机验证码
		public static string Code = ("Code" + Cs).EncryptMD5();
		//代码生成器
		public static string Type = ("Type" + Cs).EncryptMD5();
		public static string Source = ("Source" + Cs).EncryptMD5();
		public static string Password = ("Password" + Cs).EncryptMD5();
		public static string IsLogin = ("IsLogin" + Cs).EncryptMD5();

		public static string Controller = ("Controller" + Cs).EncryptMD5();
		public static string Action = ("Action" + Cs).EncryptMD5();
		public static string Id = ("Id" + Cs).EncryptMD5();

		public static string Message = ("Message" + Cs).EncryptMD5();
	}
}
