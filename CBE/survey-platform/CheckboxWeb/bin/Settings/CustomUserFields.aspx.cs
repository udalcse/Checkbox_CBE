using System;
using System.Data;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Web.UI.Controls;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Logging;

namespace CheckboxWeb.Settings
{
    public partial class CustomUserFields : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/customUserFields.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);

            Master.HideDialogButtons();

            ConfigurePropertiesGrid();

            if (!IsAuthorizedToExport())
            {
                _exportProfile.Visible = false;
                _importProfile.Visible = false;
            }

            _properties.DataSource = ProfileManager.ListPropertyNames();
            _properties.DataBind();
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
            ClientScript.RegisterStartupScript(GetType(), "HideStatus", "HideStatus();", true);
            _newFieldError.Visible = false;
            Master.HideStatusMessage();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Grid_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case "DeleteField":
                        DeleteProperty(((MultiLanguageImageButton) e.CommandSource).CommandArgument);
                        break;

                    case "MoveUp":
                        MoveProperty(((MultiLanguageImageButton) e.CommandSource).CommandArgument, true);
                        break;

                    case "MoveDown":
                        MoveProperty(((MultiLanguageImageButton) e.CommandSource).CommandArgument, false);
                        break;
                    //case "EditField":
                    //    EditProperty(((MultiLanguageButton)e.CommandSource).CommandArgument);
                    //    break;


                    case "AddField":
                        if (ProfileManager.IsValidFieldName(((TextBox) e.Item.FindControl("newField")).Text))
                            AddProperty(((TextBox) e.Item.FindControl("newField")).Text);
                        else
                        {
                            _newFieldError.Visible = true;
                            Master.ShowStatusMessage(
                                WebTextManager.GetText("/pageText/settings/customUserFields.aspx/invalidName"),
                                StatusMessageType.Error);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
            }
        }

        private void EditProperty(string propertyName)
        {
            
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "MatrixEdit()", true);
            //Attributes["onclick"] = "javascript:return MatrixEdit('" + WebTextManager.GetText("/pageText/settings/customUserFields.aspx/deleteFieldConfirm") + "',this);";
            //if (ProfileManager.GetPropertiesList().Any(p => p.FieldType == CustomFieldType.RadioButton && p.Name == propertyName))
            //{
            //    ProfileManager.DeleteRadioButtonOptions(propertyName);
            //}

            //ProfileManager.DeleteProfileField(propertyName);
            //_properties.DataSource = ProfileManager.ListPropertyNames();
            //_properties.DataBind();

            //string message = string.Format("{0} {1}", propertyName,
            //    WebTextManager.GetText("/pageText/settings/customUserFields.aspx/deleteSuccess"));
            //Master.ShowStatusMessage(message, StatusMessageType.Success);
        }



        private void AddProperty(string propertyName)
        {
            ProfileManager.AddProfileField(propertyName.Trim(), true, _isHidden.Checked);
            _properties.DataSource = ProfileManager.ListPropertyNames();
            _properties.DataBind();

            string message = string.Format("{0} {1}", propertyName,
                WebTextManager.GetText("/pageText/settings/customUserFields.aspx/addSuccess"));
            Master.ShowStatusMessage(message, StatusMessageType.Success);
        }

        private void DeleteProperty(string propertyName)
        {
            if(ProfileManager.GetPropertiesList().Any(p => p.FieldType == CustomFieldType.RadioButton && p.Name == propertyName))
            {
                ProfileManager.DeleteRadioButtonOptions(propertyName);
            }

            ProfileManager.DeleteProfileField(propertyName);
            _properties.DataSource = ProfileManager.ListPropertyNames();
            _properties.DataBind();

            string message = string.Format("{0} {1}", propertyName,
                WebTextManager.GetText("/pageText/settings/customUserFields.aspx/deleteSuccess"));
            Master.ShowStatusMessage(message, StatusMessageType.Success);
        }

        private void MoveProperty(string propertyName, bool moveUp)
        {
            ProfileManager.MoveProfileField(propertyName, moveUp);
            _properties.DataSource = ProfileManager.ListPropertyNames();
            _properties.DataBind();

            string message;
            if (moveUp)
                message = string.Format("{0} {1}", propertyName,
                    WebTextManager.GetText("/pageText/settings/customUserFields.aspx/moveUpSuccess"));
            else
                message = string.Format("{0} {1}", propertyName,
                    WebTextManager.GetText("/pageText/settings/customUserFields.aspx/moveDownSuccess"));

            Master.ShowStatusMessage(message, StatusMessageType.Success);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConfigurePropertiesGrid()
        {
            _properties.ShowHeader = true;
            _properties.ShowFooter = true;
            _properties.AutoGenerateColumns = false;

            _properties.Columns.Add(new TemplateColumn
            {
                ItemTemplate = new CustomFieldsTemplate(),
                FooterTemplate = new CustomFieldsFooter(),
                HeaderText = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/name")
            });
            _properties.Columns.Add(new TemplateColumn
            {
                ItemTemplate = new MoveUp(),
                //FooterTemplate = new AddCustomField(),
                HeaderText = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/moveUp")
            });
            _properties.Columns.Add(new TemplateColumn
            {
                ItemTemplate = new MoveDown(),
                HeaderText = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/moveDown")
            });
            _properties.Columns.Add(new TemplateColumn
            {
                ItemTemplate = new DeleteCustomField(),
                HeaderText = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/delete")
            });
            _properties.Columns.Add(new TemplateColumn
            {
                ItemTemplate = new ToggleVisibility(),
                HeaderText = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/isHidden")
            });

            _properties.Columns.Add(new TemplateColumn
            {
                ItemTemplate = new CustomUserFieldType(),
                HeaderText = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/fieldType")
            });

            _properties.Columns.Add(new TemplateColumn
            {
                ItemTemplate = new EditType(),
                HeaderText = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/edit")
            });



            _properties.ItemCommand += Grid_ItemCommand;
        }

        private class EditType : ITemplate
        {
            #region ITemplate Members
            public void InstantiateIn(Control container)
            {
                MultiLanguageButton editBtn = new MultiLanguageButton
                {
                    //SkinID = "UserFieldEditButton",
                    Text="Edit",
                    Width = 16,
                    Height = 16,
                    //CommandName = "EditField"
                };
                editBtn.Attributes["onclick"] = "javascript:OpenMatrixEdit(this);";
                editBtn.Attributes["uframeignore"] = "true";
                editBtn.DataBinding += editBtn_DataBinding;

                container.Controls.Add(editBtn);
            }

            private static void editBtn_DataBinding(object sender, EventArgs e)
            {
                string fieldName = (string)((DataGridItem)((MultiLanguageButton)sender).NamingContainer).DataItem;
                ((MultiLanguageButton)sender).CommandArgument = fieldName;
                ((MultiLanguageButton)sender).ID = "edit_" + fieldName;
                ((MultiLanguageButton)sender).CssClass = "custom-field-edit";

                //Database db = DatabaseFactory.CreateDatabase();
                //DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_GetIsDeletable");
                //command.AddInParameter("CustomUserFieldName", DbType.String, fieldName);

                //try
                //{
                //    object showDelete = db.ExecuteScalar(command);
                //    if (!Convert.ToBoolean(showDelete))
                //    {
                //        //                        ((MultiLanguageImageButton)sender).SkinID
                //        ((MultiLanguageImageButton)sender).ImageUrl = ".." + ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/can_not_delete16.gif";
                //        ((MultiLanguageImageButton)sender).Enabled = false;
                //        ((MultiLanguageImageButton)sender).ToolTip = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/customFieldRequired");
                //        ((MultiLanguageImageButton)sender).Attributes["onclick"] = string.Empty;
                //        ((MultiLanguageImageButton)sender).Attributes["style"] = "cursor:none;";
                //    }
                //}
                //catch { }
            }
            #endregion
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static bool UpdateCustomFieldType(string fieldName, string fieldType)
        {
            try
            {
                if (!(string.IsNullOrEmpty(fieldName) && string.IsNullOrEmpty(fieldType)))
                {
                    CustomFieldType type;

                    var isTypeValid = Enum.TryParse(fieldType, out type);

                    if (isTypeValid)
                    {
                        if (ProfileManager.GetPropertiesList().Any(p => p.FieldType == CustomFieldType.RadioButton && p.Name == fieldName))
                        {
                            ProfileManager.DeleteRadioButtonOptions(fieldName);
                        }

                        ProfileManager.UpdateProfileField(new ProfileProperty()
                        {
                            Name = fieldName,
                            FieldType = type
                        });

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
            }

            return false;
        }

        private bool IsAuthorizedToExport()
        {
            var userPrincipal = UserManager.GetCurrentPrincipal();
            if (userPrincipal.IsInRole("System Administrator") || userPrincipal.IsInRole("User Administrator"))
                return true;

            return false;
        }


        private class DeleteCustomField : ITemplate
        {
            #region ITemplate Members
            public void InstantiateIn(Control container)
            {
                MultiLanguageImageButton deleteBtn = new MultiLanguageImageButton
                {
                    SkinID = "UserFieldDeleteButton",
                    Width = 16,
                    Height = 16,
                    CommandName = "DeleteField"
                };
                deleteBtn.Attributes["onclick"] = "javascript:return showRemoveFieldConfirmDialog('" + WebTextManager.GetText("/pageText/settings/customUserFields.aspx/deleteFieldConfirm") + "',this);";
                deleteBtn.Attributes["uframeignore"] = "true";
                deleteBtn.DataBinding += deleteBtn_DataBinding;

                container.Controls.Add(deleteBtn);
            }

            private static void deleteBtn_DataBinding(object sender, EventArgs e)
            {
                string fieldName = (string)((DataGridItem)((MultiLanguageImageButton)sender).NamingContainer).DataItem;
                ((MultiLanguageImageButton)sender).CommandArgument = fieldName;

                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_GetIsDeletable");
                command.AddInParameter("CustomUserFieldName", DbType.String, fieldName);

                try
                {
                    object showDelete = db.ExecuteScalar(command);
                    if (!Convert.ToBoolean(showDelete))
                    {
                        //                        ((MultiLanguageImageButton)sender).SkinID
                        ((MultiLanguageImageButton)sender).ImageUrl = ".." + ApplicationManager.ApplicationRoot + "/App_Themes/CheckboxTheme/Images/can_not_delete16.gif";
                        ((MultiLanguageImageButton)sender).Enabled = false;
                        ((MultiLanguageImageButton)sender).ToolTip = WebTextManager.GetText("/pageText/settings/customUserFields.aspx/customFieldRequired");
                        ((MultiLanguageImageButton)sender).Attributes["onclick"] = string.Empty;
                        ((MultiLanguageImageButton)sender).Attributes["style"] = "cursor:none;";
                    }
                }
                catch { }
            }
            #endregion
        }

        private class MoveUp : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                MultiLanguageImageButton moveUpBtn = new MultiLanguageImageButton
                {
                    SkinID = "UserFieldMoveUpButton",
                    Width = 16,
                    Height = 16,
                    CommandName = "MoveUp"
                };

                moveUpBtn.DataBinding += moveUpBtn_DataBinding;
                container.Controls.Add(moveUpBtn);
            }

            private static void moveUpBtn_DataBinding(object sender, EventArgs e)
            {
                string s = (string)((DataGridItem)((MultiLanguageImageButton)sender).NamingContainer).DataItem;
                ((MultiLanguageImageButton)sender).CommandArgument = s;
            }
        }

        private class MoveDown : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                MultiLanguageImageButton moveDownBtn = new MultiLanguageImageButton
                {
                    SkinID = "UserFieldMoveDownButton",
                    Width = 16,
                    Height = 16,
                    CommandName = "MoveDown"
                };

                moveDownBtn.DataBinding += moveDownBtn_DataBinding;
                container.Controls.Add(moveDownBtn);
            }

            private static void moveDownBtn_DataBinding(object sender, EventArgs e)
            {
                string s = (string)((DataGridItem)((MultiLanguageImageButton)sender).NamingContainer).DataItem;
                ((MultiLanguageImageButton)sender).CommandArgument = s;
            }
        }

        public class ToggleVisibility : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                CheckBox isEditable = new CheckBox();
                isEditable.CheckedChanged += ToggleIsEditiable_Event;
                isEditable.DataBinding += IsEditable_DataBinding;
                isEditable.AutoPostBack = true;

                container.Controls.Add(isEditable);
            }

            private void ToggleIsEditiable_Event(object sender, EventArgs e)
            {
                string fieldName = ((CheckBox)sender).ID;
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_UpdateHidden");
                command.AddInParameter("CustomUserFieldName", DbType.String, fieldName);
                command.AddInParameter("Hidden", DbType.Boolean, ((CheckBox)sender).Checked);

                try
                {
                    db.ExecuteNonQuery(command);
                }
                catch { }

                //                string message;
                //                if (((CheckBox)sender).Checked)
                //                    message = string.Format("{0} {1}", fieldName, WebTextManager.GetText("/pageText/settings/customUserFields.aspx/moveUpSuccess"));
                //                else
                //                    message = string.Format("{0} {1}", fieldName, WebTextManager.GetText("/pageText/settings/customUserFields.aspx/moveDownSuccess"));
                //
                //                ShowStatusMessage(message, StatusMessageType.Success);
            }

            private static void IsEditable_DataBinding(object sender, EventArgs e)
            {
                string fieldName = (string)((DataGridItem)((CheckBox)sender).NamingContainer).DataItem;

                ((CheckBox)sender).Checked = ProfileManager.IsFieldHidden(fieldName);
                ((CheckBox)sender).ID = fieldName;
            }
        }

        private class CustomFieldsTemplate : ITemplate
        {
            #region ITemplate Members
            public void InstantiateIn(Control container)
            {
                Label l = new Label();
                l.DataBinding += l_DataBinding;
                container.Controls.Add(l);
                ((TableCell)container).HorizontalAlign = HorizontalAlign.Left;
            }

            private static void l_DataBinding(object sender, EventArgs e)
            {
                ((Label)sender).Text = (string)((DataGridItem)((Label)sender).NamingContainer).DataItem;
            }
            #endregion
        }

        public class CustomFieldsFooter : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                TextBox addField = new TextBox { Width = 150, ID = "newField" };
                MultiLanguageImageButton addBtn = new MultiLanguageImageButton
                {
                    SkinID = "UserFieldAddButton",
                    Width = 20,
                    Height = 20,
                    CommandName = "AddField",
                    ImageAlign = ImageAlign.Top
                };

                container.Controls.Clear();
                container.Controls.Add(addField);
                container.Controls.Add(addBtn);
            }
        }



        public class CustomUserFieldType : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                var dropDownList = new DropDownList();

                //dropDownList.SelectedIndexChanged += customTypedropDown_SelectedIndexChanged;
                dropDownList.DataBinding += customTypedropDown_DataBind;
               
                container.Controls.Add(dropDownList);
            }

            //private static void customTypedropDown_SelectedIndexChanged(object sender, EventArgs e)
            //{
            //    var fieldName = ((DropDownList)sender).ID.Replace("dd_", string.Empty);
            //    var fieldType = ((DropDownList)sender).SelectedItem;

            //    ProfileManager.UpdateProfileField(new ProfileProperty()
            //    {
            //        Name = fieldName,
            //        //FieldType = ParseEnum<CustomFieldType>(fieldType.Text)
            //    });
            //}

            private static void customTypedropDown_DataBind(object sender, EventArgs e)
            {
                string fieldName = (string) ((DataGridItem) ((DropDownList) sender).NamingContainer).DataItem;
                ((DropDownList) sender).ID = "dd_" + fieldName;
                ((DropDownList) sender).CssClass = "custom-field-dd";
                var properties = ProfileManager.GetPropertiesList();

                var values = ProfileManager.GetFiledTypesList();

                if (properties != null)
                {
                    foreach (var item in values)
                        ((DropDownList)sender).Items.Add(new ListItem(item.FieldType.ToString(), item.FieldType.ToString()));
                }

                var currentType = properties.FirstOrDefault(prop => prop.Name.Equals(fieldName)).FieldType.ToString();
                ((DropDownList) sender).Items.FindByText(currentType).Selected = true;
            }
        }

    }
}