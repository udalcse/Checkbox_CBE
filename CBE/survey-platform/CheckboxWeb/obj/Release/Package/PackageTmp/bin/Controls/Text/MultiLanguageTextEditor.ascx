<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MultiLanguageTextEditor.ascx.cs" Inherits="CheckboxWeb.Controls.Text.MultiLanguageTextEditor" %>
<%@ Import Namespace="CheckboxWeb.Controls.Text"%>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Globalization.Text"%>

<asp:Repeater ID="_editorRepeater" runat="server">
    <ItemTemplate>
        <div class="formInput">
            <p><ckbx:MultiLanguageLabel ID="_fieldLbl" runat="server" AssociatedControlID="_inputTxt" TextId='<%# DataBinder.Eval(Container.DataItem, "LabelTextId") %>' /></p>
            <asp:TextBox 
                ID="_inputTxt" runat="server" 
                TextId='<%# DataBinder.Eval(Container.DataItem, "TextId") %>' 
                Text='<%# GetText(Container.DataItem)  %>'  />
        </div>
        
        <div class="clear"></div>
    </ItemTemplate>
</asp:Repeater>


<script type="text/C#" runat="server">
    
    /// Get text for a given text id
    protected string GetText(object dataItem)
    {
        //Ensure passed in item is proper type
        if(dataItem == null
            || !(dataItem is TextItem))
        {
            return string.Empty;
        }

        TextItem textItem = dataItem as TextItem;

        //If a non-null value is passed as text value, use that value
        // instead of retrieving value based on text id from text manager.
        if (textItem.TextValue != null)
        {
            return textItem.TextValue;
        }
        
        //If no text id, don't bother trying to get text
        if (Utilities.IsNullOrEmpty(textItem.TextId))
        {
            return string.Empty;
        }

        //Try to get text in specified langauge
        string textValue = TextManager.GetText(textItem.TextId, LanguageCode);

        if (Utilities.IsNullOrEmpty(textValue))
        {
            //Otherwise, get alt text in an alternate language and prefix with language code to indicate this is likely an
            // untranslated value.
            string altTextValue = TextManager.GetText(textItem.TextId, LanguageCode, string.Empty, AlternateLanguages.ToArray());

            if (Utilities.IsNotNullOrEmpty(altTextValue))
            {
                return string.Format("[{0}] {1}", LanguageCode, altTextValue);
            }
        }        
        
        //No text found for any language, simply return empty string
        return textValue ?? string.Empty;
    }
</script>