using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Create new folder
    /// </summary>
    public partial class Create : ApplicationPage
    {
        [QueryParameter("f")]
        public int? FolderId { get; set; }
        
        /// <summary>
        /// Allow editors to assign survey style templates
        /// </summary>
        protected bool CanAssignStyles
        {
            get
            {
                if (!ApplicationManager.AppSettings.AllowEditSurveyStyleTemplate)
                {
                    return User.IsInRole("System Administrator");
                }
                return true;
            }
        }
        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += OkBtn_Click;
            
            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/forms/surveys/create.aspx/createSurvey");

            //Populate folder list
            PopulateFolderList();
            
            //Populate styles
            PopulateStyleList(_styleListPC, StyleTemplateType.PC, ApplicationManager.AppSettings.DefaultStyleTemplate);

            _styleListMobile.DataSource = MobileStyleManager.GetAllStyles(TextManager.DefaultLanguage).OrderByDescending(s => s.IsDefault);
            _styleListMobile.DataBind();

        }

        /// <summary>
        /// Ensure necessary scripts loaded
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Survey management JS service
            RegisterClientScriptInclude(
                "svcSurveyManagement.js",
                ResolveUrl("~/Services/js/svcSurveyManagement.js"));

            //Helper for editing survey/report/library templates
            RegisterClientScriptInclude(
                "templateEditor.js",
                ResolveUrl("~/Resources/templateEditor.js"));

            //Service helper required by JS service
            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));
        }

        /// <summary>
        /// Fills one list with styles of the defined type
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="type"></param>
        /// <param name="defaultStyleId"></param>
        private void PopulateStyleList(DropDownList ddl, StyleTemplateType type, int defaultStyleId)
        {
            ddl.DataSource = StyleTemplateManager.ListStyleTemplatesForDataBinding(UserManager.GetCurrentPrincipal(), type);
            ddl.DataBind();
            ddl.Items.Insert(
                0,
                new ListItem(WebTextManager.GetText("/pageText/forms/surveys/import.aspx/none"), "0"));
            if (defaultStyleId > 0
                && ddl.Items.FindByValue(defaultStyleId.ToString()) != null)
            {
                ddl.SelectedValue = defaultStyleId.ToString();
            }
        }

        /// <summary>
        /// Populate folder list
        /// </summary>
        private void PopulateFolderList()
        {
            List<LightweightAccessControllable> folderList = FolderManager.ListAccessibleFolders(
                HttpContext.Current.User as CheckboxPrincipal,
                "FormFolder.Read");

            foreach (LightweightAccessControllable folder in folderList)
            {
                _folderList.Items.Add(new ListItem(
                    Utilities.StripHtml(folder.Name, 64),
                    folder.ID.ToString()));
            }

            //Add "Root" and set selected value
            _folderList.Items.Insert(
                0,
                new ListItem(WebTextManager.GetText("/pageText/forms/surveys/create.aspx/rootFolder"), "0"));

            //Set current folder as selected
            if(FolderId.HasValue
                && _folderList.Items.FindByValue(FolderId.ToString()) != null)
            {
                _folderList.SelectedValue = FolderId.ToString();
            }
        }


        /// <summary>
        /// Handle _okBtnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //Set the short URL Mapping
                if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting && !_properties.CustomUrlIsValid())
                {
                    //CustomUrlIsValid return false. It seems that error is shown at the page.
                    //That's why we shouldn't close the dialog.
                    return;
                }

                var rt = ResponseTemplateManager.CreateResponseTemplate(UserManager.GetCurrentPrincipal());
                ResponseTemplateManager.InitializeTemplate(rt, _properties.SurveyName);
                rt.BehaviorSettings.EnableScoring = _properties.EnableScoring;
                rt.BehaviorSettings.IsActive = _properties.ActivateSurvey;

                var selectedStyleTemplateId = int.Parse(_styleListPC.SelectedValue);
                rt.StyleSettings.StyleTemplateId = selectedStyleTemplateId > 0 ? selectedStyleTemplateId : (int?)null;

                var selectedMobileTemplateId = int.Parse(_styleListMobile.SelectedValue);
                if (selectedMobileTemplateId > 0)
                {
                    rt.StyleSettings.MobileStyleId = selectedMobileTemplateId;
                }

                rt.Save();

                if (rt.ID.HasValue)
                {
                    //Set the short URL Mapping
                    if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
                    {                       
                        var key = string.Format("{0}/Survey.aspx?s={1}",
                                                    ApplicationManager.ApplicationRoot,
                                                    rt.GUID.ToString().Replace("-", String.Empty));

                        _properties.SaveCustomUrl(key);                     
                    }

                    CloseAndRedirect(rt.ID.Value);
                }
            }
        }

        private void CloseAndRedirect(int id)
        {
            int selectedFolderId = int.Parse(_folderList.SelectedValue);

            if (selectedFolderId == 0)
            {
                var folder = FolderManager.GetRoot();
                folder.Add(id);
            }
            else
            {
                var folder = FolderManager.GetFolder(selectedFolderId);

                if (folder == null)
                {
                    throw new Exception("Unable to load folder with id: " + selectedFolderId);
                }

                folder.Add(id);
            }

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Redirect",
                string.Format("closeWindowAndRedirectParentPage('', null, 'Edit.aspx?f={0}&s={1}');", FolderId, id),
                true);
        }
    }
}
