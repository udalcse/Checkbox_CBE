////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
// This file and its contents are protected by United States and 
// International copyright laws. Unauthorized reproduction and/or 
// distribution of all or any portion of the code contained herein 
// is strictly prohibited and will result in severe civil and criminal 
// penalties. Any violations of this copyright will be prosecuted 
// to the fullest extent possible under law. 
// 
// THE SOURCE CODE CONTAINED HEREIN AND IN RELATED FILES IS PROVIDED 
// TO THE REGISTERED DEVELOPER FOR THE PURPOSES OF EDUCATION AND 
// TROUBLESHOOTING. UNDER NO CIRCUMSTANCES MAY ANY PORTION OF THE SOURCE
// CODE BE DISTRIBUTED, DISCLOSED OR OTHERWISE MADE AVAILABLE TO ANY 
// THIRD PARTY WITHOUT THE EXPRESS WRITTEN CONSENT OF XHEO. 
// 
// UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN 
// PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR 
// SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY XHEO PRODUCT. 
// 
// THE REGISTERED DEVELOPER ACKNOWLEDGES THAT THIS SOURCE CODE 
// CONTAINS VALUABLE AND PROPRIETARY TRADE SECRETS OF XHEO, THE 
// REGISTERED DEVELOPER AGREES TO EXPEND EVERY EFFORT TO INSURE ITS 
// CONFIDENTIALITY. 
// 
// THE END USER LICENSE AGREEMENT (EULA) ACCOMPANYING THE PRODUCT 
// PERMITS THE REGISTERED DEVELOPER TO REDISTRIBUTE THE PRODUCT IN 
// EXECUTABLE FORM ONLY IN SUPPORT OF APPLICATIONS WRITTEN USING 
// THE PRODUCT. IT DOES NOT PROVIDE ANY RIGHTS REGARDING THE 
// SOURCE CODE CONTAINED HEREIN. 
// 
// THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE. 
#endregion
////////////////////////////////////////////////////////////////////////////////
//
// Class:		BrandedLimit
// Author:		Paul Alexander
// Created:		Friday, September 13, 2002 5:25:39 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Security.Cryptography;

namespace Xheo.Licensing
{
	/// <summary>
	/// Introduces branded HTML into the output stream. The component being licensed
	/// must derive from System.Web.UI.Control so that the limit can gain access
	/// to the page hierarchy.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class BrandedLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string	_html				= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the BrandedLimit class.
		/// </summary>
		public BrandedLimit()
		{
			
		}
		
		/// <summary>
		///	Initializes a new instance of the BrandedLimit class.
		/// </summary>
		/// <param name="html">
		///		The HTML to include in the output stream.
		/// </param>
		public BrandedLimit( string html )
		{
			Html = html;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the HTML to include in the output stream.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The HTML to include in the output stream." )
		]
#endif
		public string Html
		{
			get
			{
				return _html;
			}
			set
			{
				AssertNotReadOnly();
				_html = value;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MBRL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Branded";
			}
		}

		/// <summary>
		/// Gets a value that indicates if the limit uses a GUI to enforce the limit
		/// or communicate with the user.
		/// </summary>
		public override bool IsGui
		{
			get
			{
				return true;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Validates that the license being granted is within the limits enforced by
		/// this object.
		/// </summary>
		/// <param name="context">
		///		The LicenseContext passed to <see cref="LicenseProvider.GetLicense"/>.
		/// </param>
		/// <param name="type">
		///		The Type of the object being licensed.
		/// </param>
		/// <param name="instance">
		///		The instance of the object being licensed.
		/// </param>
		/// <returns>
		///		Returns true if the limits are valid, otherwise false.
		/// </returns>
		public override bool Validate( LicenseContext context, Type type, object instance )
		{
			return true;
		}

		/// <summary>
		/// Inserts the HTML into the page.
		/// </summary>
		protected virtual void InsertHtml( object sender, EventArgs e )
		{
			Control control = sender as Control;
			control.Page.ClientScript.RegisterStartupScript(GetType(),  "LimitHtml", Html );
		}

		///<summary>
		///Summary of Granted.
		///</summary>
		///<param name="context"></param>
		///<param name="type"></param>
		///<param name="instance"></param>
		public override void Granted( LicenseContext context, Type type, object instance )
		{
			if( ExtendedLicense.IsWebRequest )
			{
				Page control = HttpContext.Current.Handler as Page;
				if( control == null )
				{
					if( instance is Page )
						control = instance as Page;
					else if( instance is System.Web.UI.Control )
						control = ((System.Web.UI.Control)control).Page;
				}
				if( control != null )
					control.PreRender += new EventHandler( InsertHtml );
			}
		}

		/// <summary>
		/// Loads the contents of a Limit from an XML string.
		/// </summary>
		/// <param name="node">
		///		The XML string to load from.
		/// </param>
		/// <returns>
		///		Returns true if the load was successful, otherwise false.
		/// </returns>
		public override bool LoadFromXml( XmlNode node )
		{
			if( String.Compare( node.Attributes[ "type" ].Value, "Branded", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( License == null || License.Version >= ExtendedLicense.v2_0 )
			{
				if( node.Attributes[ "html" ] != null )
					Html = node.Attributes[ "html" ].Value;
			}
			else
			{
				if( node.Attributes[ "html" ] == null )
					return false;

				Html = node.Attributes[ "html" ].Value.Trim();

				if( Html == null || Html.Length == 0 )
					return false;
			}

			return true;
		}

		/// <summary>
		/// Saves the contents of the Limit as an XML string.
		/// </summary>
		/// <returns>
		///		Returns the saved XML.
		/// </returns>
		public override XmlNode SaveToXml( XmlNode parent )
		{
			XmlNode limitNode = parent.OwnerDocument.CreateElement( "Limit" );
			XmlAttribute attribute = parent.OwnerDocument.CreateAttribute( "type" );
			attribute.Value = "Branded";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "html" );
			attribute.Value = Html;
			limitNode.Attributes.Append( attribute );

			parent.AppendChild( limitNode );			
			return limitNode;
		}

		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			return Html;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class BrandedLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
