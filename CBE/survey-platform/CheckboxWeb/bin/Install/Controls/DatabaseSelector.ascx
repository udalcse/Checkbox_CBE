<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatabaseSelector.ascx.cs" Inherits="CheckboxWeb.Install.Controls.DatabaseSelector" %>
    <script type="text/javascript">

        function EnableValidators<%=ClientID%>(enable)
        {
            var index = $('#<%=_currentTabIndex.ClientID %>').val();
            switch(index) {
                case "0":
                    ValidatorEnable(document.getElementById('<%=_serverTxtValidator.ClientID %>'), enable);
                    ValidatorEnable(document.getElementById('<%=_dbNameTxtValidator.ClientID %>'), enable);
                    ValidatorEnable(document.getElementById('<%=_usernameTxtValidator.ClientID %>'), enable);
                    ValidatorEnable(document.getElementById('<%=_passwordTxtValidator.ClientID %>'), enable);
                    ValidatorEnable(document.getElementById('<%=_freeformConnectionStringValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_trustedServerValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_trustedDbNameValidator.ClientID %>'), false);
                    break;
                case "1":
                    ValidatorEnable(document.getElementById('<%=_serverTxtValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_dbNameTxtValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_usernameTxtValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_passwordTxtValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_freeformConnectionStringValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_trustedServerValidator.ClientID %>'), enable);
                    ValidatorEnable(document.getElementById('<%=_trustedDbNameValidator.ClientID %>'), enable);
                    break;
                default:
                    ValidatorEnable(document.getElementById('<%=_serverTxtValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_dbNameTxtValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_usernameTxtValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_passwordTxtValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_freeformConnectionStringValidator.ClientID %>'), enable);
                    ValidatorEnable(document.getElementById('<%=_trustedServerValidator.ClientID %>'), false);
                    ValidatorEnable(document.getElementById('<%=_trustedDbNameValidator.ClientID %>'), false);
            }
        }

        $(document).ready(function () {
            $('#dbTypeTabs<%=ClientID%>').ckbxTabs({
                tabName: 'dbTypeTabs<%=ClientID%>',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index) {
                    $('#<%=_currentTabIndex.ClientID %>').val(index);
                    EnableValidators<%=ClientID%>(true);
                },
                onTabsLoaded: function(){
                    $('.tabContainer').show();
                    $('.tabContentContainer').show();
                    //$('#dbTypeTabs<%=ClientID%> li:first > a').click();
                }
            });
        });
    </script>

<%if (HorizontalTabs) { %>
<ul id="dbTypeTabs<%=ClientID%>" class="tabContainer tabsMenu" style="border-bottom:1px solid #999;">
    <li><%=SQLServerAuthenticationCaption%></li>
    <li><%=WindowsAuthenticationCaption%></li>
    <li><%=FreeformConnectionStringCaption%></li>
</ul>
<% } else {%>
<ul id="dbTypeTabs<%=ClientID%>" class="tabContainer left side-tabs" style="width:210px;margin:0;">
    <li style="width:100%;">SQL Server Authentication</li>
    <li style="width:100%;">Windows Authentication</li>
    <li style="width:100%;">Freeform Connection String</li>
</ul>
<% }%>
<div style="display:none;"><asp:TextBox ID="_currentTabIndex" runat="server" Text="0" /></div>

<div class="tabContentContainer formInput left padding10" <% if (!HorizontalTabs) { %>style="background: #f3f3f3; border-left: 1px solid #dddddd;"<% } %>>
	<div id="dbTypeTabs<%=ClientID%>-0-tabContent">
        <p><label for="<%=_serverTxt.ClientID%>"><%=ServerTxtLabel %></label></p>
        <asp:TextBox id="_serverTxt" runat="server" Width="400px" CssClass="left"></asp:TextBox>
        <asp:RequiredFieldValidator id="_serverTxtValidator" runat="server" meta:resourcekey="_serverTxtValidator" CssClass="error left message condensed" Display="Dynamic" ControlToValidate="_serverTxt"></asp:RequiredFieldValidator>
        <br class="clear" />
                                
        <p><label for="<%=_dbNameTxt.ClientID%>"><%=DbNameTxtLabel%></label></p>
        <asp:TextBox id="_dbNameTxt" runat="server" Width="400px" CssClass="left"></asp:TextBox>
        <asp:RequiredFieldValidator id="_dbNameTxtValidator" runat="server" meta:resourcekey="_dbNameTxtValidator" CssClass="error left message condensed" Display="Dynamic" ControlToValidate="_dbNameTxt"></asp:RequiredFieldValidator>
        <br class="clear" />
                                
        <p><label for="<%=_usernameTxt.ClientID%>"><%=UsernameTxtLabel%></label></p>
        <asp:TextBox id="_usernameTxt" runat="server" Width="400px" CssClass="left"></asp:TextBox>
        <asp:RequiredFieldValidator id="_usernameTxtValidator" runat="server" meta:resourcekey="_usernameTxtValidator" CssClass="error left message condensed" Display="Dynamic" ControlToValidate="_usernameTxt"></asp:RequiredFieldValidator>
        <br class="clear" />

        <p><label for="<%=_passwordTxt.ClientID%>"><%=PasswordTxtLabel%></label></p>
        <asp:TextBox id="_passwordTxt" runat="server" Width="400px" CssClass="left" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator id="_passwordTxtValidator" runat="server" meta:resourcekey="_passwordTxtValidator" CssClass="error left message condensed" Display="Dynamic" ControlToValidate="_passwordTxt"></asp:RequiredFieldValidator>
        <br class="clear" />

   </div>
	<div id="dbTypeTabs<%=ClientID%>-1-tabContent">
        <p><label for="<%=_trustedServer.ClientID%>"><%=TrustedServerLabel%></label></p>
        <asp:TextBox id="_trustedServer" runat="server" Width="400px" CssClass="left"></asp:TextBox>
        <asp:RequiredFieldValidator id="_trustedServerValidator" runat="server" meta:resourcekey="_trustedServerValidator" CssClass="error left message condensed" Display="Dynamic" ControlToValidate="_trustedServer" Enabled="false"></asp:RequiredFieldValidator>
        <br class="clear" />

        <p><label for="<%=_trustedDbName.ClientID%>"><%=TrustedDbNameLabel%></label></p>
        <asp:TextBox id="_trustedDbName" runat="server" Width="400px" CssClass="left"></asp:TextBox>
        <asp:RequiredFieldValidator id="_trustedDbNameValidator" runat="server" meta:resourcekey="_trustedDbNameValidator" CssClass="error left message condensed" Display="Dynamic" ControlToValidate="_trustedDbName" Enabled="false"></asp:RequiredFieldValidator>                                          
        <br class="clear" />
    </div>
	<div id="dbTypeTabs<%=ClientID%>-2-tabContent">
        <p><label for="<%=_freeformConnectionString.ClientID%>"><%=FreeformConnectionStringLabel%></label></p>
        <asp:TextBox id="_freeformConnectionString" runat="server" Columns="110" CssClass="left"></asp:TextBox>
        <asp:RequiredFieldValidator id="_freeformConnectionStringValidator" runat="server" meta:resourcekey="_freeformConnectionStringValidator" CssClass="error left message condensed" Display="Dynamic" ControlToValidate="_freeformConnectionString" Enabled="false"></asp:RequiredFieldValidator>
        <br class="clear" />
    </div>
</div>
<script language="C#" runat="server">
    protected override void OnInit(EventArgs e)
    {
        SQLServerAuthenticationCaption = "SQL Server Authentication";
        WindowsAuthenticationCaption = "Windows Authentication";
        FreeformConnectionStringCaption = "Freeform Connection String";
        ServerTxtLabel = "Database Server";
        DbNameTxtLabel = "Database Name";
        UsernameTxtLabel = "Username";
        PasswordTxtLabel = "Password";
        TrustedServerLabel = "Database Server";
        TrustedDbNameLabel = "Database Name";
        FreeformConnectionStringLabel = "Connection String";
        RequiredFieldValidatorMessage = "Required Field";
    }
</script>