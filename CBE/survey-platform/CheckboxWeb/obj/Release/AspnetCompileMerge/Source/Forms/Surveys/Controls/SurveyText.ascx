<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SurveyText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.SurveyText" %>
<%@ Import Namespace="Checkbox.Forms"%>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="CheckboxWeb.Controls.Text" %>
<%@ Register Src="~/Controls/Text/MultiLanguageTextEditor.ascx" TagPrefix="ckbx" TagName="TextEditor" %>

<ckbx:TextEditor ID="_textEditor" runat="server" LabelContainerCssClass="field_150" InputContainerCssClass="input" />

<script type="text/C#" runat="server">
    /// Bind text editor to survey
    protected override void AddTextItems(ResponseTemplate rt, string currentLanguage, Dictionary<string, string> textOverrides)
    {
        var surveyTextKeyMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        surveyTextKeyMap["CONTINUE"] = "/pageText/responseTemplate.cs/nextDefaultText";
        surveyTextKeyMap["BACK"] = "/pageText/responseTemplate.cs/backDefaultText";
        surveyTextKeyMap["FINISH"] = "/pageText/responseTemplate.cs/finishDefaultText";
        
        //Set current and alt languages
        _textEditor.LanguageCode = currentLanguage;
        _textEditor.AlternateLanguages = new List<string>(rt.LanguageSettings.SupportedLanguages);
        
        //Simplify code below by ensuring textoverrides is non-null
        if (textOverrides == null)
        {
            textOverrides = new Dictionary<string, string>();
        }
        
        //Clear items
        _textEditor.TextItems.Clear();
        
        //Add items to edit
        //Survey title
        _textEditor.TextItems.Add(new TextItem
        {
             LabelTextId="/pageText/surveyStyle.aspx/surveyTitle",
             TextId=rt.LanguageSettings.TitleTextId,
             TextValue = textOverrides.ContainsKey(rt.LanguageSettings.TitleTextId) ? textOverrides[rt.LanguageSettings.TitleTextId] :
                string.IsNullOrEmpty(TextManager.GetText(rt.LanguageSettings.TitleTextId, currentLanguage)) ? rt.Name : null,
             InputWidth = 250
        });

        //Continue button
        _textEditor.TextItems.Add(new TextItem
        {
            LabelTextId = "/pageText/surveyStyle.aspx/continueButton",
            TextId = rt.LanguageSettings.ContinueButtonTextId,
            TextValue = textOverrides.ContainsKey(rt.LanguageSettings.ContinueButtonTextId) ? textOverrides[rt.LanguageSettings.ContinueButtonTextId] :
                string.IsNullOrEmpty(TextManager.GetText(rt.LanguageSettings.ContinueButtonTextId, currentLanguage)) ? TextManager.GetText(surveyTextKeyMap["CONTINUE"], currentLanguage) : null,
        });

        //Finish button
        _textEditor.TextItems.Add(new TextItem
        {
            LabelTextId = "/pageText/surveyStyle.aspx/finishButton",
            TextId = rt.LanguageSettings.FinishButtonTextId,
            TextValue = textOverrides.ContainsKey(rt.LanguageSettings.FinishButtonTextId) ? textOverrides[rt.LanguageSettings.FinishButtonTextId] :
                string.IsNullOrEmpty(TextManager.GetText(rt.LanguageSettings.FinishButtonTextId, currentLanguage)) ? TextManager.GetText(surveyTextKeyMap["FINISH"], currentLanguage) : null,
        });

        //Back button
        _textEditor.TextItems.Add(new TextItem
        {
            LabelTextId = "/pageText/surveyStyle.aspx/backButton",
            TextId = rt.LanguageSettings.BackButtonTextId,
            TextValue = textOverrides.ContainsKey(rt.LanguageSettings.BackButtonTextId) ? textOverrides[rt.LanguageSettings.BackButtonTextId] :
                string.IsNullOrEmpty(TextManager.GetText(rt.LanguageSettings.BackButtonTextId, currentLanguage)) ? TextManager.GetText(surveyTextKeyMap["BACK"], currentLanguage) : null,
        });

    }
</script>


