using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Streamline.Shared.Infrastructure.WebApi;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace Web
{
	public class Global : HttpApplication
	{
		public Global()
		{
		}

		protected void Application_Start(object sender, EventArgs e)
		{
			GlobalConfiguration.get_Configuration().set_DependencyResolver(Bootstrapper.CreateDependencyResolver());
			GlobalConfiguration.Configure(new Action<HttpConfiguration>(Global.WebApiConfig.Register));
			GlobalConfiguration.get_Configuration().get_Services().Add(typeof(IExceptionLogger), new GlobalExceptionLogger());
			GlobalConfiguration.get_Configuration().get_Services().Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
			GlobalConfiguration.get_Configuration().get_Filters().Add(new InvalidModelStateFilterAttribute());
			GlobalConfiguration.get_Configuration().get_MessageHandlers().Add(new EveryRequestMessageHandler());
		}

		public class WebApiConfig
		{
			public WebApiConfig()
			{
			}

			public static void Register(HttpConfiguration configuration)
			{
				CorsHttpConfigurationExtensions.EnableCors(configuration, new EnableCorsAttribute("*", "*", "*"));
				HttpConfigurationExtensions.MapHttpAttributeRoutes(configuration);
				MediaTypeHeaderValue appXmlType = configuration.get_Formatters().get_XmlFormatter().get_SupportedMediaTypes().FirstOrDefault<MediaTypeHeaderValue>((MediaTypeHeaderValue t) => t.MediaType == "application/xml");
				configuration.get_Formatters().get_XmlFormatter().get_SupportedMediaTypes().Remove(appXmlType);
				JsonSerializerSettings serializerSettings = configuration.get_Formatters().get_JsonFormatter().get_SerializerSettings();
				DefaultContractResolver defaultContractResolver = new DefaultContractResolver();
				defaultContractResolver.set_IgnoreSerializableAttribute(true);
				serializerSettings.set_ContractResolver(defaultContractResolver);
			}
		}
	}
}