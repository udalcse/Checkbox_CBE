using System.Xml;
using Checkbox.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class ItemRuleWriter : RuleExportWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataService"></param>
        public ItemRuleWriter(RuleDataService dataService) : base(dataService)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="writer"></param>
        public override void WriteRuleData(PersistedDomainObject obj, XmlWriter writer)
        {
            var itemCondition = DataService.GetConditionForItem(obj.ID.Value);

            writer.WriteStartElement("ItemCondition");

            if (itemCondition != null)
            {
                itemCondition.WriteXml(writer);
            }

            writer.WriteEndElement(); // ItemCondition
        }
    }
}
