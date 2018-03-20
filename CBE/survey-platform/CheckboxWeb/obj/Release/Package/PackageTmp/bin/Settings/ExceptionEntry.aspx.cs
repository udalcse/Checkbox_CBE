using System;
using System.Data;
using System.Web.UI;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Data;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// Exception log entry
    /// </summary>
    public partial class ExceptionEntry : SettingsPage
    {
        [QueryParameter("EntryId")]
        public int? EntryId { get; set; }

        /// <summary>
        /// This value is needed to restore a previous exceptionsGrid state
        /// </summary>
        [QueryParameter("PageIndex")]
        public int? PageIndex { get; set; }

        /// <summary>
        /// This value is needed to restore a previous exceptionsGrid state
        /// </summary>
        [QueryParameter("PageSize")]
        public int? PageSize { get; set; }

        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (!Page.IsPostBack)
            {
                Master.SetTitle(WebTextManager.GetText("/pageText/settings/exceptionEntry.aspx/title"));
                DisplayEntry();
            }
            Master.HideDialogButtons();
            _back.Attributes.Add("href", String.Format("Exceptions.aspx?PageIndex={0}&PageSize={1}&uframe=1", PageIndex, PageSize));
        }



        /// <summary>
        /// Attempt to get entry number
        /// </summary>
        private void DisplayEntry()
        {
            try
            {
                //Get entry value
                if (!EntryId.HasValue)
                {
                    Master.ShowStatusMessage(WebTextManager.GetText("/pageText/exceptionLogEntry.aspx/entryNumberError"), StatusMessageType.Error);
                }

                Database db = DatabaseFactory.CreateDatabase("CheckboxExceptionDb");
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ExceptionLog_GetEntry");
                command.AddInParameter("EntryId", DbType.Int32, EntryId.Value);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            entryPanel.Controls.Add(new LiteralControl(Server.HtmlEncode(DbUtility.GetValueFromDataReader(reader, "MessageText", "[NO TEXT]")).Replace(Environment.NewLine, "<br />")));
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Master.ShowStatusMessage("An error occurred displaying the exception log entry: " + ex.Message, StatusMessageType.Error);
            }
        }
    }
}
