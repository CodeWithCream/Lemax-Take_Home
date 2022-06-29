using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lemax_Take_Home.DTOs
{
    public class GeolocationDto : IEquatable<GeolocationDto>
    {
        [Range(-180.0, 180.0, ErrorMessage = "Value for {0}  must be in the range {1} and {2}")]
        [DefaultValue(15.9471705)]
        public double Longitude { get; set; }

        [Range(-90.0, 90.0, ErrorMessage = "Value for {0}  must be in the range {1} and {2}")]
        [DefaultValue(45.7683477)]
        public double Latitude { get; set; }

        #region IEquatable<GeolocationDto> Members
        public bool Equals(GeolocationDto? other)
        {
            return other != null
                && Longitude == other.Longitude
                && Latitude == other.Latitude;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as GeolocationDto);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Longitude.GetHashCode();
                hash = hash * 23 + Latitude.GetHashCode();
                return hash;
            }
        }

        #endregion

        public static void Validate(GeolocationDto geolocation)
        {

        }
    }
}
