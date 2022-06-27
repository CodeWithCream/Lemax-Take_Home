using System.ComponentModel.DataAnnotations;

namespace Lemax_Take_Home.DTOs
{
    public class HotelDto
    {
        public long Id { get; set; }
        public string Name { get; protected set; } = null!;
        public float Price { get; set; }
        public GeolocationDto Geolocation { get; set; } = null!;
    }
}
