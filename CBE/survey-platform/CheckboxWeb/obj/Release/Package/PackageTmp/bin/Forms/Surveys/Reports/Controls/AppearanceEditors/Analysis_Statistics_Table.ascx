<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Analysis_Statistics_Table.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.Analysis_Statistics_Table" %>

<div class="formInput">
    <div class="left checkBox"><asp:CheckBox ID="_showResponses" runat="server" /></div>  
    <div class="left"><p><ckbx:MultiLanguageLabel runat="server" TextId="/controlText/StatisticsItemAppearanceEditor/showResponses" AssociatedControlID="_showResponses" /></p></div>
    <br class="clear"/>

    <div class="left checkBox"><asp:CheckBox ID="_showMean" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/controlText/StatisticsItemAppearanceEditor/showMean" AssociatedControlID="_showMean" /></p></div>
    <br class="clear"/>

    <div class="left checkBox"><asp:CheckBox ID="_showMedian" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" TextId="/controlText/StatisticsItemAppearanceEditor/showMedian" AssociatedControlID="_showMedian" /></p></div>
    <br class="clear"/>

    <div class="left checkBox"><asp:CheckBox ID="_showMode" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" runat="server" TextId="/controlText/StatisticsItemAppearanceEditor/showMode" AssociatedControlID="_showMode" /></p></div>
    <br class="clear"/>

    <div class="left checkBox"><asp:CheckBox ID="_showStdDeviation" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" TextId="/controlText/StatisticsItemAppearanceEditor/showStandardDeviation" AssociatedControlID="_showStdDeviation" /></p></div>
    <br class="clear"/>
</div>