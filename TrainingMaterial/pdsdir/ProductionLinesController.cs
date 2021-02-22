using Streamline.Shared.Infrastructure.WebApi;
using Streamline.Shared.LoggingV2;
using Streamline.Web.Data.Stinv;
using System;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Web.Api.Controllers.Models;

namespace Web.Api.Controllers
{
	[RoutePrefix("api/v1/production-lines")]
	[ValidateSession(Ability="Production Scheduling - View Fill Schedules")]
	public class ProductionLinesController : BaseApiController
	{
		private readonly ILogger _logger;

		private readonly StinvEntities _stinvEntities;

		public ProductionLinesController(ILogger logger)
		{
			this._logger = logger;
			this._stinvEntities = new StinvEntities();
		}

		[Route("{prodLineNo}/container-types")]
		public IHttpActionResult GetContainerTypes(string prodLineNo)
		{
			return this.Ok<List<fn_PDS_get_container_typesOutput>>(this._stinvEntities.fn_PDS_get_container_types(prodLineNo));
		}

		[Route("")]
		public IHttpActionResult GetProductionLines([ModelBinder] ProductionLinesRequest request)
		{
			bool? nullable;
			int? productId;
			bool? onlyFormulation;
			bool? onlyFilling;
			StinvEntities stinvEntity = this._stinvEntities;
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
				onlyFormulation = request.OnlyFormulation;
			}
			else
			{
				nullable = null;
				onlyFormulation = nullable;
			}
			if (request != null)
			{
				onlyFilling = request.OnlyFilling;
			}
			else
			{
				nullable = null;
				onlyFilling = nullable;
			}
			return this.Ok<List<fn_PDS_ADM_ProductionLine_M_get_linesOutput>>(stinvEntity.fn_PDS_ADM_ProductionLine_M_get_lines(productId, onlyFormulation, onlyFilling));
		}
	}
}