<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="PreviewControls.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.PreviewControls" %>
<%@ Import Namespace="Checkbox.Web"%>


<div class="previewControlsContainer">
    <div>
        <span class="previewControlsTitle"><%=WebTextManager.GetText("/pageText/previewSurvey.aspx/surveyPreview") %></span>
        <a class="tooltip previewControlsText" href="#" data-alignment="free" data-top="32" data-message="<%=WebTextManager.GetText("/pageText/previewSurvey.aspx/previewText") %>"></a>
        <!-- Paging Controls -->
        <asp:ImageButton ID="_prevBtn" runat="server" SkinID="PrevBtn" Visible="false" />
        <asp:DropDownList ID="_pageList" runat="server" data-role="none" />
        <asp:ImageButton ID="_nextBtn" runat="server" SkinID="NextBtn" Visible="false" />            
    </div>
    <div class="previewControlsPagingContainer">
        <div>
            <!-- All/Individual Pages Toggle -->
            <asp:LinkButton ID="_allToggleBtn" runat="server" />
        </div>  
    </div>
</div>
