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
// Class:		StringCollectionEditor
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
	public class StringCollectionEditor : CollectionEditor
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the StringCollectionEditor class.
		/// </summary>
		public StringCollectionEditor() : base( typeof( StringValueCollection ) )
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

			if( value is StringCollection )
			{
				IDesignerHost designer = GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				StringValueCollection newValue = new StringValueCollection( value as StringCollection );
				base.EditValue( context, provider, newValue );
				newValue.DuplicateTo( value as StringCollection );
				return newValue;
			}
			else
			{
				return base.EditValue( context, provider, value );
			}

		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class StringCollectionEditor

	/// <summary>
	/// Helper type for editing strings.
	/// </summary>
	internal class StringValue
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		string _value		= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///  Initializes a new instance of the StringValue class.
		/// </summary>
		public StringValue()
		{
		}

		/// <summary>
		///  Initializes a new instance of the StringValue class from a string.
		/// </summary>
		/// <param name="value">
		///		The value of the string.
		/// </param>
		public StringValue( string value )
		{
			_value = value;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the value of the string.
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
			return Value;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class StringValue

		
	/// <summary>
	/// Implements a strongly typed collection of StringValue objects.
	/// </summary>
	internal class StringValueCollection : CollectionBase
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///  Initializes a new instance of the StringValueCollection class.
		/// </summary>
		public StringValueCollection()
		{
		}

		/// <summary>
		///  Initializes a new instance of the StringValueCollection class from and existing
		///  collection.
		/// </summary>
		public StringValueCollection( StringCollection values )
		{
			foreach( string value in values )
				Add( new StringValue( value ) );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the StringValue at the given index.
		/// </summary>
		public StringValue this[ int index ]
		{
			get
			{
				return List[ index ] as StringValue;
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
		/// Adds a new StringValue to the collection.
		/// </summary>
		/// <param name="value">
		///		The value to add.
		/// </param>
		/// <returns>
		///		Returns the index of the value.
		/// </returns>
		public int Add( StringValue value )
		{
			return List.Add( value );
		}

		/// <summary>
		/// Removes the StringValue from the collection.
		/// </summary>
		/// <param name="value">
		///		The value to remove.
		/// </param>
		public void Remove( StringValue value )
		{
			List.Remove( value );
		}

		/// <summary>
		/// Converts the values to a string collection.
		/// </summary>
		/// <returns>
		///		Returns the collection as a StringCollection.
		/// </returns>
		public StringCollection ToCollection()
		{
			StringCollection values = new StringCollection();
			foreach( StringValue value in this )
				values.Add( value.Value );

			return values;
		}

		/// <summary>
		/// Duplicates the collection of values to the given collection.
		/// </summary>
		/// <param name="collection">
		///		The collection to duplicate values to.
		/// </param>
		public void DuplicateTo( StringCollection collection )
		{
			collection.Clear();
			foreach( StringValue value in this )
				collection.Add( value.Value );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class StringValueCollection


} // End namespace Xheo.Licensing.Design

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
