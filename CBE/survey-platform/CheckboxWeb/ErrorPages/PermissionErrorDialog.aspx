<%@ Page Language="C#" Theme="" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>Engauge&reg; Survey</title>
        <link id="ctl00_ctl00__favicon" rel="icon" href="../favicon.png" type="image/ico" />
        <ckbx:ResolvingCssElement runat="server" Source="../App_Themes/CheckboxTheme/Checkbox.css" />
    </head>
    <body>
        <div class="introPage padding15">
            <p>
                <span class="fixed_75 left">
                    <img src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/Stop.png") %>" alt="Error" />
                </span>
                <span class="left fixed_500 label">The logged-in user does not have permission to access this page or secured resource.</span>
                <br class="clear" />
            </p>
            <p>
                <span class="fixed_150 left label">Request URL:</span>
                <span style="float:left;font-family:Courier New;width:500px;"><%= Server.HtmlEncode(Request.Url.PathAndQuery) %></span>
                <br class="clear" />
            </p>
        </div>
    </body>
</html>