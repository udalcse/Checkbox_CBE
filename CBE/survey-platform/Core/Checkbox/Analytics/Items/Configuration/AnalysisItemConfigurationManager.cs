using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Security.Principal;

namespace Checkbox.Analytics.Items.Configuration
{
    ///<summary>
    ///</summary>
    public static class AnalysisItemConfigurationManager
    {
        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<param name="responseTemplateId"></param>
        ///<param name="options"></param>
        ///<param name="chartStyleAppearance"></param>
        ///<returns></returns>
        public static AnalysisItemData GetTextItem(ItemData item, int responseTemplateId, AnalysisWizardOptions options, AppearanceData chartStyleAppearance, CheckboxPrincipal principal)
        {
            if (options.GraphType == "DoNotShow")
                return null;
             
            AnalysisItemData newItemData = options.GraphType == "Details" 
                ? GetDetailsItem(item, responseTemplateId, options, principal) 
                : GetFrequencyItem(item, responseTemplateId, options, chartStyleAppearance, principal);

            return newItemData;
        }

        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<param name="responseTemplateId"></param>
        ///<param name="options"></param>
        ///<returns></returns>
        public static AnalysisItemData GetHiddenItem(ItemData item, int responseTemplateId, AnalysisWizardOptions options, CheckboxPrincipal principal)
        {
            if (options.GraphType == "DoNotShow")
                return null;

            AnalysisItemData newItemData = options.GraphType == "Details" 
                ? GetDetailsItem(item, responseTemplateId, options, principal) 
                : GetSummaryTable(item, responseTemplateId, options, principal);

            return newItemData;
        }

        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<param name="responseTemplateId"></param>
        ///<param name="options"></param>
        ///<returns></returns>
        public static AnalysisItemData GetMatrixItem(ItemData item, int responseTemplateId, AnalysisWizardOptions options, CheckboxPrincipal principal)
        {
            if (options.GraphType == "DoNotShow")
                return null;

            AnalysisItemData newItemData = GetMatrixSummary(item, responseTemplateId, options, principal);

            return newItemData;
        }

        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<param name="responseTemplateId"></param>
        ///<param name="options"></param>
        ///<param name="chartStyleAppearance"></param>
        ///<returns></returns>
        public static AnalysisItemData GetSelectItem(ItemData item, int responseTemplateId, AnalysisWizardOptions options, AppearanceData chartStyleAppearance, CheckboxPrincipal principal)
        {
            if (options.GraphType == "DoNotShow")
                return null;

            AnalysisItemData newItemData;
            if(options.GraphType == "Details")
                newItemData = GetDetailsItem(item, responseTemplateId, options, principal);
            else if(options.GraphType == "StatisticsTable")
                newItemData = GetStatisticsItem(item, responseTemplateId, options, principal);
            else if (options.GraphType == "NetPromoterScoreTable")
                newItemData = GetNetPromoterScoreTable(item, responseTemplateId, options, principal);
            else if (options.GraphType == "NetPromoterScoreStatisticsTable")
                newItemData = GetNetPromoterScoreStatisticsTable(item, responseTemplateId, options, principal);
            else
                newItemData = GetFrequencyItem(item, responseTemplateId, options, chartStyleAppearance, principal);

            return newItemData;
        }

        #region Survey_Report_Map

        private static AnalysisItemData GetStatisticsItem(ItemData item, int responseTemplateId, AnalysisWizardOptions options, CheckboxPrincipal principal)
        {
            var itemData = (StatisticsItemData)ItemConfigurationManager.CreateConfigurationData("StatisticsTable");

            if (itemData != null)
            {
                itemData.ReportOption = StatisticsItemReportingOption.All;

                itemData.AddResponseTemplate(responseTemplateId);
                itemData.AddSourceItem(item.ID.Value);

                //Create/save appearance data
                AppearanceData appearanceData = AppearanceDataManager.GetDefaultAppearanceDataForType(itemData.ItemTypeID);

                var appearance = appearanceData as StatisticsItemAppearanceData;
                appearance.SetDefaults();

                if (options.UseAliases)
                {
                    itemData.Alias = item.Alias;
                    itemData.UseAliases = true;
                }

                appearanceData.SetPropertyValue("ItemPosition", options.ItemPostion);

                itemData.CreatedBy = principal.Identity.Name;
                itemData.Save();
                appearanceData.Save(itemData.ID.Value);
            }

            return itemData;
        }

        private static AnalysisItemData GetNetPromoterScoreStatisticsTable(ItemData item, int responseTemplateId, AnalysisWizardOptions options, CheckboxPrincipal principal)
        {
            var itemData = (NetPromoterScoreStatisticsItemData)ItemConfigurationManager.CreateConfigurationData(options.GraphType);

            if (itemData != null)
            {
                itemData.AddResponseTemplate(responseTemplateId);
                itemData.AddSourceItem(item.ID.Value);

                //Create/save appearance data
                AppearanceData appearanceData = AppearanceDataManager.GetDefaultAppearanceDataForType(itemData.ItemTypeID);
                appearanceData.SetDefaults();

                if (options.UseAliases)
                {
                    itemData.Alias = item.Alias;
                    itemData.UseAliases = true;
                }

                appearanceData.SetPropertyValue("ItemPosition", options.ItemPostion);

                itemData.CreatedBy = principal.Identity.Name;
                itemData.Save();
                appearanceData.Save(itemData.ID.Value);
            }

            return itemData;
        }

        private static AnalysisItemData GetNetPromoterScoreTable(ItemData item, int responseTemplateId, AnalysisWizardOptions options, CheckboxPrincipal principal)
        {
            var itemData = (NetPromoterScoreItemData)ItemConfigurationManager.CreateConfigurationData(options.GraphType);

            if (itemData != null)
            {
                itemData.AddResponseTemplate(responseTemplateId);
                itemData.AddSourceItem(item.ID.Value);

                //Create/save appearance data
                AppearanceData appearanceData = AppearanceDataManager.GetDefaultAppearanceDataForType(itemData.ItemTypeID);
                appearanceData.SetDefaults();

                if (options.UseAliases)
                {
                    itemData.Alias = item.Alias;
                    itemData.UseAliases = true;
                }

                appearanceData.SetPropertyValue("ItemPosition", options.ItemPostion);

                itemData.CreatedBy = principal.Identity.Name;
                itemData.Save();
                appearanceData.Save(itemData.ID.Value);
            }

            return itemData;
        }

        private static AnalysisItemData GetDetailsItem(ItemData item, int responseTemplateId, AnalysisWizardOptions options, CheckboxPrincipal principal)
        {
            AnalysisItemData newItem = (DetailsItemData)ItemConfigurationManager.CreateConfigurationData("Details");
            DetailsItemAppearanceData newAppearanceData = (DetailsItemAppearanceData)AppearanceDataManager.GetDefaultAppearanceDataForType(newItem.ItemTypeID);

            newItem.AddResponseTemplate(responseTemplateId);
            newItem.AddSourceItem(item.ID.Value);

            ((DetailsItemData)newItem).GroupAnswers = true;
            ((DetailsItemData)newItem).LinkToResponseDetails = true;

            if (options.UseAliases)
            {
                newItem.Alias = item.Alias;
                newItem.UseAliases = true;
            }

            newAppearanceData.ItemPosition = options.ItemPostion;

            newItem.CreatedBy = principal.Identity.Name;
            newItem.Save();
            newAppearanceData.Save(newItem.ID.Value);
            return newItem;
        }

        private static AnalysisItemData GetSummaryTable(ItemData item, int responseTemplateId, AnalysisWizardOptions options, CheckboxPrincipal principal)
        {
            var itemData = (FrequencyItemData)ItemConfigurationManager.CreateConfigurationData("Frequency");
            FrequencyItemAppearanceData newAppearanceData = (FrequencyItemAppearanceData)AppearanceDataManager.GetDefaultAppearanceDataForType(itemData.ItemTypeID);
            newAppearanceData.SetPropertyValue("GraphType", GetGraphType("SummaryTable"));

            itemData.AddResponseTemplate(responseTemplateId);
            itemData.AddSourceItem(item.ID.Value);

            if (options.UseAliases)
            {
                itemData.Alias = item.Alias;
                itemData.UseAliases = true;
            }

            newAppearanceData.ItemPosition = options.ItemPostion;

            itemData.CreatedBy = principal.Identity.Name;
            itemData.Save();
            newAppearanceData.Save(itemData.ID.Value);
            return itemData;
        }

        /// <summary>
        /// Creates a new report item for the one item in the survey
        /// 
        /// Note: ugly code, must be rewritten.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="options"></param>
        /// <param name="chartStyleAppearance"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        private static AnalysisItemData GetFrequencyItem(ItemData item, int responseTemplateId, AnalysisWizardOptions options, AppearanceData chartStyleAppearance, CheckboxPrincipal principal)
        {
            GraphType graphType = GetGraphType(item, options);

            string itemType = item is RankOrderItemData ? "RankOrderSummary" : "SummaryChart";
            
            if (graphType == GraphType.SummaryTable)
            {
                itemType = item is RankOrderItemData ? "RankOrderSummaryTable" : "Frequency";
            }

            AnalysisItemData newItem = (AnalysisItemData)ItemConfigurationManager.CreateConfigurationData(itemType);

            AppearanceData newAppearanceData;

            if (itemType == "SummaryChart" && chartStyleAppearance != null)
            {
                newAppearanceData = chartStyleAppearance.Copy();
            }
            else
            {
                newAppearanceData = AppearanceDataManager.GetDefaultAppearanceDataForType(newItem.ItemTypeID);
            }

            if (newAppearanceData is ISummaryChartItemAppearance)
            {
                (newAppearanceData as ISummaryChartItemAppearance).AdjustTopMarginForTitle(1);
                (newAppearanceData as ISummaryChartItemAppearance).AdjustAutoMarginValuesForSpecificItems(1);
                (newAppearanceData as ISummaryChartItemAppearance).UpdateWrapTitleChars();
                (newAppearanceData as ISummaryChartItemAppearance).AdjustAutoMarginValues("center", "bottom");
            }

            newAppearanceData.SetPropertyValue("GraphType", graphType);

            if (graphType == GraphType.PieGraph || graphType == GraphType.Doughnut)
            {
                newAppearanceData.SetPropertyValue("ShowLegend", true.ToString());
            }

            newItem.AddResponseTemplate(responseTemplateId);
            newItem.AddSourceItem(item.ID.Value);

            if (options.UseAliases)
            {
                newItem.Alias = item.Alias;
                newItem.UseAliases = true;
            }
            if (newItem is FrequencyItemData)
            {
                (newItem as FrequencyItemData).DisplayStatistics = options.DisplayStatistics;
                (newItem as FrequencyItemData).DisplayAnswers = options.DisplayAnswers;
            }

            newAppearanceData.SetPropertyValue("ItemPosition", options.ItemPostion);
            newAppearanceData.SetPropertyValue("OptionsOrder", "Survey");

            newItem.CreatedBy = principal.Identity.Name;
            newItem.Save();
            newAppearanceData.Save(newItem.ID.Value);
            return newItem;
        }

        private static AnalysisItemData GetMatrixSummary(ItemData item, int responseTemplateId, AnalysisWizardOptions options, CheckboxPrincipal principal)
        {
            AnalysisItemData newItemData = null;

            if (options.GraphType == "MatrixSummary")
            {
                //Only add the matrix item if it has non-singlelinetext columns
                bool addItem = false;

                for (int matrixColumn = 1; matrixColumn <= ((MatrixItemData)item).ColumnCount; matrixColumn++)
                {
                    if (matrixColumn != ((MatrixItemData)item).PrimaryKeyColumnIndex)
                    {
                        int prototypeId = ((MatrixItemData)item).GetColumnPrototypeId(matrixColumn);
                        ItemData columnPrototype  = ItemConfigurationManager.GetConfigurationData(prototypeId);

                        //Special case since matrix sum total is an SL text
                        if (columnPrototype is MatrixSumTotalItemData || !(columnPrototype is SingleLineTextItemData))
                        {
                            addItem = true;
                            break;
                        }
                    }
                }

                if (addItem)
                {
                    newItemData = (MatrixSummaryItemData)ItemConfigurationManager.CreateConfigurationData("MatrixSummary");
                    MatrixSummaryItemAppearanceData newAppearanceData = (MatrixSummaryItemAppearanceData)AppearanceDataManager.GetDefaultAppearanceDataForType(newItemData.ItemTypeID);

                    newItemData.AddResponseTemplate(responseTemplateId);
                    ((MatrixSummaryItemData)newItemData).MatrixSourceItem = item.ID.Value;

                    if (options.UseAliases)
                    {
                        newItemData.Alias = item.Alias;
                        newItemData.UseAliases = true;
                    }

                    newAppearanceData.ItemPosition = options.ItemPostion;

                    newItemData.CreatedBy = principal.Identity.Name;
                    newItemData.Save();
                    newAppearanceData.Save(newItemData.ID.Value);
                }
            }
            return newItemData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static GraphType GetGraphType(ItemData itemData, AnalysisWizardOptions options)
        {
            if (!(itemData is SelectItemData))
            {
                return GraphType.SummaryTable;
            }
           
            if (((SelectItemData)itemData).Options.Count > options.MaxOptions)
                return GraphType.SummaryTable;

            return GetGraphType(options.GraphType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphType"></param>
        /// <returns></returns>
        private static GraphType GetGraphType(string graphType)
        {
            switch (graphType)
            {
                case "SummaryTable":
                    return GraphType.SummaryTable;
                case "StatisticsTable":
                    return GraphType.StatisticsTable;
                case "ColumnGraph":
                    return GraphType.ColumnGraph;
                case "PieGraph":
                    return GraphType.PieGraph;
                case "BarGraph":
                    return GraphType.BarGraph;
                case "LineGraph":
                    return GraphType.LineGraph;
                case "DoughnutGraph":
                    return GraphType.Doughnut;
                default:
                    return GraphType.SummaryTable;
            }
        }
        #endregion
    }
}
