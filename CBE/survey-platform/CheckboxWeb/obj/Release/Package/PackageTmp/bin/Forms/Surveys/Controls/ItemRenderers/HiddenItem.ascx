<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HiddenItem.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.HiddenItem" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Items" %>

<script type="text/C#" runat="server">
    
    /// <summary>
    /// Capture requested value.
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        if (Utilities.IsNullOrEmpty(Model.Metadata["VariableSource"])
            || Utilities.IsNullOrEmpty(Model.Metadata["VariableName"]))
        {
            return;
        }

        try
        {
            var variableSource = (HiddenVariableSource)Enum.Parse(typeof(HiddenVariableSource), Model.Metadata["VariableSource"]);
            string variableName = Model.Metadata["VariableName"];

            switch (variableSource)
            {
                case HiddenVariableSource.QueryString:
                    UpsertTextAnswer(HttpContext.Current.Request.QueryString[variableName]);
                    break;

                case HiddenVariableSource.Session:
                    if (HttpContext.Current.Session[variableName] != null)
                    {
                        UpsertTextAnswer(HttpContext.Current.Session[variableName].ToString());
                    }
                    break;

                case HiddenVariableSource.Cookie:
                    if (HttpContext.Current.Request.Cookies[variableName] != null)
                    {
                        HttpCookie cookie = HttpContext.Current.Request.Cookies[variableName];
                        UpsertTextAnswer(cookie.Value);
                    }
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
        }
    }
</script>
