namespace StormworksLuaExtract.Models
{
	public static class Constants
	{
		public const int ReadWriteTimeoutInMilliseconds = 1000;

		public const string MinifiedScriptSuffix = "--min";

		public const int LuaMaximumLength = 4096;

		public const string MinifyLuaApiEndpoint = "https://minify-lua-api.azurewebsites.net/";
	}
}
