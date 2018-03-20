//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.IO;
using System.Globalization;using System.Collections.Specialized;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Format exception messages in text format.
	/// </summary>
	public class TextExceptionFormatter : ExceptionFormatter
	{
		/// <summary>
		/// Text writer to format messages.
		/// </summary>
		private TextWriter writer;

		/// <summary>
		/// Current inner depth to use when calculating the indent of a particular line.
		/// </summary>
		private int innerDepth;

		/// <summary>
		/// String representation of the handling instance Id.
		/// </summary>
		private string handlingInstanceIdString;
    
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="writer">Text writer to write the string to.</param>
		/// <param name="exception">Exception to format the message for.</param>
		/// <param name="handlingInstanceIdString">Exception handling instance Id.</param>
		public TextExceptionFormatter(TextWriter writer, Exception exception, string handlingInstanceIdString) : base(exception)
		{
			this.writer = writer;
			this.handlingInstanceIdString = handlingInstanceIdString;
		}

		/// <summary>
		/// Get the formatter writer.
		/// </summary>
		public TextWriter Writer
		{
			get{return writer;}
		} 

		/// <summary>
		/// Get/Set the current inner depth.
		/// </summary>
		protected virtual int InnerDepth
		{
			get{return innerDepth;}
			set{innerDepth = value;}
		}

		/// <summary>
		/// Write the exception description.
		/// </summary>
		protected override void WriteDescription()
		{
			DateTime localTime = DateTime.Now.ToLocalTime();
			string localTimeString = localTime.ToString("G", DateTimeFormatInfo.InvariantInfo);

			//An exception of type {0} was caught.
			//------------------------------------
			string line = "An exception of type " + base.Exception.GetType().ToString() + " was caught.";
			string separator = new string('-', line.Length);

			Writer.WriteLine(Environment.NewLine);
			Writer.WriteLine(separator);
			Writer.Write("[" + localTimeString + "]");
			Writer.WriteLine(line);
			Writer.Write("[" + localTimeString + "]");
			Writer.WriteLine("Exception ID: " + handlingInstanceIdString);
			Writer.WriteLine(separator);
		}

		/// <summary>
		/// Write the exception information.  Recurse as necessary to walk up the exception chain.
		/// </summary>
		/// <param name="e">Exception to write.</param>
		/// <param name="outerException">Outer exception for the exception to write.</param>
		protected override void WriteException(Exception e, Exception outerException)
		{
			/*if(outerException != null)
			{
				innerDepth++;
				Indent();
				string temp = "InnerException";
				string separator = new string('-', temp.Length);
				Writer.WriteLine(temp);
				Indent();
				Writer.WriteLine(separator);

				base.WriteException(e, outerException);
				this.innerDepth--;
			}
			else
			{
				base.WriteException(e, outerException);
			} */
		}

		/// <summary>
		/// Write the exception time.
		/// </summary>
		/// <param name="utcNow"></param>
		protected override void WriteDateTime(DateTime utcNow)
		{/*
			DateTime localTime = utcNow.ToLocalTime();
			string localTimeString = localTime.ToString("G", DateTimeFormatInfo.InvariantInfo);

			Writer.Write("[" + localTimeString + "]");*/
		}

		/// <summary>
		/// Write the exception type.
		/// </summary>
		/// <param name="exceptionType"></param>
		protected override void WriteExceptionType(Type exceptionType)
		{
			//IndentAndWriteLine("TypeString", exceptionType.AssemblyQualifiedName);
		}

		/// <summary>
		/// Write the exception message.
		/// </summary>
		/// <param name="message"></param>
		protected override void WriteMessage(string message)
		{
			//IndentAndWriteLine("Message", message);
		}

		/// <summary>
		/// Write the exception source.
		/// </summary>
		/// <param name="source"></param>
		protected override void WriteSource(string source)
		{
			//IndentAndWriteLine("Source", source);
		}

		/// <summary>
		/// Write the excetpion help link.
		/// </summary>
		/// <param name="helpLink"></param>
		protected override void WriteHelpLink(string helpLink)
		{
			//IndentAndWriteLine("HelpLink", helpLink);
		}

		/// <summary>
		/// Write the property name and value.
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="propertyValue"></param>
		protected override void WritePropertyInfo(System.Reflection.PropertyInfo propertyInfo, object propertyValue)
		{
			/*Indent();
			Writer.Write(propertyInfo.Name);
			Writer.Write(" : ");
			Writer.WriteLine(propertyValue); */
		}

		/// <summary>
		/// Write the field name and value.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="fieldValue"></param>
		protected override void WriteFieldInfo(System.Reflection.FieldInfo field, object fieldValue)
		{
			/*
			Indent();
			Writer.Write(field.Name);
			Writer.Write(" : ");
			Writer.WriteLine(fieldValue); */
		}

		/// <summary>
		/// Write the stack trace.
		/// </summary>
		/// <param name="stackTrace"></param>
		protected override void WriteStackTrace(string stackTrace)
		{
			/*Indent();
			Writer.Write("StackTrace");
			Writer.Write(" : ");
			
			if(stackTrace == null || stackTrace.Length == 0)
			{
				Writer.WriteLine("Stack Trace is unavailable.");
			}
			else
			{
				string indentation = new string('\t', this.innerDepth);
				string indentedStackTrace = stackTrace.Replace("\n", "\n" + indentation);

				Writer.WriteLine(indentedStackTrace);
				Writer.WriteLine();
			} */
		}

		/// <summary>
		/// Write any name/value pairs stored in the additional info collection.
		/// </summary>
		/// <param name="additionalInfo"></param>
		protected override void WriteAdditionalInfo(NameValueCollection additionalInfo)
		{
			if(additionalInfo.Keys.Count > 0)
			{

				Writer.WriteLine("AdditionalInfo");
				Writer.WriteLine();

				foreach(string name in additionalInfo.AllKeys)
				{
					Writer.Write(name);
					Writer.Write(" : ");
					Writer.Write(additionalInfo[name]);
					Writer.Write(Environment.NewLine);
				}
			}
		}


		/// <summary>
		/// Write an indent according to the inner depth.
		/// </summary>
		protected virtual void Indent()
		{
			for(int i = 0; i < innerDepth; i++)
			{
				Writer.Write("\t");
			}
		}

		/// <summary>
		/// Write an indent and line according to the inner depth.
		/// </summary>
		/// <param name="format">Line to write.</param>
		/// <param name="arg">Arguments to write line call.</param>
		protected virtual void IndentAndWriteLine(string format, params object[] arg)
		{
			Indent();
			Writer.WriteLine(format, arg);
		}
	}
}
