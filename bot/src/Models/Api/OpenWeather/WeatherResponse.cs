using System.Collections.Generic;

namespace Shinobu.Models.Api.OpenWeather
{
    public class WeatherResponse
    {
        public int Id;
        public long Dt;
        public string Name;
        public int Cod;
        
        public Coordinates Coord;
        public IList<Weather> Weather;
        public Info Main;
        public Wind Wind;
        public RegionInfo Sys;
    }
}