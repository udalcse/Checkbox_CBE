using System;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public partial class MatrixValueListSlider : SliderBase
    {
        /// <summary>
        /// Get selected option ID
        /// </summary>
        public virtual int SelectedOptionID
        {
            get
            {
                int parseResult;
                return int.TryParse(Request[_options.UniqueID], out parseResult) ? parseResult : -1;
            }
        }

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="radioButtonScaleItem"></param>
        public override void Initialize(SurveyResponseItem radioButtonScaleItem)
        {
            base.Initialize(radioButtonScaleItem);

            InitializeOptionsList();
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

        /// <summary>
        /// Initialize options list
        /// </summary>
        protected void InitializeOptionsList()
        {
            _options.Items.Clear();

            foreach (SurveyResponseItemOption option in Model.Options)
            {
                _options.Items.Add(new ListItem("", option.OptionId.ToString()));

                if (option.IsSelected)
                    _options.SelectedValue = option.OptionId.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude("selectToUISlider", ResolveUrl("~/Resources/selectToUISlider.jQuery.js"));
        }
    }
}