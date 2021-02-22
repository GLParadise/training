using Autofac;
using Autofac.Builder;
using Autofac.Integration.WebApi;
using Streamline.Shared.LoggingV2;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Http.Dependencies;

namespace Web
{
	public class Bootstrapper
	{
		private readonly static string AppName;

		private readonly static string LoggingLevels;

		static Bootstrapper()
		{
			Bootstrapper.AppName = ConfigurationManager.AppSettings["Application.Name"];
			Bootstrapper.LoggingLevels = ConfigurationManager.AppSettings["LoggingLevels"];
		}

		public Bootstrapper()
		{
		}

		private static IContainer BuildContainer()
		{
			ContainerBuilder builder = new ContainerBuilder();
			Assembly assembly = Assembly.GetExecutingAssembly();
			builder.RegisterType<StinvEntities>().WithParameter<StinvEntities, ConcreteReflectionActivatorData, SingleRegistrationStyle>("connectionString", Bootstrapper.CreateConnectionString("stinv")).InstancePerRequest<StinvEntities, ConcreteReflectionActivatorData, SingleRegistrationStyle>(new object[0]);
			builder.RegisterType<UsersEntities>().WithParameter<UsersEntities, ConcreteReflectionActivatorData, SingleRegistrationStyle>("connectionString", Bootstrapper.CreateConnectionString("users")).InstancePerRequest<UsersEntities, ConcreteReflectionActivatorData, SingleRegistrationStyle>(new object[0]);
			builder.RegisterApiControllers(new Assembly[] { assembly });
			LogProperties logProperty = new LogProperties()
			{
				ApplicationName = Bootstrapper.AppName,
				ApplicationVersion = typeof(Bootstrapper).Assembly.GetName().Version.ToString(),
				SourceName = "None",
				UserName = "None"
			};
			builder.Register<Logger>((IComponentContext c) => new Logger(Bootstrapper.LoggingLevels, logProperty, null)).As<ILogger>().SingleInstance();
			return builder.Build(ContainerBuildOptions.None);
		}

		private static string CreateConnectionString(string connectionStringName)
		{
			return (new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString)
			{
				ApplicationName = Bootstrapper.AppName
			}).ConnectionString;
		}

		public static IDependencyResolver CreateDependencyResolver()
		{
			return new AutofacWebApiDependencyResolver(Bootstrapper.BuildContainer());
		}
	}
}