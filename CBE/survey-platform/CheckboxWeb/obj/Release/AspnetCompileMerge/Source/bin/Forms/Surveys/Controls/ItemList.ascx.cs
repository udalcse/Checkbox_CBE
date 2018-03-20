using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Web;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// List control for displaying survey items
    /// </summary>
    public partial class ItemList : Checkbox.Web.Common.UserControlBase
    {
        public bool IncludeSurveyCompatible { get; set; }
        public bool IncludeLibraryCompatible { get; set; }
        public bool IncludeReportCompatible { get; set; }
        public bool IncludeScoreDependable { get; set; }
        public bool IncludeHidden { get; set; }

        private string[] _typeToIncludeArray;
        private string[] _typeToExcludeArray;

        private bool _typesToIncludeUpdated = false;
        private bool _typesToExcludeUpdated = false;

        private string _typesToInclude;
        private string _typesToExclude;

        /// <summary>
        /// 
        /// </summary>
        public string TypesToInclude
        {
            get { return _typesToInclude; }
            set { _typesToInclude = value;
                _typesToIncludeUpdated = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TypesToExclude
        {
            get { return _typesToExclude; }
            set { _typesToExclude = value;
                _typesToExcludeUpdated = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string[] TypeToExcludeArray
        {
            get
            {
                if (_typeToExcludeArray == null || _typesToExcludeUpdated)
                {
                    _typeToExcludeArray = (TypesToExclude ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    _typesToExcludeUpdated = false;
                }

                return _typeToExcludeArray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string[] TypeToIncludeArray
        {
            get
            {
                if (_typeToIncludeArray == null || _typesToIncludeUpdated)
                {
                    _typeToIncludeArray = (TypesToInclude ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    _typesToIncludeUpdated = false;
                }

                return _typeToIncludeArray;
            }
        }

        /// <summary>
        /// Get selected item type
        /// </summary>
        public string SelectedItemType
        {
            get { return _selectedItemType.Text; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ChartType
        {
            get { return _extraData.Text; }
        }

        ///// <summary>
        ///// Get chart type from localized name
        ///// </summary>
        //public string ChartType()
        //{
        //    var chartTypes = new List<string> { "ColumnGraph", "LineGraph", "PieGraph", "BarGraph", "Doughnut" };

        //    foreach (var chartType in chartTypes)
        //    {
        //        if (WebTextManager.GetText(string.Format("/enum/graphtype/{0}", chartType)) == SelectedItemType)
        //            return chartType;
        //    }

        //    return "ColumnGraph";
        //}

        /// <summary>
        /// Default category
        /// </summary>
        public string DefaultCategory { get; set; }

        /// <summary>
        /// Default category
        /// </summary>
        public string DefaultItemType { get; set; }

        /// <summary>
        /// Allowed categories
        /// </summary>
        public string AllowedCategories { get; set; }

        /// <summary>
        /// Allowed items
        /// </summary>
        public string AllowedItems { get; set; }

        /// <summary>
        /// The control that calles ItemList, can be either Library or Survey
        /// </summary>
        public string ItemListTarget { get; set;}

        /// <summary>
        /// Override init to bind events
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //Handle item data bound to add custom attributes for showing preview image
            _categoryList.ItemDataBound += _categoryList_ItemDataBound;
        }

        /// <summary>
        /// Not null if there is only one item type is allowed
        /// </summary>
        public SimpleItemInfo DefaultItemInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _categoryList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var categoryData = e.Item.DataItem as SimpleItemInfo;
            var itemView = e.Item.FindControl("_itemTypeList") as ListView;

            if(categoryData == null || itemView == null)
            {
                e.Item.Visible = false;
                return;
            }

            var categoryInfo = ItemConfigurationManager.ListItemTypes().FirstOrDefault(category => category.CategoryName.Equals(categoryData.Name, StringComparison.InvariantCultureIgnoreCase));

            //
            if(categoryInfo == null)
            {
                e.Item.Visible = false;
                return;   
            }

            var bindList = new List<SimpleItemInfo>();

            foreach (var itemTypeInfo in categoryInfo.ItemTypes.Where(IncludeItem))
            {
                //Special case for sake of usability.  Break chart types into individual charts even
                // though there is only one underlying type.
                if (itemTypeInfo.TypeName.Equals("SummaryChart", StringComparison.InvariantCultureIgnoreCase))
                {
                    var chartTypes = new List<string> { "ColumnGraph", "LineGraph", "PieGraph", "BarGraph", "Doughnut" };

                    bindList.AddRange(chartTypes.Select(chartType => new SimpleItemInfo
                                                                         {
                                                                             Name = itemTypeInfo.TypeName,
                                                                             LocalizedName = WebTextManager.GetText(string.Format("/itemType/{0}/name", chartType)),
                                                                             ExtraData = chartType,
                                                                             Description = GetItemTypeDescription(chartType),
                                                                             PreviewImagePath = GetItemTypePreview(chartType)
                                                                         }));
                }
                else
                {
                    var item = new SimpleItemInfo
                    {
                        Parent = categoryInfo.CategoryName,
                        Name = itemTypeInfo.TypeName,
                        LocalizedName = WebTextManager.GetText(string.Format("/itemType/{0}/name", itemTypeInfo.TypeName)),
                        Description = GetItemTypeDescription(itemTypeInfo.TypeName),
                        PreviewImagePath = GetItemTypePreview(itemTypeInfo.TypeName)
                    };

                    bindList.Add(item);

                    if (DefaultItemType == itemTypeInfo.TypeName)
                        DefaultItemInfo = item;
                }
            }

            if(bindList.Count == 0)
            {
                e.Item.Visible = false;
                return;
            }

            //sort report items
            if (categoryData.Name == "Report")
            {
                bindList = bindList.OrderBy(i => i.LocalizedName).ToList();
            }

            itemView.DataSource = bindList;
            itemView.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private string GetItemTypeDescription(string itemType)
        {
            return WebTextManager.GetText("/itemType/" + itemType + "/description");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private string GetItemTypePreview(string itemType)
        {
            var fileDirectory = Server.MapPath("~/Forms/Surveys/Controls/ItemPreviewImages/");

            var files = Directory.GetFiles(fileDirectory, $"{itemType}.*");

            if (!files.Any())
            {
                System.Diagnostics.Trace.WriteLine($"Preview image for {itemType} not found");

                return string.Empty;
            }
          

            var extension = Path.GetExtension(fileDirectory + files.FirstOrDefault());

            return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/Forms/Surveys/Controls/ItemPreviewImages/" + itemType +  extension;
        }

        /// <summary>
        /// Simple container for displaying hierarchy of categories/items
        /// </summary>
        public class SimpleItemInfo
        {
            public string Parent { get; set; }
            public string Name { get; set; }
            public string LocalizedName { get; set; }
            public string ExtraData { get; set; }
            public bool Expanded { get; set; }
            public string PreviewImagePath { get; set; }
            public string Description { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SimpleItemInfo> ListItemCategories()
        {
            return
                ItemConfigurationManager
                    .ListItemTypes()
                    .Select(
                        categoryInfo => new SimpleItemInfo
                                            {
                                                Name = categoryInfo.CategoryName,
                                                LocalizedName = WebTextManager.GetText(categoryInfo.NameTextId),
                                                Expanded = (categoryInfo.CategoryName == DefaultCategory)
                                            }
                    )
                    .OrderByDescending(info => info.Expanded);
        }

        /// <summary>
        /// Determine whether to include the specified type in the list
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        private bool IncludeItem(ItemTypeInfo typeInfo)
        {
            if (!Utilities.IsNullOrEmpty(AllowedItems) && AllowedItems.IndexOf(typeInfo.TypeName) == -1)
                return false;

            //Special case for email
            if (!ApplicationManager.AppSettings.EmailEnabled
                && ("Email".Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase)
                    || "EmailResponse".Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            //Special case for upload
            if (!ApplicationManager.AppSettings.EnableUploadItem
                && "FileUpload".Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            //Special case for javascript
            if ((!ApplicationManager.AppSettings.EnableJavascriptItem || !ApplicationManager.AppSettings.AllowJavascriptItem)
                && "Javascript".Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            //Special case for scored items
            if (!IncludeScoreDependable && 
                ("CurrentScore".Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase) ||
                "ScoreMessage".Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            if (!ApplicationManager.AppSettings.AllowRatingScaleStatisticsReportItem
                && "StatisticsTable".Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (TypeToExcludeArray.Length > 0 && TypeToExcludeArray.Any
                (t => t.Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            if (TypeToIncludeArray.Length > 0)
            {
                return TypeToIncludeArray.Where(a => a.Equals(typeInfo.TypeName, StringComparison.InvariantCultureIgnoreCase)).Count() > 0;
            }

            if (typeInfo.TypeName.Contains("Hidden") && !IncludeHidden)
                return false;


            return (typeInfo.SurveyCompatible && IncludeSurveyCompatible)
                   || (typeInfo.LibraryCompatible && IncludeLibraryCompatible)
                   || (typeInfo.ReportCompatible && IncludeReportCompatible);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OptionsSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }
    }
}