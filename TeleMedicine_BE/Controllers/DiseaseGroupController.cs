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
    [Route("api/v1/disease-groups")]
    [ApiController]
    public class DiseaseGroupController : Controller
    {
        private readonly IDiseaseGroupService _diseaseGroupService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<DiseaseGroup> _pagingSupport;

        public DiseaseGroupController(IDiseaseGroupService diseaseGroupService, IMapper mapper, IPagingSupport<DiseaseGroup> pagingSupport)
        {
            _diseaseGroupService = diseaseGroupService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get all disease groups
        /// </summary>
        /// <returns>All majors</returns>
        /// <response code="200">Returns all disease groups</response>
        /// <response code="404">Not found disease groups</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<DiseaseGroupVM>> GetAllDiseaseGroups(
            [FromQuery(Name = "group-name")] String groupName,
            int offset = 1,
            int limit = 20
            )
        {
            try
            {
                IQueryable<DiseaseGroup> diseaseGroups = _diseaseGroupService.GetAll();
                if (!String.IsNullOrEmpty(groupName))
                {
                    diseaseGroups = diseaseGroups.Where(s => s.GroupName.ToUpper().Contains(groupName.Trim().ToUpper()));
                }
                Paged<DiseaseGroupVM> paged = _pagingSupport.From(diseaseGroups).GetRange(offset, limit, s => s.Id, 1).Paginate<DiseaseGroupVM>();
                return Ok(paged);
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get disease group by id
        /// </summary>
        /// <returns>Return the disease group with the corresponding id</returns>
        /// <response code="200">Returns the disease group type with the specified id</response>
        /// <response code="404">No disease group found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiseaseGroupVM>> GetDiseaseGroupById(int id)
        {
            try
            {
                DiseaseGroup currentDiseaseGroup = await _diseaseGroupService.GetByIdAsync(id);
                if(currentDiseaseGroup != null)
                {
                    return Ok(_mapper.Map<DiseaseGroupVM>(currentDiseaseGroup));
                }
                return NotFound("Can not found disease group by id: " + id);
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new disease group
        /// </summary>
        /// <response code="200">Created new disease group successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DiseaseGroupCM>> CreateDiseaseGroup([FromBody] DiseaseGroupCM model)
        {
            DiseaseGroup diseaseGroup = _mapper.Map<DiseaseGroup>(model);
            try
            {
                DiseaseGroup diseaseGroupCreated = await _diseaseGroupService.AddAsync(diseaseGroup);
                if(diseaseGroupCreated != null)
                {
                    return CreatedAtAction("GetDiseaseGroupById", new { id = diseaseGroupCreated.Id }, _mapper.Map<DiseaseGroupVM>(diseaseGroupCreated));
                }
                return BadRequest();
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a disease group
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
        public async Task<ActionResult> PutDiseaseGroup(int id, [FromBody] DiseaseGroupUM model)
        {
            DiseaseGroup currentDiseaseGroup= await _diseaseGroupService.GetByIdAsync(id);
            if (currentDiseaseGroup == null)
            {
                return NotFound();
            }
            if (currentDiseaseGroup.Id != model.Id)
            {
                return BadRequest();
            }
            try
            {
                currentDiseaseGroup.GroupName = model.GroupName.Trim();
                bool isUpdated = await _diseaseGroupService.UpdateAsync(currentDiseaseGroup);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<DiseaseGroupVM>(currentDiseaseGroup));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete disease group By Id
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
            DiseaseGroup currentDiseaseGroup = await _diseaseGroupService.GetByIdAsync(id);
            if (currentDiseaseGroup == null)
            {
                return BadRequest(new
                {
                    message = "Can not found disease group by id: " + id
                });
            }
            try
            {
                bool isDeleted = await _diseaseGroupService.DeleteAsync(currentDiseaseGroup);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
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
