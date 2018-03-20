using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Users;
using Prezza.Framework.Security;

namespace Checkbox.Web.UI.Controls.GridTemplates
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalizedHeaderAclEntryField : LocalizedHeaderTemplateField
    {
        /// <summary>
        /// Get/set css class for label
        /// </summary>
        public string LabelCssClass { get; set; }

        /// <summary>
        /// Initialize cell with label control
        /// </summary>
        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            if (cellType != DataControlCellType.DataCell)
            {
                base.InitializeCell(cell, cellType, rowState, rowIndex);
            }
            else
            {
                var entryLabel = new Label { CssClass = LabelCssClass };
                entryLabel.DataBinding += EntryLabelDataBinding;

                cell.Controls.Add(entryLabel);
            }
        }

        /// <summary>
        /// Bind label to data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void EntryLabelDataBinding(object sender, EventArgs e)
        {
            //Get the label to update
            var entryLabel = sender as Label;

            if (entryLabel == null)
            {
                return;
            }

            //Now get the data
            Control namingContainer = entryLabel.NamingContainer;

            if (namingContainer == null)
            {
                return;
            }

            var entry = DataBinder.GetDataItem(namingContainer) as IAccessControlEntry;

            if (entry == null)
            {
                return;
            }

            //For groups attempt to get the group's name.
            if (entry.AclEntryTypeIdentifier.Equals("Checkbox.Users.Group", StringComparison.InvariantCultureIgnoreCase))
            {
                int groupId;

                if (int.TryParse(entry.AclEntryIdentifier, out groupId))
                {
                    Group group = GroupManager.GetGroup(groupId);

                    if (group != null)
                    {
                        entryLabel.Text = group.Name;
                        return;
                    }
                }
            }

            //Default to entry identifier
            entryLabel.Text = entry.AclEntryIdentifier;
        }
    }
}
