using CEC.FormControlsSample.Components;
using CEC.FormControlsSample.Data;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace CEC.FormControlsSample.Pages
{
    /// <summary>
    /// Editor for Waether Forecast
    /// Almost all of the code is in the boiler plate EditComponentBase class
    /// </summary>
    public partial class WeatherForecastEditor : EditorComponentBase<WeatherForecast>
    {

        protected override string DirtyMessage => "The Weather has changed!";

        protected override string SavedMessage => "The Weather has been saved";

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Save Method called from the Button
        /// </summary>
        protected override void Save()
        {
            base.Save();
        }

        /// <summary>
        /// Cancel Method called from the Button
        /// </summary>
        protected override void Cancel()
        {
            base.Cancel();
        }

    }
}
