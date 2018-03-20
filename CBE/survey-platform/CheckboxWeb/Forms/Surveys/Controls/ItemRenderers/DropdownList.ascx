<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DropdownList.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.DropdownList" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>

<asp:ObjectDataSource 
    ID="_optionsSource" 
    runat="server" 
    SelectMethod="GetOptions" 
    TypeName="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.DropdownList" 
    DataObjectTypeName="Checkbox.Wcf.Services.Proxies.SurveyResponseItemOption"  
    OnObjectCreating="OptionsSource_ObjectCreating" />

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                <asp:DropDownList 
                    ID="_dropdownList" 
                    runat="server" 
                    DataSourceID="_optionsSource" 
                    DataTextField="Text" 
                    DataValueField="OptionId"
                    CssClass="Answer"
                    ondatabound="DropDownList_DataBound"
                />
                <asp:PlaceHolder ID="_otherPlace" runat="server">
                    <asp:TextBox ID="_otherTxt" runat="server" CssClass="Answer otherText hidden"/>
                </asp:PlaceHolder>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
<script type="text/javascript">
    $(document).ready(function () {
        <% if (OtherOptionId != -1) { %>
        $(document).on('change', '#<%= _inputPanel.ClientID %> select', function () {
            //if 'other' option is checked we have to show the text input
            dropdownOnChange(this, '<%= OtherOptionId %>');
        });
        
        //if 'other' option is checked we have to show the text input
        dropdownOnChange($('#<%= _inputPanel.ClientID %> select'), '<%= OtherOptionId %>');
        <% } %>

        if ($('#<%= _topAndOrLeftPanel.ClientID %>').hasClass('labelRight')) {
            var question = $('#<%= _textContainer.ClientID %>').find('.Question.itemNumber');
            if (question.length > 0) {
                var margin = -$('#<%= _dropdownList.ClientID %>').width() - 5 + parseInt(question.css('margin-left').replace('px', ''));
                question.css('margin-left', margin + 'px');
            }
        }

        <%if (IsCombobox) {%>
        $("#<%=_dropdownList.ClientID%>").combobox();
        <%}%>
    });

    function dropdownOnChange(select, otherOptionId) {
        var textbox = $(select).parent().parent().find('input');
        if ($(select).val() == otherOptionId) {
            textbox.show();
        } else {
            textbox.hide();
        }
    }
</script>    
