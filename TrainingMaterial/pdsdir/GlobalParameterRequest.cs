using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class GlobalParameterRequest
	{
		public int? LocationId
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public GlobalParameterRequest()
		{
		}
	}
}