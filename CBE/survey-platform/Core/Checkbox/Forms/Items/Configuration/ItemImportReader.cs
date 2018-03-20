using System.Collections.Generic;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Logic.Configuration;
using Prezza.Framework.Common;
using System.Text.RegularExpressions;
using Checkbox.Management;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Class to handle some housecleaning tasks for importing items, including updating rules so that pre-import item/option ids
    /// are mapped to corresponding item/option ids in imported surveys. For standalone mode, passing NULL rule data service to
    /// constructor will prevent rule operations.
    /// </summary>
    public class ItemImportReader
    {
        private ItemRuleImportReader RuleReader { get; set; }

        /// <summary>
        /// Map of item options by position to the option ids.  This is used later to update conditions
        /// on import to map old item/option ids to new item/option ids.  The map key is pre-import option id
        /// and value is position of option.
        /// </summary>
        private Dictionary<int, int> OptionIdMap { get; set; }

        /// <summary>
        /// Map pre-import item ids to post-import item ids
        /// </summary>
        public Dictionary<int, int> ItemIdMap { get; private set; }

        /// <summary>
        /// Map cell item it to it's prototype id for the matrix
        /// </summary>
        private Dictionary<int, int> PrototypesMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rds"></param>
        public ItemImportReader(RuleDataService rds)
        {
            RuleReader = new ItemRuleImportReader(rds);
            OptionIdMap = new Dictionary<int, int>();
            ItemIdMap = new Dictionary<int, int>();
            PrototypesMap = new Dictionary<int, int>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlNode"></param>
        public void ReadItemData(PersistedDomainObject obj, XmlNode xmlNode, object creator)
        {
            var itemId = XmlUtility.GetAttributeInt(xmlNode, "Id");

            if (itemId <= 0)
            {
                return;
            }

            RuleReader.ReadRuleData(obj ,xmlNode);

            //Store mapping between old id and new id
            ItemIdMap[itemId] = obj.ID.Value;

            var optionNodes = xmlNode.SelectNodes("Options/Option");

            //Store mapping between old item ids, old option ids and old option positions
            foreach(XmlNode optionNode in optionNodes)
            {
                var optionId = XmlUtility.GetNodeInt(optionNode.SelectSingleNode("Id")) ?? -1;
                var optionPosition = XmlUtility.GetNodeInt(optionNode.SelectSingleNode("Position")) ?? -1;

                if(optionId > 0 && optionPosition > 0)
                {
                    OptionIdMap[optionId] = optionPosition;
                }
            }

            //Handle matrix case.  This is not the best place to handle this OO-wise, but it's the best place
            // to put a fix with minimum risk to current patch date.
            ReadChildData(obj, xmlNode, creator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlNode"></param>
        private void ReadChildData(PersistedDomainObject obj, XmlNode xmlNode, object creator)
        {
            var matrixObj = obj as MatrixItemData;

            if(matrixObj == null)
            {
                return;
            }

            //Read rows to store row id maps
            ReadChildRowData(matrixObj, xmlNode);

            //Read columns to store row id/option id maps
            ReadChildColumnData(matrixObj, xmlNode, creator);

            //Read cells to store cell item id and their coordinates
            ReadChildCellData(matrixObj, xmlNode);
        }

        /// <summary>
        /// Read child rows
        /// </summary>
        /// <param name="matrixObj"></param>
        /// <param name="xmlNode"></param>
        private void ReadChildRowData(MatrixItemData matrixObj, XmlNode xmlNode)
        {

            //Handle "Other" Rows.  Currently (pre-5.3.2) the IDs of other row items
            // are not included in the export, so it's not possible to make a match.  This additional
            // data was added in 5.3.2, so the match can be made
            for (int row = 1; row <= matrixObj.RowCount; row++)
            {
                if (!matrixObj.IsRowOther(row))
                {
                    continue;
                }

                var rowOtherItem = matrixObj.GetItemAt(row, matrixObj.PrimaryKeyColumnIndex);

                if (rowOtherItem == null)
                {
                    continue;
                }

                //Find the corresponding row and item in xml
                var rowNode = xmlNode.SelectSingleNode(string.Format("Rows/Row[@Index='{0}']", row));

                if (rowNode == null)
                {
                    continue;
                }

                var rowItemId = XmlUtility.GetAttributeInt(rowNode, "RowItemId");

                //Ok, we had a match, now store the id
                if (rowItemId > 0)
                {
                    ItemIdMap[rowItemId] = rowOtherItem.ID.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrixObj"></param>
        /// <param name="xmlNode"></param>
        private void ReadChildColumnData(MatrixItemData matrixObj, XmlNode xmlNode, object creator)
        {
            //For each column prototype, match items/options
            for (int column = 1; column <= matrixObj.ColumnCount; column++)
            {
                //Do nothing for column with row texts/other option.  Those are handled in
                // ReadChildRowData
                if (column == matrixObj.PrimaryKeyColumnIndex)
                {
                    continue;
                }

                //Two parts to this.  First to store/match ids for column prototypes and options and second
                // to match
                if (matrixObj.GetColumnPrototypeId(column) <= 0)
                {
                    continue;
                }

                var columnPrototype =
                    ItemConfigurationManager.GetConfigurationData(matrixObj.GetColumnPrototypeId(column));

                if (columnPrototype == null)
                {
                    continue;
                }
                
                var protoNode = xmlNode.SelectSingleNode(string.Format("Columns/Column[@Index='{0}']/Item", column));

                if (protoNode == null)
                {
                    continue;
                }

                //Read column prototype data
                ReadItemData(columnPrototype, protoNode, creator);

                //Now get itemid mappings for children of he matrix
                for(int row = 1; row <= matrixObj.RowCount; row++)
                {
                    var childItemId = matrixObj.GetItemIdAt(row, column);
                    var childItemNode = xmlNode.SelectSingleNode(string.Format("ChildItems/Item[@Row='{0}' and @Column='{1}']", row, column));

                    if (!childItemId.HasValue || childItemId <= 0 || childItemNode == null)
                    {
                        continue;
                    }

                    var childItemXmlValue = XmlUtility.GetAttributeInt(childItemNode, "Id");

                    if (childItemXmlValue <= 0)
                    {
                        continue;
                    }

                    //Store mapping
                    ItemIdMap[childItemXmlValue] = childItemId.Value;

                    //Store prototype mapping
                    PrototypesMap[childItemId.Value] = columnPrototype.ID.Value;
                }
            }
        }

        /// <summary>
        /// Reads child cells and saves tem ID and it's coordinates to the dictionary
        /// </summary>
        /// <param name="matrixObj"></param>
        /// <param name="xmlNode"></param>
        private void ReadChildCellData(MatrixItemData matrixObj, XmlNode xmlNode)
        {
            var nodes = xmlNode.SelectNodes("ChildItems/Item");
            foreach (XmlNode childNode in nodes)
            {
                if (childNode.Attributes["Id"] == null)
                    continue;
                int ID = int.Parse(childNode.Attributes["Id"].Value);
                int column = int.Parse(childNode.Attributes["Column"].Value);
                int row = int.Parse(childNode.Attributes["Row"].Value);

                if (!string.IsNullOrEmpty(matrixObj.BindedFieldName))
                    continue;

                ItemData childItemData = matrixObj.GetItemAt(row, column);
                if (childItemData == null || !childItemData.ID.HasValue)
                    continue;

                ItemIdMap[ID] = childItemData.ID.Value;
            }
        }

        /// <summary>
        /// Update page rules to update mappings between pre-import page/item/option ids and post-import page/item/option
        /// ids.
        /// </summary>
        public void UpdateRules(RuleData[] pageRules)
        {
            RuleReader.UpdateRules(pageRules, ItemIdMap, OptionIdMap, PrototypesMap);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="itemID"></param>
        public delegate void AddResponsePipeDelegate(string name, int itemID);

        private AddResponsePipeDelegate _createResponsePipeCallback;

        /// <summary>
        /// Updates all items' pipes
        /// </summary>
        public void UpdatePipes(AddResponsePipeDelegate callback)
        {
            _createResponsePipeCallback = callback;
            foreach (int key in ItemIdMap.Keys)
            {
                ItemData itemData = ItemConfigurationManager.GetConfigurationData(ItemIdMap[key]);
                itemData.UpdatePipes(updatePipes);
                itemData.Save();
            }
        }

        /// <summary>
        /// Resolves dependencies 
        /// </summary>
        public void ResolveIdDependencies()
        {
            foreach (int key in ItemIdMap.Keys)
            {
                var itemData = ItemConfigurationManager.GetConfigurationData(ItemIdMap[key]);
                itemData.ResolveIdDependencies(ItemIdMap);
                itemData.Save();
            }
        }

        private Regex _regExp;
        /// <summary>
        /// Regexp for the pipes tokens
        /// </summary>
        protected Regex RegExp
        {
            get 
            { 
                if (_regExp == null)
                    _regExp = new Regex(ApplicationManager.AppSettings.PipePrefix + @"\w*\b", RegexOptions.IgnoreCase);
                return _regExp; 
            }
        }

        /// <summary>
        /// Callback that replaces old item ID's with new ones
        /// </summary>
        /// <param name="pipedText">Old piped text</param>
        /// <returns>New piped text</returns>
        private bool updatePipes(ref string text)
        {
            //Parse the text for tokens
            Match m = RegExp.Match(text);

            string modified = text;

            bool hasReplacedPipes = false;

            while (m.Success)
            {
                string key = m.Value;
                int oldItemID = 0;
                if (int.TryParse(key.Replace(ApplicationManager.AppSettings.PipePrefix, ""), out oldItemID))
                {
                    if (ItemIdMap.ContainsKey(oldItemID))
                    {
                        string newKey = string.Format("{0}{1:000000}", ApplicationManager.AppSettings.PipePrefix, ItemIdMap[oldItemID]);
                        modified = modified.Replace(key, newKey);
                        hasReplacedPipes = true;
                        if (_createResponsePipeCallback != null)
                        {
                            _createResponsePipeCallback(newKey, ItemIdMap[oldItemID]);
                        }
                    }
                }

                m = m.NextMatch();
            }

            text = modified;
            return hasReplacedPipes;
        } 
        
    }
}
