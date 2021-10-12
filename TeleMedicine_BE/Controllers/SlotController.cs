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
using System.Threading.Tasks;
using TeleMedicine_BE.Utils;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/slots")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SlotController : Controller
    {
        private readonly ISlotService _slotService;
        private readonly IAccountService _accountService;
        private readonly IDoctorService _doctorService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Slot> _pagingSupport;
        public SlotController(ISlotService slotService, IAccountService accountService, IDoctorService doctorService, IMapper mapper, IPagingSupport<Slot> pagingSupport)
        {
            _slotService = slotService;
            _accountService = accountService;
            _doctorService = doctorService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list slots
        /// </summary>
        /// <returns>List slots</returns>
        /// <response code="200">Returns list slots</response>
        /// <response code="404">Not found slots</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<SlotVM>> GetAllSlots(
            [FromQuery(Name = "start-assigned-date")] DateTime? startAssignedDate,
            [FromQuery(Name = "end-assigned-date")] DateTime? endAssignedDate,
            [FromQuery(Name = "doctor-id")] int doctorId,
            [FromQuery(Name = "doctor-first-name")] string doctorFirstName,
            [FromQuery(Name = "doctor-last-name")] string doctorLastName,
            [FromQuery(Name = "is-active")] bool? isActive,
            [FromQuery(Name = "start-time")] TimeSpan startTime,
            [FromQuery(Name = "end-time")] TimeSpan endTime,
            [FromQuery(Name = "order-by")] SlotFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "page-offset")] int pageOffset = 1,
            int limit = 50
        )
        {
            try
            {
                IQueryable<Slot> slotList = _slotService.Access().Include(s => s.Doctor)
                                                                       .Include(s => s.HealthCheck).ThenInclude(s => s.Patient);
                if (startAssignedDate.HasValue && endAssignedDate.HasValue)
                {
                    slotList = slotList.Where(s => s.AssignedDate.CompareTo(startAssignedDate.Value) >= 0).
                                        Where(s => s.AssignedDate.CompareTo(endAssignedDate.Value) <= 0);
                }
                else
                {
                    if (startAssignedDate.HasValue)
                    {
                        slotList = slotList.Where(s => s.AssignedDate.CompareTo(startAssignedDate.Value) >= 0);
                    }
                    if (endAssignedDate.HasValue)
                    {
                        slotList = slotList.Where(s => s.AssignedDate.CompareTo(endAssignedDate.Value) <= 0);
                    }
                }
                if (doctorId != 0)
                {
                    slotList = slotList.Where(s => s.DoctorId == doctorId);
                }
                if (!string.IsNullOrEmpty(doctorFirstName))
                {
                    IQueryable<Account> accountList = _accountService.GetAll().Where(s => s.FirstName.Trim().ToUpper().Contains(doctorFirstName.Trim().ToUpper()));
                    Account[] convertArrAccount = accountList.ToArray();
                    if (convertArrAccount.Length > 0)
                    {
                        for (int i = 0; i < convertArrAccount.Length; i++)
                        {
                            slotList = slotList.Where(s => s.DoctorId == convertArrAccount[i].Id);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(doctorLastName))
                {
                    IQueryable<Account> accountList = _accountService.GetAll().Where(s => s.LastName.Trim().ToUpper().Contains(doctorLastName.Trim().ToUpper()));
                    Account[] convertArrAccount = accountList.ToArray();
                    if (convertArrAccount.Length > 0)
                    {
                        for (int i = 0; i < convertArrAccount.Length; i++)
                        {
                            slotList = slotList.Where(s => s.DoctorId == convertArrAccount[i].Id);
                        }
                    }
                }
                if (startTime.CompareTo(TimeSpan.Zero) != 0)
                {
                    slotList = slotList.Where(s => s.StartTime.CompareTo(startTime) >= 0);
                }
                if (endTime.CompareTo(TimeSpan.Zero) != 0)
                {
                    slotList = slotList.Where(s => s.EndTime.CompareTo(endTime) <= 0);
                }
                if(isActive.HasValue)
                {
                    slotList = slotList.Where(s => s.IsActive.Value.Equals(isActive.Value));
                }
                Paged<SlotVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(SlotVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(slotList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0).Paginate<SlotVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(SlotVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(slotList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1).Paginate<SlotVM>();
                }
                else
                {
                    paged = _pagingSupport.From(slotList).GetRange(pageOffset, limit, s => s.Id, 1).Paginate<SlotVM>();
                }
                
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(SlotVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(SlotVM), splitFilter, paged);
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
        /// Get a specific slot by slot id
        /// </summary>
        /// <returns>Return the slot with the corresponding id</returns>
        /// <response code="200">Returns the slot with the specified id</response>
        /// <response code="404">No slot found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<SlotVM>> GetSlotById([FromRoute] int id)
        {
            try
            {
                Slot currentSlot = await _slotService.GetByIdAsync(id);
                if (currentSlot == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found slot by id: " + id
                    });
                }
                return Ok(_mapper.Map<SlotVM>(currentSlot));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete slot By Id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                Slot currentSlot = await _slotService.GetByIdAsync(id);
                if (currentSlot == null)
                {
                    return NotFound();
                }
                bool isDeleted = await _slotService.DeleteAsync(currentSlot);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
                }
                return BadRequest(new
                {
                    message = "Delete slot failed!"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new slot
        /// </summary>
        /// <response code="201">Created new slot successfull</response>
        /// <response code="404">Not found</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<SlotVM>> CreateSlot([FromBody] SlotCM[] model)
        {
            try
            {
                List<SlotCM> slotList = model.ToList();
                List<Slot> newListConvert = new List<Slot>();
                for(int i = 0; i <slotList.Count; i++)
                {
                    DateTime date = DateTime.Parse(slotList[i].AssignedDate.ToShortDateString());
                    Doctor currentDoctor = await _doctorService.GetByIdAsync(slotList[i].DoctorId);
                    if (currentDoctor == null)
                    {
                        return BadRequest(new
                        {
                            message = "Can not found doctor by id: " + slotList[i].DoctorId
                        });
                    }
                    List<Slot> getSlotsInDay = _slotService.GetAll().Where(s => s.DoctorId == slotList[i].DoctorId).
                                                                           Where(s => s.AssignedDate.CompareTo(date) >= 0).
                                                                           Where(s => s.AssignedDate.CompareTo(date.AddDays(1).AddSeconds(-1)) <= 0).ToList();
                    if (getSlotsInDay.Count > 0)
                    {
                        for (int j = 0; j < getSlotsInDay.Count; j++)
                        {
                            if (slotList[i].StartTime.CompareTo(getSlotsInDay[j].EndTime) < 0 && getSlotsInDay[j].StartTime.CompareTo(slotList[i].EndTime) < 0)
                            {
                                return BadRequest(new
                                {
                                    message = "Time ovelap!"
                                });
                            }
                        }
                    }
                    slotList[i].AssignedDate = date;
                    newListConvert.Add(_mapper.Map<Slot>(slotList[i]));
                }
                bool isCreated = await _slotService.AddSlotsAsync(newListConvert);
                if(isCreated)
                {
                    return Ok(new {
                        message = "Success"
                    });
                }
                return BadRequest(new
                {
                    message = "Create slot failed!"
                });
            }catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Update a slot
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult> PutTimeFrame([FromBody] SlotUM model)
        {

            Slot currentSlot = await _slotService.GetByIdAsync(model.Id);
            if(currentSlot == null)
            {
                return NotFound(new
                {
                    message = "Can not found slot"
                });
            }
            DateTime date = DateTime.Parse(model.AssignedDate.ToShortDateString());
            if (currentSlot.AssignedDate.CompareTo(model.AssignedDate) != 0 || currentSlot.StartTime.CompareTo(model.StartTime) != 0 || currentSlot.EndTime.CompareTo(model.EndTime) != 0)
            {
                List<Slot> getSlotsInDay = _slotService.GetAll().Where(s => s.DoctorId == model.DoctorId).
                                                                 Where(s => s.Id != model.Id).
                                                                 Where(s => s.AssignedDate.CompareTo(date) >= 0).
                                                                 Where(s => s.AssignedDate.CompareTo(date.AddDays(1).AddSeconds(-1)) <= 0).ToList();
                if (getSlotsInDay.Count > 0)
                {
                    for (int i = 0; i < getSlotsInDay.Count; i++)
                    {
                        if (model.StartTime.CompareTo(getSlotsInDay[i].EndTime) < 0 && getSlotsInDay[i].StartTime.CompareTo(model.EndTime) < 0)
                        {
                            return BadRequest(new
                            {
                                message = "Time ovelap!"
                            });
                        }
                    }
                }
            }
            try
            {
                currentSlot.AssignedDate = date;
                currentSlot.StartTime = model.StartTime;
                currentSlot.EndTime = model.EndTime;
                currentSlot.IsActive = model.IsActive;
                bool isUpdated = await _slotService.UpdateAsync(currentSlot);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<SlotVM>(currentSlot));
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
