//===============================================================================
// Prezza Technologies Application Framework
// Copyright � Prezza Technologies, Inc.  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, � 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using Prezza.Framework.Common;

namespace Prezza.Framework.Data.Configuration
{
    /// <summary>
    /// <para>Represents a collection of <see cref="OraclePackageData"/>s for the <see cref="ConnectionString"/> in configuration for the block.</para>
    /// </summary>   
    [Serializable]
    public class OraclePackageDataCollection : DataCollection
    {
        /// <summary>
        /// <para>Gets or sets the <see cref="OraclePackageData"/> at the specified <paramref name="index"/>.</para>
        /// </summary>
        /// <param name="index">
        /// <para>The index of the <see cref="OraclePackageData"/> to get or set.</para>
        /// </param>
        /// <value>
        /// <para>The value associated with the specified <paramref name="index"/>. If the specified <paramref name="index"/> is not found, attempting to get it returns a <see langword="null"/> reference (Nothing in Visual Basic), and attempting to set it creates a new entry using the specified <paramref name="index"/>.</para>
        /// </value>
        public OraclePackageData this[int index]
        {
            get { return (OraclePackageData)BaseGet(index); }
            set { BaseSet(index, value); }
        }

        /// <summary>
        /// <para>Gets or sets the <see cref="OraclePackageData"/> associated with the specified <paramref name="name"/>.</para>
        /// </summary>
        /// <param name="name">
        /// <para>The name of the <see cref="OraclePackageData"/> to get or set.</para>
        /// </param>
        /// <value>
        /// <para>The value associated with the specified <paramref name="name"/>. If the specified <paramref name="name"/> is not found, attempting to get it returns a <see langword="null"/> reference (Nothing in Visual Basic), and attempting to set it creates a new entry using the specified <paramref name="name"/>.</para>
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is a <see langword="null"/> reference (Nothing in Visual Basic).</para>
        /// </exception>
        public OraclePackageData this[string name]
        {
            get
            {
                ArgumentValidation.CheckForNullReference(name, "name");

                return BaseGet(name) as OraclePackageData;
            }
            set
            {
                ArgumentValidation.CheckForNullReference(name, "name");

                BaseSet(name, value);
            }
        }

        /// <summary>
        /// <para>Adds an <see cref="OraclePackageData"/> into the collection.</para>
        /// </summary>
        /// <param name="oraclePackageData">
        /// <para>The <see cref="OraclePackageData"/> to add. The value can not be a <see langword="null"/> reference (Nothing in Visual Basic).</para>
        /// </param>
        /// <remarks>
        /// <para>If a reference already exists in the collection by <seealso cref="OraclePackageData.Name"/>, it will be replaced with the new reference.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="oraclePackageData"/> is a <see langword="null"/> reference (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para><seealso cref="OraclePackageData.Name"/> is a <see langword="null"/> reference (Nothing in Visual Basic).</para>
        /// </exception>
        public void Add(OraclePackageData oraclePackageData)
        {
            ArgumentValidation.CheckForNullReference(oraclePackageData, "oraclePackageData");
            ArgumentValidation.CheckForInvalidNullNameReference(oraclePackageData.Name, typeof(OraclePackageData).FullName);

            BaseAdd(oraclePackageData.Name, oraclePackageData);
        }

        /// <summary>
        /// <para>Adds a value into the collection.</para>
        /// </summary>
        /// <param name="parameter">
        /// <para>The value to add. The value can not be a <see langword="null"/> reference (Nothing in Visual Basic).</para>
        /// </param>
        /// <remarks>
        /// <para>This method exists to support Xml Serialization.</para>
        /// </remarks>
        /// <exception cref="InvalidCastException">
        /// <para><paramref name="parameter"/> must be of type <see cref="OraclePackageData"/>.</para>
        /// </exception>
        public void Add(object parameter)
        {
            Add((OraclePackageData)parameter);
        }

        /// <summary>
        /// <para>Copies the entire <see cref="OraclePackageDataCollection"/> to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</para>
        /// </summary>
        /// <param name="array">
        /// <para>The one-dimensional <see cref="OraclePackageDataCollection"/> array that is the destination of the elements copied from <see cref="OraclePackageDataCollection"/>. The <see cref="OraclePackageData"/> array must have zero-based indexing.</para>
        /// </param>
        /// <param name="index">
        /// <para>The zero-based index in array at which copying begins.</para>
        /// </param>
        public void CopyTo(OraclePackageData[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}