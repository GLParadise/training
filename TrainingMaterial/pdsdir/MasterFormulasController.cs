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
	[RoutePrefix("api/v1/master-formulas")]
	[ValidateSession(Ability="Production Scheduling - View Fill Schedules")]
	public class MasterFormulasController : ApiController
	{
		private readonly ILogger _logger;

		private readonly StinvEntities _stinvEntities;

		public MasterFormulasController(ILogger logger, StinvEntities stinvEntities)
		{
			this._logger = logger;
			this._stinvEntities = stinvEntities;
		}

		[Route("batch-sizes")]
		public IHttpActionResult GetBatchSizes([ModelBinder] BatchSizeRequest request)
		{
			List<fn_PDS_get_batch_sizes_for_MFOutput> result = this._stinvEntities.fn_PDS_get_batch_sizes_for_MF(new int?(request.MfId), request.OnlyProduction);
			return this.Ok<List<fn_PDS_get_batch_sizes_for_MFOutput>>(result);
		}

		[Route("")]
		public IHttpActionResult GetMasterFormulasByProduct([ModelBinder] MasterFormulaRequest request)
		{
			StinvEntities stinvEntity = this._stinvEntities;
			int? nullable = new int?(request.ProductId);
			Guid? fillId = request.fill_id;
			List<fn_PDS_get_MF_by_productOutput> result = stinvEntity.fn_PDS_get_MF_by_product(nullable, new Guid?(fillId.GetValueOrDefault()), request.FillDate, request.MfConfigId, request.BatchSize, request.ScheduledFillId, request.OnlyProduction);
			return this.Ok<List<fn_PDS_get_MF_by_productOutput>>(result);
		}
	}
}