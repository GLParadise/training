using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class EditFillSequenceFormulationFinishDateResult
	{
		public Guid? FillId
		{
			get;
			set;
		}

		public string LotNo
		{
			get;
			set;
		}

		public string ProdLineNo
		{
			get;
			set;
		}

		public DateTime? ScheduledDate
		{
			get;
			set;
		}

		public EditFillSequenceFormulationFinishDateResult()
		{
		}
	}
}