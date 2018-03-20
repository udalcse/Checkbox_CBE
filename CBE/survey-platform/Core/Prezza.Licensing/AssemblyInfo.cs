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

////////////////////////////////////////////////////////////////////////////////
#warning Please remember that you are not permitted to distribute custom builds of the Xheo.Licensing.dll without first renaming it. You may renamed the dll to anything you'd like, but it may not contain the word "XHEO" in the file name.
////////////////////////////////////////////////////////////////////////////////


using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security;
using System.Diagnostics;
using Xheo.Licensing;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Prezza Licensing Engine")]
[assembly: AssemblyDescription("Licensing support for .NET")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Checkbox Survey Solutions, Inc")]
[assembly: AssemblyProduct("Prezza.Licensing")]
[assembly: AssemblyCopyright("Used under license from original author.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]	
[assembly: CLSCompliant( true )]
[assembly: ComVisible( true ) ]
[assembly: SecurityRules(SecurityRuleSet.Level1)]


// PERMISSIONS
//[assembly: IsolatedStorageFilePermission( SecurityAction.RequestMinimum, UsageAllowed = IsolatedStorageContainment.DomainIsolationByUser ) ]
//[assembly: PermissionSet( SecurityAction.RequestMinimum, Name = "Nothing" ) ]
[assembly: AllowPartiallyTrustedCallers]

// RELEASES

// Bug in v1.x does not permit this for "fallback" usage. :(
[assembly: System.Resources.SatelliteContractVersion( "2.0.0.0" ) ]
#if NET11
	[assembly: AssemblyVersion( "2.1.5000.0" )]
	[assembly: AssemblyFileVersion( "2.1.5000.27" )]
#else
	[assembly: AssemblyVersion( "2.1.3300.0" )]
	[assembly: AssemblyFileVersion( "2.1.3300.27" )]
#endif

#if ! DEBUG
[assembly: LicenseHelp( "XHEO", "support@xheo.com", "http://www.xheo.com/support" )]
#endif


[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("..\\..\\..\\Shared\\Keys.snk")]
[assembly: AssemblyKeyName("")]
[assembly: Internal.StaticResources( ResourceName = "Xheo.Licensing.Resources.Xheo.Licensing", Inherit=true )]
[assembly: Xheo.Licensing.LicenseHelper( typeof( Xheo.Licensing.XheoLicenseHelper ) ) ]