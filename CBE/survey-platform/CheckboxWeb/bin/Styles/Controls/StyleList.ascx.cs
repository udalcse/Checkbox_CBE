﻿using System.Web.UI;

namespace CheckboxWeb.Styles.Controls
{
    public partial class StyleList : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Callback for invitation selected
        /// </summary>
        public string StyleSelectedClientCallback { get; set; }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
        }
    }
}