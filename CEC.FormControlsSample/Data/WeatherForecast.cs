using System;
using System.ComponentModel.DataAnnotations;

namespace CEC.FormControlsSample.Data
{
    public class WeatherForecast : IDbRecord<WeatherForecast>
    {
        public int ID { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now.Date;

        [Required]
        [Range(-40, 60, ErrorMessage ="Only Temperatures between -40 an 60 are allowed.")]
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

        public WeatherForecast ShadowCopy()
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
