<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AddressVerifierBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.AddressVerifierBehavior" %>
<%@ Import Namespace="Checkbox.Web" %>


<!-- Region -->
<div class="itemEditorLabel_150">
    <ckbx:MultiLanguageLabel ID="_regionLbl" runat="server" TextId="/controlText/addressVerifierEditor/region" />
</div>

<div class="itemEditorInput">
    <asp:DropDownList id="_regionList" runat="server">
        <asp:ListItem Value="" Text="Any Region" />
        <asp:ListItem Value="1" Text="Auckland Regional Council" />
        <asp:ListItem Value="2" Text="Bay of Plenty Regional Council" />
        <asp:ListItem Value="3" Text="Canterbury Regional Council" />
        <asp:ListItem Value="4" Text="Gisborne District Council" />
        <asp:ListItem Value="5" Text="Hawke's Bay Regional Council" />
        <asp:ListItem Value="6" Text="Manawatu-Wanganui Regional Council" />
        <asp:ListItem Value="7" Text="Marlborough District Council" />
        <asp:ListItem Value="8" Text="Nelson City Council" />
        <asp:ListItem Value="9" Text="Northland Regional Council" />
        <asp:ListItem Value="A" Text="Otago Regional Council" />
        <asp:ListItem Value="B" Text="Southland Regional Council" />
        <asp:ListItem Value="C" Text="Taranaki Regional Council" />
        <asp:ListItem Value="D" Text="Tasman District Council" />
        <asp:ListItem Value="E" Text="Waikato Regional Council" />
        <asp:ListItem Value="F" Text="Wellington Regional Council" />
        <asp:ListItem Value="G" Text="West Coast Regional Council" />
        <asp:ListItem Value="H" Text="Outside of a Regional Council" />
    </asp:DropDownList>
</div>

<div style="clear:both"></div>

<!-- Search Type -->
<div class="itemEditorLabel_150">
    <ckbx:MultiLanguageLabel ID="_searchTypeLbl" runat="server" TextId="/controlText/addressVerifierEditor/searchType" />
</div>

<div class="itemEditorInput">
    <ckbx:MultiLanguageDropDownList ID="_searchType" runat="server" class="formatDropdown" >
        <asp:ListItem TextId="/controlText/addressVerifierEditor/searchType/location" Value="location" Selected="True"/>
        <asp:ListItem TextId="/controlText/addressVerifierEditor/searchType/dwelling" Value="dwelling"/>
        <asp:ListItem TextId="/controlText/addressVerifierEditor/searchType/postal_address" Value="postal_address"/>
        <asp:ListItem TextId="/controlText/addressVerifierEditor/searchType/combined_address" Value="combined_address"/>
        <asp:ListItem TextId="/controlText/addressVerifierEditor/searchType/post_box" Value="post_box"/>
    </ckbx:MultiLanguageDropDownList>
</div>

<div style="clear:both"></div>

<!-- Search Rule -->
<div class="itemEditorLabel_150">
    <ckbx:MultiLanguageLabel ID="_ruleLbl" runat="server" TextId="/controlText/addressVerifierEditor/rule" />
</div>

<div class="itemEditorInput">
    <ckbx:MultiLanguageDropDownList ID="_rule" runat="server" class="formatDropdown" >     
        <asp:ListItem TextId="/controlText/addressVerifierEditor/rule/strict" Value="strict" Selected="True"/>
        <asp:ListItem TextId="/controlText/addressVerifierEditor/rule/partial" Value="partial"/>
    </ckbx:MultiLanguageDropDownList>
</div>

<div style="clear:both"></div>

<!-- Rural -->
<div class="itemEditorLabel_150">
    <ckbx:MultiLanguageLabel ID="_ruralLbl" runat="server" TextId="/controlText/addressVerifierEditor/rural" />
</div>

<div class="itemEditorInput">
    <ckbx:MultiLanguageDropDownList ID="_rural" runat="server" class="formatDropdown" >
        <asp:ListItem TextId="/controlText/addressVerifierEditor/rural/both" Value="both" Selected="True"/>
        <asp:ListItem TextId="/controlText/addressVerifierEditor/rural/urban_only" Value="urban_only"/>
        <asp:ListItem TextId="/controlText/addressVerifierEditor/rural/rural_only" Value="rural_only"/>
    </ckbx:MultiLanguageDropDownList>
</div>

<div style="clear:both"></div>

<!-- Required -->
<div class="itemEditorLabel_150">
    <ckbx:MultiLanguageLabel ID="_requiredLbl" runat="server" TextId="/controlText/addressVerifierEditor/required" />
</div>

<div class="itemEditorInput">
    <asp:CheckBox ID="_requiredChk" runat="server" />
</div>

<div style="clear:both;"></div>

<asp:Panel ID="_updatedControls" runat="server">
    <!-- Alias -->
    <div class="itemEditorLabel_150">
        <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" />
    </div>

    <div class="itemEditorInput">
        <asp:TextBox ID="_aliasText" runat="server" />
    </div>
<div style="clear:both;"></div>

<!-- Default Text -->

<div class="itemEditorLabel_150">
    <ckbx:MultiLanguageLabel ID="_defaultValueLbl" runat="server" TextId="/controlText/addressVerifierEditor/defaultText" />
</div>

<div class="itemEditorInput">
    <asp:TextBox ID="_defaultTextTxt" runat="server" CssClass="datepicker"/>
</div>

<div style="clear:both;"></div>



</asp:Panel>
