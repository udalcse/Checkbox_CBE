using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class ScoreMessageBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize editor
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="pagePosition"></param>
        /// <param name="surveyId"></param>
        public void Initialize(ScoreMessageItemData itemData, int surveyId, int pagePosition)
        {
            //fill page list
            _pageList.Items.Clear();
            _pageList.Items.Add(new ListItem(TextManager.GetText("/controlText/currentScoreBehavior/allPages"), "ALL_PAGES"));

            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);
            for (int i = 1; i < pagePosition - 1; i++)
            {
                var pageId = responseTemplate.GetPageIdAtPosition(i);
                if (pageId.HasValue)
                    _pageList.Items.Add(new ListItem(TextManager.GetText("/controlText/currentScoreBehavior/page") + " " + i, pageId.ToString()));
            }

            if (itemData.PageId.HasValue
                && _pageList.Items.FindByValue(itemData.PageId.ToString()) != null)
            {
                _pageList.SelectedValue = itemData.PageId.ToString();
            }
        }

        /// <summary>
        /// Update item data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(ScoreMessageItemData itemData)
        {
            itemData.PageId = Utilities.AsInt(_pageList.SelectedValue);
        }
    }
}