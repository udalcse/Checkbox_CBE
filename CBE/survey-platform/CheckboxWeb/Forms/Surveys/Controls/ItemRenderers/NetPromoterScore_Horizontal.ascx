<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NetPromoterScore_Horizontal.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.NetPromoterScore_Horizontal" %>
<%@ Register Namespace="CheckboxWeb.Controls.ControlsForRepeater" Assembly="CheckboxWeb" TagPrefix="ctrlForRepeater" %>

<%-- Renderer for Horizontal Radio Button Scales --%>
<asp:ObjectDataSource 
    ID="_optionsSource" 
    runat="server" 
    SelectMethod="GetNonOtherOptions" 
    TypeName="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RadioButtonScale_Horizontal" 
    DataObjectTypeName="Checkbox.Forms.Items.ListOption" 
    OnObjectCreating="OptionsSource_ObjectCreating" />

<div style="min-width:<%=GetMinContainerWidth()%>;">
    <div class="net-promoter-score-label">
        <asp:Panel runat="server" ID="_leftText" CssClass="left"><%=Model.InstanceData["StartText"] %></asp:Panel>
        <asp:Panel runat="server" ID="_rightText" CssClass="right"><%=Model.InstanceData["EndText"] %></asp:Panel>
    </div>
    <div style="clear:both;"></div>

    <asp:Repeater ID="_horizontalOptionRepeater" runat="server" DataSourceID="_optionsSource" OnDataBinding="OptionRepeater_DataBinding" OnItemCreated="OptionRepeater_ItemCreated">
        <ItemTemplate>
            <div class="ratingScaleOptionContainer_horizontal" style="width:<%=OptionWidth%>;<%=GetBorderOverride()%>">
                <div class="ratingScaleOptionInput_horizontal">
                    <ctrlForRepeater:RadioButtonForRepeater ID="_inputRad" runat="server" Text="" GroupName='<%# "ScaleRads_" + Model.ItemId %>' Checked='<%# GetOptionChecked(DataBinder.Eval(Container.DataItem, "OptionId")) %>' />
                </div>
                <div class="ratingScaleOptionText_horizontal">
                    <span class="Answer">
                        <asp:Literal ID="_optionText" Text='<%# DataBinder.Eval(Container.DataItem, "Text") %>' runat="server" />
                    </span>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <div style="clear:both;"></div>
        </FooterTemplate>
    </asp:Repeater>
</div>
<div style="clear:both;"></div>
