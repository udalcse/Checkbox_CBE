<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<script type="text/javascript">
        $(function () {
            if ($('#<%= _topAndOrLeftPanel.ClientID %>').hasClass('labelRight')) {

                var question = $('#<%= _textContainer.ClientID %>').find('.Question.itemNumber');

                if (question.length > 0) {
                    var margin = -$('#<%= _inputPanel.ClientID %>').width() - 5 + parseInt(question.css('margin-left').replace('px', ''));
                    question.css('margin-left', margin + 'px');
                }
            }
            

            //init binded matrix plugin 
            var bindedMatrixModel = JSON.parse('<% = new JavaScriptSerializer().Serialize(BindedMatrixInfo) %>');
            var exportMode = '<%= ExportMode%>';
            var renderMode = '<%= RenderMode%>';
            var isDynamic = null;
            var isMobile = '<%= IsMobileSurvey%>';
            if (bindedMatrixModel)
             isDynamic = !bindedMatrixModel.IsColumnsStatic || !bindedMatrixModel.IsRowsStatic;

            if (isDynamic && exportMode === "None") {
                $('#<%=ClientID + "_matrix" %>').bindedMatrix({
                    columnFixed: bindedMatrixModel.IsColumnsStatic,
                    rowsFixed: bindedMatrixModel.IsRowsStatic,
                    columnsCount: bindedMatrixModel.ColumnCount,
                    rowsCount: bindedMatrixModel.RowsCount,
                    baseColumnsCount: bindedMatrixModel.BaseColumnCount,
                    baseRowsCount: bindedMatrixModel.BaseRowCount,
                    hasRowHeaders: bindedMatrixModel.HasRowHeaders,
                    hasColumnHeaders: bindedMatrixModel.HasColumnHeaders,
                    isPreview: renderMode === "SurveyPreview" || renderMode === "SurveyMobilePreview",
                    gridLine: bindedMatrixModel.GridLines,
                    isMobile: isMobile
                });
            }

            var matrixTable = $('#<%=ClientID + "_matrix" %>');

            var lastHeadCell = $('#<%=ClientID + "_matrix" %> .header td').last();
            


<%--            var gridLines = '<%=GridLineClassForHeader%>'.length > 0;

            if (gridLines) {
                if ($(lastHeadCell).text() === "Actions")
                    $(lastHeadCell).css("border-bottom", "1px solid grey");

                var noText = true;
                $.each($(matrixTable).find('tr'),
                    function(index, item) {
                        var text = $(item).find('td').first().text().replace(/\s+/g, '');
                        if (text.length > 0)
                            noText = false;
                    });
                if (noText) {
                    $.each($(matrixTable).find('tr'),
                        function (index, item) {
                            $(item).find('td').first().removeClass('BorderBoth').addClass('BorderTop').css("border-right", "0px solid black");
                        });
                }
            }--%>

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
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                <table id='<%=ClientID + "_matrix" %>' class="Matrix" style="width: <%= TableWidth %>;>
                    <%-- Column Text --%>
                    <asp:Label ID="_MainHeadercontrolsPlace" runat="server"  ForeColor="Black" Font-Bold="true" Font-Size="Large" />
                    <tr class="header">
                        <asp:Repeater ID="_columnHeaderRepeater" runat="server" DataSource="<%# MatrixItem.Columns %>"
                            OnItemDataBound="ColumnHeaderRepeater_ItemDataBound">
                            <ItemTemplate>
                                <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                <asp:PlaceHolder ID="_controlsPlace" runat="server"  />
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                    <%-- Scale Texts --%>
                    <asp:PlaceHolder ID="_scalePlaceholder" runat="server" Visible="<%# ContainsRatingScaleColumn %>">
                        <tr valign="bottom" class="header Answer scaleText">
                            <asp:Repeater ID="_columnScaleTextsRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>'
                                OnItemCreated="OnScaleTextItemCreated">
                                <ItemTemplate>
                                    <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                    <asp:PlaceHolder ID="_scaleTextsPlace" runat="server"  />
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </asp:PlaceHolder>
                    <%-- Column Options --%>
                    <asp:PlaceHolder ID="_columnOptionsPlaceholder" runat="server" Visible="<%#ContainsRatingScaleColumn || ContainsRadioButtonsColumn || ContainsCheckBoxesColumn || ContainsSliderColumn%>">
                        <tr class="header">
                            <asp:Repeater ID="_columnOptionsRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>'
                                OnItemDataBound="ColumnOptionsRepeater_OnItemDataBound">
                                <ItemTemplate>
                                    <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                    <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </asp:PlaceHolder>
                    <%-- Column Validation Messages --%>
                    <asp:PlaceHolder ID="_columnValidationMessagesPlaceholder" runat="server" Visible="<%#IsMatrixResponseValid %>">
                        <tr class="Error">
                            <asp:Repeater ID="_columnValidationMessagesRepeater" runat="server" DataSource='<%#MatrixItem.Columns %>'
                                OnItemDataBound="ColumnValidationMessages_OnItemDataBound">
                                <ItemTemplate>
                                    <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                    <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </asp:PlaceHolder>
                    <%-- Rows --%>
                    <asp:Repeater ID="_rowRepeater" runat="server"  DataSource='<%#MatrixItem.Rows %>' >
                        <ItemTemplate>
                            <tr class='<%#GetRowClass(((MatrixItemRow) Container.DataItem).RowNumber)%>'>
                                <asp:Repeater ID="_columnRepeater" runat="server"  DataSource='<%# MatrixItem.Columns %>'
                                    OnItemDataBound="MatrixChildItemCreated">
                                    <ItemTemplate>
                                        <asp:PlaceHolder ID="_childControlsPlace" runat="server"/>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <!--</tbody> -->
                </table>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
