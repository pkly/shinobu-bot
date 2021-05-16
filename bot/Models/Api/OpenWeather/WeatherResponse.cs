using System.Collections.Generic;

namespace Shinobu.Models.Api.OpenWeather
{
    public class WeatherResponse
    {
        public int Id;
        public long Dt = 0;
        public string Name = "Unknown";
        public int Cod;
        
        public Coordinates Coord = null!;
        public IList<Weather> Weather = new List<Weather>();
        public Info Main = null!;
        public Wind Wind = null!;
        public RegionInfo Sys = null!;
    }
}