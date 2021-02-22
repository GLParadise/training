using Streamline.Shared.LoggingV2;
using Streamline.Web.Data.Stinv;
using Streamline.Web.Data.Users;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Web.Api.Controllers.Models;

namespace Web.Api.Controllers
{
	[RoutePrefix("api/v1/params")]
	public class ParametersController : ApiController
	{
		private readonly ILogger _logger;

		private readonly UsersEntities _usersEntities;

		private readonly StinvEntities _stinvEntities;

		public ParametersController(ILogger logger, UsersEntities usersEntities, StinvEntities stinvEntities)
		{
			this._logger = logger;
			this._usersEntities = usersEntities;
			this._stinvEntities = stinvEntities;
		}

		[HttpGet]
		[Route("global")]
		public IHttpActionResult GetGlobalParameter([ModelBinder] GlobalParameterRequest request)
		{
			string name;
			int? locationId;
			UsersEntities usersEntity = this._usersEntities;
			if (request != null)
			{
				name = request.Name;
			}
			else
			{
				name = null;
			}
			if (request != null)
			{
				locationId = request.LocationId;
			}
			else
			{
				locationId = null;
			}
			return this.Ok<string>(usersEntity.sfn_SYS_get_global_parameter(name, locationId));
		}

		[Route("app")]
		public IHttpActionResult PostGetApplicationParameter(ApplicationParameterRequest request)
		{
			this._logger.LogDebug("AppParametersController.PostGetApplicationParameter");
			fn_SYS_get_application_parameterOutput result = this._usersEntities.fn_SYS_get_application_parameter(request.application_name, request.p_name, request.location_id).FirstOrDefault<fn_SYS_get_application_parameterOutput>();
			this._logger.LogDebug("AppParametersController.PostGetApplicationParameter", new object[] { result });
			return this.Ok<fn_SYS_get_application_parameterOutput>(result);
		}
	}
}