using Autofac;
using StormworksLuaExtract.Services;
using System.Threading.Tasks;

namespace StormworksLuaExtract
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<ApplicationService>().AsSelf();

			var container = builder.Build();

			await container.Resolve<ApplicationService>().Run();
		}
	}
}
