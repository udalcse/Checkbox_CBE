<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="RecipientReview.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.RecipientReview" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingCssElement runat="server" Source="../../../ControlSkins/ACLEditor/Grid.ACLEditor.css" />
</asp:Content>
<asp:Content ID="page" ContentPlaceHolderID="_pageContent" runat="server">
    <div style="width:100%;height:320px;overflow-y:scroll;overflow-x:auto;">
        <asp:GridView ID="_pendingRecipientsGrid" runat="server" AutoGenerateColumns="False" AllowPaging="True" style="width: 100%">
            <Columns>
                <asp:TemplateField ItemStyle-Width="30">
                    <ItemTemplate>
                        <asp:Image ID="Image1"
                            runat="server" 
                            Height="16"
                            Width="16"
                            ImageUrl='<%# Checkbox.Management.ApplicationManager.ApplicationRoot + "/App_Themes/" + Page.Theme + "/Images/" + DataBinder.Eval(Container.DataItem, "PanelTypeName") + ".png" %>' 
                        />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EmailToAddress" HeaderText="Email"></asp:BoundField>
                <asp:BoundField DataField="UniqueIdentifier" HeaderText="Recipient"></asp:BoundField>
            </Columns>
        </asp:GridView>
    </div>
    <hr />
    <div style="margin-right: auto; margin-left: auto; width: 150px; margin-top:5px;">    
        <btn:CheckboxButton ID="_bottomCloseButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" TextID="/pageText/forms/surveys/invitations/recipientReview.aspx/closeButton" OnClick="CloseBtn_Click" />
        <div class="clear"></div>
    </div>
</asp:Content>
