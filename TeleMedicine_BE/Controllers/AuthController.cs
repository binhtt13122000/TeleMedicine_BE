using AutoMapper;
using TeleMedicine_BE.Utils;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleMedicine_BE.ExternalService;
using TeleMedicine_BE.ViewModels;
using Role = Infrastructure.Models.Role;


namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IUploadFileService _uploadFileService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IPushNotificationService _pushNotificationService;

        public AuthController(IAccountService accountService, IJwtTokenProvider jwtTokenProvider, IUploadFileService uploadFileService, IDoctorService doctorService, IPatientService patientService,IPushNotificationService pushNotificationService, IRoleService roleService, IMapper mapper)
        {
            _accountService = accountService;
            _jwtTokenProvider = jwtTokenProvider;
            _uploadFileService = uploadFileService;
            _doctorService = doctorService;
            _patientService = patientService;
            _roleService = roleService;
            _mapper = mapper;
            _pushNotificationService = pushNotificationService;
        }

        [HttpPost("upload-image")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> uploadFileImage([FromForm] ImageCM model)
        {
            try
            {
                
                string fileUrl = await _uploadFileService.UploadFile(model.file, "service", "service-detail");
                return Ok(new
                {
                    url = fileUrl
                });
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost("login")]
        [Produces("application/json")]
        public async Task<ActionResult> Login([FromBody] AuthCM model)
        {
            if (model.LoginType < 1 || model.LoginType > 3)
            {
                return BadRequest();
            }
            var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
            string email;
            try
            {
                var token = await auth.VerifyIdTokenAsync(model.TokenId);
                email = (string)token.Claims["email"];
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }

            try
            {
                int referencesId = 0;
                Account account = _accountService.GetAccountByEmail(email);

                if (account != null)
                {
                    if(!account.Active.Value)
                    {
                        return BadRequest(new
                        {
                            message = "Account is ban!"
                        });
                    }
                    if (account.Role.Id != model.LoginType)
                    {
                        return BadRequest(new
                        {
                            message = "LoginType is incorrect!"
                        });
                    }
                    if (account.Role.Id == 1)
                    {
                        Doctor doctor = _doctorService.GetDoctorByEmail(email);
                        referencesId = doctor.Id;
                        if (!doctor.IsVerify.HasValue)
                        {
                            return Ok(new { message = "The account is not verify!" });
                        }
                    } else if(account.Role.Id == 3)
                    {
                        Patient patient = _patientService.GetPatientByEmail(email);
                        referencesId = patient.Id;
                    }

                    string accessToken = await _jwtTokenProvider.GenerateToken(account);
                    AccountProfileVM accountProfileVM = _mapper.Map<AccountProfileVM>(account);
                    return Ok(new
                    {
                        Account = accountProfileVM,
                        AccessToken = accessToken,
                        RefenrenceId = referencesId
                    });
                }
                else
                {
                    Role currentRole = _roleService.GetAll().Where(s => s.Id == model.LoginType).FirstOrDefault();
                    if (currentRole.Name.Equals("ADMIN"))
                    {
                        return StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    return Ok(new
                    {
                        Email = email
                    });
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult(e.Message);
            }
        }

        [HttpPost("fake-login")]
        [Produces("application/json")]
        public async Task<ActionResult> FakeLogin([FromQuery] string email)
        {

            try
            {
                Account account = _accountService.GetAccountByEmail(email);
                if (account != null)
                {
                    if (account.Role.Id == 1)
                    {
                        Doctor doctor = _doctorService.GetDoctorByEmail(email);
                        if (!doctor.IsVerify.HasValue)
                        {
                            return Ok(new { message = "The account is not verify!" });
                        }
                    }

                    string accessToken = await _jwtTokenProvider.GenerateToken(account);
                    AccountProfileVM accountProfileVM = _mapper.Map<AccountProfileVM>(account);
                    _ = _pushNotificationService.SendMessage("Có 1 thằng đang đăng nhập", account.Email, account.Id, null);
                    return Ok(new
                    {
                        Account = accountProfileVM,
                        AccessToken = accessToken,
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Email = email
                    });
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult(e.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] NotificationRequest model)
        {
            Account account = await _accountService.GetByIdAsync(model.Id);
            if (account == null)
            {
                return BadRequest(new
                {
                    message = "User Id is not exist."
                });
            }
            try
            {
                var response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(new List<string>() { model.Token }, "/topics/" + account.Id);
                if (response.SuccessCount > 0)
                {
                    return Ok(new { Message = "logout success!" });
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
