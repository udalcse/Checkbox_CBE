using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class RankOrderBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _rankOrderTypeList.Items.FindByValue("SelectableDragnDrop").Attributes.Add("title", WebTextManager.GetText("/itemType/rankOrderEditor/tooltip/selectableDragNDrop", "en-US"));
            _rankOrderTypeList.Items.FindByValue("TopN").Attributes.Add("title", WebTextManager.GetText("/itemType/rankOrderEditor/tooltip/tonNOrder", "en-US"));
            _rankOrderTypeList.Items.FindByValue("DragnDroppable").Attributes.Add("title", WebTextManager.GetText("/itemType/rankOrderEditor/tooltip/dragAndDropOrder", "en-US"));
            _rankOrderTypeList.Items.FindByValue("Numeric").Attributes.Add("title", WebTextManager.GetText("/itemType/rankOrderEditor/tooltip/numericRankOrder", "en-US"));

            RegisterClientScriptInclude(
             "jquery.numeric.js",
             ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Initialize the editor with the data
        /// </summary>
        /// <param name="itemTextDecorator"></param>
        public void Initialize(SelectItemTextDecorator itemTextDecorator)
        {
            RankOrderItemData itemData = itemTextDecorator.Data as RankOrderItemData;

            if (itemData == null)
                return;

            if (_rankOrderTypeList.Items.FindByValue(itemData.RankOrderType.ToString()) != null)
            {
                _rankOrderTypeList.SelectedValue = itemData.RankOrderType.ToString();
            }

            if (_optionTypeList.Items.FindByValue(itemData.RankOrderOptionType.ToString()) != null)
            {
                _optionTypeList.SelectedValue = itemData.RankOrderOptionType.ToString();
            }

            //Set panels visibility
            SetPanelsVisibility();

            if (itemData.N.HasValue)
            {
                _countOfShownOptions.Text = itemData.N.Value.ToString();
                _countOfSelectedOptions.Text = itemData.N.Value.ToString();
                _countOfRequiredRankOptions.Text = itemData.N.Value.ToString();
            }

            _randomize.Checked = itemData.Randomize;
            _requiredCheck.Checked = itemTextDecorator.Data.IsRequired;
            _aliasText.Text = itemTextDecorator.Data.Alias;
        }

        /// <summary>
        /// Update data with user inputs
        /// </summary>
        /// <param name="itemTextDecorator"></param>
        public void UpdateData(SelectItemTextDecorator itemTextDecorator)
        {
            RankOrderItemData itemData = itemTextDecorator.Data as RankOrderItemData;
            itemTextDecorator.Data.IsRequired = _requiredCheck.Checked;

            if (itemData == null)
                return;

            itemData.RankOrderType = (RankOrderType)Enum.Parse(typeof(RankOrderType), _rankOrderTypeList.SelectedValue);
            //Set To nulls            
            itemData.N = null;
            int temp;

            if (itemData.RankOrderType == RankOrderType.TopN)
            {
                itemData.RankOrderOptionType = RankOrderOptionType.Text;

                //Check for real values
                if (int.TryParse(_countOfShownOptions.Text, out temp))
                    itemData.N = temp;
            }
            else if(itemData.RankOrderType == RankOrderType.Numeric)
            {
                //Check for real values
                if (int.TryParse(_countOfRequiredRankOptions.Text, out temp))
                {
                    if (temp < 2)
                    {
                        _countOfRequiredRankOptions.Text = string.Empty;
                        itemData.N = null;
                    }
                    else
                    {
                        itemData.N = temp;
                    }
                }
            }
            else
            {
                if (itemData.RankOrderType == RankOrderType.SelectableDragnDrop)
                {
                    //Check for real values
                    if (int.TryParse(_countOfSelectedOptions.Text, out temp))
                        itemData.N = temp;
                }
                else if (itemData.RankOrderType == RankOrderType.DragnDroppable)
                {
                    itemTextDecorator.Data.IsRequired = false;
                }

                itemData.RankOrderOptionType =
                    (RankOrderOptionType)
                    Enum.Parse(typeof (RankOrderOptionType), _optionTypeList.SelectedValue);
            }


            itemData.Randomize = _randomize.Checked;
            itemTextDecorator.Data.Alias = _aliasText.Text;

            //Set panels visibility
            SetPanelsVisibility();
        }

        /// <summary>
        /// Shows or hides panels initially
        /// </summary>
        private void SetPanelsVisibility()
        {
            switch (_rankOrderTypeList.SelectedValue)
            {
                case "TopN":
                    _selectableOrderPanel.Style["display"] = "none";
                    _topNOrderPanel.Style.Remove("display");
                    _numericPanel.Style["display"] = "none";
                    _optionTypePanel.Style["display"] = "none";
                    _requiredPanel.Style.Remove("display");
                    break;
                case "DragnDroppable":
                    _selectableOrderPanel.Style["display"] = "none";
                    _topNOrderPanel.Style["display"] = "none";
                    _numericPanel.Style["display"] = "none";
                    _optionTypePanel.Style.Remove("display");
                    _requiredPanel.Style["display"] = "none";
                    break;
                case "SelectableDragnDrop":
                    _selectableOrderPanel.Style.Remove("display");
                    _topNOrderPanel.Style["display"] = "none";
                    _numericPanel.Style["display"] = "none";
                    _optionTypePanel.Style.Remove("display");
                    _requiredPanel.Style.Remove("display");
                    break;
                default:
                    _selectableOrderPanel.Style["display"] = "none";
                    _topNOrderPanel.Style["display"] = "none";
                    _numericPanel.Style.Remove("display");
                    _optionTypePanel.Style.Remove("display");
                    _requiredPanel.Style.Remove("display");
                    break;
            }
        }
    }
}