using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Checkbox.Common;
using Checkbox.Globalization.Text;


namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Implementation of an item formatter that uses XSL transformation
    /// of an item's serialized XML to format the item.
    /// </summary>
    [Serializable]
    public abstract class XsltItemFormatter : IItemFormatter
    {
        #region IItemFormatter Members

        /// <summary>
        /// Format the item
        /// </summary>
        /// <param name="item">Item to format.</param>
        /// <param name="format">Desired format.</param>
        /// <param name="showScores">Show scores</param>
        /// <returns>Item formatted as a string.</returns>
        public string Format(Item item, string format, bool showScores)
        {
            //Serialize the item
            HtmlStrippingStringWriter itemStringWriter = new HtmlStrippingStringWriter();

            //Determine if HTML should be stripped
            bool isText = "text".Equals(format, StringComparison.InvariantCultureIgnoreCase);
            itemStringWriter.StripHtml = isText;
            
            XmlTextWriter itemXmlWriter = new XmlTextWriter(itemStringWriter);

            //No formatting
            itemXmlWriter.Formatting = Formatting.None;

            //Write the item xml
            item.WriteXml(itemXmlWriter, isText);

            itemXmlWriter.Flush();
            itemXmlWriter.Close();

            var itemText = Utilities.CleanInvalidXmlCharacters(itemStringWriter.ToString());

            //Create a text reader 
            XmlTextReader reader = new XmlTextReader(itemText, XmlNodeType.Document, null);

            //Now create the output writer
            StringWriter transformStringWriter = new StringWriter();
            XmlTextWriter transformXmlWriter = new XmlTextWriter(transformStringWriter);

            //Load & run the transform
            XslCompiledTransform transform = LoadTransform(format);
            XsltArgumentList args = GetArgumentList(item);
            args.AddParam("enableScoring", string.Empty, showScores.ToString());
            
            transform.Transform(reader, args, transformXmlWriter);

            itemXmlWriter.Flush();
            itemXmlWriter.Close();

            return transformStringWriter.ToString();
        }

        /// <summary>
        /// Get any arguments to pass to the transform, such as localized text, etc.
        /// </summary>
        /// <param name="item">f</param>
        /// <returns>List of xsl arguments</returns>
        protected virtual XsltArgumentList GetArgumentList(Item item)
        {
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("noAnswerText", string.Empty, TextManager.GetText("/controlText/xslItemFormatter/noAnswerText", item.LanguageCode, "Not Answered"));

            return args;
        }

        /// <summary>
        /// Load the xsl transform object from disk.  This can be overridden
        /// to load the transform from other locations.
        /// </summary>
        /// <param name="format">Desired item format.</param>
        /// <returns><see cref="XslCompiledTransform"/> object.</returns>
        protected virtual XslCompiledTransform LoadTransform(string format)
        {
            XslCompiledTransform transform = new XslCompiledTransform();

            transform.Load(GetXslFilePath(format));

            return transform;
        }

        /// <summary>
        /// Get the file path location of the xsl transform file.  Returns
        /// an empty string by default.
        /// </summary>
        /// <param name="format">Desired text format.</param>
        /// <returns>File path of xsl tranform to use.</returns>
        protected virtual string GetXslFilePath(string format)
        {
            return string.Empty;
        }

        #endregion
    }
}
