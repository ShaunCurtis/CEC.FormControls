
# CEC.FormControls

The CEC.FormControls library provides a set of enhanced input controls based on the Blazor standard form controls.

The standard form controls based on InputBase<$T>$ don’t track changes against an existing record such as a model class.  
Every time you change a value, the controls compare the entered value against the control value, and update the modified 
state on the EditContext.  Great, but what happens when you edit a value twice.  The control only knows what the last 
entered value, it’s lost track of the original value. Change the value back to its original and the EditContext still 
reports the value and form as modified.  If you’re implemented some form of exit control on the form, a real pain to 
demand a save or save without exit when nothing has changed.

This article shows one way of overcoming this issue.

### Library Repository and Example

**CEC.FormControls** is an implementation of the standard Blazor Form Controls with the functionality needed to track 
the current value against the stored value.  

The source code is available at [https://github.com/ShaunCurtis/CEC.FormControls](https://github.com/ShaunCurtis/CEC.FormControls).

The NuGet package name is CEC.FormControls.

All the source code is available under the MIT license.

### InputBase<$T>$

All the input controls inherit from InputBase<$T>$.   Dive into the code, and you wil find the value checking code resides in 
the Property CurrentValue.  It checks the entered value against the stored value notifies the EditContext 
accordingly.  The property looks like this:

```
/// <summary>
/// Gets or sets the current value of the input.
/// </summary>

protected TValue CurrentValue
{
    get => Value!;
    set
    {
        var hasChanged = !EqualityComparer<TValue>.Default.Equals(value, Value);
        if (hasChanged)
        {
            Value = value!;
            _ = ValueChanged.InvokeAsync(Value);
            EditContext.NotifyFieldChanged(FieldIdentifier);
        }
    }
}
```
As you can see it’s protected, so can’t be overridden.  We need a new InputBase.

### Rebuild InputBase<$T>$

To simplify any future upgrades or InputBase<$T>$ changes, the abstract InputBase<$T>$ is replicated with one change. 
 The CurrentValue property is declared virtual.

```
Protected virtual TValue CurrentValue
{
  ……..
}
```

The new control is named **_FormControlBase<$TValue>$._**

##### New abstract class **_FormRecordControlBase<$TValue>$_**

We build the new functionality into a new abstract class FormRecordContralBase.  This will be the new base calls 
for our revamped controls.

The changes:

Two parameters:

1. _RecordValue_ - set to the database value – more about this later.

2. _Locked_ - I use to change the look of certain custom form controls.
````
[Parameter]public TValue RecordValue { get; set; }
[Parameter]public bool Locked { get; set; }
````
Two events:

1. _ChangedFromRecord_ – used for individual assignment of a callback function.

2. _OnRecordChange_ – a cascaded Action from the parent component – more later.
````
[Parameter] public EventCallback<bool> ChangedFromRecord { get; set; }
[CascadingParameter(Name = "OnRecordChange")] protected Action<bool> OnRecordChange { get; set; }
````
A private property - __UseRecordValue_ – that tests if _RecordValue_ checking is required.
````
        private bool _UseRecordValue => RecordValue != null;
````
Override the CurrentValue setter.  The method checks the new value against the RecordValue if one exists 
and sets the Field as unmodified in the EditContext.  Otherwise is replicates the original.  It kicks 
off the two new events passing the EditContext IsModified status.
````
        private bool _UseRecordValue => RecordValue != null;

        protected override TValue CurrentValue
        {
            get => Value;
            set
            {
                var hasChanged = !EqualityComparer<TValue>.Default.Equals(value, Value);
                // only returns true if we have an original value and it matches the current value
                var isSameAsOriginal = this._UseRecordValue && EqualityComparer<TValue>.Default.Equals(value, this.RecordValue);
                if (hasChanged)
                {
                    Value = value;
                    // if the value is the same as the orginal we set the field to unmodified
                    // otherwise we notify the edit context of the change
                    if (isSameAsOriginal) this.EditContext.MarkAsUnmodified(this.FieldIdentifier);
                    else EditContext.NotifyFieldChanged(FieldIdentifier);
                    // kick off the events
                    _ = ValueChanged.InvokeAsync(value);
                    _ = this.ChangedFromRecord.InvokeAsync(this.EditContext.IsModified());
                    this.OnRecordChange?.Invoke(this.EditContext.IsModified());
                }
            }
        }
````
##### Rename existing controls.

The final changes:

1. Change the names and namespace on the copied standard controls – I’ve called them all **_FormControlxxxxxxx_**.

2. Change the inheritance to **_FormRecordControlBase<$TValue>$_**

## Implementing the Controls

There’s an example project with **CEC.FormControls**.  There are few things to note in this project:

1. CEC.Routing is used to control routing when the edit form is dirty i.e. tying into these controls.

2. Record Edit form code is written in a boilerplate class – EditComponentBase.

##### WeatherForecast.cs

The WeatherForecast class implements a ShadowCopy of the original record (part of the IDbRecord interface).  This is a deep copy of the record class, that 
is only updated against the record on a successful save event.  The RecordValue of the control instance is linked to this.
````
    public class WeatherForecast : IDbRecord$<$WeatherForecast$>$
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

        public enum OutlookType { Sunny = 1, Rainy = 2, Cloudy = 3 }

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
````
###### WeatherForecastEditor.razor

The cascaded function wires up the _RecordFieldChanged_ event handler to each FormControl and the RecordValue on each control is bound to the ShadowCopy.
````
<EditForm EditContext="this.EditContext">
    <CascadingValue Value="@this.RecordFieldChanged" Name="OnRecordChange" TValue="Action<bool>">
        <div class="form-group row">
            <label class="col-4 col-form-label">
                Record Date
            </label>
            <div class="col-4">
                <FormControlDate class="form-control" @bind-Value="this.Record.Date" RecordValue="this.ShadowRecord.Date"></FormControlDate>
            </div>
        </div>
        <div class="form-group row">
            <label class="col-4 col-form-label">
                Temperature &deg; C
            </label>
            <div class="col-2">
                <FormControlNumber class="form-control" @bind-Value="this.Record.TemperatureC" RecordValue="this.ShadowRecord.TemperatureC"></FormControlNumber>
            </div>
            ………
    </CascadingValue>
</EditForm>
````
###### WeatherForecastEditor.razor.cs

This is spartan - all the code is implemented in EditComponentBase.cs – see below.  We just set record specific values – in this case the alert message strings.
````
    /// <summary>
    /// Editor for Weather Forecast
    /// All of the code is in the boiler plate EditComponentBase class
    /// </summary>
    public partial class WeatherForecastEditor : EditorComponentBase<WeatherForecast>
    {
        protected override string DirtyMessage => "The Weather has changed!";
        protected override string SavedMessage => "The Weather has been saved";
    }
````
###### EditComponentBase.cs

This is the boilerplate record editor component.

The RecordFieldChanged event handler sets the IsClean property based on the IsModified State of the EditContext (as returned by the event).
````
        protected virtual void RecordFieldChanged(bool changeState)
        {
            if (this.EditContext != null)
            {
                this.ExitAttempt = false;
                this.IsClean = !changeState;
                this.CheckClean();
            }
        }
````
CheckClean sorts out the Alert and turns the PageExit dialog on or off (see the CEC.Routing for detail on this).
````
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
````
Save checks the EditContext validation, saves the record (in this case simply updating the ShadowCopy), sets the EditContext to clean, calls CheckClean to clean up the PageExit and alert, and finally sets the saved custom alert message. 
````
        protected virtual void Save()
        {
            if (this.EditContext.Validate())
            {
                this.ShadowRecord = this.Record.ShadowCopy();
                this.EditContext.MarkAsUnmodified();
                this.CheckClean(true);
                this.Alert.SetAlert(this.SavedMessage, Alert.AlertSuccess);
                InvokeAsync(this.StateHasChanged);
            }
            else this.Alert.SetAlert("A validation error occurred.  Check individual fields for the relevant error.", Alert.AlertDanger);
        }
````
## Setting up CEC.FormControls

Install the **CEC.FormControls** Nuget package.

###### _Imports.razor

Add the following namespace reference.
````
@using CEC.FormControls.Components.FormControls
````

### Other References

You can read more about CEC.Routing here https://github.com/ShaunCurtis/CEC.Routing/

