using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class FillSequenceSubLotRequest : FillSequenceRequest
	{
		public Guid? parent_fill_id
		{
			get;
			set;
		}

		public FillSequenceSubLotRequest()
		{
		}
	}
}