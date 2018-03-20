using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;

namespace CheckboxWeb.Controls.Piping
{
    public partial class TermsBindingControl : UserControl
    {
        public static int ItemId { get; set; }
        public string AssociatedControlId { get; set; }
        public int ResponseTemplateId { get; set; }

        public void Initialize(int responseTemplateId, string associatedControlId)
        {
            AssociatedControlId = associatedControlId;
            ResponseTemplateId = responseTemplateId;

            if (ResponseTemplateId != 0 && _termsBinding.Items.Count == 0)
            {
                var responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);

                _termsBinding.Items.Add(new ListItem("None", "0"));
                if (responseTemplate?.SurveyTerms != null)
                {
                    foreach (var term in responseTemplate.SurveyTerms)
                    {
                        _termsBinding.Items.Add(new ListItem(term.Name, term.Name));
                    }
                }

                _mergeButton.Attributes["mergefor"] = _termsBinding.ClientID;
                _termsBinding.Attributes["termtarget"] = AssociatedControlId;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}