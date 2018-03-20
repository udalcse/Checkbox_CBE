<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DropdownList.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.DropdownList" %>
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
                    <div class="otherText" style="display: none;">
                        <asp:TextBox ID="_otherTxt" runat="server" CssClass="Answer"/>
                    </div>
                </asp:PlaceHolder>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
<script type="text/javascript">
    $(document).delegate('.ui-page', 'pageshow', function () {
        <%= ClientID %>_switchOtherTxt();
        $(document).on('change', '#<%=_dropdownList.ClientID%>', <%= ClientID %>_switchOtherTxt);
    });

    function <%= ClientID %>_switchOtherTxt() {
        //if 'other' option is checked we have to show the text input
        if ($('#<%=_dropdownList.ClientID%>').val() == '<%= OtherOptionId %>') {
            $('#<%=_containerPanel.ClientID%> .otherText').show();
        }
        else {
            $('#<%=_containerPanel.ClientID%> .otherText').hide();
        }
    }
</script>    
