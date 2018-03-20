using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Security.Principal;

namespace Checkbox.Analytics.Items.Configuration
{
    public class AnalysisItemFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="options"></param>
        /// <param name="chartStyleAppearance"></param>
        /// <returns></returns>
        public static AnalysisItemData GetItem(ItemData item, int responseTemplateId, AnalysisWizardOptions options, AppearanceData chartStyleAppearance, CheckboxPrincipal principal)
        {
            AnalysisItemData analysisItem = null;

            switch (item.ItemTypeName)
            {
                //Text Items
                case "SingleLineText":
                    options.GraphType = options.SingleLineTextGraphType;
                    if (!string.IsNullOrEmpty(options.GraphType))
                        analysisItem = AnalysisItemConfigurationManager.GetTextItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                case "MultiLineText":
                    options.GraphType = options.MultiLineTextGraphType;
                    if (!string.IsNullOrEmpty(options.GraphType))
                        analysisItem = AnalysisItemConfigurationManager.GetTextItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                //Hidden Item
                case "HiddenItem":
                    options.GraphType = options.HiddenItemGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetHiddenItem(item, responseTemplateId, options, principal);
                    break;

                //Matrix Item
                case "Matrix":
                    options.GraphType = options.MatrixGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetMatrixItem(item, responseTemplateId, options, principal);
                    break;

                //Select item
                case "RadioButtons":
                    options.GraphType = options.RadioButtonGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetSelectItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                case "DropdownList":
                    options.GraphType = options.DropDownListGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetSelectItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                case "Checkboxes":
                    options.GraphType = options.CheckboxGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetSelectItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                case "RadioButtonScale":
                    options.GraphType = options.RatingScaleGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetSelectItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                case "Slider":
                    options.GraphType = options.SliderGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetSelectItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                case "RankOrder":
                    options.GraphType = options.RankOrderGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetSelectItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                case "NetPromoterScore":
                    options.GraphType = options.NetPromoterScoreGraphType;
                    analysisItem = AnalysisItemConfigurationManager.GetSelectItem(item, responseTemplateId, options, chartStyleAppearance, principal);
                    break;

                default:
                    break;
            }

            return analysisItem;
        }
    }
}