using System;
using System.Data;
using System.Collections;
using System.Xml;

using Prezza.Framework.Logging;

using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Inputs;
using Checkbox.Data.Responses.Filters;
using Checkbox.Management;

#if MULTI_LANG_ENABLED
using Globalization.MultiLingualSupport;
#endif

namespace Checkbox.Analytics.Items
{
	/// <summary>
	/// The MatrixOpenEndedItem summarizes the answers to one or more items by
	/// counting all the repsonses with the same answers.  It outputs 
	/// the total number or responses along with the answer count and
	/// percentage for each unique answer.
	/// </summary>
	public class MatrixOpenEndedItem : AnalysisItem 
	{
		#region Constructors

		/// <summary>
		/// Overloaded. Constructor
		/// </summary>
		public MatrixOpenEndedItem()
		{
			string xPath = "//CodeDependentResources/AnalysisItemTypes/AnalysisItem[@ClassName=\"";
			xPath += this.GetType().Namespace + "." + this.GetType().Name;
			xPath += "\"]";
			base.mTypeID = Convert.ToInt32(XmlResourceManager.GetCodeDependentResourceUsingXPath(xPath).Attributes["ID"].Value);
		}

		/// <summary>
		/// Overloaded. Constructor
		/// </summary>
		/// <param name="itemID"></param>
		public MatrixOpenEndedItem(int itemID):base(itemID)
		{
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override XmlNode EditModeRun()
		{
			//Return some default XML to be visible in the editor

			//Create the basic output XML
			XmlDocument rootDoc = new XmlDocument();
			XmlElement rootNode = rootDoc.CreateElement("SummaryItem");

			string useAlias = this.GetProperty("UseAlias");

			//Loop all the items in the items collection
			foreach (int itemID in this.ItemIDs)
			{

				//Create a hashtable to store existing answers
				Hashtable answerCollection = new Hashtable();
				string totalKey = "SummaryItemAnswerTotal";
				answerCollection.Add(totalKey, 0);

				//Look up the question text for this item
				InputItem sourceItem = (InputItem)Item.GetItem(itemID);
			
				//Create a node for this item
				XmlElement itemElement = rootDoc.CreateElement("SourceItem");
				XmlAttribute itemIDAttribute = rootDoc.CreateAttribute("itemID");
				itemIDAttribute.Value = itemID.ToString();
				itemElement.Attributes.Append(itemIDAttribute);
				XmlAttribute itemTextAttribte = rootDoc.CreateAttribute("itemText");
				itemTextAttribte.Value = sourceItem.Text;
				itemElement.Attributes.Append(itemTextAttribte);
				XmlAttribute itemAliasAttribute = rootDoc.CreateAttribute("itemAlias");
				itemAliasAttribute.Value = sourceItem.Alias;
				itemElement.Attributes.Append(itemAliasAttribute);

				XmlAttribute useAliasAttribute = rootDoc.CreateAttribute("UseAlias");
				useAliasAttribute.Value = useAlias;
				itemElement.Attributes.Append(useAliasAttribute);
				
				//Add the total number of answers as an attribute on the SourceItem element
				int totalAnswers = (int)answerCollection[totalKey];
				answerCollection.Remove(totalKey);
				XmlAttribute totalAttribute = rootDoc.CreateAttribute("totalAnswers");
				totalAttribute.Value = totalAnswers.ToString();
				itemElement.Attributes.Append(totalAttribute);
	
				//Now loop through the unique answers and add their answer count
				//and percent to the output
				foreach(MatrixSet ms in ((SurveyItem)sourceItem).MatrixSets)
				{
					if(ms.Type == MatrixSetType.SingleLineInputs)
					{
						XmlElement setElement = rootDoc.CreateElement("Set");
						XmlAttribute setAnswerName = rootDoc.CreateAttribute("SetName");
						setAnswerName.Value = ms.Text;
						setElement.Attributes.Append(setAnswerName);
						XmlAttribute SetAliasAttribute = rootDoc.CreateAttribute("SetAlias");
						SetAliasAttribute.Value = ms.Alias;
						setElement.Attributes.Append(SetAliasAttribute);

						foreach(Input i in ms.Inputs)
						{
							XmlElement answerElement = rootDoc.CreateElement("Answer");
							XmlAttribute answerTextAttribute = rootDoc.CreateAttribute("answerText");
							answerTextAttribute.Value = i.ID.ToString();
							answerElement.Attributes.Append(answerTextAttribute);
							XmlAttribute aliasAttribute = rootDoc.CreateAttribute("answerAlias");
							aliasAttribute.Value = i.Alias;
							answerElement.Attributes.Append(aliasAttribute);
							setElement.AppendChild(answerElement);
						}
			
						itemElement.AppendChild(setElement);
					}
				}

				foreach (MatrixCategory mc in ((SurveyItem)sourceItem).MatrixCategories)
				{					
					//Add the category element to the XML
					XmlElement catElement = rootDoc.CreateElement("Category");
					XmlAttribute catName = rootDoc.CreateAttribute("CategoryName");
					catName.Value = mc.Text;
					catElement.Attributes.Append(catName);
					XmlAttribute catAliasAttribute = rootDoc.CreateAttribute("CategoryAlias");
					catAliasAttribute.Value = mc.Alias;
					catElement.Attributes.Append(catAliasAttribute);

					foreach(MatrixSet ms in ((SurveyItem)sourceItem).MatrixSets)
					{
						
						if(ms.Type == MatrixSetType.SingleLineInputs)
						{
							XmlElement setAnswerElement = rootDoc.CreateElement("SetAnswers");
							XmlAttribute setAnswerName = rootDoc.CreateAttribute("SetName");
							setAnswerName.Value = ms.Text;
							setAnswerElement.Attributes.Append(setAnswerName);
						
						
							int setAnswerCount = 0;
					
							//add the answer count to the count of answers in the set
                            for(int i = 0; i < ms.Inputs.Count; i++)
							{
								string answerKey = mc.Text + "_" + ms.Text;
								int answerCount = 4;
								setAnswerCount = setAnswerCount + answerCount;
							}

							XmlAttribute setAnswerCountAttribute = rootDoc.CreateAttribute("AnswerCount");
							setAnswerCountAttribute.Value = setAnswerCount.ToString();
							setAnswerElement.Attributes.Append(setAnswerCountAttribute);

							foreach(Input i in ms.Inputs)
							{
								string answerKey = mc.Text + "_" + ms.Text + "_" + i.ID;
								string answerAlias = i.Alias;
								double percent;
								int answerCount = 4;
								if(answerCount != 0)
									percent = ((double)answerCount/(double)setAnswerCount) * 100;
								else
									percent = 0;
					
                    		
								percent = Math.Round(percent, 2);
	
								//Create a node for this answer
								XmlElement answerElement = rootDoc.CreateElement("Answer");
								XmlAttribute answerTextAttribute = rootDoc.CreateAttribute("answerText");
								answerTextAttribute.Value = i.ID.ToString();
								answerElement.Attributes.Append(answerTextAttribute);
								XmlAttribute countAttribute = rootDoc.CreateAttribute("answerCount");
								countAttribute.Value = answerCount.ToString();
								answerElement.Attributes.Append(countAttribute);
								XmlAttribute percentAttribute = rootDoc.CreateAttribute("answerPercent");
								percentAttribute.Value = percent.ToString();
								answerElement.Attributes.Append(percentAttribute);
								XmlAttribute aliasAttribute = rootDoc.CreateAttribute("answerAlias");
								aliasAttribute.Value = answerAlias;
								answerElement.Attributes.Append(aliasAttribute);

								XmlAttribute colPercent = rootDoc.CreateAttribute("ColPercent");
								colPercent.Value = Convert.ToString(100 / ms.Inputs.Count);
								answerElement.Attributes.Append(colPercent);

								//Add this Answer element to the SourceItem element
								setAnswerElement.AppendChild(answerElement);
					
								answerCollection.Remove(answerKey);
							}
							catElement.AppendChild(setAnswerElement);
						}
					}
					itemElement.AppendChild(catElement);
				}

				//Add this SourceItem element to the root node
				rootNode.AppendChild(itemElement);
			}

			//return the complete XML node
			return rootNode;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="responseMetaData"></param>
		/// <returns></returns>
		public override System.Xml.XmlNode Run(System.Data.DataSet responseMetaData)
		{
			Logger.Write("Calling MatrixOpenEndedItem.Run()", "Info");
			DateTime begin = DateTime.Now;

			string useAlias = this.GetProperty("UseAlias");

			Logger.Write("Begin Filter Responses", "Debug");
			//First, filter the response data if this item has filters attached to it
			DataSet responses = null;
			if (this.Filters.Count > 0)
			{
				responses = FilterManager.FilterResponses(responseMetaData, this.Filters);
			}
			else if(this.AnalysisFilters.Count > 0)
			{
				responses = FilterManager.FilterResponses(responseMetaData, this.AnalysisFilters);
			}

			else
			{
				responses = responseMetaData;
			}

			Logger.Write("End Filter Responses", "Debug");

			//Create the basic output XML
			XmlDocument rootDoc = new XmlDocument();
			XmlElement rootNode = rootDoc.CreateElement("SummaryItem");

			Logger.Write("Begin Item loop", "Debug");
			//Loop all the items in the items collection
			foreach (int itemID in this.ItemIDs)
			{
				//Create a hashtable to store existing answers
				Hashtable answerCollection = new Hashtable();
				string totalKey = "SummaryItemAnswerTotal";
				answerCollection.Add(totalKey, 0);

				//Look up the question text for this item
				Logger.Write("Getting Item From Form", "Debug");
				InputItem sourceItem = (InputItem)Item.GetItem(itemID);
				Logger.Write("Done getting Item From Form", "Debug");
			
				//Create a node for this item
				XmlElement itemElement = rootDoc.CreateElement("SourceItem");
				XmlAttribute itemIDAttribute = rootDoc.CreateAttribute("itemID");
				itemIDAttribute.Value = itemID.ToString();
				itemElement.Attributes.Append(itemIDAttribute);
				XmlAttribute itemTextAttribte = rootDoc.CreateAttribute("itemText");
				itemTextAttribte.Value = sourceItem.Text;
				itemElement.Attributes.Append(itemTextAttribte);
				XmlAttribute itemAliasAttribute = rootDoc.CreateAttribute("itemAlias");
				itemAliasAttribute.Value = sourceItem.Alias;
				itemElement.Attributes.Append(itemAliasAttribute);
				

				XmlAttribute useAliasAttribute = rootDoc.CreateAttribute("UseAlias");
				useAliasAttribute.Value = useAlias;
				itemElement.Attributes.Append(useAliasAttribute);

				//Get all the answer rows for thus item
				Logger.Write("Getting answers for Item", "Debug");
				string query = "ItemID = " + itemID + " AND InputType = 2 AND AnswerID IS NOT Null";
				DataRow[] answers = responses.Tables[1].Select(query);
				Logger.Write("Done getting answer for Item", "Debug");

				string answerText = String.Empty;
				string categoryText = String.Empty;
				string setText = String.Empty;

				Hashtable responseCollection = new Hashtable();
				Hashtable setCategoryCollection = new Hashtable();

				Logger.Write("Begin answer loop", "Debug");
				//Loop through all the answers to this item
				foreach (DataRow row in answers)
				{
					/*
					if(row["AnswerID"] == DBNull.Value || row["MatrixSetID"] == DBNull.Value || row["MatrixCategoryID"] == DBNull.Value)
						continue;
						*/

					//Figure out if this is an open ended or option item
					int answerType = Convert.ToInt32(row["InputType"].ToString());

					categoryText = row["CategoryText"].ToString();
					setText = row["SetText"].ToString();
					answerText = row["AnswerText"].ToString();
	
					int setID = 0;
					int categoryID = 0;

					if(row["MatrixSetID"] != System.DBNull.Value)
						setID  = Convert.ToInt32(row["MatrixSetID"].ToString());

					if(row["MatrixCategoryID"] != System.DBNull.Value)
						categoryID = Convert.ToInt32(row["MatrixCategoryID"].ToString());
			

					string key = setID.ToString() + "_" + categoryID.ToString();
					if(setCategoryCollection.ContainsKey(key))
					{
						Hashtable setAnswerCollection = (Hashtable)setCategoryCollection[key];
						setAnswerCollection.Add(row["AnswerID"].ToString(), row["AnswerText"]);
						//setCategoryCollection.Add(key, answerCollection);
						(setCategoryCollection[key]) = setAnswerCollection;
					}
					else
					{
						Hashtable setAnswerCollection = new Hashtable();
						setAnswerCollection.Add(row["AnswerID"].ToString(), row["AnswerText"]);
						setCategoryCollection.Add(key, setAnswerCollection);
					}
					
				}
				Logger.Write("End answer loop", "Debug");

				//Add the total number of answers as an attribute on the SourceItem element
             /*   int totalAnswers = (int)answerCollection[totalKey];
				answerCollection.Remove(totalKey);
				XmlAttribute totalAttribute = rootDoc.CreateAttribute("totalAnswers");
				totalAttribute.Value = totalAnswers.ToString();
				itemElement.Attributes.Append(totalAttribute);*/

				//Now loop through the unique answers and add their answer count
				//and percent to the output
				Logger.Write("Begin matrix set loop", "Debug");
				foreach(MatrixSet ms in ((SurveyItem)sourceItem).MatrixSets)
				{
					if(ms.Type == MatrixSetType.SingleLineInputs)
					{
						XmlElement setElement = rootDoc.CreateElement("Set");
						XmlAttribute setAnswerName = rootDoc.CreateAttribute("SetName");
						setAnswerName.Value = ms.Text;
						setElement.Attributes.Append(setAnswerName);
						XmlAttribute SetAliasAttribute = rootDoc.CreateAttribute("SetAlias");
						SetAliasAttribute.Value = ms.Alias;
						setElement.Attributes.Append(SetAliasAttribute);

						foreach(Input i in ms.Inputs)
						{
							XmlElement answerElement = rootDoc.CreateElement("Answer");
							XmlAttribute answerTextAttribute = rootDoc.CreateAttribute("answerText");
							answerTextAttribute.Value = i.ID.ToString();
							answerElement.Attributes.Append(answerTextAttribute);
							XmlAttribute aliasAttribute = rootDoc.CreateAttribute("answerAlias");
							aliasAttribute.Value = i.Alias;
							answerElement.Attributes.Append(aliasAttribute);
							setElement.AppendChild(answerElement);

						}
			
						itemElement.AppendChild(setElement);
					}
				}
				Logger.Write("End matrix set loop", "Debug");

				Logger.Write("Begin matrix category loop", "Debug");
				foreach (MatrixCategory mc in ((SurveyItem)sourceItem).MatrixCategories)
				{					
					//Add the category element to the XML
					XmlElement catElement = rootDoc.CreateElement("Category");
					XmlAttribute catName = rootDoc.CreateAttribute("CategoryName");
					catName.Value = mc.Text;
					catElement.Attributes.Append(catName);
					XmlAttribute catAliasAttribute = rootDoc.CreateAttribute("CategoryAlias");
					catAliasAttribute.Value = mc.Alias;
					catElement.Attributes.Append(catAliasAttribute);

					foreach(MatrixSet ms in ((SurveyItem)sourceItem).MatrixSets)
					{
						
						if(ms.Type == MatrixSetType.SingleLineInputs)
						{
							XmlElement setAnswerElement = rootDoc.CreateElement("SetAnswers");
							XmlAttribute setAnswerName = rootDoc.CreateAttribute("SetName");
							setAnswerName.Value = ms.Text;
							setAnswerElement.Attributes.Append(setAnswerName);
						
						
							int setAnswerCount = 0;
					
	
							XmlAttribute setAnswerCountAttribute = rootDoc.CreateAttribute("AnswerCount");
							setAnswerCountAttribute.Value = setAnswerCount.ToString();
							setAnswerElement.Attributes.Append(setAnswerCountAttribute);

							if(setCategoryCollection.Contains(ms.ID.ToString() + "_" + mc.ID.ToString()))
							{
								foreach(System.Collections.DictionaryEntry key in ((Hashtable)setCategoryCollection[ms.ID.ToString() + "_" + mc.ID.ToString()]))
								{
								
									
									//Hashtable key2 = (Hashtable)key;
					
									//for (int counter = 0; counter < key.Keys.Count; counter++)		
									//{
									//Create a node for this answer
									XmlElement answerElement = rootDoc.CreateElement("Answer");
									XmlAttribute answerTextAttribute = rootDoc.CreateAttribute("answerText");
									answerTextAttribute.Value = key.Value.ToString();
									answerElement.Attributes.Append(answerTextAttribute);
									
									XmlAttribute colPercent = rootDoc.CreateAttribute("ColPercent");
									colPercent.Value = Convert.ToString(100 / ms.Inputs.Count);
									answerElement.Attributes.Append(colPercent);

									//Add this Answer element to the SourceItem element
									setAnswerElement.AppendChild(answerElement);
					
									//}
						
								}
							}
							catElement.AppendChild(setAnswerElement);
						}
					}
					itemElement.AppendChild(catElement);
				}
				Logger.Write("Begin matrix category loop", "Debug");

				Logger.Write("Begin filter loop", "Debug");
				foreach (Filter filter in this.Filters)
				{
					XmlElement filterElement = rootDoc.CreateElement("Filter");
					XmlAttribute filterName = rootDoc.CreateAttribute("FilterName");
					filterName.Value = filter.Name;
					filterElement.Attributes.Append(filterName);
					itemElement.AppendChild(filterElement);
				}
				Logger.Write("End filter loop", "Debug");
			
				//Add this SourceItem element to the root node
				rootNode.AppendChild(itemElement);
				sourceItem = null;
			}//foreach item in the internal collection
			Logger.Write("End Item loop", "Debug");


			DateTime end = DateTime.Now;
			TimeSpan elapsed = end.Subtract(begin);
			Logger.Write("Exiting MatrixOpenEndedItem.Run(), Elapsed: " + elapsed.TotalMilliseconds, "Info");
			//return the complete XML node
			return rootNode;
		}


#if MULTI_LANG_ENABLED
		public override XmlNode Run(DataSet responseMetaData, string language)
		{
			XmlNode runResult = Run(responseMetaData);
			try
			{
				// update the ItemText, ItemDescription to match the language
				LanguageManager lm = new LanguageManager();
				runResult.SelectSingleNode("SourceItem").Attributes["itemText"].Value = 
					lm.GetText(runResult.SelectSingleNode("SourceItem").Attributes["itemText"].Value, language);

				// update the answer texts
				XmlNodeList answerList = runResult.SelectNodes("SourceItem/Answer");
				for(int i = 0; i < answerList.Count; i++)
				{
					answerList[i].Attributes["answerText"].Value = 
						lm.GetText(answerList[i].Attributes["answerText"].Value, language);
				}
			}
			catch(Exception e)
			{
				return runResult;
			}
			return runResult;
		}

		public override XmlNode EditModeRun(string language)
		{
			XmlNode runResult = EditModeRun();
			try
			{
				// update the ItemText, ItemDescription to match the language
				LanguageManager lm = new LanguageManager();
				runResult.SelectSingleNode("SourceItem").Attributes["itemText"].Value = 
					lm.GetText(runResult.SelectSingleNode("SourceItem").Attributes["itemText"].Value, language);
				// update the answer texts
				XmlNodeList answerList = runResult.SelectNodes("SourceItem/Answer");
				for(int i = 0; i < answerList.Count; i++)
				{
					answerList[i].Attributes["answerText"].Value = 
						lm.GetText(answerList[i].Attributes["answerText"].Value, language);
				}
			}
			catch(Exception e)
			{
				return runResult;
			}

			return runResult;
		}

#endif

		/// <summary>
		/// Records the current answer in the answer collection
		/// </summary>
		/// <param name="answerCollection"></param>
		/// <param name="answer"></param>
		/// <param name="alias"></param>
		/// <param name="category"></param>
		/// <param name="incrementCount"></param>
		/// <param name="initialCount"></param>
		/// <param name="setID"></param>
		/// <param name="setText"></param>
		/// <param name="totalKey"></param>
		private void RecordAnswer(Hashtable answerCollection, string category, string setText, string answer, int setID, string alias, string totalKey, int initialCount, bool incrementCount)
		{
			string answerKey = category + "_" + setText + "_" + answer;
			int answerCount = 0;
			//Add the item to the hashtable if needed, otherwise increment its total
			if (answerCollection.ContainsKey(answerKey))
			{
				answerCount = Convert.ToInt32(((Hashtable)answerCollection[answerKey])["answerCount"]);
				answerCount++;
				((Hashtable)answerCollection[answerKey])["answerCount"] = answerCount;
			}
			else
			{
				//Create a hashtable to store this answer
				Hashtable row = new Hashtable();
				row.Add("answerText",answerKey);
				row.Add("answerAlias", alias);
				row.Add("answerCount", initialCount);
				
				answerCollection.Add(answerKey, row);
			}
			if(!answerCollection.ContainsKey("Set" + setID.ToString() + "Category" + category))
			{
				answerCollection.Add("Set" + setID.ToString() + "Category" + category, 1);
			}

			//Increment the total count
			int totalCount = Convert.ToInt32(answerCollection[totalKey]);
			int setCount = Convert.ToInt32(answerCollection["Set" + setID.ToString() + "Category" + category]);
			if(incrementCount == true)
			{
				if(answerCount > 0 || (answerCollection.ContainsKey(answerKey) && initialCount == 1))
				{
					totalCount++;
					setCount++;

				}
			}
			answerCollection[totalKey] = totalCount;
			answerCollection["Set" + setID.ToString() + "Category" + category] = setCount;
		}

	}
}
