using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Page;
using CheckboxWeb.Services;

namespace CheckboxWeb.Settings
{
    public partial class CustomFieldsImport : SettingsPage
    {
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.IsDialog = true;
            Master.Title = WebTextManager.GetText("/pageText/settings/customFieldsImport.aspx/title");
            Master.HideDialogButtons();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
            ClientScript.RegisterStartupScript(GetType(), "HideStatus", "HideStatus();", true);
            Master.HideStatusMessage();
            _errorPanel.Visible = false;
            _sucessPanel.Visible = false;

            if (IsPostBack)
            {
                if (!ValidateFileExtension())
                {
                    var uploadStatus = "Incorrect file extension";
                    ShowStatusMessage(uploadStatus);
                    _errorFileUpload.Value = uploadStatus;
                }
                else
                {
                    _uploader.Visible = false;
                    _errorFileUpload.Value = string.Empty;
                }
            }
        }

        protected void _wizard_NextButtonClick(object sender, EventArgs e)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ProfileProperty>));
            var fileBytes = Upload.GetDataForFile(HttpContext.Current, _uploadedFilePathTxt.Text);
            var ms = new MemoryStream(fileBytes);
            object deserializedData;
            string error = string.Empty;

            try
            {
                deserializedData = xmlSerializer.Deserialize(ms);
            }
            catch
            {
                ShowErrorMessage("The file is invalid");
                return;
            }

            if (deserializedData is List<ProfileProperty>)
            {
                var allPropsFromXml = deserializedData as List<ProfileProperty>;
                var currentProfileProperties = ProfileManager.GetPropertiesList();
                var propertiesToImport = allPropsFromXml.Where(pXml => currentProfileProperties.All(currProp => currProp.Name != pXml.Name)).ToList();
                var conflictProps = allPropsFromXml.Except(propertiesToImport).ToList();
                conflictProps.ForEach(p => error += ($"{p.Name} was not imported. There were conflicts with existing profile properties.<br>"));

                foreach (var profileProperty in propertiesToImport)
                {
                    ProfileManager.AddProfileField(profileProperty.Name, true, profileProperty.IsHidden);

                    if (profileProperty.FieldType == CustomFieldType.Matrix)
                    {
                        SaveMatrixField(profileProperty as MatrixProfileProperty);
                        
                    }
                    else if (profileProperty.FieldType == CustomFieldType.RadioButton)
                    {
                        ProfileManager.AddRadioButtonField(profileProperty as RadioButtonField);
                    }
                    else
                    {
                        ProfileManager.UpdateProfileField(profileProperty);
                    }
                }

                if (string.IsNullOrEmpty(error))
                    ShowSuccessMessage("The profile properties were imported successfully");
                else
                    ShowErrorMessage(error);
            }
        }

        protected void _wizard_PreviousButtonClick(object sender, EventArgs e)
        {
            _uploader.Visible = true;
        }

        private void SaveMatrixField(MatrixProfileProperty matrix)
        {
            MatrixField matrixField = new MatrixField();

            if (!string.IsNullOrEmpty(matrix.Name))
            {
                matrixField.FieldName = matrix.Name;
                matrixField.GridLines = matrix.GridLines;
                matrixField.IsColumnsFixed = matrix.IsColumnsFixed;
                matrixField.IsRowsFixed = matrix.IsRowsFixed;
                matrixField.BuildMatrixCells(matrix.ColumnHeaders, matrix.RowHeaders, matrix.ColumnHeaders.Count, matrix.RowHeaders.Count);
                ProfileManager.AddMatrixField(matrixField);
            }
        }

        /// <summary>
        /// Maps the RadioButton option ids to be able to save them.
        /// </summary>
        /// <param name="fieldOptions">The field options.</param>
        /// <param name="aliases">The aliases.</param>
        private void MapRadioButtonOptionIds(List<RadioButtonFieldOption> fieldOptions, List<RadioButtonFieldOption> aliases  )
        {
            foreach (var alias in aliases)
            {
                var radioButtonFieldOption =
                    fieldOptions.FirstOrDefault(
                        radioOption => radioOption.Name.Equals(alias.Name));

                if (radioButtonFieldOption != null)
                    alias.Id = radioButtonFieldOption.Id;
            }
        }

        private bool ValidateFileExtension()
        {
            return ".xml".Equals(Path.GetExtension(_uploadedFileNameTxt.Text), StringComparison.InvariantCultureIgnoreCase);
        }

        private void ShowErrorMessage(string message)
        {
            _errorMessageLiteral.Text = message;
            _errorPanel.Visible = true;
        }

        private void ShowSuccessMessage(string message)
        {
            _successMessageLiteral.Text = message;
            _sucessPanel.Visible = true;
        }

        private void ShowStatusMessage(string message)
        {
            Master.ShowStatusMessage(message, StatusMessageType.Error);
        }
    }
}