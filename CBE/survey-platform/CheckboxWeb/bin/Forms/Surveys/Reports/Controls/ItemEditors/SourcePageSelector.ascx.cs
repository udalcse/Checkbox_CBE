using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class SourcePageSelector : Checkbox.Web.Common.UserControlBase
    {
        protected class PageSource
        {
            public int ID { set; get; }
            public int Position { set; get; }
            public bool IsSelected { set; get; }
        }

        private ResponseTemplate _responseTemplate = null;

        /// <summary>
        /// Get language code for selector
        /// </summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// Get list of selected items
        /// </summary>
        protected List<PageSource> PageList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IncludeTotalSurveyScore 
        {
            get
            {
                if (string.IsNullOrEmpty(_selectedPagesTxt.Value))
                    return false;
                return _selectedPagesTxt.Value.Split(',').Select(p => Convert.ToInt32(p)).ToList().Contains(-1);
            }
        }

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="sourceResponseTemplateId"></param>
        /// <param name="languageCode"></param>
        /// <param name="selectedPages"></param>
        /// <param name="includeSurveyScore"></param>
        public void Initialize(int sourceResponseTemplateId, string languageCode, List<int> selectedPages, bool includeSurveyScore)
        {
            LanguageCode = languageCode;
            PageList = new List<PageSource>();

            _responseTemplate = ResponseTemplateManager.GetResponseTemplate(sourceResponseTemplateId);

            if (!IsPostBack) 
                UpdateSelectedPages(selectedPages, includeSurveyScore);
        }

        /// <summary>
        /// Get list of selected items
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedPages()
        {
            if (string.IsNullOrEmpty(_selectedPagesTxt.Value))
                return new List<int>();

            var list = _selectedPagesTxt.Value.Split(',').Select(p => Convert.ToInt32(p)).ToList();
            list.Remove(-1);

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateSelectedPages(List<int> selectedPages, bool includeSurveyScore)
        {
            PageList.Clear();
            for (int i = 1; i < _responseTemplate.PageCount - 1; i++)
            {
                int? pageId = _responseTemplate.GetPageIdAtPosition(i);
                if (pageId.HasValue)
                {
                    PageList.Add(new PageSource
                    {
                        ID = pageId.Value,
                        IsSelected = selectedPages.Contains(pageId.Value),
                        Position = i
                    });
                }
            }
            //add total survey score option
            PageList.Add(new PageSource
            {
                ID = -1,
                IsSelected = includeSurveyScore
            });
            _selectedPagesTxt.Value = string.Join(",", PageList.Where(p => p.IsSelected).Select(p => p.ID));
        }
    }
}