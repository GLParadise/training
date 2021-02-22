using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class ConfirmationRequest : BaseRequest
	{
		public Guid esig_id
		{
			get;
			set;
		}

		public ConfirmationRequest()
		{
		}
	}
}