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
    [Route("api/v1/drugs")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1,2")]
    public class DrugController : ControllerBase
    {
        private readonly IDrugService _drugService;
        private readonly IDrugTypeService _drugTypeService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Drug> _pagingSupport;
        public DrugController(IDrugService drugService, IMapper mapper, IPagingSupport<Drug> pagingSupport, IDrugTypeService drugTypeService)
        {
            _drugService = drugService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
            _drugTypeService = drugTypeService;
        }

        /// <summary>
        /// Get list drugs
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
        /// <returns>List drugs</returns>
        /// <response code="200">Returns list drugs</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<Paged<DrugVM>> GetAllDrug(
            [FromQuery(Name = "name")] string name,
            [FromQuery(Name = "origin")] string drugOrigin,
            [FromQuery(Name = "producer")] string producer,
            [FromQuery(Name = "drug-form")] string drugForm,
            [FromQuery(Name = "drug-type")] int[] drugTypeIds,
            [FromQuery(Name = "order-by")] DrugFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "limit")] int limit = 20,
            [FromQuery(Name = "pageOffset")] int pageOffset = 1
        )
        {
            try
            {
                IQueryable<Drug> drugsQuery = _drugService.GetAll(_ => _.DrugType);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    drugsQuery = drugsQuery.Where(_ => _.Name.ToUpper().Contains(name.Trim().ToUpper()));
                }
                if (!string.IsNullOrWhiteSpace(producer))
                {
                    drugsQuery = drugsQuery.Where(_ => _.Producer.ToUpper().Contains(producer.Trim().ToUpper()));
                }
                if (!string.IsNullOrWhiteSpace(drugOrigin))
                {
                    drugsQuery = drugsQuery.Where(_ => _.DrugOrigin.ToUpper().Contains(drugOrigin.Trim().ToUpper()));
                }
                if (!string.IsNullOrWhiteSpace(drugForm))
                {
                    drugsQuery = drugsQuery.Where(_ => _.DrugForm.ToUpper().Contains(drugForm.Trim().ToUpper()));
                }
                if(drugTypeIds != null && drugTypeIds.Length > 0)
                {
                    drugsQuery = drugsQuery.Where(_ => drugTypeIds.Contains(_.DrugTypeId));
                }
                Paged<DrugVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(DrugVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(drugsQuery)
                   .GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0)
                   .Paginate<DrugVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(DrugVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(drugsQuery)
                   .GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1)
                   .Paginate<DrugVM>();
                }
                else
                {
                    paged = _pagingSupport.From(drugsQuery)
                   .GetRange(pageOffset, limit, s => s.Id, 1)
                   .Paginate<DrugVM>();
                }
        
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(DrugVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(DrugVM), splitFilter, paged);
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
        /// Get a specific drug by drug id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET {
        ///         "id" : 1
        ///     }
        /// </remarks>
        /// <returns>Return the drug with the corresponding id</returns>
        /// <response code="200">Returns the drug type with the specified id</response>
        /// <response code="404">No drug found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public ActionResult<DrugVM> GetDrugById([FromRoute] int id)
        {
            try
            {
                Drug drug = _drugService.GetAll(_ => _.DrugType).FirstOrDefault(_ => _.Id == id);
                if (drug == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<DrugVM>(drug));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new drug
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST 
        ///     {
        ///         "name": "Paradon",    
        ///         "producer": "Công ty synopharm",    
        ///         "drugOrigin": "Trung quốc",
        ///         "drugForm": "Viên nén",
        ///         "drugTypeId": 2
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Created new drug</response>
        /// <response code="400">Field is not matched or duplicated. Drug type id is not exist.</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<DrugVM>> CreateDrug([FromBody] DrugCM model)
        {
            DrugType drugType = await _drugTypeService.GetByIdAsync(model.DrugTypeId);
            if(drugType == null)
            {
                return BadRequest(new
                {
                    message = "Drug Type is not exist."
                });
            }
            Drug drug = _mapper.Map<Drug>(model);
            try
            {
                drug.DrugType = drugType;
                Drug createdDrug = await _drugService.AddAsync(drug);

                if (createdDrug != null)
                {
                    return CreatedAtAction("GetDrugById", new { id = createdDrug.Id }, _mapper.Map<DrugVM>(createdDrug));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete drug
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
        public async Task<ActionResult> DeleteDrug([FromRoute] int id)
        {
            Drug currentDrug = await _drugService.GetByIdAsync(id);
            if (currentDrug == null)
            {
                return NotFound();
            }

            try
            {
                bool isDeleted = await _drugService.DeleteAsync(currentDrug);
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
        /// Update a drug
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT 
        ///     {
        ///         "id": 10,
        ///         "name": "Paradon",    
        ///         "producer": "Công ty synopharm",    
        ///         "drugOrigin": "Trung quốc",
        ///         "drugForm": "Viên nén",
        ///         "drugTypeId": 2
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<DrugVM>> UpdateDrug(int id,[FromBody] DrugUM model)
        {
            if(id != model.Id)
            {
                return BadRequest();
            }
            DrugType drugType = await _drugTypeService.GetByIdAsync(model.DrugTypeId);
            if (drugType == null)
            {
                return BadRequest(new
                {
                    message = "Drug Type is not exist."
                });
            }

            Drug drug = await _drugService.GetByIdAsync(model.Id);
            if(drug == null)
            {
                return BadRequest(new
                {
                    message = "Drug is not exist."
                });
            }
            try
            {
                drug.DrugForm = model.DrugForm;
                drug.DrugOrigin = model.DrugOrigin;
                drug.DrugTypeId = model.DrugTypeId;
                drug.Name = model.Name;
                drug.Producer = model.Producer;
                drug.DrugType = drugType;
                bool isSuccess = await _drugService.UpdateAsync(drug);
                if (isSuccess)
                {
                    return Ok(_mapper.Map<DrugVM>(drug));
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
