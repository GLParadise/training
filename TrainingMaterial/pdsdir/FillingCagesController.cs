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
	[RoutePrefix("api/v1/filling-cages")]
	[ValidateSession(Ability="Production Scheduling - View Fill Schedules")]
	public class FillingCagesController : ApiController
	{
		private readonly ILogger _logger;

		private readonly StinvEntities _stinvEntities;

		public FillingCagesController(ILogger logger, StinvEntities stinvEntities)
		{
			this._logger = logger;
			this._stinvEntities = stinvEntities;
		}

		[Route("")]
		public IHttpActionResult Get([ModelBinder] FillingCagesRequest request)
		{
			List<fn_PDS_get_filling_cagesOutput> result = this._stinvEntities.fn_PDS_get_filling_cages(request.ProdLineNo, new int?(request.ProductId), request.OnlyProduction, request.CanChangeCage, request.FillId, request.FillDate, request.FillingCageConfigId, request.BatchSize, request.BatchSizeUomCode);
			return this.Ok<List<fn_PDS_get_filling_cagesOutput>>(result);
		}
	}
}