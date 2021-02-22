using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Extensions
{
	public static class DecimalExtensions
	{
		public static string ToStringPrec4(this decimal input)
		{
			return input.ToString("##,###.0000");
		}

		public static string ToStringPrec4(this decimal? input)
		{
			return input.GetValueOrDefault().ToStringPrec4();
		}
	}
}