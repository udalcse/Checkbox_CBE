using System;
using System.Collections.Generic;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public class RankOrderBase : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="radioButtonScaleItem"></param>
        public virtual void Initialize(SurveyResponseItem radioButtonScaleItem)
        {
            base.Initialize(radioButtonScaleItem, null);
        }

        /// <summary>
        /// Get options with points
        /// </summary>
        public virtual Dictionary<int, double> GetOptionsWithPoints()
        {
            return null;
        }

        /// <summary>
        /// Get shown options count
        /// </summary>
        protected int ShownOptionsCount
        {
            get
            {
                int temp;

                //If the 'N' value isn't defined.
                if (!int.TryParse(Model.InstanceData["N"], out temp))
                    return Model.Options.Length;

                return Math.Min(temp, Model.Options.Length);
            }
        }

        /// <summary>
        /// Get orientation
        /// </summary>
        protected String Orientation
        {
            get { return (Model.AppearanceData["Orientation"] ?? "horizontal").ToLower(); }
        }

        /// <summary>
        /// Determine if value should be shown or not.
        /// </summary>
        protected bool ShowValue
        {
            get
            {
                bool temp;
                return bool.TryParse(Model.AppearanceData["ShowValue"], out temp) && temp;
            }
        }

        /// <summary>
        /// Determine if value should be shown or not.
        /// </summary>
        protected string OptionLabelOrientation
        {
            get
            {
                return Model.AppearanceData["OptionLabelOrientation"];
            }
        }

        /// <summary>
        /// Get rank order option type
        /// </summary>
        protected RankOrderOptionType OptionType
        {
            get
            {
                return
                    (RankOrderOptionType)
                    Enum.Parse(typeof(RankOrderOptionType), Model.InstanceData["RankOrderOptionType"]);
            }
        }

    }
}