using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class ProductionLinesRequest
	{
		public bool? OnlyFilling
		{
			get;
			set;
		}

		public bool? OnlyFormulation
		{
			get;
			set;
		}

		public int? ProductId
		{
			get;
			set;
		}

		public ProductionLinesRequest()
		{
		}
	}
}