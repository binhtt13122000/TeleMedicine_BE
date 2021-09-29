using AutoMapper;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleMedicine_BE.Utils;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/diseases")]
    [ApiController]
    public class DiseaseController : Controller
    {
        private readonly IDiseaseService _diseaseService;
        private readonly IDiseaseGroupService _diseaseGroupService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Disease> _pagingSupport;

        public DiseaseController(IDiseaseService diseaseService, IDiseaseGroupService diseaseGroupService , IMapper mapper, IPagingSupport<Disease> pagingSupport)
        {
            _diseaseService = diseaseService;
            _diseaseGroupService = diseaseGroupService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list diseases
        /// </summary>
        /// <returns>List diseases</returns>
        /// <response code="200">Returns list diseases groups</response>
        /// <response code="404">Not found diseases</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<DiseaseVM>> GetDiseases(
            [FromQuery(Name = "disease-code")] string diseaseCode,
            [FromQuery(Name = "name")] string name,
            [FromQuery(Name = "description")] string description,
            [FromQuery(Name = "disease-type")] int[] diseaseTypeIds,
            [FromQuery(Name = "filtering")] string filters = null,
            int offset = 1,
            int limit = 20
            )
        {
            try
            {
                IQueryable<Disease> diseasesQuery = _diseaseService.GetAll(_ => _.DiseaseGroup);
                if (!string.IsNullOrWhiteSpace(diseaseCode))
                {
                    diseasesQuery = diseasesQuery.Where(_ => _.DiseaseCode.ToUpper().Contains(diseaseCode.Trim().ToUpper()));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    diseasesQuery = diseasesQuery.Where(_ => _.Name.ToUpper().Contains(name.Trim().ToUpper()));
                }
                if (!string.IsNullOrWhiteSpace(description))
                {
                    diseasesQuery = diseasesQuery.Where(_ => _.Description.ToUpper().Contains(description.Trim().ToUpper()));
                }
                if (diseaseTypeIds != null && diseaseTypeIds.Length > 0)
                {
                    diseasesQuery = diseasesQuery.Where(_ => diseaseTypeIds.Contains(_.DiseaseGroupId));
                }
                Paged<DiseaseVM> result = _pagingSupport.From(diseasesQuery)
                   .GetRange(offset, limit, s => s.Id, 1)
                   .Paginate<DiseaseVM>();
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(DiseaseVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(DiseaseVM), splitFilter, result);
                        return Ok(JsonConvert.DeserializeObject(json));
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get a specific disease by disease id
        /// </summary>
        /// <returns>Return the disease with the corresponding id</returns>
        /// <response code="200">Returns the disease type with the specified id</response>
        /// <response code="404">No disease found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public ActionResult<DiseaseVM> GetDiseaseById([FromRoute] int id)
        {
            try
            {
                Disease disease = _diseaseService.GetAll(_ => _.DiseaseGroup).FirstOrDefault(_ => _.Id == id);
                if(disease == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<DiseaseVM>(disease));
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new disease
        /// </summary>
        /// <response code="201">Created new disease</response>
        /// <response code="400">Field is not matched or duplicated.</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<DiseaseCM>> CreateDisease([FromBody] DiseaseCM model)
        {
            DiseaseGroup diseaseGroup = await _diseaseGroupService.GetByIdAsync(model.DiseaseGroupId);
            if (diseaseGroup == null)
            {
                return BadRequest(new
                {
                    message = "Disease Group is not exist."
                });
            }
            Disease disease = _mapper.Map<Disease>(model);
            try
            {
                Disease checkExistedDisease = _diseaseService.GetAll().Where(s => s.DiseaseCode == disease.DiseaseCode).FirstOrDefault();
                if (checkExistedDisease != null)
                {
                    return BadRequest(new
                    {
                        message = "Disease Code have been existed!"
                    });
                }
                disease.DiseaseGroup = diseaseGroup;
                Disease createdDisease = await _diseaseService.AddAsync(disease);

                if (createdDisease != null)
                {
                    return CreatedAtAction("GetDiseaseById", new { id = createdDisease.Id }, _mapper.Map<DiseaseVM>(createdDisease));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete disease
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
        public async Task<ActionResult> DeleteDisease([FromRoute] int id)
        {
            Disease currentDisease = await _diseaseService.GetByIdAsync(id);
            if (currentDisease == null)
            {
                return NotFound();
            }

            try
            {
                bool isDeleted = await _diseaseService.DeleteAsync(currentDisease);
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
        /// Update a disease
        /// </summary>
        /// /// <remarks>
        /// Sample request:
        ///
        ///     PUT 
        ///     {
        ///         "id": 10,
        ///         "diseaseCode": "C32.9",    
        ///         "name": "U ác của thanh quản, không xác định",    
        ///         "description": "Trung quốc",
        ///         "diseaseGroupId": 2
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
        public async Task<ActionResult<DiseaseVM>> UpdateDisease(int id, [FromBody] DiseaseUM model)
        {
            DiseaseGroup diseaseGroup = await _diseaseGroupService.GetByIdAsync(model.DiseaseGroupId);
            if (diseaseGroup == null)
            {
                return BadRequest(new
                {
                    message = "Disease Group is not exist."
                });
            }

            Disease disease = await _diseaseService.GetByIdAsync(model.Id);
            if (id != model.Id)
            {
                return BadRequest();
            }
            if (disease == null)
            {
                return BadRequest(new
                {
                    message = "Disease is not exist."
                });
            }
            try
            {
                if(!disease.DiseaseCode.ToUpper().Equals(model.DiseaseCode.Trim().ToUpper()) && _diseaseService.GetAll().Where(s => s.DiseaseCode.Trim().ToUpper().Equals(model.DiseaseCode.Trim().ToUpper())).FirstOrDefault() != null)
                {
                    return BadRequest(new
                    {
                        message = "Disease Code have been existed!"
                    });
                }
                disease.DiseaseCode = model.DiseaseCode.Trim().ToUpper();
                disease.Name = model.Name.Trim();
                disease.Description = model.Description;
                disease.DiseaseGroup = diseaseGroup;
                disease.DiseaseGroupId = model.DiseaseGroupId;
                bool isSuccess = await _diseaseService.UpdateAsync(disease);
                if (isSuccess)
                {
                    return Ok(_mapper.Map<DiseaseVM>(disease));
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
