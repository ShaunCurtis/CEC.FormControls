using CEC.FormControlsSample.Components;
using CEC.FormControlsSample.Data;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEC.FormControlsSample.Pages
{
    public partial class WeatherForecastEditor : EditorComponentBase
    {
        public WeatherForecast Record { get; set; } = new WeatherForecast();

        public WeatherForecast ShadowRecord { get; set; }

        protected string CardBorderColour => this.IsClean ? "border-secondary" : "border-danger";

        protected string CardHeaderColour => this.IsClean ? "bg-secondary text-white" : "bg-danger text-white";

        protected override Task OnInitializedAsync()
        {
            // Set up the Edit Context
            this.EditContext = new EditContext(this.Record);

            // Register with the Edit Context OnFieldChanged Event
            this.EditContext.OnFieldChanged += OnFieldChanged;

            // Make a copy of the existing record - in this case it's always new but in the real world that won't be the case
            this.ShadowRecord = this.Record.Copy();

            // Get the actual page Url from the Navigation Manager
            this.PageUrl = this.NavManager.Uri;

            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Event handler for when a edit form field change takes place
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnFieldChanged(object sender, EventArgs e)
        {
            this.ExitAttempt = false;
            // Check the EditContext State and set the ISClean Property accordingly
            this.IsClean = !this.EditContext.IsModified();
            // opens the Alert if the record is dirty
            if (this.IsClean) this.Alert.ClearAlert();
            else this.Alert.SetAlert("The Forecast Has Changed", Alert.AlertWarning);
        }

        /// <summary>
        /// Save Method called from the Button
        /// </summary>
        protected void Save()
        {
            // Set the Shadow Copy to a copy of the current record
            // Normally run a Save/Create CRUD operation here
            this.ShadowRecord = this.Record.Copy();
            // Set the EditContext State
            this.EditContext.MarkAsUnmodified();
            this.IsClean = true;
            this.Alert.SetAlert("Forecast Saved", Alert.AlertSuccess);
            this.StateHasChanged();
        }

        /// <summary>
        /// Cancel Method called from the Button
        /// </summary>
        protected void Cancel()
        {
            this.ExitAttempt = false;
            if (this.IsClean) this.Alert.ClearAlert();
            else this.Alert.SetAlert("Forecast Changed", Alert.AlertWarning);
            this.StateHasChanged();
        }

        /// <summary>
        /// Confirm Exit Method called from the Button
        /// </summary>
        protected void ConfirmExit()
        {
            // To escape a dirty component set IsClean manually and navigate.
            this.IsClean = true;
            this.NavManager.NavigateTo("/Index");
        }

    }
}
