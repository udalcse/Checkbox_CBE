using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Forms.Items.UI;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Labelled control class.  Supports defined labels and input controls
    /// </summary>
    [ParseChildren(false)]
    public class LabelledControl : CompositeControl
    {
        private Unit _itemSpacing;

        //Layout
        private Panel _container;
        private Panel _labelPanel;
        private Panel _inputPanel;

        //Validators
        private List<BaseValidator> _validators;
        private List<Control> _otherControls;

        /// <summary>
        /// 
        /// </summary>
        public LabelledControl()
        {
            LabelPosition = LabelPosition.Left;
        }

        /// <summary>
        /// Get the list of validators
        /// </summary>
        protected List<BaseValidator> Validators
        {
            get
            {
                if (_validators == null)
                {
                    _validators = new List<BaseValidator>();
                }

                return _validators;
            }
        }

        /// <summary>
        /// Other child controls
        /// </summary>
        protected List<Control> OtherControls
        {
            get
            {
                if (_otherControls == null)
                {
                    _otherControls = new List<Control>();
                }

                return _otherControls;
            }
        }

        /// <summary>
        /// Get/set the label position
        /// </summary>
        public virtual LabelPosition LabelPosition { get; set; }

        /// <summary>
        /// Get/set the template for the label
        /// </summary>
        public virtual Control LabelControl { get; set; }

        /// <summary>
        /// Get/set id for input control
        /// </summary>
        public virtual string InputControlID { get; set; }

        /// <summary>
        /// Get/set the template for the input
        /// </summary>
        public virtual Control InputControl { get; set; }

        /// <summary>
        /// Get/set horizontal alignment of label and input within this control
        /// </summary>
        public HorizontalAlign HorizontalAlign { get; set; }

        /// <summary>
        /// Get/set vertical alignment of label and input within this control
        /// </summary>
        public VerticalAlign VerticalAlign { get; set; }

        /// <summary>
        /// Get/set width for label (via it's container)
        /// </summary>
        public Unit LabelWidth { get; set; }

        /// <summary>
        /// Get the style for the container panel
        /// </summary>
        public Unit ItemSpacing
        {
            get { return _itemSpacing; }
            set 
            {
                _itemSpacing = value;
                
                if (_container != null)
                {
                    _container.Style["padding-top"] = _itemSpacing.ToString();
                    _container.Style["padding-bottom"] = _itemSpacing.ToString();
                }
            }
        }

        /// <summary>
        /// Handle building validator collection
        /// </summary>
        /// <param name="obj"></param>
        protected override void AddParsedSubObject(object obj)
        {
            if (obj is BaseValidator)
            {
                Validators.Add((BaseValidator)obj);
            }
            else if(obj is Control)
            {
                OtherControls.Add((Control)obj);
            }
        }
        
        /// <summary>
        /// Create child controls
        /// </summary>
        protected override void CreateChildControls()
        {
            _labelPanel = GetLabelPanel();
            _inputPanel = GetInputPanel();

            //Place the panels...left
            _container = new Panel();

            if (LabelPosition == LabelPosition.Left)
            {
                _labelPanel.Style["float"] = "left";
                _inputPanel.Style["float"] = "left";

                _container.Controls.Add(_labelPanel);
                _container.Controls.Add(_inputPanel);
            }
            else if (LabelPosition == LabelPosition.Right)
            {
                _labelPanel.Style["float"] = "left";
                _inputPanel.Style["float"] = "left";

                _container.Controls.Add(_labelPanel);
                _container.Controls.Add(_labelPanel);
            }
            else if (LabelPosition == LabelPosition.Bottom)
            {
                _container.Controls.Add(_inputPanel);
                _container.Controls.Add(_labelPanel);
            }
            else if (LabelPosition == LabelPosition.Top)
            {
                _container.Controls.Add(_inputPanel);
                _container.Controls.Add(_labelPanel);
            }

            if (!_itemSpacing.IsEmpty)
            {
                _container.Style["padding-top"] = _itemSpacing.ToString();
                _container.Style["padding-bottom"] = _itemSpacing.ToString();
            }

            _container.Style["clear"] = "both";
            Controls.Add(_container);
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();

            Control labelControl = GetLabelControl();
            Control inputControl = GetInputControl();

            if (labelControl != null)
            {
                _labelPanel.Controls.Add(labelControl);
            }

            if (LabelWidth != null)
            {
                _labelPanel.Width = LabelWidth;
            }

            //Add the single input control & validators
            if (inputControl != null)
            {
                if (Utilities.IsNotNullOrEmpty(InputControlID))
                {
                    inputControl.ID = InputControlID;
                }

                _inputPanel.Controls.Add(inputControl);


                //Add validators for the input control
                foreach (BaseValidator validator in Validators)
                {
                    _inputPanel.Controls.Add(validator);
                }
            }
            else
            {
                //Add child controls and validators
                foreach (Control control in OtherControls)
                {
                    _inputPanel.Controls.Add(control);

                    //Add validators
                    foreach (BaseValidator validator in Validators)
                    {
                        if (validator.ControlToValidate.Equals(control.ID, StringComparison.InvariantCultureIgnoreCase))
                        {
                            _inputPanel.Controls.Add(validator);
                        }
                    }
                }
            }

            base.OnInit(e);
        }

        

        /// <summary>
        /// Get the label control
        /// </summary>
        /// <returns></returns>
        protected virtual Panel GetLabelPanel()
        {
            return new Panel();
        }

        /// <summary>
        /// Get the label control
        /// </summary>
        /// <returns></returns>
        protected virtual Control GetLabelControl()
        {
            return LabelControl;
        }
        
        /// <summary>
        /// Get the input control
        /// </summary>
        /// <returns></returns>
        protected virtual Panel GetInputPanel()
        {
            return new Panel();
        }

        /// <summary>
        /// Get the input control
        /// </summary>
        /// <returns></returns>
        protected virtual Control GetInputControl()
        {
            return InputControl;
        }
    }
}
