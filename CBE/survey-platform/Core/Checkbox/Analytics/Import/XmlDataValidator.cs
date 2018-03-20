using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Checkbox.Analytics.Import
{
    /// <summary>
    /// Class that performs XML import validation
    /// </summary>
    public static class XmlDataValidator
    {
        /// <summary>
        /// Advances XmlTextReader to next element with given name
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool MoveToNextElement(this XmlTextReader reader, string element)
        {
            if (string.IsNullOrEmpty(element))
            {
                while (reader.Read())
                    if (reader.NodeType == XmlNodeType.Element)
                        return true;

                return false;
            }

            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element)
                    return reader.Name == element;

            return false;
        }

        /// <summary>
        /// Performs XML validation
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="throwExceptions"></param>
        /// <returns></returns>
        public static bool Validate(XmlTextReader reader, bool throwExceptions)
        {
            if (reader.EOF)
            {
                if (throwExceptions)
                    throw new InvalidOperationException("Can not validate empty XML file");
                else
                    return false;
            }

            if (!reader.MoveToNextElement("CheckboxResponseExport"))
            {
                if (throwExceptions)
                    throw new InvalidOperationException("Can not find CheckboxResponseExport node in XML file");
                else
                    return false;
            }

            if (!reader.MoveToNextElement("SurveyId"))
            {
                if (throwExceptions)
                    throw new InvalidOperationException("Can not find Survey Data (SurveyId) in XML file");
                else
                    return false;
            }

            if (!reader.MoveToNextElement("SurveyName"))
            {
                if (throwExceptions)
                    throw new InvalidOperationException("Can not find Survey Data (SurveyName) in XML file");
                else
                    return false;
            }

            if (!reader.MoveToNextElement("Items"))
            {
                if (throwExceptions)
                    throw new InvalidOperationException("Can not find Survey Items in XML file");
                else
                    return false;
            }

            if (!reader.ReadToNextSibling("Responses"))
            {
                if (throwExceptions)
                    throw new InvalidOperationException("Can not find any response in XML file");
                else
                    return false;
            }

            return true;
        }
    }
}
