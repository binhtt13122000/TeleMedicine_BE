﻿using AutoMapper;
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
    [Route("api/v1/symptoms")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SymptomController : ControllerBase
    {
        private readonly ISymptomService _symptomService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Symptom> _pagingSupport;
        public SymptomController(ISymptomService symptomService, IMapper mapper, IPagingSupport<Symptom> pagingSupport)
        {
            _symptomService = symptomService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }


        /// <summary>
        /// Get list symptoms
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET 
        ///     {
        ///         
        ///     }
        ///
        /// </remarks>
        /// <returns>List symptoms</returns>
        /// <response code="200">Returns list symptoms</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<Paged<SymptomVM>> GetAllSymptom(
            [FromQuery(Name = "code")] string symptomCode, 
            [FromQuery(Name = "name")] string name,
            [FromQuery(Name = "is-active")] bool? isActive,
            [FromQuery(Name = "order-by")] SymptomFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "limit")] int limit = 20,
            [FromQuery(Name = "page-offset")] int pageOffset = 1
        )
        {
            try
            {
                IQueryable<Symptom> symptomsQuery= _symptomService.GetAll();
                if (!string.IsNullOrWhiteSpace(symptomCode))
                {
                    symptomsQuery = symptomsQuery.Where(_ => _.SymptomCode.ToUpper().Contains(symptomCode.Trim().ToUpper()));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    symptomsQuery = symptomsQuery.Where(_ => _.Name.ToUpper().Contains(name.Trim().ToUpper()));
                }
                if(isActive.HasValue)
                {
                    symptomsQuery = symptomsQuery.Where(s => s.IsActive.Value.Equals(isActive.Value));
                }

                Paged<SymptomVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(SymptomVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(symptomsQuery)
                   .GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0)
                   .Paginate<SymptomVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(SymptomVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(symptomsQuery)
                   .GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1)
                   .Paginate<SymptomVM>();
                }
                else
                {
                    paged = _pagingSupport.From(symptomsQuery)
                   .GetRange(pageOffset, limit, s => s.Id, 1)
                   .Paginate<SymptomVM>();
                }
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(SymptomVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(SymptomVM), splitFilter, paged, PropertyRenameAndIgnoreSerializerContractResolver.IgnoreMode.EXCEPT);
                        return Ok(JsonConvert.DeserializeObject(json));
                    }
                }
                return Ok(paged);
            } catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get a specific symptom by symptom id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET {
        ///         "id" : 1
        ///     }
        /// </remarks>
        /// <returns>Return the symptom with the corresponding id</returns>
        /// <response code="200">Returns the symptom type with the specified id</response>
        /// <response code="404">No symptom found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<SymptomVM>> GetSymptomById([FromRoute] int id)
        {
            try
            {
                Symptom symptom = await _symptomService.GetByIdAsync(id);
                if (symptom == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<SymptomVM>(symptom));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new symptom
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST 
        ///     {
        ///         "symptomCode": "SS12",    
        ///         "name": "Động kinh",    
        ///         "description": "Ngất xỉu",    
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Created new symptoms</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<SymptomVM>> CreateSymptom([FromBody] SymptomCM model)
        {
            Symptom symptom = _mapper.Map<Symptom>(model);
            if (_symptomService.GetAll().Where(s=> s.SymptomCode.Trim().ToUpper().Equals(model.SymptomCode.Trim().ToUpper())).FirstOrDefault() != null)
            {
                return BadRequest(new
                {
                    message = "duplicate"
                });
            }
            try
            {
                symptom.SymptomCode = symptom.SymptomCode.Trim().ToUpper();
                Symptom createdSymptom = await _symptomService.AddAsync(symptom);

                if(createdSymptom != null)
                {
                    return CreatedAtAction("GetSymptomById", new { id = createdSymptom.Id }, _mapper.Map<SymptomVM>(createdSymptom));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Delete symptom
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
        public async Task<ActionResult> DeleteSymptom([FromRoute] int id)
        {
            Symptom currentSymptom = await _symptomService.GetByIdAsync(id);
            if (currentSymptom == null)
            {
                return NotFound();
            }

            try
            {
                bool isDeleted = await _symptomService.DeleteAsync(currentSymptom);
                if (isDeleted)
                {
                    return Ok(new { 
                        message="success"
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
        /// Update a symptom
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT 
        ///     {
        ///         "id": 11,
        ///         "symptomCode": "SS12",    
        ///         "name": "Động kinh",    
        ///         "description": "Ngất xỉu",    
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<SymptomVM>> UpdateSymptom([FromBody] SymptomUM model)
        {
            Symptom currentSymptom = await _symptomService.GetByIdAsync(model.Id);
            if(currentSymptom == null)
            {
                return NotFound();
            }
            if (!model.SymptomCode.ToUpper().Equals(currentSymptom.SymptomCode.ToUpper()) && _symptomService.GetAll().
                Where(s => s.SymptomCode.Trim().ToUpper().Equals(model.SymptomCode.Trim().ToUpper())).FirstOrDefault() != null)
            {
                return BadRequest(new
                {
                    message = "Symptom Code is duplicated"
                });
            }
            try
            {
                currentSymptom.Description = model.Description;
                currentSymptom.Name = model.Name;
                currentSymptom.SymptomCode = model.SymptomCode;
                currentSymptom.IsActive = model.IsActive;
                bool isSuccess = await _symptomService.UpdateAsync(currentSymptom);
                if (isSuccess)
                {
                    return Ok(_mapper.Map<SymptomVM>(currentSymptom));
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
