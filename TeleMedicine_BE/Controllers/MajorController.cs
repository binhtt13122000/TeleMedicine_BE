﻿using AutoMapper;
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
    [Route("api/v1/majors")]
    [ApiController]
    public class MajorController : Controller
    {
        private readonly IMajorService _majorService;
        private IMapper _mapper;
        private IPagingSupport<Major> _pagingSupport;

        public MajorController(IMajorService majorService, IMapper mapper, IPagingSupport<Major> pagingSupport)
        {
            _majorService = majorService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get all majors
        /// </summary>
        /// <returns>All majors</returns>
        /// <response code="200">Returns all majors</response>
        /// <response code="404">Not found majors</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<MajorVM>> GetAllMajor(
            [FromQuery(Name = "name")] String name,
            int offset = 1,
            int limit = 20
        )
        {
            try
            {
                IQueryable<Major> majorList = _majorService.GetAll();
                if(!String.IsNullOrEmpty(name))
                {
                    majorList = majorList.Where(s => s.Name.ToUpper().Contains(name.Trim().ToUpper()));
                }
                Paged<MajorVM> paged = _pagingSupport.From(majorList).GetRange(offset, limit, s => s.Id, 1).Paginate<MajorVM>();
                return Ok(paged);
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get major by id
        /// </summary>
        /// <returns>Return the major with the corresponding id</returns>
        /// <response code="200">Returns the major type with the specified id</response>
        /// <response code="404">No major found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MajorVM>> GetMajorById(int id)
        {
            try
            {
                Major currentMajor = await _majorService.GetByIdAsync(id);
                if(currentMajor != null)
                {
                    return Ok(_mapper.Map<MajorVM>(currentMajor));
                }
                return NotFound("Can not found major by id: " + id);
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new major
        /// </summary>
        /// <response code="200">Created new major successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MajorCM>> CreateMajor([FromBody] MajorCM model)
        {
            Major major = _mapper.Map<Major>(model);
            try
            {
                Major majorCreated =  await _majorService.AddAsync(major);
                if(majorCreated != null)
                {
                    return CreatedAtAction("GetMajorById", new { id = majorCreated.Id }, _mapper.Map<MajorVM>(majorCreated));
                }
                return BadRequest();
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a major
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
        public async Task<ActionResult> PutMajor(int id, [FromBody] MajorUM model)
        {
            Major currentMajor = await _majorService.GetByIdAsync(id);
            if(currentMajor == null)
            {
                return NotFound();
            }
            if(currentMajor.Id != model.Id)
            {
                return BadRequest();
            }
            try
            {
                currentMajor.Name = model.Name;
                currentMajor.Description = model.Description;
                bool isUpdated = await _majorService.UpdateAsync(currentMajor);
                if(isUpdated)
                {
                    return Ok(_mapper.Map<MajorVM>(currentMajor));
                }
                return BadRequest();
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete major By Id
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
            Major currentMajor = await _majorService.GetByIdAsync(id);
            if(currentMajor == null)
            {
                return BadRequest(new
                {
                    message = "Can not found major by id: " + id
                });
            }
            try
            {
                bool isDeleted = await _majorService.DeleteAsync(currentMajor);
                if(isDeleted)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
                }
                return BadRequest();
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}