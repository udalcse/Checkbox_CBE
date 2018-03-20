using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class MessagesUpdatedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int OldId { set; get; }
        
        /// <summary>
        /// 
        /// </summary>
        public int NewId { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessagesDeletedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MessagesUpdated(object sender, MessagesUpdatedEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MessagesDeleted(object sender, MessagesDeletedEventArgs e);

    /// <summary>
    /// Scoring message
    /// </summary>
    [Serializable]
    public class ScoreMessageItemData : MessageItemData
    {
        private ScoreMessageDictionary _messageDictionary;
        private List<int> _deletedMessages;

		/// <summary>
		/// 
		/// </summary>
        protected const string MessageTableName = "Messages";

        /// <summary>
        /// 
        /// </summary>
        public event MessagesUpdated MessagesUpdated;

        /// <summary>
        /// 
        /// </summary>
        public event MessagesDeleted MessagesDeleted;

        /// <summary>
        /// Get/set id of page to print score of.  If value is null, all items in survey before
        /// score item are used.
        /// </summary>
        public int? PageId { get; set; }

        /// <summary>
        /// Get sorted list of messages
        /// </summary>
        public List<ScoreMessage> Messages
        {
            get
            {
                return new List<ScoreMessage>(MessageDictionary.Values.OrderBy(msg => msg.LowScore));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<int> DeletedMessages
        {
            get { return _deletedMessages ?? (_deletedMessages = new List<int>()); }
        }

        /// <summary>
        /// Get message dictionary
        /// </summary>
        protected ScoreMessageDictionary MessageDictionary
        {
            get { return _messageDictionary ?? (_messageDictionary = new ScoreMessageDictionary()); }
        }

        /// <summary>
        /// Get the score message text id
        /// </summary>
        /// <param name="scoreMessageId"></param>
        /// <returns></returns>
        public string GetMessageTextId(Int32 scoreMessageId)
        {
            if (scoreMessageId <= 0)
            {
                return string.Empty;
            }

            return GetTextID(scoreMessageId.ToString());
        }

        /// <summary>
        /// Add a message
        /// </summary>
        /// <param name="lowScore"></param>
        /// <param name="highScore"></param>
        public int AddScoreRange(double? lowScore, double? highScore)
        {
            //Create a message object
            var msg = new ScoreMessage
            {
                LowScore = lowScore,
                HighScore = highScore,
                MessageId = MessageDictionary.GetNextTempMessageId(),
            };

            MessageDictionary[msg.MessageId] = msg;

            return msg.MessageId;
        }

        /// <summary>
        /// Remove a message
        /// </summary>
        /// <param name="scoreMessageId"></param>
        public void RemoveScoreRange(int scoreMessageId)
        {
            if (MessageDictionary.ContainsKey(scoreMessageId))
            {
                MessageDictionary.Remove(scoreMessageId);
            }

            if (!DeletedMessages.Contains(scoreMessageId))
            {
                DeletedMessages.Add(scoreMessageId);
            }
        }

        /// <summary>
        /// Update an existing score range
        /// </summary>
        /// <param name="scoreMessageId"></param>
        /// <param name="lowScore"></param>
        /// <param name="highScore"></param>
        public void UpdateScoreRange(Int32 scoreMessageId, double? lowScore, double? highScore)
        {
            if (MessageDictionary.ContainsKey(scoreMessageId))
            {
                MessageDictionary[scoreMessageId].LowScore = lowScore;
                MessageDictionary[scoreMessageId].HighScore = highScore;
            }
        }

        #region ItemData Methods


        /// <summary>
        /// Get name of load proc
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetScoreMessage"; } }

        /// <summary>
        /// Load item data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            PageId = DbUtility.GetValueFromDataRow<int?>(data, "PageId", null);
        }

        /// <summary>
        /// Create dataset to contain item data
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new ItemDataSet(ObjectTypeName, ItemDataTableName, "ItemId", MessageTableName);
        }


        /// <summary>
        /// Load from data set
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            if (!data.Tables.Contains(MessageTableName))
            {
                return;
            }

            MessageDictionary.Clear();

            DataRow[] messageRows = data.Tables[MessageTableName].Select("ItemId = " + ID);

            foreach (DataRow messageRow in messageRows)
            {
                var msg = new ScoreMessage
                {
                    LowScore = DbUtility.GetValueFromDataRow<double?>(messageRow, "LowScore", null),
                    HighScore = DbUtility.GetValueFromDataRow<double?>(messageRow, "HighScore", null),
                    MessageId = DbUtility.GetValueFromDataRow(messageRow, "ScoreMessageId", 0)
                };

                MessageDictionary[msg.MessageId] = msg;
            }
        }

        /// <summary>
        /// Create an instance of the item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified to create score message item data.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertScoreMessage");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("PageID", DbType.Int32, PageId);

            db.ExecuteNonQuery(command, t);

            UpdateScoreMessages(db, t);
        }

        /// <summary>
        /// Update an instance of the item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified to update score message item data.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateScoreMessage");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("PageID", DbType.Int32, PageId);

            db.ExecuteNonQuery(command, t);

            UpdateScoreMessages(db, t);
        }

        /// <summary>
        /// Update scoring messages
        /// </summary>
        /// <param name="db"></param>
        /// <param name="t"></param>
        protected virtual void UpdateScoreMessages(Database db, IDbTransaction t)
        {
            //Remove deleted
            foreach (var deletedMessageId in DeletedMessages)
            {
                DBCommandWrapper msgCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_SM_DeleteMessage");
                msgCommand.AddInParameter("ScoreMessageId", DbType.Int32, deletedMessageId);
                msgCommand.AddInParameter("ItemID", DbType.Int32, ID);

                if (MessagesDeleted != null)
                    MessagesDeleted(this, new MessagesDeletedEventArgs{Id = deletedMessageId});

                db.ExecuteNonQuery(msgCommand, t);
            }

            var messages = new List<ScoreMessage>();

            //Update existing/Add new
            foreach (ScoreMessage msg in MessageDictionary.Values)
            {
                DBCommandWrapper msgCommand;

                if (msg.MessageId > 0)
                {
                    msgCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_SM_UpdateMessage");
                    msgCommand.AddInParameter("ScoreMessageId", DbType.Int32, msg.MessageId);
                }
                else
                {
                    msgCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_SM_AddMessage");
                    msgCommand.AddOutParameter("ScoreMessageId", DbType.Int32, 4);
                }
                
                msgCommand.AddInParameter("ItemId", DbType.Int32, ID);
                msgCommand.AddInParameter("LowScore", DbType.Double, msg.LowScore);
                msgCommand.AddInParameter("HighScore", DbType.Double, msg.HighScore);

                db.ExecuteNonQuery(msgCommand, t);

                if (msg.MessageId <= 0)
                {
                    object outVal = msgCommand.GetParameterValue("ScoreMessageId");

                    if (outVal != null && outVal != DBNull.Value)
                    {
                        var args = new MessagesUpdatedEventArgs {OldId = msg.MessageId, NewId = (int) outVal};
                        
                        msg.MessageId = (int)outVal;

                        if (MessagesUpdated != null)
                            MessagesUpdated(this, args);
                    }
                }

                messages.Add(msg);
            }

            MessageDictionary.Clear();
            foreach (var scoreMessage in messages)
            {
                MessageDictionary.Add(scoreMessage.MessageId, scoreMessage);
            }

            //Clear deleted messages
            DeletedMessages.Clear();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementValue("PageId", PageId);

			 PersistedDomainObjectDataSet ds = GetConfigurationDataSet(ID.Value);

			 DataRow[] messageRows = ds.Tables[MessageTableName].Select("ItemId = " + ID);

			 int cnt = messageRows.Length;

			writer.WriteStartElement("ScoreMessages");
			writer.WriteAttributeString("Count", cnt.ToString());

			foreach (DataRow messageRow in messageRows)
			{
				writer.WriteStartElement("Message");

				var msg = new ScoreMessage
				{
					LowScore = DbUtility.GetValueFromDataRow<double?>(messageRow, "LowScore", null),
					HighScore = DbUtility.GetValueFromDataRow<double?>(messageRow, "HighScore", null),
					MessageId = DbUtility.GetValueFromDataRow(messageRow, "ScoreMessageId", 0)
				};

				writer.WriteElementValue("LowScore", msg.LowScore);
				writer.WriteElementValue("HighScore", msg.HighScore);
                
                writer.WriteStartElement("MessageTexts");
                WriteTextValue(writer, GetMessageTextId(msg.MessageId), msg.MessageId.ToString());
				writer.WriteEndElement(); //Message Texts
                writer.WriteEndElement(); //Message

			}

			writer.WriteEndElement();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            PageId = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("PageId"));

            var messageNodes = xmlNode.SelectNodes("ScoreMessages/Message");

            foreach(XmlNode messageNode in messageNodes)
            {
                var msg = new ScoreMessage
                              {
                                  LowScore = XmlUtility.GetNodeDouble(messageNode.SelectSingleNode("LowScore")),
                                  HighScore = XmlUtility.GetNodeDouble(messageNode.SelectSingleNode("HighScore"))
                              };


                MessageDictionary.Add(-1, msg);

				Save();

                var messageTextNodes = messageNode.SelectNodes("MessageTexts/Text");
                var messageTextId = GetMessageTextId(msg.MessageId);

                foreach (XmlNode messageTextNode in messageTextNodes)
                {
                    LoadTextFromNode(messageTextNode, messageTextId);
                }
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        /// <param name="pageIdMap"></param>
        protected internal override void UpdateImportId(Dictionary<int, ItemData> itemIdMap, Dictionary<int, TemplatePage> pageIdMap)
        {
            if (!PageId.HasValue || PageId.Value == 0)
                return;

            PageId = pageIdMap[PageId.Value].ID;
        }

        #region Other

        /// <summary>
        /// Create an instance of a score message
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new ScoreMessageItem();
        }

        /// <summary>
        /// Create an instance of a score item text decorator
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new ScoreMessageTextDecorator(this, languageCode);
        }

        #endregion

        /// <summary>
        /// Updates pipes
        /// </summary>
        /// <param name="callback"></param>
        public override void UpdatePipes(ItemData.UpdatePipesCallback callback)
        {
            base.UpdatePipes(callback);

            foreach (int key in MessageDictionary.Keys)
            {
                updatePipes(callback, GetMessageTextId(MessageDictionary[key].MessageId));
            }
        }
    }
}
