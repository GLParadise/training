using System;
using System.Runtime.CompilerServices;

namespace Web.Api.Controllers.Models
{
	public class FillSequenceRequest : BaseRequest
	{
		public decimal? batch_size
		{
			get;
			set;
		}

		public string batch_size_uom
		{
			get;
			set;
		}

		public int changeover_time_days
		{
			get;
			set;
		}

		public int changeover_time_hours
		{
			get;
			set;
		}

		public Guid? confirm_e_sig_id
		{
			get;
			set;
		}

		public string container_type_code
		{
			get;
			set;
		}

		public Guid? create_esig_id
		{
			get;
			set;
		}

		public int expiration_interval
		{
			get;
			set;
		}

		public string expiration_interval_type
		{
			get;
			set;
		}

		public int? filling_cage_config_id
		{
			get;
			set;
		}

		public string is_fast_pack
		{
			get;
			set;
		}

		public bool is_formulation_batch
		{
			get;
			set;
		}

		public string is_r_d_lot
		{
			get;
			set;
		}

		public bool is_sub_lot
		{
			get;
			set;
		}

		public string lot_no
		{
			get;
			set;
		}

		public int? mf_config_id
		{
			get;
			set;
		}

		public string mf_number
		{
			get;
			set;
		}

		public int? mold_type_id
		{
			get;
			set;
		}

		public string notes
		{
			get;
			set;
		}

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

		public DateTime scheduled_date
		{
			get;
			set;
		}

		public bool? should_copy_mf_items
		{
			get;
			set;
		}

		public int? stack_height
		{
			get;
			set;
		}

		public int? vials_per_card
		{
			get;
			set;
		}

		public FillSequenceRequest()
		{
		}
	}
}