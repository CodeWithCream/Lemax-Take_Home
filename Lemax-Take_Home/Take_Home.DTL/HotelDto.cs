using System.ComponentModel.DataAnnotations;

namespace Lemax_Take_Home.DTOs
{
    public class HotelDto : IEquatable<HotelDto>
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public float Price { get; set; }
        public GeolocationDto Geolocation { get; set; } = null!;


        #region IEquatable<HotelDto> Members
        public bool Equals(HotelDto? other)
        {
            return other != null
                && Id == other.Id
                && Name == other.Name
                && Price == other.Price
                && Geolocation.Equals(other.Geolocation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as HotelDto);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
