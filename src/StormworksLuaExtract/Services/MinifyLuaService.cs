using System.Net.Http;
using System.Text;
using StormworksLuaExtract.Helpers;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Services
{
	public class MinifyLuaService
	{
		public string Minify(string script)
		{
			using (var httpClient = new HttpClient())
			{
				var httpContent = new StringContent(script, Encoding.UTF8);
				using (var response = httpClient.PostAsync(Constants.MinifyLuaApiEndpoint, httpContent).Result)
				{
					if (!response.IsSuccessStatusCode)
					{
						ConsoleHelper.WriteWarning("Failed to minify lua script.");
						return null;
					}

					var minifiedScript = response.Content.ReadAsStringAsync().Result;
					return minifiedScript;
				}
			}
		}
	}
}
