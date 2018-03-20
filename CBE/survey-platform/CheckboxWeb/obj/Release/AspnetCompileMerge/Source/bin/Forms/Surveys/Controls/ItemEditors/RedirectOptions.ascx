<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RedirectOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.RedirectOptions" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>

<script type="text/javascript">
    $(function () {
        $('#<%=_delayTimeTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>

<div class="padding10">
    <div class="formInput radioList" style="padding-bottom:0">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_linkTypeList" ID="_linkTypeLbl" runat="server" TextId="/controlText/redirectItemEditor/linkOption" /></p>
        <ckbx:MultiLanguageRadioButtonList ID="_linkTypeList" runat="server" RepeatColumns="2" RepeatDirection="Vertical" AutoPostBack="true">
            <asp:ListItem Text="Display Link to Url" Value="LinkToUrl" TextId="/controlText/redirectItemEditor/linkToUrl" />
            <asp:ListItem Text="Automatically Redirect to Url" Value="RedirectToUrl" TextId="/controlText/redirectItemEditor/autoRedirectToUrl" />
            <asp:ListItem Text="Display Link to Restart Survey" Value="LinkToRestart" TextId="/controlText/redirectItemEditor/linkToRestart" />
            <asp:ListItem Text="Automatically Restart Survey" Value="AutomaticallyRestart" TextId="/controlText/redirectItemEditor/autoRestart" />
        </ckbx:MultiLanguageRadioButtonList>
    </div>
    
    <br class="clear" />

    <div class="formInput radioList" style="padding-bottom:0">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_linkTargetList" ID="_linkTargetLbl" runat="server" TextId="/controlText/redirectItemEditor/linkOption" /></p>
        <ckbx:MultiLanguageRadioButtonList ID="_linkTargetList" runat="server" RepeatColumns="2" RepeatDirection="Vertical" AutoPostBack="false">
            <asp:ListItem Text="Open in the Same Window" Value="SameWindow" Selected="True" TextId="/controlText/redirectItemEditor/openInTheSameWindow" />
            <asp:ListItem Text="Open in New Window/Tab" Value="NewWindow" TextId="/controlText/redirectItemEditor/openInNewWindowTab"  />
        </ckbx:MultiLanguageRadioButtonList>
    </div>
    
    <br class="clear" />

    <asp:Panel ID="_delayTimePnl" runat="server" >
        <p>
            <ckbx:MultiLanguageLabel AssociatedControlID="_delayTimeTxt" ID="_delayTimeLbl" runat="server" TextId="/controlText/redirectItemEditor/automaticDelay" />
            <asp:TextBox ID="_delayTimeTxt" runat="server"  MaxLength="2" Width="30" />
        </p>
    </asp:Panel>

    <br class="clear" />
    
    <asp:Panel ID="_linkUrlPanel" runat="server">
        <div class="formInput">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_urlTxt" ID="_linkUrlLbl" runat="server" TextId="/controlText/redirectItemEditor/url" /></p>
            <asp:TextBox ID="_urlTxt" runat="server" Width="400" />
            <pipe:PipeSelector ID="_pipeSelectorURL" runat="server" />
        </div>
        
        <br class="clear" />
    </asp:Panel>
    
    <asp:Panel ID="_linkTextPanel" runat="server">
        <div class="formInput" style="padding-bottom:0">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_linkTextTxt" ID="_linkTextLbl" runat="server" TextId="/controlText/redirectItemEditor/linkText" /></p>
            <asp:TextBox ID="_linkTextTxt" runat="server" Width="400" />
            <pipe:PipeSelector ID="_pipeSelectorLink" runat="server" />
        </div>
        
        <br class="clear" />
    </asp:Panel>
</div>