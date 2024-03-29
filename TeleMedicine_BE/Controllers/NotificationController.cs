﻿using AutoMapper;
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
using TeleMedicine_BE.ExternalService;
using TeleMedicine_BE.Utils;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Notification> _pagingSupport;
        private readonly IRedisService _redisService;
        private readonly IPushNotificationService _pushNotification;

        public NotificationController(INotificationService notificationService, IMapper mapper, IAccountService accountService, IPagingSupport<Notification> pagingSupport, IRedisService redisService, IPushNotificationService pushNotificationService)
        {
            _notificationService = notificationService;
            _mapper = mapper;
            _accountService = accountService;
            _pagingSupport = pagingSupport;
            _redisService = redisService;
            _pushNotification = pushNotificationService;
        }

        /// <summary>
        /// Get list notifications
        /// </summary>
        /// <returns>All notifications</returns>
        /// <response code="200">Returns list notifications</response>
        /// <response code="404">Not found notifications</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<NotificationVM>>> GetAllRole(
            [FromQuery(Name = "content")] string content,
            [FromQuery(Name = "user-id")] int[] userId,
            [FromQuery(Name = "start-date")] DateTime? startDate,
            [FromQuery(Name = "end-date")] DateTime? endDate,
            [FromQuery(Name = "is-active")] bool? isActive,
            [FromQuery(Name = "order-by")] NotificationFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "filtering")] string filters = null,
            [FromQuery(Name = "page-offset")]  int pageOffset = 1,
            int limit = 20
        )
        {
            try
            {
                IQueryable<Notification> notifications = _notificationService.GetAll();
                if (!string.IsNullOrEmpty(content))
                {
                    notifications = notifications.Where(s => s.Content.ToUpper().Contains(content.Trim().ToUpper()));
                }
                if(userId != null && userId.Length > 0)
                {
                    notifications = notifications.Where(s => userId.Contains(s.UserId));
                }
                if(isActive.HasValue)
                {
                    notifications = notifications.Where(s => s.IsActive.Value.Equals(isActive.Value));
                }
                if(startDate.HasValue && endDate.HasValue)
                {
                    notifications = notifications.Where(s => s.CreatedDate >= startDate).Where(s => s.CreatedDate <= endDate);
                }else
                {
                    if(startDate.HasValue)
                    {
                        DateTime today = startDate.Value;
                        DateTime mid = today.AddDays(1).AddSeconds(-1);
                        notifications = notifications.Where(s => s.CreatedDate >= today).Where(s => s.CreatedDate <= mid);
                    }
                    if(endDate.HasValue)
                    {
                        DateTime today = endDate.Value;
                        DateTime mid = today.AddDays(1).AddSeconds(-1);
                        notifications = notifications.Where(s => s.CreatedDate >= today).Where(s => s.CreatedDate <= mid);
                    }
                }


                Paged<NotificationVM> paged = null;
                if (orderType == SortTypeEnum.asc && typeof(NotificationVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(notifications).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 0).Paginate<NotificationVM>();
                }
                else if (orderType == SortTypeEnum.desc && typeof(NotificationVM).GetProperty(orderBy.ToString()) != null)
                {
                    paged = _pagingSupport.From(notifications).GetRange(pageOffset, limit, p => EF.Property<object>(p, orderBy.ToString()), 1).Paginate<NotificationVM>();
                }
                else
                {
                    paged = _pagingSupport.From(notifications).GetRange(pageOffset, limit, s => s.Id, 1).Paginate<NotificationVM>();
                }
                if (!String.IsNullOrEmpty(filters))
                {
                    bool checkHasProperty = false;

                    String[] splitFilter = filters.Split(",");
                    foreach (var prop in splitFilter)
                    {
                        if (typeof(NotificationVM).GetProperty(prop) != null)
                        {
                            checkHasProperty = true;
                        }
                    }
                    if (checkHasProperty)
                    {
                        PropertyRenameAndIgnoreSerializerContractResolver jsonIgnore = new PropertyRenameAndIgnoreSerializerContractResolver();
                        string json = jsonIgnore.JsonIgnore(typeof(NotificationVM), splitFilter, paged, PropertyRenameAndIgnoreSerializerContractResolver.IgnoreMode.EXCEPT);
                        return Ok(JsonConvert.DeserializeObject(json));
                    }
                }
                if(userId != null && userId.Length > 0)
                {
                    _ = await _notificationService.SetIsSeen(userId[0]);
                }
                return Ok(paged);
            }
            catch (Exception)
            {
                return BadRequest(startDate.Value);
            }
        }

        /// <summary>
        /// Get a specific notification by notification id
        /// </summary>
        /// <returns>Return the notification with the corresponding id</returns>
        /// <response code="200">Returns the notification type with the specified id</response>
        /// <response code="404">No notification found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public ActionResult<NotificationVM> GetNotificationyId([FromRoute] int id)
        {
            try
            {
                Notification notification = _notificationService.GetAll(_ => _.User).FirstOrDefault(_ => _.Id == id);
                if (notification == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<NotificationVM>(notification));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Count numbers notifications unread
        /// </summary>
        /// <returns>Return the number notifications unread with the corresponding userId</returns>
        /// <response code="200">Returns the number notifications unread with the specified userId</response>
        /// <response code="404">No notification found with the specified userId</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("users/{userId}")]
        [Produces("application/json")]
        public async Task<ActionResult<int>> GetNumberNotificationyUnreadByUserId([FromRoute] int userId)
        {
            
            try
            {
                Account currentAccount = await _accountService.GetByIdAsync(userId);
                if (currentAccount == null)
                {
                    return BadRequest(new
                    {
                        message = "User Id is not exist."
                    });
                }
                int number = _notificationService.GetAll().Where(s => (s.UserId == userId) && !s.IsSeen.Value).Count();
                return Ok(new
                {
                    countOfUnRead = number
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create a new notification
        /// </summary>
        /// <response code="201">Created new notification</response>
        /// <response code="400">Field is not matched or duplicated. </response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<NotificationVM>> CreateNotification([FromBody] NotificationCM model)
        {
            Account account = _accountService.GetAccountByEmail(model.Email);
            if (account == null)
            {
                return BadRequest(new
                {
                    message = "User Id is not exist."
                });
            }
            Notification notification = _mapper.Map<Notification>(model);
            try
            {
                if(account != null)
                {
                    notification.CreatedDate = DateTime.Now;
                    notification.UserId = account.Id;
                    Notification notificationCreated = await _notificationService.AddAsync(notification);

                    if (notificationCreated != null)
                    {
                        await _pushNotification.SendMessage("Bạn đã nhận được một lời mời tham gia", notification.Content, account.Email, null);
                        return CreatedAtAction("GetNotificationyId", new { id = notificationCreated.Id }, _mapper.Map<NotificationVM>(notificationCreated));
                    }
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("connection")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult> MakeConnection([FromBody] NotificationRequest model)
        {
            try
            {
                bool isSuccess = await _redisService.Set("user:" + model.Email, model.Token, 1440);
                if (isSuccess)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
                }
                return BadRequest();
            } catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete notification
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteNotification([FromRoute] int id)
        {
            Notification currentNotification = await _notificationService.GetByIdAsync(id);
            if (currentNotification == null)
            {
                return NotFound();
            }

            try
            {
                bool isDeleted = await _notificationService.DeleteAsync(currentNotification);
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
        /// Update a notification
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<NotificationVM>> UpdateNotification([FromBody] NotificationUM model)
        {
            Account currentUser = await _accountService.GetByIdAsync(model.UserId);
            if (currentUser == null)
            {
                return NotFound(new
                {
                    message = "User is not exist."
                });
            }

            Notification notification = await _notificationService.GetByIdAsync(model.Id);
            if (notification == null)
            {
                return NotFound(new
                {
                    message = "Notification is not exist."
                });
            }
            try
            {
                notification.User = currentUser;
                notification.UserId = currentUser.Id;
                notification.Content = model.Content;
                notification.IsActive = model.IsActive;
                notification.Type = model.Type;
                bool isSuccess = await _notificationService.UpdateAsync(notification);
                if (isSuccess)
                {
                    return Ok(_mapper.Map<NotificationVM>(notification));
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
