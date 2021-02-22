using Streamline;
using Streamline.Shared.Infrastructure.WebApi;
using Streamline.Shared.LoggingV2;
using Streamline.Shared.Utilities;
using Streamline.Web.Data.Stinv;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Web.Api.Controllers.Extensions;
using Web.Api.Controllers.Models;

namespace Web.Api.Controllers
{
	[RoutePrefix("api/v1/production-schedules")]
	[ValidateSession(Ability="Production Scheduling - View Fill Schedules")]
	public class ProductionSchedulesController : BaseApiController
	{
		private readonly ILogger _logger;

		private readonly StinvEntities _stinvEntities;

		public ProductionSchedulesController(ILogger logger)
		{
			this._logger = logger;
			this._stinvEntities = new StinvEntities();
		}

		private static int CalculateNumberOfDaysForChangeOver(int hours)
		{
			return hours / 24;
		}

		private static int CalculateNumberOfHoursForChangeOver(int hours)
		{
			return hours - ProductionSchedulesController.CalculateNumberOfDaysForChangeOver(hours) * 24;
		}

		private static DateTime? ClearTime(DateTime? date)
		{
			if (!date.HasValue)
			{
				return null;
			}
			return new DateTime?(ProductionSchedulesController.ClearTime(date.Value.Date));
		}

		private static DateTime ClearTime(DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Local);
		}

		private fn_WIP_ContainerType_M_getOutput GetContainerType(string containerTypeCode)
		{
			return this._stinvEntities.fn_WIP_ContainerType_M_get(containerTypeCode).FirstOrDefault<fn_WIP_ContainerType_M_getOutput>();
		}

		[HttpGet]
		[Route("get-default")]
		public IHttpActionResult GetDefaultFillSequence([ModelBinder] DefaultFillSequenceRequest request)
		{
			string prodLineNo;
			StinvEntities stinvEntity = this._stinvEntities;
			if (request != null)
			{
				prodLineNo = request.ProdLineNo;
			}
			else
			{
				prodLineNo = null;
			}
			fn_PDS_WIP_ProductionFillSequence_M_get_defaultOutput result = stinvEntity.fn_PDS_WIP_ProductionFillSequence_M_get_default(prodLineNo).FirstOrDefault<fn_PDS_WIP_ProductionFillSequence_M_get_defaultOutput>();
			return this.Ok<fn_PDS_WIP_ProductionFillSequence_M_get_defaultOutput>(result);
		}

		private fn_CAG_ConfigFilling_M_getOutput GetFillingCage(int? fillingCageConfigId)
		{
			return this._stinvEntities.fn_CAG_ConfigFilling_M_get(fillingCageConfigId).FirstOrDefault<fn_CAG_ConfigFilling_M_getOutput>();
		}

		private fn_WIP_ProductionFillSequence_M_getOutput GetFillSequence(Guid? fillId)
		{
			return this._stinvEntities.fn_WIP_ProductionFillSequence_M_get(fillId).FirstOrDefault<fn_WIP_ProductionFillSequence_M_getOutput>();
		}

		[Route("get-details")]
		public IHttpActionResult GetFillSequenceDetails([ModelBinder] FillSequenceDetailsRequest request)
		{
			fn_PDS_WIP_ProductionFillSequence_M_get_detailsOutput result = this._stinvEntities.fn_PDS_WIP_ProductionFillSequence_M_get_details(request.FillId, request.IsCreateSubLot).SingleOrDefault<fn_PDS_WIP_ProductionFillSequence_M_get_detailsOutput>();
			return this.Ok<fn_PDS_WIP_ProductionFillSequence_M_get_detailsOutput>(result);
		}

		private fn_ADM_MoldType_M_getOutput GetMoldType(int moldTypeId)
		{
			return this._stinvEntities.fn_ADM_MoldType_M_get(new int?(moldTypeId)).FirstOrDefault<fn_ADM_MoldType_M_getOutput>();
		}

		private fn_WIP_PackItem_M_getOutput GetPackItem(int packItemId)
		{
			return this._stinvEntities.fn_WIP_PackItem_M_get(new int?(packItemId)).FirstOrDefault<fn_WIP_PackItem_M_getOutput>();
		}

		private fn_WIP_Product_M_getOutput GetProduct(int productId)
		{
			return this._stinvEntities.fn_WIP_Product_M_get(new int?(productId)).FirstOrDefault<fn_WIP_Product_M_getOutput>();
		}

		[Route("statuses")]
		public IHttpActionResult GetScheduleStatuses()
		{
			return this.Ok<List<fn_PDS_get_schedule_statusesOutput>>(this._stinvEntities.fn_PDS_get_schedule_statuses());
		}

		private IEnumerable<string> GetSubLots(Guid? fillId)
		{
			if (!fillId.HasValue)
			{
				return new string[0];
			}
			return 
				from fs in this._stinvEntities.fn_PDS_get_sub_lots(fillId)
				select fs.lot_no;
		}

		[Route("confirm")]
		[ValidateSession(Ability="Production Scheduling - Confirm Fill Schedule")]
		public IHttpActionResult PostConfirmFillSequence(ConfirmationRequest request)
		{
			IHttpActionResult httpActionResult;
			IEnumerable<string> strs;
			string[] array;
			this._logger.LogDebug("ProductionSchedulesController.PostConfirmFillSequence", new object[] { request });
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				fn_WIP_ProductionFillSequence_M_getOutput fillSequence = this.GetFillSequence(request.fill_id);
				if (fillSequence != null)
				{
					if (fillSequence.is_batch.GetValueOrDefault())
					{
						strs = this.GetSubLots(request.fill_id);
					}
					else
					{
						strs = null;
					}
					IEnumerable<string> subLots = strs;
					string str = AuditTrailClient.Translate("Sublot");
					string str1 = AuditTrailClient.Translate("updated");
					string sessionToken = base.SessionToken;
					string applicationName = base.ApplicationName;
					string applicationVersion = base.ApplicationVersion;
					string requestClientIpAddress = base.RequestClientIpAddress;
					string lotNo = fillSequence.lot_no;
					Guid? nullable = new Guid?(request.esig_id);
					AuditTextCollection auditTextCollections = new AuditTextCollection()
					{
						{ "Production Line Number", fillSequence.prod_line_no },
						{ "Lot Number", fillSequence.lot_no },
						{ "Master Formulation", fillSequence.mf_number }
					};
					if (subLots != null)
					{
						array = (
							from sl in subLots
							select string.Concat(new string[] { str, " ", sl, " ", str1, "." })).ToArray<string>();
					}
					else
					{
						array = null;
					}
					AuditTrailClient.Event("FG-PDS-CON", sessionToken, applicationName, applicationVersion, requestClientIpAddress, lotNo, null, nullable, auditTextCollections, array);
					this._stinvEntities.proc_PDS_confirm_schedule(request.fill_id, new int?(base.UserId), new Guid?(request.esig_id));
					transaction.Complete();
					httpActionResult = this.Ok<FillSequenceResult>(new FillSequenceResult()
					{
						FillId = request.fill_id,
						ScheduledDate = fillSequence.scheduled_date,
						ProdLineNo = fillSequence.prod_line_no,
						LotNo = fillSequence.lot_no
					});
				}
				else
				{
					this._logger.LogError("Production schedule was not found.");
					transaction.Dispose();
					httpActionResult = this.BadRequest("Production schedule was not found.");
				}
			}
			return httpActionResult;
		}

		[Route("")]
		[ValidateSession(Ability="Production Scheduling - Create Fill Schedule")]
		public IHttpActionResult PostCreateFillSequence(FillSequenceRequest request)
		{
			Guid? fillId;
			string lotNo;
			IHttpActionResult httpActionResult;
			this._logger.LogDebug("ProductionSchedulesController.PostCreateFillSequence", new object[] { request });
			request.scheduled_date = ProductionSchedulesController.ClearTime(request.scheduled_date);
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				this._stinvEntities.proc_PDS_WIP_ProductionFillSequence_M_create(request.prod_line_no, new DateTime?(request.scheduled_date), new int?(request.pack_item_id), new int?(request.expiration_interval), request.expiration_interval_type, request.vials_per_card, request.stack_height, request.batch_size, request.batch_size_uom, new int?(request.changeover_time_days), new int?(request.changeover_time_hours), request.notes, request.container_type_code, new int?(base.UserId), request.mold_type_id, request.is_r_d_lot, request.is_fast_pack, new bool?(request.is_formulation_batch), request.mf_config_id, request.filling_cage_config_id, out lotNo, out fillId);
				fn_WIP_PackItem_M_getOutput packItem = this.GetPackItem(request.pack_item_id);
				if (packItem != null)
				{
					string sessionToken = base.SessionToken;
					string applicationName = base.ApplicationName;
					string applicationVersion = base.ApplicationVersion;
					string requestClientIpAddress = base.RequestClientIpAddress;
					Guid? nullable = null;
					AuditTrailClient.Event("FG-PDS-C", sessionToken, applicationName, applicationVersion, requestClientIpAddress, lotNo, null, nullable, new AuditTextCollection()
					{
						{ "Production Line Number", request.prod_line_no },
						{ "Lot Number", lotNo },
						{ "Product NDC", packItem.NDC_Code }
					}, null);
					transaction.Complete();
					this._logger.LogDebug("ProductionSchedulesController.PostCreateFillSequence", new object[] { fillId, lotNo });
					return this.Ok<FillSequenceResult>(new FillSequenceResult()
					{
						FillId = fillId,
						ScheduledDate = new DateTime?(request.scheduled_date),
						ProdLineNo = request.prod_line_no,
						LotNo = lotNo
					});
				}
				else
				{
					this._logger.LogError("Pack item was not found.");
					transaction.Dispose();
					httpActionResult = this.BadRequest("Pack item was not found.");
				}
			}
			return httpActionResult;
		}

		[Route("sub-lot")]
		[ValidateSession(Ability="Production Scheduling - Create Fill Schedule")]
		public IHttpActionResult PostCreateSubLotFillSequence(FillSequenceSubLotRequest request)
		{
			Guid? fillId;
			string subLotNo;
			IHttpActionResult httpActionResult;
			this._logger.LogDebug("ProductionSchedulesController.PostCreateSubLotFillSequence", new object[] { request });
			request.scheduled_date = ProductionSchedulesController.ClearTime(request.scheduled_date);
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				StinvEntities stinvEntity = this._stinvEntities;
				DateTime? nullable = new DateTime?(request.scheduled_date);
				string prodLineNo = request.prod_line_no;
				int? nullable1 = new int?(request.pack_item_id);
				int? stackHeight = request.stack_height;
				int? moldTypeId = request.mold_type_id;
				decimal? batchSize = request.batch_size;
				string batchSizeUom = request.batch_size_uom;
				int? nullable2 = new int?(request.changeover_time_days);
				int? nullable3 = new int?(request.changeover_time_hours);
				string str = request.notes;
				int? nullable4 = new int?(base.UserId);
				Guid? parentFillId = request.parent_fill_id;
				stinvEntity.proc_PDS_create_sub_lot(nullable, prodLineNo, nullable1, stackHeight, moldTypeId, batchSize, batchSizeUom, nullable2, nullable3, str, nullable4, new Guid?(parentFillId.GetValueOrDefault()), request.filling_cage_config_id, request.container_type_code, new int?(request.expiration_interval), request.expiration_interval_type, out fillId, out subLotNo);
				fn_WIP_PackItem_M_getOutput packItem = this.GetPackItem(request.pack_item_id);
				if (packItem != null)
				{
					AuditTrailClient.Event("FG-PDS-C", base.SessionToken, base.ApplicationName, base.ApplicationVersion, base.RequestClientIpAddress, subLotNo, null, request.confirm_e_sig_id, new AuditTextCollection()
					{
						{ "Production Line Number", request.prod_line_no },
						{ "Lot Number", subLotNo },
						{ "Product NDC", packItem.NDC_Code }
					}, null);
					transaction.Complete();
					this._logger.LogDebug("ProductionSchedulesController.PostCreateSubLotFillSequence", new object[] { fillId, subLotNo });
					return this.Ok<FillSequenceResult>(new FillSequenceResult()
					{
						FillId = fillId,
						ScheduledDate = new DateTime?(request.scheduled_date),
						ProdLineNo = request.prod_line_no,
						LotNo = subLotNo
					});
				}
				else
				{
					string errorMsg = "Pack item was not found";
					this._logger.LogError(errorMsg);
					transaction.Dispose();
					httpActionResult = this.BadRequest(errorMsg);
				}
			}
			return httpActionResult;
		}

		[Route("delete")]
		[ValidateSession(Ability="Production Scheduling - Delete Fill Schedule")]
		public IHttpActionResult PostDeleteFillSequence(DeleteRequest request)
		{
			string errorMsg;
			IHttpActionResult httpActionResult;
			string str;
			string cageName;
			this._logger.LogDebug("ProductionSchedulesController.PostDeleteFillSequence", new object[] { request });
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				fn_WIP_ProductionFillSequence_M_getOutput fillSequence = this.GetFillSequence(request.fill_id);
				if (fillSequence != null)
				{
					int? packItemId = fillSequence.pack_item_id;
					fn_WIP_PackItem_M_getOutput packItem = this.GetPackItem(packItemId.GetValueOrDefault());
					if (packItem != null)
					{
						fn_WIP_ContainerType_M_getOutput containerType = this.GetContainerType(fillSequence.container_type_code);
						packItemId = fillSequence.mold_type_id;
						fn_ADM_MoldType_M_getOutput moldType = this.GetMoldType(packItemId.GetValueOrDefault());
						packItemId = packItem.Product_id;
						fn_WIP_Product_M_getOutput product = this.GetProduct(packItemId.GetValueOrDefault());
						fn_CAG_ConfigFilling_M_getOutput fillingCage = this.GetFillingCage(fillSequence.filling_cage_config_id);
						if (containerType == null)
						{
							errorMsg = "Container type was not found.";
							this._logger.LogError(errorMsg);
							transaction.Dispose();
							httpActionResult = this.BadRequest(errorMsg);
						}
						else if (moldType == null)
						{
							errorMsg = "Mold type was not found.";
							this._logger.LogError(errorMsg);
							transaction.Dispose();
							httpActionResult = this.BadRequest(errorMsg);
						}
						else if (product != null)
						{
							string isRdLot = (fillSequence.is_r_d_lot == "Y" ? "Yes" : "No");
							packItemId = fillSequence.changeover_time;
							int changeOverHours = ProductionSchedulesController.CalculateNumberOfHoursForChangeOver(packItemId.GetValueOrDefault());
							packItemId = fillSequence.changeover_time;
							int changeOverDays = ProductionSchedulesController.CalculateNumberOfDaysForChangeOver(packItemId.GetValueOrDefault());
							string expText = (fillSequence.expiration_interval_type == "day" ? "Number of Days to Expiration" : "Number of Months to Expiration");
							string sessionToken = base.SessionToken;
							string applicationName = base.ApplicationName;
							string applicationVersion = base.ApplicationVersion;
							string requestClientIpAddress = base.RequestClientIpAddress;
							string lotNo = fillSequence.lot_no;
							Guid? nullable = new Guid?(request.esig_id);
							AuditTextCollection auditTextCollections = new AuditTextCollection()
							{
								{ "Production Line Number", fillSequence.prod_line_no },
								{ "Lot Number", fillSequence.lot_no },
								{ "Product NDC", packItem.NDC_Code }
							};
							DeletePropertyCollection deletePropertyCollections = new DeletePropertyCollection();
							DateTime valueOrDefault = fillSequence.scheduled_date.GetValueOrDefault();
							deletePropertyCollections.Add("Scheduled Production Date", valueOrDefault.ToString(CultureInfo.InvariantCulture));
							deletePropertyCollections.Add("R & D Lot?", isRdLot);
							deletePropertyCollections.Add("Product Type", product.product_desc);
							string str1 = expText;
							packItemId = fillSequence.expiration_interval;
							if (packItemId.HasValue)
							{
								str = packItemId.GetValueOrDefault().ToString();
							}
							else
							{
								str = null;
							}
							deletePropertyCollections.Add(str1, str);
							deletePropertyCollections.Add("Container Type", containerType.container_type_desc);
							deletePropertyCollections.Add("Mold Type", moldType.mold_type_desc);
							packItemId = fillSequence.vials_per_card;
							deletePropertyCollections.Add("Vials Per Card", packItemId.GetValueOrDefault());
							packItemId = fillSequence.stack_height;
							deletePropertyCollections.Add("Stack Height", packItemId.GetValueOrDefault());
							deletePropertyCollections.Add("Batch Size", string.Format("{0} {1}", fillSequence.batch_size, fillSequence.batch_size_UOM));
							deletePropertyCollections.Add("Change Over Hours", changeOverHours);
							deletePropertyCollections.Add("Change Over Days", changeOverDays);
							deletePropertyCollections.Add("Notes", fillSequence.notes);
							deletePropertyCollections.Add("Is Formulation Batch", (fillSequence.is_batch.GetValueOrDefault() ? "Yes" : "No"));
							if (fillingCage != null)
							{
								cageName = fillingCage.cage_name;
							}
							else
							{
								cageName = null;
							}
							deletePropertyCollections.Add("Filling Component Assembly", cageName);
							AuditTrailClient.Delete("FG-PDS-D", sessionToken, applicationName, applicationVersion, requestClientIpAddress, lotNo, null, nullable, auditTextCollections, deletePropertyCollections, null);
							this._stinvEntities.proc_PDS_WIP_ProductionFillSequence_M_delete(request.fill_id, new int?(base.UserId));
							transaction.Complete();
							httpActionResult = this.Ok<FillSequenceResult>(new FillSequenceResult()
							{
								FillId = request.fill_id,
								ScheduledDate = fillSequence.scheduled_date,
								ProdLineNo = fillSequence.prod_line_no,
								LotNo = fillSequence.lot_no
							});
						}
						else
						{
							errorMsg = "Product was not found.";
							this._logger.LogError(errorMsg);
							transaction.Dispose();
							httpActionResult = this.BadRequest(errorMsg);
						}
					}
					else
					{
						errorMsg = "Pack item was not found";
						this._logger.LogError(errorMsg);
						transaction.Dispose();
						httpActionResult = this.BadRequest(errorMsg);
					}
				}
				else
				{
					errorMsg = "Production schedule was not found.";
					this._logger.LogError(errorMsg);
					transaction.Dispose();
					httpActionResult = this.BadRequest(errorMsg);
				}
			}
			return httpActionResult;
		}

		[Route("try-update-fill-date-from-ebr")]
		[ValidateSession]
		public IHttpActionResult PostTryUpdateFillDateFromEbr(ProductionSchedulesController.TryUpdateFillDateFromEbrRequest request)
		{
			DateTime? formulation_finish_date;
			bool? is_fill_date_from_EBR_bit;
			bool? is_changed;
			this._logger.LogDebug("ProductionSchedulesController.PostTryUpdateFillDateFromEbr", new object[] { request });
			this._stinvEntities.proc_PDS_try_update_fill_date_from_EBR(new Guid?(request.fill_id), out formulation_finish_date, out is_fill_date_from_EBR_bit, out is_changed);
			return this.Ok<ProductionSchedulesController.TryUpdateFillDateFromEbrResult>(new ProductionSchedulesController.TryUpdateFillDateFromEbrResult()
			{
				fill_id = request.fill_id,
				formulation_finish_date = formulation_finish_date,
				is_fill_date_from_EBR_bit = is_fill_date_from_EBR_bit,
				is_changed = is_changed
			});
		}

		[Route("unconfirm")]
		[ValidateSession(Ability="Production Scheduling - Unconfirm Fill Schedule")]
		public IHttpActionResult PostUnconfirmFillSequence(ConfirmationRequest request)
		{
			IHttpActionResult httpActionResult;
			this._logger.LogDebug("ProductionSchedulesController.PostUnconfirmFillSequence", new object[] { request });
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				fn_WIP_ProductionFillSequence_M_getOutput fillSequence = this.GetFillSequence(request.fill_id);
				if (fillSequence != null)
				{
					AuditTrailClient.Event("FG-PDS-UCON", base.SessionToken, base.ApplicationName, base.ApplicationVersion, base.RequestClientIpAddress, fillSequence.lot_no, null, new Guid?(request.esig_id), new AuditTextCollection()
					{
						{ "Production Line Number", fillSequence.prod_line_no },
						{ "Lot Number", fillSequence.lot_no },
						{ "Master Formulation", fillSequence.mf_number }
					}, null);
					this._stinvEntities.proc_PDS_unconfirm_schedule(request.fill_id, new int?(base.UserId), new Guid?(request.esig_id));
					transaction.Complete();
					httpActionResult = this.Ok<FillSequenceResult>(new FillSequenceResult()
					{
						FillId = request.fill_id,
						ScheduledDate = fillSequence.scheduled_date,
						ProdLineNo = fillSequence.prod_line_no,
						LotNo = fillSequence.lot_no
					});
				}
				else
				{
					this._logger.LogError("Production schedule was not found.");
					transaction.Dispose();
					httpActionResult = this.BadRequest();
				}
			}
			return httpActionResult;
		}

		[Route("unfinish")]
		[ValidateSession(Ability="Production Scheduling - Unfinish Fill Schedule")]
		public IHttpActionResult PostUnfinishFillSequence(ConfirmationRequest request)
		{
			IHttpActionResult httpActionResult;
			this._logger.LogDebug("ProductionSchedulesController.PostUnfinishFillSequence", new object[] { request });
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				fn_WIP_ProductionFillSequence_M_getOutput fillSequence = this.GetFillSequence(request.fill_id);
				if (fillSequence != null)
				{
					IEnumerable<string> subLots = this.GetSubLots(request.fill_id);
					string str = AuditTrailClient.Translate("Sub Lot");
					string str1 = AuditTrailClient.Translate("moved back to pending");
					AuditTrailClient.Event("FG-PDS-UFIN", base.SessionToken, base.ApplicationName, base.ApplicationVersion, base.RequestClientIpAddress, fillSequence.lot_no, null, new Guid?(request.esig_id), new AuditTextCollection()
					{
						{ "Production Line Number", fillSequence.prod_line_no },
						{ "Lot Number", fillSequence.lot_no },
						{ "Master Formulation", fillSequence.mf_number }
					}, (
						from sl in subLots
						select string.Concat(new string[] { str, " ", sl, " ", str1, "." })).ToArray<string>());
					this._stinvEntities.proc_PDS_unfinish_schedule(request.fill_id, new int?(base.UserId), new Guid?(request.esig_id));
					transaction.Complete();
					httpActionResult = this.Ok<FillSequenceResult>(new FillSequenceResult()
					{
						FillId = request.fill_id,
						ScheduledDate = fillSequence.scheduled_date,
						ProdLineNo = fillSequence.prod_line_no,
						LotNo = fillSequence.lot_no
					});
				}
				else
				{
					this._logger.LogError("Production schedule was not found.");
					transaction.Dispose();
					httpActionResult = this.BadRequest();
				}
			}
			return httpActionResult;
		}

		[HttpPut]
		[Route("update-after-production/{id}")]
		[ValidateSession(Ability="Production Scheduling - Edit Fill Schedule")]
		public IHttpActionResult PutUpdateAfterProduction(Guid id, UpdateAfterProductionRequest request)
		{
			string errorMsg;
			IHttpActionResult httpActionResult;
			FillSequenceResult result = new FillSequenceResult();
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				fn_WIP_ProductionFillSequence_M_getOutput beforeFillSequence = this.GetFillSequence(new Guid?(id));
				if (beforeFillSequence != null)
				{
					fn_WIP_PackItem_M_getOutput beforePackItem = this.GetPackItem(beforeFillSequence.pack_item_id.GetValueOrDefault());
					if (beforePackItem != null)
					{
						string sessionToken = base.SessionToken;
						string applicationName = base.ApplicationName;
						string applicationVersion = base.ApplicationVersion;
						string requestClientIpAddress = base.RequestClientIpAddress;
						string lotNo = beforeFillSequence.lot_no;
						Guid? nullable = null;
						AuditTextCollection auditTextCollections = new AuditTextCollection()
						{
							{ "Production Line Number", beforeFillSequence.prod_line_no },
							{ "Lot Number", beforeFillSequence.lot_no },
							{ "Product NDC", beforePackItem.NDC_Code }
						};
						UpdatePropertyCollection updatePropertyCollections = new UpdatePropertyCollection();
						DateTime valueOrDefault = beforeFillSequence.scheduled_date.GetValueOrDefault();
						string str = valueOrDefault.ToString(CultureInfo.InvariantCulture);
						valueOrDefault = request.ScheduledDate.GetValueOrDefault();
						updatePropertyCollections.Add("Scheduled Date", str, valueOrDefault.ToString(CultureInfo.InvariantCulture));
						updatePropertyCollections.Add("Production Line Number", beforeFillSequence.prod_line_no, request.ProdLineNo);
						updatePropertyCollections.Add("Notes", beforeFillSequence.notes, request.Notes);
						AuditTrailClient.Update("FG-PDS-U", sessionToken, applicationName, applicationVersion, requestClientIpAddress, lotNo, null, nullable, auditTextCollections, updatePropertyCollections, null);
						this._stinvEntities.proc_PDS_WIP_ProductionFillSequence_M_update_after_production(new Guid?(id), request.ScheduledDate, request.ProdLineNo, request.Notes, new int?(base.UserId));
						transaction.Complete();
						result.FillId = new Guid?(id);
						result.ScheduledDate = request.ScheduledDate;
						result.ProdLineNo = beforeFillSequence.prod_line_no;
						result.LotNo = beforeFillSequence.lot_no;
						return this.Ok<FillSequenceResult>(result);
					}
					else
					{
						errorMsg = "Before: Pack item was not found.";
						this._logger.LogError(errorMsg);
						transaction.Dispose();
						httpActionResult = this.BadRequest(errorMsg);
					}
				}
				else
				{
					errorMsg = "Production schedule was not found.";
					this._logger.LogError(errorMsg);
					transaction.Dispose();
					httpActionResult = this.BadRequest(errorMsg);
				}
			}
			return httpActionResult;
		}

		[Route("")]
		[ValidateSession(Ability="Production Scheduling - Edit Fill Schedule")]
		public IHttpActionResult PutUpdateFillSequence(FillSequenceRequest request)
		{
			string errorMsg;
			IHttpActionResult httpActionResult;
			string str;
			string cageName;
			string cageName1;
			string[] array;
			this._logger.LogDebug("ProductionSchedulesController.PutUpdateFillSequence", new object[] { request });
			request.scheduled_date = ProductionSchedulesController.ClearTime(request.scheduled_date);
			request.is_fast_pack = (request.is_fast_pack == "Y" ? "Yes" : "No");
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				fn_WIP_ProductionFillSequence_M_getOutput beforeFillSequence = this.GetFillSequence(request.fill_id);
				if (beforeFillSequence != null)
				{
					DateTime? fillDate = beforeFillSequence.fill_date;
					beforeFillSequence.fill_date = new DateTime?(ProductionSchedulesController.ClearTime(fillDate.GetValueOrDefault()));
					beforeFillSequence.is_fast_pack = (beforeFillSequence.is_fast_pack == "Y" ? "Yes" : "No");
					int? packItemId = beforeFillSequence.pack_item_id;
					fn_WIP_PackItem_M_getOutput beforePackItem = this.GetPackItem(packItemId.GetValueOrDefault());
					if (beforePackItem != null)
					{
						fn_WIP_ContainerType_M_getOutput beforeContainerType = this.GetContainerType(beforeFillSequence.container_type_code);
						packItemId = beforeFillSequence.mold_type_id;
						fn_ADM_MoldType_M_getOutput beforeMoldType = this.GetMoldType(packItemId.GetValueOrDefault());
						packItemId = beforePackItem.Product_id;
						fn_WIP_Product_M_getOutput beforeProduct = this.GetProduct(packItemId.GetValueOrDefault());
						fn_CAG_ConfigFilling_M_getOutput beforeFillingCage = this.GetFillingCage(beforeFillSequence.filling_cage_config_id);
						if (beforeContainerType == null)
						{
							errorMsg = "Before: Container type was not found.";
							this._logger.LogError(errorMsg);
							transaction.Dispose();
							httpActionResult = this.BadRequest(errorMsg);
						}
						else if (beforeMoldType == null)
						{
							errorMsg = "Before: Mold type was not found.";
							this._logger.LogError(errorMsg);
							transaction.Dispose();
							httpActionResult = this.BadRequest(errorMsg);
						}
						else if (beforeProduct != null)
						{
							packItemId = beforeFillSequence.changeover_time;
							int beforeChangeOverHours = ProductionSchedulesController.CalculateNumberOfHoursForChangeOver(packItemId.GetValueOrDefault());
							packItemId = beforeFillSequence.changeover_time;
							int beforeChangeOverDays = ProductionSchedulesController.CalculateNumberOfDaysForChangeOver(packItemId.GetValueOrDefault());
							fn_WIP_PackItem_M_getOutput afterPackItem = this.GetPackItem(request.pack_item_id);
							if (afterPackItem != null)
							{
								fn_WIP_ContainerType_M_getOutput afterContainerType = this.GetContainerType(request.container_type_code);
								packItemId = request.mold_type_id;
								fn_ADM_MoldType_M_getOutput afterMoldType = this.GetMoldType(packItemId.GetValueOrDefault());
								packItemId = afterPackItem.Product_id;
								fn_WIP_Product_M_getOutput afterProduct = this.GetProduct(packItemId.GetValueOrDefault());
								fn_CAG_ConfigFilling_M_getOutput afterFillingCage = this.GetFillingCage(request.filling_cage_config_id);
								if (afterContainerType == null)
								{
									errorMsg = "After: Container type was not found.";
									this._logger.LogError(errorMsg);
									transaction.Dispose();
									httpActionResult = this.BadRequest(errorMsg);
								}
								else if (afterMoldType == null)
								{
									errorMsg = "After: Mold type was not found.";
									this._logger.LogError(errorMsg);
									transaction.Dispose();
									httpActionResult = this.BadRequest(errorMsg);
								}
								else if (afterProduct != null)
								{
									IEnumerable<string> subLots = this.GetSubLots(request.fill_id);
									string str1 = AuditTrailClient.Translate("Sublot");
									string str2 = AuditTrailClient.Translate("moved back to pending");
									string expText = (beforeFillSequence.expiration_interval_type == "day" ? "Number of Days to Expiration" : "Number of Months to Expiration");
									string sessionToken = base.SessionToken;
									string applicationName = base.ApplicationName;
									string applicationVersion = base.ApplicationVersion;
									string requestClientIpAddress = base.RequestClientIpAddress;
									string lotNo = request.lot_no;
									Guid? confirmESigId = request.confirm_e_sig_id;
									AuditTextCollection auditTextCollections = new AuditTextCollection()
									{
										{ "Production Line Number", request.prod_line_no },
										{ "Lot Number", request.lot_no },
										{ "Product NDC", afterPackItem.NDC_Code }
									};
									UpdatePropertyCollection updatePropertyCollections = new UpdatePropertyCollection();
									DateTime valueOrDefault = beforeFillSequence.scheduled_date.GetValueOrDefault();
									string str3 = valueOrDefault.ToString(CultureInfo.InvariantCulture);
									valueOrDefault = request.scheduled_date;
									updatePropertyCollections.Add("Scheduled Production Date", str3, valueOrDefault.ToString(CultureInfo.InvariantCulture));
									updatePropertyCollections.Add("Product NDC", beforePackItem.NDC_Code, afterPackItem.NDC_Code);
									updatePropertyCollections.Add("Product Type", beforeProduct.product_desc, afterProduct.product_desc);
									string str4 = expText;
									packItemId = beforeFillSequence.expiration_interval;
									if (packItemId.HasValue)
									{
										str = packItemId.GetValueOrDefault().ToString();
									}
									else
									{
										str = null;
									}
									updatePropertyCollections.Add(str4, str, request.expiration_interval.ToString());
									updatePropertyCollections.Add("Container Type", beforeContainerType.container_type_desc, afterContainerType.container_type_desc);
									updatePropertyCollections.Add("Mold Type", beforeMoldType.mold_type_desc, afterMoldType.mold_type_desc);
									packItemId = beforeFillSequence.vials_per_card;
									string str5 = packItemId.ToString();
									packItemId = request.vials_per_card;
									updatePropertyCollections.Add("Vials Per Card", str5, packItemId.ToString());
									packItemId = beforeFillSequence.stack_height;
									string str6 = packItemId.ToString();
									packItemId = request.stack_height;
									updatePropertyCollections.Add("Stack Height", str6, packItemId.ToString());
									updatePropertyCollections.Add("Batch Size", string.Concat(beforeFillSequence.batch_size.ToStringPrec4(), " ", beforeFillSequence.batch_size_UOM), string.Concat(request.batch_size.ToStringPrec4(), " ", request.batch_size_uom));
									updatePropertyCollections.Add("Change Over Hours", beforeChangeOverHours, request.changeover_time_hours);
									updatePropertyCollections.Add("Change Over Days", beforeChangeOverDays, request.changeover_time_days);
									updatePropertyCollections.Add("Notes", beforeFillSequence.notes, request.notes);
									updatePropertyCollections.Add("Master Formula", beforeFillSequence.mf_number, request.mf_number);
									updatePropertyCollections.Add("Is Fast Pack", beforeFillSequence.is_fast_pack, request.is_fast_pack);
									bool? isBatch = beforeFillSequence.is_batch;
									updatePropertyCollections.Add("Is Formulation Batch", isBatch.GetValueOrDefault(), request.is_formulation_batch);
									if (beforeFillingCage != null)
									{
										cageName = beforeFillingCage.cage_name;
									}
									else
									{
										cageName = null;
									}
									if (afterFillingCage != null)
									{
										cageName1 = afterFillingCage.cage_name;
									}
									else
									{
										cageName1 = null;
									}
									updatePropertyCollections.Add("Filling Component Assembly", cageName, cageName1);
									if (subLots != null)
									{
										array = (
											from sl in subLots
											select string.Concat(new string[] { str1, " ", sl, " ", str2, "." })).ToArray<string>();
									}
									else
									{
										array = null;
									}
									AuditTrailClient.Update("FG-PDS-U", sessionToken, applicationName, applicationVersion, requestClientIpAddress, lotNo, null, confirmESigId, auditTextCollections, updatePropertyCollections, array);
									this._stinvEntities.proc_PDS_WIP_ProductionFillSequence_M_update(request.fill_id, new DateTime?(request.scheduled_date), request.prod_line_no, new int?(request.pack_item_id), new int?(request.expiration_interval), request.expiration_interval_type, request.vials_per_card, request.stack_height, request.batch_size, request.batch_size_uom, new int?(request.changeover_time_days), new int?(request.changeover_time_hours), request.notes, request.container_type_code, new int?(base.UserId), request.mold_type_id, new bool?(request.is_sub_lot), request.is_fast_pack, new bool?(request.is_formulation_batch), request.mf_config_id, request.should_copy_mf_items, request.filling_cage_config_id);
									transaction.Complete();
									return this.Ok<FillSequenceResult>(new FillSequenceResult()
									{
										FillId = request.fill_id,
										ScheduledDate = new DateTime?(request.scheduled_date),
										ProdLineNo = request.prod_line_no,
										LotNo = request.lot_no
									});
								}
								else
								{
									errorMsg = "After: Product was not found.";
									this._logger.LogError(errorMsg);
									transaction.Dispose();
									httpActionResult = this.BadRequest(errorMsg);
								}
							}
							else
							{
								errorMsg = "After: Pack item was not found.";
								this._logger.LogError(errorMsg);
								transaction.Dispose();
								httpActionResult = this.BadRequest(errorMsg);
							}
						}
						else
						{
							errorMsg = "Before: Product was not found.";
							this._logger.LogError(errorMsg);
							transaction.Dispose();
							httpActionResult = this.BadRequest(errorMsg);
						}
					}
					else
					{
						errorMsg = "Before: Pack item was not found.";
						this._logger.LogError(errorMsg);
						transaction.Dispose();
						httpActionResult = this.BadRequest(errorMsg);
					}
				}
				else
				{
					errorMsg = "Production schedule was not found.";
					this._logger.LogError(errorMsg);
					transaction.Dispose();
					httpActionResult = this.BadRequest(errorMsg);
				}
			}
			return httpActionResult;
		}

		[HttpPut]
		[Route("update-formulation-finish-date/{id}")]
		[ValidateSession(Ability="Production Scheduling - Edit Formulation Date")]
		public IHttpActionResult PutUpdateFormulationFinishDate(Guid id, UpdateFormulationFinishDateRequest request)
		{
			string errorMsg;
			IHttpActionResult httpActionResult;
			string str;
			EditFillSequenceFormulationFinishDateResult result = new EditFillSequenceFormulationFinishDateResult();
			using (TransactionScope transaction = TransactionUtility.CreateRequiredReadCommitted())
			{
				fn_WIP_ProductionFillSequence_M_getOutput beforeFillSequence = this.GetFillSequence(new Guid?(id));
				if (beforeFillSequence != null)
				{
					int? packItemId = beforeFillSequence.pack_item_id;
					fn_WIP_PackItem_M_getOutput beforePackItem = this.GetPackItem(packItemId.GetValueOrDefault());
					if (beforePackItem != null)
					{
						string expText = (beforeFillSequence.expiration_interval_type == "day" ? "Number of Days to Expiration" : "Number of Months to Expiration");
						string sessionToken = base.SessionToken;
						string applicationName = base.ApplicationName;
						string applicationVersion = base.ApplicationVersion;
						string requestClientIpAddress = base.RequestClientIpAddress;
						string lotNo = beforeFillSequence.lot_no;
						Guid? nullable = new Guid?(request.EsignId);
						AuditTextCollection auditTextCollections = new AuditTextCollection()
						{
							{ "Production Line Number", beforeFillSequence.prod_line_no },
							{ "Lot Number", beforeFillSequence.lot_no },
							{ "Product NDC", beforePackItem.NDC_Code }
						};
						UpdatePropertyCollection updatePropertyCollections = new UpdatePropertyCollection();
						DateTime valueOrDefault = beforeFillSequence.scheduled_date.GetValueOrDefault();
						string str1 = valueOrDefault.ToString(CultureInfo.InvariantCulture);
						valueOrDefault = request.ScheduledDate.GetValueOrDefault();
						updatePropertyCollections.Add("Scheduled Date", str1, valueOrDefault.ToString(CultureInfo.InvariantCulture));
						valueOrDefault = beforeFillSequence.fill_date.GetValueOrDefault();
						string str2 = valueOrDefault.ToString(CultureInfo.InvariantCulture);
						valueOrDefault = request.FillDate.GetValueOrDefault();
						updatePropertyCollections.Add("Formulation Date", str2, valueOrDefault.ToString(CultureInfo.InvariantCulture));
						string str3 = expText;
						packItemId = beforeFillSequence.expiration_interval;
						if (packItemId.HasValue)
						{
							str = packItemId.GetValueOrDefault().ToString();
						}
						else
						{
							str = null;
						}
						updatePropertyCollections.Add(str3, str, request.ExpirationInterval.ToString());
						updatePropertyCollections.Add("Notes", beforeFillSequence.notes, request.Notes);
						AuditTrailClient.Update("FG-PDS-U", sessionToken, applicationName, applicationVersion, requestClientIpAddress, lotNo, null, nullable, auditTextCollections, updatePropertyCollections, null);
						this._stinvEntities.proc_PDS_WIP_ProductionFillSequence_M_update_formulation_finish_date(new Guid?(id), request.FillDate, request.ScheduledDate, new int?(request.ExpirationInterval), request.ExpirationIntervalType, request.Notes, new int?(base.UserId));
						transaction.Complete();
						result.FillId = new Guid?(id);
						result.ScheduledDate = request.ScheduledDate;
						result.ProdLineNo = beforeFillSequence.prod_line_no;
						result.LotNo = beforeFillSequence.lot_no;
						return this.Ok<EditFillSequenceFormulationFinishDateResult>(result);
					}
					else
					{
						errorMsg = "Before: Pack item was not found.";
						this._logger.LogError(errorMsg);
						transaction.Dispose();
						httpActionResult = this.BadRequest(errorMsg);
					}
				}
				else
				{
					errorMsg = "Production schedule was not found.";
					this._logger.LogError(errorMsg);
					transaction.Dispose();
					httpActionResult = this.BadRequest(errorMsg);
				}
			}
			return httpActionResult;
		}

		[HttpGet]
		[Route("search")]
		public IHttpActionResult SearchSchedules([ModelBinder] SearchSchedulesRequest request)
		{
			int resultValue;
			int? nullable;
			DateTime? nullable1;
			bool? nullable2;
			string statusCode;
			string prodLineNo;
			int? productId;
			int? packItemId;
			string lotNo;
			string mfNumber;
			DateTime? nullable3;
			DateTime? nullable4;
			bool? isFormulationBatch;
			bool? onlyProduction;
			this._logger.LogDebug("ProductionSchedulesController.SearchSchedules", new object[] { request });
			StinvEntities stinvEntity = this._stinvEntities;
			if (request != null)
			{
				statusCode = request.StatusCode;
			}
			else
			{
				statusCode = null;
			}
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
				nullable = null;
				productId = nullable;
			}
			if (request != null)
			{
				packItemId = request.PackItemId;
			}
			else
			{
				nullable = null;
				packItemId = nullable;
			}
			if (request != null)
			{
				lotNo = request.LotNo;
			}
			else
			{
				lotNo = null;
			}
			if (request != null)
			{
				mfNumber = request.MfNumber;
			}
			else
			{
				mfNumber = null;
			}
			if (request != null)
			{
				nullable3 = new DateTime?(request.FromDate);
			}
			else
			{
				nullable1 = null;
				nullable3 = nullable1;
			}
			if (request != null)
			{
				nullable4 = new DateTime?(request.ToDate);
			}
			else
			{
				nullable1 = null;
				nullable4 = nullable1;
			}
			if (request != null)
			{
				isFormulationBatch = request.IsFormulationBatch;
			}
			else
			{
				nullable2 = null;
				isFormulationBatch = nullable2;
			}
			if (request != null)
			{
				onlyProduction = request.OnlyProduction;
			}
			else
			{
				nullable2 = null;
				onlyProduction = nullable2;
			}
			List<proc_PDS_search_schedulesOutput> result = stinvEntity.proc_PDS_search_schedules(statusCode, prodLineNo, productId, packItemId, lotNo, mfNumber, nullable3, nullable4, isFormulationBatch, onlyProduction, out resultValue);
			this._logger.LogDebug("ProductionSchedulesController.SearchSchedules", new object[] { result });
			return this.Ok<List<proc_PDS_search_schedulesOutput>>(result);
		}

		public class TryUpdateFillDateFromEbrRequest
		{
			public Guid fill_id
			{
				get;
				set;
			}

			public TryUpdateFillDateFromEbrRequest()
			{
			}
		}

		public class TryUpdateFillDateFromEbrResult
		{
			public Guid fill_id
			{
				get;
				set;
			}

			public DateTime? formulation_finish_date
			{
				get;
				set;
			}

			public bool? is_changed
			{
				get;
				set;
			}

			public bool? is_fill_date_from_EBR_bit
			{
				get;
				set;
			}

			public TryUpdateFillDateFromEbrResult()
			{
			}
		}
	}
}