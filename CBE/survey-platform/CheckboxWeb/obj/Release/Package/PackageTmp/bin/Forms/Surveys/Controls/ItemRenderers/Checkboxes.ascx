<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Checkboxes.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Checkboxes" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>

<%-- DataSource selects NON other options.  Other option is handled separately.  --%>

    <script type="text/javascript">
        $(function () {
            if ($('#<%= _topAndOrLeftPanel.ClientID %>').hasClass('labelRight')) {

                var question = $('#<%= _textContainer.ClientID %>').find('.Question.itemNumber');

                if (question.length > 0) {
                    var margin = -$('#<%= _inputPanel.ClientID %>').width() - 5 + parseInt(question.css('margin-left').replace('px', ''));
                    question.css('margin-left', margin + 'px');
                }
            } 
        });        
    </script>
    

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">   
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>

        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer checkBoxList">
                <table>
                <%
                    var options = GetAllOptions();
                    int cols = NumberOfColumns;
                    int rows;
                    int optionsCount = options.Count();

                    if (cols > 0)
                    {
                        rows = optionsCount/cols;
                        if (optionsCount%cols > 0)
                            rows++;
                    }
                    else
                    {
                        if (HorizontalOriented)
                        {
                            rows = 1;
                            cols = optionsCount;
                        }
                        else
                        {
                            cols = 1;
                            rows = optionsCount;
                        }
                    }

                    for(int r=0; r<rows; r++) { %>
                        <tr>
                        <% for(int c=0; c<cols; c++) {
                            int elemNum;
                            if (HorizontalOriented)
                                elemNum =  r * cols + c;
                            else
                                elemNum = c * rows + r;
                                   
                            if (elemNum >= optionsCount)
                                break;
                                   
                            var option = options.ElementAt(elemNum);
                            string optionId = option.OptionId.ToString(); %>
                            <td>
                                <div class="checkbox-item checkbox-input">         
                                    <%= GenerateCheckboxMarkup(option) %>
                                </div>
                            </td>
                            <td>
                                <div class="checkbox-item">         
                                    <label for="<%= optionId %>">
                                        <div class="checkboxToggleLabel Answer"><%= GetOptionText(option)  %></div>
                                    </label>
                                    <% if (option.IsOther && AllowOther) { %>
                                        <% if (option.IsSelected) { %>
                                        <input type="text" name="<%= UniqueID + "otherTxt" %>" id="otherTxt" value="<%= Model.InstanceData["OtherText"] %>" class="otherText" />
                                        <% } else { %>
                                        <input type="text" name="<%= UniqueID + "otherTxt" %>" id="otherTxt" value="<%= Model.InstanceData["OtherText"] %>" style="display: none;" class="otherText"/>
                                        <% } %>    
                                    <% } %>
                                </div>
                            </td>
                        <% } %>
                        </tr>
                    <% } %>
                
                </table>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    $(function () {
        $.each($('#<%= _inputPanel.ClientID%> .checkbox-item.checkbox-input'), function (ind, elem) {
            //trick for correct html formatting inside option label
            var label = $(elem).find('label');
            var content = $(elem).find('.checkboxToggleLabel');
            content.detach();
            label.append(content);
        });

        //show/hide other option on check
        $('#<%= _inputPanel.ClientID%> [otheroption="true"]').on('click', function () {
            var other = $('#<%= _inputPanel.ClientID%> #otherTxt');
            if ($(this).prop('checked'))
                other.show();
            else
                other.hide();
        });

        //provide 'none of above' functionality
        $('#<%= _inputPanel.ClientID%> .checkbox-item.checkbox-input input').on('click', function () {
            if ($(this).prop('checked')) {
                if ($(this).attr('noneofabove') == 'true') {
                    $('#<%= _inputPanel.ClientID%> .checkbox-item.checkbox-input input').not('[noneofabove="true"]').prop("checked", false);
                    $('#<%= _inputPanel.ClientID%> #otherTxt').hide();
                }
                else 
                    $('#<%= _inputPanel.ClientID%> [noneofabove="true"]').prop("checked", false);
                
                $.uniform.update($('#<%= _inputPanel.ClientID%> .checkbox-item.checkbox-input input'));
            }
        });
    });
</script>    
