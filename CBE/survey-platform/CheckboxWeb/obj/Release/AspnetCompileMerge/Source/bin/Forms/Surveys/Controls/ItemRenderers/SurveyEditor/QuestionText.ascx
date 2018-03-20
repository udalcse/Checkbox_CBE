<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="QuestionText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.QuestionText" %>
<%@ Register Src="~/Forms/Surveys/Controls/TermRenderer.ascx" TagPrefix="ckbx" TagName="TermRenderer" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web" %>

    <asp:Panel ID="_questionPanel" runat="server"  Visible='<%# Utilities.IsNotNullOrEmpty(Model.Text) ? true : false %>' CssClass="questionTextContainer Question" >
        <asp:Label ID="_textLbl"  runat="server"   />
    </asp:Panel>

    <asp:Panel ID="_descriptionPanel" runat="server"  Visible='<%# Utilities.IsNotNullOrEmpty(Model.Description) ? true : false %>' CssClass="descriptionTextContainer Description">
        <%= Model.Description %>
    </asp:Panel>
                
    <%-- Validation Message --%>
    <asp:Panel ID="_errorPanel" CssClass="Error" runat="server">
        <asp:Label ID="_errorMsgLbl" runat="server" CssClass="Error"/>
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
    /// Show error message
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        //Label associated control Id
        _textLbl.AssociatedControlID = AssociatedControlId;
        _textLbl.Text = GetQuestionText(Model);
        
        //Error message
        _errorPanel.Visible = !Model.IsValid;
        _errorMsgLbl.Text = GetErrorMessageText();
    }
    
    /// <summary>
    /// 
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
            WebTextManager.GetText("/common/surveyItemRequiredIndicator", Model.LanguageCode, string.Empty) : "";
        
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