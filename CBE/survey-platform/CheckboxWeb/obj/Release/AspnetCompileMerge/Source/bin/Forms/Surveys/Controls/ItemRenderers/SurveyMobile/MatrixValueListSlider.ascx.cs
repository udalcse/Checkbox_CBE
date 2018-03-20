using System.Linq;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class MatrixValueListSlider : SliderBase
    {
        /// <summary>
        /// Get selected option ID
        /// </summary>
        public int SelectedOptionID
        {
            get
            {
                int parseResult;
                return int.TryParse(Request["numericSlider_" + ClientID], out parseResult) ? Model.Options[parseResult].OptionId : -1;
            }
        }

        protected int AnsweredIndex
        {
            get
            {
                var selectedOption = Model.Options.Select((o, i) => new { option = o, index = i })
                    .FirstOrDefault(o => o.option.IsSelected);

                return selectedOption != null ? selectedOption.index : DefaultOptionIndex;
            }
        }

        /// <summary>
        /// Default option index
        /// </summary>
        protected int DefaultOptionIndex
        {
            get
            {
                var defaultOption = Model.Options.Select((o, i) => new { option = o, index = i })
                    .FirstOrDefault(o => o.option.IsDefault);

                return defaultOption != null ? defaultOption.index : 0;
            }
        }

        /// <summary>
        /// Default option id
        /// </summary>
        protected int? DefaultOptionId
        {
            get
            {
                var defaultOption = Model.Options.FirstOrDefault(o => o.IsDefault);
                return defaultOption != null ? defaultOption.OptionId : default(int?);
            }
        }
    }
}