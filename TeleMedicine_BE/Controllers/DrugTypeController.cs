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
    [Route("api/v1/drug-types")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1,2")]
    public class DrugTypeController : ControllerBase
    {
        private readonly IDrugTypeService _drugTypeService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<DrugType> _pagingSupport;
        public DrugTypeController(IDrugTypeService drugTypeService, IMapper mapper, IPagingSupport<DrugType> pagingSupport)
        {
            _drugTypeService = drugTypeService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list drug types
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
        /// <returns>List drug types</returns>
        /// <response code="200">Returns list drug types</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<Paged<DrugTypeVM>> GetAllDrugTypes(
            [FromQuery(Name = "name")] string name,
            [FromQuery(Name = "order-by")] DrugTypeFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "limit")] int limit = 20,
            [FromQuery(Name = "page-offset")] int pageOffset = 1
        )
        {
            try
            {
                IQueryable<DrugType> drugTypesQuery = _drugTypeService.GetAll();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    drugTypesQuery = drugTypesQuery.Where(_ => _.Name.ToUpper().Contains(name.Trim().ToUpper()));
                }
                Paged<DrugTypeVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(DrugTypeVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(drugTypesQuery)
                   .GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0)
                   .Paginate<DrugTypeVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(DrugTypeVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(drugTypesQuery)
                   .GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1)
                   .Paginate<DrugTypeVM>();
                }
                else
                {
                    paged = _pagingSupport.From(drugTypesQuery)
                   .GetRange(pageOffset, limit, s => s.Id, 1)
                   .Paginate<DrugTypeVM>();
                }
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(DrugTypeVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(DrugTypeVM), splitFilter, paged);
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
        /// Get a specific drug type by drug type id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET {
        ///         "id" : 1
        ///     }
        /// </remarks>
        /// <returns>Return the drug type with the corresponding id</returns>
        /// <response code="200">Returns the drug type with the specified id</response>
        /// <response code="404">No drug type found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<DrugTypeVM>> GetDrugTypeById([FromRoute] int id)
        {
            try
            {
                DrugType drugType = await _drugTypeService.GetByIdAsync(id);
                if (drugType == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<DrugTypeVM>(drugType));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new drug type
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST 
        ///     {
        ///         "name": "Thuốc hỗ trợ hóa trị",    
        ///         "description": "Các loại thuốc này thường có nồng độ Radium cao",    
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Created new drug type</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<DrugTypeVM>> CreateDrugType([FromBody] DrugTypeCM model)
        {
            DrugType drugType = _mapper.Map<DrugType>(model);
            try
            {
                DrugType createdDrugType = await _drugTypeService.AddAsync(drugType);

                if (createdDrugType != null)
                {
                    return CreatedAtAction("GetDrugTypeById", new { id = createdDrugType.Id }, _mapper.Map<DrugTypeVM>(createdDrugType));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete drug type
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
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteDrugType([FromRoute] int id)
        {
            DrugType currentDrugType = await _drugTypeService.GetByIdAsync(id);
            if (currentDrugType == null)
            {
                return NotFound();
            }

            try
            {
                bool isDeleted = await _drugTypeService.DeleteAsync(currentDrugType);
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
        /// Update a drug type
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT 
        ///     {
        ///         "id": 11,
        ///         "name": "Thuốc gây tê",    
        ///         "description": "Các thuốc này chứa thành phần morphin cao",    
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<DrugTypeVM>> UpdateDrugType([FromBody] DrugTypeUM model)
        {
            DrugType currentDrugType = await _drugTypeService.GetByIdAsync(model.Id);
            if (currentDrugType == null)
            {
                return NotFound();
            }
            try
            {
                currentDrugType.Description = model.Description;
                currentDrugType.Name = model.Name;
                bool isSuccess = await _drugTypeService.UpdateAsync(currentDrugType);
                if (isSuccess)
                {
                    return Ok(_mapper.Map<DrugTypeVM>(currentDrugType));
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
