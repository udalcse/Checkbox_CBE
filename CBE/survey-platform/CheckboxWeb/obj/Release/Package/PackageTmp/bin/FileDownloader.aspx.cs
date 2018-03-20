using System;
using Checkbox.Forms;

namespace CheckboxWeb
{
    public partial class FileDownloader : Checkbox.Web.Page.BasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Guid fileGuid;
            if (Guid.TryParse(Request.Params["guid"], out fileGuid))
                UploadItemManager.DownloadFile(Response, fileGuid);
        }
    }
}