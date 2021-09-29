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
    [Route("api/v1/roles")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Role> _pagingSupport;

        public RoleController(IRoleService roleService, IMapper mapper, IPagingSupport<Role> pagingSupport)
        {
            _roleService = roleService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get list roles
        /// </summary>
        /// <returns>List roles</returns>
        /// <response code="200">Returns list roles</response>
        /// <response code="404">Not found roles</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<RoleVM>> GetAllRole(
            [FromQuery(Name = "name")] string name,
            [FromQuery(Name = "filtering")] string filters = null,
            int offset = 1,
            int limit = 20
        )
        {
            try
            {
                IQueryable<Role> roleList = _roleService.GetAll();
                if (!String.IsNullOrEmpty(name))
                {
                    roleList = roleList.Where(s => s.Name.ToUpper().Contains(name.Trim().ToUpper()));
                }
                Paged<RoleVM> paged = _pagingSupport.From(roleList).GetRange(offset, limit, s => s.Id, 1).Paginate<RoleVM>();
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(RoleVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(RoleVM), splitFilter, paged);
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
        /// Get role by id
        /// </summary>
        /// <returns>Return the role with the corresponding id</returns>
        /// <response code="200">Returns the role type with the specified id</response>
        /// <response code="404">No role found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<RoleVM>> GetRoleById(int id)
        {
            try
            {
                Role currentRole = await _roleService.GetByIdAsync(id);
                if (currentRole != null)
                {
                    return Ok(_mapper.Map<RoleVM>(currentRole));
                }
                return NotFound("Can not found role by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <response code="200">Created new role successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<RoleVM>> CreateRole([FromBody] RoleCM model)
        {
            Role role = _mapper.Map<Role>(model);
            try
            {
                bool isDuplicated = _roleService.GetAll().Where(s => s.Name.Trim().ToUpper().Equals(model.Name.Trim().ToUpper())).FirstOrDefault() != null;
                if(isDuplicated)
                {
                    return BadRequest(new
                    {
                        message = "Role Name is duplicated"
                    });
                }
                role.Name = role.Name.ToUpper();
                Role roleCreated = await _roleService.AddAsync(role);
                if (roleCreated != null)
                {
                    return CreatedAtAction("GetRoleById", new { id = roleCreated.Id }, _mapper.Map<RoleVM>(roleCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update a role
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<RoleVM>> PutRole(int id, [FromBody] RoleUM model)
        {
            Role currentRole = await _roleService.GetByIdAsync(id);
            if (currentRole == null)
            {
                return NotFound();
            }
            if (currentRole.Id != model.Id)
            {
                return BadRequest();
            }
            if(!currentRole.Name.ToUpper().Equals(model.Name.ToUpper()) && _roleService.GetAll().Where(s => s.Name.Trim().ToUpper().Equals(model.Name.Trim().ToUpper())).FirstOrDefault() != null)
            {
                return BadRequest(new
                {
                    message = "Role Name is duplicated!"
                });
            }
            try
            {
                currentRole.Name = model.Name.ToUpper();
                bool isUpdated = await _roleService.UpdateAsync(currentRole);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<RoleVM>(currentRole));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete role By Id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            Role currentRole = await _roleService.GetByIdAsync(id);
            if (currentRole == null)
            {
                return BadRequest(new
                {
                    message = "Can not found role by id: " + id
                });
            }
            try
            {
                bool isDeleted = await _roleService.DeleteAsync(currentRole);
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
