using Homies.Contracts;
using Homies.Data;
using Homies.Data.Models;
using Homies.Data.ValidationConstants;
using Homies.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Homies.Services
{
    public class EventService : IEventService
    {
        private readonly HomiesDbContext context;

        public EventService(HomiesDbContext _context)
        {
            context = _context; 
        }

        public async Task AddEventAsync(Event model)
        {
            await context.Events.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task EditEventAsync(Event eventToEdit, EventFormViewModel model, DateTime start, DateTime end)
        {
            eventToEdit.Name = model.Name;
            eventToEdit.Description = model.Description;
            eventToEdit.Start = start;
            eventToEdit.End = end;
            eventToEdit.TypeId = model.TypeId;

            await context.SaveChangesAsync();
        }

        public async Task<bool> EventExistsAsync(int id)
        {
            bool eventExists = await context.Events.AnyAsync(e => e.Id == id);
            
            return eventExists;
        }

        public async Task<IEnumerable<EventAllViewModel>> GetAllEventsAsync()
        {
            var events = await context.Events
                .Select(e => new EventAllViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start,
                    Organiser = e.Organiser.UserName,
                    Type = e.Type.Name
                })
                .ToListAsync();

            return events;
        }

        public async Task<EventDetailsViewModel> GetEventDetailsAsync(int id)
        {
            var eventDetails = await context.Events
                .Where(e => e.Id == id)
                .Include(e => e.Organiser)
                .Include(e => e.Type)                
                .FirstOrDefaultAsync();

            if (eventDetails == null)
            {
                throw new ArgumentNullException("No such details exist.");
            }

            EventDetailsViewModel model = new EventDetailsViewModel()
            {
                Id = eventDetails.Id,
                Name = eventDetails.Name,
                Description = eventDetails.Description,
                CreatedOn = eventDetails.CreatedOn.ToString(ValidationConstants.DateFormat),
                Start = eventDetails.Start.ToString(ValidationConstants.DateFormat),
                End = eventDetails.End.ToString(ValidationConstants.DateFormat),
                Organiser = eventDetails.Organiser.UserName,
                Type = eventDetails.Type.Name
            };

            return model;
        }

        public async Task<Event?> GetEventToEditAsync(int id)
        {
            var eventToEdit = await context.Events.FindAsync(id);

            return eventToEdit;
        }

        public async Task<IEnumerable<EventAllViewModel>> GetMyJoinedEventsAsync(string currentUserId)
        {
            var myJoinedEvents = await context.EventsParticipants
                .Where(e => e.HelperId == currentUserId)
                .Select(e => new EventAllViewModel()
                {
                    Id = e.Event.Id,
                    Name = e.Event.Name,    
                    Start = e.Event.Start,
                    Type = e.Event.Type.Name,
                    Organiser = e.Event.Organiser.UserName
                })
                .ToListAsync();

            return myJoinedEvents;
        }

        public async Task<IEnumerable<TypeViewModel>> GetTypesAsync()
        {
            var types = await context.Types
                .Select(t => new TypeViewModel()
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();

            return types;
        }

        public async Task JoinEventAsync(string currentUserId, int id)
        {          
            var participatesInEvent = await context.EventsParticipants
                .Where(ep => ep.HelperId == currentUserId && ep.EventId == id)
                .FirstOrDefaultAsync();

            if (participatesInEvent == null)
            {
                EventParticipant ep = new EventParticipant()
                {
                    HelperId = currentUserId,
                    EventId = id,
                };

                await context.EventsParticipants.AddAsync(ep);
                await context.SaveChangesAsync();                
            }
        }

        public async Task LeaveEventAsync(string currentUserId, int id)
        {
            var eventToRemove = await context.EventsParticipants
                .Where(ep => ep.HelperId == currentUserId && ep.EventId == id)
                .FirstOrDefaultAsync();

            if (eventToRemove != null)
            {
                context.EventsParticipants.Remove(eventToRemove);
                await context.SaveChangesAsync();   
            }
        }
    }
}
