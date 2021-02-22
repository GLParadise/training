using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class ExpirationPeriodRequest
	{
		public int? PackItemId
		{
			get;
			set;
		}

		public ExpirationPeriodRequest()
		{
		}
	}
}