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
// Class:		TrialLimitForm
// Author:		Paul Alexander
// Created:		Sunday, November 17, 2002 6:16:49 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using System.Reflection;

namespace Xheo.Licensing
{
	/// <summary>
	/// Implements the GUI for a <see cref="TrialLimit"/>.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class TrialLimitForm : System.Windows.Forms.Form
	{

		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private TrialLimit _limit = null;
		private ExtendedLicense _registrationLicense = null;

		private System.Windows.Forms.Button _cancel;
		private System.Windows.Forms.Button _buyNow;
		private System.Windows.Forms.LinkLabel _link;
		private System.Windows.Forms.PictureBox _bitmap;
		private System.Windows.Forms.PictureBox _logo;
		private System.Windows.Forms.Button _try;
		private System.Windows.Forms.Button _info;
		private System.Windows.Forms.Label _daysLeft;
		private System.Windows.Forms.ProgressBar _daysProgress;
		private System.Windows.Forms.OpenFileDialog _licenseOFD;
		private System.Windows.Forms.SaveFileDialog _licenseSFD;
		private System.Windows.Forms.Label _littleWarning;
		private System.Windows.Forms.Label _expiredNotice;
		private System.Windows.Forms.Label _warning;
		private System.Windows.Forms.Label _company;
		private System.Windows.Forms.Label _title;
		private System.Windows.Forms.Panel _defaultPanel;
		private System.Windows.Forms.LinkLabel _alreadyHaveLink;
		private System.Windows.Forms.Button _register;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// Initializes a new instance of the TrialLimitForm class.
		///<summary>
		///Summary of TrialLimitForm.
		///</summary>
		public TrialLimitForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_defaultPanel.BackgroundImage = Limit.GetBitmapResource( Limit.MakeResourceString( "guisplash.jpg", typeof( ExtendedLicense ) ), null );
		}

		/// Initializes a new instance of the TrialLimitForm class.
		///<summary>
		///Summary of TrialLimitForm.
		///</summary>
		///<param name="limit"></param>
		public TrialLimitForm( TrialLimit limit ) : this()
		{
			_limit = limit;

			if( Form.ActiveForm != null )
				Icon = Form.ActiveForm.Icon;

			if( _limit.PurchaseUrl == null || _limit.PurchaseUrl.Length == 0 )
				_buyNow.Visible = false;
			if( DateTime.UtcNow > _limit.Ends )
			{
				_expiredNotice.Visible = true;
				_try.Enabled = false;
				_daysLeft.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "MT_TrialExpired" );
				_daysProgress.Value = _daysProgress.Minimum;
			}
			else
			{
				_expiredNotice.Visible = false;
				TimeSpan days			= DateTime.UtcNow - _limit.Started;
				TimeSpan allowedDays	= _limit.Ends - _limit.Started;
				_daysProgress.Maximum	= Convert.ToInt32( allowedDays.TotalDays, System.Globalization.CultureInfo.InvariantCulture );
				_daysProgress.Minimum	= 0;
				int value = Convert.ToInt32( allowedDays.TotalDays - days.TotalDays, System.Globalization.CultureInfo.InvariantCulture );
				if( value > Convert.ToInt32( allowedDays.TotalDays, System.Globalization.CultureInfo.InvariantCulture ) || value < 0 )
					throw new ExtendedLicenseException( "ET_InvalidDateRange" );
				_daysProgress.Value		= value;
				_daysLeft.Text			= Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MT_DaysRemaining", _daysProgress.Value );

				if( _daysProgress.Value > 0 )
				{
					_try.Enabled			= true;
				}
				else
				{
					_expiredNotice.Visible = true;
					_try.Enabled = false;
					_daysLeft.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "MT_TrialExpired" );
					_daysProgress.Value = _daysProgress.Minimum;
				}
			}

			if( ( _limit.Terms != null && _limit.Terms.Length > 0 ) ||
				( _limit.InfoUrl != null && _limit.InfoUrl.Length > 0 ) )
			{
				_info.Visible = true;
				_info.Enabled = true;
			}
			else
			{
				_info.Visible = false;
			}

			if( _limit.License.SignedByProLicense )
				_link.Text = "";

#if LICENSING || LICENSEREGISTRATIONS
			if( limit.ShowRegisterIfAvailable  )
			{
				if( limit.License.GetLimit( typeof( RegistrationLimit ) ) != null )
					_registrationLicense = limit.License;
				else if( limit.License.LicensePack != null )
					foreach( ExtendedLicense license in limit.License.LicensePack )
						if( license.GetLimit( typeof( RegistrationLimit ) ) != null )
						{
							_registrationLicense = license;
							break;
						}

				if( _registrationLicense != null )
					_register.Visible = true;
			}
#endif
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

		/// <summary>
		/// If logo or bitmap images are null, then replace them with defaults so it
		/// still looks professional.
		/// </summary>
		protected override void OnLoad( EventArgs e )
		{
			if( _logo.Image == null )
				_logo.Visible = false;

			if( _bitmap.Image == null )
			{
				
				_defaultPanel.BringToFront();
				AssemblyTitleAttribute titleAttribute = Attribute.GetCustomAttribute( _limit.License.LicensedType.Assembly, typeof( AssemblyTitleAttribute ), true ) as AssemblyTitleAttribute;
				if( titleAttribute == null )
					_title.Text = _limit.License.LicensedType.Assembly.GetName().Name;
				else
					_title.Text = titleAttribute.Title;

				SupportInfo info = LicenseHelpAttribute.GetSupportInfo( _limit.License.LicensedType );
				if( info.Company != "the manufacturer" )
					_company.Text = "By " + info.Company;
			}

			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._try = new System.Windows.Forms.Button();
			this._cancel = new System.Windows.Forms.Button();
			this._buyNow = new System.Windows.Forms.Button();
			this._link = new System.Windows.Forms.LinkLabel();
			this._info = new System.Windows.Forms.Button();
			this._logo = new System.Windows.Forms.PictureBox();
			this._daysLeft = new System.Windows.Forms.Label();
			this._daysProgress = new System.Windows.Forms.ProgressBar();
			this._bitmap = new System.Windows.Forms.PictureBox();
			this._littleWarning = new System.Windows.Forms.Label();
			this._licenseOFD = new System.Windows.Forms.OpenFileDialog();
			this._licenseSFD = new System.Windows.Forms.SaveFileDialog();
			this._expiredNotice = new System.Windows.Forms.Label();
			this._defaultPanel = new System.Windows.Forms.Panel();
			this._warning = new System.Windows.Forms.Label();
			this._company = new System.Windows.Forms.Label();
			this._title = new System.Windows.Forms.Label();
			this._alreadyHaveLink = new System.Windows.Forms.LinkLabel();
			this._register = new System.Windows.Forms.Button();
			this._defaultPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _try
			// 
			this._try.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._try.Enabled = false;
			this._try.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._try.Location = new System.Drawing.Point(476, 240);
			this._try.Name = "_try";
			this._try.Size = new System.Drawing.Size(92, 24);
			this._try.TabIndex = 2;
			this._try.Text = "#UIT_Try";
			this._try.Click += new System.EventHandler(this._try_Click);
			// 
			// _cancel
			// 
			this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cancel.Location = new System.Drawing.Point(476, 268);
			this._cancel.Name = "_cancel";
			this._cancel.Size = new System.Drawing.Size(92, 24);
			this._cancel.TabIndex = 3;
			this._cancel.Text = "#UI_Cancel";
			this._cancel.Click += new System.EventHandler(this._cancel_Click);
			// 
			// _buyNow
			// 
			this._buyNow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buyNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buyNow.Location = new System.Drawing.Point(476, 148);
			this._buyNow.Name = "_buyNow";
			this._buyNow.Size = new System.Drawing.Size(92, 24);
			this._buyNow.TabIndex = 0;
			this._buyNow.Text = "#UI_BuyNow";
			this._buyNow.Click += new System.EventHandler(this._buyNow_Click);
			// 
			// _link
			// 
			this._link.LinkArea = new System.Windows.Forms.LinkArea(13, 14);
			this._link.Location = new System.Drawing.Point(259, 312);
			this._link.Name = "_link";
			this._link.Size = new System.Drawing.Size(309, 27);
			this._link.TabIndex = 4;
			this._link.TabStop = true;
			this._link.Text = "Protected By XHEO|Licensing";
			this._link.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._link_LinkClicked);
			// 
			// _info
			// 
			this._info.Enabled = false;
			this._info.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._info.Location = new System.Drawing.Point(476, 212);
			this._info.Name = "_info";
			this._info.Size = new System.Drawing.Size(92, 24);
			this._info.TabIndex = 1;
			this._info.Text = "#UIT_Info";
			this._info.Click += new System.EventHandler(this._info_Click);
			// 
			// _logo
			// 
			this._logo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._logo.Cursor = System.Windows.Forms.Cursors.Hand;
			this._logo.Location = new System.Drawing.Point(476, 9);
			this._logo.Name = "_logo";
			this._logo.Size = new System.Drawing.Size(92, 92);
			this._logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this._logo.TabIndex = 6;
			this._logo.TabStop = false;
			// 
			// _daysLeft
			// 
			this._daysLeft.Location = new System.Drawing.Point(8, 312);
			this._daysLeft.Name = "_daysLeft";
			this._daysLeft.Size = new System.Drawing.Size(200, 26);
			this._daysLeft.TabIndex = 7;
			this._daysLeft.Text = "aaa";
			this._daysLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _daysProgress
			// 
			this._daysProgress.Location = new System.Drawing.Point(8, 336);
			this._daysProgress.Name = "_daysProgress";
			this._daysProgress.Size = new System.Drawing.Size(564, 26);
			this._daysProgress.TabIndex = 8;
			this._daysProgress.Value = 100;
			// 
			// _bitmap
			// 
			this._bitmap.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._bitmap.Location = new System.Drawing.Point(8, 8);
			this._bitmap.Name = "_bitmap";
			this._bitmap.Size = new System.Drawing.Size(460, 284);
			this._bitmap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this._bitmap.TabIndex = 9;
			this._bitmap.TabStop = false;
			// 
			// _littleWarning
			// 
			this._littleWarning.BackColor = System.Drawing.SystemColors.Control;
			this._littleWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._littleWarning.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this._littleWarning.Location = new System.Drawing.Point(8, 296);
			this._littleWarning.Name = "_littleWarning";
			this._littleWarning.Size = new System.Drawing.Size(560, 19);
			this._littleWarning.TabIndex = 10;
			this._littleWarning.Text = "#UIT_Warning";
			// 
			// _licenseOFD
			// 
			this._licenseOFD.DefaultExt = "lic";
			this._licenseOFD.Filter = "License Files (*.lic)|*.lic|All Files (*.*)|*.*";
			this._licenseOFD.Title = "Select License";
			// 
			// _licenseSFD
			// 
			this._licenseSFD.DefaultExt = "lic";
			this._licenseSFD.Filter = "License Files (*.lic)|*.lic|All Files (*.*)|*.*";
			this._licenseSFD.Title = "Save License";
			// 
			// _expiredNotice
			// 
			this._expiredNotice.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._expiredNotice.Location = new System.Drawing.Point(476, 104);
			this._expiredNotice.Name = "_expiredNotice";
			this._expiredNotice.Size = new System.Drawing.Size(92, 36);
			this._expiredNotice.TabIndex = 11;
			this._expiredNotice.Text = "#UIT_Expired";
			this._expiredNotice.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this._expiredNotice.Visible = false;
			// 
			// _defaultPanel
			// 
			this._defaultPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._defaultPanel.Controls.Add(this._warning);
			this._defaultPanel.Controls.Add(this._company);
			this._defaultPanel.Controls.Add(this._title);
			this._defaultPanel.Location = new System.Drawing.Point(8, 8);
			this._defaultPanel.Name = "_defaultPanel";
			this._defaultPanel.Size = new System.Drawing.Size(460, 284);
			this._defaultPanel.TabIndex = 12;
			// 
			// _warning
			// 
			this._warning.BackColor = System.Drawing.Color.Transparent;
			this._warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._warning.ForeColor = System.Drawing.Color.White;
			this._warning.Location = new System.Drawing.Point(6, 232);
			this._warning.Name = "_warning";
			this._warning.Size = new System.Drawing.Size(448, 40);
			this._warning.TabIndex = 20;
			this._warning.Text = "#UI_Warning";
			// 
			// _company
			// 
			this._company.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._company.BackColor = System.Drawing.Color.Transparent;
			this._company.ForeColor = System.Drawing.Color.White;
			this._company.Location = new System.Drawing.Point(27, 76);
			this._company.Name = "_company";
			this._company.Size = new System.Drawing.Size(419, 27);
			this._company.TabIndex = 19;
			// 
			// _title
			// 
			this._title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._title.BackColor = System.Drawing.Color.Transparent;
			this._title.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._title.ForeColor = System.Drawing.Color.White;
			this._title.Location = new System.Drawing.Point(8, 17);
			this._title.Name = "_title";
			this._title.Size = new System.Drawing.Size(438, 75);
			this._title.TabIndex = 18;
			// 
			// _alreadyHaveLink
			// 
			this._alreadyHaveLink.Location = new System.Drawing.Point(152, 312);
			this._alreadyHaveLink.Name = "_alreadyHaveLink";
			this._alreadyHaveLink.Size = new System.Drawing.Size(200, 26);
			this._alreadyHaveLink.TabIndex = 15;
			this._alreadyHaveLink.TabStop = true;
			this._alreadyHaveLink.Text = "#UI_AlreadyHaveLicense";
			this._alreadyHaveLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._alreadyHaveLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._alreadyHaveLink_LinkClicked);
			// 
			// _register
			// 
			this._register.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._register.Location = new System.Drawing.Point(476, 176);
			this._register.Name = "_register";
			this._register.Size = new System.Drawing.Size(92, 24);
			this._register.TabIndex = 0;
			this._register.Text = "#UIR_Register";
			this._register.Visible = false;
			this._register.Click += new System.EventHandler(this._register_Click);
			// 
			// TrialLimitForm
			// 
			this.AcceptButton = this._try;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._cancel;
			this.ClientSize = new System.Drawing.Size(578, 375);
			this.Controls.Add(this._expiredNotice);
			this.Controls.Add(this._littleWarning);
			this.Controls.Add(this._daysProgress);
			this.Controls.Add(this._logo);
			this.Controls.Add(this._try);
			this.Controls.Add(this._buyNow);
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._info);
			this.Controls.Add(this._bitmap);
			this.Controls.Add(this._defaultPanel);
			this.Controls.Add(this._alreadyHaveLink);
			this.Controls.Add(this._daysLeft);
			this.Controls.Add(this._link);
			this.Controls.Add(this._register);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TrialLimitForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "#UIT_Title";
			this._defaultPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the bitmap to display in the window. Should be 456 x 270.
		/// </summary>
		public Image Bitmap
		{
			get
			{
				return _bitmap.Image;
			}
			set
			{
				_bitmap.Image = value;
			}
		}

		/// <summary>
		/// Gets or sets the bitmap to display in the logo window. Should be 88 x 88.
		/// </summary>
		public Image Logo
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

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		///<summary>
		///Summary of _link_LinkClicked.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _link_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start( "http://www.xheo.com/products/licensing" );
		}

		///<summary>
		///Summary of _buyNow_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _buyNow_Click(object sender, System.EventArgs e)
		{
			string url = _limit.PurchaseUrl;
			if( url.IndexOf( '?' ) > -1 )
				url += '&';
			else
				url += '?';
			url += String.Format( System.Globalization.CultureInfo.InvariantCulture,
				"serialNumber={0}&profileHash={1}&culture={2}", 
				HttpUtility.UrlEncode( _limit.License.SerialNumber ),
				HttpUtility.UrlEncode( MachineProfile.Profile.GetComparableHash() ),
				System.Globalization.CultureInfo.CurrentUICulture.Name );
				
			if( _limit.UsePurchaseGui )
			{
				using( BuyNowForm form = new BuyNowForm( url, _limit.License ) )
				{
					if( form.ShowDialog( this ) == DialogResult.OK )
						DialogResult = DialogResult.Cancel;
				}
			}
			else
			{
				System.Diagnostics.Process.Start( url );
				if( MessageBox.Show( this, Internal.StaticResourceProvider.CurrentProvider.GetString( "MT_BNWarning" ), Internal.StaticResourceProvider.CurrentProvider.GetString( "MT_BNWarningTitle" ), MessageBoxButtons.OKCancel, MessageBoxIcon.Question ) == DialogResult.OK )
				{
					if( _licenseOFD.ShowDialog( this ) == DialogResult.OK )
					{
						_licenseSFD.FileName = Path.GetDirectoryName( _limit.License.LicensePack.Location ) + "\\" + Path.GetFileName( _licenseOFD.FileName );
						if( File.Exists( _licenseSFD.FileName ) )
							switch( MessageBox.Show( this,
									Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "M_LicenseExistsOverwrite", _licenseSFD.FileName ),
								Internal.StaticResourceProvider.CurrentProvider.GetString( "M_LicenseExistsOverwriteTitle" ),
								MessageBoxButtons.YesNoCancel,
								MessageBoxIcon.Question ) )
							{
								case DialogResult.Yes:
									break;
								case DialogResult.No:
									if( _licenseSFD.ShowDialog( this ) != DialogResult.OK )
										return;
									break;
								case DialogResult.Cancel:
									return;
							}
						if( _licenseOFD.FileName != _licenseSFD.FileName )
							File.Copy( _licenseOFD.FileName, _licenseSFD.FileName, true );
						DialogResult = DialogResult.Cancel;
						_limit.License.SurrogateLicensePack = _licenseOFD.FileName;
					}
				}
			}
		}

		///<summary>
		///Summary of _cancel_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _cancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		///<summary>
		///Summary of _try_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _try_Click(object sender, System.EventArgs e)
		{
			if( DateTime.UtcNow < _limit.Ends )
			{
				DialogResult = DialogResult.OK;
			}
			else
			{
				_try.Enabled = false;
				_expiredNotice.Visible = true;
				_daysLeft.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "MT_TrialExpired" );
				_daysProgress.Value = _daysProgress.Minimum;
			}
		}

		///<summary>
		///Summary of _info_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _info_Click(object sender, System.EventArgs e)
		{
			if( ( _limit.Terms == null || _limit.Terms.Length == 0 ) && _limit.InfoUrl != null && _limit.InfoUrl.Length > 0 )
				System.Diagnostics.Process.Start( _limit.InfoUrl );
			else
			{
				using( InfoForm form = new InfoForm( _limit.Terms, _limit.InfoUrl ) )
					form.ShowDialog( this);
			}
		}

		
		private void _alreadyHaveLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			using( SelectExistingLicenseForm form = new SelectExistingLicenseForm( _limit.License ) )
			{
				form.Owner = this;
				if( form.ShowDialog( this ) == DialogResult.OK )
					DialogResult = DialogResult.Cancel;
			}
		}

		private void _register_Click(object sender, System.EventArgs e)
		{
#if LICENSING || LICENSEREGISTRATIONS
			if( _registrationLicense != null && _registrationLicense.ShowRegistrationForm() )
			{
				DialogResult = DialogResult.Cancel;
				if( _limit.License.SurrogateLicensePack == null || _limit.License.SurrogateLicensePack.Length == 0 )
					_limit.License.SurrogateLicensePack = _limit.License.LicensePack.Location;
			}
#endif
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class TrialLimitForm
} // End namespace Xheo.Licensing.Design

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////