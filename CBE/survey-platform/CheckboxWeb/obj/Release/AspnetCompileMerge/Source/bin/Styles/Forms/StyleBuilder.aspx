<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="StyleBuilder.aspx.cs" Inherits="CheckboxWeb.Styles.Forms.StyleBuilder" MasterPageFile="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">
    <script language="javascript" type="text/javascript">
        function updateCSS(e) {
            var src = e.target;
            if (typeof (src) == 'undefined')
                src = e.srcElement;
            if (typeof (src) == 'undefined')
                return;

            var n = ($('#' + src.id.replace("Value", "Name"))).text();
            var v = src.value;

            $('#<%=_previewPanel.ClientID%>').css(n, '');
            $('#<%=_previewPanel.ClientID%>').css(n, v);
        }

        $(document).ready(function () {
            $('input.stylePropertyEditor').keypress(function (e) {
                if (e.which == 13) {
                    updateCSS(e);
                }
            });

            $('input.stylePropertyEditor').change(function (e) {
                updateCSS(e);
            });
        });
    </script>

    <div class="padding10">
        <fieldset>
            <legend><ckbx:MultiLanguageLabel ID="_sampleLbl" runat="server" SkinID="NormalWeightLabel" TextId="/pageText/styles/forms/sytleBuilder.aspx/preview" /></legend>
            <asp:Panel ID="_previewPanel" runat="server" Height="80px" ScrollBars="Vertical" CssClass="padding10">
                Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum
            </asp:Panel>
        </fieldset>
        <div style="overflow-y:scroll;">
            <div class="centerContent" style="width:375px;">
                <div class="dialogSubTitle left fixed_150">Property Name</div>
                <div class="dialogSubTitle left fixed_150">Property Value</div>
                <br class="clear" />

                <asp:Repeater runat="server" ID="_propertyRepeater">
                    <HeaderTemplate></HeaderTemplate>
                    <ItemTemplate>
                        <div class="padding5">
                            <div class="left fixed_150"><asp:Label class="stylePropertyEditor" ID="_propertyName" runat="server" SkinID="NormalWeightLabel" Text='<%# DataBinder.Eval(Container.DataItem, "Key") %>' /></div>
                            <div class="left fixed_150"><asp:TextBox class="stylePropertyEditor" ID="_propertyValue" runat="server" SkinID="Normal" Text='<%# DataBinder.Eval(Container.DataItem, "Value") %>' Width="140px" PropertyName='<%# DataBinder.Eval(Container.DataItem, "Key") %>' /></div>
                            <div class="left"><ckbx:MultiLanguageImageButton runat="server" ID="deletePropertyBtn" CommandName="DeleteProperty" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' SkinID="StyleEditorDeleteImage" ToolTipTextId="/pageText/styles/forms/styleBuilder.aspx/deleteProperty" CausesValidation="false" /></div>
                            <br class="clear" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <div class="padding5">
                    <div class="left fixed_150">
                        <asp:TextBox ID="_newPropertyNameTxt" runat="server" SkinID="Normal" Width="140px"></asp:TextBox>
                    </div>
                    <div class="left fixed_150">
                        <asp:TextBox ID="_newPropertyValueTxt" runat="server" SkinID="Normal" Width="140px"></asp:TextBox>
                    </div>
                    <div class="left"><ckbx:MultiLanguageImageButton ID="_addPropertyBtn" runat="server" SkinID="StyleEditorAddProperty" TextId="/pageText/styles/forms/styleBuilder.aspx/addNewProperty" ToolTipTextId="/pageText/styles/forms/styleBuilder.aspx/addNewProperty" Height="16" Width="16" /></div>
                    <br class="clear" />
                </div> 
            </div>
        </div>
    </div>
</asp:Content>