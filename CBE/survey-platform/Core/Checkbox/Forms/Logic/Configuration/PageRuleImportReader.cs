using System.Collections.Generic;
using System.Xml;
using Checkbox.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class PageRuleImportReader : RuleImportReader
    {
        public List<RuleData> PageRules { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataService"></param>
        public PageRuleImportReader(RuleDataService dataService) : base(dataService)
        {
            PageRules = new List<RuleData>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pageNode"></param>
        public override void ReadRuleData(PersistedDomainObject obj, XmlNode pageNode)
        {
            if(DataService == null)
            {
                return;
            }

            ImportPageBranches(obj.ID.Value, pageNode);
            ImportPageConditions(obj.ID.Value, pageNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="pageNode"></param>
        protected void ImportPageBranches(int pageId, XmlNode pageNode)
        {
            var pageBranchNodes = pageNode.SelectNodes("PageBranches/RuleData");

            foreach(XmlNode pageBranchNode in pageBranchNodes)
            {
                var branchRule = DataService.CreateEmptyBranchRule(pageId);
                branchRule.Load(pageBranchNode);

                PageRules.Add(branchRule);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="pageNode"></param>
        protected void ImportPageConditions(int pageId, XmlNode pageNode)
        {
            var pageConditionNode = pageNode.SelectSingleNode("PageCondition/RuleData");

            if(pageConditionNode != null)
            {
                var conditionRule = DataService.CreateEmptyConditionRuleForPage(pageId);
                conditionRule.Load(pageConditionNode);

                PageRules.Add(conditionRule);
            }
        }

        /// <summary>
        /// Update page rules to update mappings between pre-import page/item/option ids and post-import page/item/option
        /// ids.
        /// </summary>
        public void UpdateRules(Dictionary<int, int> pageIdMap)
        {
            if(DataService == null)
            {
                return;
            }

            DataService.UpdateBranchTargets(PageRules.ToArray(), pageIdMap);
        }
    }
}
