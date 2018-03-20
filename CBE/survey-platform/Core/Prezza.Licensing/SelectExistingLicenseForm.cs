///////////////////////////////////////////////////////////////////////////////
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
///////////////////////////////////////////////////////////////////////////////
//
// Class:		SelectExistingLicenseForm
// Author:		Paul Alexander
// Created:		Thursday, June 12, 2003 12:19:35 AM
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace Xheo.Licensing
{
	/// <summary>
	/// Summary description for SelectExistingLicenseForm.
	/// </summary>
#if LICENSING
	public
#else
	internal
#endif
		class SelectExistingLicenseForm : System.Windows.Forms.Form
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		private System.Windows.Forms.Panel _sidePanel;
		private System.Windows.Forms.LinkLabel _supportUrl;
		private System.Windows.Forms.Label _supportPhone;
		private System.Windows.Forms.LinkLabel _supportEmail;
		private System.Windows.Forms.Label _supportEmailLabel;
		private System.Windows.Forms.Label _supportPhoneLabel;
		private System.Windows.Forms.Label _supportUrlLabel;
		private System.Windows.Forms.LinkLabel _link;
		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Label _formTitle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _location;
		private System.Windows.Forms.Label _fileLocationLabel;
		private System.Windows.Forms.Button _browse;
		private System.Windows.Forms.Button _cancel;
		private System.Windows.Forms.RadioButton _thisAppOnly;

		private ExtendedLicense _originalLicense = null;
		private System.Windows.Forms.RadioButton _allApps;
		private System.Windows.Forms.OpenFileDialog _licenseOFD;
		private System.Windows.Forms.SaveFileDialog _licenseSFD;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		
		/// <summary>
		/// Initializes a new instance of the SelectExistingLicenseForm class.
		/// </summary>
		public SelectExistingLicenseForm()
		{
			throw new NotSupportedException( "Not supported. Use SelectExistingLicenseForm( Type ) instead." );
		}

		/// <summary>
		/// Initializes a new instance of the SelectExistingLicenseForm class.
		/// </summary>
		/// <param name="license">
		///		The original lincenese that should be replaced by the production license.
		/// </param>
		public SelectExistingLicenseForm( ExtendedLicense license )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_sidePanel.BackgroundImage = Limit.GetBitmapResource( Limit.MakeResourceString( "activation.jpg", typeof( ExtendedLicense ) ), null );
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );

			SupportInfo supportInfo = LicenseHelpAttribute.GetSupportInfo( license.LicensedType );

			_supportUrl.Text	= supportInfo.Url;
			_supportPhone.Text	= supportInfo.Phone;
			_supportEmail.Text	= supportInfo.Email;

			_originalLicense = license;

			if( license.SignedByProLicense )
				_link.Text = "";
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
			this._sidePanel = new System.Windows.Forms.Panel();
			this._supportUrl = new System.Windows.Forms.LinkLabel();
			this._supportPhone = new System.Windows.Forms.Label();
			this._supportEmail = new System.Windows.Forms.LinkLabel();
			this._supportEmailLabel = new System.Windows.Forms.Label();
			this._supportPhoneLabel = new System.Windows.Forms.Label();
			this._supportUrlLabel = new System.Windows.Forms.Label();
			this._link = new System.Windows.Forms.LinkLabel();
			this._ok = new System.Windows.Forms.Button();
			this._formTitle = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._location = new System.Windows.Forms.TextBox();
			this._fileLocationLabel = new System.Windows.Forms.Label();
			this._browse = new System.Windows.Forms.Button();
			this._cancel = new System.Windows.Forms.Button();
			this._thisAppOnly = new System.Windows.Forms.RadioButton();
			this._allApps = new System.Windows.Forms.RadioButton();
			this._licenseOFD = new System.Windows.Forms.OpenFileDialog();
			this._licenseSFD = new System.Windows.Forms.SaveFileDialog();
			this._sidePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _sidePanel
			// 
			this._sidePanel.Controls.Add(this._supportUrl);
			this._sidePanel.Controls.Add(this._supportPhone);
			this._sidePanel.Controls.Add(this._supportEmail);
			this._sidePanel.Controls.Add(this._supportEmailLabel);
			this._sidePanel.Controls.Add(this._supportPhoneLabel);
			this._sidePanel.Controls.Add(this._supportUrlLabel);
			this._sidePanel.Controls.Add(this._link);
			this._sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
			this._sidePanel.Location = new System.Drawing.Point(0, 0);
			this._sidePanel.Name = "_sidePanel";
			this._sidePanel.Size = new System.Drawing.Size(176, 350);
			this._sidePanel.TabIndex = 10;
			// 
			// _supportUrl
			// 
			this._supportUrl.BackColor = System.Drawing.Color.Transparent;
			this._supportUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportUrl.LinkColor = System.Drawing.Color.White;
			this._supportUrl.Location = new System.Drawing.Point(8, 216);
			this._supportUrl.Name = "_supportUrl";
			this._supportUrl.Size = new System.Drawing.Size(205, 20);
			this._supportUrl.TabIndex = 10;
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
			this._supportPhone.TabIndex = 7;
			// 
			// _supportEmail
			// 
			this._supportEmail.BackColor = System.Drawing.Color.Transparent;
			this._supportEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportEmail.LinkColor = System.Drawing.Color.White;
			this._supportEmail.Location = new System.Drawing.Point(8, 256);
			this._supportEmail.Name = "_supportEmail";
			this._supportEmail.Size = new System.Drawing.Size(205, 23);
			this._supportEmail.TabIndex = 10;
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
			this._supportEmailLabel.TabIndex = 6;
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
			this._supportPhoneLabel.TabIndex = 6;
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
			this._supportUrlLabel.TabIndex = 6;
			this._supportUrlLabel.Text = "#UI_SU";
			// 
			// _link
			// 
			this._link.BackColor = System.Drawing.Color.Black;
			this._link.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._link.ForeColor = System.Drawing.Color.White;
			this._link.LinkArea = new System.Windows.Forms.LinkArea(13, 14);
			this._link.LinkColor = System.Drawing.Color.Yellow;
			this._link.Location = new System.Drawing.Point(0, 326);
			this._link.Name = "_link";
			this._link.Size = new System.Drawing.Size(176, 24);
			this._link.TabIndex = 14;
			this._link.TabStop = true;
			this._link.Text = "Protected By XHEO|Licensing";
			this._link.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._link_LinkClicked);
			// 
			// _ok
			// 
			this._ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._ok.Location = new System.Drawing.Point(400, 312);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(88, 23);
			this._ok.TabIndex = 11;
			this._ok.Text = "#UI_OK";
			this._ok.Click += new System.EventHandler(this._ok_Click);
			// 
			// _formTitle
			// 
			this._formTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._formTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._formTitle.Location = new System.Drawing.Point(184, 8);
			this._formTitle.Name = "_formTitle";
			this._formTitle.Size = new System.Drawing.Size(266, 28);
			this._formTitle.TabIndex = 12;
			this._formTitle.Text = "#UIS_Title";
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(184, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(400, 80);
			this.label1.TabIndex = 13;
			this.label1.Text = "#UIS_Message";
			// 
			// _location
			// 
			this._location.Location = new System.Drawing.Point(184, 160);
			this._location.Name = "_location";
			this._location.Size = new System.Drawing.Size(400, 20);
			this._location.TabIndex = 14;
			this._location.Text = "";
			// 
			// _fileLocationLabel
			// 
			this._fileLocationLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._fileLocationLabel.Location = new System.Drawing.Point(184, 144);
			this._fileLocationLabel.Name = "_fileLocationLabel";
			this._fileLocationLabel.Size = new System.Drawing.Size(240, 16);
			this._fileLocationLabel.TabIndex = 15;
			this._fileLocationLabel.Text = "#UIS_LicenseFileLabel";
			// 
			// _browse
			// 
			this._browse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._browse.Location = new System.Drawing.Point(504, 184);
			this._browse.Name = "_browse";
			this._browse.Size = new System.Drawing.Size(80, 23);
			this._browse.TabIndex = 16;
			this._browse.Text = "#UI_Browse";
			this._browse.Click += new System.EventHandler(this._browse_Click);
			// 
			// _cancel
			// 
			this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cancel.Location = new System.Drawing.Point(496, 312);
			this._cancel.Name = "_cancel";
			this._cancel.Size = new System.Drawing.Size(88, 23);
			this._cancel.TabIndex = 11;
			this._cancel.Text = "#UI_Cancel";
			// 
			// _thisAppOnly
			// 
			this._thisAppOnly.Checked = true;
			this._thisAppOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._thisAppOnly.Location = new System.Drawing.Point(208, 208);
			this._thisAppOnly.Name = "_thisAppOnly";
			this._thisAppOnly.Size = new System.Drawing.Size(264, 24);
			this._thisAppOnly.TabIndex = 17;
			this._thisAppOnly.TabStop = true;
			this._thisAppOnly.Text = "#UIS_ThisApp";
			// 
			// _allApps
			// 
			this._allApps.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._allApps.Location = new System.Drawing.Point(208, 232);
			this._allApps.Name = "_allApps";
			this._allApps.Size = new System.Drawing.Size(376, 48);
			this._allApps.TabIndex = 17;
			this._allApps.Text = "#UIS_AllApps";
			// 
			// _licenseOFD
			// 
			this._licenseOFD.Filter = "License Files (*.lic)|*.lic;*.lic.xml|All Files (*.*)|*.*";
			this._licenseOFD.Title = "Select License";
			// 
			// _licenseSFD
			// 
			this._licenseSFD.DefaultExt = "lic";
			this._licenseSFD.FileName = "License.lic";
			this._licenseSFD.Filter = "License Files (*.lic)|*.lic|All Files (*.*)|*.*";
			this._licenseSFD.Title = "Save License As...";
			// 
			// SelectExistingLicenseForm
			// 
			this.AcceptButton = this._ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._cancel;
			this.ClientSize = new System.Drawing.Size(600, 350);
			this.Controls.Add(this._thisAppOnly);
			this.Controls.Add(this._browse);
			this.Controls.Add(this._fileLocationLabel);
			this.Controls.Add(this._location);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._formTitle);
			this.Controls.Add(this._ok);
			this.Controls.Add(this._sidePanel);
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._allApps);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "SelectExistingLicenseForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "#UIS_Title";
			this._sidePanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Performs last minute loading steps before the form is displayed.
		/// </summary>
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
		}
		private void _browse_Click(object sender, System.EventArgs e)
		{
			if( _licenseOFD.ShowDialog( this ) == DialogResult.OK )
			{
				_location.Text = _licenseOFD.FileName;
			}
		}

		private void _ok_Click(object sender, System.EventArgs e)
		{
			if( _location.Text == null || _location.Text.Length == 0 ||
				! File.Exists( _location.Text ) )
			{
				MessageBox.Show(
					this,
					Internal.StaticResourceProvider.CurrentProvider.GetString( "ES_MustSelectLocation" ),
					"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				return;
			}

			string destination = null;

			if( _thisAppOnly.Checked )
			{
				if( _originalLicense.IsEmbedded )
					destination = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName( _location.Text ) );
				else
					destination = Path.Combine( Path.GetDirectoryName( _originalLicense.LicensePack.Location ), Path.GetFileName( _location.Text ) );
			}
			else
			{
				destination = Path.Combine( ExtendedLicense.SharedFolder, Path.GetFileName( _location.Text ) );
			}

			if( File.Exists( destination ) )
			{
				switch( MessageBox.Show( this, 
					Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "M_LicenseExistsOverwrite", _licenseSFD.FileName ),
					Internal.StaticResourceProvider.CurrentProvider.GetString( "M_LicenseExistsOverwriteTitle" ),
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question ) )
				{
					case DialogResult.Cancel:
						return;
					case DialogResult.No:
						_licenseSFD.FileName = destination;
						if( _licenseSFD.ShowDialog( this ) != DialogResult.OK )
							return;
						destination = _licenseSFD.FileName;
						break;
				}
			}

			try
			{
				if( ! Directory.Exists( Path.GetDirectoryName( destination ) ) )
					Directory.CreateDirectory( Path.GetDirectoryName( destination ) );

				File.Copy( _location.Text, destination, true );

				_originalLicense.SurrogateLicensePack = destination;
				DialogResult = DialogResult.OK;
			}
			catch( Exception ex )
			{
				MessageBox.Show(
					this,
					Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "ES_CouldNotCopy", ex.Message ),
					"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
			}
	
		}

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
			System.Diagnostics.Process.Start( "mailto:" + _supportEmail.Text );
		}
		
		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class SelectExistingLicenseForm
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright Copyright © 2002-2005 XHEO, INC. All Rights Reserved. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
