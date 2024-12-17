using Homies.Contracts;
using Homies.Data.Models;
using Homies.Data.ValidationConstants;
using Homies.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Homies.Controllers
{
    public class EventController : BaseController
    {
        private readonly IEventService eventService;

        public EventController(IEventService _eventService)
        {
            eventService = _eventService;
        }

        public async Task<IActionResult> All()
        {
            var events = await eventService.GetAllEventsAsync();

            return View(events);
        }

        public async Task<IActionResult> Join(int id)
        {
            bool eventExists = await eventService.EventExistsAsync(id);

            if (eventExists == false)
            {
                ModelState.AddModelError("", "No such event exists.");

                return View(nameof(All));
            }
            string currentUserId = GetCurrentUserId();

            await eventService.JoinEventAsync(currentUserId, id);

            return RedirectToAction("Joined", "Event");
        }

        public async Task<IActionResult> Leave(int id)
        {
            string currentUserId = GetCurrentUserId();

            await eventService.LeaveEventAsync(currentUserId, id);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Joined()
        {
            string currentUserId = GetCurrentUserId();

            var myJoinedEvents = await eventService.GetMyJoinedEventsAsync(currentUserId);

            return View(myJoinedEvents);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            EventFormViewModel model = new EventFormViewModel()
            {
                Types = await eventService.GetTypesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Types = await eventService.GetTypesAsync();

                return View(model);
            }

            List<DateTime> dates = CheckForCorrectDateFormat(model.Start, model.End);            

            Event eventToAdd = new Event()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedOn = DateTime.Now,
                Start = dates[0],
                End = dates[1],
                TypeId = model.TypeId,
                OrganiserId = GetCurrentUserId()
            };

            await eventService.AddEventAsync(eventToAdd);

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventToEdit = await eventService.GetEventToEditAsync(id);

            if (eventToEdit == null)
            {
                return BadRequest();
            }

            if (eventToEdit.OrganiserId != GetCurrentUserId())
            {
                return Unauthorized();
            }

            EventFormViewModel model = new EventFormViewModel()
            {
                Name = eventToEdit.Name,
                Description = eventToEdit.Description,
                Start = eventToEdit.Start.ToString(ValidationConstants.DateFormat),
                End = eventToEdit.End.ToString(ValidationConstants.DateFormat),
                Types = await eventService.GetTypesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventFormViewModel model, int id)
        {
            var eventToEdit = await eventService.GetEventToEditAsync(id);
            
            if (eventToEdit == null)
            {
                return BadRequest();
            }

            if (eventToEdit.OrganiserId != GetCurrentUserId())
            {
                return Unauthorized();
            }

            List<DateTime> dates = CheckForCorrectDateFormat(model.Start, model.End);

            if (!ModelState.IsValid)
            {
                model.Types = await eventService.GetTypesAsync();

                return View(model);
            }

            await eventService.EditEventAsync(eventToEdit, model, dates[0], dates[1]);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Details(int id)
        {
            var eventDetails = await eventService.GetEventDetailsAsync(id);

            return View(eventDetails);
        }


        public List<DateTime> CheckForCorrectDateFormat(string startDate, string endDate) 
        {
           List<DateTime> result = new List<DateTime>();

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            if (!DateTime.TryParseExact(startDate, ValidationConstants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
            {
                ModelState.AddModelError(nameof(startDate), $"Invalid date format. Format should be {ValidationConstants.DateFormat}");
            }

            if (!DateTime.TryParseExact(endDate, ValidationConstants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
            {
                ModelState.AddModelError(nameof(endDate), $"Invalid date format. Format should be {ValidationConstants.DateFormat}");
            }

            result.AddRange(new[] { start, end });

            return result;
        }
    }
}
