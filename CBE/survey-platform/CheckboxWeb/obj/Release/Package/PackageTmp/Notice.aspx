<%@ Page Language="C#" AutoEventWireup="False" CodeBehind="Notice.aspx.cs" Inherits="CheckboxWeb.Notice" %>

<!DOCTYPE html>
<html>
    <head runat="server">
        <meta charset="utf-8" />
        <title id="_title" runat="server">Maintenance Notice | Checkbox&reg; Survey</title>
        <link id="_favicon" runat="server" rel="icon" href="~/favicon.png" type="image/ico" />
        
        <ckbx:ResolvingCssElement runat="server" Source="App_Themes/CheckboxTheme/Checkbox.css" />
    </head>
    <body>
<%
    var redirectUrl = !string.IsNullOrWhiteSpace(Request.QueryString["returnurl"]) ? Request.QueryString["returnurl"] : UserDefaultRedirectUrl;
%>
        <ckbx:ResolvingScriptElement runat="server" Source="Resources/localStorage.min.js" />

        <script>
            (function () {
                var script = document.createElement('script');
                script.src = 'https://www.checkbox.com/api/core/get_category_posts/?callback=parsePost&slug=maintenance-notices&count=1';
                document.body.appendChild(script);
            })();
            
            function parsePost(data) {
                if (data.posts) {
                    var post = data.posts[0];

                    var exists = window.localStorage.getItem('lastMaintenancePost');
                    if (!exists || post.id > parseInt(exists)) {
                        document.getElementById('notice_title').innerHTML = post.title;
                        document.getElementById('notice_message').innerHTML = post.content;

                        document.getElementById('roadblockNoticeWrapper').setAttribute('style', '');
                        window.localStorage.setItem('lastMaintenancePost', post.id);
                    } else {
                        window.location = '<%= redirectUrl %>';
                    }
                }
            }
        </script>

        <div class="roadblock-notice-wrapper" id="roadblockNoticeWrapper" style="display:none;">
            <img class="logo" alt="Checkbox Survey Logo" src="/App_Themes/CheckboxTheme/Images/CheckboxLogo.png" style="height:35px;" />
            <div class="roadblock-notice shadow-white-inner">
                <h2 id="notice_title"></h2>
                <div id="notice_message"></div>
                <div class="notice-footer">
                    <a href="<%= redirectUrl %>">Continue &raquo;</a>
                </div>
            </div>
        </div>
    </body>
</html>