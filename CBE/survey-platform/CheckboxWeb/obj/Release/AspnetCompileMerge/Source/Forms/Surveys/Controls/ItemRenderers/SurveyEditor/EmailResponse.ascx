<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EmailResponse.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.EmailResponse" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Management.Licensing.Limits" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Checkbox.Common" %>

<div class="padding10">
    <asp:Placeholder ID="_warningPlace" runat="server">
        <div class="warning message" style="padding:5px;margin:3px;">
            <%= WebTextManager.GetText("/controlText/emailItemEditor/emailNotEnabled") %>
        </div>
        <br class="clear" />
    </asp:Placeholder>

    <asp:Panel id="_availableEmailsWarningPanel" runat="server" CssClass="warning message" style="padding:5px;margin:3px;" Visible="false">
        <asp:Label ID="_availableEmailsWarning" runat="server" /><br />
    </asp:Panel>

    <div class="formInput">
        <p><label><ckbx:MultiLanguageLiteral ID="_formatLbl" runat="server" TextId="/controlText/emailItemEditor/emailFormat" /></label></p>
        <asp:Label ID="_format" runat="server" Text='<%# Model.Metadata["MessageFormat"] %>' />
    </div>


    <div class="formInput">
        <p><label><ckbx:MultiLanguageLiteral ID="_fromLbl" runat="server" TextId="/controlText/emailItemEditor/emailFrom" /></label></p>
        <asp:Label ID="_from" runat="server" Text='<%# Model.InstanceData["From"] %>' />
    </div>

    <div class="formInput">
        <p><label><ckbx:MultiLanguageLiteral ID="_toLbl" runat="server" TextId="/controlText/emailItemEditor/emailTo" /></label></p>
        <asp:Label ID="_to" runat="server" Text='<%# Model.InstanceData["To"] %>' />
    </div>

    <div class="formInput">
        <p><label><ckbx:MultiLanguageLiteral ID="_bccLbl" runat="server" TextId="/controlText/emailItemEditor/emailBcc" /></label></p>
        <asp:Label ID="_bcc" runat="server"  Text='<%# Model.InstanceData["Bcc"] %>' />
    </div>

    <div class="formInput">
        <p><label><ckbx:MultiLanguageLiteral ID="_subjectLbl" runat="server" TextId="/controlText/emailItemEditor/emailSubject" /></label></p>
        <asp:Label ID="_subject" runat="server" Text='<%# Model.InstanceData["Subject"] %>' />
    </div>

    <div class="formInput">
        <p><label><ckbx:MultiLanguageLiteral ID="_bodyLbl" runat="server" TextId="/controlText/emailItemEditor/emailBody" /></label></p>
        <asp:Label ID="_body" runat="server" Text='<%# GetBody() %>' />
    </div>

    <br class="clear" />
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Show/hide email warning
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _warningPlace.Visible = !ApplicationManager.AppSettings.EmailEnabled;

        //Check for remaining emails
        var emailsLimits = new EmailLimit();
        long? currentValue = emailsLimits.CurrentValue;
        long? baseValue = emailsLimits.BaseValue;

        if (currentValue.HasValue && baseValue.HasValue)
        {
            double availablePercent = ((double)currentValue.Value * 100) / baseValue.Value;
            if (availablePercent < ApplicationManager.AppSettings.MinPercentOfAvailableLimit)
            {
                _availableEmailsWarning.Text =
                    WebTextManager.GetText("/controlText/emailItemEditor/availableEmailsWarning", null,
                                           "Count of remaining emails   -----   {0}").Replace("{0}", currentValue.Value.ToString());
                _availableEmailsWarningPanel.Visible = true;
            }
        } 
    }
    
    /// <summary>
    /// Get the body of the message.
    /// </summary>
    /// <returns></returns>
    protected string GetBody()
    {
        //Get body for message
        if ("text".Equals(Model.Metadata["MessageFormat"], StringComparison.InvariantCultureIgnoreCase))
        {
            //If newlines are present, replace them with breaks so they appear
            return Model.InstanceData["Body"].Replace(Environment.NewLine, "<br />");
        }

        return Utilities.ReplaceHtmlAttributes(Model.InstanceData["Body"], true);
    }
</script>

