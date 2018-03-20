using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Logic.Configuration;

namespace Checkbox.Forms
{
    public delegate ItemData GetItemDelegate(int itemId);

    /// <summary>
    /// 
    /// </summary>
    public class TemplatePageExportWriter : RuleExportWriter
    {
        private PageRuleWriter PageRuleWriter { get; set; }
        private ItemRuleWriter ItemRuleWriter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private GetItemDelegate GetItemDelegate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataService"></param>
        public TemplatePageExportWriter(RuleDataService dataService, GetItemDelegate getItemDelegate) : base(dataService)
        {
            if (dataService != null)
            {
                PageRuleWriter = new PageRuleWriter(dataService);
                ItemRuleWriter = new ItemRuleWriter(dataService);
            }

            GetItemDelegate = getItemDelegate;
        }

        /// <summary>
        /// Write page data
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="writer"></param>
        public void WritePageData(PersistedDomainObject obj, XmlWriter writer)
        {
            var page = obj as TemplatePage;

            if(obj == null)
            {
                return;
            }

            //Rules
            WriteRuleData(obj, writer);

            //Items
            int[] pageItemIds = page.ListItemIds();
            writer.WriteStartElement("PageItems");
            writer.WriteAttributeString("Count", pageItemIds.Length.ToString());

            var itemRuleDelegate = ItemRuleWriter != null ? ItemRuleWriter.WriteRuleData : (PersistedDomainObject.WriteExternalDataCallback)null;

            if (GetItemDelegate != null)
            {
                foreach (int itemId in pageItemIds)
                {
                    ItemData item = GetItemDelegate(itemId);

                    if(item == null)
                    {
                        continue;
                    }

                    item.WriteXml(writer, itemRuleDelegate);
                }
            }

            writer.WriteEndElement(); // PageItems
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="writer"></param>
        public override void WriteRuleData(PersistedDomainObject obj, XmlWriter writer)
        {
            if(PageRuleWriter != null)
            {
                PageRuleWriter.WriteRuleData(obj, writer);
            }
        }
    }
}
