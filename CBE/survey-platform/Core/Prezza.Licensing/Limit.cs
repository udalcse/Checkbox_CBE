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
// Class:		Limit
// Author:		Paul Alexander
// Created:		Friday, September 13, 2002 4:45:21 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Web;
using System.Runtime.Remoting;
using System.Globalization;
using System.Threading;
using Microsoft.Win32;
#if NUNIT
using NUnit.Framework;
#endif

namespace Xheo.Licensing
{
	/// <summary>
	/// Base class for all license limitations.
	/// </summary>
	/// <remarks>
	///		Limits are used by the XHEO|Licensing framework to restrict a Component,
	///		Class or Control to a specific set of runtime or designtime conditions. The
	///		library contains over 15 built in limits that you can use to control the
	///		use of your protected Type.
	/// </remarks>
	[Serializable]
#if LICENSING
	public
#else
	internal
#endif
 abstract class Limit : MarshalByRefObject, IDisposable
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		[NonSerialized]
		ExtendedLicense _license = null;
		internal string _loadReference = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Releases any managed or unmangaged resources used by the limit.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Releases any managed or unmanaged resources used by the Limit.
		/// </summary>
		/// <param name="disposing">
		///		If true only unmanaged resources should be disposed.
		/// </param>
		protected virtual void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( _license != null )
				{
					ExtendedLicense lic = _license;
					_license = null;
					lic.Dispose();
				}
			}
		}

		/// <summary>
		/// Releases any resources during garbage collection.
		/// </summary>
		~Limit()
		{
			Dispose( false );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets a value that indicates if this Limit uses remote processes (such as
		/// WebServices) for enforcement.
		/// </summary>
		[
			Browsable( false ),
			Category( "Misc" ),
			Description( "Indicates if the limit uses remote resources to validate." )
		]
		public virtual bool IsRemote
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value that indicates if this Limit uses a GUI to interact with the
		/// user.
		/// </summary>
		[
			Browsable( false ),
			Category( "Misc" ),
			Description( "Indicates if the limit uses a GUI to interact with the user." )
		]
		public virtual bool IsGui
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		[
			Browsable( false ),
			Category( "Misc" ),
			Description( "Description of the limit." )
		]
		public abstract string Description
		{
			get;
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		[
			Browsable( false ),
			Category( "Misc" ),
			Description( "Display name of the limit." )
		]
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// Gets or sets a reference to the <see cref="ExtendedLicense"/> that contains this Limit.
		/// </summary>
		[Browsable( false )]
		public ExtendedLicense License
		{
			get
			{
				return _license;
			}
			set
			{
				AssertNotReadOnly();
				_license = value;
			}
		}

		/// <summary>
		/// Gets a comma separate list of XML attributes that should be ignored when creating and
		/// validating the license signature for the limit.
		/// </summary>
		protected internal virtual string UnprotectedAttributes
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the limit is time sensitive and the clock
		/// should be protected.
		/// </summary>
		[ Browsable( false ) ]
		public virtual bool IsTimeSensitive
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a string to be used for loading the custom limit. The defualt is the 
		/// fully qualified assembly name of the type.
		/// </summary>
		[Browsable(false)]
		public virtual string QualifiedTypeName
		{
			get
			{
				if( _loadReference != null )
					return _loadReference;
				else
					return GetType().AssemblyQualifiedName;
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
		/// <remarks>
		///		If validation fails, limits should set their parent <c>License</c>'s <see cref="ExtendedLicense.InvalidReason"/>
		///		to a string explaining why it could not be validated.
		/// </remarks>
#if ! DEBUG
		[ System.Diagnostics.DebuggerHidden ]
#endif
		public abstract bool Validate( LicenseContext context, Type type, object instance );

		/// <summary>
		/// Loads the contents of a Limit from an XmlNode.
		/// </summary>
		/// <param name="node">
		///		The XmlNode to load from.
		/// </param>
		/// <returns>
		///		Returns true if the load was successful, otherwise false.
		/// </returns>
		public abstract bool LoadFromXml( XmlNode node );

		/// <summary>
		/// Saves the contents of the Limit as an XmlNode.
		/// </summary>
		/// <param name="parent">
		///		The parent XmlNode to add child nodes to.
		/// </param>
		/// <returns>
		///		Returns the saved XML node.
		/// </returns>
		public abstract XmlNode SaveToXml( XmlNode parent );

		/// <summary>
		/// Performs the default xml node construction for the limit.
		/// </summary>
		/// <param name="parent">
		///		The parent XmlNode to add child nodes to.
		/// </param>
		/// <param name="limitName">
		///		 The name of the limit type.
		/// </param>
		protected XmlNode BaseSaveToXml( XmlNode parent, string limitName )
		{
			XmlNode limitNode = parent.OwnerDocument.CreateElement( "Limit" );
			XmlAttribute attribute = parent.OwnerDocument.CreateAttribute( "type" );
			attribute.Value = limitName;
			limitNode.Attributes.Append( attribute );

			parent.AppendChild( limitNode );

			return limitNode;
		}

		/// <summary>
		/// Performs the default loading from an xml node.
		/// </summary>
		/// <param name="node">
		///		The node to load from.
		/// </param>
		/// <param name="limitName">
		///		The name of the limit to verify.
		/// </param>
		/// <returns>
		///		Returns true if the base requirements have been met, otherwise false.
		/// </returns>
		protected bool BaseLoadFromXml( XmlNode node, string limitName )
		{
			if( node.Attributes[ "type" ] == null )
				return false;

			if( String.Compare( node.Attributes[ "type" ].Value, limitName, false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			return true;
		}

		/// <summary>
		/// Gets a description of the details of the limit.
		/// </summary>
		/// <returns>
		/// Returns the description.
		/// </returns>
		public virtual string GetDetailsDescription()
		{
			return "";
		}

		/// <summary>
		/// Called every time the limit is granted. Used to enforce limits that
		/// must be checked each time a Type is created. 
		/// </summary>
		/// <param name="context">
		///		The LicenseContext used to determine the environment and settings
		///		of the licensing.
		/// </param>
		/// <param name="instance">
		///		The instance of the object being licensed.
		/// </param>
		/// <param name="type">
		///		The Type of the object being licensed.
		/// </param>
		/// <remarks>
		/// Checks in this method should be very fast because they are called every time a 
		/// licensed object is created. The default implementation does nothing.
		/// </remarks>
		public virtual void Granted( LicenseContext context, Type type, object instance )
		{
		}

		/// <summary>
		/// Allows a limit to communicate it's valid status without interacting with the user. Used
		/// with Or limits to pick limits that are valid before attempting to interact with the user.
		/// </summary>
		/// <param name="context">
		///		The LicenseContext used to determine the environment and settings
		///		of the licensing.
		/// </param>
		/// <param name="instance">
		///		The instance of the object being licensed.
		/// </param>
		/// <param name="type">
		///		The Type of the object being licensed.
		/// </param>
		/// <returns></returns>
		public virtual PeekResult Peek( LicenseContext context, Type type, object instance )
		{
			return PeekResult.Unknown;
		}

		/// <summary>
		/// Asserts that the limit is not currently read only. Use before setting the value
		/// of a property.
		/// </summary>
		protected void AssertNotReadOnly()
		{
			if( License != null )
				License.AssertNotReadOnly();
		}

		/// <summary>
		/// Initializes the start and end date for a given duration based on the first time
		/// the type is licensed on the machine.
		/// </summary>
		/// <param name="type">
		///		The type to check.
		/// </param>
		/// <param name="duration">
		///		The time allowed.
		/// </param>
		/// <param name="key">
		///		Additional key information used to identify the value.
		/// </param>
		/// <param name="ends">
		///		The date when the duration has expired.
		/// </param>
		/// <param name="started">
		///		The date when the class was first licensed.
		/// </param>
		/// <remarks>
		///		Returns a value that indicates if the initialization was successful.
		/// </remarks>
		protected bool InitDuration( Type type, TimeSpan duration, string key, out DateTime started, out DateTime ends )
		{
			RegistryKey infoKey = null;
			string hashKey = null;
			bool created = false;

			started = DateTime.UtcNow;
			ends = DateTime.UtcNow;

			if( License == null || License.Version >= ExtendedLicense.v2_0 )
			{
				AssemblyName name = type.Assembly.GetName();
				hashKey = type.FullName + "." + name.Version.Major + "." + name.Version.Minor + "." + this.Name + "." + key;
			}
			else
			{
				hashKey = type.FullName + "." + this.Name + "." + key;
			}

			infoKey = GetPrivateRegistryKey( hashKey, false, out created );
			if( infoKey == null )
				return false;

			try
			{
				if( created )
				{
					started = DateTime.UtcNow;
					ends = DateTime.UtcNow.Add( duration );

					string dateData = started.ToString( System.Globalization.CultureInfo.InvariantCulture ) + "|" + ends.ToString( System.Globalization.CultureInfo.InvariantCulture );

					byte[] keyData = Encoding.UTF8.GetBytes( dateData );
					for( int index = 0; index < keyData.Length; index++ )
						keyData[ index ] ^= ( (byte)( index & 0xFF ) );
					infoKey.SetValue( "X", keyData );
				}
				else
				{
					byte[] keyData = infoKey.GetValue( "X" ) as byte[];

					if( keyData == null )
						return false;

					for( int index = 0; index < keyData.Length; index++ )
						keyData[ index ] ^= ( (byte)( index & 0xFF ) );

					string dateData = Encoding.UTF8.GetString( keyData );
					int split = dateData.IndexOf( '|' );

					started = DateTime.Parse( dateData.Substring( 0, split ), System.Globalization.CultureInfo.InvariantCulture );
					ends = DateTime.Parse( dateData.Substring( split + 1 ), System.Globalization.CultureInfo.InvariantCulture );
				}

			}
			catch
			{
				ends = DateTime.UtcNow;
				started = DateTime.UtcNow;
				return false;
			}
			finally
			{
				if( infoKey != null )
				{
					infoKey.Close();
					( (IDisposable)infoKey ).Dispose();
				}
			}

			return true;
		}
		#region Helpers
		/// <summary>
		/// Initializes the start and end date for a given duration based on the first time
		/// the type is licensed on the machine.
		/// </summary>
		/// <param name="type">
		///		The type to check.
		/// </param>
		/// <param name="duration">
		///		The time allowed.
		/// </param>
		/// <param name="ends">
		///		The date when the duration has expired.
		/// </param>
		/// <param name="started">
		///		The date when the class was first licensed.
		/// </param>
		/// <remarks>
		///		Returns a value that indicates if the initialization was successful.
		/// </remarks>
		protected bool InitDuration( Type type, int duration, out DateTime started, out DateTime ends )
		{
			return InitDuration( type, TimeSpan.FromDays( duration ), null, out started, out ends );
		}
		#endregion

		/// <summary>
		/// Updates the start and end date for a given duration.
		/// </summary>
		/// <param name="type">
		///		The type to check.
		/// </param>
		/// <param name="key">
		///		Additional key information used to identify the value.
		/// </param>
		/// <param name="ends">
		///		The date when the duration has expired.
		/// </param>
		/// <param name="started">
		///		The date when the class was first licensed.
		/// </param>
		/// <remarks>
		///		Returns a value that indicates if the initialization was successful.
		/// </remarks>
		protected bool UpdateDuration( Type type, string key, DateTime started, DateTime ends )
		{
			RegistryKey infoKey = null;
			string hashKey = null;
			bool created = false;

			if( License == null || License.Version >= ExtendedLicense.v2_0 )
			{
				AssemblyName name = type.Assembly.GetName();
				hashKey = type.FullName + "." + name.Version.Major + "." + name.Version.Minor + "." + this.Name + "." + key;
			}
			else
			{
				hashKey = type.FullName + "." + this.Name + "." + key;
			}

			infoKey = GetPrivateRegistryKey( hashKey, false, out created );
			if( infoKey == null )
				return false;

			try
			{
				string dateData = started.ToString( System.Globalization.CultureInfo.InvariantCulture ) + "|" + ends.ToString( System.Globalization.CultureInfo.InvariantCulture );

				byte[] keyData = Encoding.UTF8.GetBytes( dateData );
				for( int index = 0; index < keyData.Length; index++ )
					keyData[ index ] ^= ( (byte)( index & 0xFF ) );
				infoKey.SetValue( "X", keyData );
			}
			catch
			{
				return false;
			}
			finally
			{
				if( infoKey != null )
				{
					infoKey.Close();
					( (IDisposable)infoKey ).Dispose();
				}
			}

			return true;
		}


		/// <summary>
		/// Initializes a counter based on the first time the type is licensed on the machine.
		/// </summary>
		/// <param name="type">
		///		The type to check.
		/// </param>
		/// <param name="key">
		///		Additional key information used to identify the value.
		/// </param>
		/// <param name="count">
		///		The current counter value.
		/// </param>
		/// <param name="maxCount">
		///		The maximum count.
		/// </param>
		/// <remarks>
		///		Returns a value that indicates if the initialization was successful.
		/// </remarks>
		protected bool InitCounter( Type type, string key, ref long maxCount, out long count )
		{
			RegistryKey infoKey = null;
			string hashKey = null;
			bool created = false;

			count = 0L;

			if( License == null || License.Version >= ExtendedLicense.v2_0 )
			{
				AssemblyName name = type.Assembly.GetName();
				hashKey = type.FullName + "." + name.Version.Major + "." + name.Version.Minor + "." + this.Name + "." + key;
			}
			else
			{
				hashKey = type.FullName + "." + this.Name + "." + key;
			}

			infoKey = GetPrivateRegistryKey( hashKey, false, out created );
			if( infoKey == null )
				return false;

			try
			{
				if( created )
				{
					string countData = Convert.ToString( count, System.Globalization.CultureInfo.InvariantCulture ) + '|' + Convert.ToString( maxCount, System.Globalization.CultureInfo.InvariantCulture );

					byte[] keyData = Encoding.UTF8.GetBytes( countData );
					for( int index = 0; index < keyData.Length; index++ )
						keyData[ index ] ^= ( (byte)( index & 0xFF ) );
					infoKey.SetValue( "X", keyData );
				}
				else
				{
					byte[] keyData = infoKey.GetValue( "X" ) as byte[];

					if( keyData == null )
						return false;

					for( int index = 0; index < keyData.Length; index++ )
						keyData[ index ] ^= ( (byte)( index & 0xFF ) );

					string countData = Encoding.UTF8.GetString( keyData );

					int split = countData.IndexOf( '|' );

					count = Convert.ToInt64( countData.Substring( 0, split ), System.Globalization.CultureInfo.InvariantCulture );
					maxCount = Convert.ToInt64( countData.Substring( split + 1 ), System.Globalization.CultureInfo.InvariantCulture );

				}
			}
			catch
			{
				count = 0L;
				return false;
			}
			finally
			{
				if( infoKey != null )
				{
					infoKey.Close();
					( (IDisposable)infoKey ).Dispose();
				}
			}

			return true;
		}

		/// <summary>
		/// Initializes a counter based on the first time the type is licensed on the machine.
		/// </summary>
		/// <param name="type">
		///		The type to check.
		/// </param>
		/// <param name="key">
		///		Additional key information used to identify the value.
		/// </param>
		/// <param name="count">
		///		The current counter value.
		/// </param>
		/// <param name="maxCount">
		///		The maximum allowed for the count value.
		/// </param>
		/// <remarks>
		///		Returns a value that indicates if the initialization was successful.
		/// </remarks>
		protected bool UpdateCounter( Type type, string key, long maxCount, long count )
		{
			RegistryKey infoKey = null;
			string hashKey = null;
			bool created = false;

			if( License == null || License.Version >= ExtendedLicense.v2_0 )
			{
				AssemblyName name = type.Assembly.GetName();
				hashKey = type.FullName + "." + name.Version.Major + "." + name.Version.Minor + "." + this.Name + "." + key;
			}
			else
			{
				hashKey = type.FullName + "." + this.Name + "." + key;
			}

			infoKey = GetPrivateRegistryKey( hashKey, false, out created );
			if( infoKey == null )
				return false;

			try
			{
				string countData = Convert.ToString( count, System.Globalization.CultureInfo.InvariantCulture ) + '|' + Convert.ToString( maxCount, System.Globalization.CultureInfo.InvariantCulture );

				byte[] keyData = Encoding.UTF8.GetBytes( countData );
				for( int index = 0; index < keyData.Length; index++ )
					keyData[ index ] ^= ( (byte)( index & 0xFF ) );
				infoKey.SetValue( "X", keyData );
			}
			catch
			{
				count = 0L;
				return false;
			}
			finally
			{
				if( infoKey != null )
				{
					infoKey.Close();
					( (IDisposable)infoKey ).Dispose();
				}
			}

			return true;
		}

		/// <summary>
		/// Gets a private registry key where values for the license can be stored for the given type.
		/// </summary>
		/// <param name="key">
		///		A unique key to identify this registry section.
		/// </param>
		/// <param name="created">
		///		Accepts a value that indicates if the key was created.
		/// </param>
		/// <param name="mustExist">
		///		Indicates if the key must already exist.
		/// </param>
		/// <returns></returns>
		static public RegistryKey GetPrivateRegistryKey( string key, bool mustExist, out bool created )
		{
			RegistryKey infoKey = null;
			string clsid = Guid.NewGuid().ToString( "B", System.Globalization.CultureInfo.InvariantCulture ).ToUpper( System.Globalization.CultureInfo.InvariantCulture );


			string hash = Limit.GetCompatibleHashCode( key ).ToString( System.Globalization.CultureInfo.InvariantCulture );
			created = false;

			string uniqueKey = ".lic";

			try
			{
				bool user = false;
				try
				{
					infoKey = Registry.ClassesRoot.OpenSubKey( uniqueKey + "\\CLSlD\\", true );
					if( infoKey == null )
						infoKey = OpenOrCreateRegistryKey( Registry.ClassesRoot, uniqueKey + "\\CLSlD" );
				}
				catch( System.Security.SecurityException )
				{
					infoKey = Registry.CurrentUser.OpenSubKey( "software\\Classes\\" + uniqueKey + "\\CLS1D\\", true );
					if( infoKey == null )
						infoKey = OpenOrCreateRegistryKey( Registry.CurrentUser, "software\\Classes\\" + uniqueKey + "\\CLS1D" );
					user = true;
				}
				catch( System.UnauthorizedAccessException )
				{
					infoKey = Registry.CurrentUser.OpenSubKey( "software\\Classes\\" + uniqueKey + "\\CLS1D\\", true );
					if( infoKey == null )
						infoKey = OpenOrCreateRegistryKey( Registry.CurrentUser, "software\\Classes\\" + uniqueKey + "\\CLS1D" );
					user = true;
				}


				if( infoKey.GetValue( hash ) == null )
				{
					created = true;
					if( mustExist )
						return null;
					infoKey.SetValue( hash, clsid );
					infoKey.Close();

					if( !user )
					{
						using( RegistryKey clsidKey = Registry.ClassesRoot.OpenSubKey( "CLSID", true ) )
							infoKey = clsidKey.CreateSubKey( clsid );
					}
					else
					{
						using( RegistryKey clsidKey = OpenOrCreateRegistryKey( Registry.CurrentUser, "Software\\Classes\\CLSID" ) )
							infoKey = clsidKey.CreateSubKey( clsid );
					}

					infoKey.SetValue( null, hash );
				}
				else
				{
					clsid = infoKey.GetValue( hash ) as string;
					if( clsid == null )
						return null;
					infoKey.Close();
					if( !user )
					{
						infoKey = Registry.ClassesRoot.OpenSubKey( "CLSID\\" + clsid, true );
					}
					else
					{
						infoKey = Registry.CurrentUser.OpenSubKey( "Software\\Classes\\CLSID\\" + clsid, true );
					}
					if( infoKey.GetValue( null ) as string != hash )
						return null;
				}

			}
			catch( Exception )
			{
				if( infoKey != null )
				{
					infoKey.Close();
					( (IDisposable)infoKey ).Dispose();
				}
				infoKey = null;
			}

			return infoKey;
		}


		/// <summary>
		/// Selects the given culture for the current thread, and returns the previous
		/// culture.
		/// </summary>
		/// <param name="culture">
		///		The culture to select.
		/// </param>
		/// <returns>
		///		Returns the previous culture.
		/// </returns>
		public static CultureInfo SelectCulture( CultureInfo culture )
		{
			CultureInfo previous = System.Threading.Thread.CurrentThread.CurrentUICulture;

			if( culture.IsNeutralCulture )
			{
				foreach( CultureInfo specific in CultureInfo.GetCultures( CultureTypes.SpecificCultures ) )
					if( specific.Name.StartsWith( culture.Name ) )
					{
						culture = specific;
						break;
					}

				if( culture.IsNeutralCulture )
					return previous;
			}
			System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
			return previous;
		}

		/// <summary>
		/// Selects the culture of the current web request into the current thread, and returns the previous
		/// culture.
		/// </summary>
		/// <returns>
		///		Returns the previous culture.
		/// </returns>
		public static CultureInfo SelectWebCulture()
		{
			CultureInfo culture;
			CultureInfo previous = System.Threading.Thread.CurrentThread.CurrentUICulture;
			if( !ExtendedLicense.IsWebRequest )
				return previous;

			try
			{
				HttpContext context = HttpContext.Current;
				string[] languages = context.Request.UserLanguages;
				if( languages != null && languages.Length > 0 )
				{
					foreach( string language in languages )
					{
						try
						{
							int index = language.IndexOf( ';' );
							culture = new CultureInfo( index == -1 ? language : language.Substring( 0, index ) );
							culture = SelectCulture( culture );
							if( CultureInfo.CurrentUICulture != previous )
								return culture;
						}
						catch( ArgumentException ) { }
					}
				}

				return previous;
			}
			catch
			{
				return previous;
			}
		}

		/// <summary>
		/// Gets a Limit object based on the given name.
		/// </summary>
		/// <param name="name">
		///		The name of the limit type to retrieve.
		/// </param>
		/// <returns>
		///		Returns a new Limit object of the given type if available, otherwise
		///		null.
		/// </returns>
		[System.Security.Permissions.ReflectionPermission( System.Security.Permissions.SecurityAction.Demand )]
		public static Limit GetLimitFromName( string name )
		{
			Limit limit = null;
			bool explicitReference = false;

			try
			{
				Type type = null;

				if( name.IndexOf( '.' ) == -1 )
				{
					#region Use name
					switch( name )
					{
#if LICENSING || LICENSEACTIVATION
						case "Activation":
							return new ActivationLimit();
#endif
						case "And":
							return new AndLimit();
						case "Application":
							return new ApplicationLimit();
						case "Beta":
							return new BetaLimit();
						case "Branded":
							return new BrandedLimit();
						case "Designtime":
							return new DesigntimeLimit();
						case "Domain":
							return new DomainLimit();
						case "GUIBranded":
							return new GuiBrandedLimit();							
						case "IP":
							return new IPLimit();
#if LICENSING ||  LICENSESERVERS
						case "LicenseServer":
							return new LicenseServerLimit();							
#endif
						case "LocalComputer":
							return new LocalComputerLimit();
						case "Machine":
							return new MachineLimit();
						case "NonCommercial":
							return new NonCommercialLimit();
						case "OnServer":
							return new OnServerLimit();							
						case "Or":
							return new OrLimit();
						case "Publisher":
							return new PublisherLimit();
#if LICENSING || LICENSEREGISTRATIONS
						case "Registration":
							return new RegistrationLimit();
#endif
						case "Runtime":
							return new RuntimeLimit();
						case "Sessions":
							return new SessionsLimit();
						case "Trial":
							return new TrialLimit();
						case "Version":
							return new VersionLimit();
					}
					#endregion
				}

				if( type == null )
				{
					type = Type.GetType( name );

					if( type == null )
					{
						int pos = name.IndexOf( ',' );
						string typeName = name.Substring( 0, pos );
						string assemblyName = name.Substring( pos + 1 );

						Assembly assembly = Assembly.Load( assemblyName );
						if( assembly == null )
#if NET11 || NET10
							assembly = Assembly.LoadWithPartialName( assemblyName );
#else
							assembly = Assembly.Load( assemblyName );
#endif

						if( assembly != null )
							type = assembly.GetType( typeName, false, true );

					}

					if( type == null )
					{
#if ! LICENSING
						LicenseHelper helper = LicenseHelper.GetHelperForAssembly( Assembly.GetExecutingAssembly() );
						if( helper != null )
						{
							limit = helper.GetLimitFromName( name );
							if( limit != null )
								return limit;
						}
#endif

						throw new ExtendedLicenseException( "E_LimitDoesNotExist", name );
					}
					explicitReference = true;
				}

				limit = Activator.CreateInstance( type ) as Limit;
				if( explicitReference )
					limit._loadReference = name;
			}
			catch( ExtendedLicenseException )
			{
				throw;
			}
			catch( Exception ex )
			{
				throw new ExtendedLicenseException( "E_LimitDoesNotExist", ex, name );
			}

			return limit;
		}

		/// <summary>
		/// Validates the format of an IP address.
		/// </summary>
		/// <param name="address">
		///		The IP Address to validate.
		/// </param>
		/// <returns>
		///		Returns true if the format of the address is valid, otherwise false.
		/// </returns>
		public static bool ValidateIPAddress( string address )
		{
			return Regex.IsMatch( address, Internal.CommonRegexes.IpAddress, RegexOptions.Compiled );
		}

		/// <summary>
		/// Validates the format of a domain name.
		/// </summary>
		/// <param name="domainName">
		///		The domain name to validate.
		/// </param>
		/// <returns>
		///		Returns true if the format of the domain name is valid, otherwise false.
		/// </returns>
		public static bool ValidateDomainName( string domainName )
		{
			return Regex.IsMatch( domainName, Internal.CommonRegexes.IpAddress, RegexOptions.Compiled );
		}

		/// <summary>
		/// Validates the url is correctly formatted.
		/// </summary>
		/// <param name="url">
		///		The url to validate.
		/// </param>
		/// <param name="fullyQualified">
		///		Indicates if the url must be fully qualified. Containing a host and optional protocol.
		/// </param>
		/// <returns>
		///		Returns true if the url is valid, otherwise false.
		/// </returns>
		public static bool ValidateUrl( string url, bool fullyQualified )
		{
			return Regex.IsMatch( url, fullyQualified ? Internal.CommonRegexes.FullyQualifiedUrl : Internal.CommonRegexes.Url, RegexOptions.Compiled | RegexOptions.IgnoreCase );
		}

		/// <summary>
		/// Validates if the email is correctly formatted.
		/// </summary>
		/// <param name="email">
		///		The email address to validate.
		/// </param>
		/// <returns>
		///		Returns true if the email is valid, otherwise false.
		/// </returns>
		public static bool ValidateEmail( string email )
		{
			return Regex.IsMatch( email, Internal.CommonRegexes.Email );
		}

		/// <summary>
		/// Parses an IPAddress to it's actual long value.
		/// </summary>
		/// <param name="address">
		///		IPAddress to parse.
		/// </param>
		/// <returns>
		///		Returns the actual address of the IPAddress.
		/// </returns>
		public static long ParseIPAddress( IPAddress address )
		{
			long p1;
			long p2;
			long p3;
			long p4;

			string[] parts = address.ToString().Split( '.' );

			p1 = Convert.ToInt64( parts[ 0 ], System.Globalization.CultureInfo.InvariantCulture );
			p2 = Convert.ToInt64( parts[ 1 ], System.Globalization.CultureInfo.InvariantCulture );
			p3 = Convert.ToInt64( parts[ 2 ], System.Globalization.CultureInfo.InvariantCulture );
			p4 = Convert.ToInt64( parts[ 3 ], System.Globalization.CultureInfo.InvariantCulture );

			return ( p1 << 24 ) | ( p2 << 16 ) | ( p3 << 8 ) | p4;
		}

		/// <summary>
		/// Gets an Image for the requested resource. Resources can be defined as
		/// assembly resources or as remote resources. If the name is prefixed with
		/// http then it is assumed to be a remote resource and a <see cref="WebRequest"/>
		/// is made to retrieve the bitmap, otherwise it is assumed to be embedded
		/// in the assembly.
		/// </summary>
		/// <param name="bitmapResource">
		///		Name of the resource to retrieve, prefix with http to get a remote resource.
		/// </param>
		/// <param name="limit">
		///		The limit requesting the resource.
		/// </param>
		/// <returns>
		///		Returns the resource if found, otherwise null.
		/// </returns>
		/// <param name="parameters">
		///		Collection of additional parameters to append to the URL for remote
		///		images. Parameters should be in the format "key=value".
		/// </param>
		/// <remarks>
		///		For remote resources the serial number, expiration date, and optional parameters
		///		of the License will be appended to the request QueryString. You can use this
		///		to provide alternate images based on the conditions of the license.
		/// </remarks>
		[System.Security.Permissions.ReflectionPermission( System.Security.Permissions.SecurityAction.Demand )]
		public static Bitmap GetBitmapResource( string bitmapResource, Limit limit, params string[] parameters )
		{
			Stream imageStream = null;
			Image image = null;

			try
			{
				if( bitmapResource != null && bitmapResource.Length > 0 )
				{
					try
					{
						string bmpl = bitmapResource.ToLower( System.Globalization.CultureInfo.InvariantCulture );
						if( bmpl.StartsWith( "http://" ) || bmpl.StartsWith( "https://" ) || bmpl.StartsWith( "file://" ) )
						{
							try
							{
								StringBuilder requestUrl = new StringBuilder( bitmapResource );
								if( bitmapResource.IndexOf( '?' ) > -1 )
									requestUrl.Append( '&' );
								else
									requestUrl.Append( '?' );
								requestUrl.AppendFormat( System.Globalization.CultureInfo.InvariantCulture,
									"serialNumber={0}&expires={1}&culture={2}",
									HttpUtility.UrlEncode( limit.License.SerialNumber ),
#if LICENSING || LICENSETRIALS
 HttpUtility.UrlEncode( limit is TrialLimit ? ( (TrialLimit)limit ).Ends.ToString( System.Globalization.CultureInfo.InvariantCulture ) : limit.License.Expires.ToString( System.Globalization.CultureInfo.InvariantCulture ) ),
#else
									HttpUtility.UrlEncode( limit.License.Expires.ToString( System.Globalization.CultureInfo.InvariantCulture ) ),
#endif
 System.Globalization.CultureInfo.CurrentUICulture.Name
									);
								if( parameters != null )
									foreach( string parameter in parameters )
										requestUrl.Append( '&' + parameter );

								try
								{
									string tempFile = Limit.GetCompatibleHashCode( requestUrl.ToString() ).ToString( System.Globalization.CultureInfo.InvariantCulture ) + ".bmpcache";
									string tempPath = Path.Combine( Path.GetTempPath(), tempFile );

									if( File.Exists( tempPath ) )
									{
										if( File.GetLastWriteTime( tempPath ).AddDays( 1 ) > DateTime.UtcNow )
										{
											imageStream = new FileStream( tempPath, FileMode.Open, FileAccess.Read, FileShare.Read );
											image = Image.FromStream( imageStream );
											imageStream.Close();
											imageStream = null;
										}
									}
								}
								catch
								{
									if( imageStream != null )
										imageStream.Close();
									imageStream = null;
								}

								if( image == null )
								{
									WebRequest request = WebRequest.Create( new Uri( requestUrl.ToString() ) );
									request.Timeout = 5000;
									using( WebResponse response = request.GetResponse() )
									{
										imageStream = response.GetResponseStream();
										if( imageStream != null )
										{
											image = Image.FromStream( imageStream );
											imageStream.Close();
											imageStream = null;

											try
											{
												string tempFile = Limit.GetCompatibleHashCode( requestUrl.ToString() ).ToString( System.Globalization.CultureInfo.InvariantCulture ) + ".bmpcache";
												string tempPath = Path.Combine( Path.GetTempPath(), tempFile );

												image.Save( tempPath, image.RawFormat );
											}
											catch { }
										}
									}
								}
							}
							catch( Exception ex )
							{
								System.Diagnostics.Debug.WriteLine( ex.ToString() );
							}
						}
						else
						{
							int index = bitmapResource.IndexOf( ',' );
							string resourceName = index == -1 ? bitmapResource : bitmapResource.Substring( 0, index );
							string assemblyName = index == -1 ? null : bitmapResource.Substring( index + 1 ).Trim();

							try
							{
								System.Reflection.Assembly assembly = null;
								if( assemblyName.StartsWith( typeof( Limit ).Assembly.GetName().Name ) )
									assembly = typeof( ExtendedLicense ).Assembly;
								else
									assembly = System.Reflection.Assembly.Load( assemblyName );
								imageStream = assembly.GetManifestResourceStream( resourceName );
								if( imageStream != null )
								{
									image = Image.FromStream( imageStream );
									imageStream.Close();
									imageStream = null;
								}
							}
							catch( Exception ex )
							{
								System.Diagnostics.Debug.WriteLine( ex.ToString() );
							}
						}
					}
					finally
					{
						if( imageStream != null )
							imageStream.Close();
					}
				}

				if( image != null )
					return new Bitmap( image, image.Size );
				return image as Bitmap;
			}
			finally
			{
				if( image != null )
					image.Dispose();
			}
		}

		/// <summary>
		/// Opens or creates a registry key.
		/// </summary>
		/// <param name="key">
		///		The parent key to open from.
		/// </param>
		/// <param name="subkey">
		///		The path of the sub key to open. Sub key should be separated by \ characters.
		/// </param>
		/// <returns>
		///		Returns the opened or created registry key.
		/// </returns>
		public static RegistryKey OpenOrCreateRegistryKey( RegistryKey key, string subkey )
		{
			string[] subkeys = subkey.Split( '\\' );
			RegistryKey reg1 = key;
			RegistryKey reg2 = null;
			foreach( string sub in subkeys )
			{
				reg2 = reg1.OpenSubKey( sub, true );
				if( reg2 == null )
					reg2 = reg1.CreateSubKey( sub );
				reg1.Close();
				( (IDisposable)reg1 ).Dispose();
				reg1 = reg2;
			}

			return reg1;
		}

		/// <summary>
		/// Performs a simple value obfuscation to make editing client side values 
		/// more difficult. This is by no means secure.
		/// </summary>
		/// <param name="source">
		///		The original value.
		/// </param>
		/// <returns>
		///		Returns the masked version of the value. Call UnmaskValue 
		///		to obtain the original value.
		/// </returns>
		public static string MaskValue( string source )
		{
			char[] chars = source.ToCharArray();

			for( int index = 0; index < chars.Length; index++ )
				chars[ index ] ^= (char)( ( index * 111 ) & 0xFF );

			return Convert.ToBase64String( Encoding.Unicode.GetBytes( chars ) );
		}

		/// <summary>
		/// Reverses the simple value obfuscation performed by <see cref="MaskValue"/>.
		/// </summary>
		/// <param name="source">
		///		The original value.
		/// </param>
		/// <returns>
		///		Returns the masked version of the value. Call UnmaskValue 
		///		to obtain the original value.
		/// </returns>
		public static string UnmaskValue( string source )
		{
			char[] chars = Encoding.Unicode.GetChars( Convert.FromBase64String( source ) );

			for( int index = 0; index < chars.Length; index++ )
				chars[ index ] ^= (char)( ( index * 111 ) & 0xFF );

			return new string( chars );
		}


		internal static int GetCompatibleHashCode( string s )
		{
			uint hash = 5381u;

			int length = s.Length;
			for( int index = 0; index < length; index++ )
			{
				hash = unchecked( ( ( hash << 5 ) + hash ) ^ s[ index ] );
			}

			return unchecked( (int)hash );
		}


		/// <summary>
		/// Creates a resource string for loading an embedded resource based on the
		/// given type.
		/// </summary>
		/// <param name="resourceName">
		///		The resource to load.
		/// </param>
		/// <param name="referenceType">
		///		A reference type for scoping the resource.
		/// </param>
		/// <returns>
		///		Returns a new string representing the requested resource.
		/// </returns>
		internal static string MakeResourceString( string resourceName, Type referenceType )
		{
#if LICENSING
			return "Xheo.Licensing." + resourceName + ",Xheo.Licensing.v3";
#else
			if( _resourceRoot == null )
			{
				lock( typeof( Limit ) )
				{
					string[] resources = referenceType.Assembly.GetManifestResourceNames();
					foreach( string resource in resources )
						if( resource.EndsWith( resourceName ) )
						{
							_resourceRoot = resource.Substring( 0, resource.Length - resourceName.Length );
							break;
						}
				}
				if( _resourceRoot == null )
					throw new ExtendedLicenseException( "E_MissingEmbeddedImages" );
			}

			return _resourceRoot + resourceName + "," + referenceType.Assembly.FullName;
#endif
		}
#if ! LICENSING
		private static string _resourceRoot = null;
#endif

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class Limit

	#region TransientReadOnlyCollection
	/// <summary>
	/// Implements a transient read only collection. Values can be added, removed
	/// and	changed until the <see cref="SetReadOnly"/> method is called thus
	/// forcing the collection to a read only state.
	/// </summary>
	[Serializable]
#if LICENSING
	public
#else
	internal
#endif
	 abstract class TransientReadOnlyCollection : CollectionBase
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private bool _isReadOnly = false;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Events
	

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the TransientReadOnlyCollectionBase class.
		/// </summary>
		protected TransientReadOnlyCollection()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets a value indicating if the collection is read only.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
		} 

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations


		/// <summary>
		/// Sets the collection to a read only state. This cannot be reversed.
		/// </summary>
		public void SetReadOnly()
		{
			_isReadOnly = true;
		}

		/// <summary>
		/// Overrides the base functionality to ensure the class is read only.
		/// </summary>
		protected override void OnInsert( int index, object value )
		{
			if( _isReadOnly )
				throw new ExtendedLicenseException( "E_CollectionReadOnly", GetType() );

			base.OnInsert( index, value );
		}

		/// <summary>
		/// Overrides the base functionality to ensure the class is read only.
		/// </summary>
		protected override void OnSet( int index, object oldValue, object value )
		{
			if( _isReadOnly )
				throw new ExtendedLicenseException( "E_CollectionReadOnly", GetType() );

			base.OnSet( index, oldValue, value );
		}

		/// <summary>
		/// Overrides the base functionality to ensure the class is read only.
		/// </summary>
		protected override void OnRemove( int index, object value )
		{
			if( _isReadOnly )
				throw new ExtendedLicenseException( "E_CollectionReadOnly", GetType() );

			base.OnRemove( index, value );
		}

		/// <summary>
		/// Overrides the base functionality to ensure the class is read only.
		/// </summary>
		protected override void OnClear()
		{
			if( _isReadOnly )
				throw new ExtendedLicenseException( "E_CollectionReadOnly", GetType() );

			base.OnClear();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class TransientReadOnlyCollectionBase
	#endregion

	#region LimitCollection
	/// <summary>
	/// Services as a collection of limits.
	/// </summary>
	[ Serializable ]
#if LICENSING
	public 
#else
	internal
#endif
	class LimitCollection : TransientReadOnlyCollection
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		[NonSerialized]
		private		ExtendedLicense _license	= null;
		[NonSerialized]
		internal	int				_refCount	= 0;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///  Initializes a new instance of the LimitCollection class.
		/// </summary>
		/// <param name="license">
		///		Reference to the containing ExtendedLicense.
		/// </param>
		public LimitCollection( ExtendedLicense license )
		{
			_license = license;
		}

		///	<summary>
		///	Included for COM compliance, do not use.
		///	</summary>
		public LimitCollection()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets a reference to the containing license.
		/// </summary>
		public ExtendedLicense License
		{
			get
			{
				return _license;
			}
			set
			{
				_license = value;
			}
		}

		/// <summary>
		/// Gets or sets a limit at the given index.
		/// </summary>
		public Limit this[ int index ]
		{
			get
			{
				return List[ index ] as Limit;
			}
			set
			{
				List[ index ] = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Adds a new limit to the collection.
		/// </summary>
		/// <param name="limit">
		///		The limit to add.
		/// </param>
		public void Add( Limit limit )
		{
			List.Add( limit );
			if( limit.License == null )
				limit.License = License;
		}

		/// <summary>
		/// Inserts the limit at the given index.
		/// </summary>
		/// <param name="index">
		///		The index where to insert the limit.
		/// </param>
		/// <param name="limit">
		///		The limit to insert.
		/// </param>
		public void Insert( int index, Limit limit )
		{
			List.Insert( index, limit );
			if( limit.License == null )
				limit.License = License;
		}

		/// <summary>
		/// Removes the given index from the list.
		/// </summary>
		/// <param name="limit">
		///		The limit to remove.
		/// </param>
		public void Remove( Limit limit )
		{
			List.Remove( limit );
		}

		/// <summary>
		/// Copies the collection to an array.
		/// </summary>
		/// <param name="array">
		///		The array to copy to.
		/// </param>
		/// <param name="index">
		///		The index in the array to begin copying.
		/// </param>
		public void CopyTo( Limit[] array, int index )
		{
			List.CopyTo( array, index );
		}

		/// <summary>
		/// Gets the index of the given limit.
		/// </summary>
		/// <param name="limit">
		///		The limit to look for.
		/// </param>
		/// <returns>
		///		Returns the index of the given limit if found, otherwise -1.
		/// </returns>
		public int IndexOf( Limit limit )
		{
			return List.IndexOf( limit );
		}

		/// <summary>
		/// Determines if the collection contains the given Limit.
		/// </summary>
		/// <param name="limit">
		///		The Limit to look for.
		/// </param>
		/// <returns>
		///		Returns true if the Limit is found, otherwise false.
		/// </returns>
		public bool Contains( Limit limit )
		{
			return List.Contains( limit );
		}

		/// <summary>
		/// Ensures that all the Limits in the collection are valid for the given type
		/// and context.
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
		/// <param name="includeRemote">
		///		Indicates if limits that have <see cref="Limit.IsRemote"/> set to true 
		///		should be included in the validation.
		/// </param>
		/// <returns>
		///		Returns true if the limits are valid, otherwise false.
		/// </returns>
		public virtual bool Validate( LicenseContext context, Type type, object instance, bool includeRemote )
		{
			foreach( Limit limit in GetPrioritizedOrder( includeRemote ) )
			{
				if( ( includeRemote || !limit.IsRemote ) &&
					!limit.Validate( context, type, instance ) )
					return false;
			}

			return true;
		}

		/// <summary>
		/// Gets the limits in order of priority.
		/// </summary>
		/// <param name="includeRemote">
		///		Indicates if remote limits should be included.
		/// </param>
		/// <returns></returns>
		public LimitCollection GetPrioritizedOrder( bool includeRemote )
		{
			LimitCollection results = new LimitCollection( _license );

			ArrayList guiLimits = new ArrayList();
			ArrayList remoteLimits = new ArrayList();

			foreach( Limit limit in this )
			{
				if( limit.License == null )
					limit.License = _license;

				if( limit.IsGui )
					guiLimits.Add( limit );
				else if( limit.IsRemote )
				{
					if( includeRemote )
						remoteLimits.Add( limit );
				}
				else
					results.Add( limit );
			}

			foreach( Limit limit in remoteLimits )
				results.Add( limit );

			foreach( Limit limit in guiLimits )
				results.Add( limit );

			return results;
		}

		/// <summary>
		/// Saves the contents of the LimitCollection to the given XmlNode.
		/// </summary>
		/// <param name="parent">
		///		The parent XmlNode.
		/// </param>
		/// <returns>
		///		Returns the root node of the saved limits.
		/// </returns>
		public XmlNode SaveToXml( XmlNode parent )
		{
			XmlNode limitsNode = parent.OwnerDocument.CreateElement( "Limits" );
			
			foreach( Limit limit in this )
			{
				XmlNode limitNode = limit.SaveToXml( limitsNode );

				if( limit.GetType().Assembly.FullName != typeof( Limit ).Assembly.FullName )
				{
					XmlAttribute netTypeAttribute = parent.OwnerDocument.CreateAttribute( "netType" );
										
					netTypeAttribute.Value = limit.QualifiedTypeName;
					limitNode.Attributes.Append( netTypeAttribute );
				}
			}

			parent.AppendChild( limitsNode );

			return limitsNode;
		}

		/// <summary>
		/// Loads the contents of a LimitCollection from an XmlNode.
		/// </summary>
		/// <returns>
		///		Returns true if the load was successful, otherwise false.
		/// </returns>
		public bool LoadFromXml( XmlNode node )
		{
			if( node == null )
				return true;

			foreach( XmlNode limitNode in node.SelectNodes( "Limit" ) )
			{
				string name	= null;
				Limit limit = null;
				
				if( limitNode.Attributes[ "netType" ] == null )
					name = limitNode.Attributes[ "type" ].Value;
				else
					name = limitNode.Attributes[ "netType" ].Value;

				limit = Limit.GetLimitFromName( name );

				if( limit != null )
					limit.License = License;
				if( limit == null || ! limit.LoadFromXml( limitNode ) )
					return false;
				Add( limit );
			}

			return true;
		}

		/// <summary>
		/// Saves the contents of the limit collection to an XML string.
		/// </summary>
		/// <returns>
		///		The saved XML.
		/// </returns>
		public string ToXmlString()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.AppendChild( xmlDoc.CreateElement( "Dummy" ) );
			SaveToXml( xmlDoc.DocumentElement );
			return xmlDoc.DocumentElement.InnerXml;
		}

		/// <summary>
		/// Loads the contents of the limit collection from an XML string.
		/// </summary>
		/// <param name="xml">
		///		The XML string to load from.
		/// </param>
		public void FromXmlString( string xml )
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml( xml );
			if( ! LoadFromXml( xmlDoc.DocumentElement ) )
				throw new ExtendedLicenseException( "E_InvalidXmlForLimits");
		}

		/// <summary>
		/// Called every time the limit is granted.
		/// </summary>
		public void Granted( LicenseContext context, Type type, object instance )
		{
			foreach( Limit limit in this )
				limit.Granted( context, type, instance );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class LimitCollection
	#endregion

	#region ContainerLimit
	/// <summary>
	/// Serves as a base class for limits that contain other limits.
	/// </summary>
	[Serializable]
#if LICENSING
	public 
#else
	internal
#endif
	abstract class ContainerLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		LimitCollection _limits		= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the ContainerLimit class.
		/// </summary>
		protected ContainerLimit()
		{
		}

		/// <summary>
		/// Releases any managed or unmanged resources.
		/// </summary>
		/// <param name="disposing">
		///		Indicates if Dispose was called directly.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				foreach( Limit limit in Limits )
					limit.Dispose();
			}
			base.Dispose (disposing);
		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the collection of child limits.
		/// </summary>
		[ Browsable( false ) ]
		public LimitCollection Limits
		{
			get
			{
				if( _limits == null )
					_limits = new LimitCollection( License );

				if( License != null && License.IsReadOnly && ! _limits.IsReadOnly )
					_limits.SetReadOnly();
					
				return _limits;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the limit may use a GUI interface.
		/// </summary>
		public override bool IsGui
		{
			get
			{
				foreach( Limit limit in Limits )
					if( limit.IsGui )
						return true;
				return false;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the limit may use remote resources.
		/// </summary>
		public override bool IsRemote
		{
			get
			{
				foreach( Limit limit in Limits )
					if( limit.IsRemote )
						return true;
				return false;
			}
		}

		/// <summary>
		/// Determines if any contained limits are time sensitive.
		/// </summary>
		public override bool IsTimeSensitive
		{
			get
			{
				foreach( Limit limit in Limits )
					if( limit.IsTimeSensitive )
						return true;
				return false;
			}
		}



		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Loads the contents of a Limit from an XmlNode.
		/// </summary>
		/// <param name="node">
		///		The XmlNode to load from.
		/// </param>
		/// <returns>
		///		Returns true if the load was successful, otherwise false.
		/// </returns>
		public override bool LoadFromXml( XmlNode node )
		{
			return LoadFromXml( node, Name );
		}

		/// <summary>
		/// Loads the contents of the container limit from an XmlNode using the
		/// given name.
		/// </summary>
		/// <param name="node">
		///		The XmlNode to load from.
		/// </param>
		/// <param name="limitName">
		///		The name of the container type.
		/// </param>
		/// <returns>
		///		Returns true if the load was successful, otherwise false.
		/// </returns>
		protected bool LoadFromXml( XmlNode node, string limitName )
		{
			if( ! BaseLoadFromXml( node, limitName ) )
				return false;

			foreach( XmlNode limitNode in node.SelectNodes( "Limit" ) )
			{
				string name	= null;
				Limit limit = null;
				
				if( limitNode.Attributes[ "netType" ] == null )
					name = limitNode.Attributes[ "type" ].Value;
				else
					name = limitNode.Attributes[ "netType" ].Value;

				limit = Limit.GetLimitFromName( name );

				if( limit != null )
					limit.License = License;

				if( limit == null || ! limit.LoadFromXml( limitNode ) )
					return false;

				Limits.Add( limit );
			}

			return true;
		}

		/// <summary>
		/// Saves the contents of the Limit as an XmlNode.
		/// </summary>
		/// <param name="parent">
		///		The parent node to save to.
		/// </param>
		/// <returns>
		///		Returns the saved XmlNode.
		/// </returns>
		public override XmlNode SaveToXml( XmlNode parent )
		{
			return SaveToXml( parent, Name );
		}

		/// <summary>
		/// Saves the contents of the Limit as an XmlNode using the given limit name.
		/// </summary>
		/// <param name="limitName">
		///		The Name of the limit.
		/// </param>
		/// <param name="parent">
		///		The parent node to save to.
		/// </param>
		/// <returns>
		///		Returns the saved XmlNode.
		/// </returns>
		protected XmlNode SaveToXml( XmlNode parent, string limitName )
		{
			XmlNode limitNode = BaseSaveToXml( parent, limitName );
			
			foreach( Limit limit in Limits )
			{
				XmlNode innerLimitNode = limit.SaveToXml( limitNode );

				if( limit.GetType().Assembly.FullName != typeof( Limit ).Assembly.FullName )
				{
					XmlAttribute netTypeAttribute = parent.OwnerDocument.CreateAttribute( "netType" );
					netTypeAttribute.Value = String.Format( System.Globalization.CultureInfo.InvariantCulture,"{0}, {1}", limit.GetType().FullName, limit.GetType().Assembly.GetName().Name );
					innerLimitNode.Attributes.Append( netTypeAttribute );
				}
			}

			return limitNode;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class ContainerLimit
	#endregion

	#region PeekResult
	/// <summary>
	///  Possible results from the <see cref="Xheo.Licensing.Limit.Peek"/> method.
	/// </summary>
	public enum PeekResult
	{
		/// <summary>
		/// Unkonwn status.
		/// </summary>
		Unknown		= 0,

		/// <summary>
		/// The limit is valid in it's current state and Validate will return true without
		/// any user interaction.
		/// </summary>
		Valid		= 2,

		/// <summary>
		/// The limit is invalid. Calling Validate may require user interaction.
		/// </summary>
		Invalid		= 4,
		
		/// <summary>
		/// The limit may be valid but will require user interaction to ensure.
		/// </summary>
		NeedsUser	= 8,
	} // End enum PeekResult
	#endregion

} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
