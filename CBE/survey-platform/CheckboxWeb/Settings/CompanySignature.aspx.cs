using System;
using System.IO;
using Checkbox.Content;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class CompanySignature : SettingsPage
    {
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            Master.OkClick += Save;

            _companySignatureImageSelector.Initialize(ApplicationManager.AppSettings.CompanySignatureImageUrl, Page.IsPostBack);
            _companySignatureEnabled.Checked = ApplicationManager.AppSettings.CompanySignatureEnabled;

        }

        private void Save(object sender, EventArgs e)
        {
          
                int? imageId = _companySignatureImageSelector.GetImageId();

                if (!imageId.HasValue && !string.IsNullOrEmpty(_companySignatureImageSelector.ImageUrl))
                {
                    UploadedFileType fileType;

                    var image = new DBContentItem(null)
                    {
                        ItemUrl = _companySignatureImageSelector.ImageUrl,
                        ItemName = Path.GetFileName(_companySignatureImageSelector.ImageUrl),
                        CreatedBy = UserManager.GetCurrentPrincipal().Identity.Name
                    };

                    image.ContentType = UploadItemManager.DetermineContentType(image.ItemName, out fileType);
                    image.LastUpdated = DateTime.Now;
                    image.Save();
                }

            ApplicationManager.AppSettings.CompanySignatureEnabled = _companySignatureEnabled.Checked;
            ApplicationManager.AppSettings.CompanySignatureImageUrl = _companySignatureImageSelector.ImageUrl;

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/branding.aspx/updateSuccessful"), StatusMessageType.Success);
        }

    }
}