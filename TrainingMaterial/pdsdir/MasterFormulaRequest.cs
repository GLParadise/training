using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class MasterFormulaRequest : BaseRequest
	{
		public decimal? BatchSize
		{
			get;
			set;
		}

		public DateTime? FillDate
		{
			get;
			set;
		}

		public int? MfConfigId
		{
			get;
			set;
		}

		public bool? OnlyProduction
		{
			get;
			set;
		}

		public int ProductId
		{
			get;
			set;
		}

		public Guid? ScheduledFillId
		{
			get;
			set;
		}

		public MasterFormulaRequest()
		{
		}
	}
}