using AutoMapper;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleMedicine_BE.Utils;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Notification> _pagingSupport;

        public NotificationController(INotificationService notificationService, IMapper mapper, IAccountService accountService, IPagingSupport<Notification> pagingSupport)
        {
            _notificationService = notificationService;
            _mapper = mapper;
            _accountService = accountService;
            _pagingSupport = pagingSupport;
        }

        /// <summary>
        /// Get all notifications
        /// </summary>
        /// <returns>All roles</returns>
        /// <response code="200">Returns all notifications</response>
        /// <response code="404">Not found notifications</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<NotificationVM>> GetAllRole(
            [FromQuery(Name = "content")] String content,
            [FromQuery(Name = "user-id")] int[] userId,
            [FromQuery(Name = "start-date")] DateTime? startDate,
            [FromQuery(Name = "end-date")] DateTime? endDate,
            int offset = 1,
            int limit = 20
        )
        {
            try
            {
                IQueryable<Notification> notifications = _notificationService.GetAll();
                if (!String.IsNullOrEmpty(content))
                {
                    notifications = notifications.Where(s => s.Content.ToUpper().Contains(content.Trim().ToUpper()));
                }
                if(userId != null && userId.Length > 0)
                {
                    notifications = notifications.Where(s => userId.Contains(s.UserId));
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
                Paged<NotificationVM> paged = _pagingSupport.From(notifications).GetRange(offset, limit, s => s.Id, 1).Paginate<NotificationVM>();
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
        /// Create a new notification
        /// </summary>
        /// <response code="201">Created new notification</response>
        /// <response code="400">Field is not matched or duplicated. </response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<NotificationVM>> CreateDrug([FromBody] NotificationCM model)
        {
            Account account = await _accountService.GetByIdAsync(model.UserId);
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
                notification.User = account;
                Notification notificationCreated = await _notificationService.AddAsync(notification);

                if (notificationCreated != null)
                {
                    return CreatedAtAction("GetNotificationyId", new { id = notificationCreated.Id }, _mapper.Map<NotificationVM>(notificationCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete notification
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
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
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<NotificationVM>> UpdateNotification(int id, [FromBody] NotificationUM model)
        {
            Account currentUser = await _accountService.GetByIdAsync(model.UserId);
            if (currentUser == null)
            {
                return BadRequest(new
                {
                    message = "User is not exist."
                });
            }

            Notification notification = await _notificationService.GetByIdAsync(model.Id);
            if (notification == null)
            {
                return BadRequest(new
                {
                    message = "Notification is not exist."
                });
            }
            if(id != notification.Id)
            {
                return BadRequest();
            }
            try
            {
                notification.User = currentUser;
                notification.UserId = currentUser.Id;
                notification.CreatedDate = model.CreatedDate;
                notification.Content = model.Content;
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
