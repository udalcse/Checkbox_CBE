using System;
using System.Web.UI;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public class SliderBase : UserControlSurveyItemRendererBase
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
        /// Get min value of the item
        /// </summary>
        protected int MinValue
        {
            get
            {
                int temp;
                return int.TryParse(Model.InstanceData["MinValue"], out temp) ? temp : 0;
            }
        }

        /// <summary>
        /// Get max value of the item
        /// </summary>
        protected int MaxValue
        {
            get
            {
                int temp;
                return int.TryParse(Model.InstanceData["MaxValue"], out temp) ? temp : 0;
            }
        }

        /// <summary>
        /// Get steo size of the item
        /// </summary>
        protected int StepSize
        {
            get
            {
                int temp;
                return int.TryParse(Model.InstanceData["StepSize"], out temp) ? temp : 1;
            }
        }

        /// <summary>
        /// Get DefaultValue of the item. This value is used only in Numeric Slider
        /// </summary>
        protected int DefaultValue
        {
            get
            {
                int temp;
                return int.TryParse(Model.InstanceData["DefaultValue"], out temp) ? temp : (MinValue + MaxValue) / 2;
            }
        }

        /// <summary>
        /// Get value tyoe
        /// </summary>
        protected SliderValueType ValueType
        {
            get { return (SliderValueType)Enum.Parse(typeof(SliderValueType), Model.InstanceData["ValueType"]); }
        }


        /// <summary>
        /// Get slider value list option type
        /// </summary>
        protected SliderValueListOptionType ValueListOptionType
        {
            get { return (SliderValueListOptionType)Enum.Parse(typeof(SliderValueListOptionType), Model.InstanceData["ValueListOptionType"]); }
        }

        /// <summary>
        /// Get item width
        /// </summary>
        protected new string Width
        {
            get
            {
                //if width in slider appearance is not defined and there not many options then return default value
                if (Model.AppearanceData == null || string.IsNullOrEmpty(Model.AppearanceData["Width"]))
                {
                    //if width in slider appearance is not defined then calculate according to options number
                    return Model.Options.Length > 2 ? (Model.Options.Length * 100).ToString() + "px" : "200px";
                }

                return Model.AppearanceData["Width"] + "px";
            }
        }

        /// <summary>
        /// Get item width
        /// </summary>
        protected String Height
        {
            get
            {
                return "100%";
            }
        }

        /// <summary>
        /// Get orientation
        /// </summary>
        protected String Orientation
        {
            get { return ((Model.AppearanceData == null || String.IsNullOrEmpty(Model.AppearanceData["Orientation"])) ? "horizontal" : Model.AppearanceData["Orientation"]).ToLower(); }
        }

        /// <summary>
        /// Determine if value should be shown or not.
        /// </summary>
        protected bool ShowValue
        {
            get
            {
                bool temp;
                if (Model.AppearanceData == null)
                {
                    if (Parent is Matrix_Slider)
                    {
                        return bool.TryParse(((Matrix_Slider)Parent).Appearance["ShowValue"], out temp) ? temp : false;                        
                    }
                    return false;
                }
                return Model.AppearanceData == null ? false : (bool.TryParse(Model.AppearanceData["ShowValue"], out temp) ? temp : false);
            }
        }

        /// <summary>
        /// Position of image 
        /// </summary>
        protected string ImagePosition
        {
            get { return ((Model.AppearanceData == null || String.IsNullOrEmpty(Model.AppearanceData["ImagePosition"])) ? "Top" : Model.AppearanceData["ImagePosition"]).ToLower(); }
        }

        /// <summary>
        /// Position of alias text
        /// </summary>
        protected string AliasPosition
        {
            get { return ((Model.AppearanceData == null || String.IsNullOrEmpty(Model.AppearanceData["AliasPosition"])) ? "DontShow" : Model.AppearanceData["AliasPosition"]).ToLower(); }
        }

    }
}