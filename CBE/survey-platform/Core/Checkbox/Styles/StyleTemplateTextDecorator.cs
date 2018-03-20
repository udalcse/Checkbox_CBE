using System;
using System.Xml;
using System.Data;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Globalization.Text;

using Prezza.Framework.Common;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Styles
{
    /// <summary>
    /// Text decorator for configuring multi-language style templates.  Adds support for
    /// multi-language headers/footers, including copying and import/export.
    /// </summary>
    public class StyleTemplateTextDecorator : TextDecorator
    {
        private readonly StyleTemplate _template;

        private string _headerText;
        private string _footerText;

        private bool _headerTextModified;
        private bool _footerTextModified;

        /// <summary>
        /// Constructor for style template.
        /// </summary>
        /// <param name="language">Language to use for operations.</param>
        /// <param name="template">Style template to "decorate" with multi-language text.</param>
        /// <exception cref="NullReferenceException">When template is NULL</exception>
        public StyleTemplateTextDecorator(StyleTemplate template, string language)
            : base(language)
        {
            ArgumentValidation.CheckForNullReference(template, "StyleTemplate");

            _template = template;
        }

        /// <summary>
        /// Get/set the header text
        /// </summary>
        public string HeaderText
        {
            get
            {
                if (Utilities.IsNotNullOrEmpty(_template.HeaderTextID) && !_headerTextModified)
                {
                    return GetText(_template.HeaderTextID);
                }
                
                return _headerText;
            }

            set 
            {
                _headerText = value;
                _headerTextModified = true;
            }
        }

        /// <summary>
        /// Get/set the footer text
        /// </summary>
        public string FooterText
        {
            get
            {
                if (Utilities.IsNotNullOrEmpty(_template.FooterTextID) && !_footerTextModified)
                {
                    return GetText(_template.FooterTextID);
                }
                
                return _footerText;
            }

            set 
            {
                _footerText = value;
                _footerTextModified = true;
            }
        }

        /// <summary>
        /// Save localized text
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            if (Utilities.IsNotNullOrEmpty(_template.HeaderTextID))
            {
                SetText(_template.HeaderTextID, HeaderText);
            }

            if (Utilities.IsNotNullOrEmpty(_template.FooterTextID))
            {
                SetText(_template.FooterTextID, FooterText);
            }
        }

        /// <summary>
        /// Create an XML representation of the style template, including localized headers/footers that is
        /// suitable for export.
        /// </summary>
        /// <returns>Style template XML</returns>
        public XmlDocument GetTemplateXml()
        {
            if (Utilities.IsNotNullOrEmpty(_template.HeaderTextID)
                && Utilities.IsNotNullOrEmpty(_template.FooterTextID))
            {
                Dictionary<string, string> headerTexts = GetAllTexts(_template.HeaderTextID);
                Dictionary<string, string> footerTexts = GetAllTexts(_template.FooterTextID);

                XmlDocument templateXml = _template.ToXml();

                //Find the header/footer place holders
                if (templateXml != null)
                {
                    XmlNode headerNode = templateXml.SelectSingleNode("/CssDocument/Header");
                    XmlNode footerNode = templateXml.SelectSingleNode("/CssDocument/Footer");

                    if (headerNode != null && footerNode != null)
                    {
                        //Append header elements
                        foreach (string key in headerTexts.Keys)
                        {
                            XmlElement textElement = templateXml.CreateElement("headerText");
                            XmlAttribute languageCodeAttr = templateXml.CreateAttribute("languageCode");
                            XmlCDataSection textValueCData = templateXml.CreateCDataSection(headerTexts[key]);

                            languageCodeAttr.Value = key;

                            textElement.AppendChild(textValueCData);
                            textElement.Attributes.SetNamedItem(languageCodeAttr);

                            headerNode.AppendChild(textElement);
                        }

                        //Append footer elements
                        foreach (string key in footerTexts.Keys)
                        {
                            XmlElement textElement = templateXml.CreateElement("footerText");
                            XmlAttribute languageCodeAttr = templateXml.CreateAttribute("languageCode");
                            XmlCDataSection textValueCData = templateXml.CreateCDataSection(footerTexts[key]);

                            languageCodeAttr.Value = key;

                            textElement.AppendChild(textValueCData);
                            textElement.Attributes.SetNamedItem(languageCodeAttr);

                            footerNode.AppendChild(textElement);
                        }
                    }

                    return templateXml;
                }
            }

            return null;
        }


        /// <summary>
        /// Load the style template from the specified XML document, including localized texts
        /// </summary>
        /// <param name="doc">Document containing style template Xml</param>
        /// <param name="currentPrincipal"></param>
        /// <exception cref="NullReferenceException">When xml document is null.</exception>
        public void LoadTemplateFromXml(XmlDocument doc, ExtendedPrincipal currentPrincipal)
        {
            ArgumentValidation.CheckForNullReference(doc, "StyleTemplateXml");
            _template.Load(doc.OuterXml);

            //Now attempt to save the texts
            XmlNodeList headerTextNodes = doc.SelectNodes("/CssDocument/Header/headerText");
            XmlNodeList footerTextNodes = doc.SelectNodes("/CssDocument/Footer/footerText");

            //Store the header texts
            if (Utilities.IsNotNullOrEmpty(_template.HeaderTextID))
            {
                foreach (XmlNode headerTextNode in headerTextNodes)
                {
                    XmlAttribute languageAttr = headerTextNode.Attributes["languageCode"];

                    if (languageAttr != null && languageAttr.Value != string.Empty)
                    {
                        SetText(_template.HeaderTextID, headerTextNode.InnerText, languageAttr.Value);
                    }
                }
            }

            //Store the footer texts
            if (Utilities.IsNotNullOrEmpty(_template.FooterTextID))
            {
                foreach (XmlNode footerTextNode in footerTextNodes)
                {
                    XmlAttribute languageAttr = footerTextNode.Attributes["languageCode"];

                    if (languageAttr != null && languageAttr.Value != string.Empty)
                    {
                        SetText(_template.FooterTextID, footerTextNode.InnerText, languageAttr.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Save the template
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="t"></param>
        public void Save(ExtendedPrincipal currentPrincipal, IDbTransaction t)
        {
            StyleTemplateManager.SaveTemplate(_template, currentPrincipal );
            SetLocalizedTexts();
        }

        /// <summary>
        /// Save the style template
        /// </summary>
        public override void Save()
        {
            throw new Exception("Parameterless save method is not supported for style template.  Use overload instead.");
        }

        /// <summary>
        /// Save the style template.  The transaction context is not used, but the method must be implemented.
        /// </summary>
        /// <param name="t">Transaction context (ignored)</param>
        public override void Save(IDbTransaction t)
        {
            Save();
        }
    }
}
