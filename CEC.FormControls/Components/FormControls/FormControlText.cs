using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CEC.FormControls.Components.FormControls
{
    /// <summary>
    /// An input component for editing <see cref="string"/> values.
    /// Code base derived from Microsoft.AspNetCore.Components.Forms.InputText
    /// </summary>
    public class FormControlText : FormRecordControlBase<string>
    {

        /// <summary>
        /// Property for defining a Regular Expression to constrain the Entered Text
        /// Set the ValidationErrorComment to suitable user level text to help the user get it right!
        /// Default is empty which will bypass RegEx parsing
        /// </summary>
        [Parameter]
        public string RegEx { get; set; } = string.Empty;

        /// <summary>
        /// Property for defining the message that is tagged to the end of the error message generated when the 
        /// input does not match RegEx.  Make it user level to help them get it right!
        /// </summary>
        [Parameter]
        public string ValidationErrorComment{ get; set; } = string.Empty;

        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValue));
            builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddAttribute(5, "tabindex", TabIndex.ToString());
            builder.CloseElement();
        }
        /// <inheritdoc />

        /// <summary>
        /// Override method.  Applies the regex to the input value of set.
        /// </summary>
        protected override bool TryParseValueFromString(string value, out string result, out string validationErrorMessage)
        {
            validationErrorMessage = null;
            result = value;
            if (string.IsNullOrEmpty(this.RegEx)) return true;
            else
            {
                Regex regx = new Regex(this.RegEx, RegexOptions.IgnoreCase);
                if (regx.IsMatch(value)) return true;
                else
                {
                    validationErrorMessage = $"The {FieldIdentifier.FieldName} field does not match the defined format. {this.ValidationErrorComment}";
                    return false;
                }
            }
        }
    }
}
