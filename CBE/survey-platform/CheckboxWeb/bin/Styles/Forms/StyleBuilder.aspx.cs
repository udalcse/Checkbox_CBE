using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Styles;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Styles.Forms
{
    public partial class StyleBuilder : SecuredPage
    {
        private StyleTemplate template;
        private List<string> elementNames;

        [Memento("ElementStyle")]
        public Dictionary<string, Dictionary<string, string>> ElementStyle;
        [Memento("SavedStyleId")]
        public int SavedStyleId;
        [Memento("ElementName")]
        public string SavedElementName;

        [QueryParameter(ParameterName = "s")]
        public int StyleId;

        [QueryParameter(ParameterName = "e")]
        public string ElementName;

        private List<string> ElementNames
        {
            get
            {
                if (elementNames == null)
                {
                    if (Utilities.IsNullOrEmpty(Request.QueryString["e"]))
                    {
                        throw new Exception("No element specified");
                    }

                    elementNames = new List<string>(Request.QueryString["e"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                }

                return elementNames;
            }
        }

        protected override string PageRequiredRolePermission { get { return "Form.Edit"; } }

        private StyleTemplate StyleTemplate
        {
            get
            {
                if (template == null)
                {
                    int templateId = Convert.ToInt32(Request.QueryString["s"]);

                    if (templateId > 0)
                        template = StyleTemplateManager.GetStyleTemplate(templateId);

                    if (template == null)
                        throw new Exception("Unable to load style template with id: " + templateId);
                }

                return template;
            }
        }

        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (!Page.IsPostBack)
            {
                if (StyleId != SavedStyleId || ElementStyle == null)
                {
                    ElementStyle = new Dictionary<string, Dictionary<string, string>>();
                    SavedStyleId = StyleId;
                }
                if (!ElementStyle.ContainsKey(ElementName))
                {
                    ElementStyle[ElementName] = StyleTemplate.GetElementStyle(ElementName);
                }

                BindRepeater();
                UpdatePreview();
            }

            if (ElementStyle == null)
                throw new Exception("Unable to load style element to build.");

            _propertyRepeater.ItemCommand += _propertyRepeater_ItemCommand;
            _addPropertyBtn.Click += _addPropertyBtn_Click;
            Master.OkClick += _saveBtn_Click;
        }

        private void SaveElementStyle()
        {
            foreach (RepeaterItem item in _propertyRepeater.Items)
            {
                String propName = (item.FindControl("_propertyName") as Label).Text;
                String propValue = (item.FindControl("_propertyValue") as TextBox).Text;

                //Handle background image specially since an empty image causes web browser to double request page
                if (propName.Equals("background-image")
                        && Utilities.IsNullOrEmpty(propValue))
                {
                    ElementStyle[ElementName].Remove(propName);
                }
                else
                {
                    ElementStyle[ElementName][propName.Trim()] = propValue;
                }
            }
            SavedStyleId = StyleId;
            SavedElementName = ElementName;
        }

        void _saveBtn_Click(object sender, EventArgs e)
        {
            SaveElementStyle();
            
            var serializer = new JavaScriptSerializer();

            string propertyMap = "[";
            foreach (KeyValuePair<string, Dictionary<string, string>> pair in ElementStyle)
            {
                propertyMap += "[";
                foreach (var property in ElementStyle[pair.Key])
                {
                    propertyMap += "{ Key: '" + property.Key + "', Value: '" + property.Value + "' },";
                }
                propertyMap = propertyMap.TrimEnd(',') + "],";
            }
            propertyMap = propertyMap.TrimEnd(',') + "]";

            Master.CloseDialog("{functionName:'styleEditor.updateCustomProperty', callbackArgs:{classNames: " + serializer.Serialize(ElementStyle.Keys) + ", propertyMap: " + propertyMap + " }}", true);
        }


        void _addPropertyBtn_Click(object sender, ImageClickEventArgs e)
        {
            if (Page.IsValid)
            {
                ElementStyle[ElementName][_newPropertyNameTxt.Text.Trim()] = _newPropertyValueTxt.Text.Trim();

                _newPropertyValueTxt.Text = string.Empty;
                _newPropertyNameTxt.Text = string.Empty;

                BindRepeater();
                UpdatePreview();
            }
        }

        void _propertyRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (Utilities.IsNotNullOrEmpty(e.CommandName) && e.CommandName.Equals("DeleteProperty", StringComparison.InvariantCultureIgnoreCase))
            {
                if (e != null && e is RepeaterCommandEventArgs && e.CommandArgument != null)
                {
                    if (ElementStyle[ElementName].ContainsKey(e.CommandArgument.ToString()))
                    {
                        ElementStyle[ElementName].Remove(e.CommandArgument.ToString());

                        UpdatePreview();
                        BindRepeater();
                    }
                }
            }
        }

        private void UpdatePreview()
        {
            _previewPanel.Style.Clear();

            foreach (string key in ElementStyle[ElementName].Keys)
            {
                _previewPanel.Style[key] = ElementStyle[ElementName][key].Replace("\"", "'");
            }
        }

        private void BindRepeater()
        {
            _propertyRepeater.DataSource = ElementStyle[ElementName];

            _propertyRepeater.DataBind();
        }
    }
}
