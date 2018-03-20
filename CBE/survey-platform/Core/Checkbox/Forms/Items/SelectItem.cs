//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Checkbox.Common;
using Checkbox.Forms.Validation;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
	/// <summary>
	/// An item that allows selection from a list of inputs
	/// </summary>
	[Serializable]
	public abstract class SelectItem : LabelledItem
	{
		private Dictionary<Int32, ListOption> _optionsDictionary;
		private List<ListOption> _orderedOptions;

		/// <summary>
		/// Configure the item based on the supplied configuration.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="languageCode"></param>
		/// <param name="templateId"></param>
		public override void Configure(ItemData configuration, string languageCode, int? templateId)
		{
			ArgumentValidation.CheckExpectedType(configuration, typeof(SelectItemData));

			base.Configure(configuration, languageCode, templateId);

			var config = (SelectItemData)configuration;

			OtherTextPrompt = GetText(config.OtherTextID);
		    NoneOfAboveTextPrompt = GetText(config.NoneOfAboveTextID);

			IsRandomized = config.Randomize;
			AllowOther = config.AllowOther;

			CreateOptions(config.Options);
		}

		/// <summary>
		/// Create list options from the option meta data
		/// </summary>
		/// <param name="metaOptions"></param>
		protected virtual void CreateOptions(ReadOnlyCollection<ListOptionData> metaOptions)
		{
			OptionsDictionary.Clear();
			Options.Clear();

			foreach (ListOptionData metaOption in metaOptions)
			{
				ListOption option = CreateOption(metaOption);

				if (option != null)
				{
					OptionsDictionary[option.ID] = option;
                    
                    if (Utilities.IsTextEncoded(option.Text))
                        option.Text = Utilities.AdvancedHtmlDecode(option.Text);
					
                    Options.Add(option);
				}
			}
		    if (IsRandomized)
		    {
		        RandomizeOptionOrder();
		    }
		}

		/// <summary>
		/// Get the dictionary of item options
		/// </summary>
		protected Dictionary<int, ListOption> OptionsDictionary
		{
			get { return _optionsDictionary ?? (_optionsDictionary = new Dictionary<int, ListOption>()); }
		}

		/// <summary>
		/// Create a list option from the provided meta option
		/// </summary>
		/// <param name="metaOption"></param>
		/// <returns></returns>
		protected virtual ListOption CreateOption(ListOptionData metaOption)
		{
			var option = new ListOption {
				ID = metaOption.OptionID, 
				Alias = metaOption.Alias, 
				Category = metaOption.Category,
				IsSelected = metaOption.IsDefault, 
				IsDefault = metaOption.IsDefault,
				IsOther = metaOption.IsOther,
                IsNoneOfAbove = metaOption.IsNoneOfAbove,
                Disabled = metaOption.Disabled, 
				Points = metaOption.Points, 
				ContentID = metaOption.ContentID
            };

		    if (metaOption.IsOther)
		        option.Text = OtherTextPrompt;
            else if (metaOption.IsNoneOfAbove)
                option.Text = NoneOfAboveTextPrompt;
            else
                option.Text = GetText(metaOption.TextID);

			//Assign the delegate to get option text
			option.OptionTextDelegate += GetOptionText;

			return option;
		}

		/// <summary>
		/// Get the text of an option
		/// </summary>
		/// <param name="optionId"></param>
		/// <param name="defaultText"></param>
		/// <returns></returns>
		public virtual string GetOptionText(int optionId, string defaultText)
		{
		    var text = GetPipedText("OptionText_" + ID + "_" + optionId, defaultText);

            if (ItemTypeName == "DropdownList")
                text = Utilities.StripHtml(text);

            return text;
		}


		/// <summary>
		/// Get a list of select item options, ordered as specified
		/// by configuration
		/// </summary>
		public List<ListOption> Options
		{
			get { return _orderedOptions ?? (_orderedOptions = new List<ListOption>()); }
		}

		 /// <summary>
		/// Get a list of selected options in order displayed in survey
		/// </summary>
		public List<ListOption> SelectedOptions
		{
			get
			{
				return Options.Where(option => option.IsSelected).ToList();                
			}
		}

		/// <summary>
		/// When the response state has been restored, check the state for a persisted
		/// option order, so that randomized options can maintain the same random order
		/// within the context of a single response.
		/// </summary>
		protected override void OnStateRestored()
		{
			base.OnStateRestored();

			if (IsRandomized)
			{
				//Attempt to get the order from the response
				List<int> optionOrder = Response.GetItemOptionOrder(ID);

				//Verify that the loaded order has the correct option ids
				if (!VerifyOptionOrder(optionOrder))
				{
					//Randomize order and save. Randomize method sets Options collection to be
					// in correct order
					RandomizeOptionOrder();

					for (int i = 0; i < Options.Count; i++)
					{
						Response.SaveItemOptionOrder(ID, Options[i].ID, i + 1);
					}
				}
				else if (optionOrder.Count > 0)
				{
					//Store the ordered options if option order is found
					Options.Clear();

					foreach (int optionId in optionOrder)
					{
						if (OptionsDictionary.ContainsKey(optionId))
						{
							Options.Add(OptionsDictionary[optionId]);
						}
					}
				}
			}
		}

		/// <summary>
		/// Randomize the order of options
		/// </summary>
		protected virtual void RandomizeOptionOrder()
		{
			//Ensure "other" options stay last
			var otherOptions = Options.Where(option => option.IsOther || option.IsNoneOfAbove);

			//"Sort" the items
			var orderedOptions = new List<ListOption>();
			orderedOptions.AddRange(Utilities.RandomizeList(Options.Where(option => !option.IsOther && !option.IsNoneOfAbove).ToList()));
			orderedOptions.AddRange(otherOptions);

			Options.Clear();
			Options.AddRange(orderedOptions);
		}

		/// <summary>
		/// Override on answer data set to synchronize selected options
		/// </summary>
		protected override void OnAnswerDataSet()
		{
			base.OnAnswerDataSet();

			SynchronizeSelectedOptions();
		}

		/// <summary>
		/// Override delete answer handling, which removes answers when an
		/// item is excluded to ensure that selected options are syncrhonized.
		/// </summary>
		public override void DeleteAnswers()
		{
			base.DeleteAnswers();

			SynchronizeSelectedOptions();
		}

        /// <summary>
        /// 
        /// </summary>
        internal override void InitializeDefaults()
        {
            //If there are no answer rows, mark default option values as selected
            if (AnswerData == null || (!AnswerData.IsAnswered(ID) && !AnswerData.HasEmptyAnswer(ID)))
            {
                foreach (ListOption option in OptionsDictionary.Values)
                {
                    option.IsSelected = option.IsDefault;
                }
            }
        }

		/// <summary>
		/// Synchronize options selected status with answer data
		/// </summary>
		protected virtual void SynchronizeSelectedOptions()
		{
		    if (AnswerData == null) 
                return;
		    
            var answeredOptions = new List<int>();
                
		    //Now build the list of selected options from the answer data
		    foreach (var option in AnswerData.GetOptionAnswersForItem(ID))
		    {
		        var optionId = option.Key;
		        answeredOptions.Add(optionId);

		        if (OptionsDictionary.ContainsKey(optionId) && OptionsDictionary[optionId].IsOther)
		        {
		            OtherText = AnswerData.GetTextAnswerForItem(ID);
		        }
		    }

		    foreach (ListOption option in OptionsDictionary.Values)
		    {
		        option.IsSelected = answeredOptions.Contains(option.ID);
		    }
		}

		/// <summary>
		/// Verify that the provided order is valid for this item
		/// </summary>
		/// <param name="optionOrder"></param>
		/// <returns></returns>
		protected virtual bool VerifyOptionOrder(List<int> optionOrder)
		{
			return optionOrder.Count == Options.Count 
				&& optionOrder.All(optionId => Options.Exists(opt => opt.ID == optionId));
		}

		/// <summary>
		/// Set the answer, which should be an array of option ids,
		/// a single option id, or null.
		/// </summary>
		/// <param name="answer"></param>
		public override void SetAnswer(object answer)
		{
			//Do nothing in "Set Answer" since we need to handle option ids and not just
			// answer texts.
		}

		/// <summary>
		/// Get a string representation of the answer
		/// </summary>
		/// <returns></returns>
		public override string GetAnswer()
		{
			var sb = new StringBuilder();
			
			List<ListOption> selectedOptions = SelectedOptions;

			for (int i = 0; i < selectedOptions.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}

				string optionText = selectedOptions[i].Text;


				if (selectedOptions[i].IsOther && Utilities.IsNotNullOrEmpty(OtherText))
				{
					optionText = OtherText;
				}

				sb.Append(optionText);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Get a boolean value indicating if this item has an answer.
		/// </summary>
		public override bool HasAnswer
		{
			get
			{
				return SelectedOptions.Count > 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srcItem"></param>
		/// <param name="answer"></param>
		protected internal override void ImportAnswers(Analytics.Import.ItemInfo srcItem, List<Analytics.Data.ItemAnswer> answers)
		{
			List<ListOption> tgtOptions = new List<ListOption>();
			string other = null;

			foreach (Analytics.Data.ItemAnswer answer in answers)
			{
				if (answer.ItemId != srcItem.itemId)
					continue;

				int optIndex;
				Analytics.Import.OptionInfo srcOption = srcItem.FindOption(answer.OptionId, out optIndex);

				if (srcOption == null)
					continue;

				ListOption tgtOption = null;

				for (int i = 0; i < Options.Count; i++)
				{
					if (!string.IsNullOrEmpty(srcOption.alias) && Options[i].Alias == srcOption.alias ||
						Options[i].Text == srcOption.text)
					{
						tgtOption = Options[i];
						break;
					}
				}

				if (tgtOption == null)
					tgtOption = Options[optIndex];

				if (tgtOption != null)
				{
					tgtOptions.Add(tgtOption);

					if (tgtOption.IsOther)
						other = answer.AnswerText;
				}
			}

			int[] selection = new int[tgtOptions.Count];

			for (int i = 0; i < selection.Length; i++)
				selection[i] = tgtOptions[i].ID;

			Select(other, selection);
		}

		/// <summary>
		/// Select the answer
		/// </summary>
		/// <param name="optionIDs"></param>
        /// <param name="forceAdding"> Add selected option even if it doesn't exists in options list </param>
		public virtual void Select(bool forceAdding = false, params int[] optionIDs)
		{
			Select(null, optionIDs, forceAdding);
		}

		/// <summary>
		/// Select the answer
		/// </summary>
		/// <param name="otherText"></param>
		/// <param name="optionIDs"></param>
        /// <param name="forceAdding"> Add selected option even if it doesn't exists in options list </param>
		public virtual void Select(string otherText, IEnumerable<int> optionIDs, bool forceAdding = false)
		{
			var newlySelectedOptions = new List<int>(optionIDs);
			OtherText = otherText;

		    if (AnswerData != null)
		    {
                //Build a list of currently selected options
                var currentlySelectedOptions = AnswerData.GetOptionAnswersForItem(ID).Select(o => o.Key).ToList();

                //Now add new options
                foreach (int optionId in newlySelectedOptions)
                {
                    if (!currentlySelectedOptions.Contains(optionId))
                    {
                        AddAnswer(optionId, otherText, forceAdding);
                    }
                }

                //Now remove unselected options
                foreach (int optionId in currentlySelectedOptions)
                {
                    if (!newlySelectedOptions.Contains(optionId))
                    {
                        DeleteAnswer(optionId);
                    }
                }
            }        

			//Update an other options
			foreach(ListOption option in _optionsDictionary.Values)
			{
                if (option.IsOther && newlySelectedOptions.Contains(option.ID))
				{
					UpdateAnswer(option.ID, otherText);
				}
			}

			//Resync. selected options
			SynchronizeSelectedOptions();

			//Fire answer changed event
			OnAnswerChanged();
		}

		/// <summary>
		/// Add an answer
		/// </summary>
		/// <param name="optionId"></param>
		/// <param name="otherText"></param>
        /// <param name="forceAdding"> Add selected option even if it doesn't exists in options list </param>
		protected virtual void AddAnswer(int optionId, string otherText, bool forceAdding = false)
		{
			//Don't allow adding answers to non-existant options
			if (!OptionsDictionary.ContainsKey(optionId) && !forceAdding)
				return;

            if (forceAdding)
            {
                ListOption option = CreateOption(new ListOptionData()
                {
                    OptionID = optionId
                });

                if (option != null)
                {
                    OptionsDictionary[option.ID] = option;

                    if (Utilities.IsTextEncoded(option.Text))
                        option.Text = Utilities.AdvancedHtmlDecode(option.Text);

                    Options.Add(option);
                }
            }

			if(OptionsDictionary.ContainsKey(optionId) && OptionsDictionary[optionId].IsOther)
			{
				AnswerData.SetOptionAnswerForItem(ID, optionId, null, otherText);
			}
            else
                AnswerData.SetOptionAnswerForItem(ID, optionId, null, string.Empty, forceAdding);
        }

		/// <summary>
		/// Delete the answer for this option
		/// </summary>
		/// <param name="optionId"></param>
		protected virtual void DeleteAnswer(int optionId)
		{
			AnswerData.DeleteOptionAnswerForItem(ID, optionId);
		}

		/// <summary>
		/// Get the answer rows for the item
		/// </summary>
		/// <param name="optionId"></param>
		/// <param name="answerText"></param>
		protected virtual void UpdateAnswer(int optionId, string answerText)
		{
            AnswerData.SetOptionAnswerForItem(ID, optionId, null, answerText);
		}

		/// <summary>
		/// Validate answers.  On the base class, just ensure that text is entered
		/// for the "other" response when answer is required.
		/// </summary>
		/// <returns></returns>
		protected override bool  ValidateAnswers()
		{
			var selectItemValidator = new SelectItemValidator();

			if(!selectItemValidator.Validate(this))
			{
				ValidationErrors.Add(selectItemValidator.GetMessage(LanguageCode));
				return false;
			}

			return true;
		}

		/// <summary>
		/// Get whether this item should randomize it's list of options.
		/// </summary>
		public bool IsRandomized { get; private set; }

        /// <summary>
		/// Get whether an "other" answer is allowed.
		/// </summary>
		public bool AllowOther { get; private set; }

		/// <summary>
		/// Specify the text for the "other" answer
		/// </summary>
		public string OtherTextPrompt { get; private set; }

        /// <summary>
        /// Specify the text for the "none of above" answer
        /// </summary>
        public string NoneOfAboveTextPrompt { get; private set; }

		/// <summary>
		/// Get the other text prompt
		/// </summary>
		public string OtherText { get; set; }

	    /// <summary>
		/// Add a placeholder answer with a null optionid to signify that the 
		/// question was seen, but has no answers.
		/// </summary>
		protected virtual void AddPlaceHolderAnswer()
		{
            /*
			//First, check to see if a placeholder already exists
			DataRow[] answerRows = AnswerData.GetAnswerRowsForItem(ID);

			//Look for the placeholder
			if (answerRows.Any(answerRow => answerRow["OptionId"] == DBNull.Value))
			{
				return;
			}

			//No placeholder found, add it
			DataRow placeHolderRow = AnswerData.CreateAnswerRow(ID);
			AnswerData.AddAnswerRow(placeHolderRow);*/
		}

		/// <summary>
		/// Remove the placeholder answer.
		/// </summary>
		protected virtual void RemovePlaceHolderAnswer()
		{
            /*
			 //First, check to see if a placeholder already exists
			DataRow[] answerRows = AnswerData.GetAnswerRowsForItem(ID);

			long? answerIdToRemove = null;

			//Look for the placeholder
			foreach(DataRow answerRow in answerRows)
			{
				if(answerRow["OptionId"] == DBNull.Value)
				{
					answerIdToRemove = DbUtility.GetValueFromDataRow<long?>(answerRow, "AnswerID", null);
				}
			}

			if(answerIdToRemove.HasValue)
			{
				AnswerData.DeleteAnswerRow(answerIdToRemove.Value);
			}*/
		}

		/// <summary>
		/// Build tranfer object to add options and answers
		/// </summary>
		/// <param name="itemDto"></param>
		protected override void BuildDataTransferObject(IItemProxyObject itemDto)
		{
			base.BuildDataTransferObject(itemDto);


			if (itemDto is SurveyResponseItem)
			{
				var optionList = new List<SurveyResponseItemOption>();

				BuildDataTransferObjectOptionList(optionList);

				((SurveyResponseItem)itemDto).Options = optionList.ToArray();
			}
		}

		/// <summary>
		/// Update object state based on answer data
		/// </summary>
		/// <param name="dto"></param>
		public override void UpdateFromDataTransferObject(IItemProxyObject dto)
		{
			base.UpdateFromDataTransferObject(dto);

			//Do nothing if object is not expected type
			if (AnswerData == null
				|| !(dto is SurveyResponseItem))
			{
				return;
			}

			//List selected answers and get "other" text
			SurveyResponseItemAnswer[] postedAnswers = ((SurveyResponseItem)dto).Answers;
			string otherAnswer = string.Empty;

			//Find "other" answer, if any
			if (AllowOther)
			{
				ListOption otherOption = Options.Find(opt => opt.IsOther);

				if (otherOption != null)
				{
					SurveyResponseItemAnswer postedOtherAnswer = postedAnswers.FirstOrDefault(posted => posted.OptionId == otherOption.ID);

					if (postedOtherAnswer != null)
					{
						otherAnswer = postedOtherAnswer.AnswerText ?? string.Empty;
					}
				}
			}

            //"Select" Option
            var forceAdding = false;
            if (!string.IsNullOrWhiteSpace(dto.Metadata["ConnectedCustomFieldKey"]))
            {
                forceAdding = true;
            }

            Select(otherAnswer, new List<int>(postedAnswers.Where(opt => opt.OptionId.HasValue).Select(opt => opt.OptionId.Value)), forceAdding);

		    CheckForEmptyAnswer(postedAnswers);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postedAnswers"></param>
        protected virtual void CheckForEmptyAnswer(SurveyResponseItemAnswer[] postedAnswers)
	    {
            if (!postedAnswers.Any())
                AnswerData.SetEmptyAnswerForItem(ID);	        
	    }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="optionList"></param>
		protected virtual void BuildDataTransferObjectOptionList(List<SurveyResponseItemOption> optionList)
		{
			optionList.AddRange(Options.Select(
				option => new SurveyResponseItemOption
				{
					IsOther = option.IsOther, 
                    IsNoneOfAbove = option.IsNoneOfAbove,
					OptionId = option.ID, 
					Points = option.Points, 
					Text = option.Text, 
                    Alias = option.Alias,
					IsSelected = option.IsSelected,
					IsDefault = option.IsDefault,
					ContentId = option.ContentID
				}
			));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="answerList"></param>
		protected override void BuildDataTransferObjectAnswerList(List<SurveyResponseItemAnswer> answerList)
		{
		    base.BuildDataTransferObjectAnswerList(answerList);

            foreach (var answer in answerList)
            {
                if (answer.OptionId.HasValue && OptionsDictionary.ContainsKey(answer.OptionId.Value))
                {
                    answer.Points = OptionsDictionary[answer.OptionId.Value].Points;
                    answer.Alias = OptionsDictionary[answer.OptionId.Value].Alias;
                }
            }
		}

		/// <summary>
		/// Get instance data for serialization
		/// </summary>
		/// <returns></returns>
		protected override NameValueCollection GetInstanceDataValuesForSerialization()
		{
			NameValueCollection values = base.GetInstanceDataValuesForSerialization();

			ListOption otherOption = Options.FirstOrDefault(p => p.IsOther);            

			values["otherText"] = OtherText;
			values["otherTextPrompt"] = otherOption == null ? "" : otherOption.Text;

			return values;
		}

		/// <summary>
		/// Get meta data values
		/// </summary>
		/// <returns></returns>
		protected override NameValueCollection GetMetaDataValuesForSerialization()
		{
			NameValueCollection values = base.GetMetaDataValuesForSerialization();

            var connectedFieldName = ProfilePropertiesUpdater.GetConnectedProfileFieldName(ID);

            if (!string.IsNullOrWhiteSpace(connectedFieldName))
                values["ConnectedCustomFieldKey"] = connectedFieldName;

            values["randomizeOptionOrder"] = IsRandomized.ToString();
			values["allowOther"] = AllowOther.ToString();
            

			return values;
		}

        /// <summary>
        /// Write the instance data to include list options.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        public override void WriteXmlInstanceData(XmlWriter writer, bool isText)
		{
			base.WriteXmlInstanceData(writer, isText);

			List<ListOption> listOptions = Options;

			writer.WriteStartElement("listOptions");

			foreach (ListOption option in listOptions)
			{
				option.WriteXml(writer);
			}

			writer.WriteEndElement();
		}

        /// <summary>
        /// Write answers to select items
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        protected override void WriteAnswers(XmlWriter writer, bool isText)
		{
			List<ListOption> listOptions = Options;

			foreach (ListOption option in listOptions)
			{
				if (option.IsSelected)
				{
					WriteOptionAnswer(option, writer);
				}
			}
		}

		/// <summary>
		/// Write the answer for a specific option
		/// </summary>
		/// <param name="option"></param>
		/// <param name="writer"></param>
		protected virtual void WriteOptionAnswer(ListOption option, XmlWriter writer)
		{
			writer.WriteStartElement("answer");
			writer.WriteAttributeString("optionId", option.ID.ToString());

            writer.WriteCData(option.IsOther ? OtherText : string.IsNullOrEmpty(option.Text) ? option.Alias : option.Text);

			writer.WriteEndElement();
		}

    }
}
