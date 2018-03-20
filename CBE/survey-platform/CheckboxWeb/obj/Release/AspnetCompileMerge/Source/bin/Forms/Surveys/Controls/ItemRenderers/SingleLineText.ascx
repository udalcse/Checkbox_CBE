<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SingleLineText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SingleLineText" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <script type="text/javascript">
        var initAutocomplete_<%=ClientID%> = function(input) {
            <% if(!string.IsNullOrWhiteSpace(AutocompleteData)) {%>
                    input.autocomplete({
                        source: <%=AutocompleteData%>
                    });
             <% } else if(!string.IsNullOrEmpty(AutocompleteRemote)) {%>
                    input.autocomplete({
                        source: function( request, response ) {
                            $.ajax({
                                url: "<%=AutocompleteRemote%>",
                                dataType: "jsonp",
                                data: {
                                    q: request.term
                                },
                                success: function(data) {
                                    for (var i=0;i<data.length;i++) {
                                        if (!data[i]) {
                                            data.splice(i,1);
                                            i--;
                                        }
                                    }
                                    response(data);
                                },
                                error: function (xhr) {
                                    alert('Request Status: ' + xhr.status + '\nStatus Text: ' + xhr.statusText + '\n' + xhr.responseText);
                                }
                            });
                      },
                      minLength: 2
                    });
            <% } %>
        }

        $(function () {
            $( window ).resize(function() {
                $(".ui-autocomplete").hide();
            });
            if ($('#<%= _topAndOrLeftPanel.ClientID %>').hasClass('labelRight')) {
                var question = $('#<%= _textContainer.ClientID %>').find('.Question.itemNumber');
                if(question.length > 0) {
                    var margin = -$('#<%= _inputPanel.ClientID %>').width() - 5 + parseInt(question.css('margin-left').replace('px', ''));
                    question.css('margin-left', margin+'px');
                }
            }
<%if (_dateInput.Visible) { %>
            $('.datepicker').datepicker({
                showOn: 'both',
                buttonImageOnly: true,
                buttonImage: '<%=ResolveUrl("~/Resources/CalendarPopup.png")%>',
                buttonText: 'Calendar',
                dateFormat: '<%=DateFormat%>',
                changeMonth: true,
                changeYear: true,
                yearRange: "-125:+50"
        });
            <% } %>

            <%if(_textInput.Visible) {%>
                initAutocomplete_<%=ClientID%>($("#<%=_textInput.ClientID%>"));
            <%}%>
            <%if(_numericInput.Visible) {%>
                initAutocomplete_<%=ClientID%>($("#<%=_numericInput.ClientID%>"));
            <%}%>
            <%if(_maskedInput.Visible) {%>
                initAutocomplete_<%=ClientID%>($("#<%=_maskedInput.ClientID%>"));
            <%}%>
        });
    </script>

    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                <asp:TextBox
                    ID="_textInput" 
                    runat="server" 
                    TextMode="SingleLine"
                />
                    
                <asp:TextBox
                    ID="_numericInput" 
                    runat="server"
                    TextMode="SingleLine"
                    Visible="false" />
                    
                <asp:TextBox
                    ID="_dateInput"
                    runat="server"
                    TextMode="SingleLine"
                    Visible="false"
                    CssClass="datepicker" />

                <div class="validationError" style="color: red; display:none">Answer is required.</div>
                <asp:TextBox ID="_maskedInput" runat="server" Visible="false" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

