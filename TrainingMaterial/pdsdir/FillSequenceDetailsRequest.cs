using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class FillSequenceDetailsRequest
	{
		public Guid? FillId
		{
			get;
			set;
		}

		public bool? IsCreateSubLot
		{
			get;
			set;
		}

		public FillSequenceDetailsRequest()
		{
		}
	}
}