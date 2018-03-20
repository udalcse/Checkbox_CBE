using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance editor for Slider items
    /// </summary>
    public partial class Slider : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Stores selected images position
        /// </summary>
        private string ImageOrientation
        {
            get
            {
                return (string)Session[ID + "_ImageOrientation"];
            }
            set
            {
                Session[ID + "_ImageOrientation"] = value;
            }
        }

        /// <summary>
        /// Stores selected alias text position
        /// </summary>
        private string AliasTextOrientation
        {
            get
            {
                return (string)Session[ID + "_AliasTextOrientation"];
            }
            set
            {
                Session[ID + "_AliasTextOrientation"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
             "jquery.numeric.js",
             ResolveUrl("~/Resources/jquery.numeric.js"));

            _imageOrientationList.SelectedIndexChanged += _imageOrientationList_SelectedIndexChanged;
            _aliasOrientationList.SelectedIndexChanged += _aliasOrientationList_SelectedIndexChanged;

            // restore values from session
            if (!string.IsNullOrEmpty(ImageOrientation))
                _imageOrientationList.SelectedValue = ImageOrientation;
            if (!string.IsNullOrEmpty(AliasTextOrientation))
                _aliasOrientationList.SelectedValue = AliasTextOrientation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _imageOrientationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImageOrientation = _imageOrientationList.SelectedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _aliasOrientationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AliasTextOrientation = _aliasOrientationList.SelectedValue;
        }

        /// <summary>
        /// Inititalize appearance editor
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (_orientationList.Items.FindByValue(data["Orientation"]) != null)
            {
                _orientationList.SelectedValue = data["Orientation"];
            }

            if (_aliasOrientationList.Items.FindByValue(data["AliasPosition"]) != null)
            {
                _aliasOrientationList.SelectedValue = data["AliasPosition"];
            }

            if (_aliasOrientationList.Items.FindByValue(data["ImagePosition"]) != null)
            {
                _imageOrientationList.SelectedValue = data["ImagePosition"];
            }

            _showLabelCkbx.Checked = 
                data["ShowValue"] != null 
                && data["ShowValue"].Equals("True", StringComparison.InvariantCultureIgnoreCase);

            _widthTxt.Text = data["Width"] ?? String.Empty;
            _heightTxt.Text = data["Height"] ?? String.Empty;
        }

        /// <summary>
        /// Update data with control inputs.
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["Orientation"] = _orientationList.SelectedValue;
            AppearanceData["ShowValue"] = _showLabelCkbx.Checked.ToString();
            AppearanceData["Width"] = _widthTxt.Text;
            AppearanceData["Height"] = _heightTxt.Text;
            AppearanceData["AliasPosition"] = _aliasOrientationList.SelectedValue;
            AppearanceData["ImagePosition"] = _imageOrientationList.SelectedValue;

            //reset session values
            ImageOrientation = null;
            AliasTextOrientation = null;
        }
    }
}