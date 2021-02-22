using Newtonsoft.Json;
using Streamline.Shared.Infrastructure.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace Web.Api.Controllers
{
	public class BaseApiController : ApiController
	{
		protected string ApplicationName
		{
			get
			{
				return ConfigurationManager.AppSettings["Application.Name"];
			}
		}

		protected string ApplicationVersion
		{
			get
			{
				return base.GetType().Assembly.GetName().Version.ToString();
			}
		}

		protected string RequestClientIpAddress
		{
			get
			{
				return base.get_Request().GetClientIpAddress();
			}
		}

		protected string SessionToken
		{
			get
			{
				return base.get_Request().Properties["SessionToken"] as string;
			}
		}

		public int UserId
		{
			get
			{
				User user = JsonConvert.DeserializeObject<User>(base.get_Request().Properties["UserJson"].ToString());
				if (user == null)
				{
					return 0;
				}
				return user.stinvUser.userId;
			}
		}

		protected string Username
		{
			get
			{
				return base.get_Request().Properties["UserName"] as string;
			}
		}

		public BaseApiController()
		{
		}
	}
}