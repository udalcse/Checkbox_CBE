using System.Collections.Generic;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Logic.Configuration;
using Prezza.Framework.Common;
using Checkbox.Security.Principal;

namespace Checkbox.Forms
{
    /// <summary>
    /// Class to handle some housecleaning tasks for importing pages, including importing items and updating rules so that pre-import page ids
    /// are mapped to corresponding page ids in imported surveys. For standalone mode, passing NULL rule data service to
    /// constructor will prevent rule operations.
    /// </summary>
    public class PageImportReader
    {
        private ItemImportReader ItemReader { get; set; }

        private PageRuleImportReader RuleReader { get; set; }

        /// <summary>
        /// Map of pre-import page ids to post-import page ids
        /// </summary>
        private Dictionary<int, int> PageIdMap { get; set; }
        
        /// <summary>
        /// Map of post-import page ids to list of post-import item ids contained in the pages
        /// </summary>
        public Dictionary<int, List<int>> PageItemMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rds"></param>
        public PageImportReader(RuleDataService rds)
        {
            ItemReader = new ItemImportReader(rds);
            RuleReader = new PageRuleImportReader(rds);
            PageIdMap = new Dictionary<int, int>();
            PageItemMap = new Dictionary<int, List<int>>();
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlNode"></param>
        public void ReadPageData(PersistedDomainObject obj, XmlNode xmlNode, object creator)
        {
            var pageId = XmlUtility.GetAttributeInt(xmlNode, "Id");

            if (pageId <= 0)
            {
                return;
            }

            //Initialize page item list
            PageItemMap[obj.ID.Value] = new List<int>();

            //Map old page id to new page id
            PageIdMap[pageId] = obj.ID.Value;

            //Load items
            LoadPageItems(obj, xmlNode, creator as CheckboxPrincipal);

            //Load Rules
            RuleReader.ReadRuleData(obj, xmlNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlNode"></param>
        public void CopyPageData(PersistedDomainObject obj, XmlNode xmlNode, object creator)
        {
            var pageId = XmlUtility.GetAttributeInt(xmlNode, "Id");

            if (pageId <= 0)
            {
                return;
            }

            //Initialize page item list
            PageItemMap[obj.ID.Value] = new List<int>();

            //Map old page id to new page id
            PageIdMap[pageId] = obj.ID.Value;

            //Copy items
            CopyPageItems(obj, xmlNode, creator as CheckboxPrincipal);

            //Load Rules
            RuleReader.ReadRuleData(obj, xmlNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pageNode"></param>
        private void LoadPageItems(PersistedDomainObject obj, XmlNode pageNode, object creator)
        {
            //Load page items
            var pageItemNodes = pageNode.SelectNodes("PageItems/Item");

            foreach (XmlNode pageItemNode in pageItemNodes)
            {
                var itemData = ItemConfigurationManager.ImportItem(creator as CheckboxPrincipal, pageItemNode, ItemReader, ItemReader.ReadItemData);

                if (itemData == null)
                {
                    continue;
                }

                //Add to mapping
                PageItemMap[obj.ID.Value].Add(itemData.ID.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pageNode"></param>
        private void CopyPageItems(PersistedDomainObject obj, XmlNode pageNode, CheckboxPrincipal principal)
        {
            //Load page items
            var pageItemNodes = pageNode.SelectNodes("PageItems/Item");

            foreach (XmlNode pageItemNode in pageItemNodes)
            {
                var itemData = ItemConfigurationManager.CopyItem(principal, pageItemNode, ItemReader.ReadItemData);

                if (itemData == null)
                {
                    continue;
                }

                //Add to mapping
                PageItemMap[obj.ID.Value].Add(itemData.ID.Value);
            }
        }

        /// <summary>
        /// Update page rules to update mappings between pre-import page/item/option ids and post-import page/item/option
        /// ids.
        /// </summary>
        public void UpdateRules()
        {
            RuleReader.UpdateRules(PageIdMap);
            ItemReader.UpdateRules(RuleReader.PageRules.ToArray());
        }

        /// <summary>
        /// Updates pipes by replacing old item ids with new ones
        /// </summary>
        public void UpdatePipes(ItemImportReader.AddResponsePipeDelegate callback)
        {
            ItemReader.UpdatePipes(callback);
        }

        /// <summary>
        /// Resolves dependencies 
        /// </summary>
        public void ResolveIdDependencies()
        {
            ItemReader.ResolveIdDependencies();
        }
    }
}

