//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace Prezza.Framework.Data
{
    /// <summary>
    /// Used with the Database.UpdateDataSet method.  Provides control over behavior when the Data
    /// Adapter's update command encounters an error.
    /// </summary>
    public enum UpdateBehavior
    {
        /// <summary>
        /// No interference with the DataAdapter's Update command.  If Update encounters
        /// an error, the update stops.  Additional rows in the Datatable are uneffected.
        /// </summary>
        Standard,
        /// <summary>
        /// If the DataAdapter's Update command encounters an error, the update will
        /// continue.  The Update command will try to update the remaining rows. 
        /// </summary>
        Continue,
        /// <summary>
        /// If the DataAdapter encounters an error, all updated rows will be rolled back
        /// </summary>
        Transactional
    }
}
