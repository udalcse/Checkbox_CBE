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
// Class:		InfoForm
// Author:		Paul Alexander
// Created:		Monday, January 06, 2003 7:54:25 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace Xheo.Licensing
{
	/// <summary>
	/// Presents a simple user interface for giving the user additional information
	/// such as trial terms and a link to additional information available online.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class InfoForm : System.Windows.Forms.Form
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		
		private System.Windows.Forms.TextBox _text;
		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.LinkLabel _link;

		private string _infoUrl = null;

		private System.Windows.Forms.Timer timer1;
		private Internal.SmallIe _browser;
		private System.ComponentModel.IContainer components;
		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Included for COM compliance, do not use.
		/// </summary>
		public InfoForm()
		{
			//throw new NotSupportedException( "Use InfoForm( string, string ) instead." );
		}
		
		///	<summary>
		/// Initializes a new instance of the InfoForm class.
		/// </summary>
		///	<param name="text">
		///		Text to display in the main window. If it contains a URL it will be displayed
		///		in the window instead.
		///	</param>
		///	<param name="infoUrl">
		///		Location where additional information is located.
		///	</param>
		public InfoForm( string text, string infoUrl )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );

			_text.Text	= text;
			_text.SelectionLength = 0;
			_text.ReadOnly = true;
			_text.BackColor = Color.FromKnownColor( KnownColor.Window );
			if( infoUrl != null && infoUrl.Length > 0 )
				_infoUrl	= infoUrl;
			else
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
			this._text = new System.Windows.Forms.TextBox();
			this._ok = new System.Windows.Forms.Button();
			this._link = new System.Windows.Forms.LinkLabel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this._browser = new Internal.SmallIe();
			((System.ComponentModel.ISupportInitialize)(this._browser)).BeginInit();
			this.SuspendLayout();
			// 
			// _text
			// 
			this._text.Location = new System.Drawing.Point(8, 8);
			this._text.Multiline = true;
			this._text.Name = "_text";
			this._text.Size = new System.Drawing.Size(440, 240);
			this._text.TabIndex = 0;
			this._text.Text = "";
			// 
			// _ok
			// 
			this._ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._ok.Location = new System.Drawing.Point(344, 256);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(104, 23);
			this._ok.TabIndex = 1;
			this._ok.Text = "#UI_OK";
			// 
			// _link
			// 
			this._link.Location = new System.Drawing.Point(8, 256);
			this._link.Name = "_link";
			this._link.Size = new System.Drawing.Size(312, 32);
			this._link.TabIndex = 2;
			this._link.TabStop = true;
			this._link.Text = "#UIAI_Online";
			this._link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._link_LinkClicked);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// _browser
			// 
			this._browser.Enabled = true;
			this._browser.Location = new System.Drawing.Point(8, 8);
			this._browser.Name = "_browser";
			this._browser.Size = new System.Drawing.Size(440, 240);
			this._browser.TabIndex = 3;
			this._browser.Visible = false;
			// 
			// InfoForm
			// 
			this.AcceptButton = this._ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(456, 296);
			this.Controls.Add(this._browser);
			this.Controls.Add(this._link);
			this.Controls.Add(this._ok);
			this.Controls.Add(this._text);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InfoForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "#UIAI_Title";
			((System.ComponentModel.ISupportInitialize)(this._browser)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		///<summary>
		///Summary of _link_LinkClicked.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void _link_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start( new Uri( _infoUrl ).ToString() );
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
		/// Ensures the form is visible.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			Show();
			BringToFront();
			Activate();
		}


		///<summary>
		///Summary of timer1_Tick.
		///</summary>
		///<param name="sender"></param>
		///<param name="e"></param>
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			timer1.Enabled = false;
			string vl = _text.Text.ToLower( System.Globalization.CultureInfo.InvariantCulture );
			if( vl != null && ( vl.StartsWith( "http://" ) || vl.StartsWith( "https://" ) || vl.StartsWith( "file://" ) ) )
			{
				_browser.Visible = true;
				_browser.Navigate ( _text.Text );
			}
		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class InfoForm
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////