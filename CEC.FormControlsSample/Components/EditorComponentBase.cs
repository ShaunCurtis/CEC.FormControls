using CEC.FormControlsSample.Data;
using CEC.Routing.Components;
using CEC.Routing.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Threading.Tasks;

namespace CEC.FormControlsSample.Components
{
    public class EditorComponentBase<TRecord> : ComponentBase, IRecordRoutingComponent where TRecord : IDbRecord<TRecord>, new()
    {

        /// <summary>
        /// Injected Navigation Manager
        /// </summary>
        [Inject]
        public NavigationManager NavManager { get; set; }

        /// <summary>
        /// Injected User Session Object
        /// </summary>
        [Inject]
        public RouterSessionService RouterSessionService { get; set; }

        public TRecord Record { get; set; } = new TRecord();

        public TRecord ShadowRecord { get; set; }

        /// <summary>
        /// IRecordRoutingComponent implementation
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// IRecordRoutingComponent implementation
        /// </summary>
        public bool IsClean { get; set; } = true;

        /// <summary>
        /// Boolean property set when the user attempts to exit a dirty component
        /// </summary>
        protected bool ExitAttempt { get; set; }

        /// <summary>
        /// Form Edit Context
        /// </summary>
        public EditContext EditContext { get; set; }

        /// <summary>
        /// Alert object used in UI by UI Alert
        /// </summary>
        public Alert Alert { get; set; } = new Alert();

        /// <summary>
        /// Message to dispay when the record is dirty
        /// </summary>
        protected virtual string DirtyMessage { get; } = "The record is unsaved.";

        /// <summary>
        /// Message to dispay when the record is dirty
        /// </summary>
        protected virtual string SavedMessage { get; } = "The record has been saved.";

        /// <summary>
        /// Message to dispay when the navigation cancelled
        /// </summary>
        protected virtual string NavigationCancelledMessage { get; } = "<b>RECORD ISN'T SAVED</b>. Either Cancel or Exit Without Saving.";

        protected string CardBorderColour => this.IsClean ? "border-secondary" : "border-danger";

        protected string CardHeaderColour => this.IsClean ? "bg-secondary text-white" : "bg-danger text-white";

        protected override Task OnInitializedAsync()
        {
            // Set up the Edit Context
            this.EditContext = new EditContext(this.Record);

            // Make a copy of the existing record - in this case it's always new but in the real world that won't be the case
            this.ShadowRecord = this.Record.ShadowCopy();

            this.PageUrl = this.NavManager.Uri;
            this.RouterSessionService.ActiveComponent = this;
            this.RouterSessionService.NavigationCancelled += OnNavigationCancelled;
            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Event Handler for the Navigation Cancelled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnNavigationCancelled(object sender, EventArgs e)
        {
            this.ExitAttempt = true;
            this.Alert.SetAlert(this.NavigationCancelledMessage, Alert.AlertDanger);
            InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        /// Event handler for the RecordFromControls FieldChanged Event
        /// </summary>
        /// <param name="changestate"></param>
        protected virtual void RecordFieldChanged(bool changeState)
        {
            if (this.EditContext != null)
            {
                this.ExitAttempt = false;
                this.IsClean = !changeState;
                this.CheckClean();
            }
        }

        protected void CheckClean(bool setclean = false)
        {
            if (setclean) this.IsClean = true;
            if (this.IsClean)
            {
                this.Alert.ClearAlert();
                this.RouterSessionService.SetPageExitCheck(false);
            }
            else
            {
                this.RouterSessionService.SetPageExitCheck(true);
                this.Alert.SetAlert(this.DirtyMessage, Alert.AlertWarning);
            }
        }

        /// <summary>
        /// Save Method called from the Button
        /// </summary>
        protected virtual void Save()
        {
            // Set the Shadow Copy to a copy of the current record
            // Normally run a Save/Create CRUD operation here
            this.ShadowRecord = this.Record.ShadowCopy();
            // Set the EditContext State
            this.EditContext.MarkAsUnmodified();
            this.CheckClean(true);
            this.Alert.SetAlert(this.SavedMessage, Alert.AlertSuccess);
            InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        /// Cancel Method called from the Button
        /// </summary>
        protected virtual void Cancel()
        {
            this.ExitAttempt = false;
            this.CheckClean();
            InvokeAsync(this.StateHasChanged);
        }
        /// <summary>
        /// Confirm Exit Method called from the Button
        /// </summary>
        protected virtual void ConfirmExit()
        {
            // To escape a dirty component set IsClean manually and navigate.
            this.CheckClean(true);
            // Exit to the cancelled page (if it exists), otherwise go to the last page.  Exit to root as last resort. 
            if (!string.IsNullOrEmpty(this.RouterSessionService.NavigationCancelledUrl)) this.NavManager.NavigateTo(this.RouterSessionService.NavigationCancelledUrl);
            else if (!string.IsNullOrEmpty(this.RouterSessionService.LastPageUrl)) this.NavManager.NavigateTo(this.RouterSessionService.LastPageUrl);
            else this.NavManager.NavigateTo("/");
        }


    }
}
