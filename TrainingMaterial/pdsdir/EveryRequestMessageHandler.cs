using Newtonsoft.Json;
using Streamline.Shared.LoggingV2;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace Web
{
	public class EveryRequestMessageHandler : DelegatingHandler
	{
		private readonly static string UsersApiRootUrl;

		static EveryRequestMessageHandler()
		{
			EveryRequestMessageHandler.UsersApiRootUrl = ConfigurationManager.AppSettings["api_users"];
		}

		public EveryRequestMessageHandler()
		{
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			string str;
			string str1;
			ILogger logger = (ILogger)GlobalConfiguration.get_Configuration().get_DependencyResolver().GetService(typeof(ILogger));
			if (request != null && request.Headers != null && request.Method != HttpMethod.Options && request.Method != HttpMethod.Head && request.Method != HttpMethod.Trace)
			{
				LogProperties logProperties = logger.LogProperties;
				HttpMethod method = request.Method;
				if (method != null)
				{
					str = method.ToString();
				}
				else
				{
					str = null;
				}
				Uri requestUri = request.RequestUri;
				if (requestUri != null)
				{
					str1 = requestUri.ToString();
				}
				else
				{
					str1 = null;
				}
				logProperties.SourceName = string.Concat(str, " ", str1);
				if (request.Headers.Authorization == null || string.IsNullOrEmpty(request.Headers.Authorization.Scheme))
				{
					logger.LogProperties.UserName = "Not logged in";
				}
				else
				{
					string sessionToken = request.Headers.Authorization.Scheme;
					request.Properties.Add("SessionToken", sessionToken);
					string requestUrl = Path.Combine(EveryRequestMessageHandler.UsersApiRootUrl, "user", sessionToken);
					try
					{
						using (HttpClient client = new HttpClient())
						{
							string result = client.GetStringAsync(requestUrl).Result;
							string userName = "No username";
							Guid userId = Guid.Empty;
							if (!string.IsNullOrWhiteSpace(result))
							{
								EveryRequestMessageHandler.User user = JsonConvert.DeserializeObject<EveryRequestMessageHandler.User>(result);
								userName = user.userName;
								userId = user.id;
							}
							request.Properties.Add("UserJson", result);
							request.Properties.Add("UserName", userName);
							request.Properties.Add("UserId", userId);
							logger.LogProperties.UserName = userName;
						}
					}
					catch
					{
						logger.LogProperties.UserName = "Bad Token";
					}
				}
			}
			return base.SendAsync(request, cancellationToken);
		}

		private class User
		{
			public Guid id
			{
				get;
				set;
			}

			public string userName
			{
				get;
				set;
			}

			public User()
			{
			}
		}
	}
}