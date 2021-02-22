using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class DeleteRequest : BaseRequest
	{
		public Guid esig_id
		{
			get;
			set;
		}

		public DeleteRequest()
		{
		}
	}
}