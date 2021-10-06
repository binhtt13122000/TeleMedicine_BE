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
    [Route("api/v1/time-frames")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1,2")]
    public class TimeFrameController : Controller
    {
        private readonly ITimeFrameService _timeFrameService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<TimeFrame> _pagingSupport;

        public TimeFrameController(ITimeFrameService timeFrameService, IMapper mapper, IPagingSupport<TimeFrame> pagingSupport)
        {
            _timeFrameService = timeFrameService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list time-frames
        /// </summary>
        /// <returns>List time frames</returns>
        /// <response code="200">Returns list time frames</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<Paged<TimeFrameVM>> GetAllTimeFrames(
            TimeSpan startTime,
            TimeSpan endTime,
            [FromQuery(Name = "order-by")] TimeFrameFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "page-offset")] int pageOffset = 1,
            int limit = 20
        )
        {
            try
            {
                IQueryable<TimeFrame> timeFrames = _timeFrameService.GetAll();
                if(startTime.CompareTo(TimeSpan.Zero) != 0)
                {
                    timeFrames = timeFrames.Where(s => s.StartTime.CompareTo(startTime) >= 0);
                }
                if(endTime.CompareTo(TimeSpan.Zero) != 0)
                {
                    timeFrames = timeFrames.Where(s => s.EndTime.CompareTo(endTime) <= 0);
                }
                Paged<TimeFrameVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(TimeFrameVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(timeFrames).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0).Paginate<TimeFrameVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(TimeFrameVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(timeFrames).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1).Paginate<TimeFrameVM>();
                }
                else
                {
                    paged = _pagingSupport.From(timeFrames).GetRange(pageOffset, limit, s => s.Id, 1).Paginate<TimeFrameVM>();
                }
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(TimeFrameVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(TimeFrameVM), splitFilter, paged);
                        return Ok(JsonConvert.DeserializeObject(json));
                    }
                }
                return Ok(paged);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Get a specific timeframe by timeframe id
        /// </summary>
        /// <returns>Return the time-frame with the corresponding id</returns>
        /// <response code="200">Returns the time-frame type with the specified id</response>
        /// <response code="404">No time-frame found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<TimeFrameVM>> GetTimeFrameById(int id)
        {
            try
            {
                TimeFrame timeFrame = await _timeFrameService.GetByIdAsync(id);
                if (timeFrame == null)
                {
                    return NotFound("Can not found time frame by id: " + id);
                }
                return Ok(_mapper.Map<TimeFrameVM>(timeFrame));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new time frame
        /// </summary>
        /// <response code="200">Created new time frame successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<TimeFrameVM>> CreateTimeFrame([FromBody] TimeFrameCM[] model)
        {
            try
            {
                List<TimeFrame> convertTimeFrames = new List<TimeFrame>();
                List<int> getTimeFramesId = _timeFrameService.GetAll().Select(s => s.Id).ToList();
                if (getTimeFramesId.Count > 0)
                {

                    _timeFrameService.DeleteListTimeFrame(getTimeFramesId);
                }

                foreach (TimeFrameCM item in model.ToList())
                {
                    await _timeFrameService.AddAsync(_mapper.Map<TimeFrame>(item));

                }
                return Ok(new
                {
                    message = "Create time frames success"
                });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Update a time frame
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult> PutTimeFrame([FromBody] TimeFrameUM model)
        {
            TimeFrame currentTimeFrame = await _timeFrameService.GetByIdAsync(model.Id);
            if(currentTimeFrame.EndTime.CompareTo(model.EndTime) != 0 || currentTimeFrame.StartTime.CompareTo(model.StartTime) != 0)
            {
                List<TimeFrame> getTimeFrames = _timeFrameService.GetAll().Where(s => s.Id != model.Id).ToList();
                if (getTimeFrames.Count > 0)
                {
                    for (int i = 0; i < getTimeFrames.Count; i++)
                    {
                        if (model.StartTime.CompareTo(getTimeFrames[i].EndTime) < 0 && getTimeFrames[i].StartTime.CompareTo(model.EndTime) < 0)
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
                currentTimeFrame.StartTime = model.StartTime;
                currentTimeFrame.EndTime = model.EndTime;
                bool isUpdated = await _timeFrameService.UpdateAsync(currentTimeFrame);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<TimeFrameVM>(currentTimeFrame));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete time frame By Id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                TimeFrame currentTimeFrame = await _timeFrameService.GetByIdAsync(id);
                if (currentTimeFrame == null)
                {
                    return BadRequest();
                }
                bool isDeleted = await _timeFrameService.DeleteAsync(currentTimeFrame);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Deleted time frame success"
                    });
                }
                return BadRequest(new
                {
                    message = "Delete time frame failed!"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
