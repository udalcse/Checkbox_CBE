<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="SetDateFilter.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.SetDateFilter" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/jquery-ui-timepicker-addon.js" />
    <ckbx:ResolvingScriptElement ID="_datePickerLocaleResolver" runat="server" />
    
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('#<%=_startDate.ClientID%>').datetimepicker({
                numberOfMonths: 2
            });
            $('#<%=_endDate.ClientID%>').datetimepicker({
                numberOfMonths: 2
            });
            $('#<%=_startDate.ClientID%>').change(function () { updateClearBtn('<%=_startDate.ClientID%>'); });
            $('#<%=_endDate.ClientID%>').change(function () { updateClearBtn('<%=_endDate.ClientID%>'); });
            updateClearBtn('<%=_startDate.ClientID%>');
            updateClearBtn('<%=_endDate.ClientID%>');

            $('.ckbxEditableCancel').click(function (evt) {
                $('#' + $(evt.target).attr('associatedInput')).val('');
                $(evt.target).hide('slow');
            });
        });

        function updateClearBtn(id) {
            if ($('#' + id).val())
                $('[associatedInput="' + id + '"]').show();
            else
                $('[associatedInput="' + id + '"]').hide();
        }
    </script>

    <div class="padding10" >
        <div class="formInput left fixed_200">
            <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_startDate" TextId="/pageText/surveys/reports/setdatefilter.aspx/startDate">Start Date</ckbx:MultiLanguageLabel></p>
            <asp:TextBox ID="_startDate" runat="server"></asp:TextBox>
            <div class="hand checkOff ckbxEditableCancel" title="Cancel" associatedInput="<%=_startDate.ClientID%>" style="display:none">&nbsp;</div>
        </div>
        <div class="formInput left fixed_200" style="padding-left:20px">
            <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" AssociatedControlID="_endDate" TextId="/pageText/surveys/reports/setdatefilter.aspx/endDate">End Date</ckbx:MultiLanguageLabel></p>
            <asp:TextBox ID="_endDate" runat="server"></asp:TextBox>
            <div class="hand checkOff ckbxEditableCancel" title="Cancel" associatedInput="<%=_endDate.ClientID%>" style="display:none">&nbsp;</div>
        </div>
        <br class="clear"/>
    </div>
</asp:Content>