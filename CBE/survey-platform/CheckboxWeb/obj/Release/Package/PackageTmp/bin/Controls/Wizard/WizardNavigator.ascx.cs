using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Controls.Wizard.WizardControls
{
    public partial class WizardNavigator : WizardTemplateControl
    {
        private const String NAVIGATION_COMMAND_NAME = "SwitchStep";

        private Boolean _allowForwardNavigation = false;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _wizardNavigationRepeater.DataSource = AssociatedWizardControl.WizardSteps;
            _wizardNavigationRepeater.DataBind();
            _wizardNavigationRepeater.ItemCommand += WizardNavigationRepeater_ItemCommand;

        }

        #region Properties       

        /// <summary>
        /// Gets/Sets a flag indicating whether or not users are allowed to navigate to subsequent steps directly
        /// </summary>
        /// <remarks>Backward navigation is controlled by the AllowReturn property of each WizardStep</remarks>
        public Boolean AllowForwardNavigation 
        {
            get { return _allowForwardNavigation; }
            set { _allowForwardNavigation = value; }
        }
        #endregion

        #region Event handlers

        /// <summary>
        /// Handles the item command event of the repeater; navigates the wizard to the desired step
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void WizardNavigationRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == NAVIGATION_COMMAND_NAME)
            {
                AssociatedWizardControl.MoveTo((WizardStepBase)AssociatedWizardControl.FindControl((String)e.CommandArgument));

            }
        }

        /// <summary>
        /// Handles the item data bound event of the repeater; sets the text and style of each nav item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void WizardNavigationRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
            {
                var bindingStep = e.Item.DataItem as WizardStep;
                PlaceHolder content = e.Item.FindControl("_navListItem") as PlaceHolder;
                if (content != null) content.Controls.Clear();
                //Set up the state of the list item based on the state of the associated wizard step

                /* Rendering logic:
                 * if the binding step is the active step render the title in a span and set the style to selected
                 * if the binding step is prior to the active step 
                 *  and (the binding step is prior to a step with allowreturn=false that is also prior to the active step OR the binding step has allowreturn=false), render the title in a span and set the style to completed
                 *  else render the title in a link and set the style to completed
                 * if the binding step is subsequent to the active step and AllowForwardNavigation is true, render the title in a link and set the style to pending
                 * if the binding step is subsequent to the active step and AllowForwardNavigation is false, render the title in a span and set the style to pending
                 */

                //Determine the index of the current wizard step so we can use it to determine if it's before or after the currently selected step
                Int32 stepIndex = 0;
                Int32 bindingStepIndex = 0;
                Int32 nextAllowReturnFalseIndex = 0;
                Boolean continueCounting = true;
                foreach (WizardStep possibleStep in AssociatedWizardControl.WizardSteps)
                {
                    if (possibleStep == bindingStep)
                    {
                        continueCounting = false;
                    }
                    if (continueCounting)
                    {
                        bindingStepIndex++;
                    }
                    else
                    {
                        if (possibleStep != null && !possibleStep.AllowReturn)
                        {
                            nextAllowReturnFalseIndex = stepIndex;
                        }
                    }
                    stepIndex++;
                }

                //Set up common controls
                LiteralControl listItemStart = new LiteralControl("<li class=\"WizardItem\">");
                LiteralControl listItemEnd = new LiteralControl("</li>");
                Panel completedMark = new Panel { CssClass = "WizardCompletedMarkPending" };
                Image completedPlaceHolder = new Image { SkinID = "WizardCompletedPlaceholder" };
                completedMark.Controls.Add(completedPlaceHolder);

                if (bindingStepIndex == AssociatedWizardControl.ActiveStepIndex)
                {
                    Label title = new Label { CssClass = "WizardLinkSelectedHead", Text = bindingStep.Title };
                    LiteralControl selectedListItemStart = new LiteralControl("<li class=\"WizardItem SelectedBody\">");
                    LiteralControl selectedListItemEnd = new LiteralControl("</li>");
                    content.Controls.Add(selectedListItemStart);
                    content.Controls.Add(completedMark);
                    content.Controls.Add(title);
                    content.Controls.Add(selectedListItemEnd);
                }
                else if (bindingStepIndex < AssociatedWizardControl.ActiveStepIndex)
                {
                    if ((!bindingStep.AllowReturn) || (nextAllowReturnFalseIndex > bindingStepIndex && nextAllowReturnFalseIndex < AssociatedWizardControl.ActiveStepIndex))
                    {
                        Label title = new Label { CssClass = "WizardLinkDone", Text = bindingStep.Title };
                        completedMark.CssClass = "WizardCompletedMark";
                        listItemStart = new LiteralControl("<li class=\"WizardItemDone\">");
                        content.Controls.Add(listItemStart);
                        content.Controls.Add(completedMark);
                        content.Controls.Add(title);
                        content.Controls.Add(listItemEnd);
                    }
                    else 
                    {

                        Label title = new Label { CssClass = "WizardLinkDone", Text = bindingStep.Title/*, CommandName = NAVIGATION_COMMAND_NAME, CommandArgument = bindingStep.ID*/ };
                        completedMark.CssClass = "WizardCompletedMark";
                        listItemStart = new LiteralControl("<li class=\"WizardItemDone\">");
                        content.Controls.Add(listItemStart);
                        content.Controls.Add(completedMark);
                        content.Controls.Add(title);
                        content.Controls.Add(listItemEnd);
                    }                    
                }
                else if (bindingStepIndex > AssociatedWizardControl.ActiveStepIndex && !_allowForwardNavigation)
                {
                    Label title = new Label { CssClass = "WizardLink", Text = bindingStep.Title };
                    content.Controls.Add(listItemStart);
                    content.Controls.Add(completedMark);
                    content.Controls.Add(title);
                    content.Controls.Add(listItemEnd);
                }
                else //(selectedStepIndex > AssociatedWizardControl.ActiveStepIndex && _allowForwardNavigation)
                {
                    Label title = new Label { CssClass = "WizardLink", Text = bindingStep.Title/*, CommandName = NAVIGATION_COMMAND_NAME, CommandArgument = bindingStep.ID*/ };
                    content.Controls.Add(listItemStart);
                    content.Controls.Add(completedMark);
                    content.Controls.Add(title);
                    content.Controls.Add(listItemEnd);
                }                
            }
        }

        /// <summary>
        /// Handles the active step changed event of the associated wizard control; re-binds the repeater to 
        /// pick up the selected step in order to properly highlight the nav bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void AssociatedWizardControl_ActiveStepChanged(object sender, EventArgs e)
        {
            _wizardNavigationRepeater.DataSource = AssociatedWizardControl.WizardSteps;
            _wizardNavigationRepeater.DataBind();
        }

        #endregion

    }
}