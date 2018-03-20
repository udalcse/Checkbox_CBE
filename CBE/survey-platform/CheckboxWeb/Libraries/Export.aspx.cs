using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Users;
using Telerik.Web.UI;

using Checkbox.Web.Page;
using Checkbox.Forms;
using Checkbox.Pagination;
using Checkbox.Web;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Libraries
{
    public partial class Export : SecuredPage, IStatusPage
    {
        private class LibraryGridObject
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int ItemsCount { get; set; }
        }

        protected override string PageRequiredRolePermission { get { return "Library.View"; } }

        private string SortExpression { get; set; }
        private string SortDirection { get; set; }

		[QueryParameter("id")]
		public string IdList { get; set; }

        protected override void OnPageInit()
        {
            base.OnPageInit();
            //_newLibraryWindow.NavigateUrl = "Create.aspx";
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            WireStatusControl(_librariesGrid);
            _statusControl.HideStatus();
        }

        protected void _librariesGrid_NeedDataSource(object sender, EventArgs e)
        {
            PaginationContext paginationContext = new PaginationContext
            {
                //FilterField = _searchControl.SearchField,
                //FilterValue = _searchControl.SearchValue,
                CurrentPage = _librariesGrid.CurrentPageIndex + 1,
                PageSize = _librariesGrid.PageSize,
                Permissions = new List<string> {"Library.View"},
                SortField = SortExpression,
                SortAscending = true,
                PermissionJoin = Checkbox.Security.PermissionJoin.All
            };

			string[] idArr = IdList.Split(',');

			List<LibraryGridObject> gridObjects = new List<LibraryGridObject>();

			foreach (string idTxt in idArr)
			{
				if (string.IsNullOrEmpty(idTxt))
					continue;

				int id = int.Parse(idTxt);

				LibraryTemplate lib = LibraryTemplateManager.GetLibraryTemplate(id);

				gridObjects.Add(
				new LibraryGridObject
				{
					ID = lib.ID.Value,
					Name = TextManager.GetText(lib.NameTextID, TextManager.DefaultLanguage),
					Description = "Download",
					ItemsCount = LibraryTemplateManager.GetTemplateItemsCount(lib.ID.Value)
				}
			);
			}

            //List<LightweightLibraryTemplate> libraries = LibraryTemplateManager.GetAvailableLibraryTemplates(HttpContext.Current.User as CheckboxPrincipal, paginationContext);


            _librariesGrid.DataSource = gridObjects;

            //Set parameters
            _librariesGrid.VirtualItemCount = paginationContext.ItemCount;
        }

        protected void _librariesGrid_SortCommand(object sender, EventArgs e)
        {
        }

        protected void _librariesGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "DeleteSelected":
                    DeleteSelected();
                    break;
                case "CopySelected":
                    CopySelected();
                    break;
                case "ExportSelected":
                    ExportSelected();
                    break;
            }
        }

        private void ExportSelected()
        {
			// TODO; implement export dialog
			/*
            if (_librariesGrid.SelectedItems.Count > 0)
            {
                StringBuilder ids = new StringBuilder();
                foreach (GridItem item in _librariesGrid.SelectedItems)
                {
                    ids.Append(((GridDataItem)item)["ID"].Text + ',');
                }

                RadAjaxManager mgr = RadAjaxManager.GetCurrent(Page);
                mgr.ResponseScripts.Add(String.Format("window.location.href = \"{0}\";", "Export.aspx?id=" + ids.ToString()));
            }

			 * TODO uncomment when export will be ready
			 */

			// TODO remove it when export will be ready
			if (_librariesGrid.SelectedItems.Count > 0)
				ShowStatusMessage("Export function is not implemented yet.", StatusMessageType.Error);

			//ShowStatusMessage(string.Format("{0} template(s) exported.", _librariesGrid.SelectedItems.Count.ToString()), StatusMessageType.Success);
		}

        private void CopySelected()
        {
			// TODO; implement library copy logic

			//if (_librariesGrid.SelectedItems.Count > 0)
			//{
			//    foreach (GridDataItem item in _librariesGrid.SelectedItems)
			//    {
			//        int id = Int32.Parse(item["ID"].Text);
			//        LibraryTemplateManager.CopyLibraryTemplate(id, HttpContext.Current.User as CheckboxPrincipal, WebTextManager.GetUserLanguage());
			//    }

			//    ShowStatusMessage(string.Format("{0} template(s) copied.", _librariesGrid.SelectedItems.Count.ToString()), StatusMessageType.Success);
			//    _librariesGrid.MasterTableView.Rebind();
			//}

			 // TODO uncomment when copy function will be ready


			// TODO remove it when copy function will be ready
			if (_librariesGrid.SelectedItems.Count > 0)
				ShowStatusMessage("Copy function is not implemented yet.", StatusMessageType.Error);
        }

        private void DeleteSelected()
        {
            if (_librariesGrid.SelectedItems.Count > 0)
            {
                foreach (GridItem item in _librariesGrid.SelectedItems)
                {
                    int id = Int32.Parse(((GridDataItem)item)["ID"].Text);
                    LibraryTemplateManager.DeleteLibraryTemplate(UserManager.GetCurrentPrincipal(), id);
                }

                ShowStatusMessage(string.Format("{0} template(s) deleted.", _librariesGrid.SelectedItems.Count.ToString()), StatusMessageType.Success);
                _librariesGrid.MasterTableView.Rebind();
            }
        }

        protected void SearchControl_Search(object sender, EventArgs e)
        {
        }

        protected void SearchControl_ClearSearch(object sender, EventArgs e)
        {
        }

        #region IStatusPage Members

        public void WireStatusControl(Control sourceControl)
        {
            RadAjaxManager manager = RadAjaxManager.GetCurrent(this.Page);
            manager.AjaxSettings.AddAjaxSetting(sourceControl, _statusControl);
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
            _statusControl.Message = message;
            _statusControl.MessageType = messageType;
            _statusControl.ActionText = actionText;
            _statusControl.ActionArgument = actionArgument;
            _statusControl.ShowStatus();
        }

        #endregion
    }
}
