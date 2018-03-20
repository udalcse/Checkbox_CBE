//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Specialized;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item to contain a displayable message
    /// </summary>
    [Serializable]
    public class Message : ResponseItem
    {
        private string _text;
        private bool _reportableSection;

        /// <summary>
        /// Get the text of the message to display
        /// </summary>
        public virtual string Text
        {
            get { return GetPipedText("Message", _text); }
        }

        /// <summary>
        /// Get the text of the message to display
        /// </summary>
        public virtual bool ReportableSection
        {
            get { return _reportableSection; }
        }

        /// <summary>
        /// Configure the message item based on the item configuration and language code.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(MessageItemData));

            base.Configure(configuration, languageCode, templateId);

            MessageItemData config = (MessageItemData)configuration;

            _text = GetText(config.TextID);
            _reportableSection = config.ReportableSectionBreak;
        }

        /// <summary>
        /// Build data transfer object by adding text.
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is SurveyResponseItem)
            {
                ((SurveyResponseItem)itemDto).Text = Text;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {
        }


        /// <summary>
        /// Get instance data
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection values = base.GetInstanceDataValuesForSerialization();

            values["Text"] = Text;

            return values;
        }
    }
}
