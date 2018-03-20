using System;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Content;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.IO;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// Configure application branding
    /// </summary>
    public partial class Branding : SettingsPage
    {
        /// <summary>
        /// Page init
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            _headerType.Items.Clear();
            _headerType.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/branding.aspx/headerTypeImage"), "Logo"));
            _headerType.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/branding.aspx/headerTypeText"), "Text"));

            _headerType.SelectedValue = ApplicationManager.AppSettings.HeaderTypeChosen.ToString();

            _fontName.DataBind();
            _fontStyle.DataBind();
            _fontSize.DataBind();


            int intHeaderFontSize;
            string headerFontSize = ApplicationManager.AppSettings.HeaderFontSize;
            if (int.TryParse(headerFontSize, out intHeaderFontSize))
            {
                if (_fontSize.Items.FindByValue(string.Format("{0}px", intHeaderFontSize)) != null)
                {
                    _fontSize.SelectedValue = string.Format("{0}px", intHeaderFontSize);
                }
            }
            else
            {
                if (_fontSize.Items.FindByValue(headerFontSize) != null)
                {
                    _fontSize.SelectedValue = headerFontSize;
                }
            }

            if (Utilities.IsNotNullOrEmpty(ApplicationManager.AppSettings.HeaderFont)
                && _fontName.Items.FindByValue(ApplicationManager.AppSettings.HeaderFont) != null)
            {
                _fontName.SelectedValue = ApplicationManager.AppSettings.HeaderFont;
            }

            _selectedColor.Text = ApplicationManager.AppSettings.HeaderTextColor;
            _headerText.Text = WebTextManager.GetText("/siteText/headerText");

            _logoImageSelector.Initialize(ApplicationManager.AppSettings.HeaderLogo, Page.IsPostBack);

            Master.OkClick += Save;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            SetControlState();

            RegisterClientScriptInclude(
                "colorPicker.js",
                ResolveUrl("~/Resources/mColorPicker.min.js"));
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetControlState()
        {
            if ("Logo".Equals(_headerType.SelectedValue, StringComparison.InvariantCultureIgnoreCase))
            {
                _logoOptionsPanel.Visible = true;
                _textPanel.Visible = false;
            }
            else
            {
                _logoOptionsPanel.Visible = false;
                _textPanel.Visible = true;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.HeaderTypeChosen = (AppSettings.HeaderType)Enum.Parse(typeof(AppSettings.HeaderType), _headerType.SelectedValue);

            if (ApplicationManager.AppSettings.HeaderTypeChosen == AppSettings.HeaderType.Logo)
            {
                int? imageId = _logoImageSelector.GetImageId();

                if (!imageId.HasValue && !String.IsNullOrEmpty(_logoImageSelector.ImageUrl))
                {
                    UploadedFileType fileType;

                    var image = new DBContentItem(null);
                    image.ItemUrl = _logoImageSelector.ImageUrl;
                    image.ItemName = Path.GetFileName(_logoImageSelector.ImageUrl);
                    image.CreatedBy = UserManager.GetCurrentPrincipal().Identity.Name;
                    image.ContentType = UploadItemManager.DetermineContentType(image.ItemName, out fileType);
                    image.LastUpdated = DateTime.Now;
                    image.Save();
                }

                ApplicationManager.AppSettings.HeaderLogo = _logoImageSelector.ImageUrl;
            }
            else
            {
                ApplicationManager.AppSettings.HeaderFont = _fontName.SelectedValue;
                int intHeaderFontSize;
                if (int.TryParse(_fontSize.SelectedValue.Replace("px", string.Empty), out intHeaderFontSize))
                {
                    ApplicationManager.AppSettings.HeaderFontSize = intHeaderFontSize.ToString();
                }
                else
                {
                    ApplicationManager.AppSettings.HeaderFontSize = _fontSize.SelectedValue;
                }

                ApplicationManager.AppSettings.HeaderTextColor = _selectedColor.Text;
                TextManager.SetText("/siteText/headerText", WebTextManager.GetUserLanguage(), _headerText.Text.Trim());
            }

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/branding.aspx/updateSuccessful"), StatusMessageType.Success);
        }

    }
}
