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
// Class:		FailureReportForm
// Author:		Paul Alexander
// Created:		Monday, November 18, 2002 3:35:01 AM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;
using System.Web.Services.Protocols;
using System.Diagnostics;
using Internal;

namespace Xheo.Licensing
{
	/// <summary>
	/// Displays a license violation and report summary when no valid licenses can
	/// be found.
	/// </summary>
#if LICENSING
	public
#else
	internal
#endif
	class FailureReportForm : System.Windows.Forms.Form
	{

		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private static bool _dontShowAgain	= false;
		private static bool _firstTime		= true;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox _dontShow;
		private System.Windows.Forms.LinkLabel _supportEmail;
		private System.Windows.Forms.Label _supportPhone;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel _supportUrl;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.TabControl _tabs;
		private System.Windows.Forms.TabPage _detailsTab;
		private System.Windows.Forms.TabPage _assembliesTab;
		private System.Windows.Forms.ListView _assemblyList;
		private System.Windows.Forms.ColumnHeader _assemblyHeader;
		private System.Windows.Forms.ColumnHeader _versionHeader;
		private System.Windows.Forms.ColumnHeader _locationHeader;
		private System.Windows.Forms.Panel _headerPanel;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.TabPage _systemInfoTab;
		private System.Windows.Forms.Button _copy;
		private System.Windows.Forms.TextBox _systemInfo;
		private System.Windows.Forms.TextBox _details;
		private System.Windows.Forms.ColumnHeader _fileVersionHeader;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		///	<summary>
		/// Initializes a new instance of the FailureReportForm class.
		///	</summary>
		public FailureReportForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );
			_headerPanel.BackgroundImage = Limit.GetBitmapResource( Limit.MakeResourceString( "FailureTile.jpg", typeof( ExtendedLicense ) ), null );
		}

		/// <summary>
		/// Initializes a new instance of the FailureReportForm class.
		/// </summary>
		/// <param name="context">
		///		The current LicenseContext.
		/// </param>
		/// <param name="type">
		///		The Type requesting a license.
		/// </param>
		/// <param name="instance">
		///		The instance requesting the license.
		/// </param>
		/// <param name="reason">
		///		Reason why licensing failed.
		/// </param>
		public FailureReportForm( LicenseContext context, Type type, object instance, string reason )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );
			_headerPanel.BackgroundImage = Limit.GetBitmapResource( Limit.MakeResourceString( "FailureTile.jpg", typeof( ExtendedLicense ) ), null );

			SupportInfo supportInfo = LicenseHelpAttribute.GetSupportInfo( type );

			_details.Text = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "EF_NoLicense",
				type.Name,
				supportInfo.Company );

			if( supportInfo.IncludeDetails && reason != null && reason.Length > 0 )
				_details.Text += Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "EF_Details", reason );

			_supportUrl.Text	= supportInfo.Url;
			_supportPhone.Text	= supportInfo.Phone;
			_supportEmail.Text	= supportInfo.Email;

			if( supportInfo.IncludeAssemblies )
				InitAssemblies();
			else
				_tabs.TabPages.Remove( _assembliesTab );

			if( supportInfo.IncludeSystemInfo )
				InitSystemInfo();
			else
				_tabs.TabPages.Remove( _systemInfoTab );

			if( ! _firstTime )
				_dontShow.Checked = DontShowAgain;
			else
				_firstTime = false;

		}

		/// <summary>
		/// Initializes a new instance of the FailureReportForm class.
		/// </summary>
		/// <param name="details">
		///		Detailed information about the failure.
		/// </param>
		/// <param name="message">
		///		The message to display to the user.
		/// </param>
		/// <param name="theAssembly">
		///		The assembly the error should be reported for. Used to gather support
		///		information such as email, phone and url.
		/// </param>
		public FailureReportForm( string message, string details, Assembly theAssembly )
		{
			InitializeComponent();
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );
			_headerPanel.BackgroundImage = Limit.GetBitmapResource( Limit.MakeResourceString( "FailureTile.jpg", typeof( ExtendedLicense ) ), null );

			SupportInfo supportInfo = LicenseHelpAttribute.GetSupportInfo( theAssembly );

			_details.Text = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "EF_Generic",
				message,
				supportInfo.Company );

			if( supportInfo.IncludeDetails && details != null && details.Length > 0)
				_details.Text += Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "EF_Details", details );

			_supportUrl.Text	= supportInfo.Url;
			_supportPhone.Text	= supportInfo.Phone;
			_supportEmail.Text	= supportInfo.Email;

			if( supportInfo.IncludeAssemblies )
				InitAssemblies();
			else
				_tabs.TabPages.Remove( _assembliesTab );

			if( supportInfo.IncludeSystemInfo )
				InitSystemInfo();
			else
				_tabs.TabPages.Remove( _systemInfoTab );

			_dontShow.Visible = false;
		}

		#region Helper Methods
		private void InitAssemblies()
		{
			try
			{
				foreach( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() )
				{
					try
					{
						ListViewItem item = _assemblyList.Items.Add( assembly.GetName().Name, 0 );
						item.SubItems.Add( assembly.GetName().Version.ToString() );
						try
						{
							FileVersionInfo info = null;

							if( !StaticResourceProvider.IsDynamic(assembly) &&
								String.Compare( "file", assembly.CodeBase.Substring( 0, 4 ), true, System.Globalization.CultureInfo.InvariantCulture ) == 0 )
							{
								info = FileVersionInfo.GetVersionInfo( new Uri( assembly.CodeBase ).LocalPath );
								if( info != null )
									item.SubItems.Add( info.FileVersion );
							}				

							if( info == null )
							{
								AssemblyFileVersionAttribute attr = Attribute.GetCustomAttribute( assembly, typeof( AssemblyFileVersionAttribute ), true ) as AssemblyFileVersionAttribute;
								if( attr != null )
								{
									item.SubItems.Add( attr.Version );
								}
								else
								{
									AssemblyInformationalVersionAttribute attr2 = Attribute.GetCustomAttribute( assembly, typeof( AssemblyInformationalVersionAttribute ), true ) as AssemblyInformationalVersionAttribute;
							
									if( attr2 != null )
										item.SubItems.Add( attr2.InformationalVersion );
									else
										item.SubItems.Add( assembly.GetName().Version.ToString() );
								}
							}
						}
						catch{}
						if( !StaticResourceProvider.IsDynamic(assembly) )
							item.SubItems.Add( assembly.CodeBase );
						else
							item.SubItems.Add( "Dynamic" );
					}
					catch{}
				}

			}
			catch( Exception ex )
			{
				ListViewItem item = _assemblyList.Items.Add( "Error!", 0 );
				item.SubItems.Add( "Error!" );
				item.SubItems.Add( ex.Message );
			}
		}
		private void InitSystemInfo()
		{
			StringBuilder systemInfo = new StringBuilder();

			try
			{
				systemInfo.AppendFormat( System.Globalization.CultureInfo.InvariantCulture,
					"Platform: {0}\r\nOS Version: {1}\r\n", Environment.OSVersion.Platform, Environment.OSVersion.Version );
				systemInfo.AppendFormat( System.Globalization.CultureInfo.InvariantCulture,
					"CLR Version: {0}\r\n", Environment.Version );
				systemInfo.AppendFormat( System.Globalization.CultureInfo.InvariantCulture,
					"Culture: {0}/{1}\r\n", System.Threading.Thread.CurrentThread.CurrentCulture, System.Threading.Thread.CurrentThread.CurrentUICulture );
				systemInfo.AppendFormat( System.Globalization.CultureInfo.InvariantCulture,
					"Encoding: {0}\r\n", System.Text.Encoding.Default.EncodingName );
				systemInfo.AppendFormat( System.Globalization.CultureInfo.InvariantCulture,
					"IIS Available: {0}\r\n", ExtendedLicense.IisIsAvailable );
			}
			catch( Exception ex )
			{
				systemInfo.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "EF_ErrorInfo", ex.Message ) );
			}

			_systemInfo.Text = systemInfo.ToString();
		}
		#endregion

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
			this.panel2 = new System.Windows.Forms.Panel();
			this._copy = new System.Windows.Forms.Button();
			this._tabs = new System.Windows.Forms.TabControl();
			this._detailsTab = new System.Windows.Forms.TabPage();
			this._details = new System.Windows.Forms.TextBox();
			this._assembliesTab = new System.Windows.Forms.TabPage();
			this._assemblyList = new System.Windows.Forms.ListView();
			this._assemblyHeader = new System.Windows.Forms.ColumnHeader();
			this._versionHeader = new System.Windows.Forms.ColumnHeader();
			this._fileVersionHeader = new System.Windows.Forms.ColumnHeader();
			this._locationHeader = new System.Windows.Forms.ColumnHeader();
			this._systemInfoTab = new System.Windows.Forms.TabPage();
			this._systemInfo = new System.Windows.Forms.TextBox();
			this._headerPanel = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this._dontShow = new System.Windows.Forms.CheckBox();
			this._supportEmail = new System.Windows.Forms.LinkLabel();
			this._supportPhone = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._supportUrl = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this._ok = new System.Windows.Forms.Button();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.panel2.SuspendLayout();
			this._tabs.SuspendLayout();
			this._detailsTab.SuspendLayout();
			this._assembliesTab.SuspendLayout();
			this._systemInfoTab.SuspendLayout();
			this._headerPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._copy);
			this.panel2.Controls.Add(this._tabs);
			this.panel2.Controls.Add(this._headerPanel);
			this.panel2.Controls.Add(this._dontShow);
			this.panel2.Controls.Add(this._supportEmail);
			this.panel2.Controls.Add(this._supportPhone);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this._supportUrl);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this._ok);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(520, 367);
			this.panel2.TabIndex = 17;
			// 
			// _copy
			// 
			this._copy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._copy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._copy.Location = new System.Drawing.Point(432, 69);
			this._copy.Name = "_copy";
			this._copy.Size = new System.Drawing.Size(74, 23);
			this._copy.TabIndex = 3;
			this._copy.Text = "#UIF_Copy";
			this._copy.Click += new System.EventHandler(this._copy_Click);
			// 
			// _tabs
			// 
			this._tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._tabs.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this._tabs.Controls.Add(this._detailsTab);
			this._tabs.Controls.Add(this._assembliesTab);
			this._tabs.Controls.Add(this._systemInfoTab);
			this._tabs.Location = new System.Drawing.Point(8, 72);
			this._tabs.Name = "_tabs";
			this._tabs.SelectedIndex = 0;
			this._tabs.Size = new System.Drawing.Size(502, 200);
			this._tabs.TabIndex = 0;
			// 
			// _detailsTab
			// 
			this._detailsTab.Controls.Add(this._details);
			this._detailsTab.Location = new System.Drawing.Point(4, 25);
			this._detailsTab.Name = "_detailsTab";
			this._detailsTab.Size = new System.Drawing.Size(494, 171);
			this._detailsTab.TabIndex = 0;
			this._detailsTab.Text = "#UIF_Details";
			// 
			// _details
			// 
			this._details.BackColor = System.Drawing.Color.White;
			this._details.Dock = System.Windows.Forms.DockStyle.Fill;
			this._details.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._details.Location = new System.Drawing.Point(0, 0);
			this._details.Multiline = true;
			this._details.Name = "_details";
			this._details.ReadOnly = true;
			this._details.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._details.Size = new System.Drawing.Size(494, 171);
			this._details.TabIndex = 0;
			this._details.Text = "";
			// 
			// _assembliesTab
			// 
			this._assembliesTab.Controls.Add(this._assemblyList);
			this._assembliesTab.Location = new System.Drawing.Point(4, 25);
			this._assembliesTab.Name = "_assembliesTab";
			this._assembliesTab.Size = new System.Drawing.Size(494, 171);
			this._assembliesTab.TabIndex = 1;
			this._assembliesTab.Text = "#UIF_Assemblies";
			// 
			// _assemblyList
			// 
			this._assemblyList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this._assemblyHeader,
																							this._versionHeader,
																							this._fileVersionHeader,
																							this._locationHeader});
			this._assemblyList.Dock = System.Windows.Forms.DockStyle.Fill;
			this._assemblyList.FullRowSelect = true;
			this._assemblyList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._assemblyList.Location = new System.Drawing.Point(0, 0);
			this._assemblyList.Name = "_assemblyList";
			this._assemblyList.Size = new System.Drawing.Size(494, 171);
			this._assemblyList.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this._assemblyList.TabIndex = 16;
			this._assemblyList.View = System.Windows.Forms.View.Details;
			// 
			// _assemblyHeader
			// 
			this._assemblyHeader.Text = "#UIF_Assembly";
			this._assemblyHeader.Width = 200;
			// 
			// _versionHeader
			// 
			this._versionHeader.Text = "#UIF_Version";
			this._versionHeader.Width = 90;
			// 
			// _fileVersionHeader
			// 
			this._fileVersionHeader.Text = "#UIF_FileVersion";
			this._fileVersionHeader.Width = 90;
			// 
			// _locationHeader
			// 
			this._locationHeader.Text = "#UIF_Location";
			this._locationHeader.Width = 800;
			// 
			// _systemInfoTab
			// 
			this._systemInfoTab.Controls.Add(this._systemInfo);
			this._systemInfoTab.Location = new System.Drawing.Point(4, 25);
			this._systemInfoTab.Name = "_systemInfoTab";
			this._systemInfoTab.Size = new System.Drawing.Size(494, 171);
			this._systemInfoTab.TabIndex = 2;
			this._systemInfoTab.Text = "#UIF_SysInfo";
			// 
			// _systemInfo
			// 
			this._systemInfo.BackColor = System.Drawing.Color.White;
			this._systemInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._systemInfo.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._systemInfo.Location = new System.Drawing.Point(0, 0);
			this._systemInfo.Multiline = true;
			this._systemInfo.Name = "_systemInfo";
			this._systemInfo.ReadOnly = true;
			this._systemInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._systemInfo.Size = new System.Drawing.Size(494, 171);
			this._systemInfo.TabIndex = 0;
			this._systemInfo.Text = "";
			// 
			// _headerPanel
			// 
			this._headerPanel.BackColor = System.Drawing.Color.Red;
			this._headerPanel.Controls.Add(this.label4);
			this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._headerPanel.Location = new System.Drawing.Point(0, 0);
			this._headerPanel.Name = "_headerPanel";
			this._headerPanel.Size = new System.Drawing.Size(520, 56);
			this._headerPanel.TabIndex = 27;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Font = new System.Drawing.Font("Arial Black", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(8, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(352, 48);
			this.label4.TabIndex = 0;
			this.label4.Text = "#UIF_Title";
			// 
			// _dontShow
			// 
			this._dontShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._dontShow.Checked = true;
			this._dontShow.CheckState = System.Windows.Forms.CheckState.Checked;
			this._dontShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._dontShow.Location = new System.Drawing.Point(280, 336);
			this._dontShow.Name = "_dontShow";
			this._dontShow.Size = new System.Drawing.Size(144, 24);
			this._dontShow.TabIndex = 25;
			this._dontShow.Text = "#UIF_DontShow";
			// 
			// _supportEmail
			// 
			this._supportEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._supportEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportEmail.Location = new System.Drawing.Point(112, 296);
			this._supportEmail.Name = "_supportEmail";
			this._supportEmail.Size = new System.Drawing.Size(392, 16);
			this._supportEmail.TabIndex = 2;
			this._supportEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._supportEmail_LinkClicked);
			// 
			// _supportPhone
			// 
			this._supportPhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._supportPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportPhone.Location = new System.Drawing.Point(112, 312);
			this._supportPhone.Name = "_supportPhone";
			this._supportPhone.Size = new System.Drawing.Size(238, 16);
			this._supportPhone.TabIndex = 20;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(8, 312);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(130, 23);
			this.label3.TabIndex = 23;
			this.label3.Text = "#UI_SP";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(8, 296);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 23);
			this.label2.TabIndex = 22;
			this.label2.Text = "#UI_SE";
			// 
			// _supportUrl
			// 
			this._supportUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._supportUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._supportUrl.Location = new System.Drawing.Point(112, 280);
			this._supportUrl.Name = "_supportUrl";
			this._supportUrl.Size = new System.Drawing.Size(392, 16);
			this._supportUrl.TabIndex = 1;
			this._supportUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._supportUrl_LinkClicked);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 280);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 23);
			this.label1.TabIndex = 21;
			this.label1.Text = "#UI_SU";
			// 
			// _ok
			// 
			this._ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._ok.Location = new System.Drawing.Point(430, 336);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(80, 23);
			this._ok.TabIndex = 4;
			this._ok.Text = "#UI_OK";
			this._ok.Click += new System.EventHandler(this._ok_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 367);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(520, 22);
			this.statusBar1.TabIndex = 18;
			// 
			// FailureReportForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 389);
			this.ControlBox = false;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.statusBar1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(472, 320);
			this.Name = "FailureReportForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "#UIF_Title";
			this.TopMost = true;
			this.panel2.ResumeLayout(false);
			this._tabs.ResumeLayout(false);
			this._detailsTab.ResumeLayout(false);
			this._assembliesTab.ResumeLayout(false);
			this._systemInfoTab.ResumeLayout(false);
			this._headerPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion



		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gers or sets a value that indicates if the form should be shown again.
		/// </summary>
		public static bool DontShowAgain
		{
			get
			{
				return _dontShowAgain;
			}
			set
			{
				_dontShowAgain = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		///<summary>
		///Summary of OnLoad.
		///</summary>
		///<param name="e"></param>
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			SetStyle( ControlStyles.AllPaintingInWmPaint, true );
		}

		///<summary>
		///Summary of _ok_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _ok_Click( object sender, System.EventArgs e )
		{
			_dontShowAgain = _dontShow.Checked;
		}

		///<summary>
		///Summary of _supportUrl_LinkClicked.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _supportUrl_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			if( _supportUrl.Text != null && _supportUrl.Text.Length > 0 && _supportUrl.Text != "N/A" )
				System.Diagnostics.Process.Start( _supportUrl.Text );
		}

		///<summary>
		///Summary of _supportEmail_LinkClicked.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _supportEmail_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			if( _supportEmail.Text != null && _supportEmail.Text.Length > 0 && _supportEmail.Text != "N/A" )
				System.Diagnostics.Process.Start( "mailto:" + _supportEmail.Text + "?subject=License%20Failure" );
		}

		///<summary>
		///Summary of _copy_Click.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _copy_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject( MakeInfo(), true );
		}
		#region Helper Methods
		private string MakeInfo()
		{
			StringBuilder info = new StringBuilder();

			info.Append( "License failure details\r\n-------------------------------\r\n\r\n" );
			info.Append( _details.Text );
			info.Append( "\r\n\r\nSystem Info\r\n-------------------------------\r\n\r\n" );
			info.Append( _systemInfo.Text );
			info.Append( "\r\n\r\nAssemblies\r\n-------------------------------\r\n\r\n" );

			foreach( ListViewItem item in _assemblyList.Items )
			{
				info.AppendFormat( System.Globalization.CultureInfo.InvariantCulture,
					"{0}\r\n{1}\r\n{2}\r\n\r\n",
					item.Text,
					item.SubItems[ 1 ].Text,
					item.SubItems[ 2 ].Text );
			}

			return info.ToString();
		}
		#endregion


		/// <summary>
		/// Displays a failure form as a modal dialog.
		/// </summary>
		/// <param name="context">
		///		The LicenseContext that failed.
		/// </param>
		/// <param name="type">
		///		The Type of the object being licensed.
		/// </param>
		/// <param name="instance">
		///		The instance of the object being licensed.
		/// </param>
		/// <param name="reason">
		///		The reason for the failure.
		/// </param>
		public static void Show( LicenseContext context, Type type, object instance, string reason )
		{
			try
			{
				using( FailureReportForm form = new FailureReportForm( context, type, instance, reason ) )
				{
					form.ShowDialog();
				}
			}
			catch{}
		}

		/// <summary>
		/// Displays a failure form as a modal dialog.
		/// </summary>
		/// <param name="details">
		///		Detailed information about the failure.
		/// </param>
		/// <param name="message">
		///		The message to display to the user.
		/// </param>
		/// <param name="theAssembly">
		///		The assembly the error should be reported for. Used to gather support
		///		information such as email, phone and url.
		/// </param>
		public static void Show( string message, string details, Assembly theAssembly )
		{
			try
			{
				using( FailureReportForm form = new FailureReportForm( message, details, theAssembly ) )
				{
					form.ShowDialog( Form.ActiveForm );
				}
			}
			catch{}
		}

		/// <summary>
		/// Displays a failure form as a modal dialog.
		/// </summary>
		/// <param name="ex">
		///		The Exception that caused the problem.
		/// </param>
		/// <param name="message">
		///		The message to display to the user.
		/// </param>
		/// <param name="theAssembly">
		///		The assembly the error should be reported for. Used to gather support
		///		information such as email, phone and url.
		/// </param>
		public static void Show( string message, Exception ex, Assembly theAssembly )
		{
			try
			{
				using( FailureReportForm form = new FailureReportForm( message, PreProcessException( ex ), theAssembly ) )
				{
					form.ShowDialog( Form.ActiveForm );
				}
			}
			catch{}
		}

		/// <summary>
		/// Processes an exception and builds a presentable error message by cleaning 
		/// out any stack trace and Type information.
		/// </summary>
		/// <param name="ex">
		///		The exception to process.
		/// </param>
		/// <returns>
		///		Returns a processed message.
		/// </returns>
		public static string PreProcessException( Exception ex )
		{
//#if DEBUG
//			return ex == null ? "" : ex.ToString();
//#else
			try
			{
				if( ex is SoapException )
				{
					string message = ex.Message;
					if( message.IndexOf( "Exception: " ) > -1 )
					{
						message = message.Substring( message.LastIndexOf( "Exception: " ) + 11 );
						message = message.Substring( 0, message.IndexOf( '\n' ) );					
					}
					if( message.IndexOf( "-->" ) > -1 )
						message = message.Substring( message.LastIndexOf( "-->" ) + 3 );
					return message.Trim();
				}
				else
				{
					return ex.Message;
				}
			}
			catch
			{
				return ex == null ? "" : ex.ToString();
			}
//#endif
		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class FailureReportForm
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////