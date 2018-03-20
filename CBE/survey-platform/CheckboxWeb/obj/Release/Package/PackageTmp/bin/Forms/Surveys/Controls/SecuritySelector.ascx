<%@ Control Language="C#" CodeBehind="SecuritySelector.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.SecuritySelector" %>
<%@ Register TagPrefix="security" TagName="GrantAccess" Src="~/Controls/Security/GrantAccessControl.ascx" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">

    //Bind click events to radio buttons and apply attributes
    $(document).ready(function () {
        //Set attributes and bind events
        $('#<%=_securitySelection.ClientID %> input').each(function (index) {
            var descIndex = index + 1;
            $(this).attr('descDiv', 'securityType' + descIndex + 'Desc');
            $(this).attr('canTake', 'whoCanTake' + descIndex);
            $(this).attr('descIndex', descIndex);
            $(this).click(toggleDescription);
        });

        //Set initial state
        $('#accessListContainer').hide();
        $('#descriptionContainer div').hide();
        $('#whoCanTakeContainer div').hide();
        $('#<%=_passwordPanel.ClientID %>').hide();
        $('#<%=_respondentsListPanel.ClientID %>').hide();

        var selected = $('#<%=_securitySelection.ClientID %> input:checked');

        if (selected.length > 0) {
            $('#' + selected.attr('descDiv')).show();
            $('#' + selected.attr('canTake')).show();

            //Show password or edit ACL option if necessary
            if (selected.attr('descIndex') == '2') {
                $('#<%=_passwordPanel.ClientID %>').show();
            }

            if (selected.attr('descIndex') == '3') {
                $('#<%=_respondentsListPanel.ClientID %>').show();
            }
        }
    });

    //Show grant access dialog
    function showGrantAccess() {
        _grantSurveyAccessshowGrids();
        $('#permissionSelect').hide('blind', null, 500, function () { $('#accessListContainer').show('blind', null, 500); });
    }
    
    //
    function hideGrantAccess() {
        $('#accessListContainer').hide('blind', null, 500, function () { $('#permissionSelect').show('blind', null, 500); });
    }

    //Toggle description displayed when element clicked
    function toggleDescription() {
        //Hide all divs
        $('#descriptionContainer div').hide();
        $('#whoCanTakeContainer div').hide();
        $('#<%=_passwordPanel.ClientID %>').hide();
        $('#accessListContainer').hide();
        $('#<%=_respondentsListPanel.ClientID %>').hide();

        $('#' + $(this).attr('descDiv')).show();
        $('#' + $(this).attr('canTake')).show();

        //Show password or edit ACL option if s
        if ($(this).attr('descIndex') == '2') {
            $('#<%=_passwordPanel.ClientID %>').show();
        }

        if ($(this).attr('descIndex') == '3') {
            $('#<%=_respondentsListPanel.ClientID %>').show();
        }
    }
</script>
<div id="permissionSelect">
    <div class="dialogSubTitle">
        <div class="left fixed_300">
            <%=WebTextManager.GetText("/controlText/forms/surveys/controls/SecuritySelector/securityType") %>
        </div>
        <div class="left fixed_300">
            <%=WebTextManager.GetText("/controlText/forms/surveys/controls/SecuritySelector/whoCanTake") %>
        </div>
        <br class="clear" />
    </div>
    <div class="dialogSubContainer">
        <div class="left fixed_300">
            <ckbx:MultiLanguageRadioButtonList ID="_securitySelection" runat="server">
                <asp:ListItem Text="Public" TextId="/enum/securityType/public" Value="Public" />
                <asp:ListItem Text="Password Protected" TextId="/enum/securityType/passwordProtected" Value="PasswordProtected" />
                <asp:ListItem Text="Access List" TextId="/enum/securityType/accessControlList" Value="AccessControlList" />
                <asp:ListItem Text="Registered Users" TextId="/enum/securityType/allRegisteredUsers" Value="AllRegisteredUsers" />
                <asp:ListItem Text="InvitationOnly" TextId="/enum/securityType/invitationOnly" Value="InvitationOnly" />
            </ckbx:MultiLanguageRadioButtonList>

            <asp:Panel ID="_passwordPanel" runat="server">
                <div class="formInput">
                    <p><ckbx:MultiLanguageLabel ID="_passwordLbl" runat="server" TextId="/controlText/forms/surveys/controls/SecuritySelector/password" AssociatedControlID="_passwordTxt" /></p>
                    <asp:TextBox ID="_passwordTxt" runat="server" Width="250" />
                </div>
            </asp:Panel>
        
            <asp:Panel ID="_respondentsListPanel" runat="server" CssClass="padding10">        
                <btn:CheckboxButton ID="_editRespondentsList" OnClientClick="showGrantAccess();return false;" runat="server" TextId="/controlText/forms/surveys/controls/SecuritySelector/respondentsList" CssClass="ckbxButton roundedCorners shadow999 border999 orangeButton" />
            </asp:Panel>
        </div>

        <asp:Panel ID="_additionalOptionsPanel" runat="server" CssClass="left fixed_300">
            <div id="whoCanTakeContainer" style="font-weight:bold;">
                <div id="whoCanTake1"><%=WebTextManager.GetText("/controlText/forms/surveys/controls/SecuritySelector/takePublic")%></div>
                <div id="whoCanTake2"><%=WebTextManager.GetText("/controlText/forms/surveys/controls/SecuritySelector/takePasswordProtected")%></div>
                <div id="whoCanTake3"><%=WebTextManager.GetText("/controlText/forms/surveys/controls/SecuritySelector/takeAccessControlList")%></div>
                <div id="whoCanTake4"><%=WebTextManager.GetText("/controlText/forms/surveys/controls/SecuritySelector/takeAllRegisteredUsers")%></div>
                <div id="whoCanTake5"><%=WebTextManager.GetText("/controlText/forms/surveys/controls/SecuritySelector/takeInvitationOnly")%></div>
            </div>
            <%-- Containers for Descriptive Text --%>
            <div id="descriptionContainer" class="shadow999 border999 inset" style="margin-top:10px;padding:5px;background-color:#FEFEFE;">
                <div id="securityType1Desc"><%=WebTextManager.GetText("/enum/securityType/public/description")%></div>
                <div id="securityType2Desc"><%=WebTextManager.GetText("/enum/securityType/passwordProtected/description")%></div>
                <div id="securityType3Desc"><%=WebTextManager.GetText("/enum/securityType/accessControlList/description")%></div>
                <div id="securityType4Desc"><%=WebTextManager.GetText("/enum/securityType/allRegisteredUsers/description")%></div>
                <div id="securityType5Desc"><%=WebTextManager.GetText("/enum/securityType/invitationOnly/description")%></div>
            </div>
        </asp:Panel>
    </div>
    <br class="clear" />
</div>
<div  id="accessListContainer" class="dialogSubContainer">
    <div class="dialogSubTitle">
        <div class="left" style="width:400px;">
            <%=WebTextManager.GetText("/controlText/forms/surveys/controls/SecuritySelector/chooseRespondents")%>
        </div>
         <div class="right">        
            <btn:CheckboxButton ID="_backToPermissions" OnClientClick="hideGrantAccess();return false;" runat="server" TextId="/controlText/forms/surveys/controls/SecuritySelector/backToPermissions" CssClass="ckbxButton roundedCorners shadow999 border999 orangeButton" />
        </div>
        <br class="clear"/>
    </div>
    <br class="clear"/>
    <div>
        <security:GrantAccess ID="_grantSurveyAccess" runat="server" />
    </div>
</div>
