<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="false" CodeBehind="Default.aspx.cs" Inherits="CheckboxWeb.Default" %>
<%@ MasterType VirtualPath="~/Admin.Master" %>
<%@ Import Namespace="Checkbox.Configuration.Install" %>
<%@ Import Namespace="Checkbox.Management"%>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="styles" ContentPlaceHolderID="_styleContent" runat="server">
    <style type="text/css">
        .newsLink{color:Green; font-weight:bold;}
        .newslist{text-align:left;}
        .newstitle{	margin-left:3px;font-weight:bold;margin-bottom:3px;	margin-top:3px;}
        .newslist li{line-height:22px;margin-left:15px;}
        .newslist li a{color:black;	line-height:18px;text-decoration: none;}
        .newslist li a:hover{text-decoration:underline;}
        table.limitTable{margin-bottom:15px;margin-top:5px;border:1px dotted silver;}
        table.limitTable th{background-color:#DEDEDE;padding:0px 10px;}
        table.limitTable td.values{text-align:center;}
        table.limitTable tr.highlighted{color:Red;font-weight:bold;}
    </style>
</asp:Content>

<asp:Content ID="scripts" ContentPlaceHolderID="_scriptContent" runat="server">
    <script type="text/javascript">
        /*$(document).ready(function () {
            resize();
            $(window).resize(resize);
        });
        function resize() {
            $('.rightPanel').height($(window).height() - 52 - 28);
        }*/
    </script>
</asp:Content>

<asp:Content ID="_pageContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="rightPanel introPage">
        <asp:Panel ID="_anonymousView" runat="server" Visible="false" CssClass="padding15">
            <p>
                <%=AnonymousWelcomeMessage%>
            </p>
            <p>
                <%=CommonWelcomeMessage%>
            </p>
        </asp:Panel>

        <asp:Panel ID="_loggedInView" runat="server" Style="padding:15px;">
            <div style="margin-left: auto; margin-right: auto; width: 80%;">
                <h3>Welcome, <asp:LoginName ID="_loginName" runat="server" FormatString="{0}" /></h3>
                <asp:PlaceHolder ID="_hostingWarningPlace" runat="server">
                    <div style="margin-bottom:15px;">
                        <asp:Panel ID="_hostingWarningMessagePanel" runat="server" Visible="false" style="text-align:center;padding: 5px; margin-left: auto; margin-right: auto; width: 80%; text-align: center; font-family: Arial, Helvetica, sans-serif; font-weight: normal; font-size: small;">
                            <strong>Your account will expire in <asp:Literal ID="_hostingExpirationWarningDays" runat="server" /> days on <asp:Literal ID="_hostingExpirationWarningDate" runat="server" />.</strong>
                            <br />
                            <asp:Literal ID="_renewMsg" runat="server" />
                            <br />
                            
                        </asp:Panel>

                        <asp:Panel ID="_hostingExpirationPanel" runat="server" style="text-align:center;padding: 5px; margin-left: auto; margin-right: auto; width: 80%; text-align: center; font-family: Arial, Helvetica, sans-serif; color: black; font-weight: normal; font-size: small;">
                            <strong>Your Checkbox Online account is active until: <asp:Literal ID="_hostingExpirationDate" runat="server" />.</strong>
                        </asp:Panel>                        
                    </div>
                </asp:PlaceHolder>
                <asp:Panel ID="_timeZoneNotSetWarning" runat="server" class="warning message">
                    <%= WebTextManager.GetText("/siteText/timeZoneNotSetWarning") %><br />
                    <%= WebTextManager.GetText("/siteText/setItNow") %>
                    <ckbx:MultiLanguageDropDownList ID="_timeZone" runat="server">
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-12" Value="-12" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-11" Value="-11" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-10" Value="-10" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-9" Value="-9" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-8" Value="-8" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-7" Value="-7" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-6" Value="-6" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-5" Value="-5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-4.5" Value="-4.5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-4" Value="-4" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-3.5" Value="-3.5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-3" Value="-3" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-2" Value="-2" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-1" Value="-1" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+0" Value="0" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+1" Value="1" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+2" Value="2" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+3" Value="3" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+3.5" Value="3.5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+4" Value="4" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+4.5" Value="4.5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+5" Value="5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+5.5" Value="5.5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+5.75" Value="5.75" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+6" Value="6" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+6.5" Value="6.5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+7" Value="7" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+8" Value="8" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+9" Value="9" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+9.5" Value="9.5" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+10" Value="10" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+11" Value="11" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+12" Value="12" />
                        <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+13" Value="13" />
                    </ckbx:MultiLanguageDropDownList>
                    <btn:CheckboxButton runat="server" ID="_setBtn" TextId="/siteText/set" CssClass="ckbxButton orangeButton roundedCorners border999 shadow999" />
                    <br />
                    <%= WebTextManager.GetText("/siteText/youCanChangeItAnyTime") %>

                </asp:Panel>
                <p>
                    <%=CommonWelcomeMessage%>
                </p>
                <p>
                    <%=RoleSpecificWelcomeMessage%>
                </p>                
                <asp:PlaceHolder ID="_newsPlace" runat="server">
                    <div style="text-align:center; border: 1px dotted #C0C0C0; font-family: Arial, Helvetica, sans-serif; color: white; font-weight: normal; font-size: small;">
                        <asp:LinkButton ID="_showNews" runat="server" Text="Show News" CssClass="newsLink" />
                            
                        <asp:Panel ID="_pnlHideNews" runat="server">
                            <div style="color:black; text-align:left; background-color:#DEDEDE;margin-top:0; margin-bottom:3px;width:100%;">
                                <div class="left"><p class="newstitle">Customer News</p></div>
                                <div class="right">
                                    <asp:LinkButton ID="_hideNews" runat="server" Text="Hide News" CssClass="newsLink" style="color:#333333" />
                                </div>
                                <div class="clear"></div>
                            </div>
                        </asp:Panel>
                        <div class="padding10">
                            <asp:Literal ID="_newsLbl" runat="server" SkinID="Label" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <p>
                    Checkbox® v<%=ApplicationInstaller.ApplicationAssemblyVersion%> - Web Survey Software -- Copyright ©2012, Checkbox Survey Solutions, Inc.
                </p>
                <asp:Panel ID="_limitContainer" runat="server" Visible="false">
                    <div style="font-weight:bold;font-size:16px;color:#333333; margin-bottom:5px;"><%=Checkbox.Web.WebTextManager.GetText("/licenseLimit/licenseLimits")%></div>
                    <asp:PlaceHolder ID="_licenseLimitsPlaceholder" runat="server" />
                </asp:Panel>
            </div>
        </asp:Panel>
    </div>
</asp:Content>