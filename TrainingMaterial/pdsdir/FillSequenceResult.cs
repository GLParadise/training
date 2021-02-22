using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class FillSequenceResult
	{
		public Guid? FillId
		{
			get;
			set;
		}

		public string LotNo
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

		public FillSequenceResult()
		{
		}
	}
}