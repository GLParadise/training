using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class UpdateFormulationFinishDateRequest
	{
		public Guid EsignId
		{
			get;
			set;
		}

		public int ExpirationInterval
		{
			get;
			set;
		}

		public string ExpirationIntervalType
		{
			get;
			set;
		}

		public DateTime? FillDate
		{
			get;
			set;
		}

		public string Notes
		{
			get;
			set;
		}

		public DateTime? ScheduledDate
		{
			get;
			set;
		}

		public UpdateFormulationFinishDateRequest()
		{
		}
	}
}