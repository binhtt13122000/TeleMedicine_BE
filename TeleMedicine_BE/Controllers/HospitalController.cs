﻿using AutoMapper;
using BusinessLogic.Services;
using Infrastructure.Models;
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
    [Route("api/v1/hospitals")]
    [ApiController]
    [Authorize]
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
        /// Get list hospitals
        /// </summary>
        /// <returns>List hospitals</returns>
        /// <response code="200">Returns list hospitals</response>
        /// <response code="404">Not found hospitals</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<HospitalVM>> GetAllHospital(
            [FromQuery(Name = "hospital-code")] string hospitalCode,
            [FromQuery(Name = "name")] string name,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "asc-by")] string ascBy = null,
            [FromQuery(Name = "desc-by")] string descBy = null,
            int limit = 20,
            int offset = 1
        )
        {
            try
            {
                IQueryable<Hospital> hospitalList = _hospitalService.GetAll();
                if (!String.IsNullOrEmpty(hospitalCode))
                {
                    hospitalList = hospitalList.Where(s => s.HospitalCode.ToUpper().Contains(hospitalCode.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(name))
                {
                    hospitalList = hospitalList.Where(s => s.Name.ToUpper().Contains(name.Trim().ToUpper()));
                }
                Paged<HospitalVM> paged = null;
                if (!string.IsNullOrEmpty(ascBy) && typeof(HospitalVM).GetProperty(ascBy) != null)
                {
                    paged = _pagingSupport.From(hospitalList).GetRange(offset, limit, p => EF.Property<object>(p, ascBy), 0).Paginate<HospitalVM>();
                }
                else if (!string.IsNullOrEmpty(descBy) && typeof(HospitalVM).GetProperty(descBy) != null)
                {
                    paged = _pagingSupport.From(hospitalList).GetRange(offset, limit, p => EF.Property<object>(p, descBy), 1).Paginate<HospitalVM>();
                }
                else
                {
                    paged = _pagingSupport.From(hospitalList).GetRange(offset, limit, s => s.Id, 1).Paginate<HospitalVM>();
                }
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(HospitalVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(HospitalVM), splitFilter, paged);
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
        /// Get hospital by id
        /// </summary>
        /// <returns>Return the hospital with the corresponding id</returns>
        /// <response code="200">Returns the hospital type with the specified id</response>
        /// <response code="404">No hospital found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
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
        public async Task<ActionResult<HospitalCM>> CreateHospital([FromBody] HospitalCM model)
        {
            Hospital hospital = _mapper.Map<Hospital>(model);

            if (_hospitalService.GetAll().Where(s => s.HospitalCode.Trim().ToUpper().Equals(model.HospitalCode.Trim().ToUpper())).FirstOrDefault() != null)
            {
                return BadRequest(new
                {
                    message = "Hospital Code duplicated",
                });
            }
            try
            {
                hospital.HospitalCode = hospital.HospitalCode.Trim().ToUpper();
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
        public async Task<ActionResult<HospitalVM>> PutHospital(int id, [FromBody] HospitalUM model)
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
            if(!model.HospitalCode.Trim().ToUpper().Equals(currentHospital.HospitalCode.ToUpper()) && 
                _hospitalService.GetAll().Where(s => s.HospitalCode.Trim().ToUpper().Equals(model.HospitalCode.Trim().ToUpper())).FirstOrDefault() != null
                )
            {
                return BadRequest(new
                {
                    message = "Hospital Code is duplicated"
                });
            }
            try
            {
                currentHospital.Name = model.Name.Trim();
                currentHospital.Address = model.Address.Trim();
                currentHospital.Description = model.Description.Trim();
                currentHospital.HospitalCode = model.HospitalCode.Trim().ToUpper();
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
        public async Task<ActionResult> DeleteById(int id)
        {
            Hospital currentHospital = await _hospitalService.GetByIdAsync(id);
            if (currentHospital == null)
            {
                return BadRequest(new
                {
                    message = "Can not found hospital by id: " + id
                });
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
