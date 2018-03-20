using System;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Web.Common;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class SingleLineTextAutocomplete : UserControlBase
    {
        public void Initialize(SingleLineTextItemData data)
        {
            if (data == null)
                return;

            RebuildLists(data.AutocompleteListId);

            _autocompleteLists.SelectedValue = data.AutocompleteListId.HasValue ? data.AutocompleteListId.ToString() : "empty";

            if (ApplicationManager.AppSettings.AllowAutocompleteRemoteSource)
            {
                if (!string.IsNullOrEmpty(data.AutocompleteRemote))
                {
                    _remoteServer.Text = data.AutocompleteRemote;
                    _sourceList.SelectedValue = "Remote";
                }
            }
            else
            {
                _sourceList.Visible = false;
                _remotePanel.Visible = false;
            }
        }

        private void RebuildLists(int? listId)
        {
            _autocompleteLists.Items.Clear();

            //add no autocomplete option 
            _autocompleteLists.Items.Add(new ListItem(TextManager.GetText("/controlText/singleLineTextEditor/noListSelected"), "empty"));

            //add new option
            _autocompleteLists.Items.Add(new ListItem(TextManager.GetText("/controlText/singleLineTextEditor/addNewList"), "new"));

            //add all lists
            var allLists = AutocompleteListManager.ListAllListNames();
            foreach (var list in allLists)
            {
                var item = new ListItem(list.Value, list.Key.ToString());
                _autocompleteLists.Items.Add(item);
            }

            _autocompleteLists.SelectedValue = listId.HasValue ? listId.ToString() : "empty";
        }

        public void SaveData(SingleLineTextItemData data)
        {
            if (data == null)
                return;

            if (_sourceList.SelectedValue == "Remote" && ApplicationManager.AppSettings.AllowAutocompleteRemoteSource)
            {
                data.AutocompleteListId = null;
                data.AutocompleteRemote = Utilities.RemoveScript(_remoteServer.Text);
            }
            else
            {
                data.AutocompleteRemote = null;
                var listId = Utilities.AsInt(_autocompleteLists.SelectedValue);

                var name = _listName.Text;
                if (_autocompleteLists.SelectedValue.Equals("new", StringComparison.InvariantCultureIgnoreCase))
                {
                    var items = _listData.Text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    listId = AutocompleteListManager.CreateList(name, items);
                }
                if (listId >= 1000)
                {
                    var items = _listData.Text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    AutocompleteListManager.EditList(listId.Value, name, items);
                }

                data.AutocompleteListId = listId;

                RebuildLists(listId);
            }
        }
    }
}