<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Status.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Status" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/TimeLimits.ascx" TagName="TimeLimits" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Surveys/Controls/ResponseLimits.ascx" TagName="ResponseLimits" TagPrefix="ckbx" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content ID="_head" ContentPlaceHolderID="_headContent" runat="server">
    <script type="text/javascript" language="javascript">
        function showActivateConfirm(src) {
            showConfirmDialog(src, false, '<%=WebTextManager.GetText("/pageText/surveyStatus.aspx/activateConfirm") %>');
        }

        function showDeactivateConfirm(src) {
            showConfirmDialog(src, false, '<%=WebTextManager.GetText("/pageText/surveyStatus.aspx/deactivateConfirm") %>');
        }
    </script>
</asp:Content>

<asp:Content ID="_pageContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/surveyStatus.aspx/currentStatus") %></div>
        <div style="background-color: white;margin:2px;padding:3px;">
            <asp:Label ID="_statusLbl" runat="server"  />
        </div>
        
        <div class="clear" style="height:25px;"></div>
        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/surveyStatus.aspx/pauseSurvey") %></div>
        <div>
            <div style="margin:3px;">
                <asp:Label ID="_pauseLbl" runat="server" />
            </div>
            
            <br class="clear" />

            <btn:CheckboxButton ID="_activateBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextId="/pageText/surveyStatus.aspx/activate" />
            <btn:CheckboxButton ID="_deactivateBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextId="/pageText/surveyStatus.aspx/deactivate" />
        </div>
        <div class="clear" style="height:25px;"></div>
            
        <ckbx:TimeLimits ID="_timeLimits" runat="server" />
    
        <div class="clear" style="height:25px;"></div>
        
        <ckbx:ResponseLimits ID="_responseLimits" runat="server" />
    </div>
</asp:Content>
