using CEC.FormControlsSample.Components;
using CEC.FormControlsSample.Data;

namespace CEC.FormControlsSample.Pages
{
    /// <summary>
    /// Editor for Weather Forecast
    /// All of the code is in the boiler plate EditComponentBase class
    /// </summary>
    public partial class WeatherForecastEditor : EditorComponentBase<WeatherForecast>
    {

        protected override string DirtyMessage => "The Weather has changed!";

        protected override string SavedMessage => "The Weather has been saved";

    }
}
