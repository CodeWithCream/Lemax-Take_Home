using GeoSpatialLib;
using NetTopologySuite.Geometries;

namespace GeoSpatialLibTests
{
    /// <summary>
    /// Calculations checked based on https://www.meridianoutpost.com/resources/etools/calculators/calculator-latitude-longitude-distance.php?
    /// </summary>
    [TestClass]
    public class GeoDistanceTests
    {
        [TestMethod]
        public void Get_Distance_With_Equal_Points_Returns_0()
        {
            var point1 = new Point(15.8779049, 45.7539264);
            var point2 = new Point(15.8779049, 45.7539264);

            var distance = GeoDistance.Calculate(point1, point2);

            Assert.AreEqual(0, distance);
        }
               
        [TestMethod]
        public void Get_Distance_Returns_Distance_Between_Two_Points_On_The_Same_Hemisphere()
        {

            var point1 = new Point(15.8779049, 45.7539264);
            var point2 = new Point(15.9287167, 45.7612322);
            var expectedDistance = 4.02;

            var distance = Math.Round(GeoDistance.Calculate(point1, point2), 2);

            Assert.AreEqual(expectedDistance, distance);
        }

        [TestMethod]
        public void Get_Distance_Returns_Distance_Between_Two_Points_On_The_Opposite_Hemisphere()
        {

            var point1 = new Point(15.8779049, 45.7539264);
            var point2 = new Point(-15.9287167, 45.7612322);
            var expectedDistance = 2450.97;

            var distance = Math.Round(GeoDistance.Calculate(point1, point2), 2);

            Assert.AreEqual(expectedDistance, distance);
        }

        [TestMethod]
        public void Get_Distance_Opration_Is_Comutative()
        {
            var point1 = new Point(15.8779049, 45.7539264);
            var point2 = new Point(15.9287167, 45.7612322);

            var distance1 = GeoDistance.Calculate(point1, point2);
            var distance2 = GeoDistance.Calculate(point2, point1);

            Assert.AreEqual(distance1, distance2);
        }
    }
}