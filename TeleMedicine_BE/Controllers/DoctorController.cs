using AutoMapper;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TeleMedicine_BE.ExternalService;
using TeleMedicine_BE.Utils;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/doctors")]
    [ApiController]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IHospitalService _hospitalService;
        private readonly IRoleService _roleService;
        private readonly IAccountService _accountService;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IMajorService _majorService;
        private readonly IUploadFileService _uploadFileService;
        private readonly ICertificationService _certificationService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Doctor> _pagingSupport;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ISendEmailService _sendEmailService;
        private readonly INotificationService _notificationService;
        private readonly IRedisService _redisService;


        public DoctorController(IDoctorService doctorService,IJwtTokenProvider jwtTokenProvider, ISendEmailService sendEmailService, IRoleService roleService, IHospitalService hospitalService, IAccountService accountService, IMajorService majorService, IUploadFileService uploadFileService, ICertificationService certificationService, IMapper mapper, IPagingSupport<Doctor> pagingSupport, IPushNotificationService pushNotificationService, INotificationService notificationService, IRedisService redisService)
        {
            _accountService = accountService;
            _doctorService = doctorService;
            _sendEmailService = sendEmailService;
            _roleService = roleService;
            _jwtTokenProvider = jwtTokenProvider;
            _hospitalService = hospitalService;
            _majorService = majorService;
            _certificationService = certificationService;
            _uploadFileService = uploadFileService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
            _pushNotificationService = pushNotificationService;
            _notificationService = notificationService;
            _redisService = redisService;
            
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<DoctorVM>>> GetAllDoctors(
            [FromQuery(Name = "email")] string email,
            [FromQuery(Name = "practising-certificate")] string practisingCertificate,
            [FromQuery(Name = "certificate-code")] string certificateCode,
            [FromQuery(Name = "place-certificate")] string placeCertificate,
            [FromQuery(Name = "date-start-certificate")] DateTime? dateStartCertificate,
            [FromQuery(Name = "date-end-certificate")] DateTime? dateEndCertificate,
            [FromQuery(Name = "scope-certificate")] string scopeCertificate,
            [FromQuery(Name = "number-start-consultants")] int numberStartConsultants,
            [FromQuery(Name = "number-end-consultants")] int numberEndConsultants,
            [FromQuery(Name = "start-rating")] int startRating,
            [FromQuery(Name = "end-rating")] int endRating,
            [FromQuery(Name = "is-active")] bool? isActive,
            [FromQuery(Name = "majors")] List<int> majors,
            [FromQuery(Name = "certifications")] List<int> certifications,
            [FromQuery(Name = "date-health-check")] DateTime? dateHealthCheck,
            [FromQuery(Name = "time-start")] TimeSpan timeStart,
            [FromQuery(Name = "time-end")] TimeSpan timeEnd,
            [FromQuery(Name = "latitude")] double latitude,
            [FromQuery(Name = "longitude")] double longitude,
            [FromQuery(Name = "name-doctor")] string nameDoctor,
            [FromQuery(Name = "hospitals")] List<int> hospitals,
            [FromQuery(Name = "order-by")] DoctorFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "is-update")] bool? isUpdate,
            [FromQuery(Name = "is-verify")] int isVerify = 0,
            [FromQuery(Name = "filtering")] string filters = null,
            int limit = 50,
            [FromQuery(Name = "page-offset")]  int pageOffset = 1
        )
        {
            string doctorListKey = "DOCTOR_LIST";
            var token = Request.Headers[HeaderNames.Authorization];
            if (!String.IsNullOrEmpty(token))
            {
                var role = _jwtTokenProvider.GetPayloadFromToken(token[0].Replace("Bearer", "").Trim(), "role");

                int roleId;
                if (int.TryParse(role.Result, out roleId))
                {
                    Role currentRole = _roleService.GetAll().Where(s => s.Id == roleId).FirstOrDefault();
                    if (currentRole != null)
                    {
                        if (currentRole.Name.ToUpper().Equals("PATIENT") || currentRole.Name.ToUpper().Equals("DOCTOR"))
                        {
                            if (Request.QueryString.Value.ToString().ToLower().Trim() == "?limit=8&page-offset=1")
                            {
                                
                                if (isUpdate.HasValue)
                                {
                                    IDictionary<String, DoctorVM> cache = await _redisService.GetList<DoctorVM>("UPDATE:*");
                                    if(cache != null)
                                    {
                                        return Ok(cache);
                                    }
                                }
                                else
                                {
                                    NumarablePaged<DoctorVM> cache = await _redisService.Get<NumarablePaged<DoctorVM>>(doctorListKey);
                                    if (cache != null)
                                    {
                                        return Ok(cache);
                                    }
                                    else
                                    {
                                        IQueryable<Doctor> doctorList = _doctorService.access().Include(s => s.CertificationDoctors).ThenInclude(s => s.Certification)
                                                                                           .Include(s => s.HospitalDoctors).ThenInclude(s => s.Hospital)
                                                                                           .Include(s => s.MajorDoctors).ThenInclude(s => s.Major)
                                                                                           .Include(s => s.Slots).Where(s => s.IsVerify == true);
                                        Paged<DoctorVM> paged = _pagingSupport.From(doctorList).GetRange(1, 8, s => s.Rating, 1).Paginate<DoctorVM>();
                                        bool succcess = await _redisService.Set(doctorListKey, paged, 60);
                                        if (succcess)
                                        {
                                            return Ok(paged);
                                        }
                                        return BadRequest();
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
            //if(string.IsNullOrEmpty(email) && 
            //    string.IsNullOrEmpty(placeCertificate) && 
            //    string.IsNullOrEmpty(practisingCertificate) &&
            //    string.IsNullOrEmpty(certificateCode) &&
            //    !dateStartCertificate.HasValue &&
            //    !dateEndCertificate.HasValue &&
            //    string.IsNullOrEmpty(scopeCertificate) &&
            //    numberStartConsultants == 0 && numberEndConsultants == 0 &&
            //    startRating == 0 && endRating == 0 && !isActive.HasValue &&
            //    !dateHealthCheck.HasValue && ) 
            //{

            //}
            try
            {
                IQueryable<Doctor> doctorList = _doctorService.access().Include(s => s.CertificationDoctors).ThenInclude(s => s.Certification)
                                                                       .Include(s => s.HospitalDoctors).ThenInclude(s => s.Hospital)
                                                                       .Include(s => s.MajorDoctors).ThenInclude(s => s.Major)
                                                                       .Include(s => s.Slots);
                Paged<DoctorVM> paged = null;
                bool isMapped = false;
                var accessToken = Request.Headers[HeaderNames.Authorization];
                if (!String.IsNullOrEmpty(accessToken))
                {
                    var role = _jwtTokenProvider.GetPayloadFromToken(accessToken[0].Replace("Bearer", "").Trim(), "role");

                    int roleId;
                    if (int.TryParse(role.Result, out roleId))
                    {
                        Role currentRole = _roleService.GetAll().Where(s => s.Id == roleId).FirstOrDefault();
                        if (currentRole != null)
                        {
                            if (currentRole.Name.ToUpper().Equals("PATIENT") || currentRole.Name.ToUpper().Equals("DOCTOR"))
                            {
                                doctorList = doctorList.Where(s => s.IsVerify == true);
                            }
                        }
                    }
                }
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

                if (dateHealthCheck.HasValue)
                {
                    if (timeStart.CompareTo(TimeSpan.Zero) != 0 && timeEnd.CompareTo(TimeSpan.Zero) != 0)
                    {
                        doctorList = doctorList.Where(s => s.Slots.Any(s => s.AssignedDate.Date.CompareTo(dateHealthCheck.Value.Date) == 0
                                                                            && s.HealthCheckId == null
                                                                            && s.StartTime.CompareTo(timeStart) >= 0 && s.EndTime.CompareTo(timeEnd) <= 0));
                    }
                    else
                    {
                        if (timeStart.CompareTo(TimeSpan.Zero) != 0)
                        {
                            doctorList = doctorList.Where(s => s.Slots.Any(s => s.AssignedDate.Date.CompareTo(dateHealthCheck.Value.Date) == 0
                                                                            && s.StartTime.CompareTo(timeStart) >= 0
                                                                            && s.HealthCheckId == null));
                        }
                        else if (timeEnd.CompareTo(TimeSpan.Zero) != 0)
                        {
                            doctorList = doctorList.Where(s => s.Slots.Any(s => s.AssignedDate.Date.CompareTo(dateHealthCheck.Value.Date) == 0
                                                                            && s.EndTime.CompareTo(timeEnd) <= 0 && s.HealthCheckId == null));
                        }else
                        {
                            doctorList = doctorList.Where(s => s.Slots.Any(s => s.AssignedDate.Date.CompareTo(dateHealthCheck.Value.Date) == 0 && s.HealthCheckId == null));
                        }
                    }
                }
                else
                {

                    if (timeStart.CompareTo(TimeSpan.Zero) != 0 && timeEnd.CompareTo(TimeSpan.Zero) != 0)
                    {
                        doctorList = doctorList.Where(s => s.Slots.Any(s => s.StartTime.CompareTo(timeStart) >= 0 && s.EndTime.CompareTo(timeEnd) <= 0 && s.HealthCheckId == null));
                    }
                    else
                    {
                        if (timeStart.CompareTo(TimeSpan.Zero) != 0)
                        {
                            doctorList = doctorList.Where(s => s.Slots.Any(s => s.StartTime.CompareTo(timeStart) >= 0 && s.HealthCheckId == null));
                        }
                        else if (timeEnd.CompareTo(TimeSpan.Zero) != 0)
                        {
                            doctorList = doctorList.Where(s => s.Slots.Any(s => s.EndTime.CompareTo(timeEnd) <= 0 && s.HealthCheckId == null));
                        }
                    }
                }

                if(!String.IsNullOrEmpty(nameDoctor))
                {
                    doctorList = doctorList.Where(s => s.Name.ToLower().Contains(nameDoctor.ToLower().Trim()));
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
                if(isActive.HasValue)
                {
                    doctorList = doctorList.Where(s => s.IsActive.Value.Equals(isActive.Value));
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
                    if (isVerify == -2)
                    {
                        doctorList = doctorList.Where(s => s.IsVerify == null);
                    }
                }
                
                if(majors != null && majors.Count > 0)
                {
                    foreach(int item in majors)
                    {
                        Major currentMajor = _majorService.GetAll(s => s.MajorDoctors).Where(s => s.Id == item).FirstOrDefault();
                        if (currentMajor == null)
                        {
                            return BadRequest(new {
                                message = "Major id must be existed!"
                            });
                        }
                    }
                        foreach(int item in majors)
                        {
                            
                            doctorList = doctorList.Where(s => s.MajorDoctors.Any(s => s.MajorId == item));
                        }
                }

                if (hospitals != null && hospitals.Count > 0)
                {
                    foreach (int item in hospitals)
                    {
                        Hospital currentHospital = _hospitalService.GetAll(s => s.HospitalDoctors).Where(s => s.Id == item).FirstOrDefault();
                        if (currentHospital == null)
                        {
                            return BadRequest(new
                            {
                                message = "Hospital id must be existed!"
                            });
                        }
                    }
                    foreach (int item in hospitals)
                    {

                        doctorList = doctorList.Where(s => s.HospitalDoctors.Any(s => s.HospitalId == item));
                    }
                }

                if (certifications != null && certifications.Count > 0)
                {
                    foreach (int item in certifications)
                    {
                        Certification currentCertification = _certificationService.GetAll(s => s.CertificationDoctors).Where(s => s.Id == item).FirstOrDefault();
                        if (currentCertification == null)
                        {
                            return BadRequest(new
                            {
                                message = "Certification id must be existed!"
                            });
                        }
                    }
                    foreach (int item in certifications)
                    {

                        doctorList = doctorList.Where(s => s.CertificationDoctors.Any(s => s.CertificationId == item));
                    }
                }

                if (latitude != 0 && longitude != 0)
                {
                    List<DoctorVM> converDoctorList = new List<DoctorVM>();
                    List<DoctorVM> _mappedDoctorVmList = new List<DoctorVM>();
                    foreach (Doctor doctor in doctorList)
                    {
                        _mappedDoctorVmList.Add(_mapper.Map<DoctorVM>(doctor));
                    }
                    var currentCoordinate = new GeoCoordinate(latitude, longitude);
                    List<Hospital> hospitalList = _hospitalService.GetAll().AsEnumerable().OrderBy(location => currentCoordinate.GetDistanceTo(new GeoCoordinate(location.Lat, location.Long))).ToList();
                    foreach (Hospital itemHospital in hospitalList)
                    {
                        foreach (DoctorVM itemDoctor in _mappedDoctorVmList)
                        {
                            if (itemDoctor.HospitalDoctors.Any(s => s.Hospital.Id == itemHospital.Id))
                            {
                                if (!converDoctorList.Contains(itemDoctor))
                                {
                                    converDoctorList.Add(itemDoctor);
                                }
                            }
                        }
                    }
                    List<Doctor> convertDoctorVmToDoctor = new List<Doctor>();
                    foreach (DoctorVM doctorItem in converDoctorList)
                    {
                        convertDoctorVmToDoctor.Add(_mapper.Map<Doctor>(doctorItem));
                    }
                    doctorList = convertDoctorVmToDoctor.AsQueryable();
                    paged = _pagingSupport.From(doctorList).GetRange(pageOffset, limit, null, 1).Paginate<DoctorVM>();
                    isMapped = true;
                }
                if (!isMapped)
                {

                    if (orderType == SortTypeEnum.asc && typeof(DoctorVM).GetProperty(orderBy.ToString()) != null)
                    {
                        paged = _pagingSupport.From(doctorList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0).Paginate<DoctorVM>();
                    }
                    else if (orderType == SortTypeEnum.desc && typeof(DoctorVM).GetProperty(orderBy.ToString()) != null)
                    {
                        paged = _pagingSupport.From(doctorList).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1).Paginate<DoctorVM>();
                    }
                    else
                    {
                        paged = _pagingSupport.From(doctorList).GetRange(pageOffset, limit, s => s.Id, 1).Paginate<DoctorVM>();
                    }
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
                        string json = jsonIgnore.JsonIgnore(typeof(DoctorVM), splitFilter, paged, PropertyRenameAndIgnoreSerializerContractResolver.IgnoreMode.EXCEPT);
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
        /// Get a specific doctor by type
        /// </summary>
        /// <returns>Return the doctor with the type</returns>
        /// <response code="200">Returns the doctor with the type</response>
        /// <response code="404">No doctor found with the type</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{search}")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<DoctorVM> GetDoctorByType([FromRoute] string search, [FromQuery(Name = "search-type")] SearchType searchType)
        {
            try
            {
                Doctor currentDoctor = null;
                if (searchType == SearchType.Id)
                {
                    try
                    {
                        int result = Int32.Parse(search);
                         IQueryable<Doctor> doctorList = _doctorService.access().Include(s => s.CertificationDoctors).ThenInclude(s => s.Certification)
                                                                       .Include(s => s.HospitalDoctors).ThenInclude(s => s.Hospital)
                                                                       .Include(s => s.MajorDoctors).ThenInclude(s => s.Major);
                        currentDoctor = doctorList.Where(s => s.Id == result).FirstOrDefault();
                    }
                    catch (FormatException)
                    {
                        return BadRequest();
                    }
                }
                else if (searchType == SearchType.Email && !string.IsNullOrEmpty(search))
                {
                    IQueryable<Doctor> doctorList = _doctorService.access().Include(s => s.CertificationDoctors).ThenInclude(s => s.Certification)
                                                                       .Include(s => s.HospitalDoctors).ThenInclude(s => s.Hospital)
                                                                       .Include(s => s.MajorDoctors).ThenInclude(s => s.Major);
                    currentDoctor = doctorList.Where(s => s.Email.Trim().ToUpper().Equals(search.Trim().ToUpper())).FirstOrDefault();
                }
                if (currentDoctor != null)
                {
                    return Ok(_mapper.Map<DoctorVM>(currentDoctor));
                }
                return NotFound();
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
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1, 2")]
        public async Task<ActionResult<DoctorVM>> PutDoctor([FromForm] DoctorUM model)
        {

            List<HospitalDoctorWithRegisterCM> newHospitalDoctors = new List<HospitalDoctorWithRegisterCM>();
            List<MajorDoctorWithRegisterCM> newMajorDoctors = new List<MajorDoctorWithRegisterCM>();
            if (model.HospitalDoctors != null)
            {
                newHospitalDoctors = JsonConvert.DeserializeObject<List<HospitalDoctorWithRegisterCM>>(model.HospitalDoctors);
            }
            if (model.MajorDoctors != null)
            {
                newMajorDoctors = JsonConvert.DeserializeObject<List<MajorDoctorWithRegisterCM>>(model.MajorDoctors);
            }

            Doctor currentDoctor = _doctorService.access().Include(s => s.CertificationDoctors).ThenInclude(s => s.Certification)
                                                                       .Include(s => s.HospitalDoctors).ThenInclude(s => s.Hospital)
                                                                       .Include(s => s.MajorDoctors).ThenInclude(s => s.Major).Where(s => s.Id == model.Id).FirstOrDefault();
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

            List<MajorDoctor> convertMajor = new List<MajorDoctor>();
            List<HospitalDoctor> convertHospital = new List<HospitalDoctor>();


            if (newHospitalDoctors != null && newHospitalDoctors.Count > 0)
            {
                for (int i = 0; i < newHospitalDoctors.Count; i++)
                {
                    if (_hospitalService.GetAll().Where(s => s.Id == newHospitalDoctors[i].HospitalId).FirstOrDefault() == null)
                    {
                        return BadRequest(new
                        {
                            message = "Hospital is not existed!"
                        });
                    }
                    convertHospital.Add(_mapper.Map<HospitalDoctor>(newHospitalDoctors[i]));
                }
            }
            if (newMajorDoctors != null && newMajorDoctors.Count > 0)
            {
                for (int i = 0; i < newMajorDoctors.Count; i++)
                {
                    if (_majorService.GetAll().Where(s => s.Id == newMajorDoctors[i].MajorId).FirstOrDefault() == null)
                    {
                        return BadRequest(new
                        {
                            message = "Major is not existed!"
                        });
                    }
                    convertMajor.Add(_mapper.Map<MajorDoctor>(newMajorDoctors[i]));
                }
            }

            if(currentDoctor.MajorDoctors != null && currentDoctor.MajorDoctors.Count > 0 && convertMajor.Count > 0)
            {
                foreach(MajorDoctor item in currentDoctor.MajorDoctors)
                {
                    foreach(MajorDoctor checkItem in convertMajor)
                    {
                        if(item.MajorId == checkItem.MajorId)
                        {
                            return BadRequest(new
                            {
                                message = "Major have been registered!"
                            });
                        }
                    }
                }
            }

            if (currentDoctor.HospitalDoctors != null && currentDoctor.HospitalDoctors.Count > 0 && convertHospital.Count > 0)
            {
                foreach (HospitalDoctor item in currentDoctor.HospitalDoctors)
                {
                    foreach (HospitalDoctor checkItem in convertHospital)
                    {
                        if (item.HospitalId == checkItem.HospitalId)
                        {
                            return BadRequest(new
                            {
                                message = "Hospital have been registered!"
                            });
                        }
                    }
                }
            }


            Account currentAccount = _accountService.GetAccountByEmail(currentDoctor.Email);
            if(currentAccount != null)
            {
                currentDoctor.Avatar = currentAccount.Avatar;
                currentDoctor.Name = currentAccount.FirstName.Trim() + " "  + currentAccount.LastName.Trim();
            }
            try
            {
                if (convertMajor != null && convertMajor.Count > 0)
                {
                    if(currentDoctor.MajorDoctors != null && currentDoctor.MajorDoctors.Count > 0)
                    {
                        foreach(MajorDoctor item in convertMajor)
                        {
                            currentDoctor.MajorDoctors.Add(item);
                        }
                    }else
                    {
                        List<MajorDoctor> newMajorDoctor = new List<MajorDoctor>();
                        foreach(MajorDoctor item in convertMajor)
                        {
                            newMajorDoctor.Add(item);
                        }
                        currentDoctor.MajorDoctors = newMajorDoctor.ToArray();
                    }
                }
                if (convertHospital != null && convertHospital.Count > 0)
                {
                    if (currentDoctor.HospitalDoctors != null && currentDoctor.HospitalDoctors.Count > 0)
                    {
                        foreach (HospitalDoctor item in convertHospital)
                        {
                            currentDoctor.HospitalDoctors.Add(item);
                        }
                    }
                    else
                    {
                        List<HospitalDoctor> newHospitalDoctor = new List<HospitalDoctor>();
                        foreach (HospitalDoctor item in convertHospital)
                        {
                            newHospitalDoctor.Add(item);
                        }
                        currentDoctor.HospitalDoctors = newHospitalDoctor.ToArray();
                    }
                }
                if(model.PractisingCertificate != null)
                { 
                    string fileUrl = await _uploadFileService.UploadFile(model.PractisingCertificate, "service", "service-detail");
                    currentDoctor.PractisingCertificate = fileUrl;
                }
                currentDoctor.CertificateCode = model.CertificateCode.Trim().ToUpper();
                currentDoctor.PlaceOfCertificate = model.PlaceOfCertificate.Trim();
                currentDoctor.DateOfCertificate = model.DateOfCertificate;
                currentDoctor.ScopeOfPractice = model.ScopeOfPractice.Trim();
                currentDoctor.Description = model.Description.Trim();
                currentDoctor.IsActive = model.IsActive;
                bool isUpdated = await _doctorService.UpdateAsync(currentDoctor);
                if (isUpdated)
                {
                    List<Notification> notifications = new();
                    IQueryable<Account> accountList = _accountService.GetAll().Where(s => s.RoleId == 2);
                    accountList.ToList().ForEach(s =>
                    {
                        _pushNotificationService.SendMessage("Có 1 tài khoản bác sĩ cần được xác thực.", "Tài khoản " + currentDoctor.Email +" yêu cầu xác thực!", s.Email, null);
                        Notification notification = new Notification();
                        notification.Content = "Có một yêu cầu xét duyệt-/doctors/" + currentDoctor.Email;
                        notification.Type = Constants.Notification.REQUEST_VERIFY;
                        notification.IsSeen = false;
                        notification.IsActive = true;
                        notification.UserId = s.Id;
                        notification.CreatedDate = DateTime.Now;
                        notifications.Add(notification);
                    });
                    await _redisService.Set("UPDATE:" + currentDoctor.Email, currentDoctor, 60);
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
        /// <remarks>
        /// Sample request:
        ///
        ///     hospitalDoctors: "[{ "hospitalId": 1},{"hospitalId": 2},] "
        ///     majorDoctors: "[{"majorId": 1}]"
        /// </remarks>
        /// <response code="201">Created new doctor successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        public async Task<ActionResult<DoctorVM>> CreateNewDoctor([FromForm] DoctorCM model)
        {
            List<HospitalDoctorWithRegisterCM> newHospitalDoctors = new List<HospitalDoctorWithRegisterCM>();
            List<MajorDoctorWithRegisterCM> newMajorDoctors = new List<MajorDoctorWithRegisterCM>();
            if (model.HospitalDoctors != null)
            {
                newHospitalDoctors = JsonConvert.DeserializeObject<List<HospitalDoctorWithRegisterCM>>(model.HospitalDoctors);
            }
            if(model.MajorDoctors != null)
            {
                newMajorDoctors = JsonConvert.DeserializeObject<List<MajorDoctorWithRegisterCM>>(model.MajorDoctors);
            }


            List<HospitalDoctor> convertHospitalDoctor = new List<HospitalDoctor>();
            List<MajorDoctor> convertMajorDoctor = new List<MajorDoctor>();

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
                if (newHospitalDoctors != null && newHospitalDoctors.Count > 0)
                {
                    for (int i = 0; i < newHospitalDoctors.Count; i++)
                    {
                        if (_hospitalService.GetAll().Where(s => s.Id == newHospitalDoctors[i].HospitalId).FirstOrDefault() == null)
                        {
                            return BadRequest(new
                            {
                                message = "Hospital is not existed!"
                            });
                        }
                        convertHospitalDoctor.Add(_mapper.Map<HospitalDoctor>(newHospitalDoctors[i]));
                    }
                }
                if (newMajorDoctors != null && newMajorDoctors.Count > 0)
                {
                    for (int i = 0; i < newMajorDoctors.Count; i++)
                    {
                        if (_majorService.GetAll().Where(s => s.Id == newMajorDoctors[i].MajorId).FirstOrDefault() == null)
                        {
                            return BadRequest(new
                            {
                                message = "Major is not existed!"
                            });
                        }
                        convertMajorDoctor.Add(_mapper.Map<MajorDoctor>(newMajorDoctors[i]));
                    }
                }

                model.CertificateCode = model.CertificateCode.Trim().ToUpper();
                Doctor mappedDoctor = new Doctor();
                mappedDoctor.Email = model.Email;
                Account currentAccount = _accountService.GetAccountByEmail(model.Email);
                if (currentAccount != null)
                {
                    mappedDoctor.Avatar = currentAccount.Avatar;
                    mappedDoctor.Name = currentAccount.FirstName.Trim() + " " + currentAccount.LastName.Trim();
                }

                string fileUrl = await _uploadFileService.UploadFile(model.PractisingCertificate, "service", "service-detail");
                mappedDoctor.PractisingCertificate = fileUrl;
                mappedDoctor.CertificateCode = model.CertificateCode;
                mappedDoctor.PlaceOfCertificate = model.PlaceOfCertificate;
                mappedDoctor.DateOfCertificate = model.DateOfCertificate;
                mappedDoctor.ScopeOfPractice = model.ScopeOfPractice;
                mappedDoctor.Description = model.Description;
                mappedDoctor.MajorDoctors = convertMajorDoctor;
                mappedDoctor.HospitalDoctors = convertHospitalDoctor;

                Doctor doctorCreated = await _doctorService.AddAsync(mappedDoctor);
                List<Notification> notifications = new();
                if (doctorCreated != null)
                {
                    IQueryable<Account> accountList = _accountService.GetAll().Where(s => s.RoleId == 2);
                    accountList.ToList().ForEach(s =>
                    {
                        _pushNotificationService.SendMessage("Có một tài khoản mới đang yêu cầu được xác thực", "Tài khoản " + model.Email +" yêu cầu xác thực!", s.Email, null);
                        Notification notification = new();
                        notification.Content = "Có một yêu cầu xét duyệt-/doctors/" + model.Email;
                        notification.Type = Constants.Notification.REQUEST_VERIFY;
                        notification.IsSeen = false;
                        notification.IsActive = true;
                        notification.UserId = s.Id;
                        notification.CreatedDate = DateTime.Now;
                        notifications.Add(notification);
                    });
                    _notificationService.AddManyAsync(notifications);
                    IQueryable<Doctor> doctorList = _doctorService.GetAll(s => s.HospitalDoctors, s => s.CertificationDoctors, s => s.MajorDoctors);
                    Doctor doctor = doctorList.Where(s => s.Id == doctorCreated.Id).FirstOrDefault();
                    return CreatedAtAction("GetDoctorByType", new { search = doctorCreated.Id }, _mapper.Map<DoctorVM>(doctorCreated));
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
        /// Add Certification for Doctor Existed
        /// </summary>
        /// <response code="201">Add new certifications doctor successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost("{doctorId}/certifications")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        public async Task<ActionResult<DoctorVM>> AddCertificationInDoctorExisted([FromRoute] int doctorId, [FromForm] CertificationDoctorWithRegisterCM model)
        {
            try
            {
                Doctor currentDoctor = _doctorService.GetAll(s => s.CertificationDoctors, s => s.MajorDoctors, s => s.HospitalDoctors).Where(s => s.Id == doctorId).FirstOrDefault();
                if(currentDoctor == null)
                {
                    return NotFound(new
                    {
                        message = "Can not found doctor by id!"
                    });
                }

                if (model.Evidence != null)
                {
                    Certification currentCertification = await _certificationService.GetByIdAsync(model.CertificationId);
                    if(currentCertification != null)
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.Evidence, "service", "service-detail");
                        CertificationDoctor certificationDoctorCreated = new CertificationDoctor();
                        certificationDoctorCreated.Evidence = fileUrl;
                        certificationDoctorCreated.DateOfIssue = model.DateOfIssue;
                        certificationDoctorCreated.CertificationId = model.CertificationId;
                        certificationDoctorCreated.Certification = currentCertification;

                        if(currentDoctor.CertificationDoctors != null && currentDoctor.CertificationDoctors.Count > 0)
                        {
                            foreach(CertificationDoctor item in currentDoctor.CertificationDoctors)
                            {
                                if(item.CertificationId == model.CertificationId)
                                {
                                    return BadRequest(new
                                    {
                                        message = "Certification have been existed!"
                                    });
                                }
                            }
                            currentDoctor.CertificationDoctors.Add(certificationDoctorCreated);
                        }else
                        {
                            List<CertificationDoctor> certificationDoctors = new List<CertificationDoctor>();
                            certificationDoctors.Add(certificationDoctorCreated);
                            currentDoctor.CertificationDoctors = certificationDoctors.ToArray();
                        }
                        bool isUpdated = await _doctorService.UpdateAsync(currentDoctor);
                        if(isUpdated)
                        {
                            return Ok(_mapper.Map<DoctorVM>(currentDoctor));
                        }
                    }
                    return BadRequest();
                }
                return BadRequest();
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2")]
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
                currentDoctor.IsActive = false;
                bool isDeleted = await _doctorService.UpdateAsync(currentDoctor);
                if (isDeleted)
                {
                    await _redisService.RemoveKey("DOCTOR_LIST");
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2")]
        public async Task<ActionResult> VerifyDoctorById(int id, [FromQuery] DoctorStatusVerify mode)
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
                if(mode == DoctorStatusVerify.ACCEPT)
                {
                    currentDoctor.IsVerify = true;
                }else if(mode == DoctorStatusVerify.CANCEL)
                {
                    currentDoctor.IsVerify = false;
                }
                bool isVerify = await _doctorService.UpdateAsync(currentDoctor);
                if (isVerify)
                {
                    EmailForm mail = new EmailForm();
                    mail.ToEmail = currentDoctor.Email;
                    mail.Subject = mode == DoctorStatusVerify.ACCEPT ? "Thông báo tài khoản được xác nhận" : "Thông báo tài khoản đã bị từ chối";
                    mail.Message = mode == DoctorStatusVerify.ACCEPT ? "Chúc mừng tài khoản của bạn đã được xác nhận. Bây giờ bạn đã có thể đăng nhập."
                                                         : "Tài khoản của bạn đã bị từ chối. Mong có thể làm việc với bạn trong tương lai.";
                    await _sendEmailService.SendEmail(mail);
                    await _redisService.RemoveKey("DOCTOR_LIST");
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
