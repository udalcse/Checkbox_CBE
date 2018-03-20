using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Styles.Controls
{
    public partial class FormPreview : Checkbox.Web.Common.UserControlBase
    {
        private string headerTextId;
        private string footerTextId;

        public string StyleCss { get; set; }

        /// <summary>
        /// Handle prerender to set header & footer html
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Utilities.IsNotNullOrEmpty(HeaderHtml))
            {
                _headerPlace.Controls.Clear();
                _headerPlace.Controls.Add(new LiteralControl(HeaderHtml));
            }

            if (Utilities.IsNotNullOrEmpty(FooterHtml))
            {
                _footerPlace.Controls.Clear();
                _footerPlace.Controls.Add(new LiteralControl(FooterHtml));
            }
        }

        public string HeaderHtml { get { return TextManager.GetText(HeaderTextID, SelectedLanguage); } }
        public string FooterHtml { get { return TextManager.GetText(FooterTextID, SelectedLanguage); } }

        public string HeaderTextID
        {
            get { return headerTextId; }
            set { headerTextId = value; }
        }

        public string FooterTextID
        { 
            get { return footerTextId; }
            set { footerTextId = value; }
        }

        public string SelectedLanguage { get; set; }

        internal void Update()
        {
            if (Utilities.IsNotNullOrEmpty(HeaderHtml))
            {
                _headerPlace.Controls.Clear();
                _headerPlace.Controls.Add(new LiteralControl(HeaderHtml));
            }

            if (Utilities.IsNotNullOrEmpty(FooterHtml))
            {
                _footerPlace.Controls.Clear();
                _footerPlace.Controls.Add(new LiteralControl(FooterHtml));
            }

            _cssContainer.Controls.Add(new LiteralControl("<style type=\"text/css\">" + StyleCss.Replace("body{", "#_previewContainer{") + "</style>"));
        }
    }
}