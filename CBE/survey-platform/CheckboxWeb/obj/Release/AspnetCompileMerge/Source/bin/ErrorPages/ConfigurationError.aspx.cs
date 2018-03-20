using System;
using System.Web.UI;

namespace CheckboxWeb.ErrorPages
{
    public partial class ConfigurationError : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Application["ConfigurationErrors"] == null)
            {
                return;
            }

            var errors = (string)Application["ConfigurationErrors"];

            if (!String.IsNullOrEmpty(errors))
            {

                errors = errors.Replace(Environment.NewLine, "<br />");
                errors = errors.Replace("FAILED --", "&deg;&nbsp;");

                _errorMessages.Text = errors;
            }

            Session.Abandon();
        }
    }
}
