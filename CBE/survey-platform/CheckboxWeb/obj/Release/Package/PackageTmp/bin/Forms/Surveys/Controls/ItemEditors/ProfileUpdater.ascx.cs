using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;
using ProfileManager=Checkbox.Security.ProfileManager;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Item editor for profile updater items
    /// </summary>
    public partial class ProfileUpdater : UserControlItemEditorBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override IRuleEditor RuleEditor { get { return _conditionEditor; } }

        /// <summary>
        /// 
        /// </summary>
        public override IItemRuleDisplay RuleDisplay { get { return _ruleDisplay; } }

        /// <summary>
        /// 
        /// </summary>
        protected override Control PreviewContainer
        {
            get { return _previewPlace; }
        }

        /// <summary>
        /// 
        /// </summary>
        private class QuestionBindingObject
        {
            public string Text { get; set; }
            public int ItemId { get; set; }
        }

        /// <summary>
        /// Set column header texts
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _propertiesGrid.RowDeleting += _propertiesGrid_RowDeleting;
            _addBtn.Click += _addBtn_Click;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        /// <summary>
        /// Get property list
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private List<ProfileUpdaterItemData.ProfileUpdaterProperty> GetPropertyList(string propertyType)
        {
            if (HttpContext.Current.Session[ID + "_ProfileUpdater" + propertyType + "Properties"] == null)
            {
                HttpContext.Current.Session[ID + "_ProfileUpdater" + propertyType + "Properties"] = new List<ProfileUpdaterItemData.ProfileUpdaterProperty>();
            }

            return HttpContext.Current.Session[ID + "_ProfileUpdater" + propertyType + "Properties"] as List<ProfileUpdaterItemData.ProfileUpdaterProperty>;
        }

        /// <summary>
        /// Set property list
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="properties"></param>
        private void SetPropertyList(string propertyType, List<ProfileUpdaterItemData.ProfileUpdaterProperty> properties)
        {
            HttpContext.Current.Session[ID + "_ProfileUpdater" + propertyType + "Properties"] = properties;
        }

        /// <summary>
        /// Initialize editor
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="pagePosition"></param>
        /// <param name="data"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        public override void Initialize(int templateId, int pagePosition, ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            _activeDisplay.Initialize(data);

            if (!isPagePostBack)
            {
                _currentTabIndex.Text = hidePreview ? "1" : "0";
            }

            if (data != null
                && data is ProfileUpdaterItemData)
            {
                //Reset collection
                if (!isPagePostBack)
                {
                    SetPropertyList("Added", new List<ProfileUpdaterItemData.ProfileUpdaterProperty>());
                    SetPropertyList("Deleted", new List<ProfileUpdaterItemData.ProfileUpdaterProperty>());
                    SetPropertyList("Current", new List<ProfileUpdaterItemData.ProfileUpdaterProperty>(((ProfileUpdaterItemData)data).Properties.Values));
                }
            }


            BindLists();
        }

        /// <summary>
        /// Bind item lists
        /// </summary>
        private void BindLists()
        {
            //Bind lists
            _propertiesGrid.DataSource = GetPropertyList("Current");
            _propertiesGrid.DataBind();

            //Bind question list
            _questionList.DataSource = 
                ItemConfigurationManager
                    .ListResponseTemplateItems(
                        TemplateId,
                        (PagePosition - 1),
                        false,
                        false,
                        true,
                        CurrentLanguage)
                    .Select(itemData => new QuestionBindingObject {Text = itemData.PositionText + " " + itemData.GetText(CurrentLanguage), ItemId = itemData.ItemId});

            _questionList.DataTextField = "Text";
            _questionList.DataValueField = "ItemId";
            _questionList.DataBind();

            //Bind property list
            _propertyList.DataSource = ProfileManager.ListPropertyNames();
            _propertyList.DataBind();

            //Check if any questions are available, if not hide inputs
            _addPlace.Visible = _questionList.Items.Count > 0;
            _noQuestionsPlace.Visible = !_addPlace.Visible;
        }

        /// <summary>
        /// Add new property to update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _addBtn_Click(object sender, EventArgs e)
        {
            //Add a new property to the updater
            //TODO: Validation

            if (ItemData is ProfileUpdaterItemData)
            {
                var newProperty = new ProfileUpdaterItemData.ProfileUpdaterProperty(
                    Int32.Parse(_questionList.SelectedValue),
                    ProfileManager.CheckboxProvider.ProviderName,
                    _propertyList.SelectedValue);

                //Add to "added" and "current" lists
                GetPropertyList("Added").Add(newProperty);
                GetPropertyList("Current").Add(newProperty);

                //Rebind
                BindLists();
            }
        }



        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _propertiesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //Find item deleted
            var currentPropertyList = GetPropertyList("Current");

            if (currentPropertyList.Count <= e.RowIndex)
            {
                return;
            }

            //Add to deleted list
            var propertyToDelete = currentPropertyList[e.RowIndex];

            var deletedPropertyList = GetPropertyList("Deleted");
            deletedPropertyList.Add(propertyToDelete);
            SetPropertyList("Deleted", deletedPropertyList);
            
            //Remove from current
            currentPropertyList.RemoveAt(e.RowIndex);
            SetPropertyList("Current", currentPropertyList);

            //Rebind
            BindLists();
        }

        ///// <summary>
        ///// Delete selected updates
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="e"></param>
        //private void _propertiesGrid_DeleteCommand(object source, GridCommandEventArgs e)
        //{
        //    foreach (GridItem item in _propertiesGrid.SelectedItems)
        //    {
        //        if (item.DataItem != null && item.DataItem is ProfileUpdaterItemData.ProfileUpdaterProperty)
        //        {
        //            //Remove item from current and add to "deleted" list
        //            List<ProfileUpdaterItemData.ProfileUpdaterProperty> currentList = GetPropertyList("Current");

        //            if (currentList.Contains((ProfileUpdaterItemData.ProfileUpdaterProperty)item.DataItem))
        //            {
        //                currentList.Remove((ProfileUpdaterItemData.ProfileUpdaterProperty)item.DataItem);
        //            }

        //            GetPropertyList("Deleted").Add((ProfileUpdaterItemData.ProfileUpdaterProperty)item.DataItem);
        //        }
        //    }
        //}

        /// <summary>
        /// Update profile updater data with new values
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (ItemData is ProfileUpdaterItemData)
            {
                //Remove deleted properties
                foreach (var deletedProperty in GetPropertyList("Deleted"))
                {
                    ((ProfileUpdaterItemData)ItemData).RemoveProperty(
                        deletedProperty.SourceItemId,
                        deletedProperty.ProviderName,
                        deletedProperty.PropertyName);
                }

                //Add new properties
                foreach (var addedProperty in GetPropertyList("Added"))
                {
                    ((ProfileUpdaterItemData)ItemData).AddProperty(
                        addedProperty.SourceItemId,
                        addedProperty.PropertyName,
                        addedProperty.PropertyName);
                }

                ((ProfileUpdaterItemData)ItemData).Alias = _aliasText.Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int SaveData()
        {
            _currentTabIndex.Text = HidePreview ? "1" : "0";

            return base.SaveData();
        }
    }
}