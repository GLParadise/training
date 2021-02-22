using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class MoldTypeVialsPerCardStackHeightRequest
	{
		public int pack_item_id
		{
			get;
			set;
		}

		public string prod_line_no
		{
			get;
			set;
		}

		public MoldTypeVialsPerCardStackHeightRequest()
		{
		}
	}
}