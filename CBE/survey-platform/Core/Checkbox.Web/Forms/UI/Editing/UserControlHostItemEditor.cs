using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;

namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Simple item editor control that hosts user controls
    /// </summary>
    public class UserControlHostItemEditor : Checkbox.Web.Common.WebControlBase, IItemEditor
    {
        private UserControlItemEditorBase _userControl;

        /// <summary>
        /// 
        /// </summary>
        public UserControlHostItemEditor()
        {
            UpdateTextDecoratorOptions = true;
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
         
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
         
        }

        /// <summary>
        /// Get/set override for user control to use for rendering
        /// </summary>
        public string ControlNameOverride { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ItemTextDecorator TextDecorator
        {
            get
            {
                if (_userControl != null)
                {
                    return _userControl.TextDecorator;
                }

                return null;
            }
        }

        /// <summary>
        /// Throw an exception if user control has no value
        /// </summary>
        private void EnsureUserControl()
        {
            if (_userControl == null)
            {
                throw new Exception("ItemData must be set first before accessing methods/properties of UserControlHostItemEditor.");
            }
        }

        /// <summary>
        /// Get control base path as absolute path from application root. e.g. /Forms/Surveys/Controls/ItemEditors
        /// </summary>
        protected virtual string ControlBasePath { get { return "/Forms/Surveys/Controls/ItemEditors"; } }

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="itemData"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        public void Initialize(int templateId, int pagePosition, ItemData itemData, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            //Clear children before adding
            Controls.Clear();

            //If it isn't PostBack, ensure that child ViewState is empty.
            if (!isPagePostBack)
                ClearChildViewState();

            if (itemData == null)
            {
                throw new Exception("Attempt to set NULL itemData for UserControlHostItemEditor.");
            }

            string controlPath = Utilities.IsNullOrEmpty(ControlNameOverride)
                ? string.Format("{0}{1}/{2}.ascx", ApplicationManager.ApplicationRoot, ControlBasePath, itemData.ItemTypeName)
                : string.Format("{0}{1}/{2}.ascx", ApplicationManager.ApplicationRoot, ControlBasePath, ControlNameOverride);

            //Attempt to load the user control
            var tempControl = new UserControl();
            _userControl = tempControl.LoadControl(controlPath) as UserControlItemEditorBase;

            //Ensure control was created
            if (_userControl == null)
            {
                throw new Exception(string.Format("Control located at [{0}] could not be loaded or was not a UserControlItemEditorBase object.", controlPath));
            }

            //Assign an id
            _userControl.ID = "ItemEditor_" + itemData.ID;
            
            //Add user control to controls collection
            Controls.Add(_userControl);

            //Set model
            _userControl.Initialize(templateId, pagePosition, itemData, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);
        }

        /// <summary>
        /// Control supports embedding appearance editors
        /// </summary>
        public bool SupportsEmbeddedAppearanceEditor
        {
            get
            {
                EnsureUserControl();
                return _userControl.SupportsEmbeddedAppearanceEditor;
            }
        }

        /// <summary>
        /// Embed appearance editor in item editor
        /// </summary>
        /// <param name="appearanceEditor"></param>
        public void EmbedAppearanceEditor(IAppearanceEditor appearanceEditor)
        {
            EnsureUserControl();
            _userControl.EmbedAppearanceEditor(appearanceEditor);
        }

        public bool UpdateTextDecoratorOptions { get; set; }

        /// <summary>
        /// Validate inputs
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            EnsureUserControl();

            //return _userControl.Validate() && !IsAliasDuplicate();
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
        /// Update data
        /// </summary>
        public void UpdateData(bool updateTextDecoratorOptions)
        {
            EnsureUserControl();
            _userControl.UpdateData(updateTextDecoratorOptions);
        }

        /// <summary>
        /// Persist data to database
        /// </summary>
        public int SaveData()
        {
            EnsureUserControl();
            return _userControl.SaveData();
        }


        /// <summary>
        /// Determine if user entered enough information.
        /// </summary>
        /// <param name="message">Contains a warning message, which information should be added to the item</param>
        /// <returns></returns>
        public virtual bool DoesItemContainEnoughInformation(out string message)
        {
            EnsureUserControl();
            return _userControl.DoesItemContainEnoughInformation(out message);
        }

        private bool IsAliasDuplicate()
        {
            var aliases = ItemConfigurationManager.GetItemAliases();
            var currAlias = WebUtilities.GetRequestFormValueBySubstring("_aliasText");
            return aliases.Any(a => a == currAlias);
        }
    }
}
