using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Validation;

namespace CheckboxWeb.Install.Controls
{
    /// <summary>
    /// Email Options Settings Editor
    /// </summary>
    public partial class EMailOptions : Checkbox.Web.Common.UserControlBase
    {
        #region Control Texts
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string SystemEmailAddressValidatorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string DefaultInvitationSenderNameValidatorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string LineLengthValidatorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string SystemEmailAddressCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string DefaultInvitationSenderNameCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string LineLengthCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string LimitEmailLineLengthCaption
        {
            get;
            set;
        }
        #endregion Control Texts

        #region Public Data Properties
        /// <summary>
        /// System email address
        /// </summary>
        public string SystemEmailAddress
        {
            get
            {
                return _systemEmailAddress.Text;
            }
            set
            {
                _systemEmailAddress.Text = value;
            }
        }
        /// <summary>
        /// Default email in From
        /// </summary>
        public string DefaultInvitationSenderName
        {
            get
            {
                return _defaultInvitationSenderName.Text;
            }
            set
            {
                _defaultInvitationSenderName.Text = value;
            }
        }
        /// <summary>
        /// Do we need to limit the line length
        /// </summary>
        public bool LimitEmailLineLength
        {
            get
            {
                return _limitEmailLineLength.Checked;
            }
            set
            {
                _limitEmailLineLength.Checked = value;
            }
        }
        /// <summary>
        /// Line length limit
        /// </summary>
        public string LineLength
        {
            get
            {
                return _lineLength.Text;
            }
            set
            {
                _lineLength.Text = value;
            }
        }

        #endregion Public Data Properties

        /// <summary>
        /// Validate user input
        /// </summary>
        /// <returns></returns>
        public bool ValidateInputs()
        {
            bool valid = true;

            EmailValidator validator = new EmailValidator();
            if (string.IsNullOrEmpty(_systemEmailAddress.Text))
            {
                _systemEmailAddressValidator.Visible = true;
                valid = false;
            }
            else if (!validator.Validate(_systemEmailAddress.Text))
            {
                _systemEmailAddressError.Visible = true;
                valid = false;
            }

            if (string.IsNullOrEmpty(_defaultInvitationSenderName.Text))
            {
                _defaultInvitationSenderNameValidator.Visible = true;
                valid = false;
            }

            if (_limitEmailLineLength.Checked)
            {
                int length;
                if (!Int32.TryParse(_lineLength.Text, out length))
                {
                    _lineLengthValidator.Visible = true;
                    valid = false;
                }
            }

            return valid;
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _limitEmailLineLength.Text = LimitEmailLineLengthCaption;
        }
    }
}