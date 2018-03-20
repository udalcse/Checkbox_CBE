using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Delegate for preview controls events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PreviewControlsEvent(object sender, PreviewControlEventArgs e);

    /// <summary>
    /// Event args for preview controls events
    /// </summary>
    public class PreviewControlEventArgs : EventArgs
    {
        /// <summary>
        /// Get/set selected page
        /// </summary>
        public string SelectedPage { get; set; }
    }

    /// <summary>
    /// User control for survey preview navigation
    /// </summary>
    public partial class PreviewControls : Checkbox.Web.Common.UserControlBase
    {
        public event PreviewControlsEvent SelectedPageChanged;

        /// <summary>
        /// Bind page change handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _pageList.AutoPostBack = true;
            _pageList.SelectedIndexChanged += _pageList_SelectedIndexChanged;
            _nextBtn.Click += _nextBtn_Click;
            _prevBtn.Click += _prevBtn_Click;
        }

        /// <summary>
        /// Initialize control with source template.
        /// </summary>
        /// <param name="responseTemplate"></param>
        public void Initialize(ResponseTemplate responseTemplate)
        {
            BindPageList(responseTemplate);
        }

        /// <summary>
        /// Set selected page
        /// </summary>
        /// <param name="selectedPage"></param>
        public void SetSelectedPage(string selectedPage)
        {
            if (_pageList.Items.FindByValue(selectedPage) != null)
            {
                _pageList.SelectedValue = selectedPage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSelectedPage()
        {
            return _pageList.SelectedValue;
        }

        /// <summary>
        /// Bind list of pages
        /// </summary>
        /// <param name="responseTemplate"></param>
        private void BindPageList(ResponseTemplate responseTemplate)
        {
            //All pages
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/allPages"),
                "ALL_PAGES"));
            /*
            //Add specialized pages depending on survey settings
            if (responseTemplate.BehaviorSettings.SecurityType == SecurityType.AccessControlList
                || responseTemplate.BehaviorSettings.SecurityType == SecurityType.AllRegisteredUsers)
            {
                _pageList.Items.Add(new ListItem(
                    WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/loginPage"),
                    "LOGIN"));
            
            }

            //Password
            if (responseTemplate.BehaviorSettings.SecurityType == SecurityType.PasswordProtected)
            {
                _pageList.Items.Add(new ListItem(
                   WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/passwordPage"),
                   "PASSWORD"));
            }

            //Save and quit
            if (responseTemplate.BehaviorSettings.AllowContinue
                && responseTemplate.BehaviorSettings.ShowSaveAndQuit)
            {
                _pageList.Items.Add(new ListItem(
                    WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/saveAndExitPage"),
                    "SAVE_AND_EXIT"));
            }

            //Edit response
            if (responseTemplate.BehaviorSettings.AllowEdit)
            {
                _pageList.Items.Add(new ListItem(
                    WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/editResponse"),
                    "EDIT_RESPONSE"));
            }

            //Language select
            if (responseTemplate.LanguageSettings.SupportedLanguages.Count > 1)
            {
                _pageList.Items.Add(new ListItem(
                    WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/selectLanguage"),
                    "LANGUAGE_SELECT"));
            }
            */

            //Individual pages
            int[] pageIds = responseTemplate.ListTemplatePageIds();

            //Prior to 5.0, pages could have negative positions.  Adjust positions so first content page is page one.
            int contentPagePosition = 1;

            for (int i = 0; i < pageIds.Length; i++)
            {
                TemplatePage tp = responseTemplate.GetPage(pageIds[i]);

                //Add all non-hidden and non-completion pages
                if (tp != null
                    && tp.PageType == TemplatePageType.ContentPage)
                {
                    _pageList.Items.Add(new ListItem(
                        WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/page") + " " + contentPagePosition,
                        tp.ID.ToString()));

                    contentPagePosition++;
                }
            }
            
            //Completion
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/completionPage"),
               "COMPLETION"));
        }

        /// <summary>
        /// Handle change in page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedPageChanged != null)
            {
                SelectedPageChanged(
                    this,
                    new PreviewControlEventArgs { SelectedPage = _pageList.SelectedValue });
            }
        }

        /// <summary>
        /// Prev page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _prevBtn_Click(object sender, ImageClickEventArgs e)
        {
            if (_pageList.SelectedIndex > 0)
            {
                _pageList.SelectedIndex = _pageList.SelectedIndex - 1;
                SelectedPageChanged(
                    this,
                    new PreviewControlEventArgs { SelectedPage = _pageList.SelectedValue });
            }
        }

        /// <summary>
        /// Next page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _nextBtn_Click(object sender, ImageClickEventArgs e)
        {
            if (_pageList.SelectedIndex < (_pageList.SelectedIndex - 1))
            {
                _pageList.SelectedIndex = _pageList.SelectedIndex + 1;
                SelectedPageChanged(
                    this,
                    new PreviewControlEventArgs { SelectedPage = _pageList.SelectedValue });
            }
        }
    }
}