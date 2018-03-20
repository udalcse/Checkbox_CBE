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
// Class:		RegistrationLimit
// Author:		Paul Alexander
// Created:		Sunday, November 17, 2002 6:15:48 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
//using Xheo.Licensing.Design;

namespace Xheo.Licensing
{
#if LICENSING || LICENSEREGISTRATIONS
	/// <summary>
	/// Displays a form where the user can register the license.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class RegistrationLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string				_licenseServerUrl		= null;
		private string				_followUpUrl			= null;
		private string				_privacyPolicyUrl		= null;
		private StringCollection	_alternates				= new StringCollection();
		private string				_logoBitmapResource		= null;
		private int					_reminderPeriod			= 7;
		private string				_requiredFields			= "firstname,lastname";
		private bool				_isMandatory			= false;
		private string				_serialNumberRegex		= null;
		private string				_serialNumberMask		= null;
		private bool				_serialNumberIsAllCaps	= false;

		private bool				_showDetails		= false;
		
		private CustomRegistrationFieldCollection _customFields = new CustomRegistrationFieldCollection();
	
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the RegistrationLimit class.
		/// </summary>
		public RegistrationLimit()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets a value that indicates if the registration is mandatory.
		/// </summary>
#if LICENSING
		[ 
			Category( "Limits" ),
			Description( "Indicates if registration is mandatory." ),
			DefaultValue( false )
		]
#endif
		public bool IsMandatory
		{
			get
			{
				return _isMandatory;
			}
			set
			{
				AssertNotReadOnly();
				_isMandatory = value;
			}
		}
        
		/// <summary>
		/// Gets or sets the number of days between registration reminders.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The number of days between registration reminders." ),
			DefaultValue( 7 )
		]
#endif
		public int ReminderPeriod
		{
			get
			{
				return _reminderPeriod;
			}
			set
			{
				AssertNotReadOnly();
				_reminderPeriod = value;
			}
		}

		/// <summary>
		/// Gets a collection of <see cref="CustomRegistrationField"/> fields to
		/// display on the form.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Collection of custom fields to display on the form." ),
			NotifyParentProperty( true )
		]
#endif
		public CustomRegistrationFieldCollection CustomFields
		{
			get
			{
				if( License != null && License.IsReadOnly )
					_customFields.SetReadOnly();
				return _customFields;
			}
		}

		/// <summary>
		/// Gets the URL to open when the registration is complete.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "A URL to open when the registration is complete." )
		]
#endif
		public string FollowUpUrl
		{
			get
			{
				return _followUpUrl;
			}
			set
			{
				AssertNotReadOnly();
				if( value == null || value.Length == 0 )
					_followUpUrl = null;
				else
					_followUpUrl = new Uri( value ).ToString();
			}
		}

		/// <summary>
		/// Gets the URL where privacy polciy information can be read.
		/// </summary>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "The URL where privacy polciy information can be read." )
		]
#endif
		public string PrivacyPolicyUrl
		{
			get
			{
				return _privacyPolicyUrl;
			}
			set
			{
				AssertNotReadOnly();
				if( value == null || value.Length == 0 )
					_privacyPolicyUrl = null;
				else
					_privacyPolicyUrl = new Uri( value ).ToString();
			}
		}

		/// <summary>
		/// Gets the URL where the license will be registered. 
		/// </summary>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "The URL where the licnse will be registered." )
		]
#endif
		public string LicenseServerUrl
		{
			get
			{
				return _licenseServerUrl;
			}
			set
			{
				AssertNotReadOnly();
				if( value == null || value.Length == 0 )
					_licenseServerUrl = null;
				else
					_licenseServerUrl = new Uri( value ).ToString();
			}
		}

		/// <summary>
		/// Gets a collection of alternate server URLs to use in case a connection to the primary
		/// server cannot be established. If the license is read only then a copy of the collection
		/// is returned and any changes made are lost.
		/// </summary>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "Collection of alternate server URLs to use in case a connection to the primary server cannot be established" ),
		Editor( typeof( Design.StringCollectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )
		]
#endif
		public StringCollection Alternates
		{
			get
			{
				if( License != null && License.IsReadOnly )
				{
					StringCollection readOnlyCollection = new StringCollection();
					foreach( string server in _alternates )
						readOnlyCollection.Add( server );
					return readOnlyCollection;
				}
				return _alternates;
			}
		}

		/// <summary>
		/// Gets or sets the assembly qualified name of a resource to display in the Logo Bitmap
		/// area of the form. Should be 88 x 88. You can also specify a remote JPEG or GIF
		/// image by using a fully qualified URL.
		/// </summary>
		/// <remarks>
		/// Only URLs beginning with http or https can be used.
		/// </remarks>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "The assembly qualified name of the LogoBitmap resource. Example: MyComponent.bmp,MyAssembly." )
		]
#endif
		public string LogoBitmapResource
		{
			get
			{
				return _logoBitmapResource;
			}
			set
			{
				AssertNotReadOnly();
				string vl = value == null ? null : value.ToLower( System.Globalization.CultureInfo.InvariantCulture );
				if( value != null && ( vl.StartsWith( "http://" ) || vl.StartsWith( "https://" ) || vl.StartsWith( "file://" ) ) )
					_logoBitmapResource = new Uri( value ).ToString();
				else
					_logoBitmapResource = value;
			}
		}


		/// <summary>
		/// Gets or sets a comma separated list of required fields.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "A comma separated list of required fields." ),
			DefaultValue( "firstname,lastname" )
		]
#endif
		public string RequiredFields
		{
			get
			{
				return _requiredFields;
			}
			set
			{
				AssertNotReadOnly();
				_requiredFields = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the additional information
		/// should be shown expanded initially.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates if the additional information should be shown expanded initially." ),
			DefaultValue( false )
		]
#endif
		public bool ShowDetails
		{
			get
			{
				return _showDetails;
			}
			set
			{
				AssertNotReadOnly();
				_showDetails = value;
			}
		}

		/// <summary>
		/// Gets or sets a regular expression to use when validating the serial number. This
		/// is not used to protect the license, only help the user from entering a badly
		/// formatted serial number.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "A regular expression to use when validating the serial number. This is not used to protect the license, only help the user from entering a badly formatted serial number." ),
			DefaultValue( null )
		]
#endif
		public string SerialNumberRegex
		{
			get
			{
				return _serialNumberRegex;
			}
			set
			{
				AssertNotReadOnly();
				_serialNumberRegex = value;
			}
		}


		/// <summary>
		/// Gets or sets an input mask to use when formatting the serial number text box. An
		/// X is a value placeholder. Ex: XXXX-XXXX-XXXX-XXXX. The special value * indicates
		/// that any format is valid.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "An input mask to use when formatting the serial number text box. An X is a value placeholder. Ex: XXXX-XXXX-XXXX-XXXX. The special value * indicates that any format is valid." ),
			DefaultValue( "*" )
		]
#endif
		public string SerialNumberMask
		{
			get
			{
				return _serialNumberMask;
			}
			set
			{
				AssertNotReadOnly();
				_serialNumberMask = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the serial number entered should be
		/// converted to all caps automatically.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates whether the serial number entered should be converted to all caps automatically." ),
			DefaultValue( false )
		]
#endif
		public bool SerialNumberIsAllCaps
		{
			get
			{
				return _serialNumberIsAllCaps;
			}
			set
			{
				AssertNotReadOnly();
				_serialNumberIsAllCaps = value;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MRL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Registration";
			}
		}

		/// <summary>
		/// Gets a value that indicates if a GUI is displayed to interact with the user.
		/// </summary>
		public override bool IsGui
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets a value that indicates if this Limit uses remote processes (such as
		/// WebServices) for enforcement.
		/// </summary>
		public override bool IsRemote
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


		internal RegistrationForm MakeForm()
		{
			RegistrationForm form = new RegistrationForm();
			
			if( _logoBitmapResource != null && _logoBitmapResource.Length > 0 )
			{
				Image image = Limit.GetBitmapResource( _logoBitmapResource, this );
				if( image != null )
					form.LogoBitmap = image;
			}
			form.License = this.License;
			form.RequiredFields = _requiredFields;
			if( _licenseServerUrl != null && _licenseServerUrl.Length > 0 )
				form.LicenseServerUrls.Add( _licenseServerUrl );
			foreach( string server in _alternates )
				if( server != null && server.Length > 0 )
					form.LicenseServerUrls.Add( server );
			form.PrivacyPolicyUrl = _privacyPolicyUrl;
			form.FollowUpUrl = _followUpUrl;
			form.GetLicense = ( License.SerialNumber == null || License.SerialNumber.Length == 0 );
			form.IsMandatory = form.IsMandatory | _isMandatory;

			foreach( CustomRegistrationField field in _customFields )
				form.AddCustomField( field.Name, field.DisplayName, field.FieldType );

			if( License.Version >= ExtendedLicense.v2_0 )
			{
				form.ShowDetails				= _showDetails;
				form.SerialNumberIsAllCaps		= _serialNumberIsAllCaps;

				if( _serialNumberMask != null )
					form.SerialNumberMask			= _serialNumberMask;
				form.SerialNumberRegex			= _serialNumberRegex;
			}

			return form;
		}

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
			if( ExtendedLicense.IsWebRequest )
			{
				if( ! ( License.SerialNumber != null && License.SerialNumber.Length > 0 && ! _isMandatory ) )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_MustRegister" );
					return false;
				}

				return true;
			}

			using( RegistrationForm form = MakeForm() )
			{
				if( ! form.IsMandatory && ! form.GetLicense )
				{
					string nextReminder = License.MetaValues[ "RegisterReminder" ];

					try
					{
						if( nextReminder != null && DateTime.Parse( nextReminder, System.Globalization.CultureInfo.InvariantCulture ) > DateTime.UtcNow )
							return true;
					}
					catch{}
				}

				if( ! form.GetLicense && License.MetaValues[ "Registered" ] == "True" )
					return true;

				switch( form.ShowDialog() )
				{
					case DialogResult.OK:
						if( ( form.GetLicense || form.GotLicense) || ! form.License.UnlockBySerial )
							License.MetaValues[ "Registered" ] = "True";
						License._saveOnValid = true;
						//License.LicensePack.Save( true );

						if( form.GetLicense || form.GotLicense )
						{
							License.SurrogateLicensePack = form.SaveLocation;
							License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_ReplacedBy", form.SaveLocation );
							return false;
						}
						else
						{
							if( form.License.UnlockBySerial )
							{
								License.UnlockedLicense = form.License;
								License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_Unlocked" );
								return false;
							}
						}

						return true;
					case DialogResult.Abort:
						if( ! form.IsMandatory )
						{
							License.MetaValues[ "Registered" ] = "True";
							License._saveOnValid = true;
							//License.LicensePack.Save( true );
						}
						return ! form.IsMandatory;
					case DialogResult.Cancel:
						License.MetaValues[ "RegisterReminder" ] = DateTime.Today.AddDays( ReminderPeriod ).ToString( System.Globalization.CultureInfo.InvariantCulture );
						License._saveOnValid = true;
						//License.LicensePack.Save( true );
						if( form.IsMandatory )
							License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_DidNotRegister" );
						return ! form.IsMandatory;
				}

				return false;
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
			if( String.Compare( node.Attributes[ "type" ].Value, "Registration", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( node.Attributes[ "licenseServerUrl" ] != null )
				LicenseServerUrl = node.Attributes[ "licenseServerUrl" ].Value;

			Alternates.Clear();
			foreach( XmlNode alternate in node.SelectNodes( "Alternate" ) )
				Alternates.Add( alternate.InnerText );

			CustomFields.Clear();
			foreach( XmlNode fieldNode in node.SelectNodes( "Field" ) )
			{
				if( fieldNode.Attributes[ "name" ] == null )
					return false;
                if( fieldNode.Attributes[ "type" ] == null )
					return false;
				CustomRegistrationField field = new CustomRegistrationField(
					fieldNode.Attributes[ "name" ].Value,
					null,
					((CustomFieldType)Enum.Parse( typeof( CustomFieldType ), fieldNode.Attributes[ "type" ].Value, true ))  );

				if( fieldNode.Attributes[ "displayName" ] != null )
					field.DisplayName = fieldNode.Attributes[ "displayName" ].Value;

				_customFields.Add( field );
			}

			if( node.Attributes[ "requiredFields" ] != null )
				RequiredFields = node.Attributes[ "requiredFields" ].Value;

			if( node.Attributes[ "followUpUrl" ] != null )
				FollowUpUrl = node.Attributes[ "followUpUrl" ].Value;

			if( node.Attributes[ "privacyPolicyUrl" ] != null )
				PrivacyPolicyUrl = node.Attributes[ "privacyPolicyUrl" ].Value;

			if( node.Attributes[ "logoBitmapResource" ] != null )
				LogoBitmapResource = node.Attributes[ "logoBitmapResource" ].Value;

			if( node.Attributes[ "isMandatory" ] != null )
				IsMandatory = XmlConvert.ToBoolean( node.Attributes[ "isMandatory" ].Value );

			if( node.Attributes[ "showDetails" ] != null )
				ShowDetails = XmlConvert.ToBoolean( node.Attributes[ "showDetails" ].Value );

			if( node.Attributes[ "serialNumberIsAllCaps" ] != null )
				SerialNumberIsAllCaps = XmlConvert.ToBoolean( node.Attributes[ "serialNumberIsAllCaps" ].Value );

			if( node.Attributes[ "serialNumberMask" ] != null )
				SerialNumberMask = node.Attributes[ "serialNumberMask" ].Value;

			if( node.Attributes[ "serialNumberRegex" ] != null )
				SerialNumberRegex = node.Attributes[ "serialNumberRegex" ].Value;

			if( node.Attributes[ "reminderPeriod" ] != null )
				ReminderPeriod	= XmlConvert.ToInt32( node.Attributes[ "reminderPeriod" ].Value );

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
			Version v1_1 = ExtendedLicense.v1_1;

			XmlNode limitNode = parent.OwnerDocument.CreateElement( "Limit" );
			XmlAttribute attribute = parent.OwnerDocument.CreateAttribute( "type" );
			attribute.Value = "Registration";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "requiredFields" );
			attribute.Value = RequiredFields;
			limitNode.Attributes.Append( attribute );

			if( _licenseServerUrl != null && _licenseServerUrl.Length > 0 )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "licenseServerUrl" );
				attribute.Value = LicenseServerUrl;
				limitNode.Attributes.Append( attribute );
			}

			if( _followUpUrl != null && _followUpUrl.Length > 0 )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "followUpUrl" );
				attribute.Value = _followUpUrl;
				limitNode.Attributes.Append( attribute );
			}

			if( _privacyPolicyUrl != null && _privacyPolicyUrl.Length > 0 )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "privacyPolicyUrl" );
				attribute.Value = _privacyPolicyUrl;
				limitNode.Attributes.Append( attribute );
			}

			if( _logoBitmapResource != null && _logoBitmapResource.Length > 0 )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "logoBitmapResource" );
				attribute.Value = _logoBitmapResource;
				limitNode.Attributes.Append( attribute );
			}

			foreach( string alternate in _alternates )
			{
				XmlNode alternateNode = parent.OwnerDocument.CreateElement( "Alternate" );
				alternateNode.InnerText = alternate;
				limitNode.AppendChild( alternateNode );
			}

			foreach( CustomRegistrationField field in _customFields )
			{
				if( field.Name == null || field.Name.Length == 0 )
					throw new ExtendedLicenseException( "E_CustomFieldValidName" );

				XmlNode fieldNode = parent.OwnerDocument.CreateElement( "Field" );
				attribute = parent.OwnerDocument.CreateAttribute( "name" );
				attribute.Value = field.Name;
				fieldNode.Attributes.Append( attribute );
				
				if( field.DisplayName != null && field.DisplayName.Length > 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "displayName" );
					attribute.Value = field.DisplayName;
					fieldNode.Attributes.Append( attribute );
				}

				attribute = parent.OwnerDocument.CreateAttribute( "type" );
                attribute.Value = field.FieldType.ToString();
				fieldNode.Attributes.Append( attribute );

				limitNode.AppendChild( fieldNode );
			}

			if( _isMandatory )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "isMandatory" );
				attribute.Value = XmlConvert.ToString( IsMandatory );
				limitNode.Attributes.Append( attribute );
			}

			if( _reminderPeriod != 7 )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "reminderPeriod" );
				attribute.Value = XmlConvert.ToString( ReminderPeriod );
				limitNode.Attributes.Append( attribute );
			}

			if( License == null || License.Version >= ExtendedLicense.v2_0 )
			{
				if( _showDetails )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "showDetails" );
					attribute.Value = XmlConvert.ToString( ShowDetails );
					limitNode.Attributes.Append( attribute );				
				}

				if( _serialNumberIsAllCaps )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "serialNumberIsAllCaps" );
					attribute.Value = XmlConvert.ToString( SerialNumberIsAllCaps );
					limitNode.Attributes.Append( attribute );				
				}

				if( _serialNumberRegex != null && _serialNumberRegex.Length > 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "serialNumberRegex" );
					attribute.Value = SerialNumberRegex;
					limitNode.Attributes.Append( attribute );				
				}

				if( _serialNumberMask != "*" && _serialNumberMask != null && _serialNumberMask.Length > 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "serialNumberMask" );
					attribute.Value = SerialNumberMask;
					limitNode.Attributes.Append( attribute );				
				}

			}

			parent.AppendChild( limitNode );
			return limitNode;
		}

		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			return String.Format( System.Globalization.CultureInfo.InvariantCulture,"Register license with {0}.", LicenseServerUrl );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
	} // End class RegistrationLimit

	/// <summary>
	/// Represents an entry in the <see cref="RegistrationLimit.CustomFields"/> collection.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class CustomRegistrationField
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private CustomFieldType _fieldType		= CustomFieldType.Text;
		private string			_name		= null;
		private string			_displayName	= null;
		
		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the CustomRegistrationField class.
		/// </summary>
		public CustomRegistrationField()
		{
		}

		/// <summary>
		/// Initializes a new instance of the CustomRegistrationField class.
		/// </summary>
		/// <param name="displayName">
		///		The label or display name for the field. If null uses name.
		/// </param>
		/// <param name="name">
		///		The field name used when sending data to the license server.
		/// </param>
		/// <param name="fieldType">
		///		The CustomFieldType to use.
		/// </param>
		public CustomRegistrationField( string name, string displayName, CustomFieldType fieldType )
		{
			Name	= name;
			DisplayName = displayName;
			FieldType	= fieldType;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the field name of the custom field. When <see cref="FieldType"/>
		/// is <see cref="CustomFieldType.Custom"/> this should be the assembly 
		/// qualified name of a control that implements <see cref="ICustomFormControl"/>.
		/// </summary>
		[ ParenthesizePropertyName( true ) ]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the display name of the custom field. When null uses <see cref="Name"/>.
		/// </summary>
		public string DisplayName
		{
			get
			{
				return _displayName;
			}
			set
			{
				_displayName = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of the custom field.
		/// </summary>
		public CustomFieldType FieldType
		{
			get
			{
				return _fieldType;
			}
			set
			{
				_fieldType = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Overrides the base implementation to return the field name instead.
		/// </summary>
		public override string ToString()
		{
			if( Name != null )
				return Name;
			return base.ToString();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class CustomRegistrationField


	/// <summary>
	/// Implements a strongly typed collection of <see cref="CustomRegistrationField"/>
	/// entries.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class CustomRegistrationFieldCollection : TransientReadOnlyCollection
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the CustomRegistrationFieldCollection class.
		/// </summary>

		public CustomRegistrationFieldCollection()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets a CustomRegistrationField at the given index.
		/// </summary>
		public CustomRegistrationField this[ int index ]
		{
			get
			{
				return List[ index ] as CustomRegistrationField;
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
		/// Adds a new CustomRegistrationField to the collection.
		/// </summary>
		/// <param name="customField">
		///		The CustomRegistrationField to add.
		///		</param>
		public void Add( CustomRegistrationField customField )
		{
			List.Add( customField );
		}

		/// <summary>
		/// Inserts the CustomRegistrationField at the given index.
		/// </summary>
		/// <param name="index">
		///		The index to insert the CustomRegistrationField.
		///		</param>
		///		<param name="customField">
		///			The CustomRegistrationField to insert.
		///			</param>
		public void Insert( int index, CustomRegistrationField customField )
		{
			List.Insert( index, customField );
		}

		/// <summary>
		/// Removes the given index from the list.
		/// </summary>
		/// <param name="customField">
		///		The customField to remove.
		///		</param>
		public void Remove( CustomRegistrationField customField )
		{
			List.Remove( customField );
		}

		/// <summary>
		/// Copies the collection to an array.
		/// </summary>
		/// <param name="array">
		///		The array to copy to.
		///		</param>
		///		<param name="index">
		///			The index in the array to begin copying.
		///			</param>
		public void CopyTo( CustomRegistrationField[] array, int index )
		{
			List.CopyTo( array, index );
		}

		/// <summary>
		/// Gets the index of the given customField.
		/// </summary>
		/// <param name="customField">
		///		The customField to look for.
		///		</param>
		///		<returns>
		///			Returns the index of the given CustomRegistrationField if found, otherwise -1.
		///			</returns>
		public int IndexOf( CustomRegistrationField customField )
		{
			return List.IndexOf( customField );
		}

		/// <summary>
		/// Determines if the collection contains the given CustomRegistrationField.
		/// </summary>
		/// <param name="customField">
		///		The CustomRegistrationField to look for.
		///		</param>
		///		<returns>
		///			Returns true if the CustomRegistrationField is found, otherwise false.
		///			</returns>
		public bool Contains( CustomRegistrationField customField )
		{
			return List.Contains( customField );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class CustomRegistrationFieldCollection
#endif
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////

