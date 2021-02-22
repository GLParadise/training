using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class PackItemsRequest
	{
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

		public PackItemsRequest()
		{
		}
	}
}