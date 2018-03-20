<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RadioButtons.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RadioButtons" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Common" %>
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
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent" Style="">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>

        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer radioButtonList">
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
                                <div class="radio-item">
                                    <%= GenerateRadioButtonMarkup(option) %>
                                </div>
                            </td>
                            <td>
                                <div class="radio-item">
                                    <label for="<%= optionId %>"> 
                                        <div class="radioButtonLabel Answer"><%= Utilities.AdvancedHtmlDecode(GetOptionText(option)) %></div>
                                    </label>
                                    <% if (option.IsOther && AllowOther) { %>
                                        <% if (option.IsSelected) { %>
                                        <input type="text" name="<%= UniqueID + "_otherTxt" %>" id="otherTxt" value="<%= Model.InstanceData["OtherText"] %>" class="otherText"/>
                                        <% } else { %>
                                        <input type="text" name="<%= UniqueID + "_otherTxt" %>" id="otherTxt" value="<%= Model.InstanceData["OtherText"] %>" style="display: none;" class="otherText"/>
                                        <% } %>    
                                    <% } %>                                
                                 </div>
                            </td>
                        <% } %>
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
        $.each($('#<%= _inputPanel.ClientID%> .radio-item'), function (ind, elem) {
            //trick for correct html formatting inside option label
            var label = $(elem).find('label');
            var content = $(elem).find('.radioButtonLabel');
            content.detach();
            label.append(content);
        });
        
        //show/hide other option on check
        $('#<%= _inputPanel.ClientID%>').on('change', '.radio', function () {           
            var other = $(this).closest('td').next().find('#otherTxt');

            if (other.length) {
                other.show();
            } else {
                $('#<%= _inputPanel.ClientID%> #otherTxt').hide();
            }
        });
    });
</script>    

