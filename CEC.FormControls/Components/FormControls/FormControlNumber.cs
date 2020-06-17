using System;
using System.Globalization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;


namespace CEC.FormControls.Components.FormControls
{
    /// <summary>
    /// An input component for editing numeric values.
    /// Supported numeric types are <see cref="int"/>, <see cref="long"/>, <see cref="short"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>.
    /// </summary>
    public class FormControlNumber<TValue> : FormRecordControlBase<TValue>
    {

        /// <summary>
        /// Property for defining the maximum value for the number
        /// </summary>
        [Parameter]
        public TValue MaxValue { get; set; }

        /// <summary>
        /// Property for defining the minimun value for the number
        /// </summary>
        [Parameter]
        public TValue MinValue { get; set; }

        /// <summary>
        /// Property for defining the number of deciaml places for the number
        /// This will only applies to Decimal types
        /// int Minimum value means don't apply - default for int is 0 which is a value number of decimal places!
        /// </summary>
        [Parameter]
        public int DecimalPlaces { get; set; } = int.MinValue;

        /// <summary>
        /// Property for defining if the Min/Max/Decimal Places constraints are applied.  
        /// Checking for default values doesn't work as numeric defaullts are 0 which is a valid entry
        /// </summary>
        [Parameter]
        public bool ApplyConstraints { get; set; }

        private static string _stepAttributeValue; // Null by default, so only allows whole numbers as per HTML spec

        static FormControlNumber()
        {
            // Unwrap Nullable<T>, because InputBase already deals with the Nullable aspect
            // of it for us. We will only get asked to parse the T for nonempty inputs.
            var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
            if (targetType == typeof(int) ||
                targetType == typeof(long) ||
                targetType == typeof(short) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal))
            {
                _stepAttributeValue = "any";
            }
            else  throw new InvalidOperationException($"The type '{targetType}' is not a supported numeric type.");
        }

        /// <summary>
        /// Gets or sets the error message used when displaying an a parsing error.
        /// </summary>
        [Parameter] public string ParsingErrorMessage { get; set; } = "The field must be a number.";

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "step", _stepAttributeValue);
            builder.AddMultipleAttributes(2, AdditionalAttributes);
            builder.AddAttribute(3, "type", "number");
            builder.AddAttribute(4, "class", CssClass);
            builder.AddAttribute(5, "value", BindConverter.FormatValue(CurrentValueAsString));
            builder.AddAttribute(6, "onchange", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddAttribute(7, "tabindex", TabIndex.ToString());
            builder.CloseElement();
        }

        /// <inheritdoc />
        /// <summary>
        /// Override Method. 
        /// Handles constraint checking - min/max and decimal places if enabled
        /// </summary>
        protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
        {
            if (BindConverter.TryConvertTo<TValue>(value, CultureInfo.InvariantCulture, out result))
            {
                if (this.ApplyConstraints)
                {
                    validationErrorMessage = null;
                    if (this.CheckValueAgainstConstraints(result)) return true;
                    else
                    {
                        validationErrorMessage = $"The {FieldIdentifier.FieldName} entered value is not within the constraints {this.MinValue} and {this.MaxValue}";
                        return false;
                    }
                }
                validationErrorMessage = null;
                return true;
            }
            else
            {
                validationErrorMessage = string.Format(ParsingErrorMessage, FieldIdentifier.FieldName);
                return false;
            }
        }

        /// <summary>
        /// Formats the value as a string. Derived classes can override this to determine the formatting used for <c>CurrentValueAsString</c>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string representation of the value.</returns>
        protected override string FormatValueAsString(TValue value)
        {
            // Avoiding a cast to IFormattable to avoid boxing.
            switch (value)
            {
                case null:
                    return null;

                case int @int:
                    return BindConverter.FormatValue(@int, CultureInfo.InvariantCulture);

                case long @long:
                    return BindConverter.FormatValue(@long, CultureInfo.InvariantCulture);

                case short @short:
                    return BindConverter.FormatValue(Convert.ToSingle(@short), CultureInfo.InvariantCulture);

                case float @float:
                    return BindConverter.FormatValue(@float, CultureInfo.InvariantCulture);

                case double @double:
                    return BindConverter.FormatValue(@double, CultureInfo.InvariantCulture);

                case decimal @decimal:
                    if (this.ApplyConstraints && this.DecimalPlaces >= -1)
                    {
                        var val =  decimal.Round(Convert.ToDecimal(@decimal), this.DecimalPlaces, MidpointRounding.AwayFromZero);
                        return BindConverter.FormatValue(val, CultureInfo.InvariantCulture);
                        }
                    else return BindConverter.FormatValue(@decimal, CultureInfo.InvariantCulture);

                default:
                    throw new InvalidOperationException($"Unsupported type {value.GetType()}");
            }
        }

        /// <summary>
        /// Method to check the entered value against the min and max values if checking enabled.
        /// Handles min only and max only situations by setting a null value to the max/min value for the numeric type
        /// </summary>
        protected bool CheckValueAgainstConstraints(TValue value)
        {
            switch (value)
            {
                case int @val:
                    {
                        var trip = false;
                        if (MaxValue != null)
                        {
                            var max = Convert.ToInt32(this.MaxValue);
                            if (Math.Max(val, max) == val) trip = true;
                        }
                        if (MaxValue != null)
                        {
                            var min = Convert.ToInt32(this.MinValue);
                            if (Math.Min(val, min) == val) trip = true;
                        }
                        return trip;
                    }

                case long @val:
                    {
                        var trip = false;
                        if (MaxValue != null)
                        {
                            var max = Convert.ToInt64(this.MaxValue);
                            if (Math.Max(val, max) == val) trip = true;
                        }
                        if (MaxValue != null)
                        {
                            var min = Convert.ToInt64(this.MinValue);
                            if (Math.Min(val, min) == val) trip = true;
                        }
                        return trip;
                    }

                case short @val:
                    {
                        var trip = false;
                        if (MaxValue != null)
                        {
                            var max = Convert.ToInt16(this.MaxValue);
                            if (Math.Max(val, max) == val) trip = true;
                        }
                        if (MaxValue != null)
                        {
                            var min = Convert.ToInt16(this.MinValue);
                            if (Math.Min(val, min) == val) trip = true;
                        }
                        return trip;
                    }

                case float @val:
                    {
                        var trip = false;
                        if (MaxValue != null)
                        {
                            var max = Convert.ToSingle(this.MaxValue);
                            if (Math.Max(val, max) == val) trip = true;
                        }
                        if (MaxValue != null)
                        {
                            var min = Convert.ToSingle(this.MinValue);
                            if (Math.Min(val, min) == val) trip = true;
                        }
                        return trip;
                    }

                case double @val:
                    {
                        var trip = false;
                        if (MaxValue != null)
                        {
                            var max = Convert.ToDouble(this.MaxValue);
                            if (Math.Max(val, max) == val) trip = true;
                        }
                        if (MaxValue != null)
                        {
                            var min = Convert.ToDouble(this.MinValue);
                            if (Math.Min(val, min) == val) trip = true;
                        }
                        return trip;
                    }

                case decimal @val:
                    {
                        var trip = false;
                        if (MaxValue != null)
                        {
                            var max = Convert.ToDecimal(this.MaxValue);
                            if (Math.Max(val, max) == val) trip = true;
                        }
                        if (MaxValue != null)
                        {
                            var min = Convert.ToDecimal(this.MinValue);
                            if (Math.Min(val, min) == val) trip = true;
                        }
                        return trip;
                    }

                default:
                    return false;
            }
        }
    }
}
