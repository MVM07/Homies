namespace Homies.Models
{
    public class EventAllViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime Start { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Organiser { get; set; } = string.Empty;
    }
}
