using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class UpdateAfterProductionRequest
	{
		public string Notes
		{
			get;
			set;
		}

		public string ProdLineNo
		{
			get;
			set;
		}

		public DateTime? ScheduledDate
		{
			get;
			set;
		}

		public UpdateAfterProductionRequest()
		{
		}
	}
}