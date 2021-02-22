using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class ApplicationParameterRequest
	{
		public string application_name
		{
			get;
			set;
		}

		public int? location_id
		{
			get;
			set;
		}

		public string p_name
		{
			get;
			set;
		}

		public ApplicationParameterRequest()
		{
		}
	}
}