using System;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Forms.Data;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Forms;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Security.Principal;
using System.Web;
using Checkbox.Security;
using Checkbox.Users;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Move/Copy survey page
    /// </summary>
    public partial class CopyItem : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        [QueryParameter("s", IsRequired=true)]
        public int ResponseTemplateId { get; set; }

        [QueryParameter("i", IsRequired=true)]
        public int ItemId { get; set; }

        [QueryParameter("ip")]
        public int? ItemPosition { get; set; }

        [QueryParameter("p")]
        public int? FromPageId { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Get edit language
        /// </summary>
        public string EditLanguage
        {
            get
            {
                return Utilities.IsNotNullOrEmpty(LanguageCode)
                           ? LanguageCode
                           : ResponseTemplate.LanguageSettings.DefaultLanguage;
            }
        }

        //
        public string DefaultAction
        {
            get
            {
                return "m".Equals(Request.QueryString["a"], StringComparison.InvariantCultureIgnoreCase)
                    ? "move"
                    : "copy";
            }
        }

            /// <summary>
        /// Get response template to add items to
        /// </summary>
        public ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null && ResponseTemplateId > 0)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);
                }

                return _responseTemplate;
            }
        }

        /// <summary>
        /// Get position of selected page
        /// </summary>
        private TemplatePage SelectedPage
        {
            get
            {
                int pageId;

                if (int.TryParse(_destinationPageList.SelectedValue, out pageId))
                {
                    return ResponseTemplate.GetPage(pageId);
                }

                return null;
            }
        }
        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += _okButton_Click;

            PopulatePageList();
            PopulateItemList();

            //Set title
            Master.Title = WebTextManager.GetText("/pageText/forms/surveys/copyItem.aspx/moveCopyItem") + " - " +
                            Utilities.StripHtml(
                                SurveyMetaDataProxy.GetItemText(
                                    ItemId,
                                    EditLanguage,
                                    false, 
                                    false), 
                                64);

            Master.ClientCancelClickArgs = "{op: 'moveItem', result: 'cancel'}";

            if (DefaultAction == "move")
            {
                _radMove.Checked = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateItemList()
        {
            _destinationItemList.Items.Clear();

            if (SelectedPage == null)
            {
                //Page not found, show an error
                throw new Exception("Unable to load selected page.");
            }

            //Add "First Item" Option
            _destinationItemList.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/copyItem.aspx/firstItem"), "1"));

            int positionCount = 1;
            string afterItemText = WebTextManager.GetText("/pageText/forms/surveys/copyItem.aspx/afterItem", WebTextManager.GetUserLanguage(), "After item {0} - {1}");

            foreach (int itemId in SelectedPage.ListItemIds())
            {
                ItemData item = ResponseTemplate.GetItem(itemId);
                if (item != null)
                {
                    //  TODO: Use survey edit language for item text
                    string itemText = Utilities.StripHtml(
                        ItemConfigurationManager.GetItemText(item.ID.Value, EditLanguage, null, false, false),
                        64);

                    _destinationItemList.Items.Add(new ListItem(
                        string.Format(afterItemText, positionCount, itemText),
                        (positionCount + 1).ToString()));       //Add 1 to position count since we want to add item "after"

                    positionCount++;
                }
            }

            if (_destinationItemList.Items.Count > 1)
            {
                //Add "Last Item" option
                _destinationItemList.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/lastItem"), positionCount.ToString()));
            }

            //if (_destinationItemList.Items.Count == 0)
            //{
            //    _destinationItemList.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/copyItem.aspx/noItems"), "-1"));
            //}

            //Attempt to set default position, if any
            if (ItemPosition.HasValue
                && SelectedPage.ID == FromPageId
                && _destinationItemList.Items.FindByValue(ItemPosition.ToString()) != null)
            {
                _destinationItemList.SelectedValue = ItemPosition.ToString();
            }
            else
            {
                //Otherwise, choose "last item" by default
                _destinationItemList.SelectedIndex = _destinationItemList.Items.Count - 1;
            }
        }

        /// <summary>
        /// Populate list of pages and their items
        /// </summary>
        private void PopulatePageList()
        {
            _destinationPageList.Items.Clear();

            int[] pageIds = ResponseTemplate.ListTemplatePageIds();
                        

            //Loop through pages, but skip 1st (hidden) and last (completion) pages.
            for (int pageIndex = 1; pageIndex < pageIds.Length - 1; pageIndex++)
            {
                //Hidden items page has position 1, so normal pages actual position is page index minus 1.  i.e "Page 1" is actually second
                // page in survey's pages collection.
                _destinationPageList.Items.Add(new ListItem(
                                                   WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/page") + pageIndex,
                                                   pageIds[pageIndex].ToString()));
            }
            
            //If item is answerable we can`t move/copy it to completion page. For that reason we remove completion page            
            var item = ResponseTemplate.GetItem(ItemId);
            var needCompletionPage = !item.ItemIsIAnswerable;
            if (needCompletionPage)
            {
                _destinationPageList.Items.Add(new ListItem(
                                                   WebTextManager.GetText("/enum/templatePageType/completion"),
                                                   pageIds[pageIds.Length - 1].ToString()));
            }
            

            //Set default page
            if (Page.IsPostBack)
            {
                return;
            }

            //Use specified page, if any, otherwise default to page
            // containing target item
            if (FromPageId.HasValue
                && _destinationPageList.Items.FindByValue(FromPageId.ToString()) != null)
            {
                _destinationPageList.SelectedValue = FromPageId.ToString();
            }
            else
            {
                var itemPage = ResponseTemplate.GetPagePositionForItem(ItemId);

                //Since copy/move isn't allowed with destination of hidden items page,
                // subtract one from item page to determine item index.  Decrement again
                // to account for fact that first page has position 1, but our list
                // of items here is 0 indexed.
                if (itemPage.HasValue)
                {
                    var pageIndex = itemPage.Value - 2;

                    if (pageIndex >= 0 && pageIndex < _destinationPageList.Items.Count)
                    {
                        _destinationPageList.SelectedIndex = pageIndex;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _destinationPageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateItemList();
            Master.HideStatusMessage();

            if (SelectedPage.ID.HasValue)
            {
                int fromPageId =
                    FromPageId ?? ResponseTemplate.GetPageAtPosition(ResponseTemplate.GetPagePositionForItem(ItemId).Value).ID ?? -1;
                if (ResponseTemplate.WillRulesBeChangedIfItemMovesToPage(ItemId, fromPageId, SelectedPage.ID.Value))
                    Master.ShowStatusMessage(WebTextManager.GetText("/pageText/forms/surveys/copyItem.aspx/rulesWillBeChangedWarning"), StatusMessageType.Warning);
            }
        }

        /// <summary>
        /// Handle ok button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _okButton_Click(object sender, System.EventArgs e)
        {
            int position = Int32.Parse(_destinationItemList.SelectedValue);

            if (_radCopy.Checked)
            {
                CopySurveyItem(position);
            }
            else
            {
                MoveSurveyItem(position);
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        private void MoveSurveyItem(int position)
        {
            if (ResponseTemplate == null)
            {
                throw new Exception("Unable to load template [" + ResponseTemplateId + "] to move/copy item");
            }

            
            //Find survey item in page
             var oldPagePosition = ResponseTemplate.GetPagePositionForItem(ItemId);

            TemplatePage oldPage = null;

             if (oldPagePosition.HasValue)
             {
                 oldPage = ResponseTemplate.GetPageAtPosition(oldPagePosition.Value);
             }

            if (oldPage != null 
                && oldPage.ID.HasValue
                && SelectedPage != null
                && SelectedPage.ID.HasValue)
            {
                ResponseTemplate.MoveItemToPage(ItemId, oldPage.ID.Value, SelectedPage.ID.Value, position);
                ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                ResponseTemplate.Save();
                ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplateId);
                //Remove metadata from cache to refresh cache
                SurveyMetaDataProxy.RemoveItemFromCache(ItemId);
            }

            var oldPageId = oldPage != null && oldPage.ID.HasValue ? oldPage.ID.Value : -1;

            Master.CloseDialog("{op: 'moveItem', result: 'ok', sourcePage: " + oldPageId + ", targetPage: '" + SelectedPage.ID + "'}", true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        private void CopySurveyItem(int position)
        {
            if (ResponseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + ResponseTemplateId + "]");
            }

            if (SelectedPage == null || !SelectedPage.ID.HasValue)
            {
                return;
            }

            ItemData item = ResponseTemplate.GetItem(ItemId);
            ItemData copy = ItemConfigurationManager.CopyItem(item, (CheckboxPrincipal)HttpContext.Current.User);

            if (copy == null)
                return;

            if (copy.ID.HasValue && item.ID.HasValue)
            {
                //Check if any existing binded profile properties on item
                var bindedField = ProfileManager.GetPropertiesList()
                    .FirstOrDefault(prop => prop.BindedItemId.Any(bindedItemId => bindedItemId == item.ID));
                //If there is a binded item / copy this state too on new ID
                if (bindedField != null)
                {
                    PropertyBindingManager.AddSurveyItemProfilePropertyMapping((int)copy.ID, bindedField.FieldId);

                    if (bindedField.FieldType == CustomFieldType.RadioButton) {
                        var radioButtonField = ProfileManager.GetRadioButtonField(bindedField.Name, UserManager.GetCurrentPrincipal().UserGuid);
                        var radioOptionAliases = ProfileManager.GetRadioOptionAliases(item.ID.Value, bindedField.Name);
                        radioButtonField.Options.ForEach(o =>
                        {
                            var alias = radioOptionAliases.Where(a => a.Key == o.Name);
                            o.Alias = alias.Any() ? alias.FirstOrDefault().Value : string.Empty;
                        });
                        ProfileManager.AddRadioButtonFieldOptionAlias(copy.ID.Value, radioButtonField.Options);
                    }
                }

                ResponseTemplate.AddItemToPage(SelectedPage.ID.Value, copy.ID.Value, position);
            }
            else
            {
                throw new Exception("Error creating copy of item.");
            }

            ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            ResponseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplateId);

            Master.CloseDialog("{op: 'moveItem', result: 'ok', targetPage:'" + SelectedPage.ID + "'}", true);
        }
    }
}
