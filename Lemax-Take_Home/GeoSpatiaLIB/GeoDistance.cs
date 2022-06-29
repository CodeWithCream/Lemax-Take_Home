using NetTopologySuite.Geometries;

namespace GeoSpatialLib
{
    /// <summary>
    /// Operations with geo data
    /// </summary>
    public static class GeoDistance
    {
        private const double MinutesInDegree = 60;
        /// <summary>
        /// Calculates distance between 2 coordinates
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>Calculated distance in kilometers</returns>
        public static double Calculate(Point point1, Point point2)
        {
            if (point1.CompareTo(point2) == 0)
            {
                return 0;
            }
            else
            {
                var theta = point1.X - point2.X;
                var distance_rad = Math.Acos(
                    Math.Sin(Deg2rad(point1.Y)) * Math.Sin(Deg2rad(point2.Y)) +
                    Math.Cos(Deg2rad(point1.Y)) * Math.Cos(Deg2rad(point2.Y)) * Math.Cos(Deg2rad(theta)));
                var distance_deg = Rad2deg(distance_rad);
                var distance_miles = NauticalMileToMile(distance_deg * MinutesInDegree);

                return Miles2Km(distance_miles);
            }
        }

        /// <summary>
        /// Converts decimal degrees to radians
        /// </summary>
        private static double Deg2rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        /// <summary>
        /// converts radians to decimal degrees
        /// </summary>
        private static double Rad2deg(double rad)
        {
            return rad / Math.PI * 180.0;
        }

        private static double Miles2Km(double miles)
        {
            return miles * 1.609344;
        }

        private static double NauticalMileToMile(double nauticalMile)
        {
            return nauticalMile * 1.1515;
        }
    }
}
