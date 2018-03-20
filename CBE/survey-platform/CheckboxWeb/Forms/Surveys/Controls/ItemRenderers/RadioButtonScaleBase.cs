
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Base class for radio button scale horizontal and vertical renderer
    /// </summary>
    public abstract class RadioButtonScaleBase : Checkbox.Web.Common.UserControlBase
    {
        //Get id of selected item option
        public virtual int? GetSelectedOptionId() { return null; }

        /// <summary>
        /// Get/set radio button scale item
        /// </summary>
        protected SurveyResponseItem Model { get; set; }

        /// <summary>
        /// Get/set appearance
        /// </summary>
        protected SimpleNameValueCollection Appearance { get { return Model.AppearanceData; } }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="radioButtonScaleItem"></param>
        public void Initialize(SurveyResponseItem radioButtonScaleItem)
        {
            //Store values
            Model = radioButtonScaleItem;
        }

        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetAllOptions()
        {
            return Model.Options;
        }

        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetNonOtherOptions()
        {
            return Model.Options.Where(a => !a.IsOther);
        }

    }
}