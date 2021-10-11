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
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly IDoctorService _doctorService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Account> _pagingSupport;

        public AccountController(IAccountService accountService, IDoctorService doctorService, IRoleService roleService, IMapper mapper, IPagingSupport<Account> pagingSupport)
        {
            _accountService = accountService;
            _doctorService = doctorService;
            _roleService = roleService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list accounts
        /// </summary>
        /// <returns>List accounts</returns>
        /// <response code="200">Returns all accounts</response>
        /// <response code="404">Not found accounts</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<Paged<AccountManageVM>> GetAll(
            [FromQuery(Name = "email")] string email,
            [FromQuery(Name = "first-name")] string firstName,
            [FromQuery(Name = "last-name")] string lastName,
            [FromQuery(Name = "ward")] string ward,
            [FromQuery(Name = "street-address")] string streetAddress,
            [FromQuery(Name = "locality")] string locality,
            [FromQuery(Name = "city")] string city,
            [FromQuery(Name = "postal-code")] string postalCode,
            [FromQuery(Name = "phone")] string phone,
            [FromQuery(Name = "start-dob")] DateTime? startDob,
            [FromQuery(Name = "end-dob")] DateTime? endDob,
            [FromQuery(Name = "order-by")] AccountFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "is-male")] int isMale = 0,
            [FromQuery(Name = "active")] int active = 0,
            [FromQuery(Name = "role-name")] string roleName = null,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "page-offset")]  int pageOffset = 1,
            int limit = 20
        )
        {
            try
            {
                IQueryable<Account> accountsQuery = _accountService.GetAll(_ => _.Role).Where(s => !s.Role.Name.Trim().ToUpper().Equals("ADMIN"));
                List<String> doctorQuery = _doctorService.GetAll().Where(s => s.IsVerify == true).Select(s => s.Email.Trim().ToUpper()).ToList();
                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    accountsQuery = accountsQuery.Where(_ => _.FirstName.ToUpper().Contains(firstName.Trim().ToUpper()));
                }

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    accountsQuery = accountsQuery.Where(_ => _.LastName.ToUpper().Contains(lastName.Trim().ToUpper()));
                }

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    accountsQuery = accountsQuery.Where(_ => _.Phone.ToUpper().Contains(phone.Trim().ToUpper()));
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    accountsQuery = accountsQuery.Where(_ => _.Email.ToUpper().Contains(email.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(ward))
                {
                    accountsQuery = accountsQuery.Where(s => s.Ward.ToUpper().Contains(ward.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(streetAddress))
                {
                    accountsQuery = accountsQuery.Where(s => s.StreetAddress.ToUpper().Contains(streetAddress.Trim().ToUpper()));
                }
                if(!string.IsNullOrEmpty(locality))
                {
                    accountsQuery = accountsQuery.Where(s => s.Locality.ToUpper().Contains(locality.Trim().ToUpper()));
                }
                if(!string.IsNullOrEmpty(city))
                {
                    accountsQuery = accountsQuery.Where(s => s.City.ToUpper().Contains(city.Trim().ToUpper()));
                }
                if(!string.IsNullOrEmpty(postalCode))
                {
                    accountsQuery = accountsQuery.Where(s => s.PostalCode.ToUpper().Contains(postalCode.Trim().ToUpper()));
                }
                if(startDob.HasValue && endDob.HasValue)
                {
                    accountsQuery = accountsQuery.Where(s => s.Dob.CompareTo(startDob.Value) >= 0).Where(s => s.Dob.CompareTo(endDob.Value) <= 0);
                }
                else
                {
                    if (startDob.HasValue)
                    {
                        accountsQuery = accountsQuery.Where(s => s.Dob.CompareTo(startDob.Value) >= 0);
                    }
                    if(endDob.HasValue)
                    {
                        accountsQuery = accountsQuery.Where(s => s.Dob.CompareTo(endDob.Value) <= 0);
                    }
                }
                if(isMale != 0)
                {
                    if(isMale == 1)
                    {
                        accountsQuery = accountsQuery.Where(s => s.IsMale == true);
                    }
                    if(isMale == -1)
                    {
                        accountsQuery = accountsQuery.Where(s => s.IsMale == false);
                    }
                }
                if(active != 0)
                {
                    if(active == 1)
                    {
                        accountsQuery = accountsQuery.Where(s => s.Active == true);
                    }
                    if(active == -1)
                    {
                        accountsQuery = accountsQuery.Where(s => s.Active == false);
                    }
                }
                if(!string.IsNullOrEmpty(roleName))
                {
                    accountsQuery = accountsQuery.Where(s => s.Role.Name.ToUpper().Contains(roleName.Trim().ToUpper()));
                }
                if(doctorQuery != null && doctorQuery.Count > 0)
                {
                        accountsQuery = accountsQuery.Where(s => s.Role.Name.Equals("PATIENT") || (s.Role.Name.Equals("DOCTOR") && doctorQuery.Contains(s.Email.Trim().ToUpper())));
                }
                Paged<AccountManageVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(AccountManageVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(accountsQuery).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0).Paginate<AccountManageVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(AccountManageVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(accountsQuery).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1).Paginate<AccountManageVM>();
                }
                else
                {
                    paged = _pagingSupport.From(accountsQuery).GetRange(pageOffset, limit, s => s.RegisterTime, 1).Paginate<AccountManageVM>();
                }
                if (!string.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(AccountManageVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(AccountManageVM), splitFilter, paged);
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
        /// Get a specific account by type
        /// </summary>
        /// <returns>Return the account with the type</returns>
        /// <response code="200">Returns the account with the type</response>
        /// <response code="404">No account found with the type</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{search}")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AccountProfileVM>> GetAccountByType([FromRoute] string search, [FromQuery(Name = "search-type")] SearchType searchType)
        {
            try
            {
                Account account = null;
                if (searchType == SearchType.Id)
                {
                    try
                    {
                        int result = Int32.Parse(search);
                        account = await _accountService.GetByIdAsync(result);
                    }
                    catch (FormatException)
                    {
                        return BadRequest();
                    }
                }
                else if(searchType == SearchType.Email && !string.IsNullOrEmpty(search))
                {
                    account = _accountService.GetAccountByEmail(search.Trim());
                }
                if(account != null)
                {
                    return Ok(_mapper.Map<AccountProfileVM>(account));
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a account
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AccountProfileVM>> UpdateAccount([FromBody] AccountProfileUM model)
        {
            try
            {
                Account account = await _accountService.GetByIdAsync(model.Id);
                if (account == null)
                {
                    return NotFound();
                }

                account.FirstName = model.FirstName;
                account.LastName = model.LastName;
                account.Phone = model.Phone;
                account.IsMale = model.IsMale;
                account.Locality = model.Locality;
                account.PostalCode = model.PostalCode;
                account.Avatar = model.Avatar;
                account.City = model.City;
                account.StreetAddress = model.StreetAddress;
                account.Dob = model.Dob;

                bool isUpdated = await _accountService.UpdateAsync(account);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<AccountProfileVM>(account));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Change status account
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ChangeStatus([FromRoute] int id)
        {
            try {
                Account account = await _accountService.GetByIdAsync(id);
                if (account == null)
                {
                    return NotFound();
                }

                account.Active = !account.Active;

                bool isUpdated = await _accountService.UpdateAsync(account);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<AccountProfileVM>(account));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete account By Id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteAccount([FromRoute] int id)
        {
            Account currentAccount = await _accountService.GetByIdAsync(id);
            if (currentAccount == null)
            {
                return NotFound();
            }

            try
            {
                bool isDeleted = await _accountService.DeleteAsync(currentAccount);
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
        /// Create a new account
        /// </summary>
        /// <response code="201">Created new account successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<AccountProfileVM>> CreateNewAccount([FromBody] AccountProfileCM model)
        {
            try
            {
                Account currentAccount = _accountService.GetAll().Where(s => s.Email.Trim().ToUpper().Equals(model.Email.Trim().ToUpper())).FirstOrDefault();
                if (currentAccount != null)
                {
                    return BadRequest(new
                    {
                        message = "Email have been registered!"
                    });
                }
                Role currenRole = await _roleService.GetByIdAsync(model.RoleId);
                if(currenRole == null)
                {
                    return BadRequest(new
                    {
                        message = "Can not found role by id!"
                    });
                }
                
                Account convertAccount = _mapper.Map<Account>(model);
                convertAccount.Email = model.Email.Trim().ToLower();
                convertAccount.FirstName = model.FirstName.Trim();
                convertAccount.LastName = model.LastName.Trim();
                convertAccount.StreetAddress = model.StreetAddress.Trim();
                convertAccount.RoleId = model.RoleId;
                convertAccount.Role = currenRole;
                Account accountCreated = await _accountService.AddAsync(convertAccount);
                if (accountCreated != null)
                {
                    return CreatedAtAction("GetAccountByType", new { search = accountCreated.Id }, _mapper.Map<AccountProfileVM>(accountCreated));
                }
                return BadRequest(new
                {
                    message = "Create account failed!"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
