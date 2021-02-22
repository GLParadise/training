using Streamline.Shared.Infrastructure.WebApi;
using Streamline.Shared.LoggingV2;
using Streamline.Web.Data.Stinv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Web.Api.Controllers.Models;

namespace Web.Api.Controllers
{
	[RoutePrefix("api/v1/pack-items")]
	[ValidateSession(Ability="Production Scheduling - View Fill Schedules")]
	public class PackItemsController : BaseApiController
	{
		private readonly ILogger _logger;

		private readonly StinvEntities _stinvEntities;

		public PackItemsController(ILogger logger, StinvEntities stinvEntities)
		{
			this._logger = logger;
			this._stinvEntities = stinvEntities;
		}

		[HttpGet]
		[Route("filter")]
		public IHttpActionResult FilterPackItems([ModelBinder] FilterPackItemsRequest request)
		{
			string prodLineNo;
			int? productId;
			string ndc;
			StinvEntities stinvEntity = this._stinvEntities;
			if (request != null)
			{
				prodLineNo = request.ProdLineNo;
			}
			else
			{
				prodLineNo = null;
			}
			if (request != null)
			{
				productId = request.ProductId;
			}
			else
			{
				productId = null;
			}
			if (request != null)
			{
				ndc = request.Ndc;
			}
			else
			{
				ndc = null;
			}
			return this.Ok<List<fn_PDS_WIP_PackItem_M_filterOutput>>(stinvEntity.fn_PDS_WIP_PackItem_M_filter(prodLineNo, productId, ndc));
		}

		[Route("")]
		public IHttpActionResult GetPackItems([ModelBinder] PackItemsRequest request)
		{
			List<fn_PDS_WIP_PackItem_M_getOutput> result = this._stinvEntities.fn_PDS_WIP_PackItem_M_get(new int?(request.ProductId), request.OnlyProduction);
			return this.Ok<List<fn_PDS_WIP_PackItem_M_getOutput>>(result);
		}

		[HttpPost]
		[Route("moldtype-vialspercard-stackheight")]
		public IHttpActionResult PostGetMoldTypeVialsPerCardStackHeight(MoldTypeVialsPerCardStackHeightRequest request)
		{
			fn_PDS_WIP_PackItem_M_get_moldtype_vialspercard_stackheightOutput result = this._stinvEntities.fn_PDS_WIP_PackItem_M_get_moldtype_vialspercard_stackheight(new int?(request.pack_item_id), request.prod_line_no).SingleOrDefault<fn_PDS_WIP_PackItem_M_get_moldtype_vialspercard_stackheightOutput>();
			return this.Ok<fn_PDS_WIP_PackItem_M_get_moldtype_vialspercard_stackheightOutput>(result);
		}
	}
}