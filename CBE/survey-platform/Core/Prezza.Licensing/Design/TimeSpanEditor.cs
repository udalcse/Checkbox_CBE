////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2004 XHEO, INC. All Rights Reserved.
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
// Class:		TimeSpanEditor
// Author:		Paul Alexander
// Created:		Sunday, September 12, 2004
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Xheo.Licensing.Design
{
	/// <summary>
	/// Embedded control used to edit a TimeSpan.
	/// </summary>
	class TimeSpanEditControl : System.Windows.Forms.UserControl
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private IWindowsFormsEditorService	_editService = null;

		private System.Windows.Forms.TextBox _days;
		private System.Windows.Forms.TextBox _hours;
		private System.Windows.Forms.TextBox _minutes;
		private System.Windows.Forms.TextBox _seconds;
		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors


		/// <summary>
		/// Initializes a new instance of the TimeSpanEditControl class.
		/// </summary>
		public TimeSpanEditControl( IWindowsFormsEditorService editService )
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			_editService = editService;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._days = new System.Windows.Forms.TextBox();
			this._hours = new System.Windows.Forms.TextBox();
			this._minutes = new System.Windows.Forms.TextBox();
			this._seconds = new System.Windows.Forms.TextBox();
			this._ok = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _days
			// 
			this._days.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._days.AutoSize = false;
			this._days.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._days.Location = new System.Drawing.Point(0, 0);
			this._days.Name = "_days";
			this._days.Size = new System.Drawing.Size(40, 16);
			this._days.TabIndex = 0;
			this._days.Text = "000";
			this._days.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _hours
			// 
			this._hours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._hours.AutoSize = false;
			this._hours.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._hours.Location = new System.Drawing.Point(48, 0);
			this._hours.Name = "_hours";
			this._hours.Size = new System.Drawing.Size(16, 16);
			this._hours.TabIndex = 1;
			this._hours.Text = "00";
			this._hours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _minutes
			// 
			this._minutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._minutes.AutoSize = false;
			this._minutes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._minutes.Location = new System.Drawing.Point(72, 0);
			this._minutes.Name = "_minutes";
			this._minutes.Size = new System.Drawing.Size(16, 16);
			this._minutes.TabIndex = 1;
			this._minutes.Text = "00";
			this._minutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _seconds
			// 
			this._seconds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._seconds.AutoSize = false;
			this._seconds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._seconds.Location = new System.Drawing.Point(96, 0);
			this._seconds.Name = "_seconds";
			this._seconds.Size = new System.Drawing.Size(16, 16);
			this._seconds.TabIndex = 1;
			this._seconds.Text = "00";
			this._seconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _ok
			// 
			this._ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._ok.Location = new System.Drawing.Point(56, 24);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(56, 24);
			this._ok.TabIndex = 2;
			this._ok.Text = "OK";
			this._ok.Click += new System.EventHandler(this._ok_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(64, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(8, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = ":";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(88, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(8, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = ":";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TimeSpanEditControl
			// 
			this.Controls.Add(this.label1);
			this.Controls.Add(this._ok);
			this.Controls.Add(this._hours);
			this.Controls.Add(this._days);
			this.Controls.Add(this._minutes);
			this.Controls.Add(this._seconds);
			this.Controls.Add(this.label2);
			this.Name = "TimeSpanEditControl";
			this.Size = new System.Drawing.Size(112, 48);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the actual time span edited.
		/// </summary>
		public TimeSpan TimeSpan
		{
			get
			{
				try
				{
					return new TimeSpan(
						int.Parse( _days.Text, System.Globalization.CultureInfo.CurrentCulture ),
						int.Parse( _hours.Text, System.Globalization.CultureInfo.CurrentCulture ),
						int.Parse( _minutes.Text, System.Globalization.CultureInfo.CurrentCulture ),
						int.Parse( _seconds.Text, System.Globalization.CultureInfo.CurrentCulture ) );
				}
				catch
				{
					return TimeSpan.Zero;
				}
			}
			set
			{
				_days.Text		= value.Days.ToString( System.Globalization.CultureInfo.CurrentCulture );
				_hours.Text		= value.Hours.ToString( System.Globalization.CultureInfo.CurrentCulture );
				_minutes.Text	= value.Minutes.ToString( System.Globalization.CultureInfo.CurrentCulture );
				_seconds.Text	= value.Seconds.ToString( System.Globalization.CultureInfo.CurrentCulture );
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		private void _ok_Click(object sender, System.EventArgs e)
		{
			_editService.CloseDropDown();
		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class TimeSpanEditControl

	#region TimeSpanEditor
	/// <summary>
	/// Implements a UI Type Editor for a time span value.
	/// </summary>
	public class TimeSpanEditor : UITypeEditor
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private IWindowsFormsEditorService _editService = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Edits the value.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
		{
			if( context != null && context.Instance != null && provider != null )
			{
				_editService = provider.GetService( typeof( IWindowsFormsEditorService ) ) as IWindowsFormsEditorService;
				if( _editService == null )
					return base.EditValue( context, provider, value );

				TimeSpanEditControl ctrl = new TimeSpanEditControl( _editService );
				ctrl.TimeSpan = (TimeSpan)value;
				_editService.DropDownControl( ctrl );
				return ctrl.TimeSpan;
			}

			return base.EditValue (context, provider, value);
		}

		/// <summary>
		/// Gets the edit style.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
		{
			if( context != null && context.Instance != null )
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle (context);
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class TimeSpanEditor
	#endregion

} // End namespace Xheo.Licensing


////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2004 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
