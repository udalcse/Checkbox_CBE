//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Checkbox.Content;
using Checkbox.Security;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Abstract class to contain configuration information for select items
    /// </summary>
    [Serializable]
    public abstract class SelectItemData : LabelledItemData, ICloneable
    {
        private ListData _optionsList;

        /// <summary>
        /// Get name of ItemLists table
        /// </summary>
        public virtual string ItemListsTableName { get { return "ItemLists"; } }

        /// <summary>
        /// Get name of item options table
        /// </summary>
        public virtual string ItemOptionsTableName { get { return "ItemOptions"; } }

        /// <summary>
        /// Get/set whether the options associated with a select item should be presented in random order.
        /// </summary>
        public virtual bool Randomize { get; set; }

        /// <summary>
        /// Get/set whether an "other" response, i.e. a response not in the list of options, is allowed.
        /// </summary>
        public virtual bool AllowOther { get; set; }

        /// <summary>
        /// Get/set list id
        /// </summary>
        public int ListId { get; protected set; }

        /// <summary>
        /// Gets or sets the bindined property identifier.
        /// </summary>
        /// <value>
        /// The bindined property identifier.
        /// </value>
        public int? BindinedPropertyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the binded property.
        /// </summary>
        /// <value>
        /// The name of the binded property.
        /// </value>
        public string BindedPropertyName { get; set; }

        /// <summary>
        /// Get options list
        /// </summary>
        public ListData OptionsList
        {
            get
            {
                if (_optionsList == null)
                {
                    _optionsList = new ListData();

                    if (ListId > 0)
                    {
                        _optionsList.Load(ListId);
                    }
                }

                return _optionsList;
            }

            private set { _optionsList = value; }
        }

        /// <summary>
        /// Get/set the ID of the text to display as the prompt for the other response.
        /// </summary>
        public virtual string OtherTextID
        {
            get { return GetTextID("otherText"); }
        }

        /// <summary>
        /// Get/set the ID of the text to display as the prompt for the none of above response.
        /// </summary>
        public virtual string NoneOfAboveTextID
        {
            get { return GetTextID("noneOfAboveText"); }
        }

        /// <summary>
        /// Create configuration data for items
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new ItemDataSet(ObjectTypeName, ItemDataTableName, "ItemId", ItemListsTableName, ItemOptionsTableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is SurveyItemMetaData)
            {
                //Add options
                var optionList = new List<SurveyOptionMetaData>();
                var languageList = ((SurveyItemMetaData) itemDto).TextData.Select(td => td.LanguageCode);

                foreach (var itemOption in Options)
                {
                    var optionMetaData = new SurveyOptionMetaData
                    {
                        Id = itemOption.OptionID,
                        IsOther = itemOption.IsOther,
                        IsNoneOfAbove = itemOption.IsNoneOfAbove,
                        Alias = itemOption.Alias,
                        Points = itemOption.Points,
                        IsDefault = itemOption.IsDefault,
                        Position = itemOption.Position
                    };

                    var textDataList = new List<TextData>();

                    foreach (var languageCode in languageList)
                    {
                        var textData = new TextData {LanguageCode = languageCode, TextValues = new SimpleNameValueCollection()};
                        textData.TextValues["OptionText"] = TextManager.GetText(itemOption.TextID, languageCode);

                        if (itemOption.IsOther)
                        {
                            textData.TextValues["OptionText"] = TextManager.GetText(OtherTextID, languageCode);
                        }
                        else if (itemOption.IsNoneOfAbove)
                        {
                            textData.TextValues["OptionText"] = TextManager.GetText(NoneOfAboveTextID, languageCode);
                        }

                        textDataList.Add(textData);
                    }

                    optionMetaData.TextData = textDataList.ToArray();

                    optionList.Add(optionMetaData);
                }

                ((SurveyItemMetaData) itemDto).Options = optionList.ToArray();
            }
        }

        /// <summary>
        /// Remove an option from the list
        /// </summary>
        public void RemoveOption(int optionId)
        {
            OptionsList.RemoveOption(optionId);
        }

        /// <summary>
        /// Add a list option
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="category"></param>
        /// <param name="isDefault"></param>
        /// <param name="position"></param>
        /// <param name="points"></param>
        /// <param name="isOther"></param>
        /// <param name="isNoneOfAbove"></param>
        /// <param name="contentID"></param>
        /// <param name="temporaryGuid"> </param>
        public ListOptionData AddOption(string alias, string category, bool isDefault, int position, double points, bool isOther, bool isNoneOfAbove, int? contentID, Guid? temporaryGuid = null)
        {
            //The item ID is set when the list is saved
            return OptionsList.AddOption(-1, alias, category, isDefault, position, points, isOther, isNoneOfAbove, contentID, -1, temporaryGuid);
        }

        /// <summary>
        /// Update an option
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="alias"></param>
        /// <param name="category"></param>
        /// <param name="isDefault"></param>
        /// <param name="position"></param>
        /// <param name="points"></param>
        /// <param name="isOther"></param>
        /// <param name="isNoneOfAbove"></param>
        /// <param name="contentID"></param>
        /// <param name="temporaryGuid"> </param>
        public void UpdateOption(Int32 optionId, string alias, string category, bool isDefault, int position, double points, bool isOther, bool isNoneOfAbove, int? contentID, Guid? temporaryGuid = null)
        {
            OptionsList.UpdateOption(optionId, -1, alias, category, isDefault, position, points, isOther, isNoneOfAbove, contentID, temporaryGuid);
        }

        /// <summary>
        /// Item options
        /// </summary>
        public virtual ReadOnlyCollection<ListOptionData> Options
        {
            get
            {
                return OptionsList.ListOptions;
            }
        }

        /// <summary>
        /// Get the option with the specified id
        /// </summary>
        /// <param name="optionID"></param>
        /// <returns></returns>
        public virtual ListOptionData GetOption(Int32 optionID)
        {
            return OptionsList.ListOptions.FirstOrDefault(option => option.OptionID == optionID);
        }

        /// <summary>
        /// Set column mappings
        /// </summary>
        /// <param name="data"></param>
        public override void SetConfigurationDataSetColumnMappings(DataSet data)
        {
            base.SetConfigurationDataSetColumnMappings(data);

            if (data.Tables.Contains("ItemOptions"))
            {
                DataTable dt = data.Tables["ItemOptions"];
                dt.Columns["OptionID"].ColumnMapping = MappingType.Attribute;
                dt.Columns["IsDefault"].ColumnMapping = MappingType.Attribute;
                dt.Columns["Position"].ColumnMapping = MappingType.Attribute;
            }
        }

        /// <summary>
        /// Update lists associated with the select item
        /// </summary>
        /// <param name="transaction"></param>
        protected virtual void UpdateLists(IDbTransaction transaction)
        {
            //Make sure lists contain "other", if necessary
            ListOptionData otherOption = null;

            var optionsToRemove = new List<ListOptionData>();
            ReadOnlyCollection<ListOptionData> tempListOptions = OptionsList.ListOptions;

            foreach (ListOptionData option in tempListOptions)
            {
                if (option.IsOther)
                {
                    otherOption = option;

                    if (!AllowOther)
                    {
                        optionsToRemove.Add(option);
                        otherOption = null;
                    }
                }
            }

            foreach (ListOptionData option in optionsToRemove)
            {
                OptionsList.RemoveOption(option.OptionID);
            }

            UpdateOtherOption(otherOption);

            //Update the lists
            OptionsList.Save(transaction);

            Database db = DatabaseFactory.CreateDatabase();

            //AddListData will not add duplicates, so it's safe to call for all lists
            DBCommandWrapper addCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_AddListData");
            addCommand.AddInParameter("ListID", DbType.Int32, OptionsList.ID);
            addCommand.AddInParameter("ItemID", DbType.Int32, ID);
            db.ExecuteNonQuery(addCommand, transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherOption"></param>
        protected virtual void UpdateOtherOption(ListOptionData otherOption)
        {
            //Add an "other" option if necessary or move the existing "other" option to the end
            if (AllowOther && OptionsList != null)
            {
                if (otherOption == null)
                {
                    //OptionsList.AddOption(ID.Value, "other", otherOption.Category, false, OptionsList.ListOptions.Count + 1, 0, true);
                    OptionsList.AddOption(ID.Value, "other", String.Empty, false, OptionsList.ListOptions.Count + 1, 0, true, false, null);
                }
                else
                {
                    OptionsList.UpdateOption(otherOption.OptionID, -1, "other", otherOption.Category, false, OptionsList.ListOptions.Count + 1, 0, true, false, null);
                }
            }
        }

        /// <summary>
        /// Load additional data from the data set
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            //Get item list id
            if (data.Tables.Contains(ItemListsTableName))
            {
                List<int> listIds = DbUtility.ListDataColumnValues<int>(data.Tables[ItemListsTableName], "ListId",
                                                                        "ItemID = " + ID.Value.ToString(), null, false);

                if (listIds.Count > 0)
                {
                    ListId = listIds[0];
                }
            }
        }

        /// <summary>
        /// Override on commit to call for children
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnCommit(object sender, EventArgs e)
        {
            base.OnCommit(sender, e);

            OptionsList.NotifyCommit(sender, e);
        }

        /// <summary>
        /// Override onabort to call for children
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnAbort(object sender, EventArgs e)
        {
            base.OnAbort(sender, e);

            OptionsList.NotifyAbort(sender, e);
        }

        /// <summary>
        /// Get whether this represents an answerable item
        /// </summary>
        public override bool ItemIsIAnswerable
        {
            get { return true; }
        }

        /// <summary>
        /// Select items support scoring
        /// </summary>
        public override bool ItemIsIScored
        {
            get { return true; }
        }

        /// <summary>
        /// Create a text decoroator for the item that can be used to localize texts for the item
        /// and it's options.
        /// </summary>
        /// <param name="languageCode">Language for the text decorator.</param>
        /// <returns><see cref="SelectItemTextDecorator"/></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new SelectItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// Copy the item
        /// </summary>
        /// <returns></returns>
        protected override ItemData Copy()
        {
            var theCopy = (SelectItemData)base.Copy();

            if (theCopy != null)
            {
                theCopy.Randomize = Randomize;
                theCopy.AllowOther = AllowOther;

                theCopy.OptionsList = (ListData)OptionsList.Clone();

            }

            return theCopy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            Save();

            BindedPropertyName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("BindedPropertyName"));

            Randomize = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("Randomize"));

            var optionNodes = xmlNode.SelectNodes("Options/Option");
          
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    foreach (XmlNode optionNode in optionNodes)
                    {
                        var option = AddOption(
                                XmlUtility.GetNodeText(optionNode.SelectSingleNode("Alias")),
                                XmlUtility.GetNodeText(optionNode.SelectSingleNode("Category")),
                                XmlUtility.GetNodeBool(optionNode.SelectSingleNode("IsDefault")),
                                XmlUtility.GetNodeInt(optionNode.SelectSingleNode("Position")) ?? -1,
                                XmlUtility.GetNodeDouble(optionNode.SelectSingleNode("Points")) ?? 0,
                                XmlUtility.GetNodeBool(optionNode.SelectSingleNode("IsOther")),
                                XmlUtility.GetNodeBool(optionNode.SelectSingleNode("IsNoneOfAbove")),
                                ReadOptionContent(optionNode)
                            );

                        option.Disabled = XmlUtility.GetNodeBool(optionNode.SelectSingleNode("Disabled"));

                        //Save();
                        _optionsList.InsertOption(transaction, option);

                        var optionTextNodes =
                            optionNode.SelectSingleNode("OptionTexts") != null
                                ? optionNode.SelectNodes("OptionTexts/Text/TextData")
                                : optionNode.SelectNodes("ItemTexts/Text/TextData");

                        foreach (XmlNode optionTextNode in optionTextNodes)
                        {
                            LoadTextFromNode(optionTextNode, GetOptionTextID(option));
                        }
                    }

                    transaction.Commit();

                    //save mapping
                    if (!string.IsNullOrWhiteSpace(BindedPropertyName) && ID.HasValue)
                    {
                        var property = ProfileManager.GetPropertiesList().FirstOrDefault(item => item.Name.Equals(BindedPropertyName));
                        if (property != null)
                        {
                            if (PropertyBindingManager.AddItemMapping(ID.Value, property.FieldId, CustomFieldType.RadioButton))
                            {
                                SaveRadioButtonBindedAliases(xmlNode, BindedPropertyName);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessProtected");

                    transaction.Rollback();
                    OnAbort(this, EventArgs.Empty);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            //Now commit
            OnCommit(this, EventArgs.Empty);

            //we must set AllowOther after importing all options, because one of them may have isOther flag
            //but be imported 3rd 
            AllowOther = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("AllowOther"));

            Save();
        }

        /// <summary>
        /// Saves the RadioButton binded aliases.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        private void SaveRadioButtonBindedAliases(XmlNode xmlNode, string bindedPropertyName)
        {
            ArgumentValidation.CheckForNullReference(xmlNode, "xmlNode");

            var optionsAliases = xmlNode.SelectNodes("OptionAliases/Alias");

            if (optionsAliases != null)
            {
                List<RadioButtonFieldOption> aliases = new List<RadioButtonFieldOption>();

                foreach (XmlNode aliasNode in optionsAliases)
                {
                    var name = XmlUtility.GetNodeText(aliasNode.SelectSingleNode("Name"));
                    var optionId = XmlUtility.GetNodeInt(aliasNode.SelectSingleNode("OptionId")).Value;
                    var aliasTitle = XmlUtility.GetNodeText(aliasNode.SelectSingleNode("AliasTitle"));

                    aliases.Add(
                        new RadioButtonFieldOption(name, false, aliasTitle, optionId, this.ID.Value));
                }

                ProfileManager.AddRadioButtonFieldOptionAlias(this.ID.Value, aliases, bindedPropertyName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer,
            WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("AllowOther", AllowOther.ToString());

            writer.WriteElementString("Randomize", Randomize.ToString());

            writer.WriteStartElement("Options");

            writer.WriteAttributeString("Count", OptionsList.ListOptions.Count.ToString());

            foreach (ListOptionData option in OptionsList.ListOptions)
            {
                if (option.Deleted)
                    continue;

                writer.WriteStartElement("Option");

                writer.WriteElementString("Id", option.OptionID.ToString());
                writer.WriteElementString("Alias", option.Alias);
                writer.WriteElementString("Category", option.Category);
                writer.WriteElementString("Disabled", option.Disabled.ToString());
                writer.WriteElementString("IsDefault", option.IsDefault.ToString());
                writer.WriteElementString("IsOther", option.IsOther.ToString());
                writer.WriteElementString("Points", option.Points.ToString());
                writer.WriteElementString("Position", option.Position.ToString());
                writer.WriteElementString("IsNoneOfAbove", option.IsNoneOfAbove.ToString());

                WriteOptionContent(writer, option.ContentID);

                writer.WriteStartElement("OptionTexts");
                WriteTextValue(writer, GetOptionTextID(option), option.Position.ToString());
                writer.WriteEndElement(); //optionTexts

                writer.WriteEndElement(); // option
            }

            writer.WriteEndElement(); //options

            WriteOptionAliases(writer);
        }

        private void WriteOptionAliases(XmlWriter writer)
        {
            if (BindinedPropertyId.HasValue && this.ID.HasValue)
            {
                var radioButton = PropertyBindingManager.GetBindedPropertyByItemId(this.ID.Value);
                var user = UserManager.GetCurrentPrincipal();
                if (radioButton != null && user != null)
                {
                    writer.WriteElementString("BindedPropertyName", radioButton.Name);

                    var radioButtonStructure = ProfileManager.GetRadioButtonField(radioButton.Name, user.UserGuid);
                    var itemAliases = ProfileManager.GetRadioOptionAliases(this.ID.Value, radioButton.Name);

                    if (radioButtonStructure != null && radioButtonStructure.Options.Any())
                    {
                        writer.WriteStartElement("OptionAliases");

                        for (int i = 0; i < radioButtonStructure.Options.Count; i++)
                        {
                            writer.WriteStartElement("Alias");
                            writer.WriteElementString("Name", radioButtonStructure.Options[i].Name);
                            writer.WriteElementString("AliasTitle", itemAliases[radioButtonStructure.Options[i].Name]);
                            writer.WriteElementString("OptionId", radioButtonStructure.Options[i].Id.ToString());
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                }
            }
        }

        private string GetOptionTextID(ListOptionData option)
        {
            if (option.IsOther)
                return OtherTextID;
            if (option.IsNoneOfAbove)
                return NoneOfAboveTextID;

            return option.TextID;
        }

        /// <summary>
        /// Write the information about option content in xml
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="contentID"></param>
        protected void WriteOptionContent(XmlWriter writer, int? contentID)
        {
            writer.WriteStartElement("OptionContent");

            if (contentID.HasValue)
            {                                
                var contentItem = DBContentManager.GetItem(contentID.Value);
                
                writer.WriteStartElement("Data");
                if (contentItem.Data != null)
                {
                    writer.WriteCData(Convert.ToBase64String(contentItem.Data));
                }
                writer.WriteEndElement(); //Image data

                writer.WriteElementString("ContentType", contentItem.ContentType);
                writer.WriteElementString("ItemUrl", contentItem.ItemUrl);
                writer.WriteElementString("ItemName", contentItem.ItemName);
                
            }

            writer.WriteEndElement(); //optionImage
        }

        /// <summary>
        /// Read the information about option content
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        protected int? ReadOptionContent(XmlNode xmlNode)
        {
            var contentNode = xmlNode.SelectSingleNode("OptionContent");

            if (contentNode == null || !contentNode.HasChildNodes)
            {
                return null;
            }

            string contentData = XmlUtility.GetNodeText(contentNode.SelectSingleNode("Data"));

            //Create a new content item
            DBContentItem contentItem = new DBContentItem(null);
            contentItem.Data = string.IsNullOrEmpty(contentData) ? null : Convert.FromBase64String(contentData);
            contentItem.ItemUrl = XmlUtility.GetNodeText(contentNode.SelectSingleNode("ItemUrl"));
            contentItem.ItemName = Path.GetFileName(contentItem.ItemUrl);
            contentItem.CreatedBy = UserManager.GetCurrentPrincipal().Identity.Name;
            contentItem.ContentType = XmlUtility.GetNodeText(contentNode.SelectSingleNode("ContentType"));
            contentItem.LastUpdated = DateTime.Now;
            contentItem.Save();

            return contentItem.ItemID;
        }

        /// <summary>
        /// Ruturns the clone of the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var theClone = (SelectItemData)base.Copy();

            theClone._optionsList = _optionsList != null ? (ListData)_optionsList.Clone() : null;
            theClone.AllowOther = AllowOther;
            theClone.ListId = ListId;
            theClone.Randomize = Randomize;
            if (this is SelectManyData)
            {
                ((SelectManyData)(theClone)).MinToSelect = ((SelectManyData)(this)).MinToSelect;
                ((SelectManyData)(theClone)).MaxToSelect = ((SelectManyData)(this)).MaxToSelect;
                ((SelectManyData)(theClone)).AllowNoneOfAbove = ((SelectManyData)(this)).AllowNoneOfAbove;
            }
            return theClone;
        }

        /// <summary>
        /// Updates pipes
        /// </summary>
        /// <param name="callback"></param>
        public override void UpdatePipes(ItemData.UpdatePipesCallback callback)
        {
            base.UpdatePipes(callback);

            updatePipes(callback, OtherTextID);
            foreach (ListOptionData option in this.Options)
            {
                updatePipes(callback, option.TextID);
            }
        }
    }
}
