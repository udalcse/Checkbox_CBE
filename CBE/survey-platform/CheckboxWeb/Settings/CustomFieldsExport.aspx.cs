using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class CustomFieldsExport : SettingsPage
    {
        private Dictionary<string, Control> _propertyCheckboxes;

        private List<ProfileProperty> _profileProperties;

        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.Title = WebTextManager.GetText("/pageText/settings/customFieldsExport.aspx/title");
            Master.IsDialog = true;

            _propertyCheckboxes = new Dictionary<string, Control>();
            _profileProperties = ProfileManager.GetPropertiesList();

            Master.OkVisible = false;
            Master.ForceLeftButtonAlign = true;
            _exportProperties.Enabled = false;
            _exportProperties.Click += ExportProperties_Click;

            _profileFieldRepeater.DataSource = _profileProperties.ToDictionary(property => property.Name,
                property => property.FieldType);
            _profileFieldRepeater.DataBind();

        }

        protected void PropertyList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                _propertyCheckboxes.Add(e.Item.DataItem.ToString(), e.Item.FindControl("_propertyCheckBox"));
        }

        protected void PropertyCheckBox_Click(object sender, EventArgs e)
        {
            _exportProperties.Enabled = _propertyCheckboxes.Any(item => ((CheckBox) item.Value).Checked);
        }

        protected void ExportProperties_Click(object sender, EventArgs e)
        {
            List<ProfileProperty> propertiesToSerialize = new List<ProfileProperty>();
            GetDownloadResponse("PropertiesExport.xml");

            foreach (var checkboxKey in _propertyCheckboxes.Where(p => ((CheckBox)p.Value).Checked).Select(p => p.Key))
            {
                var property =
                    _profileProperties.FirstOrDefault(
                        item => item.Name.Equals(((CheckBox) _propertyCheckboxes[checkboxKey]).Text));
                
                if (property != null)
                {
                    if (property.FieldType == CustomFieldType.Matrix)
                    {
                        var matrix = ProfileManager.GetMatrixField(property.Name, UserManager.GetCurrentPrincipal().UserGuid);
                        var matrixToSerialize = new MatrixProfileProperty(matrix);
                        propertiesToSerialize.Add(matrixToSerialize);
                        
                    }
                    else if (property.FieldType == CustomFieldType.RadioButton)
                    {
                        var radioButton = ProfileManager.GetRadioButtonField(property.Name, UserManager.GetCurrentPrincipal().UserGuid);

                        propertiesToSerialize.Add(radioButton);
                    }
                    else
                    {
                        propertiesToSerialize.Add(property);
                    }
                }
            }

            XmlSerializer ser = new XmlSerializer(typeof(List<ProfileProperty>));
            ser.Serialize(Response.Output, propertiesToSerialize);
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// Configures response stream for xml
        /// </summary>
        /// <param name="fileName"></param>
        protected void GetDownloadResponse(string fileName)
        {
            Response.Expires = -1;
            Response.BufferOutput = Checkbox.Management.ApplicationManager.AppSettings.BufferResponseExport;
            Response.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            Response.ContentType = "application/octet-stream";
        }

    }
}