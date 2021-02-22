using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers
{
	public class ProdLineInfoResult
	{
		public string description
		{
			get;
			set;
		}

		public int? @value
		{
			get;
			set;
		}

		public ProdLineInfoResult()
		{
		}
	}
}