using AutoMapper;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleMedicine_BE.Utils;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/hospitals")]
    [ApiController]
    public class HospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Hospital> _pagingSupport;
        public HospitalController(IHospitalService hospitalService, IMapper mapper, IPagingSupport<Hospital> pagingSupport)
        {
            _hospitalService = hospitalService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get all hospitals
        /// </summary>
        /// <returns>All hospitals</returns>
        /// <response code="200">Returns all hospitals</response>
        /// <response code="404">Not found hospitals</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<HospitalVM>> GetAllHospital(
            [FromQuery(Name = "hospital-code")] String hospitalCode,
            [FromQuery(Name = "name")] String name,
            int limit = 20,
            int offset = 1
        )
        {
            try
            {
                IQueryable<Hospital> hospitalList = _hospitalService.GetAll();
                if (!String.IsNullOrEmpty(hospitalCode))
                {
                    hospitalList = hospitalList.Where(s => s.HospitalCode.Contains(hospitalCode));
                }
                if (!String.IsNullOrEmpty(name))
                {
                    hospitalList = hospitalList.Where(s => s.Name.Contains(name.Trim()));
                }
                Paged<HospitalVM> pageModel = _pagingSupport.From(hospitalList).GetRange(offset, limit, s => s.Id, 1).Paginate<HospitalVM>();
                return Ok(pageModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get hospital by id
        /// </summary>
        /// <returns>Return the hospital with the corresponding id</returns>
        /// <response code="200">Returns the hospital type with the specified id</response>
        /// <response code="404">No hospital found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HospitalVM>> GetHospitalById(int id)
        {
            try
            {
                Hospital currentHospital = await _hospitalService.GetByIdAsync(id);
                if (currentHospital != null)
                {
                    HospitalVM returnHospital = _mapper.Map<HospitalVM>(currentHospital);
                    return Ok(returnHospital);
                }
                else
                {
                    return NotFound("Can not found hospital by id: " + id);
                }
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Create a new hospital
        /// </summary>
        /// <response code="200">Created new hospital successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HospitalCM>> CreateHospital([FromBody] HospitalCM model)
        {
            Hospital hospital = _mapper.Map<Hospital>(model);

            if (_hospitalService.IsDuplicated(model.HospitalCode))
            {
                return BadRequest(new
                {
                    message = "Hospital Code duplicated",
                });
            }
            try
            {
                Hospital hospitalCreated = await _hospitalService.AddAsync(hospital);
                if(hospitalCreated != null)
                {
                    return CreatedAtAction("GetHospitalById", new { id = hospitalCreated.Id }, _mapper.Map<HospitalVM>(hospitalCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a hospital
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Route("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PutHospital(int id, [FromBody] HospitalUM model)
        {
            Hospital currentHospital = await _hospitalService.GetByIdAsync(id);
            if (currentHospital == null)
            {
                return NotFound();
            }
            if(currentHospital.Id != model.Id)
            {
                return BadRequest();
            }
            if(!model.HospitalCode.ToUpper().Equals(currentHospital.HospitalCode.ToUpper()) && _hospitalService.IsDuplicated(model.HospitalCode))
            {
                return BadRequest(new
                {
                    message = "Hospital Code is duplicated"
                });
            }
            try
            {
                currentHospital.Name = model.Name;
                currentHospital.Address = model.Address;
                currentHospital.Description = model.Description;
                currentHospital.HospitalCode = model.HospitalCode;
                bool isUpdated = await _hospitalService.UpdateAsync(currentHospital);
                if(isUpdated)
                {
                    return Ok(_mapper.Map<HospitalVM>(currentHospital));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete hospital By Id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteById(int id)
        {
            Hospital currentHospital = await _hospitalService.GetByIdAsync(id);
            if (currentHospital == null)
            {
                return BadRequest(new
                {
                    message = "Can not found hospital by id: " + id
                });;
            }
            try
            {
                bool isDeleted = await _hospitalService.DeleteAsync(currentHospital);
                if(isDeleted)
                {
                    return Ok(new
                    {
                        message = "success"
                    });
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
