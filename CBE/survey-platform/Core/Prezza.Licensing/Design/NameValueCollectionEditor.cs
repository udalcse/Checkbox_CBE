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
// Class:		NameValueCollectionEditor
// Author:		Paul Alexander
// Created:		Saturday, November 16, 2002 11:04:53 AM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Specialized;

namespace Xheo.Licensing.Design
{
	/// <summary>
	/// Implements an Editor for string collections
	/// </summary>
	public class NameValueCollectionEditor : CollectionEditor
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the NameValueCollectionEditor class.
		/// </summary>
		public NameValueCollectionEditor() : base( typeof( NameValueEntryCollection ) )
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		///<summary>
		///Summary of EditValue.
		///</summary>
		///<param name="context"></param>
		///<param name="provider"></param>
		///<param name="value"></param>
		///<returns></returns>
		public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
		{
			if( value is NameValueCollection )
			{
				NameValueEntryCollection newValue = new NameValueEntryCollection( value as NameValueCollection );
				base.EditValue( context, provider, newValue );
				newValue.DuplicateTo( value as NameValueCollection );
				return value;
			}
			else
			{
				return base.EditValue( context, provider, value );
			}

		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class NameValueCollectionEditor

	/// <summary>
	/// Helper type for editing strings.
	/// </summary>
	internal class NameValueEntry
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string _value		= null;
		private string _key			= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///  Initializes a new instance of the NameValueEntry class.
		/// </summary>
		public NameValueEntry()
		{
		}

		/// <summary>
		///  Initializes a new instance of the NameValueEntry class from a string.
		/// </summary>
		/// <param name="value">
		///		The value of the string.
		/// </param>
		/// <param name="key">
		///		The key of the entry.
		/// </param>
		public NameValueEntry( string key, string value )
		{
			_key	= key;
			_value	= value;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the value of the entry.
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Gets or sets the key of the entry;
		/// </summary>
		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		///<summary>
		///Summary of ToString.
		///</summary>
		///<returns></returns>	
		public override string ToString()
		{
			return Key;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class NameValueEntry

		
	/// <summary>
	/// Implements a strongly typed collection of NameValueEntry objects.
	/// </summary>
	internal class NameValueEntryCollection : CollectionBase
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///  Initializes a new instance of the NameValueEntryCollection class.
		/// </summary>
		public NameValueEntryCollection()
		{
		}

		/// <summary>
		///  Initializes a new instance of the NameValueEntryCollection class from and existing
		///  collection.
		/// </summary>
		public NameValueEntryCollection( NameValueCollection values )
		{
			foreach( string key in values.Keys )
				if( key != null )
				foreach( string value in values.GetValues( key ) )
					Add( new NameValueEntry( key, value ) );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the NameValueEntry at the given index.
		/// </summary>
		public NameValueEntry this[ int index ]
		{
			get
			{
				return List[ index ] as NameValueEntry;
			}
			set
			{
				List[ index ] = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Adds a new NameValueEntry to the collection.
		/// </summary>
		/// <param name="value">
		///		The value to add.
		/// </param>
		/// <returns>
		///		Returns the index of the value.
		/// </returns>
		public int Add( NameValueEntry value )
		{
			return List.Add( value );
		}

		/// <summary>
		/// Removes the NameValueEntry from the collection.
		/// </summary>
		/// <param name="value">
		///		The value to remove.
		/// </param>
		public void Remove( NameValueEntry value )
		{
			List.Remove( value );
		}

		/// <summary>
		/// Duplicates the collection of values to the given collection.
		/// </summary>
		/// <param name="collection">
		///		The collection to duplicate values to.
		/// </param>
		public void DuplicateTo( NameValueCollection collection )
		{
			collection.Clear();
			foreach( NameValueEntry value in this )
				if( value.Key != null && value.Value != null )
				collection.Add( value.Key, value.Value );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class NameValueEntryCollection
} // End namespace Xheo.Licensing.Design

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
