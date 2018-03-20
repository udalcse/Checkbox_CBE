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
// Class:		StaticResourceProvider
// Author:		Paul Alexander
// Created:		Monday, June 09, 2003 10:51:50 AM
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Text;
#if SHARED || FORMRESOURCES
using System.Windows.Forms;
#endif
using System.Diagnostics;
using System.ComponentModel;

#if ! SHARED && ! RESOURCEPROVIDER && ! NOSHAREDWARNINGS
#warning You should define the RESOURCEPROVIDER directive in your project confiruation to alert other embedded files that the StaticResourceProvider object is available. You can define NOSHAREDWARNINGS to suppress this warning.
#endif

#if ! SHARED && ! FORMRESOURCES && ! NOSHAREDWARNINGS
#warning You should define the FORMRESOURCES directive in your project configruation if you would like support for localizing windows forms. You can define NOSHAREDWARNINGS to suppress this warning.
#endif

namespace Internal
{
	/// <summary>
	/// Implements static methods for retrieving static language resource from
	/// an assembly and supporting files.
	/// </summary>
	/// <remarks>
	///		For the provider to locate resources, the default namespace must be
	///		the same as the assembly name. All resources should be stored in a 
	///		Resources namespace (folder).
	/// </remarks>
	[ System.Runtime.InteropServices.ComVisible( false ) ]
	internal	sealed class StaticResourceProvider
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private Hashtable	_resourceManagers	= new Hashtable();
		private string		_assemblyName		= null;
		private Assembly	_assembly			= null;
		
		private static Hashtable	_providers			= new Hashtable();
		private static Hashtable	_resources			= new Hashtable();

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the StaticResourceProvider class.
		/// </summary>
		public StaticResourceProvider( string assemblyName )
		{
			_assemblyName = assemblyName;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the name of the assembly this instance will load resources for.
		/// </summary>
		public string AssemblyName
		{
			get
			{
				return _assemblyName;
			}
		}

		/// <summary>
		/// Gets the root assembly to load resources for.
		/// </summary>
		private Assembly Assembly
		{
			get
			{
				if( _assembly == null )
					_assembly = Assembly.Load( AssemblyName );
				return _assembly;
			}
		}

		/// <summary>
		/// Gets the <see cref="StaticResourceProvider"/> for the current assembly.
		/// </summary>
		public static StaticResourceProvider CurrentProvider
		{
			get
			{
				StackTrace trace = new StackTrace( 1, false );
				return MakeProviderForAssembly( trace.GetFrame( 0 ).GetMethod().DeclaringType.Assembly );
			}
		}

		/// <summary>
		/// Gets the <see cref="StaticResourceProvider"/> for the assembly of the
		/// method that called the current method.
		/// </summary>
		public static StaticResourceProvider ParentProvider
		{
			get
			{
				StackTrace trace = new StackTrace( 2, false );
				return MakeProviderForAssembly( trace.GetFrame( 0 ).GetMethod().DeclaringType.Assembly );
			}
		}	

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Gets a <see cref="StaticResourceProvider"/> for the given assembly.
		/// </summary>
		/// <param name="hostAssembly">
		///		The assemlby to get the resources for.
		/// </param>
		/// <returns>
		///		Returns a new StaticResourceProvider.
		/// </returns>
		public static StaticResourceProvider MakeProviderForAssembly( string hostAssembly )
		{
			StaticResourceProvider provider = null;
			if( _providers[ hostAssembly ] == null )
			{
				lock( typeof( StaticResourceProvider ) )
				{
					if( _providers[ hostAssembly ] == null )
					{
						provider = new StaticResourceProvider( hostAssembly );
						_providers[ hostAssembly ] = provider;
					}
				}
			}

			if( provider == null )
				provider = _providers[ hostAssembly ] as StaticResourceProvider;

			return provider;
		}
		#region Overloads

		/// <summary>
		/// Gets a <see cref="StaticResourceProvider"/> for the given assembly.
		/// </summary>
		/// <param name="hostAssembly">
		///		The assemlby to get the resources for.
		/// </param>
		/// <returns>
		///		Returns a new StaticResourceProvider.
		/// </returns>
		public static StaticResourceProvider MakeProviderForAssembly( Assembly hostAssembly )
		{
			StaticResourceProvider provider = MakeProviderForAssembly( hostAssembly.FullName );			
			if( provider._assembly == null )
				provider._assembly = hostAssembly;
			return provider;
		}

		#endregion

#if SHARED || FORMRESOURCES
		/// <summary>
		/// Localizes a menu resource.
		/// </summary>
		/// <param name="menu">
		///		The menu to localize.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <param name="culture">
		///		The culture to load the resource for. If null uses the current UI culture.
		/// </param>
		/// <remarks>
		///		LocalizeMenu looks through each of the menu items and attempts to localize
		///		the name. The menu name must be prefixed with the pound sign (#) for it
		///		to be localized.
		/// </remarks>
		public void LocalizeMenu( Menu menu, bool inherit, CultureInfo culture )
		{
			if( menu == null )
				throw new ArgumentNullException( "menu" );
			LocalizeMenuItems( menu.MenuItems, inherit, culture );
		}
		#region Helpers
		private void LocalizeMenuItems( Menu.MenuItemCollection items, bool inherit, CultureInfo culture )
		{
			foreach( MenuItem item in items )
			{
				if( item.Text != null && item.Text.StartsWith( "#" ) )
					item.Text = GetString( item.Text.Substring( 1 ), inherit, false, culture );
				if( item.IsParent )
					LocalizeMenuItems( item.MenuItems, inherit, culture );
			}
		}
		#endregion

		/// <summary>
		/// Localizes a windows form control.
		/// </summary>
		/// <param name="control">
		///		The control to localize.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <param name="culture">
		///		The culture to load the resource for. If null uses the current UI culture.
		/// </param>
		public void LocalizeControl( System.Windows.Forms.Control control, bool inherit, CultureInfo culture )
		{
			if( control == null )
				throw new ArgumentNullException( "control" );

			if( culture == null )
				culture = CultureInfo.CurrentUICulture;

			if( control.Text != null && control.Text.StartsWith( "#" ) )
			{
				string id = control.Text.Substring( 1 );
				control.Text = GetString( id, inherit, false, culture );
				if( control.Text == id )
					control.Text = "#"  + id;
			}

			if( control.HasChildren )
				foreach( Control child in control.Controls )
					LocalizeControl( child, inherit, culture );

			if( control.ContextMenu != null )
				LocalizeMenu( control.ContextMenu, inherit, culture );
			
			if( control is Form )
			{
				Form form = control as Form;
				if( form.Menu != null )
					LocalizeMenu( form.Menu, inherit, culture );
			}

			if( control is ListView )
			{
				foreach( ColumnHeader child in ((ListView)control).Columns )
					LocalizeObject( child, inherit, culture );
			}
			else if( control is ToolBar )
			{
				foreach( ToolBarButton button in ((ToolBar)control).Buttons )
					LocalizeObject( button, inherit, culture );
			}
			else if( control is StatusBar )
			{
				foreach( StatusBarPanel panel in ((StatusBar)control).Panels )
					LocalizeObject( panel, inherit, culture );
			}
		}
		#region Overloads
		/// <summary>
		/// Localizes a windows form control.
		/// </summary>
		/// <param name="control">
		///		The control to localize.
		/// </param>
		public void LocalizeControl( System.Windows.Forms.Control control )
		{
			StaticResourcesAttribute sra = Attribute.GetCustomAttribute( Assembly, typeof( StaticResourcesAttribute ), true ) as StaticResourcesAttribute;
			bool inherit = false;
			if( sra != null )
				inherit = sra.Inherit;
			LocalizeControl( control, inherit, null );
		}
		#endregion
#endif
		
		/// <summary>
		///		Localizes an object. All public string properties will be checked.
		/// </summary>
		/// <param name="obj">
		///		The object to localize.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <param name="culture">
		///		The culture to load the resource for. If null uses the current UI culture.
		/// </param>
		public void LocalizeObject( object obj, bool inherit, CultureInfo culture )
		{
			Stack stack = new Stack();
			LocalizeObject( obj, inherit, culture, stack );
		}
		#region Helpers
		private void LocalizeObject( object obj, bool inherit, CultureInfo culture, Stack stack )
		{
			foreach( PropertyDescriptor property in TypeDescriptor.GetProperties( obj ) )
			{
				if( property.IsLocalizable )
				{
					if( property.PropertyType == typeof( string ) && ! property.IsReadOnly )
					{
						string value = property.GetValue( obj ) as string;
						if( value != null && value.StartsWith( "#" ) )
							property.SetValue( obj, GetString( value.Substring( 1 ), inherit, false, culture ) );
					}
					else if( typeof( ICollection ).IsAssignableFrom( property.PropertyType ) )
					{
						stack.Push( obj );
						ICollection collection = property.GetValue( obj ) as ICollection;
						if( collection != null )
							foreach( object child in collection )
							{
								if( ! stack.Contains( child ) )
									LocalizeObject( child, inherit, culture, stack );
							}
						stack.Pop();
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// Gets a resource string with the given resource name.
		/// </summary>
		/// <param name="resourceName">
		///		Name of the resource that provides the text for the message.
		/// </param>
		/// <param name="throwOnError">
		///		Indicates if an exception is thrown if the resource is not found. When
		///		false and the resource is not found, the resource name will be returned
		///		instead.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <param	 name="culture">
		///		The culture to load the resource for. If null uses the current UI culture.
		/// </param>
		/// <returns>
		///		Returns the string if found.
		/// </returns>
		/// <exception cref="MissingManifestResourceException">
		///		If the resource does not exist and throwOnError is true.
		/// </exception>
		public string GetString( string resourceName, bool inherit, bool throwOnError, CultureInfo culture )
		{
			string result = null;
			if( culture == null )
				culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

			if( inherit )
			{
				// Removed since it short circuits inheritenc. Not sure why.
				//				if( _resources[ Assembly.FullName + "." + culture.Name ] != null )
				//				{
				//					ResourceManager manager = _resourceManagers[ _resources[ Assembly.FullName + "." + culture.Name ] ] as ResourceManager;
				//					try
				//					{
				//						result = manager.GetString( resourceName, culture );
				//					}
				//					catch( MissingManifestResourceException ){}
				//				}

				if( result == null )
				{
					StackTrace stack = new StackTrace( 1, false );
					ArrayList tryList = new ArrayList( stack.FrameCount );
					for( int index = 0; index < stack.FrameCount; index++ )
					{
						try
						{
							StackFrame frame	= stack.GetFrame( index );
							if( frame == null )
								continue;
							MethodBase method	= frame.GetMethod();
							if( method == null )
								continue;
							if( method.DeclaringType == null )
								continue;
							Assembly	asm		= method.DeclaringType.Assembly;

							if( asm != Assembly && ! tryList.Contains( asm.GetName().Name ) 
								&& ! asm.FullName.StartsWith( "System" ) 
								&& ! asm.FullName.StartsWith( "mscorlib" ) 
								)
							{
								tryList.Add( asm.GetName().Name );
								result = GetResourceStringFromAssembly( resourceName, asm, culture );
								if( result != null )
								{
									_resources[ Assembly.FullName + "." + culture.Name ] = asm.FullName;
									return result;
								}
							}
						}
						catch( MissingManifestResourceException )
						{
							continue;
						}
					}
				}
			}

			if( result == null )
				try
				{
					result = GetResourceStringFromAssembly( resourceName, Assembly, culture );
					_resources[ Assembly.FullName + "." + culture.Name ] = Assembly.FullName;
				}
				catch( MissingManifestResourceException )
				{
				}
			
			if( result != null )
				return result;

			if( throwOnError )
			{
				if( resourceName != "E_MissingResource" )
					throw new MissingManifestResourceException( FormatResourceString( "E_MissingResource", true, false, culture, resourceName ) );
				else
					throw new MissingManifestResourceException( "Missing resource " + resourceName );
			}

			return resourceName;
		}

        /// <summary>
        /// Detect if an assembly is static
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        internal static bool IsDynamic(Assembly assembly)
        {
            try
            {
                //Previously, Xheo tested for dynamic assmeblies by checking:
                // assembly is System.Reflection.Emit.AssemblyBuilder

                //In .NET 4.0, dynamic assembly is System.Reflection.Emit.InternalAssemblyBuilder,
                //which can't be checked directly because type is declared internal.  For now, check type
                //by looking for invalid operation exceptions on CodeBase property.
                var testVal = assembly.CodeBase;
            }
            catch (NotSupportedException)
            {
                return true;
            }

            return false;
        }

		#region Helpers
		private string GetResourceStringFromAssembly( string resourceName, Assembly assembly, CultureInfo culture )
		{
			if(IsDynamic(assembly) )
				return null;
			ResourceManager manager = _resourceManagers[ assembly.FullName ] as ResourceManager;
			if( manager == null )
			{
				StaticResourcesAttribute sra = Attribute.GetCustomAttribute( assembly, typeof( StaticResourcesAttribute ), true ) as StaticResourcesAttribute;
				string resourcesName = null;
				if( sra == null || ( sra.ResourceName == null || sra.ResourceName.Length == 0 ) && ( sra.BaseResourceName == null || sra.BaseResourceName.Length == 0 ) )
					resourcesName = assembly.GetName().Name + ".Resources." + Assembly.GetName().Name;
				else if( sra.ResourceName != null )
					resourcesName = sra.ResourceName;
				else
					resourcesName = sra.BaseResourceName + "." + Assembly.GetName().Name;

				manager = new ResourceManager( resourcesName, assembly );
				manager.IgnoreCase = true;
				_resourceManagers[ assembly.FullName ] = manager;
			}
			if( culture == null )
				culture = CultureInfo.CurrentUICulture;
			string resource = manager.GetString( resourceName, culture );
			
			//			if( culture != System.Globalization.CultureInfo.InvariantCulture && culture.Parent != System.Globalization.CultureInfo.InvariantCulture )
			//			{
			//				string neutralResult = GetResourceStringFromAssembly( resourceName, assembly, System.Globalization.CultureInfo.InvariantCulture );
			//				if( resource == neutralResult )
			//					return GetResourceStringFromAssembly( resourceName, assembly, culture.Parent );
			//			}
			
			if( resource == null )
				return null;

			return resource;
		}
		#endregion
		#region Overloads

		/// <summary>
		/// Gets a resource string with the given resource name.
		/// </summary>
		/// <param name="resourceName">
		///		Name of the resource that provides the text for the message.
		/// </param>
		/// <param name="throwOnError">
		///		Indicates if an exception is thrown if the resource is not found. When
		///		false and the resource is not found, the resource name will be returned
		///		instead.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <returns>
		///		Returns the string if found otherwise returns resourceName if throwOnError is false.
		/// </returns>
		/// <exception cref="MissingManifestResourceException">
		///		If the resource does not exist and throwOnError is true.
		/// </exception>
		public string GetString( string resourceName, bool inherit, bool throwOnError )
		{
			return GetString( resourceName, inherit, throwOnError, null );
		}

		/// <summary>
		/// Gets a resource string with the given resource name.
		/// </summary>
		/// <param name="resourceName">
		///		Name of the resource that provides the text for the message.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <returns>
		///		Returns the string if found otherwise returns resourceName.
		/// </returns>
		/// <exception cref="MissingManifestResourceException">
		///		If the resource does not exist and throwOnError is true.
		/// </exception>
		public string GetString( string resourceName, bool inherit )
		{
			return GetString( resourceName, inherit, false, null );
		}

		/// <summary>
		/// Gets a resource string with the given resource name.
		/// </summary>
		/// <param name="resourceName">
		///		Name of the resource that provides the text for the message.
		/// </param>
		/// <returns>
		///		Returns the string if found otherwise returns resourceName.
		/// </returns>
		/// <exception cref="MissingManifestResourceException">
		///		If the resource does not exist and throwOnError is true.
		/// </exception>
		public string GetString( string resourceName )
		{
			StaticResourcesAttribute sra = Attribute.GetCustomAttribute( Assembly, typeof( StaticResourcesAttribute ), true ) as StaticResourcesAttribute;
			bool inherit = false;
			if( sra != null )
				inherit = sra.Inherit;
			return GetString( resourceName, inherit, false, null );
		}


		#endregion

		/// <summary>
		/// Formats a resource string with the given resource name.
		/// </summary>
		/// <param name="resourceName">
		///		Name of the resource that provides the text for the message.
		/// </param>
		/// <param name="args">
		///		Parameters to pass to the <see cref="System.String.Format(string,object)"/> method.
		/// </param>
		/// <param name="throwOnError">
		///		Indicates if an exception is thrown if the resource is not found. When
		///		false and the resource is not found, the resource name will be returned
		///		instead.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <param name="culture">
		///		The culture to load the resource for. If null uses the current UI culture.
		/// </param>
		/// <returns>
		///		Returns the formatted message if found.
		/// </returns>
		/// <exception cref="ApplicationException">
		///		If the resource does not exist.
		/// </exception>
		public string FormatResourceString( bool inherit, bool throwOnError, CultureInfo culture, string resourceName, params object[] args )
		{
			if( culture == null )
				culture = CultureInfo.CurrentUICulture;
			string result = GetString( resourceName, inherit, throwOnError, culture );
			if( result != null )
			{
				if( args != null )
				{
					for( int index = args.Length - 1; index >= 0; index-- )
					{
						string arg = args[ index ] as string;
						if( arg != null && arg != resourceName &&
							( arg.StartsWith( "E_" ) || arg.StartsWith( "M_" ) || arg.StartsWith ( "#" ) ) )
						{
							string argresult = GetString( arg, inherit, throwOnError, culture );
							args[ index ] = String.Format( culture, argresult, args );
						}
					}
				}
				return String.Format( culture, result, args );
			}
			return resourceName;
		}
		#region Overloads
		/// <summary>
		/// Formats a resource string with the given resource name.
		/// </summary>
		/// <param name="resourceName">
		///		Name of the resource that provides the text for the message.
		/// </param>
		/// <param name="args">
		///		Parameters to pass to the <see cref="System.String.Format(string,object)"/> method.
		/// </param>
		/// <param name="throwOnError">
		///		Indicates if an exception is thrown if the resource is not found. When
		///		false and the resource is not found, the resource name will be returned
		///		instead.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <returns>
		///		Returns the formatted message if found.
		/// </returns>
		/// <exception cref="ApplicationException">
		///		If the resource does not exist.
		/// </exception>
		public string FormatResourceString( bool inherit, bool throwOnError, string resourceName, params object[] args )
		{
			return FormatResourceString( inherit, throwOnError, null, resourceName, args );
		}

		/// <summary>
		/// Formats a resource string with the given resource name.
		/// </summary>
		/// <param name="resourceName">
		///		Name of the resource that provides the text for the message.
		/// </param>
		/// <param name="args">
		///		Parameters to pass to the <see cref="System.String.Format(string,object)"/> method.
		/// </param>
		/// <param name="inherit">
		///		Indicates if this provider will inherit resources form assemblies higher
		///		in the calling chain.
		/// </param>
		/// <returns>
		///		Returns the formatted message if found.
		/// </returns>
		/// <exception cref="ApplicationException">
		///		If the resource does not exist.
		/// </exception>
		public string FormatResourceString( bool inherit, string resourceName, params object[] args )
		{
			return FormatResourceString( inherit, false, null, resourceName, args );
		}

		/// <summary>
		/// Formats a resource string with the given resource name.
		/// </summary>
		/// <param name="resourceName">
		///		Name of the resource that provides the text for the message.
		/// </param>
		/// <param name="args">
		///		Parameters to pass to the <see cref="System.String.Format(string,object)"/> method.
		/// </param>
		/// <returns>
		///		Returns the formatted message if found.
		/// </returns>
		/// <exception cref="ApplicationException">
		///		If the resource does not exist.
		/// </exception>
		public string FormatResourceString( string resourceName, params object[] args )
		{
			StaticResourcesAttribute sra = Attribute.GetCustomAttribute( Assembly, typeof( StaticResourcesAttribute ), true ) as StaticResourcesAttribute;
			bool inherit = false;
			if( sra != null )
				inherit = sra.Inherit;
			return FormatResourceString( inherit, false, null, resourceName, args );
		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class StaticResourceProvider

	/// <summary>
	/// Declares the static resource information for a given assembly.
	/// </summary>
	[
	AttributeUsage( AttributeTargets.Assembly , AllowMultiple = false )
	]
#if SHARED
	public
#else
	internal
#endif
		sealed class StaticResourcesAttribute : Attribute
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string	_resourceName		= null;
		private string	_baseResourceName	= null;
		private bool	_inherit			= false;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the StaticResourcesAttribute class.
		/// </summary>
		public StaticResourcesAttribute()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the full resource name of the resource file for the static 
		/// resources in the assembly. If set, overrides any value stored in the
		/// <see cref="BaseResourceName"/> property.
		/// </summary>
		public string ResourceName
		{
			get
			{
				return _resourceName;
			}
			set
			{
				_resourceName = value;
			}
		}

		/// <summary>
		/// Gets or sets the base name of the resource file for the static resources in the assembly.
		/// </summary>
		public string BaseResourceName
		{
			get
			{
				return _baseResourceName;
			}
			set
			{
				_baseResourceName = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if resources should be inerited by
		/// default for the current assembly.
		/// </summary>
		public bool Inherit
		{
			get
			{
				return _inherit;
			}
			set
			{
				_inherit = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations
		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class StaticResourcesAttribute
} // End namespace Xheo

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
