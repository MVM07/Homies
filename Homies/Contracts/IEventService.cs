using Homies.Data.Models;
using Homies.Models;

namespace Homies.Contracts
{
    public interface IEventService
    {
        Task<IEnumerable<EventAllViewModel>> GetAllEventsAsync();

        Task JoinEventAsync(string currentUserId, int id);

        Task<bool> EventExistsAsync(int id);

        Task LeaveEventAsync(string currentUserId, int id);

        Task <IEnumerable<EventAllViewModel>> GetMyJoinedEventsAsync(string currentUserId);

        Task<IEnumerable<TypeViewModel>> GetTypesAsync();

        Task AddEventAsync(Event model);

        Task<Event?> GetEventToEditAsync(int id);

        Task<EventDetailsViewModel> GetEventDetailsAsync(int id);

        Task EditEventAsync(Event eventToEdit, EventFormViewModel model, DateTime start, DateTime end);
    }
}
