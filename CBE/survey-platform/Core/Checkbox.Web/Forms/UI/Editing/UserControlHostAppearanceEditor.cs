using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Management;

namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Appearance editor that acts as a host for a user control to do the actual appearance editing.
    /// </summary>
    public class UserControlHostAppearanceEditor : Checkbox.Web.Common.WebControlBase, IAppearanceEditor
    {
        private UserControlAppearanceEditorBase _userControl;

        /// <summary>
        /// Wrap control in div.
        /// </summary>
        protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }

        /// <summary>
        /// Get/set override for user control to use for rendering
        /// </summary>
        public string ControlNameOverride { get; set; }

        /// <summary>
        /// Throw an exception if user control has no value
        /// </summary>
        private void EnsureUserControl()
        {
            if (_userControl == null)
            {
                throw new Exception("AppearanceData must be set first before accessing methods/properties of UserControlHostAppearanceEditor.");
            }
        }

        /// <summary>
        /// Get path to folder containing appearance editors
        /// </summary>
        protected virtual string BaseControlPath { get { return "/Forms/Surveys/Controls/AppearanceEditors"; } }

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="data"></param>
        public void Initialize(AppearanceData data)
        {
            if (data == null)
            {
                throw new Exception("Attempt to set NULL data for UserControlHostAppearanceEditor.");
            }

            string controlPath = Utilities.IsNullOrEmpty(ControlNameOverride)
                ? string.Format("{0}{1}/{2}.ascx", ApplicationManager.ApplicationRoot, BaseControlPath, data.AppearanceCode)
                : string.Format("{0}{1}/{2}.ascx", ApplicationManager.ApplicationRoot, BaseControlPath, ControlNameOverride);

            //Attempt to load the user control
            UserControl tempControl = new UserControl();
            _userControl = tempControl.LoadControl(controlPath) as UserControlAppearanceEditorBase;

            //Ensure control was created
            if (_userControl == null)
            {
                throw new Exception(string.Format("Control located at [{0}] could not be loaded or was not a UserControlAppearanceEditorBase object.", controlPath));
            }

            //Set model
            _userControl.Initialize(data);

            //Add user control to controls collection
            Controls.Add(_userControl);
        }

        /// <summary>
        /// Validate inputs
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            EnsureUserControl();
            return _userControl.Validate();
        }

        /// <summary>
        /// Update data
        /// </summary>
        public void UpdateData()
        {
            EnsureUserControl();
            _userControl.UpdateData();
        }

        /// <summary>
        /// Save data
        /// </summary>
        public void SaveData()
        {
            EnsureUserControl();
            _userControl.SaveData();
        }
    }
}
