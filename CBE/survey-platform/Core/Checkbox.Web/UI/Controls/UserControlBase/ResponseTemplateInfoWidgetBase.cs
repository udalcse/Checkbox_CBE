using System;
using System.Web.UI;
using Checkbox.Forms;
using Telerik.Web.UI;

namespace Checkbox.Web.UI.Controls.UserControlBase
{
    /// <summary>
    /// Base class for survey information widgets designed to be used on the survey homepage.  Supports ajax
    /// requests to populate data in addition to normal page flow. To make ajax request, post back with
    /// event argument of PopulateWidget, e.g.
    /// window["<%= RadAjaxManager.GetCurrent(Page).ClientID  %>"].AjaxRequest("PopulateWidget");
    /// </summary>
    public abstract class ResponseTemplateInfoWidgetBase : UserControl
    {
        /// <summary>
        /// Get/set id of survey used by widget
        /// </summary>
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// Get/set folder id
        /// </summary>
        public int? FolderId { get; set; }

        /// <summary>
        /// Get/set response template to use for widget
        /// </summary>
        public ResponseTemplate ResponseTemplate { get; set; }

        /// <summary>
        /// Get whether to populate on ajax request or not.
        /// </summary>
        protected virtual bool EnableAjax { get { return false; } }

        /// <summary>
        /// Get reference to label that contains title. Used by base class automatically set title textid
        /// when titletextid property is set.
        /// </summary>
        protected virtual MultiLanguageLabel TitleLabel { get { return null; } }

        /// <summary>
        /// Use existing response template reference or template id to get a 
        /// reference to the response template to use.
        /// </summary>
        /// <returns></returns>
        protected ResponseTemplate GetResponseTemplate()
        {
            if (ResponseTemplate == null
                && ResponseTemplateId > 0)
            {
                ResponseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);
            }

            if (ResponseTemplate == null)
            {
                throw new Exception("Unable to load info widget because no template was specified or template with given id [" + ResponseTemplateId + "] could not be loaded.");
            }

            return ResponseTemplate;
        }

        /// <summary>
        /// Populate widget controls
        /// </summary>
        protected abstract void PopulateWidgetControls(ResponseTemplate template);

        /// <summary>
        /// Add ajax setting for handling any loading panels
        /// </summary>
        /// <param name="ajaxManager"></param>
        protected virtual void AddAjaxSetting(RadAjaxManager ajaxManager) { }

        /// <summary>
        /// Override on load to handle populating widget controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Try to bind ajax request
            if (EnableAjax && Visible)
            {
                if (Page != null)
                {
                    RadAjaxManager ajaxMgr = RadAjaxManager.GetCurrent(Page);

                    if (ajaxMgr != null)
                    {
                        //Add include script
                        Page.ClientScript.RegisterClientScriptInclude("ajaxWidget", ResolveUrl("~/Resources/ajaxWidget.js"));

                        //Set load script
                        Page.ClientScript.RegisterStartupScript(
                            GetType(),
                            "bindLoad",
                            "bindLoad('" + RadAjaxManager.GetCurrent(Page).ClientID + "', 'PopulateWidget');",
                            true);

                        //Bind ajax request to populate dialog
                        ajaxMgr.AjaxRequest += ajaxMgr_AjaxRequest;

                        //Add ajax setting to trigger loading panel
                        AddAjaxSetting(ajaxMgr);

                        //Return so control can flow to normal processing in event ajax isn't going to happen
                        return;
                    }
                }
            }

            //No ajax, so populate widgets normally
            PopulateWidgetControls(GetResponseTemplate());
        }


        /// <summary>
        /// Event handler for populating widgets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ajaxMgr_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument.Equals("PopulateWidget", StringComparison.InvariantCultureIgnoreCase))
            {
                PopulateWidgetControls(GetResponseTemplate());
            }
        }
    }
}
