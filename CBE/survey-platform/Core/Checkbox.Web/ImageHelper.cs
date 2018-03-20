//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.IO;
using System.Web;
using System.Net;
using System.Data;

using Prezza.Framework.Data;

namespace Checkbox.Web
{
	/// <summary>
	/// Static image-related utilities
	/// </summary>
	public class ImageHelper
	{
		private ImageHelper()
		{
		}

		/// <summary>
		/// Get the binary data for a stored image.
		/// </summary>
        /// <param name="imageID">ID of image to get data for.</param>
		/// <returns>Byte array with binary data, null if data could not be obtained.</returns>
		public static byte[] GetImageData(int imageID)
		{
			//Attempt to get the data for the image
			byte[] imgData = null;

            using (IDataReader imgReader = DbUtility.GetImage(imageID))
            {
                try
                {
                    if (imgReader.Read())
                    {
                        if (imgReader["ImageData"] != DBNull.Value)
                        {
                            imgData = (byte[])imgReader["ImageData"];
                        }
                        else if (imgReader["ImageUrl"] != DBNull.Value)
                        {
                            try
                            {
                                //Attempt to get the image from disk
                                string imageUrl = (string)imgReader["ImageUrl"];

                                //Len(http://) = 7
                                if (imageUrl.Length > 7)
                                {
                                    string filePath;

                                    try
                                    {
                                        //Now get rid of the server name in the path
                                        filePath = imageUrl.Substring(7);
                                        
                                        if (filePath.IndexOf("/") >= 0)
                                        {
                                            filePath = filePath.Substring(filePath.IndexOf("/"));
                                            filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
                                        }
                                        else
                                        {
                                            filePath = string.Empty;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        filePath = string.Empty;
                                    }

                                    if (filePath != string.Empty)
                                    {
                                        if (File.Exists(filePath))
                                        {
                                            try
                                            {
                                                BinaryReader reader = new BinaryReader(File.OpenRead(filePath));
                                                imgData = new byte[reader.BaseStream.Length];
                                                reader.Read(imgData, 0, imgData.Length);
                                            }
                                            catch (Exception)
                                            {
                                                imgData = null;
                                            }
                                        }
                                    }

                                    //If we couldn't find the file on disk, attempt a URL download
                                    if (imgData == null)
                                    {
                                        //Attempt to download the data
                                        try
                                        {
                                            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create((string)imgReader["ImageUrl"]);

                                            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                                            Stream stream = webResponse.GetResponseStream();

                                            imgData = new byte[webResponse.ContentLength];
                                            stream.Read(imgData, 0, imgData.Length);
                                            stream.Close();
                                            webResponse.Close();
                                        }
                                        catch (Exception)
                                        {
                                            imgData = null;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                imgData = null;
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    imgReader.Close();
                }
            }

			return imgData;
		}

	    /// <summary>
	    /// Get the binary data for a stored image.
	    /// </summary>
	    /// <param name="imageID">ID of image to get data for.</param>
	    /// <param name="contentType"></param>
	    /// <returns>Byte array with binary data, null if data could not be obtained.</returns>
	    public static byte[] GetImageData(int imageID, out String contentType)
        {
            //Attempt to get the data for the image
            byte[] imgData = null;
	        contentType = string.Empty;

            using (IDataReader imgReader = DbUtility.GetImage(imageID))
            {
                try
                {
                    if (imgReader.Read())
                    {
                        if (imgReader["ImageData"] != DBNull.Value)
                        {
                            imgData = (byte[])imgReader["ImageData"];
                            contentType = imgReader["ContentType"] != DBNull.Value ? imgReader["ContentType"].ToString() : "";
                        }
                        else if (imgReader["ImageUrl"] != DBNull.Value)
                        {
                            try
                            {
                                //Attempt to get the image from disk
                                string imageUrl = (string)imgReader["ImageUrl"];

                                //Len(http://) = 7
                                if (imageUrl.Length > 7)
                                {
                                    string filePath;

                                    try
                                    {
                                        //Now get rid of the server name in the path
                                        filePath = imageUrl.Substring(7);

                                        if (filePath.IndexOf("/") >= 0)
                                        {
                                            filePath = filePath.Substring(filePath.IndexOf("/"));
                                            filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
                                        }
                                        else
                                        {
                                            filePath = string.Empty;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        filePath = string.Empty;
                                    }

                                    if (filePath != string.Empty)
                                    {
                                        if (File.Exists(filePath))
                                        {
                                            try
                                            {
                                                BinaryReader reader = new BinaryReader(File.OpenRead(filePath));
                                                imgData = new byte[reader.BaseStream.Length];
                                                reader.Read(imgData, 0, imgData.Length);
                                                contentType = GetContentType(Path.GetFileName(filePath));
                                            }
                                            catch (Exception)
                                            {
                                                imgData = null;
                                            }
                                        }
                                    }

                                    //If we couldn't find the file on disk, attempt a URL download
                                    if (imgData == null)
                                    {
                                        //Attempt to download the data
                                        try
                                        {
                                            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create((string)imgReader["ImageUrl"]);

                                            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                                            contentType = webResponse.ContentType;

                                            Stream stream = webResponse.GetResponseStream();

                                            imgData = new byte[webResponse.ContentLength];
                                            stream.Read(imgData, 0, imgData.Length);
                                            stream.Close();
                                            webResponse.Close();
                                        }
                                        catch (Exception)
                                        {
                                            imgData = null;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                imgData = null;
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    imgReader.Close();
                }
            }

            return imgData;
        }

        /// <summary>
        /// Get content type of the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetContentType(string fileName)
        {
            string result = "application/octetstream";
            string extension = Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                result = regKey.GetValue("Content Type").ToString();
            return result;
        }
	}
}
