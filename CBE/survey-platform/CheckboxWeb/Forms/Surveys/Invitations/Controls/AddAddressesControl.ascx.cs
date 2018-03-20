using System;
using System.Collections.Generic;
using Checkbox.Forms.Validation;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    public partial class AddAddressesControl : Checkbox.Web.Common.UserControlBase
    {
        private List<string> _invalidAddresses;
        private List<string> _validAddresses;

        public event EventHandler AdressesAdded;
        

        #region Properties

        public List<string> SelectedAddresses
        {
            get
            {
                _textEntryError.Visible = false;

                if (_invalidAddresses == null)
                {
                    _invalidAddresses = new List<string>();
                }
                if (_validAddresses == null)
                {
                    _validAddresses = new List<string>();
                }

                ProcessAddresses();

                _importTxt.Text = string.Empty;

                if (_invalidAddresses.Count > 0)
                {
                    //Re-bind the text box with only invalid email addresses                    
                    foreach (string invalidAddress in _invalidAddresses)
                    {
                        _importTxt.Text += invalidAddress + Environment.NewLine;
                    }

                    _textEntryError.Text = WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/AddAddressesControl.ascx/error");
                    _textEntryError.Visible = true;
                }

                return _validAddresses;
            }
        }

        #endregion

        protected void AddAddressesButton_Click(object sender, EventArgs e)
        {
            //_textEntryError.Visible = false;
            //ProcessAddresses();

            //if (InvalidAddresses.Count > 0)
            //{
            //    StringBuilder invalidAddressList = new StringBuilder();
            //    invalidAddressList.Append("<ul>");
            //    foreach (string invalidAddress in InvalidAddresses)
            //    {
            //        invalidAddressList.Append("<li>");
            //        invalidAddressList.Append(invalidAddress);
            //        invalidAddressList.Append("</li>");
            //    }
            //    invalidAddressList.Append("</ul>");

            //    _textEntryError.Text = WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/AddAddressesControl.ascx/error") + invalidAddressList.ToString();
            //    _textEntryError.Visible = true;
            //}
            //else
            //{
            //    SelectedAddresses = _validAddresses;

            //    if (AdressesAdded != null)
            //    {
            //        AdressesAdded(this, new EventArgs());
            //    }
            //}
        }

        private void ProcessAddresses()
        {
            List<string> panelists = new List<string>(_importTxt.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

            EmailValidator validator = new EmailValidator();
            foreach (string address in panelists)
            {
                if (validator.Validate(address))
                {
                    _validAddresses.Add(address);
                }
                else
                {
                    _invalidAddresses.Add(address);
                }
            }
        }
    }
}