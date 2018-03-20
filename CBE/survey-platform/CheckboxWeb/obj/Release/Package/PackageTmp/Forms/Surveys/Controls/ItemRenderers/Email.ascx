<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Email.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Email" %>
<%@ Import Namespace="Checkbox.Management"%>
<%@ Import Namespace="Checkbox.Web"%>

<div style="float:left;">
    <asp:Placeholder ID="_warningPlace" runat="server">
        <div class="warning message" style="padding:5px;margin:3px;">
            <%= WebTextManager.GetText("/controlText/emailItemEditor/emailNotEnabled") %>
        </div>
        <div class="clear"></div>
    </asp:Placeholder>
    
    <br />

    <div class="itemEditorLabel_125">
        <ckbx:MultiLanguageLabel ID="_formatLbl" runat="server" TextId="/controlText/emailItemEditor/emailFormat" />:
    </div>

    <div class="itemEditorInput">
        <asp:Label ID="_format" runat="server" Text='<%# Model.Metadata["MessageFormat"] %>' />
    </div>

    <div class="clear"></div>

    <div class="itemEditorLabel_125">
        <ckbx:MultiLanguageLabel ID="_fromLbl" runat="server" TextId="/controlText/emailItemEditor/emailFrom" />:
    </div>

    <div class="itemEditorInput">
        <asp:Label ID="_from" runat="server" Text='<%# Model.InstanceData["From"] %>' />
    </div>

    <div class="clear"></div>

    <div class="itemEditorLabel_125">
        <ckbx:MultiLanguageLabel ID="_toLbl" runat="server" TextId="/controlText/emailItemEditor/emailTo" />:
    </div>

    <div class="itemEditorInput">
        <asp:Label ID="_to" runat="server" Text='<%# Model.InstanceData["To"] %>' />
    </div>

    <div class="clear"></div>

    <div class="itemEditorLabel_125">
        <ckbx:MultiLanguageLabel ID="_bccLbl" runat="server" TextId="/controlText/emailItemEditor/emailBcc" />:
    </div>

    <div class="itemEditorInput">
        <asp:Label ID="_bcc" runat="server"  Text='<%# Model.InstanceData["Bcc"] %>' />
    </div>

    <div class="clear"></div>

    <div class="itemEditorLabel_125">
        <ckbx:MultiLanguageLabel ID="_subjectLbl" runat="server" TextId="/controlText/emailItemEditor/emailSubject" />:
    </div>

    <div class="itemEditorInput">
        <asp:Label ID="_subject" runat="server" Text='<%# Model.InstanceData["Subject"] %>' />
    </div>

    <div class="clear"></div>

    <div class="itemEditorLabel_125">
        <ckbx:MultiLanguageLabel ID="_bodyLbl" runat="server" TextId="/controlText/emailItemEditor/emailBody" />:
    </div>

    <div class="itemEditorInput">
        <asp:Label ID="_body" runat="server" Text='<%# GetBody() %>' />
    </div>

    <div class="clear"></div>
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

        return Model.InstanceData["Body"];
    }
</script>

