//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Collections.Specialized;

namespace Prezza.Framework.Common
{
	/// <summary>
	/// Simple routines to get Xml data
	/// </summary>
	public static class XmlUtility
	{
		/// <summary>
		/// Get the value of a node as an enumerated type.
		/// </summary>
		/// <param name="node">Node to get the enumerated type data from.</param>
		/// <param name="enumType">Enumerated type for the return data.</param>
		/// <param name="required">Indicates whether this node is required to exist and contain a value.</param>
		/// <returns>Enumtype with the value contained in the node text.</returns>
		public static object GetNodeEnum(XmlNode node, Type enumType, bool required)
		{
			string stringVal = GetNodeText(node, required);

			return Enum.Parse(enumType, stringVal, true);
		}

		/// <summary>
		/// Get the text of a node as an enumerated type.
		/// </summary>
		/// <param name="node">Node to get the enumerated type data from.</param>
		/// <param name="enumType">Enumerated type for the return data.</param>
		/// <returns>Enumtype with the value contained in the node text.</returns>
		public static object GetNodeEnum(XmlNode node, Type enumType)
		{
			return GetNodeEnum(node, enumType, false);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static DateTime? GetNodeDate(XmlNode node, bool required = false)
        {
            var stringVal = GetNodeText(node, required);

            return GetDate(stringVal);
        }

        /// <summary>
        /// Get a date value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultures">Explicit list of cultures to use for conversion.  
        /// If none specified, us and euro culture will be used.</param>        
        /// <returns></returns>
        private static DateTime? GetDate(string value, params CultureInfo[] cultures)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            List<CultureInfo> cultureList = cultures.Length == 0 ? GetCultures() : new List<CultureInfo>(cultures);

            foreach (CultureInfo culture in cultureList)
            {
                DateTime dummyDate;

                if (DateTime.TryParse(value, culture, DateTimeStyles.AllowWhiteSpaces, out dummyDate))
                {
                    return dummyDate;
                }
            }

            return null;
        }


        /// <summary>
        /// Get cultures for conversions
        /// </summary>
        /// <returns></returns>
        private static List<CultureInfo> GetCultures()
        {
            //Attempt to use current thread culture by default.  This can be changed via machine configuration
            // or via the globalization element in the system.web portion of the web.config.
            var list = new List<CultureInfo> { CultureInfo.CurrentCulture, GetUsCulture(), GetRotwCulture() };

            //French grouping separator is a non-breaking space, ascii 160, which
            // is not possible to enter in UTF-8, so we'll add a special french
            // culture clone that looks for a space, ascii 32, as the grouping separator
            var customFrench = new CultureInfo("fr-FR") { NumberFormat = { NumberGroupSeparator = " " } };

            list.Add(customFrench);

            return list;
        }

        /// <summary>
        /// Get culture for us date/number
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetUsCulture()
        {
            return new CultureInfo("en-US");
        }

        /// <summary>
        /// Get culture for rest of world date/culture
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetRotwCulture()
        {
            return new CultureInfo("fr-FR");
        }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="node"></param>
	    /// <param name="required"></param>
	    /// <returns></returns>
	    public static double? GetNodeDouble(XmlNode node, bool required = false)
        {
            string stringVal = GetNodeText(node, required);

            if (stringVal != null)
            {
                if (stringVal != string.Empty)
                {
                    return double.Parse(stringVal, CultureInfo.InvariantCulture);
                }
            }

            return null;
        }


	    /// <summary>
	    /// Get the value of a node as an integer.
	    /// </summary>
	    /// <param name="node">Node to get value of.</param>
	    /// <param name="defaultValue"></param>
	    /// <returns>Integer value of the node data.</returns>
	    public static int? GetNodeInt(XmlNode node, int? defaultValue = null)
		{
			return GetNodeInt(node, false, defaultValue);
		}

	    ///<summary>
	    /// Get the value of a node as an integer.
	    /// </summary>
	    /// <param name="node">Node to get value of.</param>
	    /// <param name="required">Indicates that the node must not be null and must contain a value.</param>
	    ///<param name="defaultValue"></param>
	    ///<returns>Integer value of the node data.</returns>
	    public static int? GetNodeInt(XmlNode node, bool required, int? defaultValue)
		{
			string stringVal = GetNodeText(node, required);

			if(stringVal != null)
			{
				if(stringVal != string.Empty)
				{
					return int.Parse(stringVal);
				}
			}

		    return defaultValue;
		}

		/// <summary>
		/// Get the value of a node as text.
		/// </summary>
		/// <param name="node">Node to get the value of.</param>
		/// <returns>Value of a node as text.</returns>
		public static string GetNodeText(XmlNode node)
		{
			return GetNodeText(node, false);
		}

		/// <summary>
		/// Get the value of a node as text.
		/// </summary>
		/// <param name="node">Node to get the value of.</param>
		/// <param name="required">Indicates whether the node must not be null and must contain a value.</param>
		/// <returns>Value of the node as text.</returns>
		public static string GetNodeText(XmlNode node, bool required)
		{
		    if(node == null)
			{
				if(required)
				{
					throw new Exception("Unable to get required value from node.  Node was null.");
				}

				return string.Empty;
			}
		    
            return node.InnerText;
		}

	    /// <summary>
        /// Get the value of an Xml node as a boolean.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>True if the text of the attribute is "true" otherwise false is returned.</returns>
        public static bool GetNodeBool(XmlNode node)
        {
            return GetNodeBool(node, false);
        }

        /// <summary>
        /// Get the value of an Xml node as a boolean.
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="required">Indicates whether the node must exist and contain a value.</param>
        /// <returns>True if the node of the attribute is "true" otherwise false is returned.</returns>
        public static bool GetNodeBool(XmlNode node, bool required)
        {
            if (String.Compare(GetNodeText(node, required), "true", true) == 0)
            {
                return true;
            }
            
            return false;
        }

	    /// <summary>
        /// Get node text as csv
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static string GetNodeTextAsCSV(XmlNodeList nodes)
        {
            return GetNodeTextAsCSV(nodes, false);
        }

        /// <summary>
        /// Get node text as csv
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static string GetNodeTextAsCSV(XmlNodeList nodes, bool required)
        {
            string csvText = string.Empty;

            foreach (XmlNode node in nodes)
            {
                string text = GetNodeText(node, required);

                if (text != null && text.Trim() != string.Empty)
                {
                    if (csvText != string.Empty)
                    {
                        csvText = csvText + ",";
                    }

                    csvText = csvText + text;
                }
            }

            return csvText;
        }

		/// <summary>
		/// Get the value of an Xml node attribute as text.
		/// </summary>
		/// <param name="node">Node containing the attribute.</param>
		/// <param name="name">Name of the attribute.</param>
		/// <returns>String containing the value of the attribute.</returns>
		public static string GetAttributeText(XmlNode node, string name)
		{
			return GetAttributeText(node, name, false);
		}

		/// <summary>
		/// Get the value of an Xml node attribute as text.
		/// </summary>
		/// <param name="node">Node containing the attribute.</param>
		/// <param name="name">Name of the attribute.</param>
		/// <param name="required">Indicates whether the node attribute must exist and contain a value.</param>
		/// <returns>String containing the value of the attribute.</returns>
		public static string GetAttributeText(XmlNode node, string name, bool required)
		{
			ArgumentValidation.CheckForEmptyString(name, "name");

			if(node == null)
			{
				if(required)
				{
					throw new Exception("Unable to get value of attribute:  " + name + ".  Node was null.");
				}

				return string.Empty;
			} 
			else 
			{
				XmlAttribute attr = node.Attributes[name];

				if(attr == null)
				{
					if(required)
					{
						throw new Exception("Unable to get value of attribute:  " + name + ".  Node was null.");
					}

					return string.Empty;
				}
				else
				{
					return attr.Value;
				}
			}				
		}

		/// <summary>
		/// Get the value of an Xml node attribute as a boolean.
		/// </summary>
		/// <param name="node">Node containing the attribute.</param>
		/// <param name="name">Name of the attribute.</param>
		/// <returns>True if the text of the attribute is "true" otherwise false is returned.</returns>
		public static bool GetAttributeBool(XmlNode node, string name)
		{
			return GetAttributeBool(node, name, false);
		}

		/// <summary>
		/// Get the value of an Xml node attribute as a boolean.
		/// </summary>
		/// <param name="node">Node containing the attribute.</param>
		/// <param name="name">Name of the attribute.</param>
		/// <param name="required">Indicates whether the node attribute must exist and contain a value.</param>
		/// <returns>True if the text of the attribute is "true" otherwise false is returned.</returns>
		public static bool GetAttributeBool(XmlNode node, string name, bool required)
		{
			if(String.Compare(GetAttributeText(node, name, required), "true", true) == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Get the value of an Xml node attribute as an integer.
		/// </summary>
		/// <param name="node">Node containing the attribute.</param>
		/// <param name="name">Name of the attribute.</param>
		/// <returns>Integer value of the attribute.</returns>
		public static int GetAttributeInt(XmlNode node, string name)
		{
			return GetAttributeInt(node, name, false);
		}

		
		/// <summary>
		/// Get the value of an Xml node attribute as an integer.
		/// </summary>
		/// <param name="node">Node containing the attribute.</param>
		/// <param name="name">Name of the attribute.</param>
		/// <param name="required">Indicates whether the attribute must exist and contain a value.</param>
		/// <returns>Integer value of the attribute.</returns>
		public static int GetAttributeInt(XmlNode node, string name, bool required)
		{
			string stringVal = GetAttributeText(node, name, required);

			if((stringVal == string.Empty) && (!required))
			{
				return 0;
			}
			else
			{
				return int.Parse(GetAttributeText(node, name, required));
			}
		}

		/// <summary>
		/// Get the value of an Xml node attribute as a enumerated type.
		/// </summary>
		/// <param name="node">Node containing the attribute.</param>
		/// <param name="name">Name of the attribute.</param>
		/// <param name="enumType">Enumerated type to return.</param>
		/// <returns>Attribute value as enumeratd type.</returns>
		public static object GetAttributeEnum(XmlNode node, string name, Type enumType)
		{
			return GetAttributeEnum(node, name, enumType, false);
		}

		/// <summary>
		/// Get the value of an Xml node attribute as a enumerated type.
		/// </summary>
		/// <param name="node">Node containing the attribute.</param>
		/// <param name="name">Name of the attribute.</param>
		/// <param name="enumType">Enumerated type to return.</param>
		/// <param name="required">Indicates whether the attribute must exist and contain a value.</param>
		/// <returns>Attribute value as enumeratd type.</returns>
		public static object GetAttributeEnum(XmlNode node, string name, Type enumType, bool required)
		{
			string stringVal = GetAttributeText(node, name, required);

			return Enum.Parse(enumType, stringVal, true);
		}

        /// <summary>
        /// Serialize a name value collection using the specified writer.
        /// </summary>
        /// <param name="writer">Xml writer</param>
        /// <param name="values">Name values collection to serialize.</param>
        /// <param name="includeNullValues">Specify where items with null values will be
        /// written and flagged with an isNull value.</param>
        public static void SerializeNameValueCollection(XmlWriter writer, NameValueCollection values, bool includeNullValues)
        {
            if (values != null && writer != null)
            {
                foreach (string key in values.Keys)
                {
                    writer.WriteStartElement(key);

                    //Add an "isNull" value
                    if (values[key] == null)
                    {
                        writer.WriteAttributeString("isNull", true.ToString());
                    }
                    else
                    {
                        writer.WriteCData(values[key]);
                    }

                    writer.WriteEndElement();
                }
            }
        }
	}
}
