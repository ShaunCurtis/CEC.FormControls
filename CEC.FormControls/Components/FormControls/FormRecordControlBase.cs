using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CEC.FormControls.Components.FormControls
{
    /// <summary>
    /// A base class derived from the RecordInputBase for form input components.
    /// The class implements extra functionality for comparing the current value
    /// to the stored database value.
    /// 
    /// The class hides the Current Value Property and implements a new version if the 
    /// UseRecordValue property is set. Note We can't test against default because some types have
    /// default values that are meaningful such as bool!
    /// 
    /// Normally RecordValue is assigned to a shadow copy of the database record which is updated to
    /// the current copy when the record is saved.
    /// 
    /// Implementation of how RecordValue is updated is your responsiblity.
    /// </summary>
    public abstract class FormRecordControlBase<TValue> : FormControlBase<TValue>
    {

        /// <summary>
        /// Gets or Sets the value for the field as stored in the Data Object
        /// Should point to a value that gets updated when the record is saved
        /// </summary>
        [Parameter]
        public TValue RecordValue { get; set; }

        /// <summary>
        /// Gets or Sets a callback that signals if the change to the current value matches the Original Value
        /// If _UseOriginalValue is false will be called when the value changes - i.e. reflect ValueChanged
        /// </summary>
        [Parameter]
        public EventCallback<bool> ChangedFromRecord { get; set; }

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


        /// <summary>
        /// Hides the base class CurrentValue.  If UseOriginalValue is not set the property calls the base class setter
        /// If set implements compares against the Original value and modifies the event handling.
        /// </summary>
        protected override TValue CurrentValue
        {
            get => Value;
            set
            {
                if (!this._UseRecordValue)
                {
                    var hasChanged = !EqualityComparer<TValue>.Default.Equals(value, Value);
                    if (hasChanged)
                    {
                        Value = value;
                        _ = ValueChanged.InvokeAsync(value);
                        _ = this.ChangedFromRecord.InvokeAsync(true);
                        EditContext.NotifyFieldChanged(FieldIdentifier);
                    }
                }
                else
                {
                    var hasChanged = !EqualityComparer<TValue>.Default.Equals(value, Value);
                    if (hasChanged)
                    {
                        Value = value;
                        _ = ValueChanged.InvokeAsync(value);
                        var originalHasChanged = !EqualityComparer<TValue>.Default.Equals(this.Value, this.RecordValue);
                        if (originalHasChanged)
                        {
                            this.EditContext.NotifyFieldChanged(FieldIdentifier);
                            _ = this.ChangedFromRecord.InvokeAsync(true);
                        }
                        else
                        {
                            this.EditContext.MarkAsUnmodified(this.FieldIdentifier);
                            _ = this.ChangedFromRecord.InvokeAsync(false);
                        }
                    }
                }
            }
        }
    }
}
