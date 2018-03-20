using System;
using System.Data;
using System.Xml;
using Checkbox.Globalization.Text;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Class for localizable item data.  Provides basic support needed for import/export, etc.
    /// </summary>
    [Serializable]
    public abstract class LocalizableResponseItemData : ResponseItemData
    {
        /// <summary>
        /// Get a textID for the item
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        protected virtual string GetTextID(string suffix)
        {
            if (ID != null && ID > 0)
            {
                return "/" + TextIdPrefix + "/" + ID + "/" + suffix;
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string[] GetTextIdSuffixes()
        {
            return new string[] {};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteItemTexts(XmlWriter writer)
        {
            writer.WriteStartElement("ItemTexts");
            writer.WriteAttributeString("UseTextIds", "true");

            WriteItemTextValues(writer);

            writer.WriteEndElement(); // ItemTexts
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteItemTextValues(XmlWriter writer)
        {
            var textIdSuffixes = GetTextIdSuffixes();

            foreach (string suffix in textIdSuffixes)
            {
                WriteTextValue(writer, GetTextID(suffix), suffix);  
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="textId"></param>
        /// <param name="textKey"></param>
        protected void WriteTextValue(XmlWriter writer, string textId, string textKey)
        {
            writer.WriteStartElement("Text");

            DataTable table = TextManager.GetTextData(textId);

            foreach (DataRow row in table.Rows)
            {
                writer.WriteStartElement("TextData");
                writer.WriteAttributeString("TextKey", textKey);
                writer.WriteAttributeString("Language", row[1].ToString());
                writer.WriteCData(row[2].ToString());
                writer.WriteEndElement(); // TextData
            }

            writer.WriteEndElement(); // Text
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            WriteItemTexts(writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            Save();

            ReadItemTexts(xmlNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        public void ReadItemTexts(XmlNode xmlNode)
        {
            var itemTextsNode = xmlNode.SelectSingleNode("ItemTexts");
            var textIdSuffixes = GetTextIdSuffixes();

            if(itemTextsNode == null)
            {
                return;
            }

            //Check of text ids are used
            var textIdsUsed = XmlUtility.GetAttributeBool(itemTextsNode, "UseTextIds");

            if(!textIdsUsed)
            {
                ReadItemTextsWithoutTextId(itemTextsNode, textIdSuffixes);
                return;
            }

            //Check for item texts
            foreach (var textIdSuffix in textIdSuffixes)
            {
                var textId = GetTextID(textIdSuffix);
                var textNodes = itemTextsNode.SelectNodes("Text/TextData[@TextKey='" + textIdSuffix + "']");
                foreach(XmlNode textNode in textNodes)
                {
                    LoadTextFromNode(textNode, textId);
                }
            }
        }

        /// <summary>
        /// Early 5.x exports did not have text ids with text data so the order was the determining factor. This attempts to reproduce
        /// that logic
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="args"></param>
        private void ReadItemTextsWithoutTextId(XmlNode xmlNode, params string[] args)
        {
            var textNodes = xmlNode.SelectNodes("Text/TextData");

            for(int i =0; i < args.Length; i++)
            {
                if(textNodes.Count <= i)
                {
                    return;
                }

                LoadTextFromNode(textNodes[i], GetTextID(args[i]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textNode"></param>
        /// <param name="textId"></param>
        protected void LoadTextFromNode(XmlNode textNode, string textId)
        {
            var language = XmlUtility.GetAttributeText(textNode, "Language");

            if (string.IsNullOrEmpty(language))
            {
                language = XmlUtility.GetAttributeText(textNode.FirstChild, "Language");

                if (string.IsNullOrEmpty(language)) 
                    return;
            }

            var textValue = string.Empty;

            //For backwards compatibility, look for value attribute
            if(textNode.Attributes["Value"] != null)
            {
                textValue = textNode.Attributes["Value"].Value;
            }
            else
            {
                textValue = XmlUtility.GetNodeText(textNode);
            }

            TextManager.SetText(textId, language, textValue);
        }
    }
}
