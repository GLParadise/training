using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class BaseRequest
	{
		public Guid? fill_id
		{
			get;
			set;
		}

		public BaseRequest()
		{
		}
	}
}