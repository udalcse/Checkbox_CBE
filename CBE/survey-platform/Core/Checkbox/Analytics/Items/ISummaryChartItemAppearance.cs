namespace Checkbox.Analytics.Items
{
    ///<summary>
    ///</summary>
    public interface ISummaryChartItemAppearance
    {
        ///<summary>
        ///</summary>
        ///<param name="prevLegendAlign"></param>
        ///<param name="prevLegendVertAlign"></param>
        void AdjustAutoMarginValues(string prevLegendAlign, string prevLegendVertAlign);
        
        ///<summary>
        ///</summary>
        ///<param name="numberOfSourceItems"></param>
        void AdjustTopMarginForTitle(int numberOfSourceItems);

        ///<summary>
        ///</summary>
        ///<param name="numberOfSourceItems"></param>
        ///<param name="sourceItemId"> </param>
        void AdjustAutoMarginValuesForSpecificItems(int numberOfSourceItems, int? sourceItemId=null);

        ///<summary>
        ///</summary>
        void UpdateWrapTitleChars();
    }
}
