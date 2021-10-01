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
    [Route("api/v1/patients")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Patient> _pagingSupport;

        public PatientController(IPatientService patientService, IMapper mapper, IPagingSupport<Patient> pagingSupport)
        {
            _patientService = patientService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list patients
        /// </summary>
        /// <returns>List patients</returns>
        /// <response code="200">Returns list patients</response>
        /// <response code="404">Not found patients</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<PatientVM>> GetAllPatients(
            [FromQuery(Name = "email")] string email,
            [FromQuery(Name = "background-disease")] string backgroundDisease,
            [FromQuery(Name = "allergy")] string allergy,
            [FromQuery(Name = "blood-group")] string bloodGroup,
            [FromQuery(Name = "field-by")] PatientFieldEnum fieldBy,
            [FromQuery(Name = "sort-by")] SortTypeEnum sortBy,
            [FromQuery(Name = "filtering")] string filters = null,
            int limit = 50,
            int pageOffset = 1
        )
        {
            try
            {
                IQueryable<Patient> patientList = _patientService.GetAll();
                if (!string.IsNullOrEmpty(email))
                {
                    patientList = patientList.Where(s => s.Email.Trim().ToUpper().Contains(email.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(backgroundDisease))
                {
                    patientList = patientList.Where(s => s.BackgroundDisease.Trim().ToUpper().Contains(backgroundDisease.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(allergy))
                {
                    patientList = patientList.Where(s => s.Allergy.Trim().ToUpper().Contains(allergy.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(bloodGroup))
                {
                    patientList = patientList.Where(s => s.BloodGroup.Trim().ToUpper().Contains(bloodGroup.Trim().ToUpper()));
                }
                Paged<PatientVM> paged = null;
                if (sortBy == SortTypeEnum.asc && typeof(PatientVM).GetProperty(fieldBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(patientList).GetRange(pageOffset, limit, p => EF.Property<object>(p, fieldBy.ToString()), 0).Paginate<PatientVM>();
                }
                else if (sortBy == SortTypeEnum.desc && typeof(PatientVM).GetProperty(fieldBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(patientList).GetRange(pageOffset, limit, p => EF.Property<object>(p, fieldBy.ToString()), 1).Paginate<PatientVM>();
                }
                else
                {
                    paged = _pagingSupport.From(patientList).GetRange(pageOffset, limit, s => s.Id, 1).Paginate<PatientVM>();
                }
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(PatientVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(PatientVM), splitFilter, paged);
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
        /// Get a specific patient by patient id
        /// </summary>
        /// <returns>Return the patient with the corresponding id</returns>
        /// <response code="200">Returns patient with the specified id</response>
        /// <response code="404">No patient found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public ActionResult GetPatientById(int id)
        {
            try
            {
                IQueryable<Patient> patientList = _patientService.GetAll(s => s.HealthChecks);
                Patient currentPatient = patientList.Where(s => s.Id == id).FirstOrDefault();
                if (currentPatient != null)
                {
                    return Ok(_mapper.Map<PatientVM>(currentPatient));
                }
                return NotFound("Can not found patient by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new patient
        /// </summary>
        /// <response code="200">Created new patient successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<PatientVM>> CreateNewPatient([FromBody] PatientCM model)
        {
            try
            {
                Patient checkExistedDoctorWithEmail = _patientService.GetAll().Where(s => s.Email.Trim().ToUpper().Equals(model.Email.Trim().ToUpper())).FirstOrDefault();
                if (checkExistedDoctorWithEmail != null)
                {
                    return BadRequest(new
                    {
                        message = "Email have been registered!"
                    });
                }
                model.Email = model.Email.Trim();
                Patient patientCreated = await _patientService.AddAsync(_mapper.Map<Patient>(model));
                if (patientCreated != null)
                {
                    return CreatedAtAction("GetPatientById", new { id = patientCreated.Id }, _mapper.Map<PatientVM>(patientCreated));
                }
                return BadRequest(new
                {
                    message = "Create patient failed!"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete Patient By Id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                Patient currentPatient = await _patientService.GetByIdAsync(id);
                if (currentPatient == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found patient by id: " + id
                    });
                }
                bool isDeleted = await _patientService.DeleteAsync(currentPatient);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
                }
                return BadRequest(new
                {
                    message = "Delete patient failed!"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a patient
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<PatientVM>> PutPatient(int id, [FromBody] PatientUM model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest();
                }
                Patient currentPatient = await _patientService.GetByIdAsync(model.Id);
                if (currentPatient == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found patient by id: " + id
                    });
                }
                currentPatient.BackgroundDisease = model.BackgroundDisease.Trim();
                currentPatient.Allergy = model.Allergy.Trim();
                currentPatient.BloodGroup = model.BloodGroup.Trim();
                bool isUpdated = await _patientService.UpdateAsync(currentPatient);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<PatientVM>(currentPatient));
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
