using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace CheckboxWeb.Controls.Wizard.WizardControls
{
    public abstract class WizardTemplateControl : Checkbox.Web.Common.UserControlBase
    {

        #region Properties

        /// <summary>
        /// Gets/Sets the ID of the associated Wizard control
        /// </summary>
        /// <remarks>Required if the WizardNavigator is hosted outside of the Wizard control</remarks>
        public String WizardControlID { get; set; }

        /// <summary>
        /// Gets the Wizard control associated with this WizardNavigator
        /// </summary>
        /// <remarks>If this WizardNavigator is hosted in the header template of a Wizard, 
        /// that Wizard is used, otherwise the Wizard specified by WizardControlID is used. </remarks>
        protected System.Web.UI.WebControls.Wizard AssociatedWizardControl
        {
            get
            {
                //See if this control is nested in a Wizard control
                Control parent = this.Parent;
                while (parent != null)
                {
                    if (parent is System.Web.UI.WebControls.Wizard)
                    {
                        return parent as System.Web.UI.WebControls.Wizard;
                    }
                    parent = parent.Parent;
                }

                if (!String.IsNullOrEmpty(WizardControlID))
                {
                    foreach (System.Web.UI.WebControls.Wizard wizard in this.Page.Controls.OfType<System.Web.UI.WebControls.Wizard>())
                    {
                        if (wizard.ID == WizardControlID)
                        {
                            return wizard;
                        }
                    }
                }
                else if (String.IsNullOrEmpty(WizardControlID))
                {
                    throw new ArgumentNullException("No WizardControlID specified");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Could not find Wizard control with ID: " + WizardControlID);
                }

                return null;
            }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            AssociatedWizardControl.ActiveStepChanged += new EventHandler(AssociatedWizardControl_ActiveStepChanged);
        }

        protected abstract void AssociatedWizardControl_ActiveStepChanged(object sender, EventArgs e);
    }
}
