using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class FilterProductsRequest
	{
		public string ProdDesc
		{
			get;
			set;
		}

		public string ProdLineNo
		{
			get;
			set;
		}

		public FilterProductsRequest()
		{
		}
	}
}