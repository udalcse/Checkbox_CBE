using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Wrapper for textbox that can handle client timezones correctly. 
    /// Note: dateUtils.js must be included!
    /// </summary>
    public class DateTimePicker : TextBox
    {
        /// <summary>
        /// Number of months to display
        /// </summary>
        public int? NumberOfMonths
        {
            get;
            set;
        }


        /// <summary>
        /// Minimum date value
        /// </summary>
        public DateTime? MinDateTime
        {
            get
            {
                if (Page != null && Page.Request != null && Page.Request.Form != null && !string.IsNullOrEmpty(Page.Request.Form[UniqueID + "_date"]))
                    return System.DateTime.Parse(Page.Request.Form[UniqueID + "_date"]);

                if (ViewState["MinDateTime"] == null)
                    return null;

                return (DateTime)ViewState["MinDateTime"];
            }
            set
            {
                ViewState["MinDateTime"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BeforeShow
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string OnSelect
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NoDefaultDate
        {
            get;
            set;
        }

        /// <summary>
        /// Render script after the control being rendered
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            string script = @"<script>$(document).ready(function(){ 
                    $('#" + ClientID + "').datetimepicker({               numberOfMonths: " + (NumberOfMonths ?? 1).ToString() +
                          (string.IsNullOrEmpty(BeforeShow) ? "" : @",
                            beforeShow:" + BeforeShow) +
                          (string.IsNullOrEmpty(OnSelect) ? "" : @",
                            onSelect:" + OnSelect) +
                            (!MinDateTime.HasValue ? "" : @",
                            minDate: new Date('" + MinDateTime.Value.AddMinutes(1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") + "')") +
                           @"   });       
                    " + (NoDefaultDate ? ("$('#" + ClientID + "').datepicker( {defaultDate : null }); ") : "") + (DateTime.HasValue ? ("$('#" + ClientID + "').datetimepicker('setDate', new Date('" + DateTime.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") + "')); ") : "")
            + "  });</script>";

            writer.Write(script);
        }

        /// <summary>
        /// Date Time
        /// </summary>
        public DateTime? DateTime
        {
            get
            {
                if (Page != null && Page.Request != null && Page.Request.Form != null && Page.Request.Form.AllKeys.Contains(UniqueID + "_date"))
                {
                    if (!string.IsNullOrEmpty(Page.Request.Form[UniqueID + "_date"]))
                    {
                        return System.DateTime.Parse(Page.Request.Form[UniqueID + "_date"]);
                    }

                    return null;
                }                

                if (ViewState["DateTime"] == null)
                    return null;

                return (DateTime)ViewState["DateTime"];
            }
            set
            {
                ViewState["DateTime"] = value; 
            }
        }

        /// <summary>
        /// Format to display the default date value
        /// </summary>
        public string DateFormat
        {
            get;
            set;
        }
    }
}
