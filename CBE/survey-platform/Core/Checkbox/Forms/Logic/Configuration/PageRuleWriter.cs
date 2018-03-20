using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Checkbox.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class PageRuleWriter : RuleExportWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataService"></param>
        public PageRuleWriter(RuleDataService dataService) : base(dataService)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="page"></param>
        /// <param name="writer"></param>
        public override void WriteRuleData(PersistedDomainObject page, XmlWriter writer)
        {
            RuleData[] pageBranches = DataService.GetBranchRulesForPage(page.ID.Value);

            writer.WriteStartElement("PageBranches");
            writer.WriteAttributeString("Count", pageBranches.Length.ToString());

            foreach (RuleData rule in pageBranches)
                rule.WriteXml(writer);

            writer.WriteEndElement(); // PageBranches

            RuleData pageCondition = DataService.GetConditionForPage(page.ID.Value);

            writer.WriteStartElement("PageCondition");

            if (pageCondition != null)
                pageCondition.WriteXml(writer);
            //else writer.WriteNull();

            writer.WriteEndElement(); // PageCondition
        }
    }
}
