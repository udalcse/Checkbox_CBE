<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Edit.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Periods.Edit" EnableEventValidation="false" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    
    <script language="javascript" type="text/javascript" src="<%=ResolveUrl("~/Resources/DialogHandler.js")%>"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $.datepicker.setDefaults({
            showOn: 'both',
            buttonImageOnly: true,
            buttonImage: '<%=ResolveUrl("~/Resources/CalendarPopup.png") %>',
            buttonText: 'Calendar'
        });

        $("#<%=_start.ClientID%>").datepicker({
            onSelect: function (dateText, inst) {
                var d1 = new Date(dateText);
                var d2 = new Date($("#<%=_finish.ClientID%>").val());

                if (d1 > d2) {
                    $("#<%=_finish.ClientID%>").val(dateText);
                }
            }
        });
        $("#<%=_finish.ClientID%>").datepicker({
            onSelect: function (dateText, inst) {
                var d2 = new Date(dateText);
                var d1 = new Date($("#<%=_start.ClientID%>").val());

                if (d1 > d2) {
                    $("#<%=_start.ClientID%>").val(dateText);
                }
            }
        });

        $("#ui-datepicker-div").css("display", "none");

    });     
                           
</script>

    <div class="dialogSubContainer">
        <div class="formInput">
            <p><label for="<%=_name.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveys/periods/add.aspx/name")%></label></p>
            <asp:TextBox ID="_name" runat="server" width="300px"/>
        </div>
        <div class="formInput">
            <p><label for="<%=_start.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveys/periods/add.aspx/start")%></label></p>
            <asp:TextBox ID="_start" runat="server" CssClass="datepicker"/>
        </div>
        <div class="formInput">
            <p><label for="<%=_finish.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveys/periods/add.aspx/finish")%></label></p>
            <asp:TextBox ID="_finish" runat="server" CssClass="datepicker"/>
        </div>
    </div>

    
</asp:Content>
