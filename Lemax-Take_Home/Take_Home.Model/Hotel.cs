using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Take_Home.DAL.InMemory"),
            InternalsVisibleTo("Take_Home.Services"),
           InternalsVisibleTo("Take_Home.Services.Tests")]
namespace Take_Home.Model
{
    public class Hotel : IEquatable<Hotel>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; }

        [Required]
        public Point Geolocation { get; set; }

        public Hotel(string name, float price, Point geolocation)
        {
            Validate(name, price, geolocation);

            Name = name;
            Price = price;
            Geolocation = geolocation;
        }
        public static void Validate(string name, float price, Point geolocation)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be empty", nameof(name));
            }
            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than 0", nameof(price));
            }
            if (geolocation.X < -180 || geolocation.X > 180)
            {
                throw new ArgumentException("Longitude must be in the range -180 and +180", nameof(geolocation));
            }
            if (geolocation.Y < -90 || geolocation.Y > 90)
            {
                throw new ArgumentException("Latitudee must be in the range -90 and +90", nameof(geolocation));
            }
        }

        #region IEquatable<Hotel> Members
        public bool Equals(Hotel? other)
        {
            return other != null
                && Id == other.Id
                && Name == other.Name
                && Price == other.Price
                && Geolocation.CompareTo(other.Geolocation) == 0;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Hotel);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}