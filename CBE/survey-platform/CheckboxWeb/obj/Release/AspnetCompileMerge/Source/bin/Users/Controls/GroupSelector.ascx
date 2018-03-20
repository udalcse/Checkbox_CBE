<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GroupSelector.ascx.cs" Inherits="CheckboxWeb.Users.Controls.GroupSelector" %>
<%@ Import Namespace="Checkbox.Web" %>

<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/users/general.js" />

<script type="text/javascript" language="javascript">
    function addToPostback(func) {
        var old__doPostBack = __doPostBack;
        if (typeof __doPostBack != 'function') {
            __doPostBack = func;
        } else {
            __doPostBack = function(t, a) {
                if (func(t, a)) {
                    old__doPostBack(t, a);
                }
            };
        }
    }

    $(document).ready(function () {
        $('#<%=_selectedGroupsTxt.ClientID %>').val('');
        addToPostback(function (t, a) {
            $('#<%=_selectedGroups.ClientID %> option').each(function (index) {
                var theValue = $(this).val();

                if (index > 0) {
                    theValue = ',' + theValue;
                }

                $('#<%=_selectedGroupsTxt.ClientID %>').val($('#<%=_selectedGroupsTxt.ClientID %>').val() + theValue);
            });

            return true;
        });

        var listTransfer = new SelectListTransfer();
        listTransfer.init('<%=_availableGroupList.ClientID %>', 'available_group_list', 'availableFilterTxt', '<%=_selectedGroups.ClientID %>', 'selected_group_list', 'currentFilterTxt');

        $.configureBoxes({
            box1View: '<%=_availableGroupList.ClientID %>',
            box2View: '<%=_selectedGroups.ClientID %>',
            to1: '<%=_moveLeftButton.ClientID %>',
            to2: '<%=_moveRightButton.ClientID %>',
            box1Filter: 'availableFilterTxt',
            box2Filter: 'currentFilterTxt',
            box1Storage: 'availableStorage',
            box2Storage: 'currentGroupStorage',
            box1Clear: '<%=_availableSearchCancelBtn.ClientID %>',
            box2Clear: '<%=_currentSearchCancelBtn.ClientID %>'
        });
    });
</script>

<asp:Panel ID="_groupSelectorPanel" runat="server" Visible="true">
    <div style="display:none;">
        <asp:TextBox ID="_selectedGroupsTxt" runat="server" />
    </div>
    <div style="margin: 10px 10px 0; width:600px;">
        <div class="left aclAccumulatorWrapper" style="width:275px;margin-right:10px;">
            <div class="blueBackground clearfix" style="text-align:center;">
                <h5 style="float: left; line-height: 2em; margin: 5px 0 0 10px;"><%=WebTextManager.GetText("/controlText/users/controls/groupSelector/available")%></h5>
                <div class="aclFilter right">
                    <%--<ckbx:MultiLanguageLabel ID="_filterLbl" runat="server" TextId="/controlText/AccessListEditor.ascx/filter"></ckbx:MultiLanguageLabel>--%>
                    <input type="text" id="availableFilterTxt" />
                    <ckbx:MultiLanguageImageButton runat="server" ID="_availableSearchCancelBtn" SkinID="ACLFilterOff" OnClientClick="javascript:return false;" ToolTipTextId="/controlText/AccessListEditor.ascx/filterOffTip" />
                </div>
            </div>
            <ul id="available_group_list" class="transfer-select-list left-list">
            
            </ul>
            <div style="display:none;">
                <asp:listbox ID="_availableGroupList" CssClass="multiple-select" runat="server" Width="335" Height="300" SelectionMode="Multiple" OnDataBound="AvailableGroupList_DataBound"></asp:listbox>
                <select id="availableStorage"></select>
            </div>
        </div><!--
        <div class="left" style="margin-top:200px;margin-left:10px;margin-right:10px;visibility: hidden;">
            <btn:CheckboxButton ID="_moveRightButton" runat="server" TextID="/controlText/users/controls/groupSelector/moveRight" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClientClick="javascript:return false;" /><br /><br />
            <btn:CheckboxButton ID="_moveLeftButton" runat="server" TextID="/controlText/users/controls/groupSelector/moveLeft" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton"  OnClientClick="javascript:return false;" />
        </div>-->
        <div class="left aclAccumulatorWrapper" style="width:275px;">
            <div class="blueBackground clearfix" style="text-align:center;">
                <h5 style="float: left; line-height: 2em; margin: 5px 0 0 10px;"><%=WebTextManager.GetText("/controlText/users/controls/groupSelector/selectedGroups")%></h5>
                <div class="aclFilter right">
                    <%--<ckbx:MultiLanguageLabel ID="_currentFilterLbl" runat="server" TextId="/controlText/AccessListEditor.ascx/filter"></ckbx:MultiLanguageLabel>--%>
                    <input type="text" id="currentFilterTxt" />
                    <ckbx:MultiLanguageImageButton runat="server" ID="_currentSearchCancelBtn" SkinID="ACLFilterOff" OnClientClick="javascript:return false;" ToolTipTextId="/controlText/AccessListEditor.ascx/filterOffTip" />
                </div>
            </div>
            <ul id="selected_group_list" class="transfer-select-list right-list">
            
            </ul>
            <div style="display:none;">
                <asp:ListBox ID="_selectedGroups" CssClass="multiple-select" runat="server" Width="335" Height="300" SelectionMode="Multiple" OnDataBound="SelectedGroupList_DataBound"></asp:listbox>
                <select id="currentGroupStorage"></select>
            </div>
        </div>
        <br class="clear" />
    </div>
</asp:Panel>
<ckbx:MultiLanguageLabel ID="_noGroupsLabel" runat="server" TextId="/controlText/users/controls/groupSelector/noGroups" Visible="false"></ckbx:MultiLanguageLabel>