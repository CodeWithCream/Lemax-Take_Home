using System.ComponentModel.DataAnnotations;

namespace Lemax_Take_Home.DTOs
{
    public class CreateEditHotelDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Value for {0} cannot be empty")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Range(0, float.MaxValue, ErrorMessage = "Value for {0} must be must be between {1} and {2}")]
        public float Price { get; set; }

        [Required]
        public GeolocationDto Geolocation { get; set; } = null!;
    }
}
