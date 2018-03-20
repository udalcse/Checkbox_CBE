using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Ionic.Zip;
using Prezza.Framework.Common;

namespace Checkbox.Common
{
    /// <summary>
    /// A utility class which contains common tasks related to file IO.
    /// </summary>
    public class FileUtilities
    {
        /// <summary>
        /// Defines possible courses of action when a duplicate file is found during an attempt to save.
        /// </summary>
        private enum DuplicateNameAction
        {
            DoNothing,
            Overwrite
        }

        ///<summary>
        /// Creates a new folder.
        ///</summary>
        ///<param name="path">The full path to the folder to create.</param>
        ///<param name="deleteIfExisting">Determines if the content of the folder should be deleted if it already exists.</param>
        ///<exception cref="ArgumentNullException"></exception>
        public static void CreateFolder(string path, bool deleteIfExisting)
        {
            ArgumentValidation.CheckForEmptyString(path, "path");

            if (Directory.Exists(path) && deleteIfExisting)
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Compress the content of a folder in a zip archive.
        /// </summary>
        /// <param name="path">The fully qualified path to the directory to compress.</param>
        /// <param name="archiveName">The name of the compressed archive.</param>
        public static string CompressFolder(string path, string archiveName)
        {
            string name = JoinPathAndFileName(path, archiveName);

            if (File.Exists(name)) { File.Delete(name); }

            using (ZipFile archive = new ZipFile(name))
            {
                archive.AddDirectory(path);
                archive.Save();
            }

            return name;
        }

        /// <summary>
        /// Compress the content of a folder in a zip archive.
        /// </summary>
        /// <param name="path">The fully qualified path to the directory to compress.</param>
        /// <param name="archiveName">The name of the compressed archive.</param>
        public static void CompressFiles(string[] files, string archivePath)
        {
            DeleteFile(archivePath);

            using (ZipFile archive = new ZipFile(archivePath))
            {
                archive.AddDirectoryByName("ExportParts");

                foreach (string file in files)
                {
                    archive.AddFile(file, "ExportParts");
                }

                archive.Save();
            }
        }

        /// <summary>
        /// Creates a new file and returns a <see cref="FileStream"/> which references it.
        /// </summary>
        ///<param name="path">The full path to the folder to create.</param>
        ///<param name="fileName">The file name, including extension, to save.</param>
        /// <returns></returns>
        public static FileStream GetFileStream(string path, string fileName)
        {
            ArgumentValidation.CheckForEmptyString(path, "path");
            ArgumentValidation.CheckForEmptyString(path, "fileName");
            
            string nameAndPath = JoinPathAndFileName(path, fileName, true);

            //duplicate files are overwritten.
            return File.Create(nameAndPath);
        }

        ///<summary>
        /// Saves a file to disk.
        /// If the file already exists no action is taken.
        ///</summary>
        ///<param name="path">The full path to the folder to create.</param>
        ///<param name="fileName">The file name, including extension, to save.</param>
        ///<param name="data">The data to be saved to disk.</param>
        ///<exception cref="ArgumentNullException"></exception>
        public static void SaveFile(string path, string fileName, byte[] data)
        {
            SaveFile(path, fileName, data, (int)DuplicateNameAction.Overwrite);
        }

        ///<summary>
        /// Saves a file to disk.
        /// If the file already exists no action is taken.
        ///</summary>
        ///<param name="path">The full path to the folder to create.</param>
        ///<param name="data">The data to be saved to disk.</param>
        ///<exception cref="ArgumentNullException"></exception>
        public static void SaveFile(string path, byte[] data)
        {
            SaveFile(path, data, (int)DuplicateNameAction.Overwrite);
        }

        ///<summary>
        /// Saves a file to disk.
        /// If the file already exists the course of action is controlled by the duplicateNameAction parameter.
        ///</summary>
        ///<param name="path">The full path to the folder to create.</param>
        ///<param name="fileName">The file name, including extension, to save.</param>
        ///<param name="data">The data to be saved to disk.</param>
        ///<param name="duplicateNameAction" cref="DuplicateNameAction">Determines the action to take when the specified file name is already in use.</param>
        ///<exception cref="ArgumentNullException"></exception>
        public static void SaveFile(string path, string fileName, byte[] data, int duplicateNameAction)
        {
            string nameAndPath = JoinPathAndFileName(path, fileName, true);

            SaveFile(nameAndPath, data, duplicateNameAction);
        }

        private static void SaveFile(string path, byte[] data, int duplicateNameAction)
        {
            ArgumentValidation.CheckForEmptyString(path, "path");
            ArgumentValidation.CheckForNullReference(data, "data");

            if (File.Exists(path) && duplicateNameAction == (int)DuplicateNameAction.DoNothing)
                return;


            FileStream fileStream = File.Create(path);
            BinaryWriter binaryWriter;

            using (binaryWriter = new BinaryWriter(fileStream))
            {
                binaryWriter.Write(data);
                binaryWriter.Close();
            }

            fileStream.Close();
        }

        ///<summary>
        /// Deletes a file from disk.
        ///</summary>
        ///<param name="path">The full path to the folder to delete.</param>
        ///<exception cref="ArgumentNullException"></exception>
        public static void DeleteFile(string path)
        {
            ArgumentValidation.CheckForEmptyString(path, "path");

            if (!File.Exists(path))
                return;

            File.Delete(path);
        }

        /// <summary>
        /// Delete files in with created dates earlier than current time minus timespan.
        /// </summary>
        /// <param name="folderPath">Folder to look (non-recursively) for files in.</param>
        /// <param name="timeSpan">Min age of files to delete.</param>
        /// <param name="rethrowExceptions">Rethrow exceptions or silently ignore them.</param>
        /// <param name="extensions">Extensions to check for, do not include .period separator.</param>
        public static void DeleteFilesOlderThanTimeSpan(string folderPath, TimeSpan timeSpan, bool rethrowExceptions, params string[] extensions)
        {
            ArgumentValidation.CheckForEmptyString(folderPath, "folderPath");

            try
            {
                if (Directory.Exists(folderPath))
                {
                    DateTime now = DateTime.Now;

                    foreach (string extension in extensions)
                    {
                        string[] files = Directory.GetFiles(folderPath, "*." + extension, SearchOption.TopDirectoryOnly);
                        
                        foreach (string file in files)
                        {
                            DateTime creationTime = File.GetCreationTime(file);

                            if (creationTime < (now - timeSpan))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete all files in a folder where the folder is older than a given time span.  This method does
        /// not actually delete the folder, since doing so could cause ASP.NET app to recycle.
        /// </summary>
        /// <param name="folderPath">Folder to look (non-recursively) for files in.</param>
        /// <param name="timeSpan">Min age of files to delete.</param>
        /// <param name="searchSpec">Search fileter for files</param>
        /// <param name="rethrowExceptions">Rethrow exceptions or silently ignore them.</param>
        public static void DeleteFilesInFolderOlderThanTimeSpan(string folderPath, TimeSpan timeSpan, string searchSpec, bool rethrowExceptions)
        {
            //Do nothing of no folder or search spec specified.
            if (Utilities.IsNullOrEmpty(folderPath) || Utilities.IsNullOrEmpty(searchSpec))
            {
                return;
            }

            try
            {
                if (Directory.Exists(folderPath))
                {
                    DateTime now = DateTime.Now;

                    string[] folders = Directory.GetDirectories(folderPath, "*"  + searchSpec + "*");

                    foreach (string folder in folders)
                    {
                        DateTime creationTime = Directory.GetCreationTime(folder);

                        if (creationTime < (now - timeSpan))
                        {
                            string[] files = Directory.GetFiles(folder);

                            foreach (string file in files)
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Ensures that a file name is valid in Windows based systems.
        /// A valid file name is 160 characters less in length, ASCII,
        /// and does not contain ">", "&#60;", "|", @"", @"\", "/", ":", "*", "?".
        /// </summary>
        ///<param name="fileName">The file name, including extension, to save.</param>
        /// <returns></returns>
        ///<exception cref="ArgumentNullException"></exception>
        public static string SanitizeName(string fileName)
        {
            ArgumentValidation.CheckForEmptyString(fileName, "fileName");

            string[] invalidCharacters = { "<", ">", "|", "\"", @"\", "/", ":", "*", "?" };

            byte[] fileNameBytes = Encoding.ASCII.GetBytes(fileName.ToCharArray());
            string fileNameASCII = Encoding.ASCII.GetString(fileNameBytes);

            foreach (string @invalid in invalidCharacters)
            {
                fileNameASCII = fileNameASCII.Replace(invalid, string.Empty);
            }

            if (fileNameASCII.Length > 160)
                fileNameASCII = fileNameASCII.Substring(0, 159);

            return fileNameASCII;
        }

        /// <summary>
        /// Returns a properly formatted concatenation of a path and file name string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns></returns>
        public static string JoinPathAndFileName(string path, string fileName)
        {
            return JoinPathAndFileName(path, fileName, false);
        }

        /// <summary>
        /// Returns a properly formatted concatenation of a path and file name string.
        /// Additionally the fileName can be validated to ensure it does not contain
        /// characters which are not legal in Windows.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="sanitizeFileName">Determines if the fileName is validated.</param>
        /// <returns></returns>
        public static string JoinPathAndFileName(string path, string fileName, bool sanitizeFileName)
        {
            string spacer = path.EndsWith("/") ? string.Empty : "/";

            if (sanitizeFileName)
            {
                fileName = SanitizeName(fileName);
            }
            path = path.Replace(@"\", "/");
            string nameAndPath = string.Format("{0}{1}{2}", path, spacer, fileName);

            return nameAndPath;
        }

        /// <summary>
        /// Returns the content of a text file as a string. If the file can not be found/read an empty string is returned.
        /// </summary>
        /// <param name="fileAndPath">The fully qualified path to the text file to read.</param>
        /// <returns></returns>
        public static string ReadTextFile(string fileAndPath)
        {
            StringBuilder sb = new StringBuilder();

            if (File.Exists(fileAndPath))
            {
                StreamReader file = null;
                try
                {
                    using (file = File.OpenText(fileAndPath))
                    {
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            sb.AppendLine(line);
                        }

                        file.Close();
                    }
                }
                catch (Exception)
                {
                    if (file != null) { file.Close(); }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Iterates through a StreamReader and constructs a list of strings from the data.
        /// </summary>
        /// <param name="reader">The StreamReader to iterate through.</param>
        /// <returns>A List which contains one element for each row contained in the specified StreamReader.</returns>
        public static List<string> ReadTextStream(StreamReader reader)
        {
            List<string> data = new List<string>();

            if (reader == null)
            {
                return data;
            }

            using (reader)
            {
                while (reader.Peek() >= 0)
                {
                    data.Add(reader.ReadLine());
                }

            }

            return data;
        }


        /// <summary>
        /// Ensures that the specified directory can be written to.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool ValidateWritableDirectory(string directory)
        {
            if (Utilities.IsNullOrEmpty(directory)) return false;

            byte[] testData = { 0, 1, 2 };
            string testFileName = directory.EndsWith("\\") || directory.EndsWith("/")
                ? string.Format("{0}_AccessValidation.txt", DateTime.Now.Ticks)
                : string.Format("/{0}_AccessValidation.txt", DateTime.Now.Ticks);

            string fullPath = JoinPathAndFileName(directory, testFileName);

            try
            {
                SaveFile(directory, testFileName, testData);
                DeleteFile(fullPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="invalidCharReplacement"> </param>
        /// <returns></returns>
        public static string FixFileName(string fileName, string invalidCharReplacement = null)
        {
            return FixFileName(fileName, invalidCharReplacement, invalidCharReplacement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="invalidCharReplacement"> </param>
        /// <param name="whiteSpaceReplacement"> </param>
        /// <returns></returns>
        public static string FixFileName(string fileName, string invalidCharReplacement, string whiteSpaceReplacement)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            fileName = Utilities.AdvancedHtmlDecode(fileName);

            if (!string.IsNullOrEmpty(whiteSpaceReplacement))
                fileName.Replace(" ", whiteSpaceReplacement);

            fileName = Utilities.StripHtml(fileName);

            return SanitizeFileName(fileName, invalidCharReplacement);
        }

        /// <summary>
        /// Remove invalid characters from a file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="invalidCharReplacement"></param>
        /// <returns></returns>
        public static string SanitizeFileName(string fileName, string invalidCharReplacement)
        {
            if (string.IsNullOrEmpty(invalidCharReplacement))
                invalidCharReplacement = string.Empty;

            var allowed = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '.' };
            var chars = fileName.ToCharArray(0, fileName.Length);

            var friendly = string.Empty;

            foreach (var c in chars)
            {
                var characterAllowed = false;
                foreach (var b in allowed)
                {
                    if (b != c)
                    {
                        continue;
                    }

                    friendly += c.ToString();
                    characterAllowed = true;
                }
                if (!characterAllowed)
                {
                    friendly += invalidCharReplacement;
                }
            }

            return friendly;
        }

    }
}
