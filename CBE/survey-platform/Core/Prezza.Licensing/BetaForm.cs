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
// Class:		BetaForm
// Author:		Paul Alexander
// Created:		[!output CURRENT_DATE]
//
///////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Xheo.Licensing
{
	/// <summary>
	/// Summary description for BetaForm.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class BetaForm : Xheo.Licensing.InfoForm
	{

		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label _notice;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the BetaForm class.
		/// </summary>
		public BetaForm()
		{
			InitializeComponent();
		}

		/// <summary>
		///	Initializes a new instance of the BetaForm class.
		/// </summary>
		public BetaForm( BetaLimit limit ) : base( limit.UpdateUrl, "" )
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			_notice.Text	= Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_BetaExpired", limit.UpdateUrl );
			Text			= Internal.StaticResourceProvider.CurrentProvider.GetString( "MBL_BetaExpired" );
			Height			+= 20;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._notice = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _notice
			// 
			this._notice.Location = new System.Drawing.Point(8, 256);
			this._notice.Name = "_notice";
			this._notice.Size = new System.Drawing.Size(312, 48);
			this._notice.TabIndex = 3;

			this.Controls.Add(this._notice);
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


		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
	} // End class BetaForm
} // End namespace Xheo.Licensing

