using System.Collections.Generic;
using System.Xml;
using Checkbox.Forms.Logic.Configuration;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Interface for serialization item data that contain child item datas.
    /// </summary>
    public interface ICompositeXmlSerializable
    {
        /// <summary>
        /// Generates an item from its XML presentation. Restores all the rules related to the item.
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="rules"></param>
        /// <param name="itemTable"></param>
        /// <param name="options"></param>
        /// <param name="itemConditions"></param>
        void Load(XmlNode xmlNode, RuleDataService rules, Dictionary<int, ItemData> itemTable, Dictionary<int, ListOptionData> options, List<RuleData> itemConditions, object creator);

        /// <summary>
        /// Converts an item into its XML representation. Includes rules related to the item.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="rules"></param>
        void WriteXml(XmlWriter writer, RuleDataService rules);
    }
}
