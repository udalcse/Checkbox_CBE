<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Analysis_Details.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.Analysis_Details" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_widthTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_heightTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>

<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_widthTxt" ID="_widthLbl" runat="server" TextId="/controlText/AppearanceEditor/width" /></p>
    <asp:TextBox ID="_widthTxt" runat="server" Width="50px"/>
    
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_heightTxt" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/AppearanceEditor/height" /></p>
    <asp:TextBox ID="_heightTxt" runat="server" Width="50px" />
</div>