using AutoMapper;
using BeautyAtHome.Utils;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IDoctorService _doctorService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public AuthController(IAccountService accountService, IJwtTokenProvider jwtTokenProvider, IDoctorService doctorService, IRoleService roleService, IMapper mapper)
        {
            _accountService = accountService;
            _jwtTokenProvider = jwtTokenProvider;
            _doctorService = doctorService;
            _roleService = roleService;
            _mapper = mapper;
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
                System.Diagnostics.Debug.WriteLine(email);
                Account account = _accountService.GetAccountByEmail(email);
                System.Diagnostics.Debug.WriteLine(email);

                if (account != null)
                {
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
                        if (!doctor.IsVerify.HasValue)
                        {
                            return Ok(new { message = "The account is not verify!" });
                        }
                    }

                    string accessToken = await _jwtTokenProvider.GenerateToken(account);
                    AccountProfileVM accountProfileVM = _mapper.Map<AccountProfileVM>(account);
                    return Ok(new
                    {
                        Account = accountProfileVM,
                        AccessToken = accessToken,
                    });
                }
                else
                {
                    Role currentRole = _roleService.GetAll().Where(s => s.Id == model.LoginType).FirstOrDefault();
                    if(currentRole.Name.Equals("ADMIN"))
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
                System.Diagnostics.Debug.WriteLine(email);
                Account account = _accountService.GetAccountByEmail(email);
                System.Diagnostics.Debug.WriteLine(email);

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
    }
}
