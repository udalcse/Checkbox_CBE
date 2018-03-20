<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FacebookButton.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.FacebookButton" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Management" %>
<% if (!HideButton) { %>
<a href="#" class="fbSmall roundedCorners" onclick="showBlock('_facebookPanel', '_container');">
    Facebook
</a>
<div id="_facebookPanelParking" style="display:none">
<% } else { %>
<a href="#" class="fbSmall roundedCorners" onclick="showBlock('_facebookPanel', '_container');" style="display:none;">
    Facebook
</a>
<div id="_facebookPanelParking">
<% } %>
    <div id="_facebookPanel" pointerPos="68px">
        <div class="formInput">
        <div class="left fixed_100">
            <p>
                <ckbx:MultiLanguageLabel ID="_descriptionLbl" runat="server" AssociatedControlID="_description"
                    TextId="/facebookButton/description"></ckbx:MultiLanguageLabel></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_description" runat="server" CssClass="left"  Width="500px"/>
            <br />
        </div>
        </div>
        <div class="formInput">
        <div class="left fixed_100">
            <p>
                <ckbx:MultiLanguageLabel ID="_messageLbl" runat="server" AssociatedControlID="_message"
                    TextId="/facebookButton/message"></ckbx:MultiLanguageLabel></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_message" runat="server" CssClass="left"  Width="500px"/>
            <br />
        </div>
        </div>
        <div class="formInput">
        <div class="left fixed_100">
            <p>
                <ckbx:MultiLanguageLabel ID="_captionLbl" runat="server" AssociatedControlID="_caption"
                    TextId="/facebookButton/caption"></ckbx:MultiLanguageLabel></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_caption" runat="server" CssClass="left"  Width="500px"/>
            <br />
        </div>
        </div>
        <div class="formInput">
        <div class="left fixed_100">
            <p>
                <ckbx:MultiLanguageLabel ID="_userPromptLbl" runat="server" AssociatedControlID="_userPrompt"
                    TextId="/facebookButton/userPrompt"></ckbx:MultiLanguageLabel></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_userPrompt" runat="server" CssClass="left"  Width="500px"/>
            <br />
        </div>
        </div>
        <div class="formInput">
        <div class="left fixed_100">
            <p>
                <ckbx:MultiLanguageLabel ID="_imageLbl" runat="server" AssociatedControlID="_imageURL"
                    TextId="/facebookButton/image"></ckbx:MultiLanguageLabel></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_imageURL" runat="server" CssClass="left"  Width="500px"/>
            <br class="clear" />
            <div style="max-width: 500px; max-height: 150px; min-width: 1px; overflow:auto;">
                <img id="fb-image" src="<%=ApplicationManager.ApplicationURL + ResolveUrl("~/App_Themes/CheckboxTheme/Images/CheckboxLogo.png") %>" 
                    onclick="changeFacebookImage();" title="<%=WebTextManager.GetText("/facebookButton/imageTitleText") %>"
                    style="max-width:300px; max-height:400px;" /><br class="clear" />
            </div>
            <a href="javascript: changeFacebookImage();" class="viewSurvey"><%=WebTextManager.GetText("/facebookButton/imageTitleText") %></a>
        </div>
        </div>
        <br class="clear" />
                <%if (HideShareButton)
          { %>
          <a class="ckbxButton roundedCorners shadow999 border999 facebookButton statistics_FacebookShare" onclick="facebookShare();" style="visibility: hidden;" id="A1"><%= WebTextManager.GetText("/facebookButton/shareViaFacebook") %></a>
        <% } 
          else
          { %>
            <a class="ckbxButton roundedCorners shadow999 border999 facebookButton statistics_FacebookShare" onclick="facebookShare();" id="btn_<%= ClientID %>"><%= WebTextManager.GetText("/facebookButton/shareViaFacebook") %></a>
        <%  } %>
    </div>
</div>
<div id="fb-root" style="display:none"></div>

    <div id="changeImageModal" style="display:none;">
        <div>&nbsp;</div>
        <div class="confirmButtonsContainer">
            <div class="padding15">
                <div><%=WebTextManager.GetText("/facebookButton/changeImageDialogPrompt")%></div>
                <input id="imageURLText" type="text" style="width:600px !important;"/>
            </div>
            <div class="buttonWrapper" style="width:150px">
                <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:$.modal.close();OnChangeImageDialogClose('ok');"><%=WebTextManager.GetText("/common/ok") %></a>
                <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:$.modal.close();"><%=WebTextManager.GetText("/common/cancel") %></a>
            </div>            
        </div>
    </div> 

    <div id="linkPostedModal" style="display:none;">
        <div>&nbsp;</div>
        <div class="confirmButtonsContainer">
            <div class="padding15">
                <div><%=WebTextManager.GetText("/facebookButton/theSurveyLinkPosted")%></div>
            </div>
            <div class="buttonWrapper" style="width:70px">
                <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:$.modal.close();"><%=WebTextManager.GetText("/common/ok") %></a>
            </div>            
        </div>
    </div> 

<script>
    window.fbAsyncInit = function () {
        window.FB.init({ appId: '<%= FacebookAppID %>', status: true, cookie: true, channelUrl: '<%= ResolveUrl("~/Channel.html") %>', xfbml: true });
    };

    (function () {
        $('#<%=_imageURL.ClientID%>').val($('#fb-image').attr('src'));
        $('#<%=_imageURL.ClientID%>').focusout(function(){
            $('#fb-image').attr('src', $('#<%=_imageURL.ClientID%>').val());   
        });
        var e = document.createElement('script'); e.async = true;
        e.src = document.location.protocol + '//connect.facebook.net/en_US/all.js';
        document.getElementById('fb-root').appendChild(e);
    } ());

    function changeFacebookImage() {
        $('#imageURLText').val($('#fb-image').attr('src'));
        showDialog('changeImageModal', 150, 630);
    }

    function OnChangeImageDialogClose(arg) {
        $('#fb-image').attr('src', $('#imageURLText').val());        
    }

    function facebookShare(surveyLink) {
        var obj = {
            method: 'feed',
            link: surveyLink,
            picture: $('#fb-image').attr('src'),
            description: $('#<%=_description.ClientID%>').val(),
            name: $('#<%=_message.ClientID%>').val(),
            caption: $('#<%=_caption.ClientID%>').val(),
            user_message_prompt: $('#<%=_userPrompt.ClientID%>').val()
        };

        window.FB.ui(obj, function (response) {
            if (response && response.post_id) {
                showDialog('linkPostedModal', 120, 400);                
            }
        });
    }
</script>
