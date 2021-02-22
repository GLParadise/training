using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class FillingCagesRequest
	{
		public decimal? BatchSize
		{
			get;
			set;
		}

		public string BatchSizeUomCode
		{
			get;
			set;
		}

		public bool? CanChangeCage
		{
			get;
			set;
		}

		public DateTime? FillDate
		{
			get;
			set;
		}

		public Guid? FillId
		{
			get;
			set;
		}

		public int? FillingCageConfigId
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

		public int ProductId
		{
			get;
			set;
		}

		public FillingCagesRequest()
		{
		}
	}
}