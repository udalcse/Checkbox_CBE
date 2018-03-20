<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Message.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Message" %>
<%@ Register Src="~/Forms/Surveys/Controls/TermRenderer.ascx" TagPrefix="ckbx" TagName="TermRenderer" %>

<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms" %>

<asp:Panel ID="_messagePanel" CssClass="MessageItem" runat="server">
    <div class="itemContent textContainer Message" style="width: 100%;">
        
        <%= Utilities.CustomDecode(Utilities.StripIframes(Model.Text).Replace("<iframe", "<div class='iframe'").Replace("</iframe>", "</div>"))%>
        

    </div>
</asp:Panel>

<ckbx:TermRenderer runat="server" ID="_termRenderer"/>

<script type="text/javascript">
    $(function() {
        $('#<%= _messagePanel.ClientID %> p').each(function(i, elem) {
            if ($(elem).css('text-align') != 'left') {
                $('#<%= _messagePanel.ClientID %>').css('text-align', 'center');
                return false;
            }
        });
    });
</script>
