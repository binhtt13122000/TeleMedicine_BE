
using AutoMapper;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeleMedicine_BE.ExternalService;
using TeleMedicine_BE.Utils;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/health-checks")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HealthCheckController : Controller
    {
        private readonly IHealthCheckService _healthCheckService;
        private readonly ISlotService _slotService;
        private readonly ISymptomService _symptomService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IMapper _mapper;
        private readonly IAgoraProvider _agoraProvider;
        private readonly IPagingSupport<HealthCheck> _pagingSupport;
        private readonly INotificationService _notificationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IAccountService _accountService;
        private readonly IFirestoreService _firestoreService;


        public HealthCheckController(IHealthCheckService healthCheckService, ISlotService slotService, IDoctorService doctorService, ISymptomService symptomService, IPatientService patientService, IMapper mapper, IPagingSupport<HealthCheck> pagingSupport, IAgoraProvider agoraProvider, IPushNotificationService pushNotificationService, INotificationService notificationService, IAccountService accountService, IFirestoreService firestoreService)
        {
            _healthCheckService = healthCheckService;
            _slotService = slotService;
            _doctorService = doctorService;
            _symptomService = symptomService;
            _patientService = patientService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
            _agoraProvider = agoraProvider;
            _notificationService = notificationService;
            _pushNotificationService = pushNotificationService;
            _accountService = accountService;
            _firestoreService = firestoreService;
        }

        /// <summary>
        /// Get list health checks
        /// </summary>
        /// <returns>List health checks</returns>
        /// <response code="200">Returns list health checks</response>
        /// <response code="400">Bad requests</response>
        /// <response code="404">Not found health checks</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<HealthCheckVM>> GetAllHealthChecks(
            [FromQuery(Name = "status")] HealthCheckStatus status,
            [FromQuery(Name = "mode")] HealthCheckMode mode,
            [FromQuery(Name = "start-created-time")] DateTime? startCreatedTime,
            [FromQuery(Name = "end-created-time")] DateTime? endCreatedTime,
            [FromQuery(Name = "start-canceled-time")] DateTime? startCanceledTime,
            [FromQuery(Name = "end-canceled-time")] DateTime? endCanceledTime,
            [FromQuery(Name = "order-by")] HealthCheckFieldEnum orderBy,
            [FromQuery(Name = "mode-search")] TypeSearch modeSearch,
            [FromQuery(Name = "type-role")] TypeRole typeRole,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "patient-id")] int patientId = 0,
            [FromQuery(Name = "doctor-id")] int doctorId = 0,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "start-rating")] int startRating = 0,
            [FromQuery(Name = "end-rating")] int endRating = 0,
            [FromQuery(Name = "page-offset")] int pageOffset = 1,
            int limit = 50
        )
        {
            try
            {
                IQueryable<HealthCheck> healthChecks = _healthCheckService.access().Include(s => s.Slots).ThenInclude(s => s.Doctor)
                                                                                    .Include(s => s.Patient)
                                                                                    .Include(s => s.Prescriptions).ThenInclude(s => s.Drug).ThenInclude(s => s.DrugType)
                                                                                    .Include(s => s.HealthCheckDiseases).ThenInclude(s => s.Disease)
                                                                                    .Include(s => s.SymptomHealthChecks).ThenInclude(s => s.Symptom);
                if(status != HealthCheckStatus.ALL)
                {
                    if(status == HealthCheckStatus.BOOKED)
                    {
                        healthChecks = healthChecks.Where(s => s.Status.Equals("BOOKED"));
                    }else if(status == HealthCheckStatus.CANCELED)
                    {
                        healthChecks = healthChecks.Where(s => s.Status.Equals("CANCELED"));
                    }else if(status == HealthCheckStatus.COMPLETED)
                    {
                        healthChecks = healthChecks.Where(s => s.Status.Equals("COMPLETED"));
                    }
                }    
                if (startRating != 0 && endRating != 0)
                {
                    healthChecks = healthChecks.Where(s => s.Rating >= startRating).
                                                Where(s => s.Rating <= endRating);
                }
                
                if (startCreatedTime.HasValue && endCreatedTime.HasValue)
                {
                    healthChecks = healthChecks.Where(s => s.CreatedTime.Value.CompareTo(startCreatedTime.Value) >= 0).
                                                Where(s => s.CreatedTime.Value.CompareTo(endCreatedTime.Value) <= 0);
                }
                else
                {
                    if (startCreatedTime.HasValue)
                    {
                        healthChecks = healthChecks.Where(s => s.CreatedTime.Value.CompareTo(startCreatedTime.Value) >= 0);
                    }
                    if (endCreatedTime.HasValue)
                    {
                        healthChecks = healthChecks.Where(s => s.CreatedTime.Value.CompareTo(endCreatedTime.Value) <= 0);
                    }
                }
                if (startCanceledTime.HasValue && endCanceledTime.HasValue)
                {
                    healthChecks = healthChecks.Where(s => s.CreatedTime.Value.CompareTo(startCanceledTime.Value) >= 0).
                                                Where(s => s.CreatedTime.Value.CompareTo(endCanceledTime.Value) <= 0);
                }
                else
                {
                    if (startCanceledTime.HasValue)
                    {
                        healthChecks = healthChecks.Where(s => s.CreatedTime.Value.CompareTo(startCanceledTime.Value) >= 0);
                    }
                    if (endCanceledTime.HasValue)
                    {
                        healthChecks = healthChecks.Where(s => s.CreatedTime.Value.CompareTo(endCanceledTime.Value) <= 0);
                    }
                }
                if (patientId != 0)
                {
                    Patient currentPatient = _patientService.GetAll().Where(s => s.Id == patientId).FirstOrDefault();
                    if(currentPatient == null)
                    {
                        return BadRequest(new
                        {
                            message = "Bad Request"
                        });
                    }
                    healthChecks = healthChecks.Where(s => s.PatientId == patientId);
                }
                if(doctorId != 0)
                {
                    Doctor currentDoctor = _doctorService.GetAll().Where(s => s.Id == doctorId).FirstOrDefault();
                    if(currentDoctor == null)
                    {
                        return BadRequest(new
                        {
                            message = "Bad Request"
                        });
                    }
                    healthChecks = healthChecks.Where(s => s.Slots.Any(s => s.DoctorId == doctorId));
                }
                if(mode == HealthCheckMode.CALL)
                {
                    DateTime currentDate = DateTime.Now;
                    foreach(HealthCheck item in healthChecks)
                    {
                        if(item.Slots.Any(s => s.AssignedDate.Date.CompareTo(currentDate.Date) != 0))
                        {
                            item.Token = null;
                        }
                    }
                }
                if (modeSearch == TypeSearch.NEAREST)
                {
                    DateTime currentDate = DateTime.Today;
                    TimeSpan currentTime = DateTime.Now.TimeOfDay;
                    if(typeRole == TypeRole.DOCTOR)
                    {
                        IEnumerable<Slot> slots = _slotService.GetAll(s => s.HealthCheck).Where(s => s.HealthCheckId != null).Where(s => s.HealthCheck.Status.Equals("BOOKED")).Where(s => s.AssignedDate.CompareTo(currentDate) >= 0);
                        if (doctorId != 0)
                        {
                            slots = slots.Where(s => s.DoctorId == doctorId);
                        }

                        if(slots.Count<Slot>() > 0)
                        {
                            HealthCheck healthCheck = _healthCheckService.GetNearestHealthCheckByCondition(slots.ToList(), currentDate, currentTime);
                            return Ok(_mapper.Map<HealthCheckVM>(healthCheck));
                        }
                        return NotFound(new
                        {
                            message = "Can not found health check nearest!"
                        });
                    }
                    else if(typeRole == TypeRole.USER)
                    {
                        IEnumerable<Slot> slots = _slotService.GetAll(s => s.HealthCheck).Where(s => s.HealthCheckId != null).Where(s => s.HealthCheck.Status.Equals("BOOKED")).Where(s => s.AssignedDate.CompareTo(currentDate) >= 0);

                        if (patientId != 0)
                        {
                            slots = slots.Where(s => s.HealthCheck.PatientId == patientId);
                        }

                        if (slots.Count<Slot>() > 0)
                        {
                            HealthCheck healthCheck = _healthCheckService.GetNearestHealthCheckByCondition(slots.ToList(), currentDate, currentTime);
                            return Ok(_mapper.Map<HealthCheckVM>(healthCheck));
                        }
                        return NotFound(new
                        {
                            message = "Can not found health check nearest!"
                        });
                    }
                    
                }
                Paged<HealthCheckVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(HealthCheckVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(healthChecks)
                   .GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0)
                   .Paginate<HealthCheckVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(HealthCheckVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(healthChecks)
                   .GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1)
                   .Paginate<HealthCheckVM>();
                }
                else
                {
                    paged = _pagingSupport.From(healthChecks)
                   .GetRange(pageOffset, limit, s => s.Id, 1)
                   .Paginate<HealthCheckVM>();
                }
                if(mode != HealthCheckMode.CALL)
                {
                    List<string> splitFilter = new List<string>();
                    bool checkHasProperty = false;
                    if (mode == HealthCheckMode.NORMAL)
                    {
                        splitFilter.Add("Token");
                    }
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(HealthCheckVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(HealthCheckVM), splitFilter.ToArray(), paged, PropertyRenameAndIgnoreSerializerContractResolver.IgnoreMode.IGNORE);
                        return Ok(JsonConvert.DeserializeObject(json));
                    }
                }
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    List<string> splitFilter = filters.Split(",").ToList();
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(HealthCheckVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(HealthCheckVM), splitFilter.ToArray(), paged, PropertyRenameAndIgnoreSerializerContractResolver.IgnoreMode.EXCEPT);
                        return Ok(JsonConvert.DeserializeObject(json));
                    }
                }
                return Ok(paged);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get a specific health check by health check id
        /// </summary>
        /// <returns>Return the health check with the corresponding id</returns>
        /// <response code="200">Returns the health check type with the specified id</response>
        /// <response code="404">No health check found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public ActionResult<HealthCheckVM> GetHealthCheckById([FromRoute] int id, [FromQuery(Name = "mode")] HealthCheckMode mode)
        {
            try
            {
                IQueryable<HealthCheck> healthChecks = _healthCheckService.access().Include(s => s.Slots).ThenInclude(s => s.Doctor)
                                                                                    .Include(s => s.Patient)
                                                                                    .Include(s => s.Prescriptions).ThenInclude(s => s.Drug).ThenInclude(s => s.DrugType)
                                                                                    .Include(s => s.HealthCheckDiseases).ThenInclude(s => s.Disease)
                                                                                    .Include(s => s.SymptomHealthChecks).ThenInclude(s => s.Symptom);
                HealthCheck currentHealthCheck = healthChecks.Where(s => s.Id == id).FirstOrDefault();
                if (currentHealthCheck != null)
                {
                    HealthCheckVM convertHealthCheck = _mapper.Map<HealthCheckVM>(currentHealthCheck);
                    if(mode == HealthCheckMode.NORMAL)
                    {
                        List<string> splitFilter = new List<string>();
                        bool checkHasProperty = false;
                        splitFilter.Add("Token");
                        foreach (var prop in splitFilter)
                        {
                            if (typeof(HealthCheckVM).GetProperty(prop) != null)
                            {
                                checkHasProperty = true;
                            }
                        }
                        if (checkHasProperty)
                        {
                            PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                            string json = jsonIgnore.JsonIgnoreObject(typeof(HealthCheckVM), splitFilter.ToArray(), convertHealthCheck, PropertyRenameAndIgnoreSerializerContractResolver.IgnoreMode.IGNORE);
                            return Ok(JsonConvert.DeserializeObject(json));
                        }
                    }else if(mode == HealthCheckMode.CALL)
                    {
                        DateTime currentDate = DateTime.Now;
                        if (currentHealthCheck.Slots.Any(s => s.AssignedDate.Date.CompareTo(currentDate.Date) != 0))
                        {
                            currentHealthCheck.Token = null;
                        }
                    }
                    return Ok(_mapper.Map<HealthCheckVM>(currentHealthCheck));
                }
                return NotFound(new
                {
                    message = "Can not found health check by id: " + id
                });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Create a new health check
        /// </summary>
        /// <response code="201">Created new health check successfull</response>
        /// <response code="404">Not found</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<HealthCheckVM>> CreateHealthCheck([FromBody] HealthCheckCM model)
        {
            try
            {
                Patient currentPatient = await _patientService.GetByIdAsync(model.PatientId);
                if(currentPatient == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found patient by id: " + model.PatientId
                    });
                }
                List<SymptomHealthCheckCM> diseaseList = model.SymptomHealthChecks.ToList();
                for (int i = 0; i < diseaseList.Count; i++)
                {

                    Symptom currentSymptom = await _symptomService.GetByIdAsync(diseaseList[i].SymptomId);
                    if (currentSymptom == null)
                    {
                        return BadRequest(new { 
                            message = "Symptom can not found!"
                        });
                    }
                }
                Slot currentSlot = await _slotService.GetByIdAsync(model.SlotId);
                if(currentSlot == null)
                {
                    return BadRequest(new
                    {
                        message = "Slot does not found!"
                    });
                }
                if(currentSlot != null && currentSlot.HealthCheckId != null)
                {
                    return BadRequest(new
                    {
                        message = "Slot have been registered!"
                    });
                }
                HealthCheck healthCheckConvert = _mapper.Map<HealthCheck>(model);
                healthCheckConvert.Status = HealthCheckSta.BOOKED.ToString();
                healthCheckConvert.CreatedTime = DateTime.Now;
                healthCheckConvert.Slots.Add(currentSlot);
                healthCheckConvert.Token = _agoraProvider.GenerateToken("SLOT_" + model.SlotId, 0.ToString(), 0);
                HealthCheck healthCheckCreated = await _healthCheckService.AddAsync(healthCheckConvert);
                if (healthCheckCreated != null)
                {
                    Slot addedSlot = _slotService.GetAll(s => s.Doctor).Where(s => s.Id == model.SlotId).FirstOrDefault();
                    await _pushNotificationService.SendMessage("Bạn có một lịch hẹn mới", "Bạn có một lịch hẹn mới", addedSlot.Doctor.Email, null);
                    Notification notification = new();
                    notification.Content = "Bạn có một lịch hẹn mới-/health-checks/" + healthCheckCreated.Id;
                    notification.Type = Constants.Notification.REQUEST_HEALTHCHECK;
                    notification.IsSeen = false;
                    notification.IsActive = true;
                    notification.CreatedDate = DateTime.Now;
                    notification.UserId = _accountService.GetAccountByEmail(addedSlot.Doctor.Email).Id;
                    await _notificationService.AddAsync(notification);
                    return CreatedAtAction("GetHealthCheckById", new { id = healthCheckCreated.Id }, _mapper.Map<HealthCheckVM>(healthCheckCreated));

                    
                }
                return BadRequest(new
                {
                    message = "Create health check failed!"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("join-call")]
        public async Task<ActionResult> JoinCall([FromBody] JoinCallRequest model)
        {
            HealthCheck healthCheck = await _healthCheckService.access().Include(s => s.Slots).ThenInclude(s => s.Doctor)
                                                                                    .Include(s => s.Patient).Where(s => s.Id == model.HealthCheckId).FirstOrDefaultAsync();
            if(healthCheck == null)
            {
                return BadRequest(new
                {
                    message = "Room is not exist!"
                });
            }
            Dictionary<string, object> data = await _firestoreService.Get("healthcheck", healthCheck.Id.ToString());
            if (model.Email.ToLower().Equals(healthCheck.Patient.Email.ToLower()) || model.Email.ToLower().Equals(healthCheck.Slots.ElementAt(0).Doctor.Email.ToLower()))
            {
                if(data == null)
                {
                    
                    Dictionary<string, object> createData = new Dictionary<string, object>()
                    {
                        {"1", model.DisplayName }
                    };
                    await _firestoreService.Create("healthcheck", healthCheck.Id.ToString(), createData);
                    return Ok(new
                    {
                        Uid = 1,
                        healthCheck.Token,
                        Slot = healthCheck.Slots.ElementAt(0).Id,
                    });
                } else
                {
                    int count = data.Count;
                    data[(count + 1) + ""] = model.DisplayName;
                    await _firestoreService.Update("healthcheck", healthCheck.Id.ToString(), data);
                    return Ok(new
                    {
                        Uid = (count + 1),
                        healthCheck.Token,
                        Slot = healthCheck.Slots.ElementAt(0).Id,
                    }); ;
                }
            } else
            {
                Account account = _accountService.GetAccountByEmail(model.Email);
                if(account != null)
                {
                    if (model.IsInvited)
                    {
                        if(data == null)
                        {
                            return BadRequest(new
                            {
                                message = "You can't join!"
                            });
                        } else
                        {
                            int count = data.Count;
                            data[(count + 1) + ""] = model.DisplayName;
                            await _firestoreService.Update("healthcheck", healthCheck.Id.ToString(), data);
                            return Ok(new
                            {
                                Uid = (count + 1),
                                healthCheck.Token,
                                Slot = healthCheck.Slots.ElementAt(0).Id,
                            });
                        }
                    } else
                    {
                        await _pushNotificationService.SendMessage("Có một yêu cầu tham gia cuộc họp!", model.DisplayName, healthCheck.Slots.ElementAt(0).Doctor.Email.ToLower(), new Dictionary<string, string> {
                            {"email", model.Email },
                            {"name", model.DisplayName },
                            {"token", healthCheck.Token },
                            {"slot", healthCheck.Slots.ElementAt(0).Id.ToString() },
                            {"id", healthCheck.Id.ToString() }
                        });
                        return BadRequest(new
                        {
                            message = "Waiting!"
                        });
                    }
                } else
                {
                    if(data != null)
                    {
                        await _pushNotificationService.SendMessage("Có một yêu cầu tham gia cuộc họp!", model.DisplayName, healthCheck.Slots.ElementAt(0).Doctor.Email.ToLower(), new Dictionary<string, string> {
                            {"email", model.Email },
                            {"name", model.DisplayName },
                            {"token", healthCheck.Token },
                            {"slot", healthCheck.Slots.ElementAt(0).Id.ToString() },
                            {"id", healthCheck.Id.ToString() }
                        });
                        return BadRequest(new
                        {
                            message = "Waiting!"
                        });
                    } else
                    {
                        return BadRequest(new
                        {
                            message = "You can't join!"
                        });
                    }
                }
            }
        }

        [HttpPost("accept-request")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        public async Task<ActionResult> AcceptRequest(AcceptRequestModel model)
        {
            Dictionary<string, object> data = await _firestoreService.Get("healthcheck", model.HealthCheckId);
            if (data == null)
            {
                return BadRequest();
            } else
            {
                int count = data.Count;
                data[(count + 1) + ""] = model.DisplayName;
                await _firestoreService.Update("healthcheck", model.HealthCheckId, data);
                await _pushNotificationService.SendMessage("Yêu cầu tham gia của bạn đã được đồng ý", "", model.Email, new Dictionary<string, string> {
                {"uid", (count + 1) + ""},
                {"email", model.Email },
                {"name", model.DisplayName },
                {"token", model.Token },
                {"slot", model.Slot.ToString() }
             });
                return Ok();
            }

        }

        /// <summary>
        /// Change status health check
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> ChangeStatusHealthCheck([FromRoute] int id, [FromBody] HealthCheckStatusUM status )
        {
            if(id != status.Id)
            {
                return BadRequest();
            }
            HealthCheck currentHealthCheck = await _healthCheckService.access().Include(s => s.Slots).ThenInclude(s => s.Doctor)
                                                                                    .Include(s => s.Patient).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (currentHealthCheck == null)
            {
                return NotFound();
            }
            try
            {
                currentHealthCheck.Status = status.status.ToString();
                currentHealthCheck.ReasonCancel = status.ReasonCancel;
                bool isUpdated = await _healthCheckService.UpdateAsync(currentHealthCheck);
                if (isUpdated)
                {
                    if (status.status.Equals("CANCELED)"))
                    {
                        //int doctorId = currentHealthCheck.Slots.Select(s => s.DoctorId).FirstOrDefault();
                        //if (doctorId != 0)
                        //{
                        //    Doctor currentDoctor = await _doctorService.GetByIdAsync(doctorId);
                        //    currentDoctor.NumberOfCancels += 1;
                        //    await _doctorService.UpdateAsync(currentDoctor);
                        //    List<Slot> slotList = currentHealthCheck.Slots.ToList();
                        //    for (int i = 0; i < slotList.Count; i++)
                        //    {
                        //        slotList[i].HealthCheck = null;
                        //        slotList[i].HealthCheckId = null;
                        //        await _slotService.UpdateAsync(slotList[i]);
                        //    }
                        //}
                        await _pushNotificationService.SendMessage("Lịch hẹn đã bị hủy", "Lịch hẹn đã bị hủy", currentHealthCheck.Patient.Email, null);
                        Notification notification = new();
                        notification.Content = "Lịch hẹn đã bị hủy";
                        notification.Type = Constants.Notification.REQUEST_HEALTHCHECK;
                        notification.IsSeen = false;
                        notification.IsActive = true;
                        notification.UserId = _accountService.GetAccountByEmail(currentHealthCheck.Patient.Email).Id;
                        notification.CreatedDate = DateTime.Now;
                        await _notificationService.AddAsync(notification);
                    }
                    if(status.status.Equals("COMPLETED"))
                    {
                        int doctorId = currentHealthCheck.Slots.Select(s => s.DoctorId).FirstOrDefault();
                        System.Diagnostics.Debug.WriteLine(doctorId);
                        if (doctorId != 0)
                        {
                            Doctor currentDoctor = await _doctorService.GetByIdAsync(doctorId);
                            currentDoctor.NumberOfConsultants += 1;
                            await _firestoreService.Delete("healthcheck", id.ToString());
                            await _doctorService.UpdateAsync(currentDoctor);
                            await _pushNotificationService.SendMessage("Buổi khám bệnh đã kết thúc", "Buổi khám bệnh đã kết thúc", currentHealthCheck.Patient.Email, null);
                            Notification notification = new();
                            notification.Content = "Buổi khám bệnh đã kết thúc";
                            notification.Type = Constants.Notification.FINISH_HEATHCHECK;
                            notification.IsSeen = false;
                            notification.IsActive = true;
                            notification.UserId = _accountService.GetAccountByEmail(currentHealthCheck.Patient.Email).Id;
                            await _notificationService.AddAsync(notification);
                        }
                    }
                    return Ok(new
                    {
                        message = "success"
                    });
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete health check
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE 
        ///     {
        ///         "id": 1
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteHealthCheck([FromRoute] int id)
        {
            HealthCheck currentHealthCheck = await _healthCheckService.GetByIdAsync(id);
            if (currentHealthCheck == null)
            {
                return NotFound();
            }
            List<Slot> slotList = currentHealthCheck.Slots.ToList();
            currentHealthCheck.Status = HealthCheckSta.CANCELED.ToString();

            try
            {
                for (int i = 0; i < slotList.Count; i++)
                {
                    slotList[i].HealthCheck = null;
                    slotList[i].HealthCheckId = null;
                    await _slotService.UpdateAsync(slotList[i]);
                }
                currentHealthCheck.Slots = null;
                bool isDeleted = await _healthCheckService.UpdateAsync(currentHealthCheck);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "success"
                    });
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a health check
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<HealthCheckVM>> UpdateHealthCheck([FromQuery] HealthCheckTypeRole mode, [FromBody] HealthCheckUM model)
        {
            try
            {
                HealthCheck currentHealthCheck = await _healthCheckService.GetByIdAsync(model.Id);
                if (currentHealthCheck == null)
                {
                    return NotFound();
                }
                if (mode == HealthCheckTypeRole.USERS)
                {
                    currentHealthCheck.Rating = model.Rating;
                    currentHealthCheck.Comment = model.Comment;
                    bool isSuccess = await _healthCheckService.UpdateAsync(currentHealthCheck);
                    if (isSuccess)
                    {
                        return Ok(_mapper.Map<HealthCheckVM>(currentHealthCheck));
                    }
                    return BadRequest();
                }else if(mode == HealthCheckTypeRole.DOCTORS)
                {
                    List<HealthCheckDisease> converDisease = new List<HealthCheckDisease>();
                    List<HealthCheckDiseaseCM> diseases = model.HealthCheckDiseases.ToList();
                    if (diseases != null && diseases.Count > 0)
                    {
                        foreach (HealthCheckDiseaseCM item in diseases)
                        {
                            converDisease.Add(_mapper.Map<HealthCheckDisease>(item));
                        }
                    }

                    List<Prescription> converPrescription = new List<Prescription>();
                    List<PrescriptionHealthCheckCM> prescriptions = model.Prescriptions.ToList();
                    if (prescriptions != null && prescriptions.Count > 0)
                    {
                        foreach (PrescriptionHealthCheckCM item in prescriptions)
                        {
                            converPrescription.Add(_mapper.Map<Prescription>(item));
                        }
                    }
                    currentHealthCheck.Rating = model.Rating;
                    currentHealthCheck.Advice = model.Advice;
                    currentHealthCheck.HealthCheckDiseases = converDisease;
                    currentHealthCheck.Prescriptions = converPrescription;
                    bool isSuccess = await _healthCheckService.UpdateAsync(currentHealthCheck);
                    if (isSuccess)
                    {
                        return Ok(_mapper.Map<HealthCheckVM>(currentHealthCheck));
                    }
                    return BadRequest();
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
