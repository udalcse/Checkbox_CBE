using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using Checkbox.Common;

namespace Checkbox
{
	/// <summary>
	/// 
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		/// <param name="value"></param>
		public static void WriteElementValue<T>(this XmlWriter writer, string element, T? value) where T : struct
		{
			if (value.HasValue && value.Value is double)
				((double)(object)value.Value).ToString(NumberFormatInfo.InvariantInfo);

			writer.WriteElementString(element, value.HasValue ? value.Value.ToString() : string.Empty);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		/// <param name="value"></param>
		public static void WriteHtml(this XmlWriter writer, string element, string value)
		{
			writer.WriteStartElement(element);

			if (!string.IsNullOrEmpty(value))
				writer.WriteCData(value);

			writer.WriteEndElement();
		}

		public static string ReadHtml(this XmlReader reader, string element)
		{
			if (reader.NodeType != XmlNodeType.Element || reader.Name != element)
			{
				if (!reader.MoveToNextElement(element))
				{
					return null;
				}
			}

			if (!reader.Read())
			{
				throw new InvalidOperationException("Can not read xml (" + element + "). XML node is empty");
			}

			if (reader.IsEmptyElement || reader.NodeType == XmlNodeType.Whitespace)
			{
				return null;
			}

			return reader.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public static void WriteNull(this XmlWriter writer)
		{
			writer.WriteStartElement("Null");
			writer.WriteEndElement();
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="reader"></param>
	    /// <param name="element"></param>
	    /// <param name="defaultValue"></param>
	    /// <returns></returns>
	    public static string ReadElementNode(this XmlReader reader, string element, string defaultValue = null)
		{
            if (reader.NodeType != XmlNodeType.Element || reader.Name != element)
            {
                if (!reader.MoveToNextElement(element))
                {
                    return defaultValue;
                }
            }

            if (!reader.Read())
            {
                throw new InvalidOperationException("Can not read xml (" + element + "). XML node is empty");
            }

            if (reader.IsEmptyElement || reader.NodeType == XmlNodeType.Whitespace)
            {
                return defaultValue;
            }

	        return reader.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="element"></param>
		/// <param name="throwException"></param>
		public static bool EnsureElement(this XmlReader reader, string element, bool throwException)
		{
			return reader.EnsureElement(element, false, throwException);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="element"></param>
		/// <param name="throwException"></param>
		public static bool EnsureElement(this XmlReader reader, string element, bool moveNext, bool throwException)
		{
			while (reader.NodeType != XmlNodeType.Element)
				reader.Read();

			if (string.IsNullOrEmpty(element))
				return true;

			if (reader.Name != element)
			{
				if (moveNext)
				{
					reader.Read();

					while (reader.NodeType != XmlNodeType.Element)
						reader.Read();
				}

				bool success = reader.Name == element;

				if (throwException && !success)
					throw new InvalidOperationException("Expecting xml node [" + element + "]. Current node is [" + reader.Name + "].");

				if (success)
					return true;

				return false;
			}

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="attrIndex"></param>
		/// <returns></returns>
		public static int ReadAttributeInt(this XmlReader reader, int attrIndex)
		{
			reader.MoveToAttribute(attrIndex);
			int val = int.Parse(reader.Value);

			return val;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="attrIndex"></param>
		/// <returns></returns>
		public static string ReadAttributeAt(this XmlReader reader, int attrIndex)
		{
			reader.MoveToAttribute(attrIndex);
			return reader.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static int? ReadElementNodeInt(this XmlReader reader, string element)
		{
			return Utilities.AsInt(reader.ReadElementNode(element, string.Empty));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static double? ReadElementNodeDouble(this XmlReader reader, string element)
		{
		    return Utilities.AsDouble(reader.ReadElementNode(element, string.Empty));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static DateTime? ReadElementNodeDate(this XmlReader reader, string element)
		{
		    return Utilities.GetDate(reader.ReadElementNode(element, string.Empty));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static bool ReadElementNodeBool(this XmlReader reader, string element)
		{
		    return Utilities.AsBool(reader.ReadElementNode(element, string.Empty), false);
		}

		/// <summary>
		/// Advances XmlTextReader to next element with given name
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="element"></param>
		/// <returns></returns>
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
