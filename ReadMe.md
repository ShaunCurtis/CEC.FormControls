The CEC.FormControls library provides a set of enhanced input controls based on the Blazor standard form controls.

The base control is FormControlBase which is a direct copy of InputBase except CurrentValue is declared as virtual allowing it to be overridden.

The extra functionality is implemented in FormRecordControlBase, and all the useable form controls inherit from this:

Parameters:

        /// <summary>
        /// Gets or Sets the value for the field as stored in the Data Object
        /// Should point to a value that gets updated when the record is saved
        /// </summary>
        [Parameter]
        public TValue RecordValue { get; set; }

        /// <summary>
        /// Is used on some controls to change the way the control is displayed
        /// like readonly but allows greater flexibility
        /// </summary>
        [Parameter]
        public bool Locked { get; set; }

        /// <summary>
        /// Boolean Property that checks if a RecordValue exists and is therefore enabled
        /// </summary>
        private bool _UseRecordValue => RecordValue != null;

Event:

        /// <summary>
        /// Gets or Sets a callback that signals if the change to the current value matches the Original Value
        /// If _UseOriginalValue is false will be called when the value changes - i.e. reflect ValueChanged
        /// </summary>
        [Parameter]
        public EventCallback<bool> ChangedFromRecord { get; set; }

Current Value is overridden and implements extra functionality if the RecordValue is set, otherwise it runs thwe same code as the original.  If RecordValue is set it checks the current value against the RecordValue, updates the EditContext state and invokes the ChangedFromRecord callback event.

All the useable controls are exact copies of the Blazor originals, but inherit from FormRecordControlBase.  Use the controls in a form in the same manner as the standard controls.  The code snippet below shows the date control in use:

        <FormControlDate class="form-control" @bind-Value="this.Record.Date" RecordValue="this.ShadowRecord.Date" ChangedFromRecord="this.RecordFieldChanged"></FormControlDate>

The example project demonstrates the controls in action on a WeatherForecast record.  The project also implements the CEC.Routing library to demonstrate enhanced router navigation control when the editor page is not saved.