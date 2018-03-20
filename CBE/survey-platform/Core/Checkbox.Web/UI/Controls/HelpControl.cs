using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Summary description for HelpControl.
    /// </summary>
    [DefaultProperty("Text"),
     ToolboxData("<{0}:HelpControl runat=server></{0}:HelpControl>")]
    public class HelpControl : Image
    {
        private string _url;

        ///<summary>
        ///</summary>
        [Bindable(true),
         Category("Appearance"),
         DefaultValue("")]
        public string Text { get; set; }

        ///<summary>
        ///</summary>
        public string HelpFile { get; set; }

        /// <summary> 
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="output"> The HTML writer to write out to </param>
        protected override void Render(HtmlTextWriter output)
        {
            Attributes["style"] = "cursor:hand";

            ImageUrl = ApplicationManager.ApplicationRoot + "/Images/help.gif";
            Height = 16;
            Width = 16;
            AlternateText = "Help";

            //URL = Checkbox.Web.Configuration.ApplicationURL + "/help/index.htm#<id=" + helpfile + ">";
            //URL = Checkbox.Web.Configuration.ApplicationURL + "/help/index.htm#<id=1>";
            //URL = Checkbox.Web.Configuration.ApplicationURL + "/help/index.htm";
            _url = ApplicationManager.ApplicationRoot + "/administration/help/index.htm";

            if (HelpFile != null)
                _url = _url + "#" + HelpFile;

            string script = "window.open('" + _url + "','popped',  'toolbar=0, scrollbars=1, location=0, statusbar=0, menubar=0,resizable=1, width=650, height=500,left=100, top=100');return false;";

            Attributes.Add("onClick", script);
            Attributes.Add("title", WebTextManager.GetText("/common/newWindow"));
            base.Render(output);
        }

        ///<summary>
        ///</summary>
        public string HelpControlField { get; set; }
    }
}