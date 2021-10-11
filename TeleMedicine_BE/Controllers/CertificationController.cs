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
    [Route("api/v1/certifications")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CertificationController : Controller
    {
        private readonly ICertificationService _certificationService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Certification> _pagingSupport;

        public CertificationController(ICertificationService certificationService, IMapper mapper, IPagingSupport<Certification> pagingSupport)
        {
            _certificationService = certificationService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list certifications
        /// </summary>
        /// <returns>List certifications</returns>
        /// <response code="200">Returns list certifications</response>
        /// <response code="404">Not found certifications</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<CertificationVM>> GetAllCertifications(
            [FromQuery(Name = "name")] string name,
            [FromQuery(Name = "is-active")] bool? isActive,
            [FromQuery(Name = "order-by")] CertificationFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "page-offset")]  int pageOffset = 1,
            int limit = 20
        )
        {
            try
            {
                IQueryable<Certification> certificationList = _certificationService.GetAll();
                if (!String.IsNullOrEmpty(name))
                {
                    certificationList = certificationList.Where(s => s.Name.ToUpper().Contains(name.Trim().ToUpper()));
                }
                if(isActive.HasValue)
                {
                    certificationList = certificationList.Where(s => s.IsActive.Value.Equals(isActive.Value));
                }
                Paged<CertificationVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(CertificationVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(certificationList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0).Paginate<CertificationVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(CertificationVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(certificationList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1).Paginate<CertificationVM>();
                }
                else
                {
                    paged = _pagingSupport.From(certificationList).GetRange(pageOffset, limit, s => s.Id, 1).Paginate<CertificationVM>();
                }
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(CertificationVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(CertificationVM), splitFilter, paged);
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
        /// Get certification by id
        /// </summary>
        /// <returns>Return the certification with the corresponding id</returns>
        /// <response code="200">Returns the certification type with the specified id</response>
        /// <response code="404">No certification found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<CertificationVM>> GetCertificationById(int id)
        {
            try
            {
                Certification currentCertification = await _certificationService.GetByIdAsync(id);
                if (currentCertification != null)
                {
                    return Ok(_mapper.Map<CertificationVM>(currentCertification));
                }
                return NotFound("Can not found certification by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new certification
        /// </summary>
        /// <response code="201">Created new certification successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<CertificationCM>> CreateCertification([FromBody] CertificationCM model)
        {
            Certification certification = _mapper.Map<Certification>(model);
            try
            {
                Certification certificationCreated = await _certificationService.AddAsync(certification);
                if (certificationCreated != null)
                {
                    return CreatedAtAction("GetCertificationById", new { id = certificationCreated.Id }, _mapper.Map<CertificationVM>(certificationCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a certification
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<CertificationVM>> PutCertification([FromBody] CertificationUM model)
        {
            Certification currentCertification = await _certificationService.GetByIdAsync(model.Id);
            if (currentCertification == null)
            {
                return NotFound();
            }
            if (currentCertification.Id != model.Id)
            {
                return BadRequest();
            }
            try
            {
                currentCertification.Name = model.Name;
                currentCertification.Description = model.Description;
                bool isUpdated = await _certificationService.UpdateAsync(currentCertification);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<CertificationVM>(currentCertification));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete certification By Id
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
            Certification currentCertification = await _certificationService.GetByIdAsync(id);
            if (currentCertification == null)
            {
                return NotFound(new
                {
                    message = "Can not found certification by id: " + id
                });
            }
            try
            {
                bool isDeleted = await _certificationService.DeleteAsync(currentCertification);
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
