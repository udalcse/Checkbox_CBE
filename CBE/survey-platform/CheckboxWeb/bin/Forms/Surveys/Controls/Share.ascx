<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Share.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.Share" %>
<%@ Import Namespace="Checkbox.Web" %>
<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.ckbxtab.js" />



<script type="text/javascript">
    $(document).ready(function() {
        if ("<%= CustomEnabled %>" == "False") {
            $('#sharingTabs').ckbxTabs({
                tabName: 'sharingTabs',
                initialTabIndex: "1",
                onTabClick: function(index) { $('#<%= _currentTabIndex.ClientID %>').val(index); },
                onTabsLoaded: function() {
                    $('.tabContainer').hide();
                    $('.tabContentContainer').show();


                }
            });
        } else {
            $('#sharingTabs').ckbxTabs({
                tabName: 'sharingTabs',
                initialTabIndex: "0",
                onTabClick: function(index) { $('#<%= _currentTabIndex.ClientID %>').val(index); },
                onTabsLoaded: function() {
                    $('.tabContainer').show();
                    $('.tabContentContainer').show();


                }
            });
        }

    });


</script>



<div class="clear"></div>

<div style="display: none">
    <asp:TextBox ID="_currentTabIndex" runat="server" ></asp:TextBox>
</div>


<div class="clear"></div>
<ul id="sharingTabs" class="tabContainer ">
    <li>Custom Url</li>
    <li>Default Url</li>
</ul>

<div class="clear"></div>



<div class="tabContentContainer padding10">
    <div class="wizardContainer" id="sharingTabs-0-tabContent">
        <div>
            <%= WebTextManager.GetText("/pageText/forms/surveys/launch.aspx/LinkorEmbed") %>
        </div>
        <div class="dialogSubContainer" runat="server">
            <div class="formInput">
                <div class="left fixed_100" style="padding-right: 20px">
                    <p>
                        <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveys/launch.aspx/linkTo"/>
                    </p>
                </div>
                <div class="left" >
                    <asp:TextBox runat="server" ID="_surveyCustomURL" CssClass="left" Width="600px" ReadOnly="true" style="left: -22px; position: relative;"/>
                    <a href="<%= SurveyCustomURL %>" target="_blank" class="newWindow"></a>
                   
                </div>
                <div class="clear"></div>
             
                <div class="left fixed_100" style="padding-bottom: 30px; padding-top: 10px;">
                    <p>
                        <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/survets/launch.aspx/embed"/>
                    </p>
                </div>
                <div class="left" style="padding-top: 10px">
                    <asp:TextBox ID="_customhtmlCode" runat="server" TextMode="MultiLine" ReadOnly="true" CssClass="roundedCorners embedCode" Width="600px" Height="100px"/>
                    
                </div>
                <div class="clear"></div>
                <div class="center" >
                    <p>
                        <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveys/launch.aspx/shareVia"/>
                    </p>
                </div>
           
                <div class="left buttonWrapper">
                    <a class="ckbxButton twitterSmall" href="<%= CustomTwitterUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><%= WebTextManager.GetText("/pageMenu/survey_editor/Twitter") %></a>
                    <a class="ckbxButton facebookButton" style="padding-bottom: 3px; padding-top: 3px;" href="<%= CustomFacebookUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><%= WebTextManager.GetText("/pageMenu/survey_editor/Facebook") %></a>
                    <a class="ckbxButton blueButton" href="<%= CustomLinkedInUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><% = WebTextManager.GetText("/pageMenu/survey_editor/LinkedIn") %></a>
                    <a class="ckbxButton redButton" href="<%= CustomGplusUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><% = WebTextManager.GetText("/pageMenu/survey_editor/Google+") %></a>
                    <a href="javascript: eval($('#<%= SendCustomInvitationButton.ClientID %>').attr('href'));" class="ckbxButton orangeButton"><img src="<%= ResolveUrl("~/App_Themes/CheckboxTheme/Images/share-icon-invite.png") %>" class="btnImage" alt=""/><% = WebTextManager.GetText("/pageMenu/survey_editor/EmailInvitations") %></a>

                    <asp:LinkButton ID="SendCustomInvitationButton" runat="server" OnClick="SendCustomInvitationButton_OnClick" style="display: none">Email Invite</asp:LinkButton>                

                    
                </div>
                
            </div>
        </div>
        
        
    </div>
   
    
    <div class="wizardContainer" id="sharingTabs-1-tabContent">
        <div>
            <%= WebTextManager.GetText("/pageText/forms/surveys/launch.aspx/LinkorEmbed") %>
        </div>
        <div class="dialogSubContainer" runat="server">
            <div class="formInput">
                <div class="left fixed_100" style="padding-right: 20px">
                    <p>
                        <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveys/launch.aspx/linkTo"/>
                    </p>
                </div>
                <div class="left" >
                    <asp:TextBox runat="server" ID="_surveyUrl" CssClass="left" Width="600px" ReadOnly="true" style="left: -22px; position: relative;" />
                    <a href="<%= SurveyURL %>" target="_blank" class="newWindow"></a>
                   
                </div>
                <div class="clear"></div>
             
                <div class="left fixed_100" style="padding-bottom: 30px; padding-top: 10px;">
                    <p>
                        <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/survets/launch.aspx/embed"/>
                    </p>
                </div>
                <div class="left" style="padding-top: 10px">
                    <asp:TextBox ID="_defaultHTML" runat="server" TextMode="MultiLine" ReadOnly="true" CssClass="roundedCorners embedCode" Width="600px" Height="100px"/>
                    
                </div>
                <div class="clear"></div>
                <div class="center" >
                    <p>
                        <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveys/launch.aspx/shareVia"/>
                    </p>
                </div>
                <div class="clear"></div>
                <div class="left buttonWrapper">
                    <a class="ckbxButton twitterSmall" href="<%= TwitterUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><%= WebTextManager.GetText("/pageMenu/survey_editor/Twitter") %></a>
                    <a class="ckbxButton facebookButton"style="padding-bottom: 3px; padding-top: 3px;" href="<%= FacebookUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><%= WebTextManager.GetText("/pageMenu/survey_editor/Facebook") %></a>
                    <a class="ckbxButton blueButton" href="<%= LinkedInUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><%= WebTextManager.GetText("/pageMenu/survey_editor/LinkedIn") %></a>
                    <a class="ckbxButton redButton" href="<%= GplusUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><%= WebTextManager.GetText("/pageMenu/survey_editor/Google+") %></a>

                    <a href="javascript: eval($('#<%= SendInvitationButton.ClientID %>').attr('href'));" class="ckbxButton orangeButton"><img src="<%= ResolveUrl("~/App_Themes/CheckboxTheme/Images/share-icon-invite.png") %>" class="btnImage" alt=""/><% = WebTextManager.GetText("/pageMenu/survey_editor/EmailInvitations") %></a>
                    
                   

                    <asp:LinkButton ID="SendInvitationButton" runat="server" OnClick="SendInvitationButton_OnClick" style="display: none">Email Invite</asp:LinkButton>                


                </div>
                
            </div>
        </div>
    </div>
</div>
</>