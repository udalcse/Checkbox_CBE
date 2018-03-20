using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using Checkbox.Wcf.Services;
using Checkbox.Users;
using System.Web;
using System.Web.Services;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Libraries
{
    /// <summary>
    /// Dialog for editing items.
    /// </summary>
    public partial class EditItem : SecuredPage
    {
        private LibraryTemplate _libraryTemplate;

        [QueryParameter("i", IsRequired = true)]
        public int ItemId { get; set; }

        [QueryParameter("lid", IsRequired = true)]
        public int LibraryTemplateId { get; set; }

        [QueryParameter("w")]
        public bool? RedirectedFromWizard { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        [QueryParameter("isNew")]
        public bool? IsNew { get; set; }


        /// <summary>
        /// Get edit language
        /// </summary>
        public string EditLanguage
        {
            get
            {
                return Utilities.IsNotNullOrEmpty(LanguageCode)
                           ? LanguageCode
                           : TextManager.DefaultLanguage;
            }
        }

        /// <summary>
        /// Get response template being edited
        /// </summary>
        public LibraryTemplate LibraryTemplate
        {
            get
            {
                if (_libraryTemplate == null)
                {
                    _libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(LibraryTemplateId);

                    if (_libraryTemplate == null)
                    {
                        throw new Exception(string.Format("Unable to load library with id {0}.", LibraryTemplateId));
                    }
                }

                return _libraryTemplate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return LibraryTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Library.Edit"; } }

        /// <summary>
        /// Get/set data
        /// </summary>
        private ItemData Data { get; set; }

        /// <summary>
        /// Get/set appearance
        /// </summary>
        private AppearanceData Appearance { get; set; }

        /// <summary>
        /// Bind item to editor.
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Data = LibraryTemplate.GetItem(ItemId, true);

            if (Data == null)
            {
                throw new Exception(string.Format("Unable to load item with id {0}.", ItemId));
            }

            //Not all items have appearances, so it's ok for appearance data to be null
            Appearance = AppearanceDataManager.GetAppearanceDataForItem(ItemId);

            //Initialize component on initial view of page.
            _itemEditor.Initialize(
                LibraryTemplateId,
                1,
                Data,
                Appearance,
                EditLanguage,
                TextManager.SurveyLanguages.ToList(),
                Page.IsPostBack,
                true);

            //Bind click events
            Master.OkClick += _saveButton_Click;
            if (IsNew != null && IsNew.Value)
                Master.CancelClick += _cancelButton_Click;

            //Prevent Enter-Key press handling
            Master.PreventEnterKeyBinding();

            //Set title
            Master.SetTitle(WebTextManager.GetText("/pageText/libraries/editItem.aspx/editLibraryItem"));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (Request.QueryString["cmd"] != null)
                    ClearCurrentItem();
            }
          
        }

        /// <summary>
        /// Handle save click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _saveButton_Click(object sender, EventArgs e)
        {
            if (!_itemEditor.Validate())
            {
                var validationMessage = _itemEditor.GetValidationMessage();
                if (!string.IsNullOrEmpty(validationMessage))
                    Master.ShowStatusMessage(validationMessage, StatusMessageType.Error);
                return;
            }

            //Update data
            _itemEditor.UpdateData();

            //Save item & appearance data
            _itemEditor.SaveData();

            //Update template
            LibraryTemplate.SetItem(_itemEditor.ItemData, true);

            ////Mark template updated
            //LibraryTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);

            //Close window and refresh editor
            if (RedirectedFromWizard.HasValue && RedirectedFromWizard.Value)
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow(window.top.onDialogClosed, {op: 'addItem',result:'ok'});", true);
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow(window.top.onDialogClosed, {op: 'editItem',result:'ok', itemId:" + ItemId + ", libraryId:" + LibraryTemplateId + "});", true);
            }
        }

        /// <summary>
        /// Handle cancel click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _cancelButton_Click(object sender, EventArgs e)
        {
            ClearCurrentItem();

            closeWindow();
        }

        /// <summary>
        /// Closes this window returning back item ID and library ID
        /// </summary>
        private void closeWindow()
        {
            var args = new Dictionary<string, string>();
            args.Add("op", "addItem");
            args.Add("result", ItemId > 0 ? "ok" : "cancel");
            if (ItemId > 0)
            {
                args.Add("itemId", ItemId.ToString());
                args.Add("libraryId", LibraryTemplateId.ToString());
            }
            Master.CloseDialog(args);
        }

     
        public  void ClearCurrentItem()
        {
            if (ItemId > 0)
            {
                //Delete an empty item that has been added after choosing item type
                LibraryTemplate template = LibraryTemplateManager.GetLibraryTemplate(LibraryTemplateId);
                TemplatePage page = template.GetPageAtPosition(1);
                template.DeleteItemFromPage(page.ID.Value, ItemId);

                template.ModifiedBy = ((CheckboxPrincipal) HttpContext.Current.User).Identity.Name;
                template.Save();
            }
        }
    }
}
