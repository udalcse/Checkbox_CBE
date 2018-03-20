//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// A LabelledItem supports text and subtext labels as part of the Item's data.
    /// <remarks>
    /// LabelledItem is the common ancestor for most Items that prompt the user through a question or 
    /// imperative or directive.  Labels are typically static and non-interactive.
    /// </remarks>
    /// </summary>
    [Serializable]
    public abstract class LabelledItem : AnswerableItem
    {
        private string _text;
        private string _subText;

        /// <summary>
        /// Get the ID of the text associated with this item.
        /// </summary>
        public virtual string Text
        {
            get { return GetPipedText("Text", _text);}
        }

        /// <summary>
        /// Get the ID of the sub text associated with this item.
        /// </summary>
        public virtual string SubText
        {
            get { return GetPipedText("SubText", _subText); }
        }

        /// <summary>
        /// Get the text for the item, falling back to sub text as necessary
        /// </summary>
        /// <returns></returns>
        public virtual string GetText()
        {
            string text = Text;

            if (Utilities.IsNotNullOrEmpty(text))
            {
                return text;
            }
            
            return SubText;
        }

        /// <summary>
        /// Configure this item with the supplied configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            ArgumentValidation.CheckExpectedType(configuration, typeof(LabelledItemData));
            var config = (LabelledItemData)configuration;

            _text = GetText(config.TextID);
            _subText = GetText(config.SubTextID);
        }

        /// <summary>
        /// Buildup data transfer object.
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is SurveyResponseItem)
            {
                ((SurveyResponseItem)itemDto).Text = Text;
                ((SurveyResponseItem)itemDto).Description = SubText;
            }
        }

        /// <summary>
        /// Write the xml instance data
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        public override void WriteXmlInstanceData(XmlWriter writer, bool isText)
        {
            //Text
            writer.WriteStartElement("texts");

            writer.WriteStartElement("text");
            writer.WriteCData(string.IsNullOrEmpty(Text)? " " : Text);
            writer.WriteEndElement();

            //Subtext
            writer.WriteStartElement("subText");
            writer.WriteCData(SubText);
            writer.WriteEndElement();

            writer.WriteEndElement();
        
            base.WriteXmlInstanceData(writer, isText);

        }
    }
}
