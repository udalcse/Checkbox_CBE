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
// Class:		ApplicationLimit
// Author:		Paul Alexander
// Created:		Friday, March 12, 2004
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;

namespace Xheo.Licensing
{
	/// <summary>
	/// Summary description for ProxyForm.
	/// </summary>
	class ProxyForm : System.Windows.Forms.Form
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private IWebProxy _proxy		= null;

		private System.Windows.Forms.Label _prompt;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button _cancel;
		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.TextBox _address;
		private System.Windows.Forms.TextBox _username;
		private System.Windows.Forms.TextBox _password;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ProxyForm class.
		/// </summary>
		public ProxyForm( Uri url )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			IWebProxy proxy = ExtendedLicense.Proxy;

			if( proxy != null )
			{
				_proxy = proxy;
				Uri address = proxy.GetProxy( url );
				if( address != null && address != url )
					_address.Text = address.ToString();
				if( proxy.Credentials != null )
				{
					NetworkCredential credentials = proxy.Credentials.GetCredential( url, null );
					if( credentials.Domain == null || credentials.Domain.Length == 0 )
						_username.Text = credentials.UserName;
					else
						_username.Text = String.Format( System.Globalization.CultureInfo.InvariantCulture, "{0}\\{1}", credentials.Domain, credentials.UserName );
					_password.Text = credentials.Password;
				}					
			}

			Internal.StaticResourceProvider.CurrentProvider.LocalizeControl( this );
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
			this._prompt = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._address = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this._username = new System.Windows.Forms.TextBox();
			this._password = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._cancel = new System.Windows.Forms.Button();
			this._ok = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _prompt
			// 
			this._prompt.Location = new System.Drawing.Point(8, 8);
			this._prompt.Name = "_prompt";
			this._prompt.Size = new System.Drawing.Size(352, 40);
			this._prompt.TabIndex = 0;
			this._prompt.Text = "#UIP_Message";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 56);
			this.label1.Name = "label1";
			this.label1.TabIndex = 1;
			this.label1.Text = "#UIP_Address";
			// 
			// _address
			// 
			this._address.Location = new System.Drawing.Point(8, 72);
			this._address.Name = "_address";
			this._address.Size = new System.Drawing.Size(352, 20);
			this._address.TabIndex = 2;
			this._address.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 104);
			this.label2.Name = "label2";
			this.label2.TabIndex = 3;
			this.label2.Text = "#UIP_Username";
			// 
			// _username
			// 
			this._username.Location = new System.Drawing.Point(112, 104);
			this._username.Name = "_username";
			this._username.TabIndex = 4;
			this._username.Text = "";
			// 
			// _password
			// 
			this._password.Location = new System.Drawing.Point(112, 128);
			this._password.Name = "_password";
			this._password.PasswordChar = '*';
			this._password.TabIndex = 4;
			this._password.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 128);
			this.label3.Name = "label3";
			this.label3.TabIndex = 3;
			this.label3.Text = "#UIP_Password";
			// 
			// _cancel
			// 
			this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cancel.Location = new System.Drawing.Point(280, 160);
			this._cancel.Name = "_cancel";
			this._cancel.TabIndex = 5;
			this._cancel.Text = "#UI_Cancel";
			// 
			// _ok
			// 
			this._ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._ok.Location = new System.Drawing.Point(200, 160);
			this._ok.Name = "_ok";
			this._ok.TabIndex = 5;
			this._ok.Text = "#UI_OK";
			this._ok.Click += new System.EventHandler(this._ok_Click);
			// 
			// ProxyForm
			// 
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.AcceptButton = this._ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this._cancel;
			this.ClientSize = new System.Drawing.Size(368, 198);
			this.ControlBox = false;
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._username);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._address);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._prompt);
			this.Controls.Add(this._password);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._ok);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProxyForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "#UIP_Title";
			this.ResumeLayout(false);

		}
		#endregion

		
		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the proxy information provided.
		/// </summary>
		public IWebProxy Proxy
		{
			get
			{
				return _proxy;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		private void _ok_Click(object sender, System.EventArgs e)
		{
			try
			{
				if( _address.Text.Length == 0 )
				{
					if(  _username.Text.Length > 0 )
					{
						MessageBox.Show(
							this,
							Internal.StaticResourceProvider.CurrentProvider.GetString( "UIPE_Address" ),
							Internal.StaticResourceProvider.CurrentProvider.GetString( "UI_Error" ),
							MessageBoxButtons.OK,
							MessageBoxIcon.Error );
						_address.Select();
						return;
					}
					else
					{
						_proxy = GlobalProxySelection.GetEmptyWebProxy();
						ExtendedLicense.SetProxy( _proxy );
						DialogResult = DialogResult.OK;
						return;
					}
				}

				if( _username.Text.Length > 0 )
				{
					string username = null;
					string password = null;
					string domain = null;

					int slash = _username.Text.IndexOf( '\\' );
					if( slash == -1 )
						slash = _username.Text.IndexOf( '/' );
					if( slash != -1 )
					{
						domain = _username.Text.Substring( 0, slash );
						username = _username.Text.Substring( slash + 1 );
					}
					else
					{
						username = _username.Text;
					}						

					password = _password.Text;

					_proxy = new WebProxy( _address.Text, false, new string[ 0 ], new NetworkCredential( username, password, domain ) );
				}
				else
					_proxy = new WebProxy( _address.Text, false );

				ExtendedLicense.SetProxy( _proxy );
				DialogResult = DialogResult.OK;
			}
			catch( Exception ex )
			{
				MessageBox.Show(
					this,
					ex.Message,
					Internal.StaticResourceProvider.CurrentProvider.GetString( "UI_Error" ),
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				return;
			}
		}

		public static bool RetryException( IWin32Window owner, Exception ex, string url )
		{
			if( ex != null && ex.Message != null && ! ExtendedLicense.IsWebRequest )
			{
				string message = ex.Message.ToLower( System.Globalization.CultureInfo.InvariantCulture );
				if( message.IndexOf( "proxy" ) > -1 )
					using( ProxyForm form = new ProxyForm( new Uri( url ) ) )
						return form.ShowDialog( owner ) == DialogResult.OK;
			}

			return false;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////		

	} // End class ProxyForm
} // End namepsace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////