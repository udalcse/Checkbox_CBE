//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Globalization;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Formatter for Xml-formatted log messages.
	/// </summary>
	public class XmlExceptionFormatter : ExceptionFormatter
	{
		/// <summary>
		/// Writer to generate Xml string.
		/// </summary>
		private XmlWriter xmlWriter;

		/// <summary>
		/// Exception handling instance Id.
		/// </summary>
		private string handlingInstanceIdString;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="xmlWriter">Writer used to generate Xml string.</param>
		/// <param name="exception">Exception to handle.</param>
		/// <param name="handlingInstanceIdString">Exception handling instance Id as a string.</param>
		public XmlExceptionFormatter(XmlWriter xmlWriter, Exception exception, string handlingInstanceIdString) : base(exception)
		{
			this.xmlWriter = xmlWriter;
			this.handlingInstanceIdString = handlingInstanceIdString;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="writer">Writer used to generate Xml string.</param>
		/// <param name="exception">Exception to handle.</param>
		/// <param name="handlingInstanceIdString">Exception handling instance ID in string format.</param>
		public XmlExceptionFormatter(TextWriter writer, Exception exception, string handlingInstanceIdString) : base(exception)
		{
			XmlTextWriter textWriter = new XmlTextWriter(writer);
			textWriter.Formatting = Formatting.Indented;
			xmlWriter = textWriter;
			this.handlingInstanceIdString = handlingInstanceIdString;
		}

		/// <summary>
		/// Get the Xml writer.
		/// </summary>
		public XmlWriter Writer
		{
			get{return xmlWriter;}
		}

		/// <summary>
		/// Format the xml exception message.
		/// </summary>
		public override void Format()
		{
			Writer.WriteStartElement("Exception");
			base.Format();
			Writer.WriteEndElement();
		}

		/// <summary>
		/// Write the exception date and time.
		/// </summary>
		/// <param name="utcNow"></param>
		protected override void WriteDateTime(DateTime utcNow)
		{
			DateTime localTime = utcNow.ToLocalTime();
			string localTimeString = localTime.ToString("u", DateTimeFormatInfo.InvariantInfo);
			WriteSingleElement("DateTime", localTimeString);
		}

		/// <summary>
		/// Write the exception message.
		/// </summary>
		/// <param name="message"></param>
		protected override void WriteMessage(string message)
		{
			WriteSingleElement("Message", message);
		}

		/// <summary>
		/// Write the exception description.
		/// </summary>
		protected override void WriteDescription()
		{
			WriteSingleElement("Description", "Exception was caught: " + base.Exception.GetType().FullName);
			WriteSingleElement("ExceptionId", handlingInstanceIdString);
		}

		/// <summary>
		/// Write the exception help link.
		/// </summary>
		/// <param name="helpLink"></param>
		protected override void WriteHelpLink(string helpLink)
		{
			WriteSingleElement("HelpLink", helpLink);
		}

		/// <summary>
		/// Write the exception stack trace.
		/// </summary>
		/// <param name="stackTrace"></param>
		protected override void WriteStackTrace(string stackTrace)
		{
			WriteSingleElement("StackTrace", stackTrace);
		}

		/// <summary>
		/// Write the exception source.
		/// </summary>
		/// <param name="source"></param>
		protected override void WriteSource(string source)
		{
			WriteSingleElement("Source", source);
		}

		/// <summary>
		/// Write the exception type.
		/// </summary>
		/// <param name="exceptionType"></param>
		protected override void WriteExceptionType(Type exceptionType)
		{
			WriteSingleElement("ExceptionType", exceptionType.AssemblyQualifiedName);
		}

		/// <summary>
		/// Write the exception information.  Recurses as necessary.
		/// </summary>
		/// <param name="e">Exception to write.</param>
		/// <param name="outerException">Exception's outer exception.</param>
		protected override void WriteException(Exception e, Exception outerException)
		{
			if(outerException != null)
			{
				Writer.WriteStartElement("InnerException");
				base.WriteException(e, outerException);
				Writer.WriteEndElement();
			}
			else
			{
				base.WriteException(e, outerException);
			}
		}

		/// <summary>
		/// Write the property name and value.
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="propertyValue"></param>
		protected override void WritePropertyInfo(System.Reflection.PropertyInfo propertyInfo, object propertyValue)
		{



            if (propertyInfo.PropertyType == typeof(IDictionary) && propertyValue != null)
            {
                foreach (object key in ((IDictionary)propertyValue).Keys)
                {
                    Writer.WriteStartElement("Property");

                    object tempValue = ((IDictionary)propertyValue)[key];

                    if (tempValue == null)
                    {
                        tempValue = "NULL";
                    }

                    Writer.WriteAttributeString("name", key.ToString());
                    Writer.WriteString(tempValue.ToString());
                    Writer.WriteEndElement();
                }
            }
            else
            {
                Writer.WriteStartElement("Property");

                string propertyValueString = "NULL";

                if (propertyValue != null)
                {
                    propertyValueString = propertyValue.ToString();
                }
                Writer.WriteAttributeString("name", propertyInfo.Name);
                Writer.WriteString(propertyValueString);
                Writer.WriteEndElement();
            }            
		}

		/// <summary>
		/// Write the field name and value.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="fieldValue"></param>
		protected override void WriteFieldInfo(System.Reflection.FieldInfo field, object fieldValue)
		{
            if (field.FieldType == typeof(IDictionary) && fieldValue != null)
            {
                foreach (object key in ((IDictionary)fieldValue).Keys)
                {
                    Writer.WriteStartElement("Field");

                    object tempValue = ((IDictionary)fieldValue)[key];

                    if (tempValue == null)
                    {
                        tempValue = "NULL";
                    }

                    Writer.WriteAttributeString("name", key.ToString());
                    Writer.WriteString(tempValue.ToString());
                    Writer.WriteEndElement();
                }
            }
            else
            {
                Writer.WriteStartElement("Field");

                string fieldValueString = "NULL";

                if (fieldValue != null)
                {
                    fieldValueString = fieldValue.ToString();
                }
                Writer.WriteAttributeString("name", field.Name);
                Writer.WriteString(fieldValueString);
                Writer.WriteEndElement();
            }
		}

		/// <summary>
		/// Write the name/value pairs stored in the additional info collection.
		/// </summary>
		/// <param name="additionalInfo"></param>
		protected override void WriteAdditionalInfo(System.Collections.Specialized.NameValueCollection additionalInfo)
		{
			Writer.WriteStartElement("AdditionalInfo");

			foreach(string name in additionalInfo.AllKeys)
			{
				Writer.WriteStartElement("info");
				Writer.WriteAttributeString("name", name);
				Writer.WriteAttributeString("value", additionalInfo[name]);
				Writer.WriteEndElement();
			}

			Writer.WriteEndElement();
		}

		/// <summary>
		/// Write a single Xml element, consisting of start tag, element text, and end tag.
		/// </summary>
		/// <param name="elementName"></param>
		/// <param name="elementText"></param>
		private void WriteSingleElement(string elementName, string elementText)
		{
			Writer.WriteStartElement(elementName);
			Writer.WriteString(elementText);
			Writer.WriteEndElement();
		}
	}
}
