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
// Class:		NameValueSectionHandler
// Author:		Paul Alexander
// Created:		Thursday, January 16, 2003 9:27:06 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;


#if LICENSING
namespace Xheo.Licensing.Configuration
#else
namespace Xheo.Configuration
#endif
{
	/// <summary>
	/// Implements a section handler that can actually handle multiple entries with 
	/// the same key.
	/// </summary>
	public class NameValueSectionHandler : IConfigurationSectionHandler
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the NameValueSectionHandler class.
		/// </summary>
		public NameValueSectionHandler()
		{
		
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Parses the configuration section and returns a new NameValueCollection.
		/// </summary>
		/// <param name="parent">
		///		A parent configuration section if any.
		/// </param>
		/// <param name="configContext">
		///		An HttpConfigurationContext if called from ASP.NET.
		/// </param>
		/// <param name="section">
		///		The XmlNode representing the configuration section.
		/// </param>
		/// <returns>
		///		Returns a new NameValueCollection.
		/// </returns>
		public object Create( object parent, object configContext, XmlNode section )
		{
			ReadOnlyNameValueCollection collection	= null;
			
			if( parent is ReadOnlyNameValueCollection )
				collection = new ReadOnlyNameValueCollection( parent as ReadOnlyNameValueCollection );
			else
				collection = new ReadOnlyNameValueCollection();

			foreach( XmlNode node in section.ChildNodes )
			{
				switch( node.Name )
				{
					case "add":
						if( node.Attributes[ "key" ] == null )
							throw new ConfigurationErrorsException(
								Internal.StaticResourceProvider.CurrentProvider.GetString( 
									"E_CONFIG_MissingKeyAttribute" ), node );
						if( node.Attributes[ "value" ] == null )
                            throw new ConfigurationErrorsException( 
								Internal.StaticResourceProvider.CurrentProvider.GetString( 
									"E_CONFIG_MissingKeyAttribute" ), node );

						collection.Add( node.Attributes[ "key" ].InnerText, node.Attributes[ "value" ].InnerText );
						break;
					case "remove":
						if( node.Attributes[ "key" ] == null )
                            throw new ConfigurationErrorsException( 
								Internal.StaticResourceProvider.CurrentProvider.GetString( 
									"E_CONFIG_MissingKeyAttribute" ), node );

						collection.Remove( node.Attributes[ "key" ] .InnerText );
						break;
					case "clear":
						collection.Clear();
						break;
				}
			}
			
			collection.SetReadOnly();
			return collection;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		#region ReadOnlyNameValueCollection
		///<summary>
		///Summary of ReadOnlyNameValueCollection
		///</summary>

		private class ReadOnlyNameValueCollection : NameValueCollection
		{
			////////////////////////////////////////////////////////////////////////////////
			#region Fields


			#endregion
			////////////////////////////////////////////////////////////////////////////////

			////////////////////////////////////////////////////////////////////////////////
			#region Constructors/Destructors

		///<summary>
		///Summary of ReadOnlyNameValueCollection.
		///</summary>
			public ReadOnlyNameValueCollection()
			{
			}

		///<summary>
		///Summary of ReadOnlyNameValueCollection.
		///</summary>
		///<param name="collection"></param>
			public ReadOnlyNameValueCollection( ReadOnlyNameValueCollection collection ) : base( collection )
			{
			}

			#endregion
			////////////////////////////////////////////////////////////////////////////////

			////////////////////////////////////////////////////////////////////////////////
			#region Properties


			#endregion
			////////////////////////////////////////////////////////////////////////////////

			////////////////////////////////////////////////////////////////////////////////
			#region Operations

		///<summary>
		///Summary of SetReadOnly.
		///</summary>
			public void SetReadOnly()
			{
				IsReadOnly = true;
			}

			#endregion
			////////////////////////////////////////////////////////////////////////////////

		}
		#endregion
	} // End class NameValueSectionHandler
} // End namespace Xheo.Configuration

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
