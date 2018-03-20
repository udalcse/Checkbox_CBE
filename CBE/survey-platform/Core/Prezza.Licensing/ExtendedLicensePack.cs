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
// Class:		ExtendedLicensePack
// Author:		Paul Alexander
// Created:		Monday, September 16, 2002 3:16:37 AM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Web;
using System.Text;

namespace Xheo.Licensing
{
	#region ExtendedLicensePack
	/// <summary>
	/// Represents a collection of licenses stored in a single .LIC file.
	/// </summary>
	/// <remarks>
	///	Each license pack can serialize to and from an XML license file. Each license
	///	within the pack is signed, but the pack itself is not. Licenses can be added
	///	or removed without affecting the validity of the pack.
	/// </remarks>
	[ Serializable ]
#if LICENSING
	public
#else
	internal
#endif
		class ExtendedLicensePack : CollectionBase, IDisposable, ICloneable
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		internal string	_location		= null;
		private	bool	_saveToShared	= false;
		internal bool	_isEmbedded		= false;
		[ NonSerialized ]
		internal LicenseSigningKey	_publicKey	= null;
		private LicenseValuesCollection _metaValues = new LicenseValuesCollection();

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///  Initializes a new instance of the ExtendedLicensePack class.
		/// </summary>
		public ExtendedLicensePack()
		{
		}

		/// <summary>
		///  Initializes a new instance of the ExtendedLicensePack class from the 
		///  given file.
		/// </summary>
		/// <param name="filename">
		///		Path to the file that contains the license pack.
		/// </param>
		public ExtendedLicensePack( string filename )
		{
			Load( filename );
		}

		/// <summary>
		///  Initializes a new instance of the ExtendedLicensePack class from the 
		///  given file.
		/// </summary>
		/// <param name="filename">
		///		Path to the file that contains the license pack.
		/// </param>
		/// <param name="tryIsolatedStorage">
		///		When true attempts to load a mirrored version of the license from Isolated
		///		Storage first.
		/// </param>
		public ExtendedLicensePack( string filename, bool tryIsolatedStorage )
		{
			Load( filename, tryIsolatedStorage );
		}

		/// <summary>
		/// Disposes of any managed/unmanged resources.
		/// </summary>
		public void Dispose()
		{
			foreach( ExtendedLicense license in this )
				license.Dispose();
			Clear();
			GC.SuppressFinalize( this );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets a value that indicates if the pack was loaded directly from an assembly.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public bool IsEmbedded
		{
			get
			{
				return _isEmbedded;
			}
		}

		/// <summary>
		/// Gets a collection of arbitrary values to store with the license pack. These 
		/// values <i>are not</i> protected and may be changed without affecting the 
		/// validity of the contained licenses. Use for storing volitle information and 
		/// other unsecured information.
		/// </summary>
#if LICENSING
		[ 
		Category( "Unprotected" ),
		Description( "Collection of arbitrary values to store with the license pack. These value ARE NOT protected and may be changed without affecting the validity of the contained licenses." ),
		Editor( typeof( Design.NameValueCollectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) ),
		]
#endif
		public LicenseValuesCollection MetaValues
		{
			get
			{
				return _metaValues;
			}
		}

		/// <summary>
		/// Gets the path to the file where the license pack was originally loaded from.
		/// If not loaded from a file, returns null.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public string Location
		{
			get
			{
				return _location;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if changes to the license pack when validating
		/// should be saved to the shared licenses folder.
		/// </summary>
#if LICENSING
		[ 
		Category( "Unprotected" ),
		Description( "Indicates if changes to the license pack when validating should be saved to the shared licenses folder." ),
		DefaultValue( false ),
		]
#endif
		public bool SaveToShared
		{
			get
			{
				return _saveToShared;
			}
			set
			{
				_saveToShared = value;
			}
		}

		/// <summary>
		/// Gets or sets the ExtendedLicense at the given index.
		/// </summary>
		public ExtendedLicense this[ int index ]
		{
			get
			{
				return List[ index ] as ExtendedLicense;
			}
			set
			{
				List[ index ] = value;
			}
		}

		/// <summary>
		/// Gets or sets the ExtendedLicense with the given serial number.
		/// </summary>
		public ExtendedLicense this[ string serialNumber ]
		{
			get
			{
				return this[ IndexOf( serialNumber ) ];
			}
			set
			{
				if( value.SerialNumber != serialNumber )
					throw new ExtendedLicenseException( "E_InvalidSerialForIndexer" );

				int index = IndexOf( serialNumber );
				if( index == -1 )
					Add( value );
				else
					this[ index ] = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Gets a subset of the current license pack filtered by the given component.
		/// </summary>
		/// <param name="component">
		///		Name of the component to get licenses for.
		/// </param>
		/// <returns>
		///		Returns a new collection with the requested licenses.
		/// </returns>
		public ICollection GetLicensesForComponent( string component )
		{
			ArrayList licenses = new ArrayList();

			foreach( ExtendedLicense license in this )
				if( license.Components.Contains( component ) || license.Components.Contains( "*" ) )
					licenses.Add( license );
				else if( license.Version >= ExtendedLicense.v1_1 && license.Components.Count == 0 )
					licenses.Add( license );

			return licenses;
		}

		/// <summary>
		/// Gets the index of the ExtendedLicense with the given serial number.
		/// </summary>
		/// <param name="serialNumber">
		///		The serial number to look for.
		/// </param>
		/// <returns>
		///		Returns the index of the license if found, otherwise -1.
		/// </returns>
		public int IndexOf( string serialNumber )
		{
			for( int index = 0; index < this.Count; index++ )
				if( this[ index ].SerialNumber == serialNumber || this[ index ].AbsoluteSerialNumber == serialNumber )
					return index;

			return -1;
		}

		/// <summary>
		/// Gets the index of the license in the pack.
		/// </summary>
		/// <param name="license">
		///		The license to look for.
		/// </param>
		/// <returns>
		///		Returns the index of the license if found, otherwise -1.
		/// </returns>
		public int IndexOf( ExtendedLicense license )
		{
			return List.IndexOf( license );
		}

		/// <summary>
		/// Adds the given ExtendedLicense to the collection.
		/// </summary>
		/// <param name="license">
		///		The ExtendedLicense to add.
		/// </param>
		public void Add( ExtendedLicense license )
		{
			List.Add( license );
			license.SetLicensePack( this );
		}

		/// <summary>
		/// Inserts the license at the given index.
		/// </summary>
		/// <param name="index">
		///		The index in the collection to insert the license.
		/// </param>
		/// <param name="license">
		///		The license to be inserted.
		/// </param>
		public void Insert( int index, ExtendedLicense license )
		{
			List.Insert( index, license );
		}


		/// <summary>
		/// Removes the given ExtendedLicense from the collection.
		/// </summary>
		/// <param name="license">
		///		The ExtendedLicense to remove.
		/// </param>
		public void Remove( ExtendedLicense license )
		{
			List.Remove( license );
		}

		/// <summary>
		/// Copies the collection of ExtendedLicense objects into the array at the given index.
		/// </summary>
		/// <param name="array">
		///		The array to copy to.
		/// </param>
		/// <param name="index">
		///		The index in the array to start copying.
		/// </param>
		public void CopyTo( ExtendedLicense[] array, int index )
		{
			List.CopyTo( array, index );
		}

		/// <summary>
		/// Determines if the pack contains the given license.
		/// </summary>
		/// <param name="license">
		///		The license to look for.
		/// </param>
		/// <returns>
		///		Returns true if the license is found, otherwise false.
		/// </returns>
		public bool Contains( ExtendedLicense license )
		{
			return List.Contains( license );
		}

		/// <summary>
		/// Removes the license with the given serial number.
		/// </summary>
		/// <param name="serialNumber">
		///		The serial numbef of the license to remove.
		/// </param>
		public void Remove( string serialNumber )
		{
			int index = IndexOf( serialNumber );
			if( index != -1 )
				this.RemoveAt( index );
		}

		#region Load...
		/// <summary>
		/// Loads the contents of the license pack from a file.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the licenses were saved.
		/// </param>
		/// <param name="tryIsolatedStorage">
		///		When true attempts to load a mirrored version of the license from Isolated
		///		Storage first.
		/// </param>
		public void Load( string filename, bool tryIsolatedStorage )
		{
			if( tryIsolatedStorage && ! filename.StartsWith( "isolated:" ) )
			{
				filename = Path.GetFullPath( filename );
				if( IsfFileExists( filename ) )
				{
					try
					{
						Load( "isolated:" + filename );
						return;
					}
					catch
					{}
				}
			}

			Load( filename );
		}

		/// <summary>
		/// Loads the contents of the license pack from a file.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the licenses were saved.
		/// </param>
		public void Load( string filename )
		{
			try
			{
				if( filename.StartsWith( "isolated:" ) )
				{
					IsolatedStorageFileStream stream = null;
					try
					{
						string isolatedPath = IsfGetFileName( filename.Substring( 9 ) );
						IsolatedStorageFile isf = ExtendedLicensePack.GetIsfStore();
						stream = new IsolatedStorageFileStream( isolatedPath, FileMode.Open, FileAccess.Read, isf );
						Load( stream );
						_location = filename;
					}
					finally
					{
						if( stream != null )
							stream.Close();
					}
				}
				else
				{
					FileStream stream = new FileStream( filename, FileMode.Open, FileAccess.Read );
					try
					{
						Load( stream );
					}
					finally
					{
						stream.Close();
					}
					_location = System.IO.Path.GetFullPath( filename );
				}
			}
			catch( Exception ex )
			{
				throw new ExtendedLicenseException( "E_CouldNotLoadPack", ex, ex.Message );
			}
		}

		/// <summary>
		/// Loads the contents of the license pack from a file.
		/// </summary>
		/// <param name="stream">
		///		The stream to load from.
		/// </param>
		public void Load( Stream stream )
		{
			Clear();
			XmlDocument xmlDoc = new XmlDocument();
			//xmlDoc.PreserveWhitespace = true;
			xmlDoc.Load( stream );

			if( xmlDoc.DocumentElement.Attributes[ "saveToShared" ] != null )
				SaveToShared = XmlConvert.ToBoolean( xmlDoc.DocumentElement.Attributes[ "saveToShared" ].Value );

			foreach( XmlNode licenseNode in xmlDoc.DocumentElement.SelectNodes( "License" ) )
				Add( new ExtendedLicense( licenseNode ) );

			XmlNode metaNode = xmlDoc.DocumentElement.SelectSingleNode( "Meta/Values" );
			if( metaNode != null )
				MetaValues.LoadFromXml( metaNode );
		}
		#endregion

		#region Save(...)
		/// <summary>
		/// Saves the contents of the license pack to a stream.
		/// </summary>
		/// <param name="stream">
		///		The Stream to write the contents of the license pack to.
		/// </param>
		public void Save( Stream stream )
		{
			StreamWriter writer = new StreamWriter( stream, new UTF8Encoding( false ) );
			writer.Write( ToFormattedXmlString() );
			writer.Close();
		}

		/// <summary>
		/// Saves the contents of the license pack to a file.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the licenses should be saved.
		/// </param>
		public void Save( string filename )
		{
			Save( filename, false );
		}

		/// <summary>
		/// Saves the contents of the license pack to the file that it was originally loaded
		/// from.
		/// </summary>
		public void Save()
		{
			Save( Location );
		}

		/// <summary>
		/// Saves the contents of the license pack to a file.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the licenses should be saved.
		/// </param>
		/// <param name="useIsolatedOnFail">
		///		If the save fails because of a permission exception, attempt to save to the
		///		isolated storage location instead.
		/// </param>
		public void Save( string filename, bool useIsolatedOnFail )
		{
			if( filename == null )
			{
				if( Location != null && Location.Length > 0 )
				{
					filename = Location;
				}
				else
				{
					if( Count > 0 && this[ 0 ].SerialNumber != null && this[ 0 ].SerialNumber.Length > 0 )
						filename = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, this[ 0 ].SerialNumber + ".lic" );
					else
						filename = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, ( new Guid() ).ToString( "D", System.Globalization.CultureInfo.InvariantCulture ) + ".lic" );
				}
			}

			if( filename.StartsWith( "isolated:" ) )
			{
				IsolatedStorageFileStream stream = null;
				try
				{
					string isolatedPath = IsfGetFileName( filename.Substring( 9 ) );
					IsolatedStorageFile isf = ExtendedLicensePack.GetIsfStore();
					isf.CreateDirectory( IsfGetDirectoryName( Path.GetDirectoryName( filename.Substring( 9 ) ) ) );
					stream = new IsolatedStorageFileStream( isolatedPath, FileMode.Create, FileAccess.Write, isf );
					Save( stream );
					_location = filename;
				}
				finally
				{
					if( stream != null )
						stream.Close();
				}
			}
			else
			{
				try
				{
					FileStream stream = new FileStream( filename, FileMode.Create, FileAccess.Write );
					try
					{
						Save( stream );
					}
					finally
					{
						stream.Close();
					}
					_location = System.IO.Path.GetFullPath( filename );
				}
				catch
				{
					if( useIsolatedOnFail )
					{
						if( Location != null && Location.Length > 0 && ! Location.StartsWith( "isolated:" ) &&
							File.Exists( Location ) )
							MetaValues[ "OriginalLocation" ] = Location;
						Save( "isolated:" + filename, false );
					}
					else
						throw;
				}
			}
		}

		/// <summary>
		/// Saves the contents of the license pack to the file that it was originally loaded
		/// from.
		/// </summary>
		/// <param name="useIsolatedOnFail">
		///		If the save fails because of a permission exception, attempt to save to the
		///		isolated storage location instead.
		/// </param>
		public void Save( bool useIsolatedOnFail )
		{
			Save( Location, useIsolatedOnFail );
		}

		#endregion

		/// <summary>
		/// Gets the contents of the license pack as an XML string.
		/// </summary>
		/// <returns>
		///		An XML string representation of the license pack.
		/// </returns>
		public string ToXmlString()
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlNode		rootNode = xmlDoc.CreateElement( "Licenses" );

			xmlDoc.PreserveWhitespace = true;

			if( SaveToShared )
			{
				XmlAttribute attribute = xmlDoc.CreateAttribute( "saveToShared" );
				attribute.Value = "true";
				rootNode.Attributes.Append( attribute );
			}
	
			xmlDoc.AppendChild( xmlDoc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );

			foreach( ExtendedLicense license in this )
				license.SaveToXml( rootNode );

			XmlNode metaNode = xmlDoc.CreateElement( "Meta" );
			MetaValues.SaveToXml( metaNode );
			if( metaNode.ChildNodes[ 0 ].HasChildNodes )
				rootNode.AppendChild( metaNode );

			xmlDoc.AppendChild( rootNode );
			return xmlDoc.OuterXml;
		}

		/// <summary>
		/// Gets the contents of the license pack as an friendly XML string.
		/// </summary>
		/// <returns>
		///		An XML string representation of the license pack.
		/// </returns>
		public string ToFormattedXmlString()
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlNode		rootNode = xmlDoc.CreateElement( "Licenses" );

			xmlDoc.PreserveWhitespace = true;
			if( SaveToShared )
			{
				XmlAttribute attribute = xmlDoc.CreateAttribute( "saveToShared" );
				attribute.Value = "true";
				rootNode.Attributes.Append( attribute );
			}

			xmlDoc.AppendChild( xmlDoc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );
			
			foreach( ExtendedLicense license in this )
				license.SaveToXml( rootNode );

			XmlNode metaNode = xmlDoc.CreateElement( "Meta" );
			MetaValues.SaveToXml( metaNode );
			if( metaNode.ChildNodes[ 0 ].HasChildNodes )
				rootNode.AppendChild( metaNode );

			xmlDoc.AppendChild( rootNode );

			MemoryStream memoryStream = new MemoryStream();
			byte[] result;
			try
			{
				XmlTextWriter writer = new XmlTextWriter( memoryStream, new UTF8Encoding( false ) );
				
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 1;
				writer.IndentChar = (char)0x9;
				
				xmlDoc.Save( writer );
				writer.Flush();
				result = memoryStream.ToArray();
			}
			finally
			{
				memoryStream.Close();
			}

			return new UTF8Encoding( false ).GetString( result );
		}

		/// <summary>
		/// Loads the contents of the xml string into the license pack.
		/// </summary>
		/// <param name="xml">
		///		A string containing the XML for the license pack.
		/// </param>
		public void FromXmlString( string xml )
		{
			XmlDocument xmlDoc = new XmlDocument();
			//xmlDoc.PreserveWhitespace = true;			
			xmlDoc.LoadXml( xml );

			if( xmlDoc.DocumentElement.Attributes[ "saveToShared" ] != null )
				SaveToShared = XmlConvert.ToBoolean( xmlDoc.DocumentElement.Attributes[ "saveToShared" ].Value );
			
			Clear();
			foreach( XmlNode licenseNode in xmlDoc.DocumentElement.SelectNodes( "License" ) )
				Add( new ExtendedLicense( licenseNode ) );

			XmlNode metaNode = xmlDoc.DocumentElement.SelectSingleNode( "Meta/Values" );
			if( metaNode != null )
				MetaValues.LoadFromXml( metaNode );

			if( Count == 0 && xml.IndexOf( "License" ) > -1 )
			{
				ExtendedLicense license = new ExtendedLicense();
				license.FromXmlString( xml );
				Add( license );
			}
		}

		/// <summary>
		/// Writes the license pack to the response stream for download. The included filename
		/// is only a suggestion to the user agent.
		/// </summary>
		/// <param name="filename">
		///		Suggested file name.
		/// </param>
		/// <param name="direct">
		///		Indicates that the file should be written directly to the response and not
		///		saved as a new file. When false the user is prompted to open or save the 
		///		file on their computer.
		/// </param>
		/// <remarks>
		///		Use this method to provide license downloads on your website. You should be
		///		careful to not write any additional content to the response stream, or call
		///		Response.End() after this method.
		///		<para>
		///		<b>NOTE:</b> The response must be buffered for this method to work. In general
		///		buffering is on by default in ASP.NET applications.
		///		</para>
		/// </remarks>
		public void WriteToResponse( string filename, bool direct )
		{
			if( ! HttpContext.Current.Response.BufferOutput )
				throw new ExtendedLicenseException( "E_ResponseMustBeBuffered" );
			HttpContext.Current.Response.Clear();
			if( HttpContext.Current.Request.Browser.Browser.IndexOf( "MSIE" ) > -1 )
				HttpContext.Current.Response.ContentType = "application/ms-download";
			else
				HttpContext.Current.Response.ContentType = "application/octet-stream";
			if( ! direct )
				HttpContext.Current.Response.AppendHeader( "Content-disposition", String.Format( System.Globalization.CultureInfo.InvariantCulture,"attachment;filename=\"{0}\"", filename ) );
			HttpContext.Current.Response.Write( ToFormattedXmlString() );
		}
		#region Overrides

		/// <summary>
		/// Writes the license pack to the response stream for download. The included filename
		/// is only a suggestion to the user agent.
		/// </summary>
		/// <param name="filename">
		///		Suggested file name.
		/// </param>
		/// <remarks>
		///		Use this method to provide license downloads on your website. You should be
		///		careful to not write any additional content to the response stream, or call
		///		Response.End() after this method.
		///		<para>
		///		<b>NOTE:</b> The response must be buffered for this method to work. In general
		///		buffering is on by default in ASP.NET applications.
		///		</para>
		/// </remarks>
		public void WriteToResponse( string filename )
		{
			WriteToResponse( filename, false );
		}

		/// <summary>
		/// Writes the license pack to the response stream for download.
		/// </summary>
		/// <remarks>
		///		Use this method to provide license downloads on your website.
		///		<para>
		///		<b>NOTE:</b> The response must be buffered for this method to work. In general
		///		buffering is on by default in ASP.NET applications.
		///		</para>
		/// </remarks>
		public void WriteToResponse()
		{
			WriteToResponse( ( Location == null || Location.Length == 0 ) ? "License.lic" : Path.GetFileName( Location ), false );
		}
		#endregion

		#region Isolated Storage Helper Methods
		internal static string IsfGetDirectoryName( string path )
		{
			if( path == null || path.Length == 0 )
				return Limit.GetCompatibleHashCode( "" ).GetHashCode().ToString( "X", System.Globalization.CultureInfo.InvariantCulture );
			return Limit.GetCompatibleHashCode( Path.GetFullPath( path.ToLower( System.Globalization.CultureInfo.InvariantCulture ) ) ).ToString( "X", System.Globalization.CultureInfo.InvariantCulture );
		}
		internal static string IsfGetFileName( string path )
		{
			return Path.Combine( IsfGetDirectoryName( Path.GetDirectoryName( path ) ), Path.GetFileName( path ) );
		}
		internal static bool IsfFileExists( string path )
		{
			try
			{
				if( ! IsfDirectoryExists( Path.GetDirectoryName( path ) ) )
					return false;

				IsolatedStorageFile isf = ExtendedLicensePack.GetIsfStore();
				return isf.GetFileNames( IsfGetFileName( path ) ).Length > 0;
			}
			catch
			{
				return false;
			}
		}

		internal static bool IsfDirectoryExists( string path )
		{
			try
			{
				IsolatedStorageFile isf = ExtendedLicensePack.GetIsfStore();
				return isf.GetDirectoryNames( IsfGetDirectoryName( path ) ).Length > 0;
			}
			catch
			{
				return false;
			}
		}

		internal static IsolatedStorageFile GetIsfStore()
		{
			IsolatedStorageFile isf = null;
			try
			{
				isf = IsolatedStorageFile.GetUserStoreForAssembly();
				isf.GetDirectoryNames( "*.test" );
			}
			catch
			{
				isf = IsolatedStorageFile.GetUserStoreForDomain();
			}

			return isf;
		}
		#endregion
        
		#region ICloneable Members

		/// <summary>
		/// Clones the license pack in memory.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			ExtendedLicensePack pack = new ExtendedLicensePack();
			pack._isEmbedded	= _isEmbedded;
			pack._location		= _location;
			pack._metaValues	= _metaValues;
			pack._saveToShared	= _saveToShared;

			foreach( ExtendedLicense license in this )
			{
				ExtendedLicense lic = new ExtendedLicense();
				lic.FromXmlString( license.ToXmlString() );
				pack.Add( lic );
			}

			return pack;
		}

		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class ExtendedLicensePack
	#endregion

} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
