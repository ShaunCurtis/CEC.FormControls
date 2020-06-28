using System;

namespace CEC.FormControlsSample.Data
{
    public class WeatherForecast
    {

        public DateTime Date { get; set; } = DateTime.Now.Date;

        public int TemperatureC { get; set; } = 20;

        public bool Frost { get; set; }

        public OutlookType Outlook { get; set; } = OutlookType.Sunny;

        public string Description { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public enum OutlookType
        {
            Sunny = 1,
            Rainy = 2,
            Cloudy = 3
        }

        public WeatherForecast Copy()
        {
            return new WeatherForecast() {
                Date = this.Date,
                TemperatureC = this.TemperatureC,
                Frost = this.Frost,
                Outlook = this.Outlook,
                Description = this.Description,
                Summary = this.Summary
            };
        }
    }
}
