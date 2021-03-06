﻿using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CEC.FormControls.Components.FormControls
{
    /// <summary>
    /// A multiline input component for editing <see cref="string"/> values.
    /// This is a direct copy of Microsoft.AspNetCore.Components.Forms.InputTextArea
    /// But with different inheritance
    /// </summary>
    public partial  class FormControlTextArea : FormRecordControlBase<string>
    {
        /// <inheritdoc />
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "textarea");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder<string>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.AddAttribute(5, "tabindex", TabIndex.ToString());
            builder.AddMarkupContent(6, BindConverter.FormatValue(CurrentValue));
            builder.CloseElement();
        }
        /// <inheritdoc />
        protected override bool TryParseValueFromString(string value, out string result, out string validationErrorMessage)
        {
            result = value;
            validationErrorMessage = null;
            return true;
        }
    }
}