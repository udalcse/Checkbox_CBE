using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Management;

namespace Checkbox.Web.Forms.Items.Formatters
{
    /// <summary>
    /// Format rank order items as html or text
    /// </summary>
    [Serializable]
    public class RankOrderItemFormatter : WebItemFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string HtmlTransformFileName
        {
            get { return ApplicationManager.AppSettings.ResponseDisplayRankOrderPoints ? "RankOrderPointsToHtml.xslt" : "RankOrderToHtml.xslt"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string TextTransformFileName
        {
            get { return "RankOrderToText.xslt"; }
        }
    }
}
