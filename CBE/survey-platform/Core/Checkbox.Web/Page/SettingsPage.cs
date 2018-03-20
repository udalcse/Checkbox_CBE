namespace Checkbox.Web.Page
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsPage : SecuredPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRole
        {
            get { return "System Administrator"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            Page.ClientScript.RegisterStartupScript(GetType(), "resizePanels", "if(typeof(resizePanels) == 'function') {resizePanels();}", true);
        }
    }
}
