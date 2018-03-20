<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SingleLineText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.SingleLineText" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer singleLineInput mobile-autocomplete">
                <input ID="_textInput" runat="server" />
                <ul data-role="listview" data-filter="true" data-filter-reveal="true" data-filter-placeholder="">
                <% if(AutocompleteData != null) { %>
                    <% foreach (var item in AutocompleteData) { %>
                        <li><a href="#"><%= Utilities.AdvancedHtmlEncode(item) %></a></li>   
                    <% } %>
                <% } %>
                </ul>
                <div class="validationError" style="color: red; display:none">Answer is required.</div>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    $(document).ready(function () {
        <% if (AutocompleteData != null) { %>
            var panel_<%=ClientID%> = $('#<%= _inputPanel.ClientID %>');

            $('#<%= _textInput.ClientID %>').on('keyup', function () {
                var text = $(this).val();
                if (text === '') {
                    panel_<%=ClientID%>.find("[data-role=listview]").children().addClass('ui-screen-hidden');
                }
                panel_<%=ClientID%>.find("input[data-type='search']").val(text);
                panel_<%=ClientID%>.find("input[data-type='search']").trigger('change');
            });

            panel_<%=ClientID%>.on('click', "li", function () {
                var text = $(this).find('.ui-link-inherit').text();
                $('#<%= _textInput.ClientID %>').val(text);
                panel_<%=ClientID%>.find("input[data-type='search']").val(text);
                panel_<%=ClientID%>.find("[data-role=listview]").children().addClass('ui-screen-hidden');
            });
        <% } else if (!string.IsNullOrWhiteSpace(AutocompleteRemote)) { %>
            var panel_<%=ClientID%> = $('#<%= _inputPanel.ClientID %>');

            $('#<%= _textInput.ClientID %>').on('keyup', function () {
                var text = $(this).val();
                if (text === '') {
                    panel_<%=ClientID%>.find("[data-role=listview]").children().addClass('ui-screen-hidden');
                }

                $.ajax({
                    url: "<%=AutocompleteRemote%>",
                    dataType: "jsonp",
                    crossDomain: true,
                    data: {
                        q: text
                    }
                })
                .then(function (response) {
                    var html = '';
                    $.each(response, function (i, val) {
                        if (val) {
                            html += '<li><a class="ui-link-inherit" href="#">' + val + '</a></li>';
                        }
                    });
                    panel_<%=ClientID%>.find("[data-role=listview]").html(html);
                    panel_<%=ClientID%>.find("[data-role=listview]").listview('refresh');
                });
            });

            panel_<%=ClientID%>.on('click', "li", function () {
                var text = $(this).find('.ui-link-inherit').text();
                $('#<%= _textInput.ClientID %>').val(text);
                panel_<%=ClientID%>.find("[data-role=listview]").children().addClass('ui-screen-hidden');
            });
        <% } %>
    });
</script>
