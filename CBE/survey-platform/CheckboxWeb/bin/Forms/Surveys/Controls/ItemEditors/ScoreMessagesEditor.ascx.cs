using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public class ScoreMessageModel : Checkbox.Forms.Items.Configuration.ScoreMessage
    {
        public string MessageText { set; get; }
    }

    /// <summary>
    /// Editor for configuring scoring messages
    /// </summary>
    public partial class ScoreMessagesEditor : Checkbox.Web.Common.UserControlBase
    {
        private ScoreMessageTextDecorator _decorator;

        /// <summary>
        /// Messages
        /// </summary>
        protected List<ScoreMessageModel> Messages
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_Messages"] == null)
                {
                    HttpContext.Current.Session[ID + "_Messages"] = new List<ScoreMessageModel>();
                }

                return (List<ScoreMessageModel>)HttpContext.Current.Session[ID + "_Messages"];
            }

            set
            {
                HttpContext.Current.Session[ID + "_Messages"] = value;   
            }
        }

        /// <summary>
        /// Bind event handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _addMessageBtn.Click += _addMessageBtn_Click;
            _postMessagesBtn.Click += _postMessagesBtn_Click;
        }

        void _postMessagesBtn_Click(object sender, EventArgs e)
        {
            var messages = Request.Form["normalEntryOptionOrder"];

            using (XmlTextReader reader = new XmlTextReader(new StringReader(messages)))
            {
                Messages.Clear();

                string id = string.Empty;
                string low = string.Empty;
                string high = string.Empty;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            while (reader.MoveToNextAttribute())
                            {
                                string name = reader.Name;
                                string value = reader.Value;

                                switch (name)
                                {
                                    case "id":
                                        id = value;
                                        break;
                                    case "low":
                                        low = value;
                                        break;
                                    case "high":
                                        high = value;
                                        break;
                                }
                            }

                            break;

                        case XmlNodeType.Text:
                            float fHigh, fLow;
                            float.TryParse(high, out fHigh);
                            float.TryParse(low, out fLow);

                            Messages.Add(new ScoreMessageModel
                            {
                                HighScore = fHigh,
                                LowScore = fLow,
                                MessageId = Convert.ToInt32(id),
                                MessageText = reader.Value
                            });
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Handle add button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _addMessageBtn_Click(object sender, EventArgs e)
        {
            //  TODO: Validation
            double? minScore = Utilities.AsDouble(_minScoreTxt.Text);
            double? maxScore = Utilities.AsDouble(_maxScoreTxt.Text);

            //Figure out temporary negative ID for new messages so they can be
            // updated.
            int nextId = Messages.Count > 0
                ? Math.Min(Messages.Min(msg => msg.MessageId) - 1, -1)
                : -1;
            
            if (minScore.HasValue || maxScore.HasValue)
            {
                Messages.Add(new ScoreMessageModel
                {
                    LowScore = minScore ?? 0,
                    HighScore = maxScore ?? 0,
                    MessageText = _messageText.Text.Trim(),
                    MessageId = nextId
                });

                Messages = new List<ScoreMessageModel>(Messages.OrderBy(msg => msg.LowScore));

                //Clear inputs
                _minScoreTxt.Text = string.Empty;
                _maxScoreTxt.Text = string.Empty;
                _messageText.Text = string.Empty;
            }
        }

        /// <summary>
        /// Initialize score message editor
        /// </summary>
        /// <param name="decorator"></param>
        /// <param name="isPagePostback"></param>
        public void Initialize(ScoreMessageTextDecorator decorator, bool isPagePostback)
        {
            _decorator = decorator;

            if (!isPagePostback)
            {
                BuildMessages(decorator.Data);
                decorator.Data.MessagesUpdated += Data_MessagesUpdated;
            }
        }

        void Data_MessagesUpdated(object sender, MessagesUpdatedEventArgs e)
        {
            if (e.NewId != e.OldId)
            {
                var message = Messages.FirstOrDefault(m => m.MessageId == e.OldId);
                if (message != null)
                {
                    message.MessageId = e.NewId;
                }

                BuildMessages(sender as ScoreMessageItemData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void BuildMessages(ScoreMessageItemData data)
        {
            if(data != null)
            {
                var messageList = data.Messages.Select(
                    message => new ScoreMessageModel
                    {
                        LowScore = message.LowScore,
                        HighScore = message.HighScore,
                        MessageId = message.MessageId,
                        MessageText = _decorator.GetMessageText(message.MessageId),
                    }).ToList();

                Messages = messageList;
            }
        }
        
        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="decorator"></param>
        public void UpdateData(ScoreMessageTextDecorator decorator)
        {
            //Update existing & add new messages
            foreach (var message in Messages)
            {
                var messageText = Utilities.AdvancedHtmlDecode(message.MessageText);

                //Update existing
                if (message.MessageId > 0)
                {
                    decorator.Data.UpdateScoreRange(
                        message.MessageId,
                        message.LowScore,
                        message.HighScore);

                    decorator.SetMessageText(
                        message.MessageId,
                        messageText);
                } //Add new
                else
                {
                    int id = decorator.Data.AddScoreRange(
                        message.LowScore,
                        message.HighScore);

                    decorator.SetMessageText(id, messageText);
                }
            }

            //Remove deleted
            for (int i = 0; i < decorator.Data.Messages.Count; i++)
            {
                var message = decorator.Data.Messages[i];

                if (Messages.All(m => m.MessageId != message.MessageId))
                {
                    decorator.Data.DeletedMessages.Add(message.MessageId);
                    decorator.Data.RemoveScoreRange(message.MessageId);
                    i--;
                }
            }
        }
    }
}