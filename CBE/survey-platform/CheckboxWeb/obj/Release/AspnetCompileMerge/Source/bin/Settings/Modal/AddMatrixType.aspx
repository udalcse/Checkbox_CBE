<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddMatrixType.aspx.cs" Inherits="CheckboxWeb.Settings.Modal.AddMatrixType" MasterPageFile="~/Dialog.Master" %>

<%@ MasterType VirtualPath="~/Dialog.Master" %>


<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
<%--    
  <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>--%>
    <div class="add-matrix-container" >
        <div class="input">
            <ckbx:MultiLanguageLabel ID="_rowsCountLabel" runat="server" TextId="/pageText/settings/modal/addMatrixType/rowsCountTitle" AssociatedControlID="_rowsCount" />
            <asp:TextBox ID="_rowsCount"  TextMode="Number" runat="server" min="0" max="20" step="1" AutoPostBack="True" CausesValidation="True" Text="0"  CssClass="rows-count-input"  />
            <asp:RangeValidator ID="_rowsValidator" ControlToValidate="_rowsCount" MinimumValue="0" MaximumValue="20" Type="Integer" runat="server"  ErrorMessage="Please enter in a number between 0 and 20." ></asp:RangeValidator>
        </div>
        <br class="clear" />
        <div class="input">
            <ckbx:MultiLanguageLabel ID="_columnCountLabel" runat="server" TextId="/pageText/settings/modal/addMatrixType/columnCountTitle" AssociatedControlID="_columnCount" />
            <asp:TextBox ID="_columnCount"  TextMode="Number" runat="server" min="0" max="20" step="1" AutoPostBack="True" Text="0" CausesValidation="True"   CssClass="column-count-input" />
            <asp:RangeValidator ID="_columnValidator" ControlToValidate="_columnCount" MinimumValue="0" MaximumValue="20" Type="Integer" runat="server" ErrorMessage="Please enter in a number between 0 and 20."></asp:RangeValidator>
        </div>
        <br class="clear" />
        <div class="input">
            <ckbx:MultiLanguageLabel ID="_headersTitle" runat="server" TextId="/pageText/settings/modal/addMatrixType/addColumnHeaderTitle" AssociatedControlID="_headersCheckbox" />
            <asp:CheckBox ID="_headersCheckbox" runat="server" AutoPostBack="True"  CssClass="add-headers-checkbox" />
        </div>
        <br class="clear" />
        <div class="input">
            <ckbx:MultiLanguageLabel ID="_rowHeadersTitle" runat="server" TextId="/pageText/settings/modal/addMatrixType/addRowHeaderTitle" AssociatedControlID="_rowHeadersCheckbox" />
            <asp:CheckBox ID="_rowHeadersCheckbox" runat="server" AutoPostBack="True"  CssClass="add-row-headers-checkbox" />
        </div>
        <div class="add-matrix-grid">
            <ckbx:MultiLanguageLabel ID="_matrixLavel" runat="server" TextId="/pageText/settings/modal/addMatrixType/matrixPreview" AssociatedControlID="matrixGrid"  />
            <span class="matrix-table-validation-error" style="display: none; color: red; margin-left: 30px;"></span>
            <asp:GridView OnRowDataBound="OnRowDataBound" CellPadding="10" CellSpacing="10" runat="server" id="matrixGrid"></asp:GridView>
        </div>
        <%if (!string.IsNullOrEmpty(_columnCount.Text) && int.Parse(_columnCount.Text) != 0) %>
        <%{ %>
        <div class="col-sizes">
            <ckbx:MultiLanguageLabel ID="_columnWidthsTitle" runat="server" TextId="/pageText/settings/modal/addMatrixType/colWidth" AssociatedControlID="_colSizeRepeater"  />
            <asp:Repeater ID="_colSizeRepeater" runat="server">
              <HeaderTemplate>
                  <table>
                  <tr>
              </HeaderTemplate>
              <ItemTemplate>
                <td class="col-sizes-row" data-index="<%# DataBinder.Eval(Container, "ItemIndex", "")%>">
                    <asp:TextBox  TextMode="Number" class="column-size-txt" ID="_colSize" runat="server" Text="<%# Container.DataItem.ToString() %>"></asp:TextBox>px<br />
                    <asp:RangeValidator ID="_colSizeValidator" ControlToValidate="_colSize" MinimumValue="0" MaximumValue="2000" Type="Integer" runat="server" ErrorMessage="Please enter an integer number."></asp:RangeValidator>
                </td>
              </ItemTemplate>
              <FooterTemplate>
                  </tr>
                  </table>
              </FooterTemplate>
            </asp:Repeater>
        </div>
        <%} %>
        <div class=" left fixed_100">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_gridLinesList" ID="_gridLinesLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/gridLines" /></p>
        </div>
        <div class="formInput left">
            <ckbx:MultiLanguageDropDownList ID="_gridLinesList" runat="server" uframeignore="true">
                <asp:ListItem Value="None" Text="None" TextId="/controlText/matrixItemAppearanceEditor/none" />
                <asp:ListItem Value="Both" Text="Both" TextId="/controlText/matrixItemAppearanceEditor/both" />
                <asp:ListItem Value="Vertical" Text="Vertical" TextId="/controlText/matrixItemAppearanceEditor/vertical" />
                <asp:ListItem Value="Horizontal" Text="Horizontal" TextId="/controlText/matrixItemAppearanceEditor/horizontal" />
            </ckbx:MultiLanguageDropDownList>
        </div>
        <br class="clear"/>
        <asp:HiddenField runat="server" ID="_currentFieldName"/>
    </div>
    <script>
      
        $(function () {
            $(".column-size-txt").each(function (index, value) {
                setColumnWidth(value);
            });
        });

        //since it is iframe we need to call parent function to restore drop down value to previous one 
        $(document).on("click", "a[id*='cancel']", function() {
            var fieldName = $("[id*='_currentFieldName']").val();
            parent.restoreDropDownValue(fieldName);
        });

        $(".column-size-txt").change(function () {
            setColumnWidth(this);
        });

        function MatrixGridValidation() {
            var table = $("table[id*='_matrixGrid']");
            var headerValues = [];

            $(table).find(".HeaderRow input").each(function () {
                if ($(this).val() !== "") 
                    headerValues.push($(this).val());
            });

            if (hasDuplicates(headerValues)) {
                $(".matrix-table-validation-error").text("Cell headers should be unique").fadeIn('slow').animate({ opacity: 1.0 }, 1500).fadeOut(5000);
                return false;
            } else {
                $(table.find('input')).each(function() {
                    var missing = $(this).val() === "";
                    $(this).parent().toggleClass('error', missing);
                });

                if (table.find(".error").length > 0) {
                    $(".matrix-table-validation-error").text("Please fill cells with unique values").fadeIn('slow').animate({ opacity: 1.0 }, 1500).fadeOut(5000);
                }
            }

            var hasColumnSizeErrors = false;
            $(".column-size-txt").each(function (index, value) {
                if (!isInt($(value).val())) {
                    hasColumnSizeErrors = true;
                    $("[id$='_colSizeValidator']").first().css("visibility", "visible");
                    return false;
                }
            });
            
            return table.find(".error").length == 0 && !hasDuplicates(headerValues) && !hasColumnSizeErrors;
        }

        function isInt(value) {
            return !isNaN(value) &&
                   parseInt(Number(value)) == value &&
                   !isNaN(parseInt(value, 10));
        }

        function setColumnWidth(columnWidth) {
            var curColSize = $(columnWidth).val();
            if (curColSize == 0) curColSize = 66;
            var currColIndex = $(columnWidth).parent("td").attr("data-index");
            $("[id$='_matrixGrid']").css("width", "auto");
            $("[id$='_matrixGrid'] tr").each(function (index, value) {
                var cols = $(this).find("td");
                $(cols).each(function (idx, val) {
                    var headerInput = $(this).find("input");
                    var headerSpan = $(this).find("span");
                    var margin = 2;
                    if (idx == currColIndex) {
                        $(this).css("width", curColSize);
                        if (headerInput)
                            $(headerInput).css("width", curColSize - 2);
                        if (headerSpan)
                            $(headerSpan).css("width", curColSize - 2);
                    }
                });
            });
        }

    </script>
</asp:Content>

