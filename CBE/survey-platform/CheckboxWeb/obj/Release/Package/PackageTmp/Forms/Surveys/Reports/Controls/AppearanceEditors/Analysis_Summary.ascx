<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Analysis_Summary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.Analysis_Summary" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_widthTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_heightTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>

<div class="formInput">
    <div class="left"><p><ckbx:MultiLanguageLabel ID="includeAllLbl" AssociatedControlID="_showEmptyChk" runat="server" TextID="/pageText/styles/charts/other.ascx/includeAllAnswers" /></p></div>
    <div class="left checkBox" style="margin-left:15px;"><asp:CheckBox ID="_showEmptyChk" runat="server" /></div>
    <br class="clear"/>
    <br class="clear"/>

    <p style="text-decoration:underline;"><ckbx:MultiLanguageLabel ID="optionsOrderLbl" AssociatedControlID="_optionsOrder" runat="server" TextId="/pageText/styles/charts/other.ascx/optionsOrder" /></p>
    <ckbx:MultiLanguageRadioButtonList ID="_optionsOrder" runat="server" Width="100%">
        <asp:ListItem Value="Survey" Text="(Default) Survey Order" TextId="/pageText/styles/charts/other.ascx/optionsOrder/survey" />
        <asp:ListItem Value="Default" Text="Alphabetic" TextId="/pageText/styles/charts/other.ascx/optionsOrder/default" />
    </ckbx:MultiLanguageRadioButtonList>
    <br class="clear" />    

    <p><ckbx:MultiLanguageLabel ID="_widthLbl" AssociatedControlID="_widthTxt" runat="server" TextId="/controlText/AppearanceEditor/width" /></p>
    <asp:TextBox ID="_widthTxt" Width="50px" runat="server" />

    <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" AssociatedControlID="_heightTxt" runat="server" TextId="/controlText/AppearanceEditor/height" /></p>
    <asp:TextBox ID="_heightTxt" Width="50px" runat="server" />
</div>