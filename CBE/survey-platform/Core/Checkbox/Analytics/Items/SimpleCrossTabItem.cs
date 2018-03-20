using System;
using System.Data;
using System.Collections;
using System.Xml;

using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Inputs;
using Checkbox.Data.Responses.Filters;
using Checkbox.Management;

using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items
{
	/// <summary>
	/// Displays two option-type items along two axes of a table
	/// in each cell of the table is the percentage of respondents
	/// who selected both of those options
	/// </summary>
	public class SimpleCrossTabItem : AnalysisItem 
	{
		#region Constructors

		/// <summary>
		/// Overloaded. Constructor
		/// </summary>
		public SimpleCrossTabItem()
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
		public SimpleCrossTabItem(int itemID):base(itemID)
		{
		}

		#endregion

		#region AnalysisItem methods

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public  override XmlNode EditModeRun()
		{

			string otherBehavior = String.Empty;
			string useAlias = String.Empty;
			if (this.GetProperty("OtherBehavior") != null)
				otherBehavior = this.GetProperty("OtherBehavior");
			if (this.GetProperty("UseAlias") != null)
				useAlias = this.GetProperty("UseAlias");


			//Create the basic output XML
			XmlDocument rootDoc = new XmlDocument();
			XmlElement rootNode = rootDoc.CreateElement("SimpleCrossTabItem");

			//The first item in the collection is the independent item
			InputItem independentItem = (InputItem)Item.GetItem(Convert.ToInt32(this.ItemIDs[0]));

			//Create a node for this item
			XmlElement independentElement = rootDoc.CreateElement("IndependentItem");

			XmlAttribute itemIDAttribute = rootDoc.CreateAttribute("itemID");
			itemIDAttribute.Value = independentItem.ID.ToString();
			independentElement.Attributes.Append(itemIDAttribute);

			XmlAttribute itemTextAttribte = rootDoc.CreateAttribute("itemText");
			itemTextAttribte.Value = independentItem.Text;
			independentElement.Attributes.Append(itemTextAttribte);

			XmlAttribute itemAliasAttribute = rootDoc.CreateAttribute("itemAlias");
			itemAliasAttribute.Value = independentItem.Alias;
			independentElement.Attributes.Append(itemAliasAttribute);	
			
			XmlAttribute useAliasAttribute = rootDoc.CreateAttribute("UseAlias");
			useAliasAttribute.Value = useAlias;
			independentElement.Attributes.Append(useAliasAttribute);

			rootNode.AppendChild(independentElement);

			//The second input item is the dependent item
			InputItem dependentItem = (InputItem)Item.GetItem(Convert.ToInt32(this.ItemIDs[1]));

			//Create a node for this item
			XmlElement dependentElement = rootDoc.CreateElement("DependentItem");

			XmlAttribute itemIDAttribute2 = rootDoc.CreateAttribute("itemID");
			itemIDAttribute2.Value = dependentItem.ID.ToString();
			dependentElement.Attributes.Append(itemIDAttribute);

			XmlAttribute itemTextAttribute2 = rootDoc.CreateAttribute("itemText");
			itemTextAttribute2.Value = dependentItem.Text;
			dependentElement.Attributes.Append(itemTextAttribute2);

			XmlAttribute itemAliasAttribute2 = rootDoc.CreateAttribute("itemAlias");
			itemAliasAttribute2.Value = dependentItem.Alias;
			dependentElement.Attributes.Append(itemAliasAttribute2);
				
			XmlAttribute useAliasAttribute2 = rootDoc.CreateAttribute("UseAlias");
			useAliasAttribute2.Value = useAlias;
			dependentElement.Attributes.Append(useAliasAttribute2);

			//Add this node to the root node
			rootNode.AppendChild(dependentElement);
			
			//Loop the inputs in the independent item 
			foreach(Input independentInput in independentItem.Inputs)
			{
				//Create a child node under the independent item for each answer
				int totalCount = dependentItem.Inputs.Count;
				
				XmlElement answerElement = rootDoc.CreateElement("IndependentAnswer");
				XmlAttribute answerTextAttribute = rootDoc.CreateAttribute("answerText");
				answerTextAttribute.Value = independentInput.Text;
				answerElement.Attributes.Append(answerTextAttribute);
				XmlAttribute answerIDttribute = rootDoc.CreateAttribute("answerID");
				answerIDttribute.Value = independentInput.ID.ToString();
				answerElement.Attributes.Append(answerIDttribute);
				XmlAttribute countAttribute = rootDoc.CreateAttribute("answerCount");
				countAttribute.Value = totalCount.ToString();
				answerElement.Attributes.Append(countAttribute);
				//XmlAttribute percentAttribute = rootDoc.CreateAttribute("answerPercent");
				//percentAttribute.Value = percent.ToString();
				//answerElement.Attributes.Append(percentAttribute);
				XmlAttribute aliasAttribute = rootDoc.CreateAttribute("answerAlias");
				aliasAttribute.Value = independentInput.Alias;;
				answerElement.Attributes.Append(aliasAttribute);

				//Add this Answer element to the SourceItem element
				independentElement.AppendChild(answerElement);
			}

			//Now loop the dependent inputs
			//For each dependent input, create a dependentanswer node
			//and create an intersection node under the independent answers
			foreach(Input dependentInput in dependentItem.Inputs)
			{
				int totalCount = independentItem.Inputs.Count;

				//Create a node for this answer
				XmlElement answerElement = rootDoc.CreateElement("DependentAnswer");
				XmlAttribute answerTextAttribute = rootDoc.CreateAttribute("answerText");
				answerTextAttribute.Value = dependentInput.Text;
				answerElement.Attributes.Append(answerTextAttribute);
				XmlAttribute answerIDttribute = rootDoc.CreateAttribute("answerID");
				answerIDttribute.Value = dependentInput.ID.ToString();
				answerElement.Attributes.Append(answerIDttribute);
				XmlAttribute countAttribute = rootDoc.CreateAttribute("answerCount");
				countAttribute.Value = totalCount.ToString();
				answerElement.Attributes.Append(countAttribute);
				//XmlAttribute percentAttribute = rootDoc.CreateAttribute("answerPercent");
				//percentAttribute.Value = percent.ToString();
				//answerElement.Attributes.Append(percentAttribute);
				XmlAttribute aliasAttribute = rootDoc.CreateAttribute("answerAlias");
				aliasAttribute.Value = dependentInput.Alias;
				answerElement.Attributes.Append(aliasAttribute);

				//Add this Answer element to the dependentElement element
				dependentElement.AppendChild(answerElement);

				//Loop the independent nodes to add the intersection nodes
				foreach (XmlElement childElement in independentElement.ChildNodes)
				{
					//Add this count as a child node to the current independent answer
					XmlElement intersectionElement = rootDoc.CreateElement("DependentAnswerIntersection");
					XmlAttribute intersectionIDAttribute = rootDoc.CreateAttribute("dependentAnswerID");
					intersectionIDAttribute.Value = dependentInput.ID.ToString();
					intersectionElement.Attributes.Append(intersectionIDAttribute);
					intersectionElement.InnerText = "1";
					childElement.AppendChild(intersectionElement);
				}
			}

			//rootDoc.AppendChild(rootNode);
			//rootDoc.Save(@"d:\crosstab.xml");
			return rootNode;
			
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="responseMetaData"></param>
		/// <returns></returns>
		public override XmlNode Run(System.Data.DataSet responseMetaData)
		{
			string otherBehavior = String.Empty;
			string useAlias = String.Empty;
			if (this.GetProperty("OtherBehavior") != null)
				otherBehavior = this.GetProperty("OtherBehavior");
			if (this.GetProperty("UseAlias") != null)
				useAlias = this.GetProperty("UseAlias");
			
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

			//Create the basic output XML
			XmlDocument rootDoc = new XmlDocument();
			XmlElement rootNode = rootDoc.CreateElement("SimpleCrossTabItem");

			//Add the filter node to the root node
			XmlElement filtersElement = rootDoc.CreateElement("Filters");
			foreach (Filter filter in this.Filters)
			{
				XmlElement filterElement = rootDoc.CreateElement("Filter");
				XmlAttribute filterName = rootDoc.CreateAttribute("FilterName");
				filterName.Value = filter.Name;
				filterElement.Attributes.Append(filterName);
				filtersElement.AppendChild(filterElement);
			}
			rootNode.AppendChild(filtersElement);

			//The first item in the collection is the independent item
			//subsequent items are dependent
			InputItem independentItem = (InputItem)Item.GetItem(Convert.ToInt32(this.ItemIDs[0]));

			//Create a hashtable to store existing answers
			Hashtable independentCollection = new Hashtable();
			string totalKey = "SimpleCrossTabItemItemAnswerTotal";
			independentCollection.Add(totalKey, 0);

			//Create a node for this item
			XmlElement independentElement = rootDoc.CreateElement("IndependentItem");

			XmlAttribute itemIDAttribute = rootDoc.CreateAttribute("itemID");
			itemIDAttribute.Value = independentItem.ID.ToString();
			independentElement.Attributes.Append(itemIDAttribute);

			XmlAttribute itemTextAttribte = rootDoc.CreateAttribute("itemText");
			itemTextAttribte.Value = independentItem.Text;
			independentElement.Attributes.Append(itemTextAttribte);

			XmlAttribute itemAliasAttribute = rootDoc.CreateAttribute("itemAlias");
			itemAliasAttribute.Value = independentItem.Alias;
			independentElement.Attributes.Append(itemAliasAttribute);
				
			XmlAttribute useAliasAttribute = rootDoc.CreateAttribute("UseAlias");
			useAliasAttribute.Value = useAlias;
			independentElement.Attributes.Append(useAliasAttribute);

			//Add this element to the root doc
			rootNode.AppendChild(independentElement);
			
			//Get all the answer rows for this item
			DataRow[] independentAnswers = responses.Tables[1].Select("ItemID = " + independentItem.ID.ToString());

			//Hashtable responseCollection = new Hashtable();

			//Loop through all the answers to this item
			foreach (DataRow row in independentAnswers)
			{
				
				int answerID = Convert.ToInt32(row["OptionID"]);
				string answerText = row["OptionText"].ToString();		
				string alias = row["AnswerAlias"].ToString();

				RecordAnswer(independentCollection, answerText, answerID, alias, totalKey);
			}

			//Add the total number of answers as an attribute on the SourceItem element
			//int totalAnswers = (int)answerCollection[totalKey];
			independentCollection.Remove(totalKey);
			//XmlAttribute totalAttribute = rootDoc.CreateAttribute("totalAnswers");
			//totalAttribute.Value = totalAnswers.ToString();
			//itemElement.Attributes.Append(totalAttribute);

			//Now loop through the unique answers to the independent item
			//And create a node for each one with the total number of responses
			foreach (Input input in independentItem.Inputs)
			{					
				int answerCount;
				if(independentCollection.ContainsKey(input.Text))
				{
					answerCount = Convert.ToInt32(((Hashtable)independentCollection[input.Text])["answerCount"].ToString());
				}
				else
				{
					answerCount = 0;
				}
				int answerID = input.ID;
				string answerAlias = input.Alias;
					//float percent = ((float)answerCount/(float)totalAnswers) * 100;
					
					//Create a node for this answer
					XmlElement answerElement = rootDoc.CreateElement("IndependentAnswer");

					XmlAttribute answerTextAttribute = rootDoc.CreateAttribute("answerText");
					answerTextAttribute.Value = input.Text.ToString();
					answerElement.Attributes.Append(answerTextAttribute);

					XmlAttribute answerIDttribute = rootDoc.CreateAttribute("answerID");
					answerIDttribute.Value = answerID.ToString();
					answerElement.Attributes.Append(answerIDttribute);

					XmlAttribute countAttribute = rootDoc.CreateAttribute("answerCount");
					countAttribute.Value = answerCount.ToString();
					answerElement.Attributes.Append(countAttribute);

					//XmlAttribute percentAttribute = rootDoc.CreateAttribute("answerPercent");
					//percentAttribute.Value = percent.ToString();
					//answerElement.Attributes.Append(percentAttribute);

					XmlAttribute aliasAttribute = rootDoc.CreateAttribute("answerAlias");
					aliasAttribute.Value = answerAlias;
					answerElement.Attributes.Append(aliasAttribute);

					//Add this Answer element to the SourceItem element
					independentElement.AppendChild(answerElement);
				

			}//foreach answerkey in hashtable


			//Add the independent item element to the root node
			rootNode.AppendChild(independentElement);


			/***** Dependent Item *****/

			//The second input item is the dependent item
			InputItem dependentItem = (InputItem)Item.GetItem(Convert.ToInt32(this.ItemIDs[1]));

			//Create a hashtable to store existing answers
			Hashtable dependentCollection = new Hashtable();
			dependentCollection.Add(totalKey, 0);

			//Create a node for this item
			XmlElement dependentElement = rootDoc.CreateElement("DependentItem");

			XmlAttribute itemIDAttribute2 = rootDoc.CreateAttribute("itemID");
			itemIDAttribute2.Value = dependentItem.ID.ToString();
			dependentElement.Attributes.Append(itemIDAttribute2);

			XmlAttribute itemTextAttribute2 = rootDoc.CreateAttribute("itemText");
			itemTextAttribute2.Value = dependentItem.Text;
			dependentElement.Attributes.Append(itemTextAttribute2);

			XmlAttribute itemAliasAttribute2 = rootDoc.CreateAttribute("itemAlias");
			itemAliasAttribute2.Value = dependentItem.Alias;
			dependentElement.Attributes.Append(itemAliasAttribute2);
				
			XmlAttribute useAliasAttribute2 = rootDoc.CreateAttribute("UseAlias");
			useAliasAttribute2.Value = useAlias;
			dependentElement.Attributes.Append(useAliasAttribute2);

			//Add this node to the root node
			rootNode.AppendChild(dependentElement);
			
			//Get all the answer rows for this item
			DataRow[] dependentAnswers = responses.Tables[1].Select("ItemID = " + dependentItem.ID.ToString());

			//Loop through all the answers to this item
			foreach (DataRow row in dependentAnswers)
			{
				int answerID = Convert.ToInt32(row["OptionID"]);
				string answerText = row["OptionText"].ToString();		
				string alias = row["AnswerAlias"].ToString();

				RecordAnswer(dependentCollection, answerText, answerID, alias, totalKey);
			}

			dependentCollection.Remove(totalKey);

			//Now loop through the unique answers to the dependent item
			//And create a node for each one with the total number of responses
			//Then go through the independent collection and determine the intersection values
			foreach (Input input in dependentItem.Inputs)
			{					
				int answerCount;
				if(dependentCollection.ContainsKey(input.Text))
				{
					answerCount = Convert.ToInt32(((Hashtable)dependentCollection[input.Text])["answerCount"].ToString());
				}
				else
				{
					answerCount = 0;
				}

					int answerID = input.ID;
					string answerAlias = input.Alias;
					//float percent = ((float)answerCount/(float)totalAnswers) * 100;
					
					//Create a node for this answer
					XmlElement answerElement = rootDoc.CreateElement("DependentAnswer");

					XmlAttribute answerTextAttribute = rootDoc.CreateAttribute("answerText");
					answerTextAttribute.Value = input.Text.ToString();
					answerElement.Attributes.Append(answerTextAttribute);

					XmlAttribute answerIDttribute = rootDoc.CreateAttribute("answerID");
					answerIDttribute.Value = answerID.ToString();
					answerElement.Attributes.Append(answerIDttribute);

					XmlAttribute countAttribute = rootDoc.CreateAttribute("answerCount");
					countAttribute.Value = answerCount.ToString();
					answerElement.Attributes.Append(countAttribute);

					//XmlAttribute percentAttribute = rootDoc.CreateAttribute("answerPercent");
					//percentAttribute.Value = percent.ToString();
					//answerElement.Attributes.Append(percentAttribute);

					XmlAttribute aliasAttribute = rootDoc.CreateAttribute("answerAlias");
					aliasAttribute.Value = answerAlias;
					answerElement.Attributes.Append(aliasAttribute);

					//Add this Answer element to the dependentElement element
					dependentElement.AppendChild(answerElement);


					//Now loop the independent collection and find the intersection value for each combination
					foreach (XmlElement childElement in independentElement.ChildNodes)
					{
						int independentItemID = independentItem.ID;
						int independentAnswerID = Convert.ToInt32(childElement.Attributes["answerID"].Value);
						int dependentItemID = dependentItem.ID;
						int dependentAnswerID = answerID;

						DataTable intersectionData = DbWrapper.ExecuteDataSet("rsp_GetCrossTabIntersection",
							DbWrapper.GetParameter(independentItemID),
							DbWrapper.GetParameter(independentAnswerID),
							DbWrapper.GetParameter(dependentItemID),
							DbWrapper.GetParameter(dependentAnswerID)).Tables[0];
					
						int intersectionCount = Convert.ToInt32(intersectionData.Rows[0][0].ToString());

						//Add this count as a child node to the current independent answer
						XmlElement intersectionElement = rootDoc.CreateElement("DependentAnswerIntersection");

						XmlAttribute intersectionIDAttribute = rootDoc.CreateAttribute("dependentAnswerID");
						intersectionIDAttribute.Value = dependentAnswerID.ToString();
						intersectionElement.Attributes.Append(intersectionIDAttribute);

						intersectionElement.InnerText = intersectionCount.ToString();
						childElement.AppendChild(intersectionElement);
					}
			

			}//foreach answerkey in hashtable


			foreach (Filter filter in this.Filters)
			{
				XmlElement filterElement = rootDoc.CreateElement("Filter");
				XmlAttribute filterName = rootDoc.CreateAttribute("FilterName");
				filterName.Value = filter.Name;
				filterElement.Attributes.Append(filterName);
				rootNode.AppendChild(filterElement);
			}


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


		#endregion


		/// <summary>
		/// Records the current answer in the answer collection
		/// </summary>
		/// <param name="answerCollection"></param>
		/// <param name="answer"></param>/
		/// <param name="alias"></param>
		/// <param name="answerID"></param>
		/// <param name="totalKey"></param>
		private void RecordAnswer(Hashtable answerCollection, string answer, int answerID, string alias, string totalKey)
		{
			
			//Add the item to the hashtable if needed, otherwise increment its total
			if (answerCollection.ContainsKey(answer))
			{
				int answerCount = Convert.ToInt32(((Hashtable)answerCollection[answer])["answerCount"]);
				answerCount++;
				((Hashtable)answerCollection[answer])["answerCount"] = answerCount;
			}
			else
			{
				//Create a hashtable to store this answer
				Hashtable row = new Hashtable();
				row.Add("answerText",answer);
				row.Add("answerAlias", alias);
				row.Add("answerCount", 1);
				row.Add("answerID", answerID);
				
				answerCollection.Add(answer, row);
			}

			//Increment the total count
			int totalCount = Convert.ToInt32(answerCollection[totalKey]);
			totalCount++;
			answerCollection[totalKey] = totalCount;
		}
		

	}
}
