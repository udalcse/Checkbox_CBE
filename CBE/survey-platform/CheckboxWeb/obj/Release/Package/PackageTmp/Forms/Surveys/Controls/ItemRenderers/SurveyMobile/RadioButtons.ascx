<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RadioButtons.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.RadioButtons" %>
<%@ Import Namespace="Checkbox.Common" %>

<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent" Style="display: inline-block; *display: inline; zoom: 1; width: 100%;">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer radioButtonList mobileRadiobutton" style="width: 100%;">
                <table>
                <%
                    var options = GetAllOptions();
                    int optionsCount = options.Count();
                    
                    for(int i=0; i<optionsCount; i++) {
                        var option = options.ElementAt(i);
                        int optionId = option.OptionId; %>                       
                        <tr>
                            <% if (option.IsOther) { %>
                            <td id="other-option-radiobutton">
                            <% } else { %>
                            <td>
                            <% } %>
                                <div class="radio-item">         
                                    <%= GenerateRadioButtonMarkup(option) %>
                                    <label for="<%= optionId %>" style="width: 100%;">
                                        <div class="radioButtonLabel Answer"><%= GetOptionText(option) %></div>
                                    </label>
                                </div>
                            </td>
                        </tr>
                    <% }
                    if (OtherOption != null) { %>
                        <tr>
                            <td class="otherText">
                                <input type="text" name="<%= UniqueID + "_otherTxt" %>" id="otherTxt" value="<%= Model.InstanceData["OtherText"] %>"  />
                            </td>
                        </tr>
                    <% } %>
                </table>
                <div class="validationError" style="color: red; display:none">Answer is required.</div>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    $(function () {
        //show/hide other option on check
        var other_<%= ClientID %> = $('#<%= _inputPanel.ClientID%> .otherText');

        <% if (OtherOption != null && !OtherOption.IsSelected) { %>
        other_<%= ClientID %>.hide();
        <% } %>

        //show/hide other option on check       
        $(document).on('change', '#<%= _inputPanel.ClientID%> .radio-item', function () {
            if ($('#<%= _inputPanel.ClientID%> #other-option-radiobutton input').prop('checked')) {
                other_<%= ClientID %>.show();
            } else {
                other_<%= ClientID %>.hide();
            }
        });
    });
</script>    

