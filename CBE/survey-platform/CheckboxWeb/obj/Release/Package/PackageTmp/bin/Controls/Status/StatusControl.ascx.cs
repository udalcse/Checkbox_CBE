using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;

namespace CheckboxWeb.Controls.Status
{
    public delegate void ActionClickEventHandler(object sender, ActionClickEventArgs e);

    public partial class StatusPanel : Checkbox.Web.Common.UserControlBase
    {
        public event ActionClickEventHandler ActionClicked;

        private Unit _height;
        private Unit _width;

        #region Properties

        public String Message
        {
            get { return _messageLabel.Text; }
            set { _messageLabel.Text = value != null ? value.Trim() : String.Empty; }
        }

        //public String ActionText
        //{
        //    get { return _messageAction.Text; }
        //    set
        //    {
        //        _messageAction.Text = value != null ? value.Trim() : String.Empty;
        //        if (_messageAction.Text.Length > 0)
        //        {
        //            _messageAction.Visible = true;
        //        }
        //        else
        //        {
        //            _messageAction.Visible = false;
        //        }
        //    }
        //}

        //public String ActionArgument
        //{
        //    get { return _messageAction.CommandArgument; }
        //    set { _messageAction.CommandArgument = value != null ? value.Trim() : String.Empty; }
        //}

        public Unit Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public StatusMessageType MessageType
        {
            get
            {
                if (ViewState["StatusMessageType"] == null)
                {
                    ViewState["StatusMessageType"] = StatusMessageType.Success;
                }
                return (StatusMessageType)Enum.Parse(typeof(StatusMessageType), ViewState["StatusMessageType"].ToString());
            }
            set
            {
                ViewState["StatusMessageType"] = value;

                String statusPanelClass = FixStatusPanel ? "StatusPanel StatusPanelFixed" : "StatusPanel";

                statusPanelClass = statusPanelClass.Replace("error", "").Replace("warning", "").Replace("success", "").Trim();

                switch (value)
                {
                    case StatusMessageType.Error:
                        _statusPanel.CssClass = statusPanelClass + " error";
                        //_messageAction.CssClass = "StatusAction error";
                        break;
                    case StatusMessageType.Success:
                        _statusPanel.CssClass = statusPanelClass + " success";
                        //_messageAction.CssClass = "StatusAction success";
                        break;
                    default:
                        _statusPanel.CssClass = statusPanelClass + " warning";
                        //_messageAction.CssClass = "StatusAction warning";
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FixStatusPanel
        {
            get;
            set;
        }

        #endregion

        //protected void MessageAction_Click(object sender, EventArgs e)
        //{
        //    OnActionClicked(new ActionClickEventArgs(_messageAction.CommandArgument));
        //}

        protected virtual void OnActionClicked(ActionClickEventArgs e)
        {
            if (ActionClicked != null)
            {
                ActionClicked(this, e);
            }
        }

        public void ShowStatus()
        {
            _statusPanel.Height = _height;
            _statusPanel.Width = _width;
            _statusPanel.Visible = true;
            this.Visible = true;
        }

        public void HideStatus()
        {
            _statusPanel.Visible = false;
            this.Visible = false;
        }
    }

    public class ActionClickEventArgs : EventArgs
    {
        public String CommandArgument { get; set; }

        public ActionClickEventArgs(String commandArgument)
        {
            CommandArgument = commandArgument;
        }
    }
}