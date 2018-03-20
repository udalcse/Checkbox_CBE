/****************************************************************************
 * InstallFileInfo.cs												        *
 * Container for information about install/upgrade/patch files.			    *
 ****************************************************************************/
using System;
using System.Collections.Generic;

namespace Checkbox.Configuration.Install
{
    /// <summary>
    /// Container for information about files to copy, etc.
    /// </summary>
    [Serializable()]
    public class InstallFileInfo
    {
        private string _sourcePath;
        private string _destinationPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourcePath">Source path of the file(s) to copy.</param>
        /// <param name="destinationPath">Destination path of the file(s) to copy.</param>
        internal InstallFileInfo(string sourcePath, string destinationPath)
        {
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
        }

        /// <summary>
        /// Source Path of the file(s) to copy.
        /// </summary>
        public string SourcePath
        {
            get { return _sourcePath; }
        }

        /// <summary>
        /// Destination path of the file(s) to copy.
        /// </summary>
        public string DestinationPath
        {
            get { return _destinationPath; }
        }
    }
}
