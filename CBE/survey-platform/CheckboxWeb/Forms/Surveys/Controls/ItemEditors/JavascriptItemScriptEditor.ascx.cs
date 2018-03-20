using Checkbox.Web.Common;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class JavascriptItemScriptEditor : UserControlBase
    {
        public string Script
        {
            set { _script.Value = value; }
            get { return _script.Value; }
        }
    }
}