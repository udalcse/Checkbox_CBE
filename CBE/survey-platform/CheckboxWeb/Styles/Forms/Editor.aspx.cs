using System;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Common;
using Checkbox.Styles;
using CheckboxWeb.Styles.Controls;
using Checkbox.Security.Principal;
using Checkbox.Globalization.Text;
using Telerik.Web.UI;
using Checkbox.Management;

namespace CheckboxWeb.Styles.Forms
{
    public partial class Editor : SecuredPage, IStatusPage
    {
        protected override string  PageRequiredRolePermission { get { return "Form.Edit"; } }

        protected StyleTemplate Template
        {
            get { return (StyleTemplate)HttpContext.Current.Session["CurrentStyleTemplate"]; }
            set { HttpContext.Current.Session["CurrentStyleTemplate"] = value; }
        }

        protected override void OnPageInit()
        {
            string idText = Request.QueryString["id"];

			if (!string.IsNullOrEmpty(idText) && !Page.IsPostBack)
            {
				int id = 0;

				if (int.TryParse(idText, out id))
					Template = StyleTemplateManager.GetStyleTemplate(id);
				else EnsureTemplate();
            }
			else EnsureTemplate();

            if (!Page.IsPostBack)
            {
                AddItem(WebTextManager.GetText("/pageText/styles/forms/editor.aspx/properties"), "Properties");
                AddPageView(_stylePanelBar.FindItemByValue("Properties"));
                AddItem(WebTextManager.GetText("/pageText/styles/forms/editor.aspx/fontsColors"), "FontsColors");
                AddItem(WebTextManager.GetText("/pageText/styles/forms/editor.aspx/headerFooter"), "HeaderFooter");
                AddItem(WebTextManager.GetText("/pageText/styles/forms/editor.aspx/formControls"), "FormControls");
            }

            _stylePanelBar.ItemClick += new RadPanelBarEventHandler(_panelBar_ItemClick);

            base.OnPageInit();
        }

		private void EnsureTemplate()
		{
			if (Template == null)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(Server.MapPath("~/Resources/DefaultStyle.xml"));
				Template = StyleTemplateManager.CreateStyleTemplate(doc, HttpContext.Current.User as CheckboxPrincipal);
			}
		}

        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Create controls for title lbl
            PlaceHolder titlePlaceholder = new PlaceHolder();
            Label label = new Label { Text = WebTextManager.GetText("/pageText/styles/forms/editor.aspx/editStyle") + " - " };
            HyperLink link = new HyperLink
            {
                NavigateUrl = ApplicationManager.ApplicationRoot + "/Styles/Manage.aspx",
                Text = Utilities.StripHtml(Template.Name, 255)
            };

            titlePlaceholder.Controls.Add(label);
            titlePlaceholder.Controls.Add(link);

            ((BaseMasterPage)Master).SetTitleControl(titlePlaceholder);

            WireStatusControl(_styleEditor);

            Page.ClientScript.RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
        }

        protected override void OnPagePreRender()
        {
            UpdatePreview();
            base.OnPagePreRender();
        }

        #region PageView and PanelBar methods/events

        private void AddPageView(RadPanelItem radPanelItem)
        {
            RadPageView pageView = new RadPageView();
            pageView.ID = radPanelItem.Value;

            _styleEditor.PageViews.Clear();
            if(_styleEditor.FindPageViewByID(pageView.ID) == null)
                _styleEditor.PageViews.Add(pageView);

            pageView.CssClass = "pageView";
            pageView.Selected = true;
        }

        private void AddItem(string name, string value)
        {
            RadPanelItem item = new RadPanelItem();
            item.Text = name;
            item.Value = value;
            _stylePanelBar.Items.Add(item);
        }

        protected void _multiPage_PageViewCreated(object sender, RadMultiPageEventArgs e)
        {
            string userControlName = e.PageView.ID + ".ascx";

            Control userControl = Page.LoadControl("EditorControls/" + userControlName);
            userControl.ID = e.PageView.ID + "_userControl";

            e.PageView.Controls.Add(userControl);
        }

        private void _panelBar_ItemClick(object sender, RadPanelBarEventArgs e)
        {
            AddPageView(e.Item);
        }

        #endregion

        private void UpdatePreview()
        {
            // TODO: Multi-language enable
            //_stylePreview.SelectedLanguage = TextManager.DefaultLanguage;

            _stylePreview.HeaderHtml = TextManager.GetText(Template.HeaderTextID, TextManager.DefaultLanguage);
            _stylePreview.FooterHtml = TextManager.GetText(Template.FooterTextID, TextManager.DefaultLanguage);
            _stylePreview.StyleCss = Template.GetCss();
        }

        protected void _save_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                StyleTemplateManager.SaveTemplate(Template, HttpContext.Current.User as CheckboxPrincipal);

                HttpContext.Current.Session["CurrentStyleTemplate"] = null;
                Response.Redirect("~/Styles/Manage.aspx");
            }
            else
            {
                ShowStatusMessage(WebTextManager.GetText("/pageText/styles/editor.aspx/editorError"), StatusMessageType.Error);
            }
        }

        protected void _cancel_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentStyleTemplate"] = null;

            Response.Redirect("~/Styles/Manage.aspx");
        }

        #region IStatusPage Members

        public void WireStatusControl(Control sourceControl)
        {
        }

        public void WireUndoControl(Control sourceControl)
        {
            throw new NotImplementedException();
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType)
        {
            ShowStatusMessage(message, messageType, string.Empty, string.Empty);
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType, string actionText, string actionArgument)
        {
            _editorStatus.Message = message;
            _editorStatus.MessageType = messageType;
            _editorStatus.ActionText = actionText;
            _editorStatus.ActionArgument = actionArgument;
            _editorStatus.ShowStatus();
        }

        #endregion
    }
}