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
// Class:		BuyNowForm
// Author:		Paul Alexander
// Created:		Monday, January 06, 2003 8:00:37 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;

namespace Xheo.Licensing
{
	/// <summary>
	/// Implements a form for the user to purchase a license directly in the application.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class BuyNowForm : System.Windows.Forms.Form
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private System.Windows.Forms.StatusBarPanel _textPanel;
		private System.Windows.Forms.StatusBarPanel _progressPanel;

		private ExtendedLicense _originalLicense	= null;
		private string			_purchaseUrl		= null;
		private System.Windows.Forms.StatusBar _status;
		private Internal.SmallIe _browser = null;
		private System.Windows.Forms.Panel _browserPanel;
		private System.Windows.Forms.ProgressBar _downloadProgress;
		private System.Windows.Forms.StatusBarPanel _secureIcon;
		//private Icon _secureImage = null;
		private System.Windows.Forms.SaveFileDialog _licenseSFD;
		private bool _downloadingLicense = false;


		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label _productLabel;
		private System.Windows.Forms.Label _companyLabel;
		private System.Windows.Forms.Timer _downloadTimer;
		private System.Windows.Forms.ImageList _images;
		private System.Windows.Forms.ToolBarButton _back;
		private System.Windows.Forms.ToolBarButton _forward;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton _stop;
		private System.Windows.Forms.ToolBarButton _refresh;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton _print;
		private System.Windows.Forms.ToolBar _toolbar;
		private System.ComponentModel.IContainer components;

		private Bitmap	_buyNowTitle	= null;
		private System.Windows.Forms.LinkLabel _alreadyHaveLink;
		private Bitmap	_buyNowTile		= null;
		
		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		///<summary>
		///Summary of BuyNowForm.
		///</summary>
		static BuyNowForm()
		{
			//System.Diagnostics.Debug.Assert( false );
			//AxSHDocVw.AxWebBrowser.SHDocVwAssembly.GetType( "SHDocVw.WebBrowserClass" );
		}

		/// <summary>
		/// Included for COM compliance only, do not use. 
		/// </summary>
		public BuyNowForm()
		{
			throw new NotSupportedException( "Use BuyNowForm( string, ExtendedLicense ) instead." );
		}

		/// <summary>
		/// Initializes a new instance of the BuyNowForm class.
		/// </summary>
		/// <param name="purchaseUrl">
		///		The URL where licenses can be purchased on-demand.
		/// </param>
		/// <param name="originalLicense">
		///		The original license that will be overridden with the new purchased license.
		/// </param>
		public BuyNowForm( string purchaseUrl, ExtendedLicense originalLicense )
		{
			_browser = new Internal.SmallIe(); // new AxSHDocVw.AxWebBrowser();
			InitializeComponent();
			_buyNowTitle	= Limit.GetBitmapResource( Limit.MakeResourceString( "buynow.jpg", typeof( ExtendedLicense ) ), null ) as Bitmap;
			_buyNowTile		= Limit.GetBitmapResource( Limit.MakeResourceString( "buynowtile.jpg", typeof( ExtendedLicense ) ), null ) as Bitmap;
			panel1.BackgroundImage = _buyNowTile;
			panel2.BackgroundImage = _buyNowTitle;

			if( Form.ActiveForm != null )
				Icon = Form.ActiveForm.Icon;

			SupportInfo supportInfo = LicenseHelpAttribute.GetSupportInfo( originalLicense.LicensedType );

			if( supportInfo.Company != "the manufacturer" )
				_companyLabel.Text = "by " + supportInfo.Company;

			_productLabel.Text = supportInfo.Product;
			
			_browser.BeforeNavigate += new Internal.SmallIe.BeforeNavigateHandler(_browser_BeforeNavigate);

			_back.Enabled		= false;
			_forward.Enabled	= false;
			_images.Images.AddStrip( Limit.GetBitmapResource( Limit.MakeResourceString( "icons24.png", typeof( ExtendedLicense ) ), null ) );

			_back.ImageIndex = 0;
			_forward.ImageIndex = 1;
			_stop.ImageIndex = 2;
			_refresh.ImageIndex = 3;
			_print.ImageIndex = 4;

			_purchaseUrl		= purchaseUrl;
			_originalLicense	= originalLicense;
			_secureIcon.Icon	= null;
			_secureIcon.Text	= null;

			_browser.Navigate( "about:blank" );
			_browser.Navigate( purchaseUrl );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._status = new System.Windows.Forms.StatusBar();
			this._textPanel = new System.Windows.Forms.StatusBarPanel();
			this._progressPanel = new System.Windows.Forms.StatusBarPanel();
			this._secureIcon = new System.Windows.Forms.StatusBarPanel();
			this._browser = new Internal.SmallIe();
			this._browserPanel = new System.Windows.Forms.Panel();
			this._toolbar = new System.Windows.Forms.ToolBar();
			this._back = new System.Windows.Forms.ToolBarButton();
			this._forward = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this._stop = new System.Windows.Forms.ToolBarButton();
			this._refresh = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this._print = new System.Windows.Forms.ToolBarButton();
			this._images = new System.Windows.Forms.ImageList(this.components);
			this._downloadProgress = new System.Windows.Forms.ProgressBar();
			this._licenseSFD = new System.Windows.Forms.SaveFileDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this._productLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this._companyLabel = new System.Windows.Forms.Label();
			this._downloadTimer = new System.Windows.Forms.Timer(this.components);
			this._alreadyHaveLink = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this._textPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._progressPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._secureIcon)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._browser)).BeginInit();
			this._browserPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _status
			// 
			this._status.Location = new System.Drawing.Point(0, 431);
			this._status.Name = "_status";
			this._status.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																					   this._textPanel,
																					   this._progressPanel,
																					   this._secureIcon});
			this._status.ShowPanels = true;
			this._status.Size = new System.Drawing.Size(632, 22);
			this._status.TabIndex = 3;
			// 
			// _textPanel
			// 
			this._textPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this._textPanel.MinWidth = 300;
			this._textPanel.Width = 444;
			// 
			// _progressPanel
			// 
			this._progressPanel.MinWidth = 150;
			this._progressPanel.Width = 150;
			// 
			// _secureIcon
			// 
			this._secureIcon.MinWidth = 16;
			this._secureIcon.ToolTipText = "#UIB_ConnectionSecure";
			this._secureIcon.Width = 22;
			// 
			// _browser
			// 
			this._browser.ContainingControl = this;
			this._browser.Dock = System.Windows.Forms.DockStyle.Fill;
			this._browser.Enabled = true;
			this._browser.Location = new System.Drawing.Point(0, 36);
			this._browser.Name = "_browser";
			this._browser.Size = new System.Drawing.Size(632, 315);
			this._browser.TabIndex = 4;
			this._browser.StatusTextChanged += new Internal.SmallIe.TextChangedHandler(_browser_StatusTextChanged);
			this._browser.ProgressChanged += new Internal.SmallIe.ProgressChangedHandler(_browser_ProgressChanged);
			this._browser.CommandStateChanged += new Internal.SmallIe.CommandStateChangedHandler(_browser_CommandStateChanged);
			// 
			// _browserPanel
			// 
			this._browserPanel.Controls.Add(this._browser);
			this._browserPanel.Controls.Add(this._toolbar);
			this._browserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._browserPanel.Location = new System.Drawing.Point(0, 80);
			this._browserPanel.Name = "_browserPanel";
			this._browserPanel.Size = new System.Drawing.Size(632, 351);
			this._browserPanel.TabIndex = 6;
			// 
			// _toolbar
			// 
			this._toolbar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this._toolbar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this._back,
																						this._forward,
																						this.toolBarButton1,
																						this._stop,
																						this._refresh,
																						this.toolBarButton2,
																						this._print});
			this._toolbar.DropDownArrows = true;
			this._toolbar.ImageList = this._images;
			this._toolbar.Location = new System.Drawing.Point(0, 0);
			this._toolbar.Name = "_toolbar";
			this._toolbar.ShowToolTips = true;
			this._toolbar.Size = new System.Drawing.Size(632, 36);
			this._toolbar.TabIndex = 5;
			this._toolbar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
			this._toolbar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this._toolbar_ButtonClick);
			// 
			// _back
			// 
			this._back.Text = "#UIB_Back";
			this._back.ToolTipText = "#UIB_Back";
			// 
			// _forward
			// 
			this._forward.ToolTipText = "#UIB_Forward";
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// _stop
			// 
			this._stop.ToolTipText = "#UIB_Stop";
			// 
			// _refresh
			// 
			this._refresh.ToolTipText = "#UIB_Refresh";
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// _print
			// 
			this._print.ToolTipText = "#UIB_Print";
			// 
			// _images
			// 
			this._images.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._images.ImageSize = new System.Drawing.Size(24, 24);
			this._images.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _downloadProgress
			// 
			this._downloadProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._downloadProgress.Location = new System.Drawing.Point(448, 435);
			this._downloadProgress.Name = "_downloadProgress";
			this._downloadProgress.Size = new System.Drawing.Size(145, 16);
			this._downloadProgress.TabIndex = 9;
			// 
			// _licenseSFD
			// 
			this._licenseSFD.DefaultExt = "lic";
			this._licenseSFD.FileName = "License";
			this._licenseSFD.Filter = "License Files (*.lic)|*.lic|All Files (*.*)|*.*";
			this._licenseSFD.Title = "Save License";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._productLabel);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this._companyLabel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(632, 80);
			this.panel1.TabIndex = 10;
			// 
			// _productLabel
			// 
			this._productLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._productLabel.BackColor = System.Drawing.Color.Transparent;
			this._productLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._productLabel.ForeColor = System.Drawing.Color.White;
			this._productLabel.Location = new System.Drawing.Point(344, 32);
			this._productLabel.Name = "_productLabel";
			this._productLabel.Size = new System.Drawing.Size(272, 16);
			this._productLabel.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(344, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(144, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "#UIB_YouAreNowBuying";
			// 
			// panel2
			// 
			this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(312, 80);
			this.panel2.TabIndex = 0;
			// 
			// _companyLabel
			// 
			this._companyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._companyLabel.BackColor = System.Drawing.Color.Transparent;
			this._companyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._companyLabel.ForeColor = System.Drawing.Color.White;
			this._companyLabel.Location = new System.Drawing.Point(344, 48);
			this._companyLabel.Name = "_companyLabel";
			this._companyLabel.Size = new System.Drawing.Size(272, 16);
			this._companyLabel.TabIndex = 2;
			// 
			// _downloadTimer
			// 
			this._downloadTimer.Tick += new System.EventHandler(this._downloadTimer_Tick);
			// 
			// _alreadyHaveLink
			// 
			this._alreadyHaveLink.BackColor = System.Drawing.Color.Transparent;
			this._alreadyHaveLink.Location = new System.Drawing.Point(424, 91);
			this._alreadyHaveLink.Name = "_alreadyHaveLink";
			this._alreadyHaveLink.Size = new System.Drawing.Size(200, 16);
			this._alreadyHaveLink.TabIndex = 15;
			this._alreadyHaveLink.TabStop = true;
			this._alreadyHaveLink.Text = "#UI_AlreadyHaveLicense";
			this._alreadyHaveLink.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._alreadyHaveLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._alreadyHaveLink_LinkClicked);
			// 
			// BuyNowForm
			// 
            this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(632, 453);
			this.Controls.Add(this._alreadyHaveLink);
			this.Controls.Add(this._downloadProgress);
			this.Controls.Add(this._browserPanel);
			this.Controls.Add(this._status);
			this.Controls.Add(this.panel1);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "BuyNowForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "#UIB_BuyNowTitle";
			((System.ComponentModel.ISupportInitialize)(this._textPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._progressPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._secureIcon)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._browser)).EndInit();
			this._browserPanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Releases any managed/unmanaged resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				if( _buyNowTitle != null )
					_buyNowTitle.Dispose();
				if( _buyNowTile != null )
					_buyNowTile.Dispose();
			}
			base.Dispose (disposing);
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
		/// Initializes the form.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );
		}


		///<summary>
		///Summary of _back_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _back_Click( object sender, EventArgs e )
		{
			try
			{
				_browser.GoBack();
			}
			catch
			{
				_back.Enabled = false;
			}
		}

		///<summary>
		///Summary of _forward_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _forward_Click( object sender, EventArgs e )
		{
			try
			{
				_browser.GoForward();
			}
			catch
			{
				_forward.Enabled = false;
			}
		}

		///<summary>
		///Summary of _refresh_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _refresh_Click( object sender, EventArgs e )
		{
			try
			{
				object level = 3;
				_browser.RefreshBrowser();
			}
			catch
			{}
		}

		///<summary>
		///Summary of _stop_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _stop_Click( object sender, EventArgs e )
		{
			_browser.Stop();
			_downloadingLicense = false;
		}

		///<summary>
		///Summary of _print_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _print_Click( object sender, EventArgs e )
		{
			_browser.Print( false );
		}

		private void _browser_CommandStateChanged(object sender, Internal.SmallIe.IeToolbarCommand command, bool enabled)
		{
			switch( command )
			{
				case Internal.SmallIe.IeToolbarCommand.NavigateForward : // Forward
					_forward.Enabled = enabled;
					break;
				case Internal.SmallIe.IeToolbarCommand.NavigateBackward : // Back
					_back.Enabled = enabled;
					break;
			}
		}

		
		private void _browser_ProgressChanged(object sender, int progress, int max)
		{
			if( progress != -1 &&
				max > 0 )
			{
				_downloadProgress.Maximum = max;
				_downloadProgress.Value = progress;
				_downloadProgress.Visible = true;
			}
			else
			{
				_downloadProgress.Value = 0;
				_downloadProgress.Visible = false;
			}
		}
		
		private void _browser_StatusTextChanged(object sender, string text)
		{
			_textPanel.Text = text;
			if( _downloadingLicense && ( text == null || text.Length == 0 ) )
				_textPanel.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UIB_DownloadingLicense" );
		}

//		///<summary>
//		///Summary of _browser_SetSecureLockIcon.
//		///</summary>
//		///<param name="sender"></param>
//		///<param name="e"></param>
//		private void _browser_SetSecureLockIcon(object sender, Internal.SmallIe._SetSecureLockIconEvent e)
//		{
//			if( e.secureLockIcon != 0 )
//			{
//				_secureIcon.Icon			= _secureImage;
//				_secureIcon.ToolTipText		= Internal.StaticResourceProvider.CurrentProvider.GetString( "UIB_PageSecure" );
//			}
//			else
//			{
//				_secureIcon.Icon			= null;
//				_secureIcon.ToolTipText		= Internal.StaticResourceProvider.CurrentProvider.GetString( "UIB_PageNotSecure" );
//			}
//		}

		
		private bool _browser_BeforeNavigate(object sender, string url, string targetFrame, byte[] postData, string headers)
		{
			bool cancel = true;
			if( url != null )
			{
				if( ! _downloadingLicense && url.ToLower( System.Globalization.CultureInfo.InvariantCulture ).StartsWith( "downloadlicense:" ) )
				{
					
					Uri uri = new Uri( _browser.LocationUrl, url.Substring( 16 ) );

					WebRequest request = WebRequest.Create( uri );
					request.Timeout = 60000;
					object state = new Object();
					_downloadProgress.Visible = true;
					_downloadProgress.Maximum = 100;
					_downloadProgress.Value = 0;
					_downloadTimer.Enabled = true;
					_downloadingLicense = true;
					_textPanel.Text = Internal.StaticResourceProvider.CurrentProvider.GetString( "UIB_DownloadingLicense" );
					try
					{
						DateTime started = DateTime.UtcNow;
						IAsyncResult handle = request.BeginGetResponse( null, state );
						while( ! handle.AsyncWaitHandle.WaitOne( 0, false ) && _downloadingLicense )
						{
							Application.DoEvents();
							if( ( DateTime.UtcNow - started ).TotalMilliseconds > 60000 )
								throw new ExtendedLicenseException( "EB_RequestTimedOut" );
							System.Threading.Thread.Sleep( 10 );
						}
						_downloadProgress.Value = 100;
						_downloadTimer.Enabled = false;
						if( _downloadingLicense )
						{
							using( WebResponse response = request.EndGetResponse( handle ) )
							{
								StreamReader reader = new StreamReader( response.GetResponseStream() );
								string licenseXml = reader.ReadToEnd();

								ExtendedLicensePack pack = new ExtendedLicensePack();
								pack.FromXmlString( licenseXml );
								
								string cd			= response.Headers[ "Content-Disposition" ];
								string destination	= null;
						
								if( cd != null && cd.Length > 0 )
								{
									int index = cd.IndexOf( "filename=" );
									int endindex = -1;
									if( index > -1 )
									{
										index += 9;
										if( cd[ index ] == '\"' )
										{
											index++;
											endindex = cd.IndexOf( '\"', index );
										}
										else
										{
											endindex = cd.IndexOf( ';', index );
										}

										if( endindex == -1 )
											endindex = cd.Length;

										destination = cd.Substring( index, endindex - index );
									}
								}

								if( destination == null )
									destination = Path.GetFileName( ( _browser.LocationUrl.AbsolutePath ) );

								destination = Path.Combine( Path.GetDirectoryName( _originalLicense.LicensePack.Location ), destination );
								destination = destination.Replace( "isolated:", "" );
								if( File.Exists( destination ) )
									switch( MessageBox.Show( this, 
										Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "M_LicenseExistsOverwrite", _licenseSFD.FileName ),
										Internal.StaticResourceProvider.CurrentProvider.GetString( "M_LicenseExistsOverwriteTitle" ),
										MessageBoxButtons.YesNoCancel,
										MessageBoxIcon.Question ) )
									{
										case DialogResult.Cancel:
											return cancel;
										case DialogResult.No:
											if( _licenseSFD.ShowDialog( this ) != DialogResult.OK )
												return cancel;
											break;
									}

								pack.Save( destination, true );
								_originalLicense.SurrogateLicensePack = destination;

								if( MessageBox.Show( this,
									Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MB_SaveBackup", destination ),
									Internal.StaticResourceProvider.CurrentProvider.GetString( "MB_SaveBackupTitle" ),
									MessageBoxButtons.YesNo,
									MessageBoxIcon.Question ) == DialogResult.Yes )
								{
									try
									{
										string source = destination;
										if( _licenseSFD.ShowDialog( this ) == DialogResult.OK )
										{
											if( _licenseSFD.FileName != source )
												File.Copy( source, _licenseSFD.FileName, true );
										}
									}
									catch( Exception ex )
									{
										MessageBox.Show( this,
											Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "EB_CouldNotBackup", ex.Message ),
											"Error",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error );
									}
								}

								DialogResult = DialogResult.OK;
							}
						}
					}
					catch( Exception ex )
					{
						_downloadTimer.Enabled = false;
						MessageBox.Show(
							this,
							Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "EB_CouldNotDownload", ex.Message ),
							"Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error );
					}
					finally
					{
						_textPanel.Text = "";
						_downloadingLicense = false;
						_downloadProgress.Visible = false;
						_downloadTimer.Enabled = false;
					}
				}
			}

			return cancel;
		}

		private void _downloadTimer_Tick(object sender, System.EventArgs e)
		{
			_downloadProgress.Increment( 1 );
			if( _downloadProgress.Value == _downloadProgress.Maximum )
				_downloadProgress.Value = 0;
		}

		private void _toolbar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if( e.Button == _refresh )
				_refresh_Click( sender, e );
			else if( e.Button == _stop )
				_stop_Click( sender, e );
			else if( e.Button == _print )
				_print_Click( sender, e );
			else if( e.Button == _back )
				_back_Click( sender, e );
			else if( e.Button == _forward )
				_forward_Click( sender, e );
		}

		private void _alreadyHaveLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			using( SelectExistingLicenseForm form = new SelectExistingLicenseForm( _originalLicense ) )
			{
				form.Owner = this;
				if( form.ShowDialog( this ) == DialogResult.OK )
					DialogResult = DialogResult.OK;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class BuyNowForm
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////