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
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Patient> _pagingSupport;

        public PatientController(IPatientService patientService, IMapper mapper, IAccountService accountService, IPagingSupport<Patient> pagingSupport)
        {
            _patientService = patientService;
            _accountService = accountService;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<IEnumerable<PatientVM>> GetAllPatients(
            [FromQuery(Name = "email")] string email,
            [FromQuery(Name = "background-disease")] string backgroundDisease,
            [FromQuery(Name = "allergy")] string allergy,
            [FromQuery(Name = "blood-group")] string bloodGroup,
            [FromQuery(Name = "is-active")] bool? isActive,
            [FromQuery(Name = "order-by")] PatientFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "filtering")] string filters = null,
            int limit = 50,
            [FromQuery(Name = "page-offset")]  int pageOffset = 1
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
                if(isActive.HasValue)
                {
                    patientList = patientList.Where(s => s.IsActive.Value.Equals(isActive.Value));
                }
                Paged<PatientVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(PatientVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(patientList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0).Paginate<PatientVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(PatientVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(patientList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1).Paginate<PatientVM>();
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
                        string json = jsonIgnore.JsonIgnore(typeof(PatientVM), splitFilter, paged, PropertyRenameAndIgnoreSerializerContractResolver.IgnoreMode.EXCEPT);
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
        /// Get a specific patient by type
        /// </summary>
        /// <returns>Return the patient with the type</returns>
        /// <response code="200">Returns the patient with the type</response>
        /// <response code="404">No patient found with the type</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{search}")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<PatientVM> GetPatientByType([FromRoute] string search, [FromQuery(Name = "search-type")] SearchType searchType)
        {
            try
            {
                Patient currentPatient = null;
                if (searchType == SearchType.Id)
                {
                    try
                    {
                        int result = Int32.Parse(search);
                        IQueryable<Patient> patientList = _patientService.GetAll(s => s.HealthChecks);
                        currentPatient = patientList.Where(s => s.Id == result).FirstOrDefault();
                    }
                    catch (FormatException)
                    {
                        return BadRequest();
                    }
                }
                else if (searchType == SearchType.Email && !string.IsNullOrEmpty(search))
                {
                    currentPatient = currentPatient = _patientService.GetPatientByEmail(search.Trim());
                }
                if (currentPatient != null)
                {
                    return Ok(_mapper.Map<PatientVM>(currentPatient));
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new patient
        /// </summary>
        /// <response code="201">Created new patient successfull</response>
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

                Account currentAccount = _accountService.GetAccountByEmail(model.Email);
                Patient convertPatient = _mapper.Map<Patient>(model);
                if(currentAccount != null)
                {
                    convertPatient.Avatar = currentAccount.Avatar;
                    convertPatient.Name = currentAccount.FirstName.Trim() + " " + currentAccount.LastName.Trim();
                }

                convertPatient.Email = model.Email.Trim();
                Patient patientCreated = await _patientService.AddAsync(convertPatient);
                if (patientCreated != null)
                {
                    return CreatedAtAction("GetPatientByType", new { search = patientCreated.Id }, _mapper.Map<PatientVM>(patientCreated));
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2, 3")]
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
                currentPatient.IsActive = false;
                bool isDeleted = await _patientService.UpdateAsync(currentPatient);
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
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2, 3")]
        public async Task<ActionResult<PatientVM>> PutPatient([FromBody] PatientUM model)
        {
            try
            {
                Patient currentPatient = await _patientService.GetByIdAsync(model.Id);
                if (currentPatient == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found patient by id: " + model.Id
                    });
                }
                Account currentAccount = _accountService.GetAccountByEmail(currentPatient.Email);
                Patient convertPatient = _mapper.Map<Patient>(model);
                if (currentAccount != null)
                {
                    currentPatient.Avatar = currentAccount.Avatar;
                    currentPatient.Name = currentAccount.FirstName.Trim() + " " + currentAccount.LastName.Trim();
                }
                currentPatient.BackgroundDisease = model.BackgroundDisease.Trim();
                currentPatient.Allergy = model.Allergy.Trim();
                currentPatient.BloodGroup = model.BloodGroup.Trim();
                currentPatient.IsActive = model.IsActive;
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
