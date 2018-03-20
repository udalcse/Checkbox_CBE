using System;
using System.Xml;
using System.Collections.Generic;
using Checkbox.Security;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item to update profile information
    /// </summary>
    [Serializable]
    public class ProfileUpdater : ResponseItem
    {
        private List<ProfileUpdaterItemData.ProfileUpdaterProperty> _props;

        /// <summary>
        /// Get list of properties
        /// </summary>
        /// <returns></returns>
        public List<ProfileUpdaterItemData.ProfileUpdaterProperty> GetPropertyList()
        {
            return _props;
        }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            _props = new List<ProfileUpdaterItemData.ProfileUpdaterProperty>();
            _props.AddRange(((ProfileUpdaterItemData)configuration).Properties.Values);
        }

        /// <summary>
        /// Perform the item processing
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (Response != null && Response.Respondent != null)
            {
                Dictionary<string, string> respondentProfile = ProfileManager.GetProfile(Response.Respondent.Identity.Name);

                //Create a temp. cache of providers and profiles for this item only
                foreach (ProfileUpdaterItemData.ProfileUpdaterProperty prop in _props)
                {
                    //Find the item in the survey
                    var sourceItem = Response.GetItem(prop.SourceItemId) as IAnswerable;

                    if (sourceItem != null)
                    {
                        respondentProfile[prop.PropertyName] = sourceItem.GetAnswer();
                    }
                }

                ProfileManager.StoreProfile(Response.Respondent.Identity.Name, respondentProfile);
            }
        }

        /// <summary>
        /// Get whether the item is visible, which is only in the editor
        /// </summary>
        public override bool Visible
        {
            get { return (Response != null); }
        }

        /// <summary>
        /// Build up data transfer object
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is SurveyResponseItem)
            {
                //TODO: Properties
                //((SurveyResponseItem) itemDto).AdditionalData = GetPropertyList().ToArray();
            }
        }

        /// <summary>
        /// Write the instance data
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        public override void WriteXmlInstanceData(XmlWriter writer, bool isText)
        {
            base.WriteXmlInstanceData(writer, isText);

            writer.WriteStartElement("properties");

            foreach (ProfileUpdaterItemData.ProfileUpdaterProperty property in _props)
            {
                writer.WriteStartElement("property");
                writer.WriteElementString("name", property.PropertyName);
                writer.WriteElementString("profileProvider", property.ProviderName);
                writer.WriteElementString("sourceItem", property.SourceItemId.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
