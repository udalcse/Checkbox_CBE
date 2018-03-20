using System.Text.RegularExpressions;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public partial class Javascript : UserControlSurveyItemRendererBase
    {
        protected string StriptScriptTags(string script)
        {
            var regex = new Regex(@"<script[^>]*>|</script>");
            script = regex.Replace(script, string.Empty);

            return script;
        }
    }
}