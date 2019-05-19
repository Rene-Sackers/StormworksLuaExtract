using Autofac;
using StormworksLuaExtract.Services;

namespace StormworksLuaExtract
{
	public class Program
	{
		public static void Main()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<ApplicationService>().AsSelf();
			builder.RegisterType<VehiclesWatchService>().AsSelf();
			builder.RegisterType<LocalLuaToXmlWriteService>().AsSelf();
			builder.RegisterType<XmlToLocalLuaWriteService>().AsSelf();

			var container = builder.Build();

			container.Resolve<ApplicationService>().Run();
		}
	}
}
