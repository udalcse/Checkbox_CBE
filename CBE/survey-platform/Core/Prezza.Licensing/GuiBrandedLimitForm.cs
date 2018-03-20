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
// Class:		GuiBrandedLimitForm
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
	/// Implements the Form for the <see cref="GuiBrandedLimit"/>.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class GuiBrandedLimitForm : System.Windows.Forms.Form
	{

		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private System.Windows.Forms.Button _continue;
		private System.Windows.Forms.Button _cancel;
		private System.Windows.Forms.Button _buyNow;
		private System.Timers.Timer timer1;
		private System.Windows.Forms.LinkLabel _link;

		private GuiBrandedLimit	_limit	= null;
		private int	_delay;
		private System.Windows.Forms.PictureBox _bitmap;
		private System.Windows.Forms.LinkLabel _terms;
		private System.Windows.Forms.SaveFileDialog _licenseSFD;
		private System.Windows.Forms.OpenFileDialog _licenseOFD;
		private System.Windows.Forms.Panel _defaultPanel;
		private System.Windows.Forms.Label _warning;
		private System.Windows.Forms.Label _company;
		private System.Windows.Forms.Label _title;
		private System.Windows.Forms.LinkLabel _alreadyHaveLink;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Included for COM compliance, do not use.
		/// </summary>
		public GuiBrandedLimitForm()
		{
			throw new NotSupportedException( "Use GuiBrandedLimitForm( GuiBrandedLimit ) instead." );
		}

		/// Initializes a new instance of the GuiBrandedLimitForm class.
		///<summary>
		///Summary of GuiBrandedLimitForm.
		///</summary>
		///<param name="limit"></param>
		public GuiBrandedLimitForm( GuiBrandedLimit limit ) : base()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );
			
			if( Form.ActiveForm != null )
				Icon = Form.ActiveForm.Icon;

			_limit = limit;

			if( limit.ContinueDelay <= 0 )
			{
				timer1.Enabled = false;
				_continue.Enabled = true;
			}
			else
			{
				_delay = limit.ContinueDelay;
			}

			if( limit.PurchaseUrl == null || limit.PurchaseUrl.Length == 0 )
				_buyNow.Visible = false;

			if( limit.License.SignedByProLicense )
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

		/// <summary>
		/// If the bitmap image is null, then replace it with a default so it still looks professional.
		/// </summary>
		protected override void OnLoad( EventArgs e )
		{
			SupportInfo info = LicenseHelpAttribute.GetSupportInfo( _limit.License.LicensedType );
			if( _bitmap.Image == null )
			{
				_bitmap.SendToBack();
				_defaultPanel.BackgroundImage = Limit.GetBitmapResource( "Xheo.Licensing.guisplash.jpg,Xheo.Licensing", null );

				
				if( info.Company != "the manufacturer" )
					_company.Text = "By " + info.Company;
			}

			_title.Text = info.Product;

			Text = _title.Text;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._continue = new System.Windows.Forms.Button();
			this._cancel = new System.Windows.Forms.Button();
			this._buyNow = new System.Windows.Forms.Button();
			this.timer1 = new System.Timers.Timer();
			this._link = new System.Windows.Forms.LinkLabel();
			this._bitmap = new System.Windows.Forms.PictureBox();
			this._terms = new System.Windows.Forms.LinkLabel();
			this._licenseSFD = new System.Windows.Forms.SaveFileDialog();
			this._licenseOFD = new System.Windows.Forms.OpenFileDialog();
			this._defaultPanel = new System.Windows.Forms.Panel();
			this._warning = new System.Windows.Forms.Label();
			this._company = new System.Windows.Forms.Label();
			this._title = new System.Windows.Forms.Label();
			this._alreadyHaveLink = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
			this._defaultPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _continue
			// 
			this._continue.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._continue.Enabled = false;
			this._continue.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._continue.Location = new System.Drawing.Point(296, 312);
			this._continue.Name = "_continue";
			this._continue.Size = new System.Drawing.Size(80, 24);
			this._continue.TabIndex = 0;
			this._continue.Text = "#UI_Continue";
			// 
			// _cancel
			// 
			this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cancel.Location = new System.Drawing.Point(384, 312);
			this._cancel.Name = "_cancel";
			this._cancel.Size = new System.Drawing.Size(80, 24);
			this._cancel.TabIndex = 1;
			this._cancel.Text = "#UI_Cancel";
			// 
			// _buyNow
			// 
			this._buyNow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buyNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._buyNow.Location = new System.Drawing.Point(8, 312);
			this._buyNow.Name = "_buyNow";
			this._buyNow.Size = new System.Drawing.Size(96, 24);
			this._buyNow.TabIndex = 2;
			this._buyNow.Text = "#UI_BuyNow";
			this._buyNow.Click += new System.EventHandler(this._buyNow_Click);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.SynchronizingObject = this;
			this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// _link
			// 
			this._link.LinkArea = new System.Windows.Forms.LinkArea(13, 14);
			this._link.Location = new System.Drawing.Point(280, 8);
			this._link.Name = "_link";
			this._link.Size = new System.Drawing.Size(184, 16);
			this._link.TabIndex = 4;
			this._link.TabStop = true;
			this._link.Text = "Protected By XHEO|Licensing";
			this._link.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this._link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._link_LinkClicked);
			// 
			// _bitmap
			// 
			this._bitmap.Location = new System.Drawing.Point(8, 24);
			this._bitmap.Name = "_bitmap";
			this._bitmap.Size = new System.Drawing.Size(456, 280);
			this._bitmap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this._bitmap.TabIndex = 6;
			this._bitmap.TabStop = false;
			// 
			// _terms
			// 
			this._terms.Location = new System.Drawing.Point(104, 312);
			this._terms.Name = "_terms";
			this._terms.Size = new System.Drawing.Size(184, 24);
			this._terms.TabIndex = 3;
			this._terms.TabStop = true;
			this._terms.Text = "#UIB_ContinueTerms";
			this._terms.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._terms.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._terMSL_LinkClicked);
			// 
			// _licenseSFD
			// 
			this._licenseSFD.DefaultExt = "lic";
			this._licenseSFD.Filter = "License Files (*.lic)|*.lic|All Files (*.*)|*.*";
			this._licenseSFD.Title = "Save License";
			// 
			// _licenseOFD
			// 
			this._licenseOFD.DefaultExt = "lic";
			this._licenseOFD.Filter = "License Files (*.lic)|*.lic|All Files (*.*)|*.*";
			this._licenseOFD.Title = "Select License";
			// 
			// _defaultPanel
			// 
			this._defaultPanel.Controls.Add(this._warning);
			this._defaultPanel.Controls.Add(this._company);
			this._defaultPanel.Controls.Add(this._title);
			this._defaultPanel.Location = new System.Drawing.Point(8, 24);
			this._defaultPanel.Name = "_defaultPanel";
			this._defaultPanel.Size = new System.Drawing.Size(456, 280);
			this._defaultPanel.TabIndex = 13;
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
			this._alreadyHaveLink.Location = new System.Drawing.Point(8, 8);
			this._alreadyHaveLink.Name = "_alreadyHaveLink";
			this._alreadyHaveLink.Size = new System.Drawing.Size(200, 16);
			this._alreadyHaveLink.TabIndex = 14;
			this._alreadyHaveLink.TabStop = true;
			this._alreadyHaveLink.Text = "#UI_AlreadyHaveLicense";
			this._alreadyHaveLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._alreadyHaveLink_LinkClicked);
			// 
			// GuiBrandedLimitForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(474, 344);
			this.Controls.Add(this._alreadyHaveLink);
			this.Controls.Add(this._buyNow);
			this.Controls.Add(this._terms);
			this.Controls.Add(this._link);
			this.Controls.Add(this._continue);
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._bitmap);
			this.Controls.Add(this._defaultPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GuiBrandedLimitForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
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
		
		private void _alreadyHaveLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			using( SelectExistingLicenseForm form = new SelectExistingLicenseForm( _limit.License ) )
			{
				form.Owner = this;
				if( form.ShowDialog( this ) == DialogResult.OK )
					DialogResult = DialogResult.Cancel;
			}
		}

		///<summary>
		///Summary of timer1_Elapsed.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_delay--;
			_continue.Text = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "UIB_Continuing",  _delay );
			if( _delay < 1 )
			{
				_continue.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UI_Continue" );
				_continue.Enabled = true;
				timer1.Enabled = false;
			}
		}

		///<summary>
		///Summary of _terMSL_LinkClicked.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _terMSL_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			using( InfoForm info = new InfoForm( _limit.Terms, null ) )
				info.ShowDialog( this );
		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class GuiBrandedLimitForm
} // End namespace Xheo.Licensing.Design

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////