using System;

using System.Web.UI;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Master page base class for programmatic access to portions of the master page.
    /// </summary>
    public abstract class BaseMasterPage : MasterPage
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsDialog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsEmbedded { get; set; }

        /// <summary>
        /// Set the text to display in the title label
        /// </summary>
        /// <param name="title">Title text</param>
        public virtual void SetTitle(string title) { }

        /// <summary>
        /// Add a control as a page title.
        /// </summary>
        /// <param name="titleControl"></param>
        public virtual void SetTitleControl(Control titleControl) { }

        /// <summary>
        /// Show the error control
        /// </summary>
        /// <param name="error">Message to display</param>
        /// <param name="ex">Exception with additional information</param>
        public virtual void ShowError(string error, Exception ex) { }

        /// <summary>
        /// Hide the error control
        /// </summary>
        public virtual void HideError() { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void HideTitle(){}

        /// <summary>
        /// Get/Set the nav menu item to be highlighted 
        /// </summary>
        public virtual string ActiveMenuSection
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IsEmbedded = !string.IsNullOrEmpty(Request["UFrame"]);
        }
    }
}
