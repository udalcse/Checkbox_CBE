using System;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using Prezza.Framework.Data;

namespace Checkbox.Web.Charts
{
    ///<summary>
    ///</summary>
    public class ChartImageHandler : IChartStorageHandler 
    {

        #region IChartStorageHandler Members

        /// <summary>
        /// Delete an image from the database
        /// </summary>
        /// <param name="key"></param>
        public void Delete(string key)
        {
            List<int> imageIds = DbUtility.FindImage("CHART::" + key, true);

            foreach (int imageId in imageIds)
            {
                DbUtility.DeleteImage(imageId);
            }
        }

        /// <summary>
        /// Return a boolean indicating if a chart image with the specified key exists in the DB.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return DbUtility.FindImage("CHART::" + key, true).Count > 0;
        }

        /// <summary>
        /// Get image data from the database
        /// </summary>
        /// <param name="key"></param>
        public byte[] Load(string key)
        {
            List<int> imageIds = DbUtility.FindImage("CHART::" + key, true);

            return imageIds.Count > 0 ? ImageHelper.GetImageData(imageIds[0]) : new byte[0];
        }

        /// <summary>
        /// Store image data in the database
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Save(string key, byte[] data)
        {
            //Clear out temp images
            DbUtility.DeleteTempImages();

            //Delete any images with the same name
            Delete(key);

            //Save the chart image
            DbUtility.SaveImage(data, string.Empty, string.Empty, "CHART::" + key, string.Empty, DateTime.Now, true);
        }

        #endregion
    }
}
