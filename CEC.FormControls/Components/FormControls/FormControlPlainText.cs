using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CEC.FormControls.Components.FormControls
{
    /// <summary>
    /// A  display only Input box.  
    /// </summary>
    public partial class FormControlPlainText : ComponentBase
    {
        /// <summary>
        /// The string value that will be displayed
        /// </summary>
        [Parameter]
        public string Value { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(2, "class", "form-control-plaintext");
            builder.AddAttribute(2, "readonly", "readonly");
            builder.AddAttribute(4, "value", this.Value);
            builder.CloseElement();
        }
    }
}