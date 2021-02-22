using Streamline.Shared.Infrastructure.WebApi;
using Streamline.Shared.LoggingV2;
using Streamline.Web.Data.Stinv;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Web.Api.Controllers.Models;

namespace Web.Api.Controllers
{
	[RoutePrefix("api/v1/products")]
	[ValidateSession(Ability="Production Scheduling - View Fill Schedules")]
	public class ProductsController : BaseApiController
	{
		private readonly ILogger _logger;

		private readonly StinvEntities _stinvEntities;

		public ProductsController(ILogger logger, StinvEntities stinvEntities)
		{
			this._logger = logger;
			this._stinvEntities = stinvEntities;
		}

		[HttpGet]
		[Route("filter")]
		public IHttpActionResult FilterProducts([ModelBinder] FilterProductsRequest request)
		{
			string prodLineNo;
			string prodDesc;
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
				prodDesc = request.ProdDesc;
			}
			else
			{
				prodDesc = null;
			}
			return this.Ok<List<fn_PDS_WIP_Product_M_filterOutput>>(stinvEntity.fn_PDS_WIP_Product_M_filter(prodLineNo, prodDesc));
		}

		[Route("")]
		public IHttpActionResult GetProducts([ModelBinder] ProductsRequest request)
		{
			List<fn_PDS_WIP_Product_M_getOutput> result = this._stinvEntities.fn_PDS_WIP_Product_M_get(request.ProdLineNo, request.ProductId, request.MfId, request.OnlyProduction);
			return this.Ok<List<fn_PDS_WIP_Product_M_getOutput>>(result);
		}
	}
}