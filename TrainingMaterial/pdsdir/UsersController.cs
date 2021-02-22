using Streamline.Shared.Infrastructure.WebApi;
using Streamline.Shared.LoggingV2;
using Streamline.Web.Data.Stinv;
using Streamline.Web.Data.Users;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Web.Api.Controllers
{
	[RoutePrefix("api/v1/users")]
	[ValidateSession(Ability="Production Scheduling - View Fill Schedules")]
	public class UsersController : BaseApiController
	{
		private readonly ILogger _logger;

		private readonly StinvEntities _stinvEntities;

		private readonly UsersEntities _usersEntities;

		public UsersController(ILogger logger, StinvEntities stinvEntities, UsersEntities usersEntities)
		{
			this._logger = logger;
			this._stinvEntities = stinvEntities;
			this._usersEntities = usersEntities;
		}

		[Route("{userGuid}/abilites")]
		public IHttpActionResult GetUsersAbilities(Guid userGuid)
		{
			this._logger.LogDebug("UsersController.GetUsersAbilities", new object[] { userGuid });
			List<fn_PDS_get_user_abilitiesOutput> results = this._usersEntities.fn_PDS_get_user_abilities(new Guid?(userGuid));
			this._logger.LogDebug("UsersController.GetUsersAbilities", new object[] { userGuid });
			return this.Ok<List<fn_PDS_get_user_abilitiesOutput>>(results);
		}
	}
}