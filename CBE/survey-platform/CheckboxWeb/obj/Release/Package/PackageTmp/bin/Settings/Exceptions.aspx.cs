using System;
using System.Collections.ObjectModel;
using System.Data;
using Checkbox.Common;
using Checkbox.Web.Page;
using Checkbox.Web.UI.Controls.GridTemplates;
using Prezza.Framework.Data;
using System.Web.UI.WebControls;
using Checkbox.Web;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// Show system exceptions
    /// </summary>
    public partial class Exceptions : SettingsPage
    {
        private DataTable _dataSource;

        [QueryParameter("PageIndex")]
        public int? PageIndex { get; set; }

        [QueryParameter("PageSize")]
        public int? PageSize { get; set; }

        public DataTable DataSource
        {
            get { return _dataSource ?? (_dataSource = GetExceptionsListData()); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            Master.OkVisible = false;
            Master.CancelVisible = false;

            if (!IsPostBack)
            {
                //Initialize _exceptionsGrid with defined value
                if (PageSize.HasValue)
                    _exceptionsGrid.PageSize = PageSize.Value;

                if (PageIndex.HasValue)
                    _exceptionsGrid.PageIndex = PageIndex.Value;
            }

            UpdateGridView();

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();
            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");
            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/exceptions.aspx/title");
            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);
            Master.SetTitleControl(titleControl);
        }

        /// <summary>
        /// "Delete selected" handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDeleteSelectedClick(object sender, EventArgs e)
        {
            Database db = DatabaseFactory.CreateDatabase("CheckboxExceptionDb");
            DataTable dt = (DataTable) _exceptionsGrid.DataSource;

            ReadOnlyCollection<int> selected = (_exceptionsGrid.Columns[1] as CustomCheckBoxField).SelectedIndexes;

            foreach (int index in selected)
            {
                int dataIndex = index + _exceptionsGrid.PageIndex * _exceptionsGrid.PageSize;
                int entityId = (int) dt.Rows[dataIndex]["EntryID"];
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ExceptionLog_DeleteEntry");
                command.AddInParameter("EntryID", DbType.Int32, entityId);
                db.ExecuteNonQuery(command);
            }

            if(selected.Count > 0)
            {
                Master.ShowStatusMessage(selected.Count + " " + WebTextManager.GetText("/pageText/settings/exceptions.aspx/logsDeleted"), StatusMessageType.Success);
                UpdateGridView();
            }
            else
            {
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/exceptions.aspx/noLogsDeleted"), StatusMessageType.Error);
            }
        }

        /// <summary>
        /// "Delete all" handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDeleteAllClick(object sender, EventArgs e)
        {
            Database db = DatabaseFactory.CreateDatabase("CheckboxExceptionDb");
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ExceptionLog_DeleteAllEntries");
            int deleted = db.ExecuteNonQuery(command);

            if (deleted > 0)
            {
                Master.ShowStatusMessage(deleted + " " + WebTextManager.GetText("/pageText/settings/exceptions.aspx/logsDeleted"), StatusMessageType.Success);
                UpdateGridView();
            }
            else
            {
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/exceptions.aspx/noLogsDeleted"), StatusMessageType.Error);
            }
        }

        /// <summary>
        /// Grid Page Index Changing handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnExceptionsGridPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            _exceptionsGrid.PageIndex = e.NewPageIndex; 
            _exceptionsGrid.DataBind();
        }
        
        private void UpdateGridView()
        {
            _exceptionsGrid.DataSource = GetExceptionsListData();
            _exceptionsGrid.DataBind();
        }

        /// <summary>
        /// Gets log entries from db
        /// </summary>
        private DataTable GetExceptionsListData()
        {
            Database db = DatabaseFactory.CreateDatabase("CheckboxExceptionDb");
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ExceptionLog_ListEntries");

            DataTable dt = new DataTable();
            dt.Columns.Add("EntryId", typeof(Int32));
            dt.Columns.Add("Checked", typeof(bool));
            dt.Columns.Add("MachineName", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("DateTimeStamp", typeof(DateTime));

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var entryId = DbUtility.GetValueFromDataReader(reader, "EntryId", -1);
                        var machine = DbUtility.GetValueFromDataReader(reader, "MachineName", string.Empty);
                        var message = DbUtility.GetValueFromDataReader(reader, "MessageText", string.Empty);
                        var title = DbUtility.GetValueFromDataReader(reader, "Title", string.Empty);
                        var date = DbUtility.GetValueFromDataReader(reader, "DateTimeStamp", new DateTime());

                        if (entryId > 0 && Utilities.IsNotNullOrEmpty(machine) 
                            && Utilities.IsNotNullOrEmpty(title) && Utilities.IsNotNullOrEmpty(message))
                        {
                            DataRow row = dt.NewRow();
                            row["EntryId"] = entryId;
                            row["Checked"] = false;
                            row["MachineName"] = machine;
                            row["Title"] = title;
                            row["DateTimeStamp"] = date;
                            dt.Rows.Add(row);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return dt;
        }
    }
}
