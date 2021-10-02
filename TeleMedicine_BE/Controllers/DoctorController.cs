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
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TeleMedicine_BE.Utils;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/doctors")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IHospitalService _hospitalService;
        private readonly IMajorService _majorService;
        private readonly ICertificationService _certificationService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Doctor> _pagingSupport;

        public DoctorController(IDoctorService doctorService, IHospitalService hospitalService, IMajorService majorService, ICertificationService certificationService, IMapper mapper, IPagingSupport<Doctor> pagingSupport)
        {
            _doctorService = doctorService;
            _hospitalService = hospitalService;
            _majorService = majorService;
            _certificationService = certificationService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list doctors
        /// </summary>
        /// <returns>List doctors</returns>
        /// <response code="200">Returns list doctors</response>
        /// <response code="404">Not found doctors</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<DoctorVM>> GetAllDoctors(
            [FromQuery(Name = "email")] string email,
            [FromQuery(Name = "practising-certificate")] string practisingCertificate,
            [FromQuery(Name = "certificate-code")] string certificateCode,
            [FromQuery(Name = "place-certificate")] string placeCertificate,
            [FromQuery(Name = "date-start-certificate")] DateTime? dateStartCertificate,
            [FromQuery(Name = "date-end-certificate")] DateTime? dateEndCertificate,
            [FromQuery(Name = "scope-certificate")] string scopeCertificate,
            [FromQuery(Name = "number-start-consultants")] int numberStartConsultants,
            [FromQuery(Name = "number-end-consultants")] int numberEndConsultants,
            [FromQuery(Name = "major")] int[] majorId,
            [FromQuery(Name = "start-rating")] int startRating,
            [FromQuery(Name = "end-rating")] int endRating,
            [FromQuery(Name = "order-by")] DoctorFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "is-verify")] int isVerify = 0,
            [FromQuery(Name = "filtering")] string filters = null,
            int limit = 50,
            int pageOffset = 1
        )
        {
            try
            {
                IQueryable<Doctor> doctorList = _doctorService.GetAll(s => s.HospitalDoctors, s => s.MajorDoctors, s => s.CertificationDoctors);
                if (!String.IsNullOrEmpty(email))
                {
                    doctorList = doctorList.Where(s => s.Email.ToUpper().Contains(email.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(practisingCertificate))
                {
                    doctorList = doctorList.Where(s => s.PractisingCertificate.ToUpper().Contains(practisingCertificate.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(certificateCode))
                {
                    doctorList = doctorList.Where(s => s.CertificateCode.ToUpper().Contains(certificateCode.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(placeCertificate))
                {
                    doctorList = doctorList.Where(s => s.PlaceOfCertificate.ToUpper().Contains(placeCertificate.Trim().ToUpper()));
                }
                if (dateStartCertificate.HasValue && dateEndCertificate.HasValue)
                {
                    doctorList = doctorList.Where(s => s.DateOfCertificate.Date.CompareTo(dateStartCertificate.Value.Date) >= 0).
                                            Where(s => s.DateOfCertificate.Date.CompareTo(dateEndCertificate.Value.Date) <= 0);
                }
                else
                {
                    if (dateStartCertificate.HasValue)
                    {
                        doctorList = doctorList.Where(s => s.DateOfCertificate.Date.CompareTo(dateStartCertificate.Value.Date) >= 0);
                    }
                    if (dateEndCertificate.HasValue)
                    {
                        doctorList = doctorList.Where(s => s.DateOfCertificate.Date.CompareTo(dateEndCertificate.Value.Date) <= 0);
                    }
                }
                if (!String.IsNullOrEmpty(scopeCertificate))
                {
                    doctorList = doctorList.Where(s => s.ScopeOfPractice.ToUpper().Contains(scopeCertificate.Trim().ToUpper()));
                }
                if(numberStartConsultants != 0 && numberEndConsultants != 0)
                {
                    doctorList = doctorList.Where(s => s.NumberOfConsultants >= numberStartConsultants).Where(s => s.NumberOfConsultants <= numberEndConsultants);
                }
                else
                {
                    if(numberStartConsultants != 0)
                    {
                        doctorList = doctorList.Where(s => s.NumberOfConsultants >= numberStartConsultants);
                    }
                    if(numberEndConsultants != 0)
                    {
                        doctorList = doctorList.Where(s => s.NumberOfConsultants <= numberEndConsultants);
                    }
                }
                
                if(startRating != 0 && endRating != 0)
                {
                    doctorList = doctorList.Where(s => s.Rating >= startRating).Where(s => s.Rating <= endRating);
                }
                else
                {
                    if(startRating != 0)
                    {
                        doctorList = doctorList.Where(s => s.Rating >= startRating);
                    }
                    if(endRating != 0)
                    {
                        doctorList = doctorList.Where(s => s.Rating <= endRating);
                    }
                }

                if(isVerify != 0)
                {
                    if(isVerify == 1)
                    {
                        doctorList = doctorList.Where(s => s.IsVerify == true);
                    }
                    if(isVerify == -1)
                    {
                        doctorList = doctorList.Where(s => s.IsVerify == false);
                    }
                }
                if (majorId != null && majorId.Length > 0)
                {
                    foreach (int major in majorId)
                    {
                        Major selectMajor = _majorService.GetAll(s => s.MajorDoctors).Where(s => s.Id == major).FirstOrDefault();
                        if (selectMajor != null)
                        {
                            List<MajorDoctor> listMajorDoctor = selectMajor.MajorDoctors.ToList();
                            foreach (MajorDoctor majorDoctor in listMajorDoctor)
                            {
                                doctorList = doctorList.Where(s => s.MajorDoctors.Count > 0 && s.MajorDoctors.Any(s => s.MajorId == majorDoctor.MajorId));
                            }
                        }
                    }
                }
                
                Paged<DoctorVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(DoctorVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(doctorList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0).Paginate<DoctorVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(DoctorVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(doctorList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1).Paginate<DoctorVM>();
                }else
                {
                    paged = _pagingSupport.From(doctorList).GetRange(pageOffset, limit, s => s.Id, 1).Paginate<DoctorVM>();
                }
                
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(DoctorVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(DoctorVM), splitFilter, paged);
                        return Ok(JsonConvert.DeserializeObject(json));
                    }
                }
                return Ok(paged);
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get a specific doctor by email
        /// </summary>
        /// <returns>Return the doctor with the corresponding email</returns>
        /// <response code="200">Returns the doctor with the specified email</response>
        /// <response code="404">No doctor found with the specified email</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("email/{email}")]
        [Produces("application/json")]
        public ActionResult<DoctorVM> GetDoctorByEmail([FromRoute] string email)
        {
            try
            {
                Doctor currentDoctor = _doctorService.GetDoctorByEmail(email);
                if (currentDoctor == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found doctor by email"
                    });
                }
                return Ok(_mapper.Map<DoctorVM>(currentDoctor));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        /// <summary>
        /// Get a specific doctor by doctor id
        /// </summary>
        /// <returns>Return the doctor with the corresponding id</returns>
        /// <response code="200">Returns doctor with the specified id</response>
        /// <response code="404">No doctor found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public  ActionResult GetDoctorById(int id)
        {
            try
            {
                IQueryable<Doctor> doctorList = _doctorService.GetAll(s => s.HospitalDoctors, s => s.CertificationDoctors, s => s.MajorDoctors);
                Doctor doctor = doctorList.Where(s => s.Id == id).FirstOrDefault();

                if (doctor == null)
                {
                    return NotFound("Can not found doctor by id: " + id);
                }
                return Ok(_mapper.Map<DoctorVM>(doctor));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Update a doctor
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<DoctorVM>> PutDoctor(int id, [FromBody] DoctorUM model)
        {
            Doctor currentDoctor = await _doctorService.GetByIdAsync(model.Id);
            if (id != model.Id)
            {
                return BadRequest();
            }
            if (currentDoctor == null)
            {
                return NotFound();
            }
            if (currentDoctor.Id != model.Id)
            {
                return BadRequest();
            }
            Doctor checkExistedCertification = _doctorService.GetAll().Where(s => s.CertificateCode.Trim().ToUpper().Equals(model.CertificateCode.Trim().ToUpper())).FirstOrDefault();
            if (!currentDoctor.CertificateCode.Trim().ToUpper().Equals(model.CertificateCode.Trim().ToUpper()) && checkExistedCertification != null)
            {
                return BadRequest(new
                {
                    message = "Certification Code have been registered account!"
                }
                    );
            }
            List<CertificationDoctor> convertCetification = new List<CertificationDoctor>();
            List<CertificationDoctorWithRegisterCM> certificationDoctors = model.CertificationDoctors.ToList();
            if(certificationDoctors != null && certificationDoctors.Count > 0)
            {
                foreach(CertificationDoctorWithRegisterCM item in certificationDoctors)
                {
                    convertCetification.Add(_mapper.Map<CertificationDoctor>(item));
                }
            }

            List<MajorDoctor> convertMajor = new List<MajorDoctor>();
            List<MajorDoctorWithRegisterCM> majorDoctors = model.MajorDoctors.ToList();
            if (majorDoctors != null && majorDoctors.Count > 0)
            {
                foreach (MajorDoctorWithRegisterCM item in majorDoctors)
                {
                    convertMajor.Add(_mapper.Map<MajorDoctor>(item));
                }
            }

            List<HospitalDoctor> convertHospital = new List<HospitalDoctor>();
            List<HospitalDoctorWithRegisterCM> hospitalDoctors = model.HospitalDoctors.ToList();
            if (hospitalDoctors != null && hospitalDoctors.Count > 0)
            {
                foreach (HospitalDoctorWithRegisterCM item in hospitalDoctors)
                {
                    convertHospital.Add(_mapper.Map<HospitalDoctor>(item));
                }
            }
            try
            {
                if(convertCetification != null && convertCetification.Count > 0)
                {
                    currentDoctor.CertificationDoctors = convertCetification;
                }
                if (convertMajor != null && convertMajor.Count > 0)
                {
                    currentDoctor.MajorDoctors = convertMajor;
                }
                if (convertHospital != null && convertHospital.Count > 0)
                {
                    currentDoctor.HospitalDoctors = convertHospital;
                }
                currentDoctor.PractisingCertificate = model.PractisingCertificate.Trim();
                currentDoctor.CertificateCode = model.CertificateCode.Trim().ToUpper();
                currentDoctor.PlaceOfCertificate = model.PlaceOfCertificate.Trim();
                currentDoctor.DateOfCertificate = model.DateOfCertificate;
                currentDoctor.ScopeOfPractice = model.ScopeOfPractice.Trim();
                currentDoctor.Description = model.Description.Trim();
                bool isUpdated = await _doctorService.UpdateAsync(currentDoctor);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<DoctorVM>(currentDoctor));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new doctor
        /// </summary>
        /// <response code="200">Created new doctor successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<DoctorVM>> CreateNewDoctor([FromBody] DoctorCM model)
        {
            HospitalDoctorWithRegisterCM[] arrHospital = new HospitalDoctorWithRegisterCM[model.HospitalDoctors.Count];
            model.HospitalDoctors.CopyTo(arrHospital, 0);

            MajorDoctorWithRegisterCM[] arrMajor = new MajorDoctorWithRegisterCM[model.MajorDoctors.Count];
            model.MajorDoctors.CopyTo(arrMajor, 0);

            CertificationDoctorWithRegisterCM[] arrCertification = new CertificationDoctorWithRegisterCM[model.CertificationDoctors.Count];
            model.CertificationDoctors.CopyTo(arrCertification, 0);
            try
            {
                Doctor checkExistedDoctorWithEmail = _doctorService.GetAll().Where(s => s.Email.Trim().ToUpper().Equals(model.Email.Trim().ToUpper())).FirstOrDefault();
                if (checkExistedDoctorWithEmail != null)
                {
                    return BadRequest(new
                    {
                        message = "Email have been registered!"
                    });
                }
                Doctor checkExistedDoctorWithCertification = _doctorService.GetAll().Where(s => s.CertificateCode.Trim().ToUpper().Equals(model.CertificateCode.Trim().ToUpper())).FirstOrDefault();
                if (checkExistedDoctorWithCertification != null)
                {
                    return BadRequest(new
                        {
                            message = "Certification Code have been registered account!"
                        }
                        );
                }
                if(arrHospital != null && arrHospital.Length > 0)
                {
                    for(int i = 0; i < arrHospital.Length; i++)
                    {
                        if(_hospitalService.GetAll().Where(s => s.Id == arrHospital[i].HospitalId).FirstOrDefault() == null)
                        {
                            return BadRequest(new
                            {
                                message = "Hospital is not existed!"
                            });
                        }
                    }
                }
                if(arrMajor != null && arrMajor.Length > 0)
                {
                    for(int i = 0; i < arrMajor.Length; i++)
                    {
                        if(_majorService.GetAll().Where(s => s.Id == arrMajor[i].MajorId).FirstOrDefault() == null)
                        {
                            return BadRequest(new
                            {
                                message = "Major is not existed!"
                            });
                        }
                    }
                }
                if(arrCertification != null && arrCertification.Length > 0)
                {
                    for (int i = 0; i < arrCertification.Length; i++)
                    {
                        if (_certificationService.GetAll().Where(s => s.Id == arrCertification[i].CertificationId).FirstOrDefault() == null)
                        {
                            return BadRequest(new
                            {
                                message = "Certification is not existed!"
                            });
                        }
                    }
                }
                model.CertificateCode = model.CertificateCode.Trim().ToUpper();
                Doctor doctorCreated = await _doctorService.AddAsync(_mapper.Map<Doctor>(model));
                if(doctorCreated != null)
                {
                    IQueryable<Doctor> doctorList = _doctorService.GetAll(s => s.HospitalDoctors, s => s.CertificationDoctors, s => s.MajorDoctors);
                    Doctor doctor = doctorList.Where(s => s.Id == doctorCreated.Id).FirstOrDefault();
                    return Ok(_mapper.Map<DoctorVM>(doctor));
                }
                return BadRequest(new
                {
                    message = "Create doctor failed!"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete Doctor By Id
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
                Doctor currentDoctor = await _doctorService.GetByIdAsync(id);
                if (currentDoctor == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found doctor by id: " + id
                    });
                }
                currentDoctor.IsVerify = false;
                bool isDeleted = await _doctorService.UpdateAsync(currentDoctor);
                if (isDeleted)
                {
                    return Ok(new { 
                        message = "Success"
                    });
                }
                return BadRequest();
            }catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Verify Doctor By Id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> VerifyDoctorById(int id)
        {
            try
            {
                Doctor currentDoctor = await _doctorService.GetByIdAsync(id);
                if (currentDoctor == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found doctor by id: " + id
                    });
                }
                currentDoctor.IsVerify = true;
                bool isDeleted = await _doctorService.UpdateAsync(currentDoctor);
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
