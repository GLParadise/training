using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Extensions
{
	public static class StringYesNoExtensions
	{
		public static string StringYesNoOutput(this string input)
		{
			if (input != "Y")
			{
				return "No";
			}
			return "Yes";
		}
	}
}