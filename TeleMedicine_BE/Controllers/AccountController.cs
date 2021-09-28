using AutoMapper;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Account> _pagingSupport;

        public AccountController(IAccountService accountService, IMapper mapper, IPagingSupport<Account> pagingSupport)
        {
            _accountService = accountService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        [HttpGet]
        public ActionResult<Paged<AccountManageVM>> GetAll(
            [FromQuery(Name = "first-name")] string firstName,
            [FromQuery(Name = "last-name")] string lastName,
            [FromQuery(Name = "phone")] string phone,
            [FromQuery(Name = "email")] string email,
            [FromQuery(Name = "filtering")] string filters = null,
            int offset = 1,
            int limit = 20
        )
        {
            try
            {
                IQueryable<Account> accountsQuery = _accountService.GetAll(_ => _.Role);
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
                Paged<AccountManageVM> paged = _pagingSupport.From(accountsQuery).GetRange(offset, limit, s => s.RegisterTime, 1).Paginate<AccountManageVM>();
                if (!String.IsNullOrEmpty(filters))
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

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountProfileVM>> GetAccountById([FromRoute] int id)
        {
            try
            {
                Account account = await _accountService.GetByIdAsync(id);
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

        [HttpPut]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        [Produces("application/json")]
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
    }
}
