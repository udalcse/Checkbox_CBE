<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ReportItems.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ReportItems" %>

<div class="left fixed_250">
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_radioButtons" TextId="/pageText/reportItems.ascx/radioButtons">Radio  buttons</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList runat="server" ID="_radioButtons"></ckbx:MultiLanguageDropDownList>
    </div>
    
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_checkboxes" TextId="/pageText/reportItems.ascx/checkboxes">Checkboxes</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList runat="server" ID="_checkboxes"></ckbx:MultiLanguageDropDownList>
    </div>
    
    <asp:Panel runat="server" ID="_userInputItemsPanel">  
        <div class="formInput">
            <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_singleLineText" TextId="/pageText/reportItems.ascx/slt">Single line text</ckbx:MultiLanguageLabel></p>
            <ckbx:MultiLanguageDropDownList runat="server" ID="_singleLineText"></ckbx:MultiLanguageDropDownList>
        </div>

        <div class="formInput">
            <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_multiLineText" TextId="/pageText/reportItems.ascx/mlt">Multi line text</ckbx:MultiLanguageLabel></p>
            <ckbx:MultiLanguageDropDownList runat="server" ID="_multiLineText"></ckbx:MultiLanguageDropDownList>
        </div>
    </asp:Panel>

    <br class="clear"/>
</div>
 <div class="left fixed_250">
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_slider" TextId="/pageText/reportItems.ascx/slider">Slider</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList runat="server" ID="_slider"></ckbx:MultiLanguageDropDownList>
    </div>

    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_netPromoterScore" TextId="/pageText/reportItems.ascx/netPromoterScore">Net Promoter Score</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList Width="200px" runat="server" ID="_netPromoterScore"></ckbx:MultiLanguageDropDownList>
    </div>

     <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_rankOrder" TextId="/pageText/reportItems.ascx/rankOrder">RankOrder</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList runat="server" ID="_rankOrder"></ckbx:MultiLanguageDropDownList>
    </div>
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_ratingScale" TextId="/pageText/reportItems.ascx/ratingScale">Rating scale</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList Width="200px" runat="server" ID="_ratingScale"></ckbx:MultiLanguageDropDownList>
    </div>
    
    <br class="clear"/>
</div>

<div class="left">
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_dropDownList" TextId="/pageText/reportItems.ascx/dropDownList">Drop down list</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList runat="server" ID="_dropDownList"></ckbx:MultiLanguageDropDownList>
    </div>

    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_matrix" TextId="/pageText/reportItems.ascx/matrix">Matrix</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList runat="server" ID="_matrix"></ckbx:MultiLanguageDropDownList>
    </div>
    
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_hiddenItems" TextId="/pageText/reportItems.ascx/hiddenItems">Hidden items</ckbx:MultiLanguageLabel></p>
        <ckbx:MultiLanguageDropDownList runat="server" ID="_hiddenItems"></ckbx:MultiLanguageDropDownList>
    </div>
</div>
<br class="clear"/>
