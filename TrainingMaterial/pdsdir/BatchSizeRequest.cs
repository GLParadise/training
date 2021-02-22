using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class BatchSizeRequest
	{
		public int MfId
		{
			get;
			set;
		}

		public bool? OnlyProduction
		{
			get;
			set;
		}

		public BatchSizeRequest()
		{
		}
	}
}