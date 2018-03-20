using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class RedirectItemData : LocalizableResponseItemData
    {
        /// <summary>
        /// Get name of data table
        /// </summary>
        public override string ItemDataTableName { get { return "RedirectItemData"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetRedirect"; } }

        /// <summary>
        /// Get/set whether to redirect automatically or to display a link
        /// </summary>
        public virtual bool RedirectAutomatically { get; set; }

        /// <summary>
        /// Get/set whether to redrect automatically or to display a link
        /// </summary>
        public virtual int? AutoRedirectDelayTime { get; set; }

        /// <summary>
        /// Restart the survey
        /// </summary>
        public virtual bool RestartSurvey { get; set; }

        /// <summary>
        /// Get/set whether to open in the same window or tab
        /// </summary>
        public virtual bool OpenInNewWindow { get; set; }

        /// <summary>
        /// Get/set the URL to redirect to
        /// </summary>
        public virtual string URL { get; set; }

        /// <summary>
        /// Get/set the URL text to display
        /// </summary>
        public virtual string URLTextID
        {
            get { return GetTextID("urlText"); }
        }

        /// <summary>
        /// Load item data from the specified data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            URL = DbUtility.GetValueFromDataRow(data, "Url", string.Empty);
            RedirectAutomatically = DbUtility.GetValueFromDataRow(data, "AutoRedirect", false);
            AutoRedirectDelayTime = DbUtility.GetValueFromDataRow(data, "AutoRedirectDelayTime", 0);
            RestartSurvey = DbUtility.GetValueFromDataRow(data, "RestartSurvey", false);
            OpenInNewWindow = DbUtility.GetValueFromDataRow(data, "OpenInNewWindow", false);
        }

        /// <summary>
        /// Create an instance of the <see cref="ItemData"/> in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.  Unable to insert data for redirect item.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertRedirect");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Url", DbType.String, URL);
            command.AddInParameter("UrlTextID", DbType.String, URLTextID);
            command.AddInParameter("AutoRedirect", DbType.Boolean, RedirectAutomatically);
            command.AddInParameter("AutoRedirectDelayTime", DbType.Int32, AutoRedirectDelayTime);
            command.AddInParameter("RestartSurvey", DbType.Boolean, RestartSurvey);
            command.AddInParameter("OpenInNewWindow", DbType.Boolean, OpenInNewWindow);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an instance of the <see cref="ItemData"/> in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.  Unable to update data for redirect item.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateRedirect");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Url", DbType.String, URL);
            command.AddInParameter("UrlTextID", DbType.String, URLTextID);
            command.AddInParameter("AutoRedirect", DbType.Boolean, RedirectAutomatically);
            command.AddInParameter("AutoRedirectDelayTime", DbType.Int32, AutoRedirectDelayTime);
            command.AddInParameter("RestartSurvey", DbType.Boolean, RestartSurvey);
            command.AddInParameter("OpenInNewWindow", DbType.Boolean, OpenInNewWindow);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string[] GetTextIdSuffixes()
        {
            return new List<string>(base.GetTextIdSuffixes()) { "urlText" }.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("AutoRedirect", RedirectAutomatically.ToString());
            writer.WriteElementString("AutoRedirectDelayTime", AutoRedirectDelayTime.ToString());
            writer.WriteElementString("RestartSurvey", RestartSurvey.ToString());
            writer.WriteElementString("OpenInNewWindow", OpenInNewWindow.ToString());
            writer.WriteElementString("URL", URL);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

			RedirectAutomatically = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("AutoRedirect"));
            AutoRedirectDelayTime = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("AutoRedirectDelayTime"));
            RestartSurvey = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("RestartSurvey"));
            OpenInNewWindow = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("OpenInNewWindow"));
            URL = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("URL"));
		}

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new RedirectItem();
        }

        /// <summary>
        /// Get a text decorator for the <see cref="RedirectItem"/>
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new RedirectItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto); 
            
            if (itemDto is ItemMetaData)
            {
                var textDataList = ((ItemMetaData)itemDto).TextData;

                foreach (var textData in textDataList)
                {
                    textData.TextValues["NavText"] = 
                        RestartSurvey
                            ? TextManager.GetText(
                                "/controlText/redirectItemRenderer/restartSurvey",
                                TextManager.DefaultLanguage)
                            : Utilities.TruncateText(URL, 50);
                }
            }
        }
    }
}