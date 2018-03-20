<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ANALYSIS_NET_PROMOTER_SCORE.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.ANALYSIS_NET_PROMOTER_SCORE" %>
<div class="formInput">
    <div class="left checkBox"><asp:CheckBox ID="_showDetractors" runat="server" /></div>  
    <div class="left"><p><ckbx:MultiLanguageLabel runat="server" TextId="/controlText/NetPromoterScoreItemAppearanceEditor/showDetractors" AssociatedControlID="_showDetractors" /></p></div>
    <br class="clear"/>

    <div class="left checkBox"><asp:CheckBox ID="_showPassive" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/controlText/NetPromoterScoreItemAppearanceEditor/showPassive" AssociatedControlID="_showPassive" /></p></div>
    <br class="clear"/>

    <div class="left checkBox"><asp:CheckBox ID="_showPromoters" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" TextId="/controlText/NetPromoterScoreItemAppearanceEditor/showPromoters" AssociatedControlID="_showPromoters" /></p></div>
    <br class="clear"/>

    <div class="left checkBox"><asp:CheckBox ID="_showNps" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" runat="server" TextId="/controlText/NetPromoterScoreItemAppearanceEditor/showNps" AssociatedControlID="_showNps" /></p></div>
    <br class="clear"/>
</div>