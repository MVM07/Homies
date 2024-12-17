using System.ComponentModel.DataAnnotations;
using static Homies.Data.ValidationConstants.ValidationConstants;

namespace Homies.Models
{
    public class TypeViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(TypeNameMaxLength,
            MinimumLength = TypeNameMinLength)]
        public string Name { get; set; } = string.Empty;
    }
}