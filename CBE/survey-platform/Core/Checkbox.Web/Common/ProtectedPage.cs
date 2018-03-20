using System.ComponentModel;
using Checkbox.Web.Page;

namespace Checkbox.Web.Common
{
	/// <summary>
	/// The base class for all pages protected by licensing
	/// </summary>
	//[LicenseProvider(typeof(Xheo.Licensing.ExtendedLicenseProvider))]
    public class ProtectedPage : BasePage
    {
        private License mLicense;

        ///// <summary>
        ///// Check for a valid license
        ///// </summary>
        //public ProtectedPage()
        //{
            
        //     //Validate the license for the object type being instantiated
        //    try
        //    {
        //        mLicense = LicenseManager.Validate(typeof(ProtectedPage), this);
        //    }
        //    catch (LicenseException lex)
        //    {
                
        //        string message = lex.Message;
        //        System.Web.HttpContext.Current.Items["LicenseErrorMessage"] = message;

        //        Server.Transfer(ResolveUrl("~/ErrorPages/LicenseError.aspx"));
        //    }
        //}

        /// <summary>
        /// Dispose of the license file
        /// </summary>
        public override void Dispose()
        {
            if (mLicense != null)
            {
                mLicense.Dispose();
                mLicense = null;
            }
            base.Dispose();
        }

        ///// <summary>
        ///// Gets the license in effect for the current type
        ///// </summary>
        //public Xheo.Licensing.ExtendedLicense ActiveLicense
        //{
        //    get
        //    {
        //        if (mLicense != null)
        //        {
        //            return (Xheo.Licensing.ExtendedLicense)mLicense;
        //        }
                
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Renders the page to the output stream
        ///// - Checks the license version and writes a trial message if the active license is a trial license
        ///// </summary>
        ///// <param name="writer"></param>
        //protected override void Render(System.Web.UI.HtmlTextWriter writer)
        //{

        //    //Get the active license
        //    Xheo.Licensing.ExtendedLicense extendedLicense = ActiveLicense;
        //    //Add the trial banner if the active license is a trial license
        //    if (extendedLicense.IsTrial)
        //    {
        //        writer.Write("<TABLE cellSpacing=\"1\" cellPadding=\"1\" width=\"75%\" align=\"center\" border=\"0\" bgcolor=\"red\"><TR><TD><TABLE cellSpacing=\"1\" cellPadding=\"1\" width=\"100%\" align=\"center\" border=\"0\" bgcolor=\"white\"><tr><td class=\"ErrorMessage\" align=\"center\">Checkbox&reg; Survey Trial Evaluation Version</td></tr></table></TD></TR></TABLE>");
        //    }

            
        //    base.Render(writer);
        //}
    }
}
