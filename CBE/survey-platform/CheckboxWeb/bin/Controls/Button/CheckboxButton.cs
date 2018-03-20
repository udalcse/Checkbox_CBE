using System;
using System.Web.UI.WebControls;
using Checkbox.Web;

namespace CheckboxWeb.Controls.Button
{
    /// <summary>
    /// Button that uses sliding doors technique to show a pretty button
    /// - Also localizes text
    /// </summary>
    public class CheckboxButton : CompositeControl, IButtonControl
    {
        private LinkButton _button;
        private string _textId;
        private string _toolTipTextId;
        private static readonly object EventCommandKey = new object();
        private static readonly object EventClickKey = new object();

        #region Properties

        public String CommandName
        {
            get
            {
                EnsureChildControls();
                return _button.CommandName;
            }
            set
            {
                EnsureChildControls();
                _button.CommandName = value;
            }
        }

        public String CommandArgument
        {
            get
            {
                EnsureChildControls();
                return _button.CommandArgument;
            }
            set
            {
                EnsureChildControls();
                _button.CommandArgument = value;
            }
        }

        public String OnClientClick
        {
            get
            {
                EnsureChildControls();
                return _button.OnClientClick;
            }
            set
            {
                EnsureChildControls();
                _button.OnClientClick = value;
            }
        }

	    public Boolean CausesValidation
        {
            get
            {
                EnsureChildControls();
                return _button.CausesValidation;
            }
            set
            {
                EnsureChildControls();
                _button.CausesValidation = value;
            }
        }

        public String Text
        {
            get
            {
                EnsureChildControls();
                return _button.Text;
            }
            set
            {
                EnsureChildControls();
                _button.Text = value;
            }
		}

        public String TextId
        {
            get
            {
                EnsureChildControls();
                return _textId;
            }
            set
            {
                EnsureChildControls();
                _textId = value;

                if (!string.IsNullOrEmpty(_textId))
                {
                    _button.Text = WebTextManager.GetText(_textId);
                }
            }
        }

        public String ToolTipTextId
        {
            get
            {
                EnsureChildControls();
                return _toolTipTextId;
            }
            set
            {
                EnsureChildControls();
                _toolTipTextId = value;

                if (string.IsNullOrEmpty(_toolTipTextId))
                {
                    _button.ToolTip = WebTextManager.GetText(_toolTipTextId);
                }
            }
        }

		public override Unit Width
        {
            get
            {
                EnsureChildControls();
                return _button.Width;
            }
            set
            {
                EnsureChildControls();
                _button.Width = value;
            }
        }

        public override Unit Height
        {
            get
            {
                EnsureChildControls();
                return _button.Height;
            }
            set
            {
                EnsureChildControls();
                _button.Height = value;
            }
        }

        public string LinkButtonName
        {
            get
            {
                EnsureChildControls();
                return UniqueID + "$ctl00";
            }
        }
        

        #endregion

        protected override void CreateChildControls()
        {
            Controls.Clear();
            CreateControlHierarchy();
            ClearChildViewState();
        }

        protected virtual void CreateControlHierarchy()
        {
            _button = new LinkButton();

            _button.Command += Button_Command;
            _button.Click += Button_Click;
            Controls.Add(_button);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {

			foreach (String attributeName in Attributes.Keys)
            {
                _button.Attributes.Add(attributeName, Attributes[attributeName]);
            }

            _button.Attributes.Add("id", ClientID);
            _button.Attributes.Add("name", LinkButtonName);

            _button.CssClass = CssClass;
            _button.RenderControl(writer);
        }



        #region Event handling

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Button_Command(object sender, CommandEventArgs e)
        {
            OnCommand(new CommandEventArgs(CommandName, CommandArgument));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Button_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        protected virtual void OnCommand(CommandEventArgs e)
        {
            CommandEventHandler commandHandler = (CommandEventHandler)Events[EventCommandKey];
            if (commandHandler != null)
            {
                commandHandler(this, e);
            }
        }

        protected virtual void OnClick(EventArgs e)
        {
            EventHandler clickHandler = (EventHandler)Events[EventClickKey];
            if (clickHandler != null)
            {
                clickHandler(this, e);
            }
        }

        #endregion

        #region IButtonControl Members


        public event EventHandler Click
        {
            add
            {
                Events.AddHandler(EventClickKey, value);
            }
            remove
            {
                Events.RemoveHandler(EventClickKey, value);
            }
        }

        public event CommandEventHandler Command
        {
            add
        {
                Events.AddHandler(EventCommandKey, value);
            }
            remove
            {
                Events.RemoveHandler(EventCommandKey, value);
            }
        }

        public string PostBackUrl
        {
            get
            {
                EnsureChildControls();
                return _button.PostBackUrl;
            }
            set
            {
                EnsureChildControls();
                _button.PostBackUrl = value;
            }
        }

        public string ValidationGroup
        {
            get
            {
                EnsureChildControls();
                return _button.ValidationGroup;
            }
            set
            {
                EnsureChildControls();
                _button.ValidationGroup = value;
            }
        }

        #endregion
    }
}
