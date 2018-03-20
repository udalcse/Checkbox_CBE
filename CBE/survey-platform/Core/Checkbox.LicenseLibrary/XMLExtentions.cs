using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Checkbox.LicenseLibrary
{
    /// <summary>
    /// Taken from Checkbox.dll
    /// </summary>
    public static class XMLExtentions
    {
        public static bool MoveToNextElement(this XmlReader reader, string element)
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
    }
}
