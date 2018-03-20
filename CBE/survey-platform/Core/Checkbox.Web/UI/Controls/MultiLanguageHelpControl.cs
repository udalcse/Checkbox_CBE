/****************************************************************************
 * Control based on the HelpControl that supports multilanguage             *
 * functionality	                                                        *
 ****************************************************************************/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using Checkbox.Globalization.Text;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Summary description for HelpControl.
    /// </summary>
    public class MultiLanguageHelpControl : MultiLanguageImage
    {
        /*
        private string text;
        private string helpfile;
        private string url;
        private string file;

        /// <summary>
        /// Get/Set the text associated with this help control.  Unless otherwise configured, non-localized text will be flagged.
        /// </summary>
        public string Text
        {
            get
            {
                if (this.TextId == null || this.TextId == string.Empty)
                {
                    if (this.FlagUnlocalizedText)
                    {
                        return "{NL}" + text;
                    }
                    else
                    {
                        return text;
                    }
                }
                else
                {
                    string returnText;
                    if (languageCode == null || languageCode == string.Empty)
                    {
                        returnText = WebTextManager.GetText(TextId);
                    }
                    else
                    {
                        returnText = WebTextManager.GetText(TextId, languageCode);
                    }

                    if ((returnText == null || returnText == string.Empty) && this.FlagUnlocalizedText)
                    {
                        return "{NL}" + text;
                    }
                    else
                    {
                        return returnText;
                    }
                }
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// Get/set the helpfile associated with this help control
        /// </summary>
        public string HelpFile
        {
            get
            {
                return this.file;
            }
            set
            {
                this.file = value;
            }
        }

        /// <summary> 
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="output"> The HTML writer to write out to </param>
        protected override void Render(HtmlTextWriter output)
        {
            string script;

            this.Attributes["style"] = "cursor:hand";

            this.ImageUrl = Checkbox.Management.ApplicationManager.ApplicationRoot + "/Images/help.gif";
            this.Height = 16;
            this.Width = 16;
            this.ToolTip = "Help";
            this.ToolTipTextId = "/controlText/helpControl/help";

            this.url = Checkbox.Management.ApplicationManager.ApplicationRoot + "/administration/help/index.htm";

            if (this.file != null)
            {
                this.url = this.url + "#" + this.file;
            }

            script = "window.open('" + this.url + "','popped',  'toolbar=0, scrollbars=1, location=0, statusbar=0, menubar=0,resizable=1, width=650, height=500,left=100, top=100');return false;";

            this.Attributes.Add("onClick", script);
            this.Attributes.Add("title", WebTextManager.GetText("/common/newWindow"));
            base.Render(output);
        }

        /// <summary>
        /// 
        /// </summary>
        public string HelpControlField
        {
            set
            {
                helpfile = value;
            }
        }*/
    }
}
