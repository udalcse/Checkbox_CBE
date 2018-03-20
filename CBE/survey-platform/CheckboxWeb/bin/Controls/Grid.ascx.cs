using Checkbox.Management;

namespace CheckboxWeb.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Grid : Checkbox.Web.Common.UserControlBase
    {
        public string ItemClickCallback { get; set; }
        public string InitialSortField { get; set; }
        public string ListTemplatePath { get; set; }
        public string ListItemTemplatePath { get; set; }
        public string LoadDataCallback { get; set; }
        public string EmptyGridText { get; set; }
        public string LoadingTextId { get; set; }
        public string RenderCompleteCallback { get; set; }
        public string ArrowStyle { get; set; }
        public string GridCssClass { get; set; }
        public bool UseSimpleLoadingSpinner { get; set; }
        public int ResultsPerPage { get; set; }
        public bool IsAjaxScrollModeEnabled { get; set; }
        public string InitialFilterField { get; set; }
        public string FilterItemType { get; set; }
        public int InitialPageNumber { get; set; }
        public string InitialFilterKey { get; set; }

        /// <summary>
        /// Specify whether grid should be automatically loaded or not.  If delay is set to true, grid
        /// will not be loaded until reload grid method is called.
        /// </summary>
        public bool DelayLoad { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Grid()
        {
            DelayLoad = false;
            LoadingTextId = "/common/loading";
            ArrowStyle = "right";
            UseSimpleLoadingSpinner = false;
            InitialPageNumber = 1;
            InitialFilterKey = string.Empty;

            ResultsPerPage = ApplicationManager.AppSettings.PagingResultsPerPage;
        }

        /// <summary>
        /// Handler to clear the grid
        /// </summary>
        public string ClearGridHandler
        {
            get { return "clearGrid_" + ClientID; }
        }

        /// <summary>
        /// Handler to reload the grid
        /// </summary>
        public string ReloadGridHandler
        {
            get { return "reloadGrid_" + ClientID; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ShowLoadingHandler
        {
            get { return "showLoadingPanel_" + ClientID; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string HideLoadingHandler
        {
            get { return "hideLoadingPanel_" + ClientID; }
        }

        public string ShowSorter
        {
            get { return "showSorter_" + ClientID; }
        }
    }
}