<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX" %>

<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_directorAverages" ID="_directorAveragesLabel" runat="server" Text="Show/hide director averages" />
    <asp:CheckBox ID="_directorAverages" runat="server" CssClass="leftMargin10" />
    </p>

    <p><ckbx:MultiLanguageLabel ID="titleSizeLbl" AssociatedControlID="_titleSize" runat="server" Text="Font size" /></p>
    <ckbx:MultiLanguageDropDownList ID="_titleSize" runat="server" />
    
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_font" ID="fontLbl" runat="server" Text="Label font" /></p>
    <ckbx:MultiLanguageDropDownList ID="_font" runat="server" />
    
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_gridLine" ID="_gridLineLabel" runat="server" Text="Show/hide lines between individual director cells" />
    <asp:CheckBox ID="_gridLine" runat="server" CssClass="leftMargin10" />
    </p>

    <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemColumnHeader" ID="_itemColumnHeaderLabel" runat="server" Text="Line item column header"></ckbx:MultiLanguageLabel></p>
    <asp:TextBox ID="_itemColumnHeader" runat="server"  Width="250"/>
    
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_averagesColumnHeader" ID="__averagesColumnHeaderLabel" runat="server" Text="Averages column header" /></p>
    <asp:TextBox ID="_averagesColumnHeader" runat="server" Width="250"></asp:TextBox>
    
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_ratingDetailsHeader" ID="_ratingDetailsHeaderLabel" runat="server" Text="Rating details header" /></p>
    <asp:TextBox ID="_ratingDetailsHeader" runat="server" Width="250"></asp:TextBox>
    
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_summaryHeader" ID="_summaryHeaderLabel" runat="server" Text="Summary label" /></p>
    <asp:TextBox ID="_summaryHeader" runat="server" Width="250"></asp:TextBox>

</div>
