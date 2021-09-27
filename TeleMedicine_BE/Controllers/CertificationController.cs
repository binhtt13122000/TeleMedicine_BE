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
    [Route("api/v1/certifications")]
    [ApiController]
    public class CertificationController : Controller
    {
        private readonly ICertificationService _certificationService;
        private IMapper _mapper;
        private IPagingSupport<Certification> _pagingSupport;

        public CertificationController(ICertificationService certificationService, IMapper mapper, IPagingSupport<Certification> pagingSupport)
        {
            _certificationService = certificationService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get all certifications
        /// </summary>
        /// <returns>All certifications</returns>
        /// <response code="200">Returns all certifications</response>
        /// <response code="404">Not found certifications</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<CertificationVM>> GetAllCertifications(
            [FromQuery(Name = "name")] string name,
            int offset = 1,
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
                Paged<CertificationVM> paged = _pagingSupport.From(certificationList).GetRange(offset, limit, s => s.Id, 1).Paginate<CertificationVM>();
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
        /// <response code="200">Created new certification successfull</response>
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
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<CertificationVM>> PutCertification(int id,[FromBody] CertificationUM model)
        {
            Certification currentCertification = await _certificationService.GetByIdAsync(model.Id);
            if(id != model.Id)
            {
                return BadRequest();
            }
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
                return BadRequest(new
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
