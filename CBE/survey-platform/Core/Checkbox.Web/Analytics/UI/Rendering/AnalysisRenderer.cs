using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Xml;
using System.Data;
using System.Collections;
using System.Reflection;

using Prezza.Framework.Logging;

using Checkbox.Analytics;

namespace Checkbox.Web.Analytics.UI.Rendering
{
	/// <summary>
	/// Renders all controls associated with an Analysis
	/// </summary>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:AnalysisRenderer runat=server></{0}:AnalysisRenderer>")]
	public class AnalysisRenderer : Control, INamingContainer 
	{
		private XmlDocument mAnalysisData;

		/// <summary>
		/// Creates child controls 
		/// </summary>
		protected override void CreateChildControls()
		{
			//Ensure that the XmlDocument has been set
			if (mAnalysisData == null)
				throw new InvalidOperationException("Initilize must be called before any other operation.");

			//Get the root node from the XML document
			XmlElement rootNode = mAnalysisData.DocumentElement;

			//Parse the controls in the XML
			ParseControlsFromXML((XmlNode)rootNode);

		}

		/// <summary>
		/// Passes in the XmlDocument that contains the results of an analysis
		/// </summary>
		/// <param name="analysisData">The analysis results</param>
		public void Initialize(XmlDocument analysisData)
		{
			if (mAnalysisData == null)
				mAnalysisData = new XmlDocument();

			mAnalysisData = analysisData;
		}

		/// <summary>
		/// Parses the XML and adds the contained controls to the control collection
		/// </summary>
		/// <param name="currentNode"></param>
		private void ParseControlsFromXML(XmlNode currentNode)
		{
			Logger.Write("Entering ParseControlsFromXML()", "Debug");
			//Discover the control type of this node
			//If it's a renderer, instantiate it, initialize it,
			//and add it to the control collection of the current control
			if (currentNode.Name == "AnalysisItem")
			{
				Control itemControl = CreateRenderer(currentNode);
				this.Controls.Add(itemControl);

			}
			else
			{
				//If it's an HTML control, add it as a literal control
				//then check to see if it has child controls
				//If it does, call this method recursively on it
				LiteralControl openTag = new LiteralControl();
				openTag.Text = CreateOpenTag(currentNode);
				LiteralControl endTag = new LiteralControl();
				endTag.Text = CreateCloseTag(currentNode);
				this.Controls.Add(openTag);

				if (currentNode.HasChildNodes)
				{
					foreach (XmlNode childNode in currentNode.ChildNodes)
					{
						if (childNode.NodeType == XmlNodeType.Text)
						{
							LiteralControl plainText = new LiteralControl();
							plainText.Text = childNode.InnerText;
							this.Controls.Add(plainText);
						}
						else
						{
							ParseControlsFromXML(childNode);
						}
					}
				}

				//Close the current tag
				this.Controls.Add(endTag);

			}
			Logger.Write("Exiting ParseControlsFromXML()", "Debug");
		}

		/// <summary>
		/// Creates the opening tag from the XmlNode
		/// </summary>
		/// <param name="htmlNode">The node containing the XML to render</param>
		/// <returns>An HTML string</returns>
		private string CreateOpenTag(XmlNode htmlNode)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			
			output.Append(@"<");
			output.Append(htmlNode.Name);
						
			//Add the attributes
			foreach (XmlAttribute attrib in htmlNode.Attributes)
			{
				output.Append(" ");
				output.Append(attrib.Name);
				output.Append("=\"");
				output.Append(attrib.Value);
				output.Append("\"");
			}

			output.Append(@">");

			return output.ToString();

		}

		/// <summary>
		/// Creates the closing HTML for an XML tag
		/// </summary>
		/// <param name="htmlNode">The node containing the XML to render</param>
		/// <returns>An HTML string</returns>
		private string CreateCloseTag(XmlNode htmlNode)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();

			output.Append(@"</");
			output.Append(htmlNode.Name);
			output.Append(@">");

			return output.ToString();
		}

		/// <summary>
		/// Instatiates an appropriate ItemRenderer and calls Inititalize on it
		/// </summary>
		/// <param name="itemNode">The XML describing the results of an AnalysisItem</param>
		/// <returns>An initalized renderer</returns>
		private Control CreateRenderer(XmlNode itemNode)
		{
			Logger.Write("Entering CreateRenderer", "Debug");
			
			//Look up the metadata for this item
			int itemID = Convert.ToInt32(itemNode.Attributes["ItemID"].Value);
			DataTable registrationData = AnalysisManager.GetAnalysisItemRegistration(itemID);
			string className;
			string assemblyName;

			className = registrationData.Rows[0]["RendererClassName"].ToString();
			assemblyName = registrationData.Rows[0]["RendererAssemblyName"].ToString();
			
			//Use reflection to create the renderer
			Assembly assembly = Assembly.Load(assemblyName);
			AnalysisItemRenderer renderer = (AnalysisItemRenderer)Activator.CreateInstance(assembly.GetType(className));

			//Initialize the renderer
			renderer.Initialize(itemNode.FirstChild);
			Logger.Write("Exiting CreateRenderer", "Debug");
			
			return renderer;
		}

	}
}
