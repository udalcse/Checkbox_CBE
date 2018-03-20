<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Checkboxes.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.Checkboxes" %>
<%@ Import Namespace="Checkbox.Common" %>

<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">   
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent"  Style="*display: inline; zoom: 1;">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer checkBoxList" style="width: 100%;">
                <table>
                <%
                    var options = GetAllOptions();
                    int optionsCount = options.Count();
                    
                    for(int i=0; i<optionsCount; i++) {
                        var option = options.ElementAt(i);
                        int optionId = option.OptionId; %>                       
                        <tr>
                            <% if (option.IsOther) { %>
                            <td id="other-option-checkbox">
                            <% } else { %>
                            <td>
                            <% } %>
                                <div class="checkbox-item">         
                                    <%= GenerateCheckboxMarkup(option) %>
                                    <label for="<%= optionId %>" style="width: 100%;">
                                        <div class="checkboxToggleLabel Answer"><%= GetOptionText(option) %></div>
                                    </label>
                                </div>
                            </td>
                        </tr>
                    <% }
                    if (OtherOption != null) { %>
                        <tr>
                            <td class="otherText">
                                <input type="text" name="<%= UniqueID + "otherTxt" %>" id="otherTxt" value="<%= Model.InstanceData["OtherText"] %>"  />
                            </td>
                        </tr>
                    <% } %>
                </table>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    $(function () {
        var other_<%= ClientID %> = $('#<%= _inputPanel.ClientID%> .otherText');
        
        <% if (OtherOption != null && !OtherOption.IsSelected) { %>
        other_<%= ClientID %>.hide();
        <% } %>

        //show/hide other option on check       
        $(document).on('change', '#<%= _inputPanel.ClientID%> #other-option-checkbox', function () {
            if ($(this).find('input:checked').length) 
                other_<%= ClientID %>.show();
            else 
                other_<%= ClientID %>.hide();
        });

        //provide 'none of above' functionality
        $('#<%= _inputPanel.ClientID%> .checkbox-item input').on('click', function () {
            if ($(this).prop('checked')) {
                if ($(this).attr('noneofabove') == 'true')
                    $('#<%= _inputPanel.ClientID%> .checkbox-item input').not('[noneofabove="true"]').prop("checked", false).checkboxradio('refresh');
                else
                    $('#<%= _inputPanel.ClientID%> [noneofabove="true"]').prop("checked", false).checkboxradio('refresh');
            }
        });
    });
</script>    

