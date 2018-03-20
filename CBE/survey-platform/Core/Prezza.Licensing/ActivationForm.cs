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
// Class:		ActivationForm
// Author:		Paul Alexander
// Created:		Monday, December 30, 2002 8:00:59 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace Xheo.Licensing
{
#if LICENSING || LICENSEACTIVATION
	/// <summary>
	/// Displays an interface for the user to activate a license. Used by the
	/// <see cref="ActivationLimit"/> when validating a license that has not
	/// been activated.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
	class ActivationForm : System.Windows.Forms.Form
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private System.Windows.Forms.Label _daysLeft;
		private System.Windows.Forms.Button _continue;
		private System.Windows.Forms.LinkLabel _supportEmail;
		private System.Windows.Forms.Label _supportPhone;
		private System.Windows.Forms.SaveFileDialog _licenseSFD;

		private ExtendedLicense _license		= null;
		private ActivationLimit	_limit			= null;
		private Type			_type			= null;
		private bool			_canContinue	= false;
		private System.Windows.Forms.LinkLabel _supportUrl;
		private System.Windows.Forms.RadioButton _activateRadio;
		private System.Windows.Forms.RadioButton _continueRadio;
		private System.Windows.Forms.RadioButton _emailOrPhoneRadio;
		private System.Windows.Forms.TextBox _serialNumber;
		private System.Windows.Forms.Button _copyTo;
		private System.Windows.Forms.TextBox _hash;
		private System.Windows.Forms.Button _back;
		private System.Windows.Forms.Panel _manualPanel;
		private System.Windows.Forms.LinkLabel _link;
		private System.Windows.Forms.Button _copyToEmail;
		private System.Windows.Forms.ProgressBar _progressing;
		private System.Windows.Forms.Timer _progressTimer;
		private System.Windows.Forms.Label _formTitle;
		private System.Windows.Forms.Label _welcomeMessage;
		private System.Windows.Forms.Label _activationInstructions;
		private System.Windows.Forms.Label _daysLeftLabel;
		private System.Windows.Forms.Panel _sidePanel;
		private System.Windows.Forms.Label _supportEmailLabel;
		private System.Windows.Forms.Label _supportPhoneLabel;
		private System.Windows.Forms.Label _supportUrlLabel;
		private System.Windows.Forms.Label _copyrightWarning;
		private System.Windows.Forms.Label _activationKey;
		private System.Windows.Forms.Label _serialNumberLabel;
		private System.Windows.Forms.TextBox _uc1;
		private System.Windows.Forms.Label _machineWarning;
		private System.Windows.Forms.Label _unlockKey;
		private System.Windows.Forms.TextBox _uc2;
		private System.Windows.Forms.TextBox _uc3;
		private System.Windows.Forms.TextBox _uc4;
		private System.Windows.Forms.TextBox _uc5;
		private System.Windows.Forms.TextBox _uc6;
		private System.Windows.Forms.TextBox _uc7;
		private System.Windows.Forms.TextBox _uc8;
		private System.Windows.Forms.LinkLabel _proxyInfo;
		private System.ComponentModel.IContainer components;


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Included for COM compliance only, do not use. 
		/// </summary>
		public ActivationForm()
		{
			throw new NotSupportedException( "Use ActivationForm( ExtendedLicense, ActivationLimit, Type ) instead." );
		}

		/// <summary>
		/// Initializes a new instance of the ActivationForm class.
		/// </summary>
		/// <param name="license">
		///		The license to be activated.
		/// </param>
		/// <param name="limit">
		///		The ActivationLimit containing the parameters for the activation.
		/// </param>
		/// <param name="type">
		///		The Type being created that required a license.
		/// </param>
		public ActivationForm( ExtendedLicense license, ActivationLimit limit, Type type )
		{
			InitializeComponent();
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );

			_license	= license;
			_limit		= limit;
			_type		= type;

			SupportInfo supportInfo = LicenseHelpAttribute.GetSupportInfo( type );

			_supportUrl.Text	= supportInfo.Url;
			_supportPhone.Text	= supportInfo.Phone;
			_supportEmail.Text	= supportInfo.Email;

			TimeSpan days = limit.GracePeriod - DateTime.UtcNow;
			if( days.TotalDays >= 0 )
			{
				_daysLeft.Text = ((int)days.TotalDays).ToString( System.Globalization.CultureInfo.InvariantCulture );
			}
			else
			{
				_daysLeft.Text = "0";
				_continueRadio.Enabled = false;
			}

			if( ! limit.CanActivateByKey )
				_emailOrPhoneRadio.Enabled = false;

			if( limit.Url == null || limit.Url.Length == 0 )
			{
				_activateRadio.Enabled = false;
				_proxyInfo.Visible = false;
				if( _emailOrPhoneRadio.Enabled )
					_emailOrPhoneRadio.Checked = true;
			}

			if( ! _emailOrPhoneRadio.Enabled && ! _activateRadio.Enabled && _continueRadio.Enabled )
				_continueRadio.Checked = true;
				

			_sidePanel.BackgroundImage = null;

			if( limit.SplashResource != null && limit.SplashResource.Length > 0 )
				_sidePanel.BackgroundImage = Limit.GetBitmapResource( limit.SplashResource, limit, null );
			
			if( _sidePanel.BackgroundImage == null )
				_sidePanel.BackgroundImage = Limit.GetBitmapResource( Limit.MakeResourceString( "activation.jpg", typeof( ExtendedLicense ) ), null );

			if( license.SignedByProLicense )
				_link.Visible = false;
				
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
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
			this.components = new System.ComponentModel.Container();
			this._formTitle = new System.Windows.Forms.Label();
			this._welcomeMessage = new System.Windows.Forms.Label();
			this._activateRadio = new System.Windows.Forms.RadioButton();
			this._continue = new System.Windows.Forms.Button();
			this._daysLeftLabel = new System.Windows.Forms.Label();
			this._daysLeft = new System.Windows.Forms.Label();
			this._sidePanel = new System.Windows.Forms.Panel();
			this._supportUrl = new System.Windows.Forms.LinkLabel();
			this._supportPhone = new System.Windows.Forms.Label();
			this._supportEmail = new System.Windows.Forms.LinkLabel();
			this._supportEmailLabel = new System.Windows.Forms.Label();
			this._supportPhoneLabel = new System.Windows.Forms.Label();
			this._supportUrlLabel = new System.Windows.Forms.Label();
			this._link = new System.Windows.Forms.LinkLabel();
			this._continueRadio = new System.Windows.Forms.RadioButton();
			this._copyrightWarning = new System.Windows.Forms.Label();
			this._licenseSFD = new System.Windows.Forms.SaveFileDialog();
			this._emailOrPhoneRadio = new System.Windows.Forms.RadioButton();
			this._manualPanel = new System.Windows.Forms.Panel();
			this._uc8 = new System.Windows.Forms.TextBox();
			this._uc7 = new System.Windows.Forms.TextBox();
			this._uc6 = new System.Windows.Forms.TextBox();
			this._uc5 = new System.Windows.Forms.TextBox();
			this._uc4 = new System.Windows.Forms.TextBox();
			this._uc3 = new System.Windows.Forms.TextBox();
			this._uc2 = new System.Windows.Forms.TextBox();
			this._uc1 = new System.Windows.Forms.TextBox();
			this._unlockKey = new System.Windows.Forms.Label();
			this._activationInstructions = new System.Windows.Forms.Label();
			this._serialNumber = new System.Windows.Forms.TextBox();
			this._hash = new System.Windows.Forms.TextBox();
			this._activationKey = new System.Windows.Forms.Label();
			this._serialNumberLabel = new System.Windows.Forms.Label();
			this._copyTo = new System.Windows.Forms.Button();
			this._back = new System.Windows.Forms.Button();
			this._copyToEmail = new System.Windows.Forms.Button();
			this._progressing = new System.Windows.Forms.ProgressBar();
			this._progressTimer = new System.Windows.Forms.Timer(this.components);
			this._machineWarning = new System.Windows.Forms.Label();
			this._proxyInfo = new System.Windows.Forms.LinkLabel();
			this._sidePanel.SuspendLayout();
			this._manualPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _formTitle
			// 
			this._formTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._formTitle.Location = new System.Drawing.Point(184, 8);
			this._formTitle.Name = "_formTitle";
			this._formTitle.Size = new System.Drawing.Size(266, 28);
			this._formTitle.TabIndex = 10;
			this._formTitle.Text = "#UIA_Title";
			// 
			// _welcomeMessage
			// 
			this._welcomeMessage.Location = new System.Drawing.Point(184, 40);
			this._welcomeMessage.Name = "_welcomeMessage";
			this._welcomeMessage.Size = new System.Drawing.Size(408, 60);
			this._welcomeMessage.TabIndex = 11;
			this._welcomeMessage.Text = "#UIA_Welcome";
			// 
			// _activateRadio
			// 
			this._activateRadio.Checked = true;
			this._activateRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._activateRadio.Location = new System.Drawing.Point(216, 128);
			this._activateRadio.Name = "_activateRadio";
			this._activateRadio.Size = new System.Drawing.Size(133, 30);
			this._activateRadio.TabIndex = 0;
			this._activateRadio.TabStop = true;
			this._activateRadio.Text = "#UIA_Online";
			// 
			// _continue
			// 
			this._continue.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._continue.Location = new System.Drawing.Point(512, 272);
			this._continue.Name = "_continue";
			this._continue.Size = new System.Drawing.Size(80, 24);
			this._continue.TabIndex = 4;
			this._continue.Text = "#UI_Continue";
			this._continue.Click += new System.EventHandler(this._continue_Click);
			// 
			// _daysLeftLabel
			// 
			this._daysLeftLabel.BackColor = System.Drawing.Color.Transparent;
			this._daysLeftLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._daysLeftLabel.ForeColor = System.Drawing.Color.White;
			this._daysLeftLabel.Location = new System.Drawing.Point(10, 8);
			this._daysLeftLabel.Name = "_daysLeftLabel";
			this._daysLeftLabel.Size = new System.Drawing.Size(92, 32);
			this._daysLeftLabel.TabIndex = 7;
			this._daysLeftLabel.Text = "#UIA_Days";
			// 
			// _daysLeft
			// 
			this._daysLeft.BackColor = System.Drawing.Color.Transparent;
			this._daysLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._daysLeft.ForeColor = System.Drawing.Color.Yellow;
			this._daysLeft.Location = new System.Drawing.Point(10, 39);
			this._daysLeft.Name = "_daysLeft";
			this._daysLeft.Size = new System.Drawing.Size(92, 40);
			this._daysLeft.TabIndex = 8;
			this._daysLeft.Text = "18";
			// 
			// _sidePanel
			// 
			this._sidePanel.BackColor = System.Drawing.Color.White;
			this._sidePanel.Controls.Add(this._supportUrl);
			this._sidePanel.Controls.Add(this._supportPhone);
			this._sidePanel.Controls.Add(this._supportEmail);
			this._sidePanel.Controls.Add(this._daysLeft);
			this._sidePanel.Controls.Add(this._daysLeftLabel);
			this._sidePanel.Controls.Add(this._supportEmailLabel);
			this._sidePanel.Controls.Add(this._supportPhoneLabel);
			this._sidePanel.Controls.Add(this._supportUrlLabel);
			this._sidePanel.Controls.Add(this._link);
			this._sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
			this._sidePanel.Location = new System.Drawing.Point(0, 0);
			this._sidePanel.Name = "_sidePanel";
			this._sidePanel.Size = new System.Drawing.Size(176, 352);
			this._sidePanel.TabIndex = 9;
			// 
			// _supportUrl
			// 
			this._supportUrl.BackColor = System.Drawing.Color.Transparent;
			this._supportUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportUrl.LinkColor = System.Drawing.Color.White;
			this._supportUrl.Location = new System.Drawing.Point(8, 216);
			this._supportUrl.Name = "_supportUrl";
			this._supportUrl.Size = new System.Drawing.Size(205, 20);
			this._supportUrl.TabIndex = 1;
			this._supportUrl.TabStop = true;
			this._supportUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._supportUrl_LinkClicked);
			// 
			// _supportPhone
			// 
			this._supportPhone.BackColor = System.Drawing.Color.Transparent;
			this._supportPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportPhone.ForeColor = System.Drawing.Color.White;
			this._supportPhone.Location = new System.Drawing.Point(8, 296);
			this._supportPhone.Name = "_supportPhone";
			this._supportPhone.Size = new System.Drawing.Size(205, 19);
			this._supportPhone.TabIndex = 5;
			// 
			// _supportEmail
			// 
			this._supportEmail.BackColor = System.Drawing.Color.Transparent;
			this._supportEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportEmail.LinkColor = System.Drawing.Color.White;
			this._supportEmail.Location = new System.Drawing.Point(8, 256);
			this._supportEmail.Name = "_supportEmail";
			this._supportEmail.Size = new System.Drawing.Size(205, 23);
			this._supportEmail.TabIndex = 3;
			this._supportEmail.TabStop = true;
			this._supportEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._supportEmail_LinkClicked);
			// 
			// _supportEmailLabel
			// 
			this._supportEmailLabel.BackColor = System.Drawing.Color.Transparent;
			this._supportEmailLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportEmailLabel.ForeColor = System.Drawing.Color.White;
			this._supportEmailLabel.Location = new System.Drawing.Point(8, 240);
			this._supportEmailLabel.Name = "_supportEmailLabel";
			this._supportEmailLabel.Size = new System.Drawing.Size(205, 29);
			this._supportEmailLabel.TabIndex = 2;
			this._supportEmailLabel.Text = "#UI_SE";
			// 
			// _supportPhoneLabel
			// 
			this._supportPhoneLabel.BackColor = System.Drawing.Color.Transparent;
			this._supportPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportPhoneLabel.ForeColor = System.Drawing.Color.White;
			this._supportPhoneLabel.Location = new System.Drawing.Point(8, 280);
			this._supportPhoneLabel.Name = "_supportPhoneLabel";
			this._supportPhoneLabel.Size = new System.Drawing.Size(205, 28);
			this._supportPhoneLabel.TabIndex = 4;
			this._supportPhoneLabel.Text = "#UI_SP";
			// 
			// _supportUrlLabel
			// 
			this._supportUrlLabel.BackColor = System.Drawing.Color.Transparent;
			this._supportUrlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportUrlLabel.ForeColor = System.Drawing.Color.White;
			this._supportUrlLabel.Location = new System.Drawing.Point(8, 200);
			this._supportUrlLabel.Name = "_supportUrlLabel";
			this._supportUrlLabel.Size = new System.Drawing.Size(205, 28);
			this._supportUrlLabel.TabIndex = 0;
			this._supportUrlLabel.Text = "#UI_SU";
			// 
			// _link
			// 
			this._link.BackColor = System.Drawing.Color.Black;
			this._link.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._link.ForeColor = System.Drawing.Color.White;
			this._link.LinkArea = new System.Windows.Forms.LinkArea(13, 14);
			this._link.LinkColor = System.Drawing.Color.Yellow;
			this._link.Location = new System.Drawing.Point(0, 328);
			this._link.Name = "_link";
			this._link.Size = new System.Drawing.Size(176, 24);
			this._link.TabIndex = 6;
			this._link.TabStop = true;
			this._link.Text = "Protected By XHEO|Licensing";
			this._link.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._link_LinkClicked);
			// 
			// _continueRadio
			// 
			this._continueRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._continueRadio.Location = new System.Drawing.Point(216, 192);
			this._continueRadio.Name = "_continueRadio";
			this._continueRadio.Size = new System.Drawing.Size(226, 30);
			this._continueRadio.TabIndex = 2;
			this._continueRadio.Text = "#UIA_ACont";
			// 
			// _copyrightWarning
			// 
			this._copyrightWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._copyrightWarning.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this._copyrightWarning.Location = new System.Drawing.Point(184, 304);
			this._copyrightWarning.Name = "_copyrightWarning";
			this._copyrightWarning.Size = new System.Drawing.Size(408, 56);
			this._copyrightWarning.TabIndex = 13;
			this._copyrightWarning.Text = "#UI_Warning";
			// 
			// _licenseSFD
			// 
			this._licenseSFD.FileName = "doc1";
			// 
			// _emailOrPhoneRadio
			// 
			this._emailOrPhoneRadio.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._emailOrPhoneRadio.Location = new System.Drawing.Point(216, 160);
			this._emailOrPhoneRadio.Name = "_emailOrPhoneRadio";
			this._emailOrPhoneRadio.Size = new System.Drawing.Size(215, 29);
			this._emailOrPhoneRadio.TabIndex = 1;
			this._emailOrPhoneRadio.Text = "#UIA_Phone";
			// 
			// _manualPanel
			// 
			this._manualPanel.Controls.Add(this._uc8);
			this._manualPanel.Controls.Add(this._uc7);
			this._manualPanel.Controls.Add(this._uc6);
			this._manualPanel.Controls.Add(this._uc5);
			this._manualPanel.Controls.Add(this._uc4);
			this._manualPanel.Controls.Add(this._uc3);
			this._manualPanel.Controls.Add(this._uc2);
			this._manualPanel.Controls.Add(this._uc1);
			this._manualPanel.Controls.Add(this._unlockKey);
			this._manualPanel.Controls.Add(this._activationInstructions);
			this._manualPanel.Controls.Add(this._serialNumber);
			this._manualPanel.Controls.Add(this._hash);
			this._manualPanel.Controls.Add(this._activationKey);
			this._manualPanel.Controls.Add(this._serialNumberLabel);
			this._manualPanel.Location = new System.Drawing.Point(184, 40);
			this._manualPanel.Name = "_manualPanel";
			this._manualPanel.Size = new System.Drawing.Size(408, 227);
			this._manualPanel.TabIndex = 3;
			this._manualPanel.Visible = false;
			// 
			// _uc8
			// 
			this._uc8.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._uc8.Location = new System.Drawing.Point(288, 184);
			this._uc8.MaxLength = 8;
			this._uc8.Name = "_uc8";
			this._uc8.Size = new System.Drawing.Size(91, 26);
			this._uc8.TabIndex = 7;
			this._uc8.Text = "";
			this._uc8.WordWrap = false;
			this._uc8.TextChanged += new System.EventHandler(this._uc1_TextChanged);
			// 
			// _uc7
			// 
			this._uc7.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._uc7.Location = new System.Drawing.Point(192, 184);
			this._uc7.MaxLength = 8;
			this._uc7.Name = "_uc7";
			this._uc7.Size = new System.Drawing.Size(91, 26);
			this._uc7.TabIndex = 6;
			this._uc7.Text = "";
			this._uc7.WordWrap = false;
			this._uc7.TextChanged += new System.EventHandler(this._uc1_TextChanged);
			// 
			// _uc6
			// 
			this._uc6.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._uc6.Location = new System.Drawing.Point(96, 184);
			this._uc6.MaxLength = 8;
			this._uc6.Name = "_uc6";
			this._uc6.Size = new System.Drawing.Size(91, 26);
			this._uc6.TabIndex = 5;
			this._uc6.Text = "";
			this._uc6.WordWrap = false;
			this._uc6.TextChanged += new System.EventHandler(this._uc1_TextChanged);
			// 
			// _uc5
			// 
			this._uc5.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._uc5.Location = new System.Drawing.Point(0, 184);
			this._uc5.MaxLength = 8;
			this._uc5.Name = "_uc5";
			this._uc5.Size = new System.Drawing.Size(91, 26);
			this._uc5.TabIndex = 4;
			this._uc5.Text = "";
			this._uc5.WordWrap = false;
			this._uc5.TextChanged += new System.EventHandler(this._uc1_TextChanged);
			// 
			// _uc4
			// 
			this._uc4.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._uc4.Location = new System.Drawing.Point(288, 152);
			this._uc4.MaxLength = 8;
			this._uc4.Name = "_uc4";
			this._uc4.Size = new System.Drawing.Size(91, 26);
			this._uc4.TabIndex = 3;
			this._uc4.Text = "";
			this._uc4.WordWrap = false;
			this._uc4.TextChanged += new System.EventHandler(this._uc1_TextChanged);
			// 
			// _uc3
			// 
			this._uc3.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._uc3.Location = new System.Drawing.Point(192, 152);
			this._uc3.MaxLength = 8;
			this._uc3.Name = "_uc3";
			this._uc3.Size = new System.Drawing.Size(91, 26);
			this._uc3.TabIndex = 2;
			this._uc3.Text = "";
			this._uc3.WordWrap = false;
			this._uc3.TextChanged += new System.EventHandler(this._uc1_TextChanged);
			// 
			// _uc2
			// 
			this._uc2.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._uc2.Location = new System.Drawing.Point(96, 152);
			this._uc2.MaxLength = 8;
			this._uc2.Name = "_uc2";
			this._uc2.Size = new System.Drawing.Size(91, 26);
			this._uc2.TabIndex = 1;
			this._uc2.Text = "";
			this._uc2.WordWrap = false;
			this._uc2.TextChanged += new System.EventHandler(this._uc1_TextChanged);
			// 
			// _uc1
			// 
			this._uc1.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._uc1.Location = new System.Drawing.Point(0, 152);
			this._uc1.MaxLength = 8;
			this._uc1.Name = "_uc1";
			this._uc1.Size = new System.Drawing.Size(91, 26);
			this._uc1.TabIndex = 0;
			this._uc1.Text = "";
			this._uc1.WordWrap = false;
			this._uc1.TextChanged += new System.EventHandler(this._uc1_TextChanged);
			this._uc1.Enter += new System.EventHandler(this._uc1_Enter);
			// 
			// _unlockKey
			// 
			this._unlockKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._unlockKey.Location = new System.Drawing.Point(0, 136);
			this._unlockKey.Name = "_unlockKey";
			this._unlockKey.Size = new System.Drawing.Size(128, 16);
			this._unlockKey.TabIndex = 13;
			this._unlockKey.Text = "#UIA_ActivationKey";
			// 
			// _activationInstructions
			// 
			this._activationInstructions.Dock = System.Windows.Forms.DockStyle.Top;
			this._activationInstructions.Location = new System.Drawing.Point(0, 0);
			this._activationInstructions.Name = "_activationInstructions";
			this._activationInstructions.Size = new System.Drawing.Size(408, 48);
			this._activationInstructions.TabIndex = 8;
			this._activationInstructions.Text = "#UIA_Inst";
			// 
			// _serialNumber
			// 
			this._serialNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._serialNumber.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._serialNumber.Location = new System.Drawing.Point(0, 64);
			this._serialNumber.Name = "_serialNumber";
			this._serialNumber.ReadOnly = true;
			this._serialNumber.Size = new System.Drawing.Size(405, 22);
			this._serialNumber.TabIndex = 10;
			this._serialNumber.TabStop = false;
			this._serialNumber.Text = "";
			// 
			// _hash
			// 
			this._hash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._hash.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._hash.Location = new System.Drawing.Point(0, 104);
			this._hash.Name = "_hash";
			this._hash.ReadOnly = true;
			this._hash.Size = new System.Drawing.Size(405, 22);
			this._hash.TabIndex = 12;
			this._hash.TabStop = false;
			this._hash.Text = "";
			// 
			// _activationKey
			// 
			this._activationKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._activationKey.Location = new System.Drawing.Point(0, 88);
			this._activationKey.Name = "_activationKey";
			this._activationKey.Size = new System.Drawing.Size(128, 16);
			this._activationKey.TabIndex = 11;
			this._activationKey.Text = "#UIA_Key";
			// 
			// _serialNumberLabel
			// 
			this._serialNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._serialNumberLabel.Location = new System.Drawing.Point(0, 48);
			this._serialNumberLabel.Name = "_serialNumberLabel";
			this._serialNumberLabel.Size = new System.Drawing.Size(128, 16);
			this._serialNumberLabel.TabIndex = 9;
			this._serialNumberLabel.Text = "#UI_SN";
			// 
			// _copyTo
			// 
			this._copyTo.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._copyTo.Location = new System.Drawing.Point(184, 272);
			this._copyTo.Name = "_copyTo";
			this._copyTo.Size = new System.Drawing.Size(112, 24);
			this._copyTo.TabIndex = 6;
			this._copyTo.Text = "#UIA_Clipboard";
			this._copyTo.Visible = false;
			this._copyTo.Click += new System.EventHandler(this._copyTo_Click);
			// 
			// _back
			// 
			this._back.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._back.Location = new System.Drawing.Point(424, 272);
			this._back.Name = "_back";
			this._back.Size = new System.Drawing.Size(80, 24);
			this._back.TabIndex = 8;
			this._back.Text = "#UIA_Back";
			this._back.Visible = false;
			this._back.Click += new System.EventHandler(this._back_Click);
			// 
			// _copyToEmail
			// 
			this._copyToEmail.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._copyToEmail.Location = new System.Drawing.Point(304, 272);
			this._copyToEmail.Name = "_copyToEmail";
			this._copyToEmail.Size = new System.Drawing.Size(112, 24);
			this._copyToEmail.TabIndex = 7;
			this._copyToEmail.Text = "#UIA_Email";
			this._copyToEmail.Visible = false;
			this._copyToEmail.Click += new System.EventHandler(this._copyToEmail_Click);
			// 
			// _progressing
			// 
			this._progressing.Location = new System.Drawing.Point(184, 275);
			this._progressing.Name = "_progressing";
			this._progressing.Size = new System.Drawing.Size(320, 18);
			this._progressing.TabIndex = 5;
			this._progressing.Visible = false;
			// 
			// _progressTimer
			// 
			this._progressTimer.Tick += new System.EventHandler(this._progressTimer_Tick);
			// 
			// _machineWarning
			// 
			this._machineWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._machineWarning.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this._machineWarning.Location = new System.Drawing.Point(184, 88);
			this._machineWarning.Name = "_machineWarning";
			this._machineWarning.Size = new System.Drawing.Size(408, 29);
			this._machineWarning.TabIndex = 12;
			this._machineWarning.Text = "#UIA_MWarning";
			// 
			// _proxyInfo
			// 
			this._proxyInfo.Location = new System.Drawing.Point(400, 272);
			this._proxyInfo.Name = "_proxyInfo";
			this._proxyInfo.Size = new System.Drawing.Size(100, 24);
			this._proxyInfo.TabIndex = 14;
			this._proxyInfo.TabStop = true;
			this._proxyInfo.Text = "#UI_ProxyInfo";
			this._proxyInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._proxyInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._proxyInfo_LinkClicked);
			// 
			// ActivationForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(602, 352);
			this.Controls.Add(this._manualPanel);
			this.Controls.Add(this._progressing);
			this.Controls.Add(this._copyrightWarning);
			this.Controls.Add(this._sidePanel);
			this.Controls.Add(this._continue);
			this.Controls.Add(this._activateRadio);
			this.Controls.Add(this._formTitle);
			this.Controls.Add(this._continueRadio);
			this.Controls.Add(this._emailOrPhoneRadio);
			this.Controls.Add(this._copyTo);
			this.Controls.Add(this._back);
			this.Controls.Add(this._copyToEmail);
			this.Controls.Add(this._machineWarning);
			this.Controls.Add(this._welcomeMessage);
			this.Controls.Add(this._proxyInfo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ActivationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "#UIA_Title";
			this._sidePanel.ResumeLayout(false);
			this._manualPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets a value indicating if the license validation can continue.
		/// </summary>
		public bool CanContinue
		{
			get
			{
				return _canContinue;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		///<summary>
		///Summary of _continue_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _continue_Click(object sender, System.EventArgs e)
		{
			if( _limit.IsWorking )
			{
				_limit.Abort();
			}
			else
			{
				if( _continueRadio.Checked && _limit.GracePeriod > DateTime.UtcNow )
				{
					DialogResult = DialogResult.OK;
					_canContinue = true;
				}
				else if( _emailOrPhoneRadio.Checked )
				{
					if( _manualPanel.Visible )
					{
						string unlockKey = 
							_uc1.Text +
							_uc2.Text +
							_uc3.Text +
							_uc4.Text +
							_uc5.Text +
							_uc6.Text +
							_uc7.Text +
							_uc8.Text;
						
						if( _license.PublicKey.ValidateActivationUnlockKey( MachineProfile.Profile.GetComparableHash( _license.Version ), unlockKey ) )
						{
							_limit.UnlockHash			= MachineProfile.Profile.GetComparableHash( _license.Version );
							_limit.ActivationKey		= unlockKey;
							_license.UnlockedLicense	= _license;
							_license._saveOnValid		= true;
							DialogResult = DialogResult.OK;
							_canContinue = true;
						}
						else
						{
							MessageBox.Show(
								this,
								Internal.StaticResourceProvider.CurrentProvider.GetString( "E_InvalidActivationKey" ),
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error );
						}
					}
					else
					{
						_manualPanel.Visible = true;
						_copyTo.Visible = true;
						_copyToEmail.Visible = true;
						_back.Visible = true;
						_serialNumber.Text = _license.SerialNumber;
						_hash.Text = LicenseSigningKey.MakeActivationKey( _license );
						_uc1.Select();
					}
				}
				else
				{
					_continue.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UI_Cancel" );
					_progressing.Visible = true;
					_progressing.Value = 0;
					_progressTimer.Enabled = true;
					Cursor = Cursors.AppStarting;
					Application.DoEvents();
					if( ! _limit.ActivateWithServer( _type ) )
					{
						if( _limit.License.SurrogateLicensePack != null )
						{
							DialogResult = DialogResult.Cancel;
							return;
						}
						_progressing.Value = _progressing.Maximum;
						_progressTimer.Enabled = false;

						if( ! _limit.IsCanceled )
							FailureReportForm.Show( LicenseManager.CurrentContext, _limit.License.LicensedType, null, _license.InvalidReason );
					}
					else
					{
						_canContinue = true;
						DialogResult = DialogResult.OK;
					}
					Cursor = Cursors.Default;
					_continue.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UI_Continue" );
					_continue.Enabled = true;	
					_progressing.Visible = false;
					_progressTimer.Enabled = false;
				}
			}
		}

		///<summary>
		///Summary of _copyTo_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _copyTo_Click(object sender, System.EventArgs e)
		{
			string data = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MAL_Clipboard",
				_serialNumber.Text,
				_hash.Text );

			Clipboard.SetDataObject( data, true );
		}

		private void _copyToEmail_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start( "mailto:" + _supportEmail.Text + 
				"?subject=Product%20Activation&body=Serial%20Number:%20" + 
				System.Web.HttpUtility.UrlEncode( _serialNumber.Text ) + 
				"%0A%0DActivation%20Key:%20" +
				System.Web.HttpUtility.UrlEncode( _hash.Text ) );
		}

		///<summary>
		///Summary of _back_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _back_Click(object sender, System.EventArgs e)
		{
			_manualPanel.Visible = false;
			_copyTo.Visible = false;
			_copyToEmail.Visible = false;
			_back.Visible = false;
		}

		///<summary>
		///Summary of _link_LinkClicked.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _link_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start( "http://www.xheo.com/products/licensing/" );
		}

		private void _supportUrl_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start( _supportUrl.Text );		
		}

		private void _supportEmail_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start( "mailto:" + _supportEmail.Text + 
				"?subject=Product%20Registration&body=Serial%20Number:%20" + 
				System.Web.HttpUtility.UrlEncode( _serialNumber.Text ) + 
				"%0A%0DActivation%20Key:%20" +
				System.Web.HttpUtility.UrlEncode( _hash.Text ) );
		}

		private void _progressTimer_Tick(object sender, System.EventArgs e)
		{
			_progressing.Increment(1);
			if( _progressing.Value == _progressing.Maximum )
				_progressing.Value = 0;
		}

		
		private void _uc1_TextChanged(object sender, System.EventArgs e)
		{
			TextBox tb = sender as TextBox;
			if( tb.Text != tb.Text.ToUpper( System.Globalization.CultureInfo.InvariantCulture ) )
			{
				tb.Text = tb.Text.ToUpper( System.Globalization.CultureInfo.InvariantCulture );
				tb.SelectionStart = tb.Text.Length;
			}
			else
			{
				if( tb.Text.Length == 8 )
				{
					Control next = _manualPanel.GetNextControl( tb, true );
					if( next != null )
						next.Select();
				}
			}
		}

		private void _uc1_Enter(object sender, System.EventArgs e)
		{
			TextBox tb = sender as TextBox;

		}

		private void _proxyInfo_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			using( ProxyForm form = new ProxyForm( new Uri( _limit.Url ) ) )
				form.ShowDialog( this );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class ActivationForm
	#endif
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////