
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Checkbox.Web.Forms.UI.Templates
{
    /// <summary>
    /// Container for items in a template
    /// </summary>
    public class ControlLayoutZone : CompositeControl, IWebLayoutZone
    {
        /// <summary>
        /// Get/set whether the control is in layout design mode
        /// </summary>
        public bool LayoutDesignMode { get; set; }

        /// <summary>
        /// Get/set whether to show the element as a block element (rather than an inline element)
        /// </summary>
        public bool DesignBlockMode { get; set; }

        /// <summary>
        /// Get/set the zone name
        /// </summary>
        public string ZoneName { get; set; }

        /// <summary>
        /// Clear child controls
        /// </summary>
        public void Clear()
        {
            Controls.Clear();
        }

        /// <summary>
        /// Add a control to the zone.
        /// </summary>
        /// <param name="control"></param>
        public void AddControl(Control control)
        {
            if (control != null)
            {
                Controls.Add(control);
            }
        }

        /// <summary>
        /// Remove a control from the zone.
        /// </summary>
        /// <param name="control"></param>
        public void RemoveControl(Control control)
        {
            if (control != null && Controls.Contains(control))
            {
                Controls.Remove(control);
            }
        }

        /// <summary>
        /// Render the control
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (LayoutDesignMode)
            {
                if (DesignBlockMode)
                {
                    writer.Write("<fieldset><legend class=\"answer\">" + ZoneName + "</legend><br />");
                }
                else
                {
                    writer.Write("<fieldset style=\"display:inline;\"><legend class=\"answer\">" + ZoneName + "</legend><br />");
                }
            }
            
            base.Render(writer);

            if (LayoutDesignMode)
            {
                writer.Write("<br /></fieldset>");
            }
        }
    }
}
