﻿<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NetPromoterScore_Vertical.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.NetPromoterScore_Vertical" %>
<%@ Register Namespace="CheckboxWeb.Controls.ControlsForRepeater" Assembly="CheckboxWeb" TagPrefix="ctrlForRepeater" %>
<%@ Import Namespace="Checkbox.Common"%>


<%-- Renderer for Horizontal Radio Button Scales --%>
<asp:ObjectDataSource 
    ID="_optionsSource" 
    runat="server" 
    SelectMethod="GetNonOtherOptions" 
    TypeName="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RadioButtonScale_Vertical" 
    DataObjectTypeName="Checkbox.Forms.Items.ListOption"  
    OnObjectCreating="OptionsSource_ObjectCreating" />
    
<div class="ratingScaleOptionContainer_vertical" style="height:<%=ContainerHeight %>;">
    <asp:Repeater ID="_horizontalOptionRepeater"  runat="server" DataSourceID="_optionsSource" OnDataBinding="OptionRepeater_DataBinding" OnItemCreated="OptionRepeater_ItemCreated">
        <ItemTemplate>
            <div class="ratingScaleOuterOptionContainer_vertical" style="height:<%= OptionHeight %>;">
                <div class="ratingScaleMiddleOptionContainer_vertical" >
                    <div class="ratingScaleInnerOptionContainer_vertical">
                        <ctrlForRepeater:RadioButtonForRepeater ID="_inputRad" runat="server" GroupName='<%# "ScaleRads_" + Model.ItemId %>' Checked='<%# GetOptionChecked(DataBinder.Eval(Container.DataItem, "OptionId")) %>' /><span class="Answer"><%# DataBinder.Eval(Container.DataItem, "Text") %></span>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    </div>

<div class="ratingScaleTextContainer_vertical" style="height:<%= ContainerHeight %>;">
    <div class="ratingScaleOuterTextContainer_vertical<%= VerticalSeparator? "Separator" : "" %>" style="height:<%= GetTextHeight("start")%>;">
        <div class="ratingScaleMiddleTextContainer_vertical  ratingScaleStartText_vertical">
            <div class="ratingScaleInnerTextContainer_vertical<%= VerticalSeparator? "Separator" : "" %>">
                <span class="answer"><%= Utilities.AdvancedHtmlEncode(Model.InstanceData["StartText"]) %></span>
            </div>
        </div>
    </div>
    <div class="ratingScaleOuterTextContainer_vertical<%= VerticalSeparator? "Separator" : "" %>" style="height:<%= GetTextHeight("middle")%>;">
        <div class="ratingScaleMiddleTextContainer_vertical  ratingScaleMidText_vertical">
            <div class="ratingScaleInnerTextContainer_vertical<%= VerticalSeparator? "Separator" : "" %>">
                <span class="answer"><%= Utilities.AdvancedHtmlEncode(Model.InstanceData["MidText"])%></span>
            </div>
        </div>
    </div>
    <div class="ratingScaleOuterTextContainer_vertical<%= VerticalSeparator? "Separator" : "" %>" style="height:<%= GetTextHeight("end")%>;">
        <div class="ratingScaleMiddleTextContainer_vertical  ratingScaleEndText_vertical" >
            <div class="ratingScaleInnerTextContainer_vertical<%= VerticalSeparator? "Separator" : "" %>">
                <span class="answer"><%= Utilities.AdvancedHtmlEncode(Model.InstanceData["EndText"])%></span>
            </div>
        </div>
    </div>
</div>