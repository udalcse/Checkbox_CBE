using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Controls.Button
{
    public partial class DefaultButton : Checkbox.Web.Common.UserControlBase, IButtonControl
    {
        public event EventHandler Click;
        public event CommandEventHandler Command;

        /// <summary>
        /// Get/set link button name
        /// </summary>
        public string LinkButtonName { get; set; }

        /// <summary>
        /// Bind events
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _button.Command += _button_Command;
            _button.Click += _button_Click;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _button_Click(object sender, EventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _button_Command(object sender, CommandEventArgs e)
        {
            if (Command != null)
            {
                Command(this, e);
            }
        }

        /// <summary>
        /// Add attributes to button on pre-render
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (String attributeName in Attributes.Keys)
            {
                _button.Attributes.Add(attributeName, Attributes[attributeName]);
            }
        }

        #region IButtonControl Members

        /// <summary>
        /// 
        /// </summary>
        public bool CausesValidation
        {
            get { return _button.CausesValidation; }
            set { _button.CausesValidation = value; }
        }
   

        /// <summary>
        /// 
        /// </summary>
        public string CommandArgument
        {
            get { return _button.CommandArgument; }
            set { _button.CommandArgument = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandName
        {
            get { return _button.CommandName; }
            set { _button.CommandName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PostBackUrl
        {
            get { return _button.PostBackUrl; }
            set { _button.PostBackUrl = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return _buttonText.Text; }
            set { _buttonText.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ValidationGroup
        {
            get { return _button.ValidationGroup; }
            set { _button.ValidationGroup = value; }
        }

        #endregion
    }
}