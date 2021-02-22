using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Extensions
{
	public static class BooleanExtensions
	{
		public static string ToYesNoString(this bool input)
		{
			if (!input)
			{
				return "No";
			}
			return "Yes";
		}

		public static string ToYesNoString(this bool? input)
		{
			return input.GetValueOrDefault().ToYesNoString();
		}
	}
}