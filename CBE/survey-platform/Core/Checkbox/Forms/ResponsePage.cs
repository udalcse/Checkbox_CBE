using System;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Forms.Logic;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms
{
    /// <summary>
    /// Page container for response items
    /// </summary>
    [Serializable]
    public class ResponsePage : Page, IXmlSerializable
    {
        private List<Rule> _rules;
        private bool _exclude;
        private Dictionary<int, List<string>> _validationErrors;

        private List<Item> _items;

        /// <summary>
        /// Internal constructor, for use by the Response
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="position"></param>
        /// <param name="pageType"></param>
        /// <param name="randomize"></param>
        /// <param name="layoutTemplateID"></param>
        internal ResponsePage(Int32 pageID, Int32 position, TemplatePageType pageType, bool randomize, int? layoutTemplateID)
            : base(pageID, position)
        {
            PageType = pageType;
            Randomize = randomize;
            LayoutTemplateId = layoutTemplateID;
        }

        /// <summary>
        /// Get/set page type
        /// </summary>
        public TemplatePageType PageType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private bool Randomize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? LayoutTemplateId { get; private set; }

        /// <summary>
        /// Response associated with the page
        /// </summary>
        internal Response Parent { get; set; }

        /// <summary>
        /// Get the list of items on the page in order of appearance.  If randomization
        /// is on, the list will be randomized.
        /// </summary>
        public override List<Item> Items
        {
            get
            {
                if (_items == null)
                    RebuildItemList();

                return _items;
            }
        }

        /// <summary>
        /// Build the list of items for the page
        /// </summary>
        /// <returns></returns>
        public void RebuildItemList()
        {
            //Build item order
            List<int> orderedItemIDs = Randomize && PageType != TemplatePageType.Completion
                ? RandomizeItems()
                : ItemIDs;

            //Now build the list
            _items = orderedItemIDs.Where(itemID => Parent.ContainsItem(itemID)).Select(itemID => Parent.GetItem(itemID)).Where(theItem => theItem != null && theItem.IsActive).ToList();
        }

        /// <summary>
        /// Randomize the item order
        /// </summary>
        private List<Int32> RandomizeItems()
        {
            //Check the response to see if the item order already has been randomized
            List<int> persistedOrder = Parent.GetPageItemOrder(PageId);

            if (persistedOrder.Count == 0)
            {
                persistedOrder = Utilities.RandomizeList(ItemIDs);

                for (int i = 0; i < persistedOrder.Count; i++)
                {
                    Parent.SavePageItemOrder(PageId, persistedOrder[i], i + 1);
                }
            }

            //If not, randomize it now
            return persistedOrder;
        }

        /// <summary>
        /// Determine if the page is valid
        /// </summary>
        public bool Valid
        {
            get
            {
                return ValidateItems();
            }
        }

        /// <summary>
        /// Validate that items are valid
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateItems()
        {
            ValidationErrors.Clear();

            bool valid = true;

            List<Item> items = Items;

            foreach (Item i in items)
            {
                //Only validate items that are not excluded
                if (!i.Excluded)
                {
                    if (i is ResponseItem)
                    {
                        ((ResponseItem)i).Validate();

                        if (!((ResponseItem)i).Valid)
                        {
                            valid = false;

                            AddValidationErrors(i.ID, ((ResponseItem)i).ValidationErrors);
                        }
                    }
                }
            }

            return valid;
        }

        /// <summary>
        /// Add to the list of validation errors for the page
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="errors"></param>
        protected void AddValidationErrors(int itemId, List<string> errors)
        {
            //Only add errors for items with item numbers
            int? itemNumber = Parent.GetItemNumber(itemId);

            if (itemNumber.HasValue)
            {
                ValidationErrors[itemNumber.Value] = errors;
            }
        }

        /// <summary>
        /// Get page validation errors
        /// </summary>
        public Dictionary<int, List<string>> ValidationErrors
        {
            get { return _validationErrors ?? (_validationErrors = new Dictionary<int, List<string>>()); }
        }

        /// <summary>
        /// Handle on load to run rules and other events
        /// </summary>
        /// <param name="fireEvents">Indicate if the page should fire load events or not.</param>
        internal void OnLoad(bool fireEvents)
        {
            //Run page rules, which will reset excluded flag.  Page will be marked as excluded if
            // specified as excluded due to evaluation of conditions/branching.
            RunRules(RuleEventTrigger.Load);

            if (!Excluded)
            {
                if (fireEvents)
                {
                    //The binding has to be done on a per-page request basis since event handlers are not serializable
                    // and when response is restored from workflow backing store, events won't exist any more.
                    //Bind item load event to page load evet
                    foreach (Item item in Items)
                    {
                        Load += item.Page_Load;
                    }

                    var handler = (EventHandler)Events[EventPageLoad];
                    if (handler != null)
                        handler(this, EventArgs.Empty);
                }

                //Run item rules
                RunItemRules();

                //Consider a page excluded if all of its items are excluded
                bool excluded = Items.Aggregate(true, (current, item) => current && item.Excluded);
                ExcludedNoItems = excluded;
                //Exclude page
                _exclude = excluded;
            }
        }


        /// <summary>
        /// Handle unload
        /// </summary>
        internal void OnUnLoad()
        {
            //If this page is already excluded by conditions which are run during RuleEventTrigger.OnLoad event(not because it doesn't have any visible items) - miss executing the branches. 
            if ((!Excluded) || (Excluded && ExcludedNoItems))
            {
                RunRules(RuleEventTrigger.UnLoad);
                if (!Excluded)
                {
                    var handler = (EventHandler)Events[EventPageUnLoad];
                    if (handler != null)
                        handler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Run rule for items
        /// </summary>
        protected void RunItemRules()
        {
            foreach (Item i in Items)
            {
                i.Excluded = false;
                var responseItem = i as ResponseItem;
                if (responseItem != null)
                {
                    responseItem.RunRules();
                }
            }
        }

        /// <summary>
        /// Run page rules
        /// </summary>
        /// <param name="trigger"></param>
        /// <remarks>Excluded flag is cleared (set to false) here, a side effect of which is that all
        /// items on the page will also have their Excluded flags set.  As a result, page rules should always
        /// be run before item rules.</remarks>
        public void RunRules(RuleEventTrigger trigger)
        {
            //Clear the excluded flag since it may have been set externally by the response during
            // a page branch operation or when determining that all items on the page are excluded
            // after running item rules.  In either case, the flag will be set AFTER running page
            // rules, so clearing the flag shouldn't have impact. on those operations.
            Excluded = false;

            Parent.RulesEngine.RunRules(PageId, Parent, trigger);
        }

        /// <summary>
        /// Gets a flag indicating whether this Page participates in a Response
        /// </summary>
        /// <remarks>
        /// This property is set to false if the Page is conditionally excluded
        /// </remarks>
        public bool Excluded
        {
            get { return _exclude; }
            internal set
            {
                _exclude = value;

                foreach (Item item in Items)
                {
                    //if 'value' is true and the whole page should be excluded, exclude each item
                    //if not and 'value' is false, do not touch items which already been excluded by rules  
                    if (value || !item.Excluded)
                        item.Excluded = value;
                }
            }
        }


        /// <summary>
        /// Gets a flag indicating whether this Page was excluded because of page doesn't have visible items
        /// </summary>
        /// <remarks>
        /// This property is set to true if the Page is excluded because of page doesn't have visible items
        /// </remarks>
        public bool ExcludedNoItems
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a List containing the <see cref="Rule"/>s, if any, for this Page
        /// </summary>
        internal List<Rule> Rules
        {
            get { return _rules ?? (_rules = new List<Rule>()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ItemsLoaded
        {
            get { return Items.Any() || !ItemIDs.Any(); }
        }

        #region Events

        /// <summary>
        /// Gets the <see cref="EventHandlerList"/> for this Page
        /// </summary>
        protected EventHandlerList Events
        {
            get { return EventHandlers ?? (EventHandlers = new EventHandlerList()); }
        }

        private static readonly object EventPageLoad = new object();
        private static readonly object EventPageUnLoad = new object();

        /// <summary>
        /// Page load event. Fire when page is loaded by a response.
        /// </summary>
        public event EventHandler Load
        {
            add { Events.AddHandler(EventPageLoad, value); }
            remove { Events.RemoveHandler(EventPageLoad, value); }
        }

        /// <summary>
        /// Page UnLoad event.  Fired when a page is unloaded (i.e. moving to another page) by the response.
        /// </summary>
        public event EventHandler UnLoad
        {
            add { Events.AddHandler(EventPageUnLoad, value); }
            remove { Events.RemoveHandler(EventPageUnLoad, value); }
        }


        #endregion


        /// <summary>
        /// Get data transfer object for item. This object is suitable for binding to item renderers
        /// and/or to external data processes and removes the need for references to core Checkbox dlls.
        /// </summary>
        /// <returns></returns>
        public SurveyResponsePage GetDataTransferObject()
        {
            var pageDto = new SurveyResponsePage
            {
                PageId = PageId,
                LayoutTemplateId = LayoutTemplateId,
                Position = Position,
                //ValidationErrors  = ValidationErrors -- TODO: Validation errors
                ItemIds = Items.Where(item => !item.Excluded).Select(item => item.ID).ToArray(),
                HasSPC = Parent.RulesEngine.HasSamePageConditions(Items),
                PageType = PageType.ToString()
            };

            return pageDto;
        }
        
        #region IXmlSerializable Members

        /// <summary>
        /// Return NULL per MSDN documentation
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Load the page data from XML
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write the page data to XML
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            //Write page attributes
            writer.WriteStartElement("page");
            writer.WriteAttributeString("pageId", PageId.ToString());
            writer.WriteAttributeString("position", Position.ToString());
            writer.WriteAttributeString("isExcluded", Excluded.ToString());
            writer.WriteAttributeString("layoutTemplateId", LayoutTemplateId.ToString());

            //Serialize items
            List<Item> items = Items;

            foreach (Item item in items)
            {
                item.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
