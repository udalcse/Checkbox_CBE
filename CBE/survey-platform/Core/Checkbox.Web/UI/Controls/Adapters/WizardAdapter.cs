using System.Web.UI.WebControls;
using System.Web.UI;

namespace Checkbox.Web.UI.Controls.Adapters
{
    /// <summary>
    /// The wizard adpater will render the standard wizard control using div's rather than
    /// tables.
    /// </summary>
    public class WizardAdapter : System.Web.UI.WebControls.Adapters.WebControlAdapter
    {
        #region Member Variables

        private WebControlAdapterExtender _extender = null;

        // CSS class names for the div's
        private const string CSS_WIZARD = "Wizard";
        private const string CSS_STEP = "WizardStepContainer";
        private const string CSS_NAV = "WizardNavContainer";
        private const string CSS_HEADER = "WizardHeaderContainer";
        private const string CSS_SIDEBAR = "WizardSidebarContainer";
        private const string CSS_ACTIVE = "WizardActive";

        // The ID's of the underlying control's containers.  
        private const string CONTAINERID_FINISHNAVIGATION_TEMPLATE = "FinishNavigationTemplateContainerID";
        private const string CONTAINERID_STARTNAVIGATION_TEMPLATE = "StartNavigationTemplateContainerID";
        private const string CONTAINERID_STEPNAVIGATION_TEMPLATE = "StepNavigationTemplateContainerID";
        private const string CONTAINERID_HEADER = "HeaderContainer";
        private const string CONTAINERID_SIDEBAR = "SideBarContainer";

        // The ID's of the underlying controls within the containers - the ID's are important as the underlying
        // control (I assume) uses these to wire up the event handlers to respond to button clicks etc.
        private const string CONTROLID_STARTNEXT = "StartNext";
        private const string CONTROLID_STEPPREVIOUS = "StepPrevious";
        private const string CONTROLID_STEPNEXT = "StepNext";
        private const string CONTROLID_FINISHPREVIOUS = "FinishPrevious";
        private const string CONTROLID_FINISH = "Finish";
        private const string CONTROLID_CANCEL = "Cancel";

        private const string CONTROLID_SIDEBARLIST = "SideBarList";
        private const string CONTROLID_SIDEBARBUTTON = "SideBarButton";

        #endregion

        #region Private Properties

        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((_extender == null) && (Control != null)) ||
                    ((_extender != null) && (Control != _extender.AdaptedControl)))
                {
                    _extender = new WebControlAdapterExtender(Control);
                }

                System.Diagnostics.Debug.Assert(_extender != null, "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }


        #endregion

        #region Rendering Overrides

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Extender.RenderBeginTag(writer, CSS_WIZARD);
            }
            else
            {
                base.RenderBeginTag(writer);
            }
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Extender.RenderEndTag(writer);
            }
            else
            {
                base.RenderEndTag(writer);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Wizard wizard = Control as Wizard;
                if (wizard != null)
                {
                    // Render Side Bar
                    RenderSideBar(writer, wizard);

                    // Render Header
                    RenderHeader(writer, wizard);

                    // Render WizardStep
                    RenderStep(writer, wizard);

                    // If ActiveStep.StepType is Auto then we determine the type of navigation
                    // by the order in which the steps are declared
                    bool auto = (wizard.ActiveStep.StepType == WizardStepType.Auto);
                    int numberOfSteps = wizard.WizardSteps.Count;
                    int index = wizard.ActiveStepIndex;

                    // If on first page of wizard
                    if ((auto && index == 0) ||
                       wizard.ActiveStep.StepType == WizardStepType.Start)
                    {
                        RenderStartNavigation(writer, wizard);
                    }
                    else if ((auto && index >= 0 && index < numberOfSteps - 1) ||
                       wizard.ActiveStep.StepType == WizardStepType.Step)
                    {
                        RenderStepNavigation(writer, wizard);
                    }
                    else if ((auto && index == numberOfSteps - 1) ||
                       wizard.ActiveStep.StepType == WizardStepType.Finish)
                    {
                        RenderFinishNavigation(writer, wizard);
                    }
                }
            }
            else
            {
                // Use the default rendering of the control
                base.RenderContents(writer);
            }
        }

        #endregion

        #region Private Rendering Methods

        private void RenderFinishNavigation(HtmlTextWriter writer, Wizard wizard)
        {
            RenderNavigation(writer, wizard, CONTAINERID_FINISHNAVIGATION_TEMPLATE, wizard.FinishNavigationTemplate);
        }

        private void RenderStartNavigation(HtmlTextWriter writer, Wizard wizard)
        {
            RenderNavigation(writer, wizard, CONTAINERID_STARTNAVIGATION_TEMPLATE, wizard.StartNavigationTemplate);
        }

        private void RenderStepNavigation(HtmlTextWriter writer, Wizard wizard)
        {
            if (wizard.ActiveStep is TemplatedWizardStep)
            {
                RenderNavigation(writer, wizard, CONTAINERID_STEPNAVIGATION_TEMPLATE, ((TemplatedWizardStep)wizard.ActiveStep).CustomNavigationTemplate);
            }
            else
            {
                RenderNavigation(writer, wizard, CONTAINERID_STEPNAVIGATION_TEMPLATE, wizard.StepNavigationTemplate);
            }
        }

        private void RenderNavigation(HtmlTextWriter writer, Wizard wizard, string containerID, ITemplate template)
        {
            // Locate the name of the underlying container that will host the step navigation controls
            // You just have to know this name - it is used by the underyling Wizard control
            Control container = wizard.FindControl(containerID);
            
            // Check the container exists - academic it will always exist 
            if (container != null)
            {                
                // Start the step navigation with a DIV
                WebControlAdapterExtender.WriteBeginDiv(writer, CSS_NAV, "");
                writer.Write("<hr/>");
               
                // If a Template has been defined then use this
                if (template != null)
                {
                    container.RenderControl(writer);
                }
                else //if (!(wizard.ActiveStep is TemplatedWizardStep))
                {
                    switch (containerID)
                    {
                        case CONTAINERID_STARTNAVIGATION_TEMPLATE:
                            {
                                // Only display Next and (optionally) Cancel
                                RenderSubmit(writer, wizard, container, wizard.StartNextButtonStyle, wizard.StartNextButtonImageUrl, wizard.StartNextButtonText, wizard.StartNextButtonType, CONTROLID_STARTNEXT);
                                if (wizard.DisplayCancelButton)
                                    RenderSubmit(writer, wizard, container, wizard.CancelButtonStyle, wizard.CancelButtonImageUrl, wizard.CancelButtonText, wizard.CancelButtonType, CONTROLID_CANCEL);
                                break;
                            }
                        case CONTAINERID_STEPNAVIGATION_TEMPLATE:
                            {
                                // Display Previous (if AllowReturn true in previous step), Next and (optionally) Cancel
                                if (wizard.WizardSteps[wizard.ActiveStepIndex - 1].AllowReturn)
                                {
                                    RenderSubmit(writer, wizard, container, wizard.StepPreviousButtonStyle, wizard.StepPreviousButtonImageUrl, wizard.StepPreviousButtonText, wizard.StepPreviousButtonType, CONTROLID_STEPPREVIOUS);
                                }
                                RenderSubmit(writer, wizard, container, wizard.StepNextButtonStyle, wizard.StepNextButtonImageUrl, wizard.StepNextButtonText, wizard.StepNextButtonType, CONTROLID_STEPNEXT);
                                if (wizard.DisplayCancelButton)
                                    RenderSubmit(writer, wizard, container, wizard.CancelButtonStyle, wizard.CancelButtonImageUrl, wizard.CancelButtonText, wizard.CancelButtonType, CONTROLID_CANCEL);
                                break;
                            }
                        case CONTAINERID_FINISHNAVIGATION_TEMPLATE:
                            {
                                // Display Previous, Complete and (optionally) Cancel
                                RenderSubmit(writer, wizard, container, wizard.FinishPreviousButtonStyle, wizard.FinishPreviousButtonImageUrl, wizard.FinishPreviousButtonText, wizard.FinishPreviousButtonType, CONTROLID_FINISHPREVIOUS);
                                RenderSubmit(writer, wizard, container, wizard.FinishCompleteButtonStyle, wizard.FinishCompleteButtonImageUrl, wizard.FinishCompleteButtonText, wizard.FinishCompleteButtonType, CONTROLID_FINISH);
                                if (wizard.DisplayCancelButton)
                                    RenderSubmit(writer, wizard, container, wizard.CancelButtonStyle, wizard.CancelButtonImageUrl, wizard.CancelButtonText, wizard.CancelButtonType, CONTROLID_CANCEL);
                                break;
                            }

                        default:
                            break;
                    }

                }
            }
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void RenderSubmit(HtmlTextWriter writer, Wizard wizard, Control container, Style buttonStyle, string buttonImageUrl, string buttonText, ButtonType buttonType, string submitControlRootName)
        {
            // Locate the corresponding control placeholder that the Wizard has defined within it's base 
            // Control heirarchy. For example, StepNextButton.
            string idWithType = WebControlAdapterExtender.MakeIdWithButtonType(submitControlRootName, buttonType);
            Control btn = container.FindControl(idWithType);

            string id = container.ID + "_" + submitControlRootName;

            // Will only be null if the submitControlRootName value was passed in. 
            if (btn != null)
            {
                // Register the id of the button placeholder so that we can raise postback events
                Page.ClientScript.RegisterForEventValidation(btn.UniqueID);

                // Only use client side (javascript based) submits for Links
                bool clientSubmit = (buttonType == ButtonType.Link);

                string javascript = "";
                if (clientSubmit)
                {
                    PostBackOptions options = new PostBackOptions(btn, "", "", false, false, false, clientSubmit, true, wizard.ID);
                    javascript = "javascript:" + Page.ClientScript.GetPostBackEventReference(options);
                    javascript = Page.Server.HtmlEncode(javascript);
                }
                Extender.WriteSubmit(writer, buttonType, buttonStyle.CssClass, id, buttonImageUrl, javascript, buttonText);
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Wizard wizard = Control as Wizard;
            if (wizard != null)
            {
                TemplatedWizardStep activeStep = wizard.ActiveStep as TemplatedWizardStep;
                if (activeStep != null)
                {
                    if ((activeStep.ContentTemplate != null) && (activeStep.Controls.Count == 1))
                    {
                        Control container = activeStep.ContentTemplateContainer;
                        if (container != null)
                        {
                            container.Controls.Clear();
                            activeStep.ContentTemplate.InstantiateIn(container);
                            container.DataBind();
                        }
                    }
                }
            }
        }

        private void RenderStep(HtmlTextWriter writer, Wizard wizard)
        {
            WebControlAdapterExtender.WriteBeginDiv(writer, CSS_STEP, "");

            // TODO: Can't get the template steps to render properly
            // This method still renders the template's contents but does so in
            // a table structure. Should be possible to override this behaviour some
            // how but I haven't worked it out yet.
            // 
            //if (wizard.ActiveStep is TemplatedWizardStep)
            //{
            //    TemplatedWizardStep step = wizard.ActiveStep as TemplatedWizardStep;
            //    Control container = step.ContentTemplateContainer;
            //    if (container != null)
            //    {
            //        container.Controls.Clear();
            //        step.ContentTemplate.InstantiateIn(container);
            //        container.DataBind();
            //    }
            //}
            //else
            {
                foreach (Control control in wizard.ActiveStep.Controls)
                {
                    control.RenderControl(writer);
                }
            }
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void RenderHeader(HtmlTextWriter writer, Wizard wizard)
        {
            WebControlAdapterExtender.WriteBeginDiv(writer, CSS_HEADER, "");

            if (wizard.HeaderTemplate != null)
            {
                string containerID = CONTAINERID_HEADER;
                Control container = wizard.FindControl(containerID);

                // Not sure why I don't need this line but it works anyway!
                //wizard.HeaderTemplate.InstantiateIn(container);
                container.RenderControl(writer);
            }
            else
            {
                writer.Write(wizard.HeaderText);
            }
            WebControlAdapterExtender.WriteEndDiv(writer);
        }

        private void RenderSideBar(HtmlTextWriter writer, Wizard wizard)
        {
            if (wizard.DisplaySideBar)
            {
                WebControlAdapterExtender.WriteBeginDiv(writer, CSS_SIDEBAR, "");
                Control container = wizard.FindControl(CONTAINERID_SIDEBAR);

                if (wizard.SideBarTemplate != null)
                {
                    // Not sure why I don't need this line but it works anyway!
                    //wizard.SideBarTemplate.InstantiateIn(container);
                    container.RenderControl(writer);
                }
                else
                {

                    Control listContainer = container.FindControl(CONTROLID_SIDEBARLIST);

                    int listIndex = 0;
                    writer.WriteLine();

                    foreach (WizardStepBase step in wizard.WizardSteps)
                    {
                        // Find the control within the container that contains the linkbutton
                        Control control = listContainer.Controls[listIndex];

                        // Find the LinkButton itself
                        LinkButton linkButton = control.FindControl(CONTROLID_SIDEBARBUTTON) as LinkButton;
                        
                        // Get the postback javascript code and register the LinkButton control so that we can 
                        // raise postback events 
                        string javascript = Page.ClientScript.GetPostBackClientHyperlink(linkButton, "", true);

                        // Render the LinkButton using Anchors
                        writer.WriteBeginTag("a");
                        if (wizard.ActiveStepIndex == listIndex)
                        {
                            writer.WriteAttribute("class", CSS_ACTIVE);
                        }
                        writer.WriteAttribute("href", javascript);
                        writer.WriteAttribute("id", linkButton.ClientID);

                        // This is what the default rendering does - not sure it's cross-browser acceptable
                        if (step.StepType == WizardStepType.Complete)
                        {
                            writer.WriteAttribute("disabled", "disabled");
                        }
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Write(step.Title);
                        writer.WriteEndTag("a");

                        writer.WriteLine();

                        listIndex++;
                    }
                }

                WebControlAdapterExtender.WriteEndDiv(writer);
            }

        }

        #endregion
    }
}
