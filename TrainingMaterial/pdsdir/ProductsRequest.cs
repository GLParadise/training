using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class ProductsRequest
	{
		public int? MfId
		{
			get;
			set;
		}

		public bool? OnlyProduction
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

		public ProductsRequest()
		{
		}
	}
}