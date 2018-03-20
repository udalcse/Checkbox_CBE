using System;
using System.Data;
using System.Xml;
using System.Collections.Generic;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class BranchPageActionData : ActionData
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BranchPageActionData()
        {
			ReceiverType = ActionReceiverType.Page;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId">Id of page to branch from.</param>
        public BranchPageActionData(int pageId)
            : base(pageId, ActionReceiverType.Page)
        {
        }

        ///<summary>
        ///</summary>
        public int? GoToPageID { get; set; }

        ///<summary>
        ///</summary>
        public string EditLanguage { get; set; }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="rulesData"></param>
        public override void Load(int actionId, RulesObjectSet rulesData)
        {
            ActionId = actionId;
            
            var action = rulesData.GetBranchAction(actionId);

            if (action != null)
                GoToPageID = action.GoToPageId;
            else
            {
                throw new Exception("Error.This branch has many target pages.");
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteAttributeString("GoToPageID", GoToPageID.HasValue ? GoToPageID.Value.ToString() : "0");
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
		public override void Load(XmlNode xmlNode)
		{
		    GoToPageID = XmlUtility.GetAttributeInt(xmlNode, "GoToPageID");
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rds"></param>
        /// <param name="itemIdMap"></param>
        /// <param name="optionIdMap"></param>
        public override void UpdatItemAndOptionIdMappings(RuleDataService rds, Dictionary<int, int> itemIdMap, Dictionary<int, int> optionIdMap)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rulesData"></param>
        /// <param name="targetPageId"></param>
        public void SetTargetPageId(RulesObjectSet rulesData,int targetPageId)
        {
            GoToPageID = targetPageId;
            var action = rulesData.GetBranchAction(ActionId);
            if (action != null)
                action.GoToPageId = targetPageId;
            else
            {
                throw new Exception("Error.This branch has many target pages.");
            }
        }
        /// <summary>
        /// Get the position of the page to branch to within the survey
        /// </summary>
        public string GoToPagePositionAsString
        {
            get
            {
                //TODO: Go To Page Position

                //if (GoToPageID.HasValue && Context != null)
                //{
                //    TemplatePage page = Context.GetPage(GoToPageID.Value);

                //    if (page != null)
                //    {
                //        if (page.Position == Context.TemplatePages[Context.TemplatePages.Count - 1].Position)
                //        {
                //            return TextManager.GetText("/pageBranch/completeSurvey", EditLanguage);
                //        }

                //        return page.Position.ToString();
                //    }
                //}

                return string.Empty;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //TODO: ToString()
            return string.Empty;

            //if (Context.TemplatePages.IndexOf(Context.GetPage(GoToPageID.Value)) == (Context.TemplatePages.Count - 1))
            //{
            //    return TextManager.GetText("/pageBranch/completeSurvey", EditLanguage);
            //}
            //else
            //{
            //    builder.Append(TextManager.GetText("/pageBranch/goToPage", EditLanguage));
            //    builder.Append(" ");
            //    if (GoToPageID.HasValue)
            //        builder.Append(Context.GetPage(GoToPageID.Value).Position);
            //}

            //return builder.ToString();
        }
    }
}
