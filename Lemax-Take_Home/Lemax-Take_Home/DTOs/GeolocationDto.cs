using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lemax_Take_Home.DTOs
{
    public class GeolocationDto
    {
        [Range(-180.0, 180.0, ErrorMessage = "Value for {0}  must be in the range {1} and {2}")]
        [DefaultValue(15.9471705)]
        public double Longitude { get; set; }

        [Range(-90.0, 90.0, ErrorMessage = "Value for {0}  must be in the range {1} and {2}")]
        [DefaultValue(45.7683477)]
        public double Latitude { get; set; }
    }
}
