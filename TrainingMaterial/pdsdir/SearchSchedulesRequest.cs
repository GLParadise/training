using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class SearchSchedulesRequest
	{
		public DateTime FromDate
		{
			get;
			set;
		}

		public bool? IsFormulationBatch
		{
			get;
			set;
		}

		public string LotNo
		{
			get;
			set;
		}

		public string MfNumber
		{
			get;
			set;
		}

		public bool? OnlyProduction
		{
			get;
			set;
		}

		public int? PackItemId
		{
			get;
			set;
		}

		public string ProdLineNo
		{
			get;
			set;
		}

		public int? ProductId
		{
			get;
			set;
		}

		public string StatusCode
		{
			get;
			set;
		}

		public DateTime ToDate
		{
			get;
			set;
		}

		public SearchSchedulesRequest()
		{
		}
	}
}