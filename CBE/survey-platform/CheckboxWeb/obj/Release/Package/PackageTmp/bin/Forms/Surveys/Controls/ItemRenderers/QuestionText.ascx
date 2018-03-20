<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="QuestionText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.QuestionText" %>
<%@ Register Src="~/Forms/Surveys/Controls/TermRenderer.ascx" TagPrefix="ckbx" TagName="TermRenderer" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web" %>

    <asp:Panel ID="_itemNumberPanel" runat="server" CssClass="Question itemNumber"><asp:Literal ID="_itemNumberTxt" runat="server" />.</asp:Panel>

    <asp:Panel ID="_questionPanel" runat="server"  Visible='<%# Utilities.IsNotNullOrEmpty(Model.Text) %>' CssClass="questionTextContainer Question">
        <asp:Literal ID="_textLbl" runat="server" />
    </asp:Panel>
    <div style="clear:both;"></div>

    <asp:Panel ID="_descriptionPanel" runat="server"  Visible='<%# Utilities.IsNotNullOrEmpty(Model.Description) %>' CssClass="descriptionTextContainer Description">
        <%= Utilities.ReplaceHtmlAttributes(Model.Description, RenderMode == RenderMode.SurveyEditor) %>
    </asp:Panel>
    <div style="clear:both;"></div>

    <div style="clear:both;"></div>
                
    <%-- Validation Message --%>
    <asp:Panel ID="_errorPanel" CssClass="Error" runat="server">
        <asp:Label ID="_errorMsgLbl" runat="server" CssClass="Error" />
    </asp:Panel>

    <ckbx:TermRenderer runat="server" ID="_termRenderer" />

<script type="text/javascript">
    $(function () {
        //apply borders css if any
        var tablesWithBorders = $(".questionTextContainer table[border!='0']");
        $(tablesWithBorders).each(function (idx, val) {
            var borderWidth = $(this).attr("border");
            $(this).css({
                "border-width": borderWidth + "px",
            });
            $(this).find("td").css({
                "border-width": borderWidth + "px",
            });
        });
    });
</script>

<script type="text/C#" runat="server">

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        _textLbl.Text = GetQuestionText(Model);

        _itemNumberPanel.Visible = ShowItemNumber;
        _itemNumberTxt.Text = ItemNumber.HasValue ? ItemNumber.ToString() : string.Empty;

        if (ShowItemNumber)
            _questionPanel.CssClass += " numberedQuestion";
            
    }
    
    /// <summary>
    /// Show error message
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();
        
        //Error message
        _errorPanel.Visible = !Model.IsValid;
        _errorMsgLbl.Text = GetErrorMessageText();
        
        //Use client script manager to register invalid response popup so it will only be shown
        // once.
       // Session
        
        var updated = HttpContext.Current.Session["PageItemsUpdateCondition_" + Model.ParentTemplateId];
        if ((updated==null || (bool)updated) && !Model.IsValid && Page is CheckboxWeb.Survey && ((CheckboxWeb.Survey)Page).ShowValidationAlert)
        {
            Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "validationAlert",
                    "alert('" +
                    WebTextManager.GetText("/pageText/survey.aspx/pageValidationPopup", Model.LanguageCode, string.Empty) +
                    "');",
                    true
                );
        }
    }

    /// <summary>
    /// Get text of question and include item number and/or required field indicator.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected string GetQuestionText(SurveyResponseItem item)
    {
        var itemText = Utilities.CustomDecode(item.Text).Trim();

        //Handle adding "required" flag if necessary to item text.
        if (!item.AnswerRequired)
        {
            return string.IsNullOrEmpty(AssociatedControlId)
                       ? itemText
                       : string.Format("<label for=\"{0}\">{1}</label>", AssociatedControlId, itemText);
        }

        string requiredIndicator = item.AnswerRequired && ResponseTemplateManager.GetShowAsterisksSetting(item.ParentTemplateId) ?
            WebTextManager.GetText("/common/surveyItemRequiredIndicator", Model.LanguageCode, "*") : "";
        
        var modifiedString = string.Format("<span class=\"required\">{0}</span>", requiredIndicator);
        
        //Find spot to include indicator.  In many cases where WYSIWYG editor is used, item will start with HTML tag.  If this is case,
        // add text as first element inside of tag
        if (itemText.StartsWith("<"))
        {
            var startPosition = itemText.IndexOf(">") + 1;

            //Not sure if this is a valid case, but it means item text is one big HTML tag, so add required indicator to end.
            if (startPosition >= itemText.Length)
            {
                return string.IsNullOrEmpty(AssociatedControlId)
                  ? itemText + modifiedString
                  : string.Format("<label for=\"{0}\">{1}{2}</label>", AssociatedControlId, itemText, modifiedString);
            }
            
            //Start position == 0 means we found no close tag, so put on beginning since we don't know what else to do.  This fall-through case
            // is later in this method, so do nothing here.  Insert text inside first HTML element in item text
            if (startPosition > 0)
            {
                return string.IsNullOrEmpty(AssociatedControlId)
                         ? itemText.Insert(startPosition, modifiedString)
                         : string.Format("<label for=\"{0}\">{1}</label>", AssociatedControlId, itemText.Insert(startPosition, modifiedString));   
            }
        }

        
        if (itemText.StartsWith("<p>", StringComparison.InvariantCultureIgnoreCase) && itemText.Length > 3)
        {
            return string.IsNullOrEmpty(AssociatedControlId)
                       ? itemText.Insert(3, modifiedString)
                       : string.Format("<label for=\"{0}\">{1}</label>", AssociatedControlId, itemText.Insert(3, modifiedString));
        }

        //Fall through case.  Simply concatenate required indicator and item text
        return string.IsNullOrEmpty(AssociatedControlId)
                   ? modifiedString + itemText
                   : string.Format("<label for=\"{0}\">{1}{2}</label>", AssociatedControlId, modifiedString, itemText);
    }

</script>