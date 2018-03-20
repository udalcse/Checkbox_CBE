using System.Collections.Generic;
using System.Xml;
using Checkbox.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class ItemRuleImportReader : RuleImportReader
    {
        /// <summary>
        /// 
        /// </summary>
        public List<RuleData> ItemRules { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataService"></param>
        public ItemRuleImportReader(RuleDataService dataService) : base(dataService)
        {
            ItemRules = new List<RuleData>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="itemNode"></param>
        public override void ReadRuleData(PersistedDomainObject obj, XmlNode itemNode)
        {
            if(DataService == null)
            {
                return;
            }

            var itemConditionNode = itemNode.SelectSingleNode("ItemCondition/RuleData");

            if (itemConditionNode != null)
            {
                var conditionRule = DataService.CreateEmptyConditionRuleForItem(obj.ID.Value);
                conditionRule.Load(itemConditionNode);
                ItemRules.Add(conditionRule);
            }
        }

        /// <summary>
        /// Update page rules to update mappings between pre-import page/item/option ids and post-import page/item/option
        /// ids.
        /// </summary>
        public void UpdateRules(RuleData[] pageRules, Dictionary<int, int> itemIdMap, Dictionary<int, int> optionIdMap, Dictionary<int, int> prototypesMap)
        {
            if (DataService == null)
            {
                return;
            }

            if(pageRules != null)
            {
                DataService.UpdateItemAndOptionIds(pageRules, itemIdMap, optionIdMap, prototypesMap);
            }

            DataService.UpdateItemAndOptionIds(ItemRules.ToArray(), itemIdMap, optionIdMap, prototypesMap);
        }
    }
}
