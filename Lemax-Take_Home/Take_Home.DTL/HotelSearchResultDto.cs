namespace Take_Home.DTL
{
    public class HotelSearchResultDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public float Price { get; set; }
        public double Distance { get; set; }
        public double Fit { get; set; }
    }
}
