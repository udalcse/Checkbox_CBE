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
// Class:		MachineProfile
// Author:		Paul Alexander
// Created:		Monday, December 30, 2002 5:53:39 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using Microsoft.Win32;

namespace Xheo.Licensing
{
	/// <summary>
	/// Manages a machine profile used by the license activation system. Profiles 
	/// represent a collection of generally non volitale system properties that can
	///  collectively be used to identify certain machines.
	/// </summary>
	[Serializable]
#if LICENSING
	public 
#else
	internal
#endif
		sealed class MachineProfile
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		private readonly static Version v1_1				= new Version( 1, 1 );

		/// <summary>
		/// The current profile hash version.
		/// </summary>
		private readonly static Version CurrentVersion		= v1_1;

		private Hashtable		_properties = new Hashtable();
		private string			_hash		= null;
		private static	MachineProfile	_profile	= new MachineProfile();

		
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the MachineProfile class.
		/// </summary>
		public MachineProfile()
		{
			FillFromMachine();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties
	
		/// <summary>
		/// Gets the profile of the current machine.
		/// </summary>
		public static MachineProfile Profile
		{
			get
			{
				return _profile;
			}
		}


		/// <summary>
		/// Gets the comparable hash for the MachineProfile.
		/// </summary>
		public string Hash
		{
			get
			{
				if( _hash == null )
					_hash = GetComparableHash();
				return _hash;
			}
		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Determines if the MachineProfile is comparatively similar to the given
		/// hash.
		/// </summary>
		/// <param name="profileHash">
		///		The hash to compare to.
		/// </param>
		/// <param name="tolerance">
		///		The tolerance level.
		/// </param>
		/// <returns>
		///		Returns true if the current machine profile is within the tolerance
		///		level of the given profile hash.
		/// </returns>
		public bool CompareTo( string profileHash, int tolerance )
		{
			return CompareHash( this.GetComparableHash(), profileHash ) <= tolerance;
		}

		/// <summary>
		/// Fills the current profile from properties of the current machine.
		/// </summary>
		private void FillFromMachine()
		{
			_properties[ "00MacAddress" ]		= GetMacAddress();
			_properties[ "00Processors" ]		= GetProcessors();
			_properties[ "00ProductId" ]		= GetWindowsProductId();
			_properties[ "01394Controllers" ]	= Get1394Controllers();
			_properties[ "0IDEControllers" ]	= GetIDEControllers();
			_properties[ "0PhysicalMemory" ]	= GetPhysicalMemory();
			_properties[ "0SCSIControllers" ]	= GetSCSIControllers();
			_properties[ "0USBControllers" ]	= GetUSBControllers();
			_properties[ "AudioControllers" ]	= GetAudioControllers();
			_properties[ "Drives" ]				= GetDrives();
			_properties[ "HID" ]				= GetHID();
			_properties[ "Modems" ]				= GetModems();
			_properties[ "VideoControlers" ]	= GetVideoControllers();
		}

		/// <summary>
		/// Creates a comparable hash value for the current profile. The hash can be used
		/// to compare how similar one machine profile is to another without actually
		/// having knowledge of the exact machine characteristics.
		/// </summary>
		/// <param name="version">
		///		The hash version to generate. Use to create backwards compatible hash codes.
		/// </param>
		/// <returns>
		///		Returns a comparable has for the current machine.
		/// </returns>
		public string GetComparableHash( Version version )
		{
			StringBuilder hash	= new StringBuilder();
			StringBuilder tmp	= new StringBuilder();
			string[] keys = new string[ _properties.Keys.Count ];
			_properties.Keys.CopyTo( keys, 0 );
			Array.Sort( keys );
			Version v = v1_1;

			hash.AppendFormat( System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", v.Major, v.Minor );

			foreach( string key in keys )
			{
				if( hash.Length > 0 )
					hash.Append( "-" );
				object value = _properties[ key ];
				if( value == null )
					value = "";
				if( value.GetType() == typeof( string[] ) )
				{
					tmp.Length = 0;
					foreach( string entry in ((string[])value) )
					{
						if( tmp.Length > 0 )
							tmp.Append( "," );
						
						tmp.AppendFormat( CultureInfo.InvariantCulture, "{0:X}", entry == null ? GetHashCode( "" ) : GetHashCode( entry ) );
					}

					hash.AppendFormat( CultureInfo.InvariantCulture, "{0:X}", GetHashCode( tmp.ToString() ) );
				}
				else
				{
					hash.AppendFormat( CultureInfo.InvariantCulture, "{0:X}", GetHashCode( value.ToString() ) );
				}
			}
			return hash.ToString();
		}
		#region Overloads
		/// <summary>
		/// Creates a comparable hash value for the current profile. The hash can be used
		/// to compare how similar one machine profile is to another without actually
		/// having knowledge of the exact machine characteristics.
		/// </summary>
		/// <returns>
		///		Returns a comparable has for the current machine.
		/// </returns>
		public string GetComparableHash()
		{
			return GetComparableHash( CurrentVersion );
		}
		#endregion

		/// <summary>
		/// Compares two machine profile hashes and determines the amount of difference.
		/// </summary>
		/// <param name="hash">
		///		The of the machine to compare.
		/// </param>
		/// <param name="comparedHash">
		///		The hashed value of another machine profile to compare to.
		/// </param>
		/// <returns>
		///		Returns the difference rating from 0 (same) to 10 (very different).
		/// </returns>
		public static int CompareHash( string hash, string comparedHash )
		{
			if( hash == null )
				throw new ArgumentNullException( "hash" );

			if( comparedHash == null )
				throw new ArgumentNullException( "comparedHash" );

			double differences = 0.0;
			string[] values			= hash.Split( '-' );
			string[] comparedValues	= comparedHash.Split( '-' );

			if( values.Length != comparedValues.Length )
				return 10;

			for( int index = 0; index < values.Length; index++ )
			{
				string	value			= values[ index ];
				string	comparedValue	= comparedValues[ index ];


				if( String.Compare( value, comparedValue, false, CultureInfo.InvariantCulture ) != 0 )
				{
					if( index == 0 )		// Version
						differences += 10;
					else if( index == 3 )
						differences += 0;
					else if( index <= 3 )	// Primary hardware and windows ID
						differences += 2;
					else if( index <= 9 )	// Misc
						differences += 1;
					else					// Volatile 
						differences += 0.5;
				}
			}

			
			return differences > 10 ? 10 : (int)Math.Ceiling( differences );
		}

		/// <summary>
		/// Gets the name of the hardware types that are different between the two hashes.
		/// </summary>
		/// <param name="hash">
		///		The of the machine to compare.
		/// </param>
		/// <param name="comparedHash">
		///		The hashed value of another machine profile to compare to.
		/// </param>
		/// <returns>
		///		Returns the collection of hardware changes detected between the two hashes.
		/// </returns>
		public static string[] GetDifferences( string hash, string comparedHash )
		{
			ArrayList differences = new ArrayList();

			if( hash == null )
				throw new ArgumentNullException( "hash" );

			if( comparedHash == null )
				throw new ArgumentNullException( "comparedHash" );

			string[] values			= hash.Split( '-' );
			string[] comparedValues	= comparedHash.Split( '-' );

			string[] hardware = new string[] {
												 "Activation Version",
												 "MAC Address",
												 "Processors",
												 "Product ID",
												 "1394 Controllers",
												 "IDE Controllers",
												 "Physical Memory",
												 "SCSI Controllers",
												 "USB Controllers",
												 "Audio Controllers",
												 "Drives",
												 "Human Interface Devices",
												 "Modems",
												 "Video Controlers"
											 };

			if( values.Length != comparedValues.Length )
				return hardware;

			for( int index = 0; index < values.Length; index++ )
			{
				string	value			= values[ index ];
				string	comparedValue	= comparedValues[ index ];


				if( String.Compare( value, comparedValue, false, CultureInfo.InvariantCulture ) != 0 )
					differences.Add( hardware[ index ] );
			}

			return differences.ToArray( typeof( string ) ) as string[];
		}

		/// <summary>
		/// Gets the Product ID for the current windows OS installation.
		/// </summary>
		/// <returns>
		///		Returns the Product ID for the current windows OS installation.
		/// </returns>
		private string GetWindowsProductId()
		{
			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( "SOFTWARE\\Microsoft\\Windows\\CurrentVersion", false ) )
			{
				if( key == null )
					return Guid.NewGuid().ToString();
				   
				return key.GetValue( "ProductId", "None!" ) as string;
			}
		}

		/// <summary>
		/// Gets the Hardware IDs of any installed IDE drives.
		/// </summary>
		/// <returns>
		///		Returns a list of hardware IDs of any installed IDE drives.
		/// </returns>
		private string[] GetDrives()
		{	
			ArrayList	drives = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Enum\IDE" ) )
			{
				if( key != null )
				{
					foreach( string subKeyName in key.GetSubKeyNames() )
					{
						using( RegistryKey deviceKey = key.OpenSubKey( subKeyName ) )
						{
							if( deviceKey != null )
							{
								foreach( string instanceKeyName in deviceKey.GetSubKeyNames() )
								{
									using( RegistryKey instanceKey = deviceKey.OpenSubKey( instanceKeyName ) )
									{
										if( instanceKey != null )
										{
											object value = instanceKey.GetValue( "HardwareID" );
											if( value is string[] )
												drives.Add( ((string[])value)[ 0 ] );
											else
												drives.Add( (string)value );
										}
									}
								}
							}
						}
					}
				}
			}

			return drives.ToArray( typeof( string ) ) as string[];
		}

		/// <summary>
		/// Gets an the number of bytes of physical memory installed on the machine.
		/// </summary>
		/// <returns>
		///		Returns the number of bytes of physical memory instlaled on the machine.
		/// </returns>
		private string GetPhysicalMemory()
		{
			UInt64			memory	= 0;			
	
			try
			{
				if( Environment.OSVersion.Platform != PlatformID.Win32NT ||
					Environment.OSVersion.Version < new Version( 4, 0 ) )
				{
					_MEMORYSTATUS memstat = new _MEMORYSTATUS();
					memstat.dwLength = (uint)SafeSizeof( typeof( _MEMORYSTATUS ) );
					GlobalMemoryStatus( ref memstat );
					memory = memstat.dwTotalPhys;
				}
				else
				{
					_MEMORYSTATUSEX memstat = new _MEMORYSTATUSEX();
					memstat.dwLength = (uint)SafeSizeof( typeof( _MEMORYSTATUSEX ) );
					GlobalMemoryStatusEx( ref memstat );
					memory = memstat.ullTotalPhys;			
				}
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( ex.ToString() );
				return Guid.NewGuid().ToString();
			}

			return memory.ToString( System.Globalization.CultureInfo.InvariantCulture );
		}
		#region Helpers
		private int SafeSizeof( Type type )
		{
			if( type == null )
				throw new ArgumentNullException( "type" );	
			
			try
			{
				return SafeSizeofRecursive( type, 0 );				
			}
			catch( Exception ex )
			{
				throw new NotSupportedException( null, ex );
			}
		}
		private int SafeSizeofRecursive( Type type, int depth )
		{
			if( depth > 10 )
				throw new StackOverflowException();

			if( type.IsPrimitive )
			{
				Array array = Array.CreateInstance( type, 1 );
				return Buffer.ByteLength( array );
			}
			else
			{
				int size = 0;
				foreach( System.Reflection.FieldInfo field in type.GetFields() )
				{
					size += SafeSizeofRecursive( field.FieldType, depth + 1 );
				}

				return size;
			}
		}
		#endregion

		///<summary>
		///Summary of GetVideoControllers.
		///</summary>
		///<returns></returns>	
		private string[] GetVideoControllers()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{4D36E968-E325-11CE-BFC1-08002BE10318}" ) )
			{
				if( key != null )
				{
					foreach( string id in key.GetSubKeyNames() )
					{
						if( id.StartsWith( "0" ) )
						{
							using( RegistryKey deviceKey = key.OpenSubKey( id ) )
							{
								if( deviceKey != null )
								{
									string deviceId = deviceKey.GetValue( "DriverDesc" ) as string + deviceKey.GetValue( "MatchingDeviceId" ) as string;
									if( deviceId != null )
										controllers.Add( deviceId );
								}
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		///<summary>
		///Summary of GetUSBControllers.
		///</summary>
		///<returns></returns>	
		private string[] GetUSBControllers()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{36FC9E60-C465-11CF-8056-444553540000}" ) )
			{
				if( key != null )
				{
					foreach( string id in key.GetSubKeyNames() )
					{
						if( id.StartsWith( "0" ) )
						{
							using( RegistryKey deviceKey = key.OpenSubKey( id ) )
							{
								if( deviceKey != null )
								{
									string deviceId = deviceKey.GetValue( "DriverDesc" ) as string + deviceKey.GetValue( "MatchingDeviceId" ) as string;
									if( deviceId != null )
										controllers.Add( deviceId );
								}
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		///<summary>
		///Summary of GetHID.
		///</summary>
		///<returns></returns>	
		private string[] GetHID()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{745A17A0-74D3-11D0-B6FE-00A0C90F57DA}" ) )
			{
				if( key != null )
				{
					foreach( string id in key.GetSubKeyNames() )
					{
						if( id.StartsWith( "0" ) )
						{
							using( RegistryKey deviceKey = key.OpenSubKey( id ) )
							{
								if( deviceKey != null )
								{
									string deviceId = deviceKey.GetValue( "DriverDesc" ) as string + deviceKey.GetValue( "MatchingDeviceId" ) as string;
									if( deviceId != null )
										controllers.Add( deviceId );
								}
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		///<summary>
		///Summary of GetIDEControllers.
		///</summary>
		///<returns></returns>	
		private string[] GetIDEControllers()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{4D36E96A-E325-11CE-BFC1-08002BE10318}" ) )
			{
				if( key != null )
				{
					foreach( string id in key.GetSubKeyNames() )
					{
						if( id.StartsWith( "0" ) )
						{
							using( RegistryKey deviceKey = key.OpenSubKey( id ) )
							{
								if( deviceKey != null )
								{
									string deviceId = deviceKey.GetValue( "DriverDesc" ) as string + deviceKey.GetValue( "MatchingDeviceId" ) as string;
									if( deviceKey.GetValue( "MasterIdDataCheckSum" ) != null )
										deviceId += deviceKey.GetValue( "MasterIdDataCheckSum" ).ToString();
									if( deviceId != null )
										controllers.Add( deviceId );
								}
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		///<summary>
		///Summary of GetSCSIControllers.
		///</summary>
		///<returns></returns>	
		private string[] GetSCSIControllers()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{4D36E97B-E325-11CE-BFC1-08002BE10318}" ) )
			{
				if( key != null )
				{
					foreach( string id in key.GetSubKeyNames() )
					{
						if( id.StartsWith( "0" ) )
						{
							using( RegistryKey deviceKey = key.OpenSubKey( id ) )
							{
								if( deviceKey != null )
								{
									string deviceId = deviceKey.GetValue( "DriverDesc" ) as string + deviceKey.GetValue( "MatchingDeviceId" ) as string;
									if( deviceKey.GetValue( "MasterIdDataCheckSum" ) != null )
										deviceId += deviceKey.GetValue( "MasterIdDataCheckSum" ).ToString();
									if( deviceId != null )
										controllers.Add( deviceId );
								}
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		///<summary>
		///Summary of Get1394Controllers.
		///</summary>
		///<returns></returns>	
		private string[] Get1394Controllers()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{6BDD1FC1-810F-11D0-BEC7-08002BE2092F}" ) )
			{
				if( key != null )
				{
					foreach( string id in key.GetSubKeyNames() )
					{
						if( id.StartsWith( "0" ) )
						{
							using( RegistryKey deviceKey = key.OpenSubKey( id ) )
							{
								if( deviceKey != null )
								{
									string deviceId = deviceKey.GetValue( "DriverDesc" ) as string + deviceKey.GetValue( "MatchingDeviceId" ) as string;
									if( deviceId != null )
										controllers.Add( deviceId );
								}
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		///<summary>
		///Summary of GetAudioControllers.
		///</summary>
		///<returns></returns>	
		private string[] GetAudioControllers()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{4D36E96C-E325-11CE-BFC1-08002BE10318}" ) )
			{
				if( key != null )
				{
					foreach( string id in key.GetSubKeyNames() )
					{
						using( RegistryKey subKey = key.OpenSubKey( "Settings" ) )
						{
							if( subKey != null )
							{
								if( subKey != null )
								{
									using( RegistryKey deviceKey = key.OpenSubKey( id ) )
									{
										if( deviceKey != null )
										{
											string deviceId = deviceKey.GetValue( "DriverDesc" ) as string + deviceKey.GetValue( "MatchingDeviceId" ) as string;
											if( deviceId != null )
												controllers.Add( deviceId );
										}
									}
								}
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		///<summary>
		///Summary of GetModems.
		///</summary>
		///<returns></returns>	
		private string[] GetModems()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{4D36E96D-E325-11CE-BFC1-08002BE10318}" ) )
			{
				if( key != null )
				{
					foreach( string id in key.GetSubKeyNames() )
					{
						using( RegistryKey subKey = key.OpenSubKey( "Settings" ) )
						{
							if( subKey != null )
							{
								using( RegistryKey deviceKey = key.OpenSubKey( id ) )
								{
									if( deviceKey != null )
									{
										string deviceId = deviceKey.GetValue( "DriverDesc" ) as string + deviceKey.GetValue( "MatchingDeviceId" ) as string;
										if( deviceId != null )
											controllers.Add( deviceId );
									}
								}
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		///<summary>
		///Summary of GetProcessors.
		///</summary>
		///<returns></returns>	
		private string[] GetProcessors()
		{	
			ArrayList	controllers = new ArrayList();

			using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Class\{50127DC3-0F36-415E-A6CC-4CB3BE910B65}" ) )
			{
				if( key == null )
					return new string[] { "Unknown" };

				foreach( string id in key.GetSubKeyNames() )
				{
					if( id.StartsWith( "0" ) )
					{
						using( RegistryKey deviceKey = key.OpenSubKey( id ) )
						{
							if( deviceKey != null )
							{
								string deviceId = deviceKey.GetValue( "MatchingDeviceId" ) as string;
								if( deviceId != null )
									controllers.Add( deviceId );
							}
						}
					}
				}
			}

			return controllers.ToArray( typeof( string ) ) as string[];
		}

		/// <summary>
		/// Gets the MAC address of the first network card found on the machine.
		/// </summary>
		/// <returns>
		///		Returns a string version of the MAC address if found, otherwise null.
		/// </returns>
		private string GetMacAddress()
		{
			try
			{
				ArrayList adapters = GetAdapterInfo.GetAdapters();

				for( int index = 0; index < adapters.Count; index++ )
				{
					IP_ADAPTERINFO_STRUCTURE ipinfo = (IP_ADAPTERINFO_STRUCTURE)adapters[ index ];
				
					StringBuilder mac = new StringBuilder();
					for( int ix = 0; ix < ipinfo.AddressLength; ix++ )
					{
						if( mac.Length > 0 )
							mac.Append( '-' );

						mac.AppendFormat( System.Globalization.CultureInfo.InvariantCulture, "{0:X2}", ipinfo.Address[ ix ] );
					}
					
					string address = mac.ToString();
					if( address == "00-00-00-00-00-00" )
						continue;

					return address;
				}

				return Guid.NewGuid().ToString();
			}
			catch
			{
				return GetMacAddressFromUuid();
			}
		}
		#region Helpers
		private string GetMacAddressFromUuid()
		{
			try
			{
				TGuid guid = new TGuid();
			
				if( Environment.OSVersion.Platform != PlatformID.Win32NT ||
					Environment.OSVersion.Version < new Version( 4, 0 ) )
					UuidCreate( ref guid );
				else
					UuidCreateSequential( ref guid );
			
				return String.Format( System.Globalization.CultureInfo.InvariantCulture,
					"{0:X2}-{1:X2}-{2:X2}-{3:X2}-{4:X2}-{5:X2}", 
					guid.m1,
					guid.m2,
					guid.m3,
					guid.m4,
					guid.m5,
					guid.m6);
			}
			catch
			{
				return Guid.NewGuid().ToString();
			}
		}
		#endregion

		#region Win32 Stuff

		#region IP Config

		// Adapted from http://www.codeguru.com/network/GetMAC.html

		/// <summary>
		/// A class to contain the constants required for the IP_ADAPTER_INFO struct
		/// </summary>
		private class Constants
		{
			/// <summary>
			/// The maximum length of an adapter name
			/// </summary>
			public const int MAX_ADAPTER_NAME_LENGTH = 260;

			/// <summary>
			/// The maximum length of the description of an adapter
			/// </summary>
			public const int MAX_ADAPTER_DESCRIPTION_LENGTH = 132;

			/// <summary>
			/// The maximum length for the physical address of the adapter
			/// </summary>
			public const int MAX_ADAPTER_ADDRESS_LENGTH = 8;
		} // End class Constants

		/// <summary>
		/// declare the IP_ADDR_STRING structure
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct IP_ADDR_STRING
		{
			/// <summary>
			/// Pointer to the next IP_ADDR_STRING structure in the list
			/// </summary>
			public int Next;

			/// <summary>
			/// This array holds the IP address
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=16)]
			public char[] IpAddress;

			/// <summary>
			/// This array holds the IP address mask
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=16)]
			public char[] IpMask;

			/// <summary>
			/// Specifies network table entry (NTE). This value corresponds to the NTEContext parameters in the AddIPAddress and DeleteIPAddress functions
			/// </summary>
			public int Context;
		} // End struct IP_ADDR_STRING

		/// <summary>
		/// declare the IP_ADAPTERINFO_STRUCTURE structure
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct IP_ADAPTERINFO_STRUCTURE
		{
			/// <summary>
			/// Pointer to the next adapter in the list of adapters
			/// </summary>
			public uint Next;

			/// <summary>
			/// Reserved
			/// </summary>
			public int ComboIndex;

			/// <summary>
			/// Specifies the name of the adapter
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=Constants.MAX_ADAPTER_NAME_LENGTH)]
			public char[] AdapterName;

			/// <summary>
			/// Specifies a description for the adapter
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=Constants.MAX_ADAPTER_DESCRIPTION_LENGTH)]
			public char[] Description;

			/// <summary>
			/// Specifies the length of the hardware address for the adapter
			/// </summary>
			public int AddressLength;

			/// <summary>
			/// Specifies the hardware address for the adapter
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=Constants.MAX_ADAPTER_ADDRESS_LENGTH)]
			public byte[] Address;

			/// <summary>
			/// Specifies the adapter index
			/// </summary>
			public int Index;

			/// <summary>
			/// Specifies the adapter type. The type is one of the following values. 
			///IF_OTHER_ADAPTERTYPE 
			///IF_ETHERNET_ADAPTERTYPE 
			///IF_TOKEN_RING_ADAPTERTYPE 
			///IF_FDDI_ADAPTERTYPE 
			///IF_PPP_ADAPTERTYPE 
			///IF_LOOPBACK_ADAPTERTYPE 
			///IF_SLIP_ADAPTERTYPE 
			///These values are defined in the header file IPIfCons.h
			/// </summary>
			public int Type;

			/// <summary>
			/// Specifies whether dynamic host configuration protocol (DHCP) is enabled for this adapter
			/// </summary>
			public int DhcpEnabled;

			/// <summary>
			/// Specifies the current IP address for this adapter
			/// </summary>
			public int CurrentIPAddress;

			/// <summary>
			/// Specifies the list of IP addresses associated with this adapter
			/// </summary>
			public IP_ADDR_STRING IpAddressList;

			/// <summary>
			/// Specifies the IP address of the default gateway for this adapter
			/// </summary>
			public IP_ADDR_STRING GatewayList;

			/// <summary>
			/// Specifies the IP address of the DHCP server for this adapter
			/// </summary>
			public IP_ADDR_STRING DhcpServer;

			/// <summary>
			/// Specifies whether this adapter uses Windows Internet Name Service (WINS)
			/// </summary>
			public byte HaveWins;

			/// <summary>
			/// Specifies the IP address of the primary WINS server
			/// </summary>
			public IP_ADDR_STRING PrimaryWinsServer;

			/// <summary>
			/// Specifies the IP address of the secondary WINS server
			/// </summary>
			public IP_ADDR_STRING SecondaryWinsServer;

			/// <summary>
			/// Specifies the time when the current DHCP lease was obtained
			/// </summary>
			public int LeaseObtained;

			/// <summary>
			/// Specifies the time when the current DHCP lease will expire
			/// </summary>
			public int LeaseExpires;
		} // End struct IP_ADAPTERINFO_STRUCTURE

		//import the functions from the dll
		/// <summary>
		/// Uses the windows API to retrieve information about the IP configuration of the local machine
		/// </summary>
		private class GetAdapterInfo
		{
			/// <summary>
			/// Constructor for the GetAdapterInfo Class
			/// </summary>
			public GetAdapterInfo()
			{
				
			}

			/// <summary>
			/// Gets the details of the first available adapter on the local machine
			/// </summary>
			/// <returns>The return value is an struct of type IP_ADAPTERINFO_STRUCTURE containing data about the Adapter</returns>
			public static IP_ADAPTERINFO_STRUCTURE GetFirstAdapter()
			{
				//declare a unit to hold the buffersize
				uint uintBufferSize = 0;

				//run the method once to find the size of the buffer required
				if( GetAdaptersInfo( new IntPtr(0), ref uintBufferSize ) != 111 )
					throw new ApplicationException( "Error retrieving buffer size" );

				//declare a space in the unmanaged memory to hold the data
				IntPtr pBuffer = Marshal.AllocCoTaskMem( (int)uintBufferSize );

				//run the function
				if( GetAdaptersInfo( pBuffer, ref uintBufferSize ) != 0 )
					throw new ApplicationException( "Error getting adapter info" );

				//read the data from the space in memory into a structure and return it to the client
				return (IP_ADAPTERINFO_STRUCTURE)Marshal.PtrToStructure(pBuffer,typeof(IP_ADAPTERINFO_STRUCTURE));
			}

			/// <summary>
			/// Gets details of all the adapters on the local machine
			/// </summary>
			/// <returns>The return object is a System.Collections.ArrayList of IPConfig.IP_ADAPTERINFO_STRUCTURE structs containing data about the Adapter</returns>
			public static ArrayList GetAdapters()
			{
				//declare an arraylist to hold adapter info
				ArrayList Adapters = new ArrayList();

				//declare an adapter structure to hold the adapter data
				IP_ADAPTERINFO_STRUCTURE Adapter;

				//get the first adapter from the machine
				Adapter = GetFirstAdapter();

				//add it to the arraylist
				Adapters.Add( Adapter );

				//loop throughchecking for new adapters until there are no more
				while( Adapter.Next != 0 )
				{
					Adapter = (IP_ADAPTERINFO_STRUCTURE)Marshal.PtrToStructure( new IntPtr( (int)Adapter.Next ), typeof( IP_ADAPTERINFO_STRUCTURE ) );
					Adapters.Add(Adapter);
				}

				//return the adapter array
				return Adapters;
			}
		}

		#endregion

		///<summary>
		///Summary of UuidCreate.
		///</summary>
		///<param name="guid"></param>
		///<returns></returns>	
		[ DllImport( "rpcrt4.dll", SetLastError = true ) ]
		private static extern uint UuidCreate( ref TGuid guid );

		///<summary>
		///Summary of UuidCreateSequential.
		///</summary>
		///<param name="guid"></param>
		///<returns></returns>	
		[ DllImport( "rpcrt4.dll", SetLastError = true ) ]
		private static extern uint UuidCreateSequential( ref TGuid guid );

		/// <summary>
		/// The Win32 function to retrieve information about the adapter
		/// </summary>
		[DllImport("IPHlpAPI.dll")]
		private static extern int GetAdaptersInfo(IntPtr pAdapterInfo, ref uint pOutBufLen);


		///<summary>
		///Summary of TGuid
		///</summary>

		private struct TGuid
		{
			///<summary>
			///</summary>
			public Int16 a;
			///<summary>
			///</summary>
			public Int16 b;
			///<summary>
			///</summary>
			public Int16 d;
			///<summary>
			///</summary>
			public Int16 m;
			///<summary>
			///</summary>
			public Int16 s;
			///<summary>
			///</summary>
			public Byte m1;
			///<summary>
			///</summary>
			public Byte m2;
			///<summary>
			///</summary>
			public Byte m3;
			///<summary>
			///</summary>
			public Byte m4;
			///<summary>
			///</summary>
			public Byte m5;
			///<summary>
			///</summary>
			public Byte m6;
		}
		///<summary>
		///Summary of GlobalMemoryStatus.
		///</summary>
		///<param name="memstat"></param>
		[ DllImport( "kernel32.dll" ) ]
		private static extern void GlobalMemoryStatus( ref _MEMORYSTATUS memstat );

		///<summary>
		///Summary of GlobalMemoryStatusEx.
		///</summary>
		///<param name="memstat"></param>
		///<returns></returns>	
		[ DllImport( "kernel32.dll", SetLastError = true ) ]
		private static extern int GlobalMemoryStatusEx( ref _MEMORYSTATUSEX memstat );

		///<summary>
		///Summary of _MEMORYSTATUS
		///</summary>

		private struct _MEMORYSTATUS
		{
			///<summary>
			///</summary>
			public UInt32 dwLength;
			///<summary>
			///</summary>
			public UInt32 dwMemoryLoad;
			///<summary>
			///</summary>
			public UInt32 dwTotalPhys;
			///<summary>
			///</summary>
			public UInt32 dwAvailPhys;
			///<summary>
			///</summary>
			public UInt32 dwTotalPageFile;
			///<summary>
			///</summary>
			public UInt32 dwAvailPageFile;
			///<summary>
			///</summary>
			public UInt32 dwTotalVirtual;
			///<summary>
			///</summary>
			public UInt32 dwAvailVirtual;
		}

		///<summary>
		///Summary of _MEMORYSTATUSEX
		///</summary>

		private struct _MEMORYSTATUSEX
		{
			///<summary>
			///</summary>
			public UInt32 dwLength;
			///<summary>
			///</summary>
			public UInt32 dwMemoryLoad;
			///<summary>
			///</summary>
			public UInt64 ullTotalPhys;
			///<summary>
			///</summary>
			public UInt64 ullAvailPhys;
			///<summary>
			///</summary>
			public UInt64 ullTotalPageFile;
			///<summary>
			///</summary>
			public UInt64 ullAvailPageFile;
			///<summary>
			///</summary>
			public UInt64 ullTotalVirtual;
			///<summary>
			///</summary>
			public UInt64 ullAvailVirtual;
			///<summary>
			///</summary>
			public UInt64 ullAvailExtendedVirtual;
		}
		#endregion

		#region GetHashCode
		private static int GetHashCode( string s )
		{
			uint hash = 5381u;

			int length = s.Length;
			for( int index = 0; index < length; index++ )
			{
				hash = unchecked( ( ( hash << 5 ) + hash ) ^ s[ index ] );
			}

			return unchecked( (int)hash );
		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class MachineProfile
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////