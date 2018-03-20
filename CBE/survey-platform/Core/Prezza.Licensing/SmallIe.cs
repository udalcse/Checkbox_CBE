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
// Class:		SmallIe
// Author:		Paul Alexander
// Created:		Friday, May 06, 2005
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Internal
{
	/// <summary>
	/// Implements a small control usefull for hosting the IE web control in a form
	/// without requiring Interop.SHDocVw.dll. Does not support all the functionality
	/// of the full control but enough to offer basic browsing, printing and navigation.
	/// </summary>
	[System.Windows.Forms.AxHost.ClsidAttribute( "{8856f961-340a-11d0-a96b-00c04fd705a2}" )]
	internal
	class SmallIe : AxHost, DWebBrowserEvents2
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private object	_ocx		= null;
		private Type	_ocxType	= null;
		private string	_firstUrl	= null;

		private AxHost.ConnectionPointCookie _cookie = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Events

		/// <summary>
		/// Fired when the status text changes.
		/// </summary>
		public event TextChangedHandler StatusTextChanged;

		/// <summary>
		/// Fired when the progress changes.
		/// </summary>
		public event ProgressChangedHandler ProgressChanged;

		/// <summary>
		/// Fired when one of the toolbar commands' state has changed.
		/// </summary>
		public event CommandStateChangedHandler CommandStateChanged;

		/// <summary>
		/// Fired when the download completes.
		/// </summary>
		public event EventHandler DownlodComplete;

		/// <summary>
		/// Fired when the download begins.
		/// </summary>
		public event EventHandler DownloadBegin;

		/// <summary>
		/// Fired when the title changes.
		/// </summary>
		public event TextChangedHandler TitleChanged;

		/// <summary>
		/// Fired when one of the properties changes.
		/// </summary>
		public event TextChangedHandler PropertyChanged;

		/// <summary>
		/// Fired when the state of the secure icon has changed.
		/// </summary>
		public event SeucureIconChangedHandler SecureIconChanged;

		/// <summary>
		/// Fired before the window navigates. Return true to cancel navigation.
		/// </summary>
		public event BeforeNavigateHandler BeforeNavigate;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SallIe class.
		/// </summary>
		public SmallIe() : base( "8856f961-340a-11d0-a96b-00c04fd705a2" )
		{
			
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the current location URL.
		/// </summary>
		#region LocationUrl
		public Uri LocationUrl
		{
			get {
				if( _ocxType == null )
					return null;
				string url = _ocxType.InvokeMember( "LocationURL", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, _ocx, new object[ 0 ] ) as string;
				return new Uri( url ); 
			}
			set {
				if( _ocxType == null )
				{
					if( value != null )
						_firstUrl = value.ToString();
					else
						_firstUrl = "about:blank";
					return;
				}
				string url = value == null ? "" : value.ToString();
				_ocxType.InvokeMember( "LocationURL", BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty, null, _ocx, new object[] { url } );
			}
		}		
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		#region ActiveX Support
		/// <summary>
		/// </summary>
		#region AttachInterfaces...
		protected override void AttachInterfaces()
		{
			_ocx = GetOcx();
			if( _ocx == null )
				throw new NotSupportedException();
			_ocxType = _ocx.GetType();
		}
		#endregion

		/// <summary>
		/// </summary>
		protected override void CreateSink()
		{
			_cookie = new ConnectionPointCookie( _ocx, this, typeof( DWebBrowserEvents2 ) ); 
			if( _firstUrl != null )
				Navigate( _firstUrl );
		}

		protected override void DetachSink()
		{
			_cookie.Disconnect();
		}

		#endregion

		/// <summary>
		/// Navigates to a specific URL.
		/// </summary>
		#region Navigate...
		public void Navigate( string url )
		{
			if( _ocxType == null )
			{
				_firstUrl = url;
				return;
			}

			object[] parameters = new object[] {
				url,
				System.Reflection.Missing.Value,
				System.Reflection.Missing.Value,
				System.Reflection.Missing.Value,
				System.Reflection.Missing.Value
			};

			_ocxType.InvokeMember( "Navigate", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, _ocx, parameters );
		}
		#endregion

		/// <summary>
		/// Refreshes the page.
		/// </summary>
		#region Refresh...
		public void RefreshBrowser(  )
		{
			_ocxType.InvokeMember( "Refresh", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, _ocx, new object[ 0 ] );
		}
		#endregion

		/// <summary>
		/// Navigates the browser back.
		/// </summary>
		#region GoBack...
		public void GoBack(  )
		{
			try
			{
				_ocxType.InvokeMember( "GoBack", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, _ocx, new object[ 0 ] );
			}
			catch { }
		}
		#endregion

		/// <summary>
		/// Navigates the browser forward.
		/// </summary>
		#region GoForward...
		public void GoForward()
		{
			try
			{
				_ocxType.InvokeMember( "GoForward", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, _ocx, new object[ 0 ] );
			}
			catch { }
		}
		#endregion

		/// <summary>
		/// Stops the current progress of the browser.
		/// </summary>
		#region Stop...
        public void Stop(  )
        {
			try
			{
				_ocxType.InvokeMember( "Stop", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, _ocx, new object[ 0 ] );
			}
			catch{}
        }
        #endregion

		/// <summary>
		/// Prints the current page.
		/// </summary>
		/// <param name="dontPrompt">
		///		Indicates if the user should be prompted.
		/// </param>
		#region Print...
		public void Print( bool dontPrompt )
		{
			ExecWB( OleCmdId.Print, dontPrompt ? OleCmdExecOption.DontPromptUser : OleCmdExecOption.PromptUser );
		}
		#region Overloads
		
		/// <summary>
		/// Prints the current page.
		/// </summary>
		public void Print()
		{
			Print( false );
		}
		#endregion
		#endregion

		/// <summary>
		/// Executes a command on the window.
		/// </summary>
		#region ExecWB...
		public void ExecWB( OleCmdId commandId, OleCmdExecOption option )
		{
			object[] parameters = new object[] {
				commandId, 
				option,
				Missing.Value,
				Missing.Value
			};

			_ocxType.InvokeMember( "ExecWB", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, _ocx, parameters );
		}
		#endregion

		#region DWebBrowserEvents2 Members

		void DWebBrowserEvents2.StatusTextChange( string Text )
		{
			if( StatusTextChanged != null )
				StatusTextChanged( this, Text );
		}

		void DWebBrowserEvents2.ProgressChange( int Progress, int ProgressMax )
		{
			if( ProgressChanged != null )
				ProgressChanged( this, Progress, ProgressMax );
		}

		void DWebBrowserEvents2.CommandStateChange( int Command, bool Enable )
		{
			if( CommandStateChanged != null )
				CommandStateChanged( this, (IeToolbarCommand)Command, Enable );
		}

		void DWebBrowserEvents2.DownloadBegin()
		{
			if( DownloadBegin != null )
				DownloadBegin( this, EventArgs.Empty );
		}

		void DWebBrowserEvents2.DownloadComplete()
		{
			if( DownlodComplete != null )
				DownlodComplete( this, EventArgs.Empty );
		}

		void DWebBrowserEvents2.TitleChange( string Text )
		{
			if( TitleChanged != null )
				TitleChanged( this, Text );
		}

		void DWebBrowserEvents2.PropertyChange( string szProperty )
		{
			if( PropertyChanged != null )
				PropertyChanged( this, szProperty );
		}

		void DWebBrowserEvents2.BeforeNavigate2( object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel )
		{
			if( BeforeNavigate != null )
			{
				if( BeforeNavigate( this, URL as string, TargetFrameName as string, PostData as byte[], Headers as string ) )
				{
					Stop();
					Cancel = true;
				}
			}
		}

		void DWebBrowserEvents2.SetSecureLockIcon( int secureLockIcon )
		{
			if( SecureIconChanged != null )
				SecureIconChanged( this, (SecureLockIcon)secureLockIcon );
		}

		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		#region Delegates
		public delegate void TextChangedHandler( object sender, string text );
		public delegate void ProgressChangedHandler( object sender, int progress, int max );
		public delegate void CommandStateChangedHandler( object sender, IeToolbarCommand command, bool enabled );
		public delegate void SeucureIconChangedHandler( object sender, SecureLockIcon newState );
		public delegate bool BeforeNavigateHandler( object sender, string url, string targetFrame, byte[] postData, string headers );

		#endregion


		#region IeToolbarCommand
		/// <summary>
		/// Toolbar commands that can change state.
		/// </summary>
		[ Flags ]
		public enum IeToolbarCommand
		{
			/// <summary>
			/// Navigate forward command.
			/// </summary>
			NavigateForward		= 0x01,
			/// <summary>
			/// Navigate backward command.
			/// </summary>
			NavigateBackward	= 0x0,

			/// <summary>
			/// Indicates the state may have changed.
			/// </summary>
			UpdateCommands		= -1

		} // End enum IeToolbarCommand
		#endregion

		#region SecureLockIcon
		/// <summary>
		/// State of the secure lock icons.
		/// </summary>
		public enum SecureLockIcon
		{
			Unsecure = 0,
			Mixed = 0x1,
			UnknownBits = 0x2,
			Bits40 = 0x3,
			Bits56 = 0x4,
			Fortezza = 0x5,
			Bits128 = 0x6
		} // End enum SecureLockIcon
		#endregion

		#region OleCmdExecOption
		/// <summary>
		/// 
		/// </summary>
		public enum OleCmdExecOption
		{
		    DoDefault = 0,
			DontPromptUser = 2,
			PromptUser = 1,
			ShowHelp = 3
		} // End enum OleCmdExecOption
		#endregion

		#region OleCmdId
		/// <summary>
		/// 
		/// </summary>
		public enum OleCmdId
		{
			AllowUilessSaveAs=0x2e,
			ClearSelection=0x12,
			Close=0x2d,
			Copy=12,
			Cut=11,
			Delete=0x21,
			DontDownloadCss=0x2f,
			EnableInteraction=0x24,
			Find=0x20,
			FocusViewControls=0x39,
			FocusViewControlsQuery=0x3a,
			GetPrintTemplate=0x34,
			GetZoomRange=20,
			HideToolbars=0x18,
			HttpEquiv=0x22,
			HttpEquivDone=0x23,
			New=2,
			OnToolbarActivated=0x1f,
			OnUnload=0x25,
			Open=1,
			PageActionBlocked=0x37,
			PageActionUiQuery=0x38,
			PageSetup=8,
			Paste=13,
			PasteSpecial=14,
			PreRefresh=0x27,
			Print=6,
			Print2=0x31,
			PrintPreview=7,
			PrintPreview2=50,
			Properties=10,
			PropertyBag2=0x26,
			Redo=0x10,
			Refresh=0x16,
			Save=3,
			SaveAs=4,
			SaveCopyAs=5,
			SelectAll=0x11,
			SetDownloadState=0x1d,
			SetPrintTemplate=0x33,
			SetProgressMax=0x19,
			SetProgressPos=0x1a,
			SetProgressText=0x1b,
			SetTitle=0x1c,
			ShowFind=0x2a,
			ShowMessage=0x29,
			ShowPageActionMenu=0x3b,
			ShowPageSetup=0x2b,
			ShowPrint=0x2c,
			ShowScriptError=40,
			Spell=9,
			Stop=0x17,
			StopDownload=30,
			Undo=15,
			UpdateCommands=0x15,
			UpdatePageStatus=0x30,
			Zoom=0x13
		} // End enum OleCmdId
		#endregion

	} // End class SallIe

	#region DBWebBrowserEvents2
	[ComImport, TypeLibType( (short)0x1010 ), InterfaceType( (short)2 ), Guid( "34A715A0-6587-11D0-924A-0020AFC7AC4D" )]
	interface DWebBrowserEvents2
	{
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 0x66 )]
		void StatusTextChange( [In, MarshalAs( UnmanagedType.BStr )] string Text );
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 0x6c )]
		void ProgressChange( [In] int Progress, [In] int ProgressMax );
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 0x69 )]
		void CommandStateChange( [In] int Command, [In] bool Enable );
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 0x6a )]
		void DownloadBegin();
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 0x68 )]
		void DownloadComplete();
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 0x71 )]
		void TitleChange( [In, MarshalAs( UnmanagedType.BStr )] string Text );
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 0x70 )]
		void PropertyChange( [In, MarshalAs( UnmanagedType.BStr )] string szProperty );
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 250 )]
		void BeforeNavigate2( [In, MarshalAs( UnmanagedType.IDispatch )] object pDisp, [In, MarshalAs( UnmanagedType.Struct )] ref object URL, [In, MarshalAs( UnmanagedType.Struct )] ref object Flags, [In, MarshalAs( UnmanagedType.Struct )] ref object TargetFrameName, [In, MarshalAs( UnmanagedType.Struct )] ref object PostData, [In, MarshalAs( UnmanagedType.Struct )] ref object Headers, [In, Out] ref bool Cancel );
		[PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime ), DispId( 0x10d )]
		void SetSecureLockIcon( [In] int SecureLockIcon );
	}

	#endregion
}

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
