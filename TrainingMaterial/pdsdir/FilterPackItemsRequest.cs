using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class FilterPackItemsRequest
	{
		public string Ndc
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

		public FilterPackItemsRequest()
		{
		}
	}
}