using System;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Checkbox.Management;
using Checkbox.Web.Page;
using System.Collections.Generic;
using Checkbox.Common;
using CheckboxWeb.Controls.Button;
using Checkbox.Globalization;


namespace CheckboxWeb
{
    public partial class Dialog : BaseMasterPage
    {
         protected override void OnLoad(EventArgs e)
         {
             base.OnLoad(e);

             _datePickerLocaleResolver.Source = "~/Resources/" + GlobalizationManager.GetDatePickerLocalizationFile();

             if (ApplicationManager.AppSettings.TypeKitGuid != null)
                 TypeKit.Text =
                     "<script type=\"text/javascript\" src=\"//use.typekit.net/" + ApplicationManager.AppSettings.TypeKitGuid +
                     ".js\"></script><script type=\"text/javascript\">try{Typekit.load();}catch(e){}</script>";
         }


        /// <summary>
        /// Event fired when "OK" button clicked.
        /// </summary>
        public event EventHandler OkClick;

        /// <summary>
        /// Event fired when "Cancel" button clicked.
        /// </summary>
        public event EventHandler CancelClick;

        public bool ForceLeftButtonAlign { get; set; }

      /// <summary>
        /// 
        /// </summary>
        public Dialog()
        {
            IsDialog = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        public override void ShowError(string error, Exception ex)
        {
            _errorMessageLiteral.Text = error;
            _errorDescriptionLiteral.Text = ex != null ? ex.Message : string.Empty;

            _errorPlace.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void PreventEnterKeyBinding()
        {
            //Add startup script to bind OK click
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "preventEnterKeyBinding",
                "_preventEnterKeyBinding = true;",
                true);
        }

        /// <summary>
        /// Hide buttons built-in to dialog
        /// </summary>
        public void HideDialogButtons()
        {
            PreventEnterKeyBinding();
            _buttonsPanel.Visible = false;
        }

        /// <summary>
        /// Hide buttons built-in to dialog
        /// </summary>
        public void ShowDialogButtons()
        {
            _buttonsPanel.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool RunsInsideUFrame
        {
            get
            {
                return !string.IsNullOrEmpty(Request["uframe"]);
            }
        }

        /// <summary>
        /// Register client scripts
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _templateHelper.Visible = !RunsInsideUFrame;
            _tinymce.Visible = !RunsInsideUFrame;
            _tinyMceUFrameFix.Visible = RunsInsideUFrame;

            //Set EnableEventValidation
            Page.EnableEventValidation = !IsEmbedded;

            //Bind ok click
            _okBtn.Click += _okBtn_Click;
            _cancelBtn.Click += _cancelBtn_Click;
        }


        /// <summary>
        /// Get auth ticket as string
        /// </summary>
        /// <returns></returns>
        private string GetAuthTicket()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            return cookie == null ? string.Empty : cookie.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _cancelBtn_Click(object sender, EventArgs e)
        {
            if (CancelClick != null)
            {
                CancelClick(sender, e);
            }
            else
            {
                if (IsDialog)
                {
                    if (string.IsNullOrWhiteSpace(ClientCancelClickArgs))
                        CloseDialog(string.Empty, false);
                    else
                        CloseDialog(ClientCancelClickArgs, true);
                }
                else
                {
                    Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _okBtn_Click(object sender, EventArgs e)
        {
            if (OkClick != null)
            {
                OkClick(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ClientCancelClickArgs { get; set; }

        /// <summary>
        /// Set page title
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            Page.Title = title;

            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "setTitle",
                "_pageTitle = \"" + title.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("<", "").Replace(">", "") + "\";",
                true);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get { return Page.Title; } set { SetTitle(value); } }

        /// <summary>
        /// 
        /// </summary>
        public void HideStatusMessage()
        {
            _statusControl.HideStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        public void ShowStatusMessage(string message, StatusMessageType messageType)
        {
            if (IsEmbedded)
                _statusControl.FixStatusPanel = false;
            ShowStatusMessage(message, messageType, string.Empty, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="actionText"></param>
        /// <param name="actionArgument"></param>
        public void ShowStatusMessage(string message, StatusMessageType messageType, string actionText, string actionArgument)
        {
            _statusControl.Message = message;
            _statusControl.MessageType = messageType;
            //_statusControl.ActionText = actionText;
            //_statusControl.ActionArgument = actionArgument;
            _statusControl.ShowStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool OkVisible
        {
            get { return _okBtn.Visible; }
            set { _okBtn.Visible = value; checkLastButtonCentered(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CancelVisible
        {
            get { return _cancelBtn.Visible; }
            set { _cancelBtn.Visible = value; checkLastButtonCentered(); }
        }

        /// <summary>
        /// If one button is hidden -- center the remained one
        /// </summary>
        private void checkLastButtonCentered()
        {
            if (!_okBtn.Visible)
                _cancelBtn.CssClass = _cancelBtn.CssClass.Replace("right", "");
            if (!_cancelBtn.Visible)
                _okBtn.CssClass = _cancelBtn.CssClass.Replace("left", "");
        }

        protected string ButtonWrapperClass
        {
            get
            {
                return ((!_okBtn.Visible || !_cancelBtn.Visible) && !ForceLeftButtonAlign) ? "buttonWrapperCenter" : "buttonWrapper";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CancelTextId
        {
            get { return _cancelBtn.TextId; }
            set { _cancelBtn.TextId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OkTextId
        {
            get { return _okBtn.TextId; }
            set { _okBtn.TextId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool OkEnable
        {
            get { return _okBtn.Enabled; }
            set { _okBtn.Enabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OkClientID
        {
            get { return _okBtn.ClientID; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CancelEnable
        {
            get { return _cancelBtn.Enabled; }
            set { _cancelBtn.Enabled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CancelClientID
        {
            get { return _cancelBtn.ClientID; }
        }

        ///<summary>
        /// Use this property to alter the css class of the cancel button
        /// </summary>
        public string CancelButtonClass
        {
            get { return _cancelBtn.CssClass; }
            set { _cancelBtn.CssClass = value; }
        }

        ///<summary>
        /// Get callback for on client close
        /// </summary>
        public string OnClientClose
        {
            get { return Utilities.AdvancedHtmlEncode(Request["onClose"]); }
        }


        /// <summary>
        /// Close dialog window with specified argument
        /// </summary>
        /// <param name="argument">An argument which will be passed to closeWindowHandler.</param>
        /// <param name="isArgumentJObject">Deternime if the argument is jScript object or simple string.</param>
        public void CloseDialog(string argument, bool isArgumentJObject)
        {
            if (isArgumentJObject)
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow('" + OnClientClose + "'," + argument + ");", true);
            else
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow('" + OnClientClose + "','" + argument + "');", true);
        }

        /// <summary>
        /// Close dialog window. Make a jScriptObject of arguments and send it to the CloseDialog handler.
        /// </summary>
        /// <param name="arguments">Set of arguments which will be sent to CloseDialog handler as jScript object.</param>
        public void CloseDialog(Dictionary<string, string> arguments)
        {
            string jScriptObject = String.Empty;
            if (arguments != null && arguments.Count > 0)
            {
                jScriptObject = arguments.Aggregate("{", (current, argument) => current + String.Format("{0}: '{1}', ", argument.Key, argument.Value));
                jScriptObject = jScriptObject.Substring(0, jScriptObject.Length - 2) + "}";
            }

            if (string.IsNullOrEmpty(jScriptObject))
            {
                jScriptObject = "{}";
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow('" + OnClientClose + "'," + jScriptObject + ");", true);
        }

        /// <summary>
        /// Close dialog window. After the closing the specified callback will be run.
        /// </summary>
        /// <param name="callbackName"></param>
        /// <param name="arguments"></param>
        public void CloseDialog(String callbackName, Dictionary<string, string> arguments)
        {
            string jScriptObject = String.Empty;
            if (arguments != null && arguments.Count > 0)
            {
                jScriptObject = arguments.Aggregate("{", (current, argument) => current + String.Format("{0}: '{1}', ", argument.Key, argument.Value));
                jScriptObject = jScriptObject.Substring(0, jScriptObject.Length - 2) + "}";
            }
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow('" + callbackName +"', " + jScriptObject + ");", true);
        }

        /// <summary>
        /// Get Ok button
        /// </summary>
        public CheckboxButton OkBtn
        {
            get { return _okBtn;}
        }
    }
}
