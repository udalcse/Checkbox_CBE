using System;
using System.Web.UI.WebControls;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Invitations;

namespace CheckboxWeb.Settings
{
    public partial class CompanyProfile : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsModal
        {
            get { return Request.Params["IsModal"] != null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool MoveNext
        {
            get { return Request.Params["moveNext"] == "true"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? ProfileId
        {
            get
            {
                int profileId;
                if (int.TryParse(Request.Params["profileId"], out profileId))
                    return profileId;

                return null;
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            _selectCompanyPanel.Visible = !IsModal;
            
            if (IsModal)
            {
                Master.SetTitle(WebTextManager.GetText("/pageText/settings/navigation.ascx/companyProfile"));
                _subtitle.Visible = false;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Master.OkClick += Master_OkClick;
            _profileList.SelectedIndexChanged += _profileList_SelectedIndexChanged;
            
            if (!IsModal)
            {
                FillProfilesDropdown();
                if (!IsPostBack)
                    FillForm();
            }
        //    else if (ProfileId.HasValue)


            _profileNameValidator.ErrorMessage = WebTextManager.GetText("/pageText/settings/companyProfile.aspx/profileNameRequired");
            _companyNameValidator.ErrorMessage = WebTextManager.GetText("/pageText/settings/companyProfile.aspx/companyNameRequired");
            _postcodeValidator.Text = WebTextManager.GetText("/pageText/settings/companyProfile.aspx/postCodeRequired");
            _cityValidator.Text = WebTextManager.GetText("/pageText/settings/companyProfile.aspx/cityRequired");
            _address1Validator.Text = WebTextManager.GetText("/pageText/settings/companyProfile.aspx/addressRequired");
        }

        private void _profileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillForm();
        }

        private void FillForm()
        {
            if (_selectCompanyPanel.Visible && _profileList.SelectedIndex == 0)
            {
                ClearForm();
            }
            else
            {
                var profile = GetSelectedCompanyProfile();

                _profileName.Text = profile.ProfileName;
                _companyName.Text = profile.GetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterCompany);
                _country.SelectedValue = profile.GetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterCountry);
                _city.Text = profile.GetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterCity);
                _state.Text = profile.GetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterState);
                _address1.Text = profile.GetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterAddress1);
                _address2.Text = profile.GetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterAddress2);
                _postcode.Text = profile.GetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterPostCode);
                _isDefaultCheckbox.Checked = profile.IsDefault;
            }
        }

        private void ClearForm()
        {
            _profileName.Text = string.Empty;
            _companyName.Text = string.Empty;
            _country.SelectedIndex = 0;
            _city.Text = string.Empty;
            _state.Text = string.Empty;
            _address1.Text = string.Empty;
            _address2.Text = string.Empty;
            _postcode.Text = string.Empty;
            _isDefaultCheckbox.Checked = false;
        }

        private void FillProfilesDropdown()
        {
            var addNewText = TextManager.GetText("/pageText/settings/companyProfile.aspx/createNew");

            _profileList.Items.Add(new ListItem(addNewText));

            foreach (var profile in CompanyProfileFacade.ListProfiles())
            {
                _profileList.Items.Add(new ListItem(profile.Value, profile.Key.ToString()));
            }

            if (!IsPostBack)
                _profileList.SelectedIndex = _profileList.Items.Count - 1;
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var profile = GetSelectedCompanyProfile();

            profile.SetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterCompany, _companyName.Text);
            profile.SetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterCountry, _country.SelectedValue);
            profile.SetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterCity, _city.Text);
            profile.SetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterState, _state.Text);
            profile.SetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterAddress1, _address1.Text);
            profile.SetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterAddress2, _address2.Text);
            profile.SetProperty(Checkbox.Invitations.CompanyProfile.Property.FooterPostCode, _postcode.Text);
            profile.ProfileName = _profileName.Text;
            profile.IsDefault = _isDefaultCheckbox.Checked;

            profile.Save();

            Session["CurrentInvitationCompanyProfile"] = profile.ProfileId;

            if (IsModal)
                Master.CloseDialog(string.Format("{{success:true, moveNext:{0}, profileId:{1}}}", MoveNext.ToString().ToLower(), profile.ProfileId), true);
            else
            {
                _profileList.Items.Clear();
                FillProfilesDropdown();
                _profileList.SelectedValue = profile.ProfileId.ToString();
            }
        }

        private Checkbox.Invitations.CompanyProfile GetSelectedCompanyProfile()
        {
            if (ProfileId.HasValue)
                return new Checkbox.Invitations.CompanyProfile(ProfileId.Value);

            int profileId;
            if (int.TryParse(_profileList.SelectedValue, out profileId))
                return new Checkbox.Invitations.CompanyProfile(profileId);

            return new Checkbox.Invitations.CompanyProfile();
        }

        #region Validators

        protected void Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !ApplicationManager.AppSettings.EnableMultiDatabase || !string.IsNullOrEmpty(args.Value);
        }

        #endregion
    }
}