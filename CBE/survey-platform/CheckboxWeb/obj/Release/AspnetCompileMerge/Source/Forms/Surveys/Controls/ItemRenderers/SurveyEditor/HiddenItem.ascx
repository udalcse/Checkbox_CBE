<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HiddenItem.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.HiddenItem" %>
<%@ Register TagName="HiddenItemEditor" TagPrefix="ckbx" Src="~/Forms/Surveys/Controls/ItemEditors/HiddenItem.ascx" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="padding10">
    <ul class="dashStatsContent allMenu">
        <li class="itemEditorLabel_125">
            <ckbx:MultiLanguageLabel ID="_fieldTypeLbl" runat="server" TextId="/controlText/hiddenItemEditor/hiddenFieldType" />:
        </li>

        <li class="itemEditorInput">
            <asp:Label ID="_fieldType" runat="server" Text='<%# WebTextManager.GetText("/enum/hiddenVariableSource/" + Model.Metadata["VariableSource"]) %>' />
        </li>
        <div class="clear"></div>
    </ul>
    <div class="clear"></div>
    
    <ul class="dashStatsContent allMenu detailZebra">
        <li class="itemEditorLabel_125">
            <ckbx:MultiLanguageLabel ID="_fromLbl" runat="server" TextId="/controlText/hiddenItemEditor/variableName" />:
        </li>

        <li class="itemEditorInput">
            <asp:Label ID="_from" runat="server" Text='<%# Model.Metadata["VariableName"] %>' />
        </li>
        <div class="clear"></div>
    </ul>

    <div class="clear"></div>

    <ul class="dashStatsContent allMenu">
        <li class="itemEditorLabel_125">
            <ckbx:MultiLanguageLabel ID="_toLbl" runat="server" TextId="/controlText/hiddenItemEditor/questionText" />:
        </li>

        <li class="itemEditorInput">
            <asp:Label ID="_to" runat="server" Text='<%# Model.Text %>' />
        </li>
        <div class="clear"></div>
    </ul>

    <div class="clear"></div>
</div>
