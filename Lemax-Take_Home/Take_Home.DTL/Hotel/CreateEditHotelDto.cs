using Lemax_Take_Home.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Take_Home.DTL.Hotel
{
    public class CreateEditHotelDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Value for {0} cannot be empty")]
        [StringLength(100)]
        [DefaultValue("new hotel")]
        public string Name { get; set; } = null!;

        [Range(0, float.MaxValue, ErrorMessage = "Value for {0} must be must be between {1} and {2}")]
        [DefaultValue(50)]
        public float Price { get; set; }

        [Required]
        public GeolocationDto Geolocation { get; set; } = null!;
    }
}
