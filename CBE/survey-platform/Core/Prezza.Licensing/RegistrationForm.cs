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
// Class:		RegistrationForm
// Author:		Paul Alexander
// Created:		Thursday, February 20, 2003 10:47:15 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Xheo.Licensing
{
#if LICENSING
	/// <summary>
	/// Displays a simple registration form and sends registration information to
	/// a <see cref="LicenseServer"/>.
	/// </summary>
	public 
#else
	internal
#endif
		class RegistrationForm : System.Windows.Forms.Form
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.GroupBox _contactInfo;
		private System.Windows.Forms.Button _details;
		private System.Windows.Forms.Panel _infoPanel;
		private System.Windows.Forms.TextBox _organization;
		private System.Windows.Forms.TextBox _first;
		private System.Windows.Forms.TextBox _middle;
		private System.Windows.Forms.TextBox _last;
		private System.Windows.Forms.Button _remind;
		private System.Windows.Forms.Panel _buttonPanel;
		private System.Windows.Forms.ProgressBar _registering;
		private System.Windows.Forms.Button _cancel;
		
		private ExtendedLicense _license	= null;
		private bool	_getLicense			= false;
		private bool	_sendToWebPage		= true;
		private StringCollection	_licenseServerUrls	= new StringCollection();
		private string	_privacyPolicyUrl			= null;
		private string	_followUpUrl		= null;
		private string	_requiredFields		= null;
		private string	_saveLocation		= null;
		private Type	_licensedType		= null;
		private bool	_canceled			= false;
		private System.Windows.Forms.Panel _customFields;
		private System.Windows.Forms.LinkLabel _privacy;
		private bool	_working			= false;
		private System.Windows.Forms.PictureBox _logo;
		private bool	_shown				= false;
		private System.Windows.Forms.SaveFileDialog _licenseSFD;
		private System.Windows.Forms.Label _organizationLabel;
		private System.Windows.Forms.Label _nameLabel;
		private System.Windows.Forms.Label _serialNumberLabel;
		private System.Windows.Forms.Label _connectionWarning;
		private bool	_gotLicense			= false;
		private System.Windows.Forms.LinkLabel _alreadyHaveLink;

		
		private string	_serialNumberMask	= null;
		private bool	_serialNumberIsAllCaps	= false;
		private string	_serialNumberRegex	= null; // @"^((TRIAL)|(\d{8}-\d{4}-\d{4}-\d{16}))$";
		private string	_serialNumber		= null;
		private System.Windows.Forms.Panel _serialNumberPanel;
		private System.Windows.Forms.LinkLabel _proxyInfo;

		private Bitmap _logoBitmap			= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// Initializes a new instance of the RegistrationForm class.
		public RegistrationForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_logoBitmap = Limit.GetBitmapResource( Limit.MakeResourceString( "Registration-Logo.gif", typeof( RegistrationForm ) ), null );
			LogoBitmap = _logoBitmap;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( _logo != null )
					_logo.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._logo = new System.Windows.Forms.PictureBox();
			this._ok = new System.Windows.Forms.Button();
			this._contactInfo = new System.Windows.Forms.GroupBox();
			this._customFields = new System.Windows.Forms.Panel();
			this._details = new System.Windows.Forms.Button();
			this._infoPanel = new System.Windows.Forms.Panel();
			this._serialNumberPanel = new System.Windows.Forms.Panel();
			this._alreadyHaveLink = new System.Windows.Forms.LinkLabel();
			this._organization = new System.Windows.Forms.TextBox();
			this._organizationLabel = new System.Windows.Forms.Label();
			this._first = new System.Windows.Forms.TextBox();
			this._nameLabel = new System.Windows.Forms.Label();
			this._middle = new System.Windows.Forms.TextBox();
			this._last = new System.Windows.Forms.TextBox();
			this._serialNumberLabel = new System.Windows.Forms.Label();
			this._connectionWarning = new System.Windows.Forms.Label();
			this._remind = new System.Windows.Forms.Button();
			this._buttonPanel = new System.Windows.Forms.Panel();
			this._privacy = new System.Windows.Forms.LinkLabel();
			this._registering = new System.Windows.Forms.ProgressBar();
			this._cancel = new System.Windows.Forms.Button();
			this._licenseSFD = new System.Windows.Forms.SaveFileDialog();
			this._proxyInfo = new System.Windows.Forms.LinkLabel();
			this._contactInfo.SuspendLayout();
			this._infoPanel.SuspendLayout();
			this._buttonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _logo
			// 
			this._logo.Location = new System.Drawing.Point(8, 8);
			this._logo.Name = "_logo";
			this._logo.Size = new System.Drawing.Size(88, 88);
			this._logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this._logo.TabIndex = 0;
			this._logo.TabStop = false;
			// 
			// _ok
			// 
			this._ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._ok.Location = new System.Drawing.Point(152, 0);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(112, 24);
			this._ok.TabIndex = 0;
			this._ok.Text = "#UIR_Register";
			this._ok.Click += new System.EventHandler(this._ok_Click);
			// 
			// _contactInfo
			// 
			this._contactInfo.Controls.Add(this._customFields);
			this._contactInfo.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._contactInfo.Location = new System.Drawing.Point(8, 128);
			this._contactInfo.Name = "_contactInfo";
			this._contactInfo.Size = new System.Drawing.Size(504, 200);
			this._contactInfo.TabIndex = 2;
			this._contactInfo.TabStop = false;
			this._contactInfo.Text = "#UIR_AdditionalInformation";
			this._contactInfo.Visible = false;
			// 
			// _customFields
			// 
			this._customFields.AutoScroll = true;
			this._customFields.Dock = System.Windows.Forms.DockStyle.Fill;
			this._customFields.DockPadding.All = 8;
			this._customFields.Location = new System.Drawing.Point(3, 16);
			this._customFields.Name = "_customFields";
			this._customFields.Size = new System.Drawing.Size(498, 181);
			this._customFields.TabIndex = 0;
			// 
			// _details
			// 
			this._details.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._details.Location = new System.Drawing.Point(8, 102);
			this._details.Name = "_details";
			this._details.Size = new System.Drawing.Size(88, 23);
			this._details.TabIndex = 1;
			this._details.Text = "#UIR_More";
			this._details.Click += new System.EventHandler(this._details_Click);
			// 
			// _infoPanel
			// 
			this._infoPanel.Controls.Add(this._proxyInfo);
			this._infoPanel.Controls.Add(this._serialNumberPanel);
			this._infoPanel.Controls.Add(this._alreadyHaveLink);
			this._infoPanel.Controls.Add(this._organization);
			this._infoPanel.Controls.Add(this._organizationLabel);
			this._infoPanel.Controls.Add(this._first);
			this._infoPanel.Controls.Add(this._nameLabel);
			this._infoPanel.Controls.Add(this._middle);
			this._infoPanel.Controls.Add(this._last);
			this._infoPanel.Controls.Add(this._serialNumberLabel);
			this._infoPanel.Controls.Add(this._connectionWarning);
			this._infoPanel.Location = new System.Drawing.Point(104, 8);
			this._infoPanel.Name = "_infoPanel";
			this._infoPanel.Size = new System.Drawing.Size(408, 120);
			this._infoPanel.TabIndex = 0;
			// 
			// _serialNumberPanel
			// 
			this._serialNumberPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._serialNumberPanel.Location = new System.Drawing.Point(0, 96);
			this._serialNumberPanel.Name = "_serialNumberPanel";
			this._serialNumberPanel.Size = new System.Drawing.Size(408, 24);
			this._serialNumberPanel.TabIndex = 18;
			// 
			// _alreadyHaveLink
			// 
			this._alreadyHaveLink.Location = new System.Drawing.Point(208, 80);
			this._alreadyHaveLink.Name = "_alreadyHaveLink";
			this._alreadyHaveLink.Size = new System.Drawing.Size(200, 16);
			this._alreadyHaveLink.TabIndex = 17;
			this._alreadyHaveLink.TabStop = true;
			this._alreadyHaveLink.Text = "#UI_AlreadyHaveLicense";
			this._alreadyHaveLink.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._alreadyHaveLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._alreadyHaveLink_LinkClicked);
			// 
			// _organization
			// 
			this._organization.Location = new System.Drawing.Point(0, 56);
			this._organization.Name = "_organization";
			this._organization.Size = new System.Drawing.Size(408, 20);
			this._organization.TabIndex = 5;
			this._organization.Text = "";
			// 
			// _organizationLabel
			// 
			this._organizationLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._organizationLabel.Location = new System.Drawing.Point(0, 40);
			this._organizationLabel.Name = "_organizationLabel";
			this._organizationLabel.Size = new System.Drawing.Size(176, 23);
			this._organizationLabel.TabIndex = 4;
			this._organizationLabel.Text = "#UIR_Organization";
			// 
			// _first
			// 
			this._first.Location = new System.Drawing.Point(0, 16);
			this._first.Name = "_first";
			this._first.Size = new System.Drawing.Size(128, 20);
			this._first.TabIndex = 1;
			this._first.Text = "";
			// 
			// _nameLabel
			// 
			this._nameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._nameLabel.Location = new System.Drawing.Point(0, 0);
			this._nameLabel.Name = "_nameLabel";
			this._nameLabel.Size = new System.Drawing.Size(80, 20);
			this._nameLabel.TabIndex = 0;
			this._nameLabel.Text = "#UIR_Name";
			// 
			// _middle
			// 
			this._middle.Location = new System.Drawing.Point(136, 16);
			this._middle.Name = "_middle";
			this._middle.Size = new System.Drawing.Size(56, 20);
			this._middle.TabIndex = 2;
			this._middle.Text = "";
			// 
			// _last
			// 
			this._last.Location = new System.Drawing.Point(200, 16);
			this._last.Name = "_last";
			this._last.Size = new System.Drawing.Size(208, 20);
			this._last.TabIndex = 3;
			this._last.Text = "";
			// 
			// _serialNumberLabel
			// 
			this._serialNumberLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._serialNumberLabel.Location = new System.Drawing.Point(0, 80);
			this._serialNumberLabel.Name = "_serialNumberLabel";
			this._serialNumberLabel.Size = new System.Drawing.Size(176, 23);
			this._serialNumberLabel.TabIndex = 6;
			this._serialNumberLabel.Text = "#UI_SN";
			// 
			// _connectionWarning
			// 
			this._connectionWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._connectionWarning.ForeColor = System.Drawing.SystemColors.ControlDark;
			this._connectionWarning.Location = new System.Drawing.Point(80, 0);
			this._connectionWarning.Name = "_connectionWarning";
			this._connectionWarning.Size = new System.Drawing.Size(272, 20);
			this._connectionWarning.TabIndex = 16;
			this._connectionWarning.Text = "#UIR_ConnectionWarning";
			this._connectionWarning.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _remind
			// 
			this._remind.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this._remind.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._remind.Location = new System.Drawing.Point(272, 0);
			this._remind.Name = "_remind";
			this._remind.Size = new System.Drawing.Size(112, 24);
			this._remind.TabIndex = 1;
			this._remind.Text = "#UIR_DontRegister";
			// 
			// _buttonPanel
			// 
			this._buttonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._buttonPanel.Controls.Add(this._ok);
			this._buttonPanel.Controls.Add(this._remind);
			this._buttonPanel.Controls.Add(this._privacy);
			this._buttonPanel.Location = new System.Drawing.Point(8, 337);
			this._buttonPanel.Name = "_buttonPanel";
			this._buttonPanel.Size = new System.Drawing.Size(506, 24);
			this._buttonPanel.TabIndex = 3;
			// 
			// _privacy
			// 
			this._privacy.Location = new System.Drawing.Point(0, 0);
			this._privacy.Name = "_privacy";
			this._privacy.Size = new System.Drawing.Size(168, 24);
			this._privacy.TabIndex = 16;
			this._privacy.TabStop = true;
			this._privacy.Text = "#UIR_Privacy";
			this._privacy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._privacy.Visible = false;
			this._privacy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._privacy_LinkClicked);
			// 
			// _registering
			// 
			this._registering.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._registering.Location = new System.Drawing.Point(8, 340);
			this._registering.Name = "_registering";
			this._registering.Size = new System.Drawing.Size(144, 17);
			this._registering.TabIndex = 14;
			this._registering.Visible = false;
			// 
			// _cancel
			// 
			this._cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cancel.Location = new System.Drawing.Point(402, 337);
			this._cancel.Name = "_cancel";
			this._cancel.Size = new System.Drawing.Size(112, 24);
			this._cancel.TabIndex = 4;
			this._cancel.Text = "#UIR_RemindMeLater";
			this._cancel.Click += new System.EventHandler(this._cancel_Click);
			// 
			// _licenseSFD
			// 
			this._licenseSFD.DefaultExt = "lic";
			this._licenseSFD.FileName = "doc1";
			this._licenseSFD.Filter = "License Files (*.lic)|*.lic|All Files (*.*)|*.*";
			// 
			// _proxyInfo
			// 
			this._proxyInfo.Location = new System.Drawing.Point(352, 0);
			this._proxyInfo.Name = "_proxyInfo";
			this._proxyInfo.Size = new System.Drawing.Size(56, 16);
			this._proxyInfo.TabIndex = 19;
			this._proxyInfo.TabStop = true;
			this._proxyInfo.Text = "#UI_ProxyInfo";
			this._proxyInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this._proxyInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._proxyInfo_LinkClicked);
			// 
			// RegistrationForm
			// 
			this.AcceptButton = this._ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(522, 368);
			this.ControlBox = false;
			this.Controls.Add(this._details);
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._registering);
			this.Controls.Add(this._buttonPanel);
			this.Controls.Add(this._infoPanel);
			this.Controls.Add(this._contactInfo);
			this.Controls.Add(this._logo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RegistrationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "#UIR_Title";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.RegistrationForm_Closing);
			this._contactInfo.ResumeLayout(false);
			this._infoPanel.ResumeLayout(false);
			this._buttonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the image to display in the logo box.
		/// </summary>
		public Image LogoBitmap
		{
			get
			{
				return _logo.Image;
			}
			set
			{
				_logo.Image = value;
			}
		}


		/// <summary>
		/// Gets or sets the License being registered. Emtpy fields will be pre-filled
		/// with values from the license.
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

				if( SerialNumber == null || SerialNumber.Length == 0 )
					SerialNumber = _license.SerialNumber;
				if( Organization == null || Organization.Length == 0 )
					Organization = _license.Organization;
				if( _license.User != null )
				{
					if( FirstName == null || FirstName.Length == 0 )
					{
						int index = _license.User.IndexOf( ' ' );
						FirstName = index > -1 ? _license.User.Substring( 0, index ) : _license.User;
					}
					if( LastName == null || LastName.Length == 0 )
					{
						int index = _license.User.LastIndexOf( ' ' );
						if( index > -1 )
							LastName = _license.User.Substring( index + 1 );
					}
					if( MiddleName == null || MiddleName.Length == 0 )
					{
						int index	= _license.User.IndexOf( ' ' );
						int lindex	= _license.User.LastIndexOf( ' ' );
						if( index > -1 && index != lindex )
							MiddleName = _license.User.Substring( index + 1, lindex - index - 1 );
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the Type of the class being registered.
		/// </summary>
		public Type LicensedType
		{
			get
			{
				if( _licensedType == null )
					return License == null ? null : License.LicensedType;
				return _licensedType;
			}
			set
			{
				_licensedType = value;
			}
		}

		/// <summary>
		/// Gets or sets the URL where privacy information is available.
		/// </summary>
		public string PrivacyPolicyUrl
		{
			get
			{
				return _privacyPolicyUrl;
			}
			set
			{
				_privacyPolicyUrl = value;
				if( value == null || value.Length == 0 )
				{
					_privacy.Visible = false;
				}
				else
				{
					_privacy.Visible = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the first name of the registrant.
		/// </summary>
		public string FirstName
		{
			get
			{
				return _first.Text;
			}
			set
			{
				_first.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the Last name of the registrant.
		/// </summary>
		public string LastName
		{
			get
			{
				return _last.Text;
			}
			set
			{
				_last.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the Middle name of the registrant.
		/// </summary>
		public string MiddleName
		{
			get
			{
				return _middle.Text;
			}
			set
			{
				_middle.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the organization of the registrant.
		/// </summary>
		public string Organization
		{
			get
			{
				return _organization.Text;
			}
			set
			{
				_organization.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the serial number of the license to be registered.
		/// </summary>
		public string SerialNumber
		{
			get
			{
				if( _shown )
				{
					StringBuilder sn = new StringBuilder();
					foreach( TextBox tb in _serialNumberPanel.Controls )
					{
						if( sn.Length > 0 )
							sn.Append( '-' );
						sn.Append( tb.Text );
					}
					return sn.ToString();
				}
				return _serialNumber;
			}
			set
			{
				if( value == null || value.Length == 0 )
					IsMandatory = true;
				if( _shown )
				{
					if( SerialNumberIsAllCaps && value != null )
						value = value.ToUpper( System.Globalization.CultureInfo.CurrentCulture );
						
					string[] sn = value == null ? new string[0] : value.Split( '-' );
					for( int index = 0; index < _serialNumberPanel.Controls.Count; index++ )
					{
						if( index < sn.Length )
							_serialNumberPanel.Controls[ index ].Text = sn[ index ];
						else
							_serialNumberPanel.Controls[ index ].Text = "";
					}

					if( sn.Length > _serialNumberPanel.Controls.Count )
					{
						int count = _serialNumberPanel.Controls.Count;
						string[] remaining = new string[ sn.Length - count ];
						Array.Copy( sn, sn.Length - remaining.Length, remaining, 0, remaining.Length );
						_serialNumberPanel.Controls[ _serialNumberPanel.Controls.Count - 1 ].Text += '-' + String.Join( "-", remaining );
					}
				}
				else
				{
					_serialNumber = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the serial number mask used to provide a cleaner interface for
		/// the end user.
		/// </summary>
		/// <remarks>
		///		The serial number mask identifies character placeholders and character
		///		units (separated by dashes). The character pattern is then used to create
		///		a set of input boxes that mimic pattern. A sample pattern would be
		///		XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX for a GUID.
		/// </remarks>
		public string SerialNumberMask
		{
			get
			{
				return _serialNumberMask;
			}
			set
			{
				if( _shown )
					throw new ExtendedLicenseException( "E_CannotChangeMask" );
				if( value != null && value.Length > 0 && ! Regex.IsMatch( value, @"^(\*)|((X+-)*X?)$" ) )
					throw new ArgumentOutOfRangeException( "value" );
				_serialNumberMask = value;
			}
		}

		/// <summary>
		/// Gets or sets a regular expression used to validate the format of the serial
		/// number. This is not used to secure the application, only to provide immediate
		/// feedback to the user if they have typed the number wrong.
		/// </summary>
		public string SerialNumberRegex
		{
			get
			{
				return _serialNumberRegex;
			}
			set
			{
				_serialNumberRegex = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the serial number contains only capital
		/// letters.
		/// </summary>
		public bool SerialNumberIsAllCaps
		{
			get
			{
				return _serialNumberIsAllCaps;
			}
			set
			{
				_serialNumberIsAllCaps = value;
				if( value )
					SerialNumber = SerialNumber.ToUpper( System.Globalization.CultureInfo.CurrentCulture );
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if a license should be requested from the
		/// License server defined by <see cref="LicenseServerUrls"/>.
		/// </summary>
		/// <remarks>
		///		When true the user can enter a serial number to register their product. When
		///		the user selects OK, a request will be made to the license server to get
		///		the license indicated by the serial number. There are no semantics placed
		///		on the value of the serial number and developers are free to develop their
		///		own numbering schemes.
		/// </remarks>
		public bool GetLicense
		{
			get
			{
				return _getLicense;
			}
			set
			{
				_getLicense = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if a license was downloaded during the 
		/// registration process. <see cref="SaveLocation"/> contains the path where the
		/// license was saved.
		/// </summary>
		/// <remarks>
		///		Even when GetLicense is false, if the license server returns an XML license
		///		it will be saved. 
		/// </remarks>
		public bool GotLicense
		{
			get
			{
				return _gotLicense;
			}
			set
			{
				_gotLicense = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if a license should be requested from the
		/// License server defined by <see cref="LicenseServerUrls"/>.
		/// </summary>
		/// <remarks>
		///		When true the values of the form will be sent in a query string to the
		///		web page defined by <see cref="FollowUpUrl"/>. When false the information
		///		is sent directly to the license server.
		/// </remarks>
		public bool SendToWebPage
		{
			get
			{
				return _sendToWebPage;
			}
			set
			{
				_sendToWebPage = value;
			}
		}

		/// <summary>
		/// Gets a collection of License Server URLs where the license can be
		/// registered. Only the first valid server is used when registering.
		/// </summary>
		public StringCollection LicenseServerUrls
		{
			get
			{
				return _licenseServerUrls;
			}
		}

		/// <summary>
		/// Gets or sets the URL to open after a successful registration. If <see cref="SendToWebPage"/>
		/// is true, the values of the form are included in the request.
		/// </summary>
		public string FollowUpUrl
		{
			get
			{
				return _followUpUrl;
			}
			set
			{
				_followUpUrl = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the detailed contact information
		/// should be displayed.
		/// </summary>
		public bool ShowDetails
		{
			get
			{
				return _contactInfo.Visible;
			}
			set
			{
				_contactInfo.Visible = value;
				SizeToDetails();
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the Details toggle button is available.
		/// </summary>
		public bool DetailsToggleAvailable
		{
			get
			{
				return _details.Visible;
			}
			set
			{
				_details.Visible = false;
			}
		}

		/// <summary>
		/// Gets or sets a comma separated list of required fields.
		/// </summary>
		/// <remarks>
		/// The available field names are: FirstName, LastName, MiddleName, Organization,
		/// SerialNumber, Email, Phone and Address. When the user selects OK any field
		/// in this list must be present or they receive a prompt. More sophisticated
		/// validation must be performed at the server.
		/// </remarks>
		public string RequiredFields
		{
			get
			{
				return _requiredFields;
			}
			set
			{
				_requiredFields = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if they registration is mandatory. The cancel
		/// button is disabled and if they attempt to close they are warned that they
		/// must register.
		/// </summary>
		public bool IsMandatory
		{
			get
			{
				return ! _remind.Enabled;
			}
			set
			{
				if( value )
				{
					_cancel.Text	= Internal.StaticResourceProvider.CurrentProvider.GetString( "UI_Cancel" );
					_ok.SetBounds( 272, 0, -1, -1, BoundsSpecified.Location );
					_remind.Enabled = false;
					_remind.Visible = false;
				}
				else
				{
					_cancel.Text	= Internal.StaticResourceProvider.CurrentProvider.GetString( "UIR_RemindMeLater" );
					_ok.SetBounds( 152, 0, -1, -1, BoundsSpecified.Location );
					_remind.Enabled = true;
					_remind.Visible = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets the location where the downloaded license should be saved.
		/// </summary>
		/// <remarks>
		/// When <see cref="GetLicense"/> is true, this specifies the location where
		/// the generated license pack should be saved to on success.7
		/// </remarks>
		public string SaveLocation
		{
			get
			{
				return _saveLocation;
			}
			set
			{
				_saveLocation = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Adds a custom field to the ContactInfo section of the form.
		/// </summary>
		/// <param name="displayName">
		///		The name display name of the field. If null or empty uses the fieldName.
		/// </param>
		/// <param name="fieldName">
		///		The name of the field. This is used as the key in the values collection
		///		sent to the registration server and included in the query string when
		///		sending values to a web page. It should not contain any special characters
		///		or spaces.
		/// </param>
		/// <param name="fieldType">
		///		The type of field to display.
		/// </param>
		public void AddCustomField( string fieldName, string displayName, CustomFieldType fieldType )
		{
			if( _shown )
				throw new ExtendedLicenseException( "ER_CannotAddCustomFieldAfterLoad" );

			Panel panel = new Panel();
			panel.Name = fieldName;
			Label label = new Label();
			TextBox text = new TextBox();
			CheckBox box = new CheckBox();
			label.Text = ( displayName == null || displayName.Length == 0 ) ? fieldName : displayName;
			box.Text = ( displayName == null || displayName.Length == 0 ) ? fieldName : displayName;
			text.Name = fieldName;
			box.Name = fieldName;
			
			switch( fieldType )
			{
				case CustomFieldType.Text:
					text.Dock = DockStyle.Top;
					panel.Controls.Add( text );

					label.Dock = DockStyle.Top;
					label.Height = Font.Height;
					panel.Controls.Add( label );
					panel.Height = text.Height + label.Height + 8;

					break;
				case CustomFieldType.Password:
					text.Dock = DockStyle.Top;
					text.PasswordChar = '*';
					panel.Controls.Add( text );

					label.Dock = DockStyle.Top;
					label.Height = Font.Height;
					panel.Controls.Add( label );
					panel.Height = text.Height + label.Height + 8;

					break;
				case CustomFieldType.MultiLine:

					text.Dock		= DockStyle.Top;
					text.Multiline	= true;
					text.ScrollBars = ScrollBars.Both;
					text.AcceptsReturn = true;
					text.Height		= 60;
					panel.Controls.Add( text );

					label.Dock = DockStyle.Top;
					label.Height = Font.Height;
					panel.Controls.Add( label );

					panel.Height = text.Height + label.Height + 8;
					break;
				case CustomFieldType.Checkbox:
					box.Height = Font.Height + 8;
					box.Dock = DockStyle.Top;
					box.FlatStyle = FlatStyle.System;
					
					panel.Controls.Add( box );
					panel.Height = box.Height + 8;
					break;
				case CustomFieldType.Header:

					label.Font = new Font( label.Font, FontStyle.Bold );
					label.Left = 4;
					label.Top = 8;
					label.AutoSize = true;
					panel.Height = label.Height + 16;
					panel.Controls.Add( label );

					Line line = new Line();
					line.Width = 500;
					line.Top = ( label.Font.Height / 2 ) + 8;
					line.Height = 2;
					panel.Controls.Add( line );
					break;
				case CustomFieldType.Address:
					Control addressControl = new AddressCustomFormControl();
					panel.Height = addressControl.Height;
					panel.Controls.Add( addressControl );
					break;
				case CustomFieldType.Custom:
					Type type = Type.GetType( fieldName, true, true );
					Control control = Activator.CreateInstance( type ) as Control;
					panel.Height = control.Height;
					panel.Controls.Add( control );
					break;
			}

			panel.Dock = DockStyle.Top;
			_customFields.Controls.Add( panel );
			panel.BringToFront();
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( panel );
		}

		/// <summary>
		/// Overrides the OnLoad method.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			if( ! IsMandatory && ( SerialNumber == null || SerialNumber.Length == 0 ) )
				IsMandatory = true;
			if( ! _customFields.HasChildren )
				this.DetailsToggleAvailable = false;

			if( RequiredFields != null && RequiredFields.Length > 0 )
			{
				foreach( string field in RequiredFields.ToLower( System.Globalization.CultureInfo.InvariantCulture ).Split( ',' ) )
					if( field != "firstname" &&
						field != "lastname" &&
						field != "middlename" &&
						field != "serialnumber" &&
						field != "organization"
						)
					{
						_contactInfo.Visible = true;
						break;						
					}
			}
			if( License == null 
#if LICENSING || LICENSETRIALS
				|| ! License.IsTrial 
#endif
				)
				_alreadyHaveLink.Visible = false;
			
			if( SerialNumberMask == null || SerialNumberMask.Length == 0 )
				SerialNumberMask = "*";

			string[] mask = SerialNumberMask.Split( '-' );
			int len		= 0;
			int width	= _serialNumberPanel.Width - ( ( mask.Length - 1 ) * 5 );
			foreach( string part in mask )
				len += part.Length;

			int left = 0;
			for( int index = 0; index < mask.Length; index ++ )
			{
				string part = mask[ index ];
				TextBox tb = new TextBox();
				tb.Font = new Font( "Courier New", 10 );
				tb.Left		= left;
				if( index == mask.Length - 1 )
					tb.Width	= _serialNumberPanel.Width - left;
				else
					tb.Width	= (int)( width * (double)part.Length / len );
				left += tb.Width + 5;
				_serialNumberPanel.Controls.Add( tb );
				if( SerialNumberMask != "*" )
					tb.MaxLength = part.Length;
				if( SerialNumberIsAllCaps )
					tb.CharacterCasing = CharacterCasing.Upper;
				tb.TextChanged += new EventHandler(tb_TextChanged);
			}

			if( LicenseServerUrls.Count == 0 )
			{
				_proxyInfo.Visible = false;
				_connectionWarning.Left += _proxyInfo.Width;

				if( _followUpUrl == null || _followUpUrl.Length == 0 )
					_connectionWarning.Visible = false;
			}

			SizeToDetails();
			_first.Focus();
			_shown = true;
			SerialNumber = _serialNumber;
			_first.Select();
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );

			if( LicensedType != null )
			{
				SupportInfo info = LicenseHelpAttribute.GetSupportInfo( LicensedType );
				Text += " - " + info.Product;
			}
		}

		private void SizeToDetails()
		{
			if( _contactInfo.Visible )
			{
				ClientSize = new Size( ClientSize.Width, _contactInfo.Location.Y + _contactInfo.Height + _buttonPanel.Height + 16 );
				_details.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UIR_Less" );
				
			}
			else
			{
				ClientSize = new Size( ClientSize.Width, _infoPanel.Location.Y + _infoPanel.Size.Height + _buttonPanel.Height + 12 );
				_details.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UIR_More" );
				_first.Focus();
			}
			CenterToScreen();
		}

		

		/// <summary>
		/// Initializes the fields on the registration form from the name value collection.
		/// </summary>
		/// <param name="values">
		///		Collection of name value pairs used to initialize the form.
		/// </param>
		/// <remarks>
		///		When setting values for a complex field that has child fields use dot
		///		notation to indicate child fields. For example address.street.
		/// </remarks>
		public void InitializeFields( NameValueCollection values )
		{
			if( values == null )
				return;

			if( values[ "firstname" ] != null )
				FirstName		= values[ "firstname" ];
			if( values[ "lastname" ] != null )
				LastName		= values[ "lastname" ];
			if( values[ "middlename" ] != null )
				MiddleName		= values[ "middlename" ];
			if( values[ "serialnumber" ] != null )
				SerialNumber	= values[ "serialnumber" ];
			if( values[ "organization" ] != null )
				Organization	= values[ "organization" ];
			
			foreach( Panel panel in _customFields.Controls )
			{
				if( panel.Controls[ 0 ] is ICustomFormControl )
				{					
					((ICustomFormControl)panel.Controls[ 0 ]).InitializeFields( panel.Name + ".", values );
				}
				else 
				{
					if( panel.Controls[ 0 ] is CheckBox )
					{
						if( values[ panel.Name ] != null )
							((CheckBox)panel.Controls[ 0 ]).Checked = Convert.ToBoolean( values[ panel.Name ], System.Globalization.CultureInfo.InvariantCulture );
					}
					else
					{
						if( values[ panel.Name ] != null )
							((TextBox)panel.Controls[ 0 ]).Text = values[ panel.Name ];
					}
				}
			}
		}

		private void _details_Click(object sender, System.EventArgs e)
		{
			_contactInfo.Visible = ! _contactInfo.Visible;
			SizeToDetails();
		}

		private void _ok_Click(object sender, System.EventArgs e)
		{
			if( GetLicense && LicensedType == null )
				throw new ExtendedLicenseException( "ER_MustSetLicenseTypeToGetLicense" );

			if( RequiredFields != null && RequiredFields.Length > 0 )
			{
				foreach( string field in RequiredFields.ToLower( System.Globalization.CultureInfo.InvariantCulture ).Split( ',' ) )
				{
					string message = null;
					switch( field )
					{
						case "firstname":
							if( FirstName == null || FirstName.Length == 0 )
							{
								message = Internal.StaticResourceProvider.CurrentProvider.GetString( "ER_FirstNameRequired" );
								_first.Focus();
							}
							break;
						case "lastname":
							if( LastName == null || LastName.Length == 0 )
							{
								message = Internal.StaticResourceProvider.CurrentProvider.GetString( "ER_LastNameRequired" );
								_last.Focus();
							}
							break;
						case "middlename":
							if( MiddleName == null || MiddleName.Length == 0 )
							{
								message = Internal.StaticResourceProvider.CurrentProvider.GetString( "ER_MiddleNameRequired" );
								_middle.Focus();
							}
							break;
						case "serialnumber":
							if( SerialNumber == null || SerialNumber.Length == 0 )
							{
								message = Internal.StaticResourceProvider.CurrentProvider.GetString( "ER_SerialNumberRequired" );
								_serialNumberPanel.Focus();
							}
							break;
						case "organization":
							if( Organization == null || Organization.Length == 0 )
							{
								message = Internal.StaticResourceProvider.CurrentProvider.GetString( "ER_OrganizationRequired" );
								_organization.Focus();
							}
							break;
						default:
							foreach( Panel panel in _customFields.Controls )
							{
								if( panel.Controls[ 0 ] is ICustomFormControl )
								{
									if( ! ((ICustomFormControl)panel.Controls[ 0 ]).ValidateField( field ) )
									{
										_contactInfo.Visible = true;
										SizeToDetails();
										Focus();
										return;
									}
								}
								else if( field == panel.Controls[ 0 ].Name.ToLower( System.Globalization.CultureInfo.InvariantCulture ) )
								{
									if( panel.Controls[ 0 ] is CheckBox )
									{
										if( ! ((CheckBox)panel.Controls[ 0 ]).Checked )
										{
											message = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "ER_YouMustSelect", panel.Controls[ 0 ].Text );
											_contactInfo.Visible = true;
											SizeToDetails();
											panel.Controls[ 0 ].Focus();
										}
									}
									else
									{
										if( ((TextBox)panel.Controls[ 0 ]).Text == null || ((TextBox)panel.Controls[ 0 ]).Text.Length == 0 )
										{
											message = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "ER_YouMustEnterAValueFor", panel.Controls[ 1 ].Text );
											_contactInfo.Visible = true;
											SizeToDetails();
											panel.Controls[ 0 ].Focus();
										}
									}
								}
							}
							break;
					}

					if( message != null )
					{
						MessageBox.Show(
							this,
							message,
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error );
						return;
					}
				}
			}

			if( GetLicense && ( SerialNumber == null || SerialNumber.Length == 0 ) )
			{
				MessageBox.Show(
					this,
					Internal.StaticResourceProvider.CurrentProvider.GetString( "ER_SerialNumberRequired" ),
					"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				_serialNumberPanel.Controls[ 0 ].Select();
				this.BringToFront();
				return;
			}

			if( SerialNumberRegex != null && SerialNumberRegex.Length > 0 && ! Regex.IsMatch( SerialNumber, SerialNumberRegex ) )
			{
				MessageBox.Show(
					this,
					Internal.StaticResourceProvider.CurrentProvider.GetString( "E_InvalidSerialNumber" ),
					"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				_serialNumberPanel.Controls[ 0 ].Select();
				this.BringToFront();
				return;
			}

			ExtendedLicense unlockedLicense = null;
			// Unlock by serial number
			if( License != null && License.PublicKey != null )
			{
				bool containsUnlock = false;
				unlockedLicense = null;
				foreach( ExtendedLicense license in License.LicensePack )
				{
					if( license.SerialNumber == License.SerialNumber )
						continue;

					if( ! license.UnlockBySerial )
						continue;

					containsUnlock = true;

					if( SerialNumber.StartsWith( license.AbsoluteSerialNumber ) )
					{
						if( License.PublicKey.ValidateSerialNumber( license, SerialNumber ) )
						{
							unlockedLicense = license;
							break;
						}
					}
				}

				if( containsUnlock && unlockedLicense == null )
				{
#if LICENSING || LICENSETRIALS
					if( License.IsTrial )
					{
						if( MessageBox.Show(
							this,
							Internal.StaticResourceProvider.CurrentProvider.GetString( "MR_ContinueInTrial" ),
							Internal.StaticResourceProvider.CurrentProvider.GetString( "MR_ContinueInTrialTitle" ),
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Question ) == DialogResult.No )
						{
							_serialNumberPanel.Controls[ 0 ].Select();
							this.BringToFront();
							return;
						}
					}
					else
#endif
					{
						MessageBox.Show(
							this,
							Internal.StaticResourceProvider.CurrentProvider.GetString( "E_InvalidSerialNumber" ),
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error );
						_serialNumberPanel.Controls[ 0 ].Select();
						this.BringToFront();
						return;
					}
				}
				
				if( unlockedLicense != null )
				{
					GetLicense = false;
					
					unlockedLicense.SerialNumber	= SerialNumber;
					unlockedLicense.Organization	= Organization;
					unlockedLicense.User			= ( FirstName + ( FirstName.Length > 0 ? " " : "" ) + MiddleName + ( MiddleName.Length > 0 ? " " : "" ) + LastName ).Trim();
					unlockedLicense._saveOnValid	= true;
				}
			}
			

			LicenseValuesCollection collection = new LicenseValuesCollection();

			collection[ "FirstName" ]		= FirstName;
			collection[ "LastName" ]		= LastName;
			collection[ "MiddleName" ]		= MiddleName;
			collection[ "Organization" ]	= Organization;
			collection[ "SerialNumber" ]	= SerialNumber;

			foreach( Panel panel in _customFields.Controls )
			{
				if( panel.Controls[ 0 ] is ICustomFormControl )
					((ICustomFormControl)panel.Controls[ 0 ]).AddValues( collection );
				else if( panel.Controls[ 0 ].Name != null && panel.Controls[ 0 ].Name.Length > 0 )
				{
					if( panel.Controls[ 0 ] is TextBox )
					{
						collection.Add( panel.Controls[ 0 ].Name, panel.Controls[ 0 ].Text );
					}
					else
					{
						collection.Add( panel.Controls[ 0 ].Name, ((CheckBox)panel.Controls[ 0 ]).Checked.ToString( System.Globalization.CultureInfo.InvariantCulture ) );
					}
				}
			}

			collection.Add( "ProfileHash", MachineProfile.Profile.Hash );
			if( LicensedType != null )
			{
				collection.Add( "AssemblyVersion", LicensedType.Assembly.GetName().Version.ToString() );
				AssemblyFileVersionAttribute attribute = Attribute.GetCustomAttribute( LicensedType.Assembly, typeof( AssemblyFileVersionAttribute ), false ) as AssemblyFileVersionAttribute;
				if( attribute == null )
				{
					AssemblyInformationalVersionAttribute attribute2 = Attribute.GetCustomAttribute( LicensedType.Assembly, typeof( AssemblyInformationalVersionAttribute ), false ) as AssemblyInformationalVersionAttribute;
					if( attribute2 != null )
						collection.Add( "FileVersion", attribute2.InformationalVersion );
					else
						collection.Add( "FileVersion", collection[ "AssemblyVersion" ] );
				}
				else
				{
					collection.Add( "FileVersion", attribute.Version );
				}
			}

#if LICENSING || LICENSESERVERS
			if( LicenseServerUrls.Count > 0 )
			{
				bool mandatory = IsMandatory;
				_infoPanel.Enabled = false;
				_buttonPanel.Enabled = false;
				_contactInfo.Enabled = false;
				_registering.Visible = true;
				_registering.Value = 0;
				_cancel.Enabled = true;
				_cancel.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UI_Cancel" );
				_canceled = false;
				_working = true;
				Cursor = Cursors.AppStarting;
				Application.DoEvents();

				ExternalLicenseServer.XheoLicensingServer server = new ExternalLicenseServer.XheoLicensingServer();

				try
				{

					object state = new object();
					server.Timeout = 30000;

					StringBuilder invalidReason = new StringBuilder();
					bool success = false;
					string licenseXml = null;
					ExtendedLicense workingLicense = unlockedLicense == null ? License : unlockedLicense;
					foreach( string url in LicenseServerUrls )
					{
						bool retry = false;
						do
						{
							retry = false;
							server.Url = url;
							server.Proxy = ExtendedLicense.Proxy;
							server.LicenseCultureSoapHeaderValue = new Xheo.Licensing.ExternalLicenseServer.LicenseCultureSoapHeader();
							server.LicenseCultureSoapHeaderValue.CultureName = System.Globalization.CultureInfo.CurrentUICulture.Name;

							try
							{
								IAsyncResult handle = server.BeginRegisterEx( workingLicense == null ? null : workingLicense.ToXmlString(), LicensedType != null ? LicensedType.Assembly.GetName().Name : null, SerialNumber, GetLicense, collection.ToXmlString(), null, state );
								while( ! handle.AsyncWaitHandle.WaitOne( 0, false ) && ! _canceled )
								{
									_registering.Increment( 1 );
									if( _registering.Value == _registering.Maximum )
										_registering.Value = 0;
									_registering.Refresh();
									Application.DoEvents();
									System.Threading.Thread.Sleep( 100 );
								}

								if( ! _canceled )
								{
									licenseXml = server.EndRegisterEx( handle );
									success = true;
								}
							}
							catch( Exception ex )
							{
								retry = ProxyForm.RetryException( this, ex, url );

								if( ! retry )
									invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "ER_CouldNotRegister", url, FailureReportForm.PreProcessException( ex ) ) );
							}
						} while( retry );
					}

					if( ! success )
						throw new ApplicationException( invalidReason.ToString() );				

					if( ! _canceled )
					{
						_registering.Value = _registering.Maximum;
						Application.DoEvents();

						if( GetLicense || licenseXml != null )
						{
							if( GetLicense && licenseXml == null )
								throw new ExtendedLicenseException( "ER_InvalidLicenseFromServer" );

							ExtendedLicensePack pack = new ExtendedLicensePack();
							pack.FromXmlString( licenseXml );
							if( SaveLocation == null || SaveLocation.Length == 0 )
							{
								string baseDirectory = ( License == null || License.LicensePack == null || License.LicensePack.Location == null || License.LicensePack.Location.Length == 0 ) ? AppDomain.CurrentDomain.BaseDirectory : System.IO.Path.GetDirectoryName( License.LicensePack.Location );
								if( pack.MetaValues[ "SuggestedSaveLocation" ] != null )
									SaveLocation = System.IO.Path.Combine( baseDirectory, pack.MetaValues[ "SuggestedSaveLocation" ] );
								else
									SaveLocation = System.IO.Path.Combine( baseDirectory, SerialNumber + ".lic" );
							}

							if( System.IO.File.Exists( SaveLocation ) )
							{
								if( MessageBox.Show(
									this,
									Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MR_LicenseExistsOverwrite", SaveLocation ),
									"Overwrite?",
									MessageBoxButtons.YesNo,
									MessageBoxIcon.Question ) == DialogResult.No )
								{
									_licenseSFD.FileName = SaveLocation;
									if( _licenseSFD.ShowDialog() == DialogResult.Cancel )
										return;
									SaveLocation = _licenseSFD.FileName;
								}
							}

							pack.Save( SaveLocation, true );
							GotLicense = true;
						}
					}
				}
				catch( Exception ex )
				{
					if( License != null )
					{
						FailureReportForm.Show( Internal.StaticResourceProvider.CurrentProvider.GetString( "ER_CouldNotComplete" ), ex, LicensedType != null ? LicensedType.Assembly : null );
					}
					else
					{
						MessageBox.Show(
							this,
							Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "ER_CouldNotComplete1", ex.Message.Length > 500 ? ex.Message.Substring( 0, 500 ) : ex.Message ),
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error );
					}
					Application.DoEvents();
					return;
				}
				finally
				{
					_infoPanel.Enabled = true;
					_buttonPanel.Enabled = true;
					_contactInfo.Enabled = true;
					_registering.Visible = false;
					IsMandatory = mandatory;
					if( ! IsMandatory )
						_cancel.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UIR_RemindMeLater" );
					_working = false;
					Cursor = Cursors.Default;
				}

				if( _canceled )
					return;
			}
#endif
			if( ! _canceled && unlockedLicense != null && ! GotLicense )
					_license = unlockedLicense;

			if( ! _canceled && FollowUpUrl != null && FollowUpUrl.Length > 0 )
			{
				StringBuilder url = new StringBuilder();
				
				if( SendToWebPage )
				{
					foreach( string key in collection.Keys )
					{
						foreach( string value in collection.GetValues( key ) )
						{
							if( value != null && value.Length > 0 )
							{
								if( url.Length > 0 )
									url.Append( '&' );
								url.Append( key );
								url.Append( '=' );
								url.Append( HttpUtility.UrlEncode( value ) );
							}
						}
					}

					url.Insert( 0, ( ( FollowUpUrl.IndexOf( '?' ) > -1 ) ? "&" : "?" ) );
					url.Insert( 0, FollowUpUrl );
					url.AppendFormat( System.Globalization.CultureInfo.InvariantCulture, "&culture={0}", System.Globalization.CultureInfo.CurrentUICulture.Name );
				}

				try
				{
					System.Diagnostics.Process.Start( url.ToString() );
				}
				catch{}
			}
		
			DialogResult = DialogResult.OK;
		}

		private void _cancel_Click(object sender, System.EventArgs e)
		{
			if( _working )
				_canceled = true;
			else
				DialogResult = DialogResult.Cancel;
		}

		private void RegistrationForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if( IsMandatory && DialogResult != DialogResult.OK )
			{
				if( MessageBox.Show(
					this,
					Internal.StaticResourceProvider.CurrentProvider.GetString( "MR_RegistrationRequired" ),
					Internal.StaticResourceProvider.CurrentProvider.GetString( "MR_RegistrationRequiredTitle" ),
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question ) == DialogResult.No )
					e.Cancel = true;
				else
					DialogResult = DialogResult.Cancel;
			}
		}

		private void _privacy_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start( PrivacyPolicyUrl );
		}

		private void _alreadyHaveLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			using( SelectExistingLicenseForm form = new SelectExistingLicenseForm( License ) )
			{
				form.Owner = this;
				if( form.ShowDialog( this ) == DialogResult.OK )
				{
					GotLicense		 = true;
					SaveLocation	 = License.SurrogateLicensePack;
					DialogResult = DialogResult.OK;
				}
			}
		}

		private void tb_TextChanged(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;

			int			index	= _serialNumberPanel.Controls.IndexOf( tb );
			string[]	mask	= SerialNumberMask.Split( '-' );

			if( tb.Text.Length >= mask[ index ].Length && index < _serialNumberPanel.Controls.Count - 1 )
				_serialNumberPanel.Controls[ index + 1 ].Select();
			
		}

		
		private void _proxyInfo_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			using( ProxyForm form = new ProxyForm( new Uri( _licenseServerUrls[ 0 ] ) ) )
			{
				form.ShowDialog( this );
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class RegistrationForm

	#region CustomFieldType enum
	/// <summary>
	/// Custom field types for the contact info section of the form.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		enum CustomFieldType
	{
		/// <summary>
		/// Display a single line text box
		/// </summary>
		Text,

		/// <summary>
		/// Display a multi-line text box. Always uses 3 lines.
		/// </summary>
		MultiLine,

		/// <summary>
		/// Display a check box.
		/// </summary>
		Checkbox,

		/// <summary>
		/// Use a control that implements <see cref="ICustomFormControl"/>. The
		/// fieldName should be the assembly qualified name of the Type to load.
		/// </summary>
		Custom,

		/// <summary>
		/// Displays a section header that doesn not collect any data.
		/// </summary>
		Header,

		/// <summary>
		/// Displays an address form for both international and US addresses.
		/// </summary>
		Address,

		/// <summary>
		/// Displays a text box with the password field set.
		/// </summary>
		Password,
	}
	#endregion

	#region ICustomFormControl
	/// <summary>
	/// Controls that implement the ICustomFormControl can be included in the
	/// Additional Info section of the <see cref="RegistrationForm"/>, and perhaps
	/// other dynamic forms in the future.
	/// </summary>
	/// <remarks>
	///		Generally a developer will want to use a <see cref="System.Windows.Forms.UserControl"/> as
	///		the base class for a custom register contact control. The control will
	///		be added to a scrolling panel so size is not an issue.
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif 
		interface ICustomFormControl
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Properties
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Performs validation on the field in the control.
		/// </summary>
		/// <param name="field">
		///		Name of the field to validate.
		/// </param>
		/// <returns>
		///		Returns true if the fields are value, otherwise false.
		/// </returns>
		bool ValidateField( string field );

		/// <summary>
		/// Adds the values of the fields to the value collection. Multiple values can
		/// be assigned to the same key.
		/// </summary>
		/// <param name="values">
		///		A collection of name/value pairs to add the field values to.
		/// </param>
		void AddValues( LicenseValuesCollection values );

		/// <summary>
		/// Initializes the fields on the control from the collection of values.
		/// </summary>
		/// <param name="prefix">
		///		The prefix to use on each key when searching for a value for 
		///		this control.
		/// </param>
		/// <param name="values">
		///		Collection of values to use when initializing.
		/// </param>
		void InitializeFields( string prefix, NameValueCollection values);

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End interface ICustomFormControl
	#endregion

	#region Line
	/// <summary>
	/// Draws a straight beveled or flat line.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class Line : System.Windows.Forms.Control
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private Color			_highlight		= Color.FromKnownColor( KnownColor.ControlLightLight );
		private Color			_shadow			= Color.FromKnownColor( KnownColor.ControlDark );
		private Orientation		_orientation	= Orientation.Horizontal;
		private	bool			_beveled		= true;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///  Initializes a new instance of the Line class.
		/// </summary>
		public Line()
		{
			ResizeRedraw	= true;
			Height			= 2;
			Width			= 100;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets a value that indicates if the line should be drawn with a 
		/// beveled look. If false, only the <see cref="Highlight"/> color is used.
		/// </summary>
		[
		Category( "Appearance" ),
		Description( "Indicates if the line should be drawn with a  beveled look. If false, only the Highlight color is used." )
		]
		public bool Beveled
		{
			get
			{
				return _beveled;
			}
			set
			{
				_beveled = value;
			}
		}

		/// <summary>
		/// Gets or sets the highlight color.
		/// </summary>
		[
		Category( "Appearance" ),
		Description( "The highlight color" )
		]
		public Color Highlight
		{
			get
			{
				return _highlight;
			}
			set
			{
				_highlight = value;
			}
		}

		/// <summary>
		/// Gets or sets the shadow color.
		/// </summary>
		[
		Category( "Appearance" ),
		Description( "The shadow color" )
		]
		public Color Shadow
		{
			get
			{
				return _shadow;
			}
			set
			{
				_shadow = value;
			}
		}

		/// <summary>
		/// Gets or sets the orientation of the line.
		/// </summary>
		[
		Category( "Appearance" ),
		Description( "The orientation of the line." )
		]
		public Orientation Orientation
		{
			get
			{
				return _orientation;
			}
			set
			{
				_orientation = value;
			}
		}

		/// <summary>
		/// Gets the width of the line.
		/// </summary>
		[
		Browsable( false ),
		Category( "Appearance" ),
		Description( "The width of the line." )
		]
		public int LineWidth
		{
			get
			{
				return Orientation == Orientation.Horizontal ? Height : Width;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Overrides <see cref="Control.OnPaint"/>.
		/// </summary>
		/// <param name="pe"></param>
		protected override void OnPaint( PaintEventArgs pe )
		{
			Graphics	g	= pe.Graphics;
			Brush		h	= null;
			Brush		s	= null;

			if( Beveled )
			{
				h = new SolidBrush( Highlight );
				s = new SolidBrush( Shadow );

				if( Orientation == Orientation.Horizontal )
				{
					g.FillRectangle( s, 0, 0, Width, LineWidth / 2 );
					g.FillRectangle( h, 0, LineWidth / 2, Width, LineWidth / 2 );
				}
				else
				{
					g.FillRectangle( s, 0, 0, LineWidth / 2, Height );
					g.FillRectangle( h, LineWidth / 2, 0, LineWidth / 2, Height );
				}
			}
			else
			{
				h = new SolidBrush( Highlight );

				if( Orientation == Orientation.Horizontal )
					g.FillRectangle( h, 0, 0, Width, LineWidth );
				else
					g.FillRectangle( h, 0, 0, LineWidth, Height );
			}

			if( h != null )
				h.Dispose();

			if( s != null )
				s.Dispose();
		}

		//		/// <summary>
		//		/// Overrides <see cref="Control.OnPaintBackground"/>.
		//		/// </summary>
		//		protected override void OnPaintBackground( PaintEventArgs e )
		//		{
		//		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class Line
	#endregion
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////