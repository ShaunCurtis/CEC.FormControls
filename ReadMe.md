The CEC.FormControls library provides a set of enhanced input controls based on the Blazor standard form controls.

The NuGet package name is CEC.FormControls.

FormControlBase is the base control. It's a direct copy of InputBase with one very important difference - CurrentValue is declared as virtual so it can be overridden.

The extra functionality is implemented in the abstract class FormRecordControlBase.  All useable form controls inherit from this.

FormRecordBase implements three new Properties:

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

One New Event:

        /// <summary>
        /// Gets or Sets a callback that signals if the change to the current value matches the Original Value
        /// If _UseOriginalValue is false will be called when the value changes - i.e. reflect ValueChanged
        /// </summary>
        [Parameter]
        public EventCallback<bool> ChangedFromRecord { get; set; }

Overrides Current Value

If the RecordValue is null it runs the same code as the original.  If set it checks the current value against the RecordValue, updates the EditContext state based on the equality check and invokes the ChangedFromRecord callback event.  See the code for details.

All useable controls are exact copies of the Blazor originals, but with inheritance set from FormRecordControlBase.  Use the controls in a form in the same manner as the standard controls.  The namespace is CEC.FormControls.Components.FormControls.

The code snippet below shows the date control in use:

        <FormControlDate class="form-control" @bind-Value="this.Record.Date" RecordValue="this.ShadowRecord.Date" ChangedFromRecord="this.RecordFieldChanged"></FormControlDate>

The example project demonstrates the controls in action on a WeatherForecast record.  The project also ties the controls into the CEC.Routing library to demonstrate enhanced router navigation control when the editor page is not saved.

You can read more about CEC.Routing here https://github.com/ShaunCurtis/CEC.Routing/

Updates:
1.0.1 - https://github.com/ShaunCurtis/CEC.FormControls/wiki/1.0.1-Updates
