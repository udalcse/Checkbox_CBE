using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Web.Forms.UI.Rendering;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SingleLineControlBase : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected string AutocompleteData { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        protected string AutocompleteRemote { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected void InitAutocomplete(WebControl input)
        {

            var autocompleteListId = Utilities.AsInt(Model.Metadata["AutocompleteListId"]);
            if (autocompleteListId.HasValue)
            {
                var data = AutocompleteListManager.ListItems(autocompleteListId.Value);

                var ser = new JavaScriptSerializer();
                AutocompleteData = ser.Serialize(data);

                input.Attributes["autocomplete"] = "off";
            }
            else if (ApplicationManager.AppSettings.AllowAutocompleteRemoteSource)
            {
                AutocompleteRemote = Model.Metadata["AutocompleteRemote"];
                input.Attributes["autocomplete"] = "off";
            }
        }
    }
}
