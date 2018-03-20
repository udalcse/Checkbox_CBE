<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Properties.ascx.cs" Inherits="CheckboxWeb.Styles.Forms.Controls.Properties" %>

<script>
    $(document).ready(
        function() {
            $('#<%=_styleNameTxt.ClientID%>').focus();
        });
</script>

<div class="padding10">
    <div class="dialogSubTitle">Style Settings</div>
    <div class="dialogSubContainer" style="padding-bottom: 0px;">
        
    <div class="left fixed_150">
        <ckbx:MultiLanguageLabel ID="_styleNameLbl" runat="server" TextId="/controlText/styles/styleName" />
    </div>
    
        <asp:TextBox ID="_styleNameTxt" runat="server" Width="350" CssClass="left" />
    <div class="clear"></div>
        

    <div class="left fixed_350" style="position:absolute; left:170px; top:90px;">
        <asp:RequiredFieldValidator Display="Dynamic" ID="_nameRequiredValidator" runat="server" CssClass="error message" ControlToValidate="_styleNameTxt" />
        <asp:CustomValidator Display="Dynamic" ID="_nameInUseValidator" runat="server" CssClass="error message" ControlToValidate="_styleNameTxt" />
    </div>
    <br class="clear" />
        
    <% if (string.IsNullOrEmpty(StyleID))
       {  %>
    <div class="left fixed_150">
        <ckbx:MultiLanguageLabel ID="_baseStyleLbl" runat="server" TextId="/controlText/styles/baseStyle" />
    </div>
    
        <asp:DropDownList ID="_baseStyle" runat="server" DataValueField="TemplateId" DataTextField="Name"/>

    <br class="clear" />
    <%} %>
        </div>
        <div class="dialogSubTitle">Security Settings</div>
    <div class="dialogSubContainer" style="padding-bottom: 0px;">
    <div class="left fixed_150">
        <ckbx:MultiLanguageCheckBox ID="_publicChk" runat="server" TextAlign="Right" TextId="/controlText/styles/public" ></ckbx:MultiLanguageCheckBox>
    </div>
    <br class="clear" />

    <div class="left fixed_150">
        <ckbx:MultiLanguageCheckBox ID="_allowEditChk" runat="server" TextAlign="Right" TextId="/controlText/styles/editable" ></ckbx:MultiLanguageCheckBox>
    </div>
    <br class="clear" />
        </div>
</div>