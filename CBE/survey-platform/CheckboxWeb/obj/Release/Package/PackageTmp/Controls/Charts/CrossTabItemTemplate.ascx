<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CrossTabItemTemplate.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.CrossTabItemTemplate" %>

<asp:PlaceHolder ID="_templatePlace" runat="server">
    <div style="float:left;">
        boo
        <asp:Label ID="_answerCountLbl" runat="server" />
    </div>
    <div style="float:left;">
        yeah
        <asp:Label ID="_answerPercentLbl" runat="server" />
    </div>
    <div class="clear"></div>
</asp:PlaceHolder>