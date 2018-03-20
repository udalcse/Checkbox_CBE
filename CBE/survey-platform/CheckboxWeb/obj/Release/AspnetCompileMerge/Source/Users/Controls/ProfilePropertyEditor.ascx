<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ProfilePropertyEditor.ascx.cs" Inherits="CheckboxWeb.Users.Controls.ProfilePropertyEditor" %>
<%@ Register TagPrefix="status" TagName="statuscontrol" Src="~/Controls/Status/StatusControl.ascx" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Management" %>

<div class="graphTableSection">
    <div style="width:100%">
        <div class="inline left" style="width: 50%">
            <asp:ListView ID="_profileList" runat="server" OnItemDataBound="ProfileList_ItemDataBound" OnDataBound="ProfileList_DataBound">
                <LayoutTemplate>
                    <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="formInput inlineStyle"">
                        <div class="left fixed_150">
                            <p class="truncate" style="padding-left: 15px"><asp:Label ID="_propertyName" runat="server" Text="<%# Container.DataItem %>"  AssociatedControlID="_propertyValue" /></p>
                        </div>
                        <div class="left">
                            <asp:TextBox ID="_propertyValue" runat="server" CssClass="left" Width="300" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:ListView>
            <asp:ListView ID="_profileRadioList" runat="server" OnItemDataBound="ProfileRadioList_ItemDataBound" OnDataBound="ProfileRadioList_DataBound">
                <LayoutTemplate>
                    <div id="radioProperties">
                        <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="formInput inlineStyle">
                        <div class="left fixed_150">
                            <p class="truncate"><asp:Label ID="_propertyName" runat="server" Text="<%# Container.DataItem %>"  AssociatedControlID="_radioButtonPanel" /></p>
                        </div>
                        <asp:Panel runat="server" ID="_radioButtonPanel" />
                    </div>
                </ItemTemplate>
            </asp:ListView>
        </div>
        <div class="inline right" style="width: 50%">
            <asp:ListView ID="_multiLineList" runat="server" OnItemDataBound="MultiLineList_ItemDataBound" OnDataBound="MultiLineList_DataBound">
                <LayoutTemplate>
                    <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="formInput inlineStyle">
                        <div class="left fixed_150">
                            <p class="truncate"><asp:Label ID="_propertyName" runat="server" Text="<%# Container.DataItem %>"  AssociatedControlID="_multiLineGridPanel" /></p>
                        </div>
                        <div class="left">
                            <asp:TextBox ID="_multiLineGridPanel" runat="server" CssClass="left multiLineProperty" Width="300" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </div>
    <div style="width:100%" class="left">
        <asp:ListView ID="_profileMatrixList" runat="server" OnItemDataBound="ProfileMatrixList_ItemDataBound" OnDataBound="ProfileMatrixList_DataBound">
            <LayoutTemplate>
                <div id="matrixProperties">
                    <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <div class="formInput">
                    <p>
                        <asp:Label ID="_propertyName" runat="server" Text="<%# Container.DataItem %>" CssClass="loginInfoLabel" AssociatedControlID="_matrixGridPanel" />
                    </p>
                    <asp:Panel runat="server" ID="_matrixGridPanel"/>
                </div>
            </ItemTemplate>
        </asp:ListView>
    </div>
</div>

<asp:Panel ID="_noPropertiesPanel" runat="server">
    <%= WebTextManager.GetText("/controlText/profilePropertyEditor/noProperties")%>
</asp:Panel>

<div id="validationError" class="error" style="display: none;padding-left:10px">There are validation errors on this page. Scroll up to see them.</div>

<script>

 <%if (ApplicationManager.AppSettings.UseHTMLEditor)
  {%>
    $(document).ready(function () {

        $('.multiLineProperty').tinymce({
            // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js")%>',
                height: 150,
                //cleanup_on_startup:true,
                remove_redundant_brs : true,
                forced_root_block : false,
                width: 540,
                relative_urls: false,
                remove_script_host: false,
                plugins: [
                    "image charmap textcolor code upload advlist link  table paste"
                ],
                paste_as_text: true,
                toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist indent outdent link |  table | styleselect fontselect fontsizeselect ",
                table_default_attributes: {
                    cellpadding: '5px',
                    border: '1px'
                },
       
                menubar: false,
                //Cause contents to be written back to base textbox on change
              
                gecko_spellcheck: true,
                fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
            });
    });

    <%
  }%>

    $(function () {

        $("body").on("click", ".add-row-btn", function (event) {
            event.preventDefault();
            var gridId = $(this).attr("table-id");
            var grid = $("#" + gridId);
            var lastRow = $(grid).find("tr:last-child").get(0);
            var hasRowsHeaders = $(grid).attr("has-rows-headers");
            var hasColumnHeaders = $(grid).attr("has-columns-headers");
            var inputValues = [];

            $(this).closest("table").find("input").each(function () {
                inputValues.push($(this).val());
            });
            //remove last element , this is add btn text
            inputValues.splice(-1, 1);
            if (hasRowsHeaders === "True") {
                var headerInput = $(this).closest("table").find("input")[0];
                if (inputValues[0] === "") {
                    $(headerInput).toggleClass('account-profile-input-required', true);
                    return;
                } else {
                    $(headerInput).toggleClass('account-profile-input-required', false);
                }
            }
            var newRow = lastRow.cloneNode(true);
            var rowLength = $(grid).find("tr").length + 2;
            if (hasColumnHeaders === "True")
                rowLength--;
            $(newRow).find("input").not(".hidden-input").not("input[id*='RowHeader']").each(function (index) {
                if (index === 0) {
                    if (hasRowsHeaders === "True") {
                        var hiddenInput = document.createElement("input");
                        $(hiddenInput).attr("style", "display: none");
                        $(hiddenInput).attr("value", inputValues[index]);
                        $(hiddenInput).attr("class", "hidden-input");
                        $(hiddenInput).attr("name", $(grid).attr("field-name") + "_RowHeader_" + (rowLength));

                        $(newRow).find("td:first-child").text(inputValues[index]);
                        $(newRow).find("td:first-child").append(hiddenInput);
                        inputValues.shift();
                    }
                }
                $(this).val(inputValues[index]);
                var baseInputName = getInputName(lastRow, grid);
                $(this).attr("name", baseInputName + "$ctl0" + rowLength + "$ctl0" + index);
            });

            //add remove link 
            var removeLink = document.createElement("a");
            $(removeLink).attr("table-id", gridId);
            $(removeLink).attr("class", "remove-row-btn");
            $(removeLink).text("Remove");
            $(newRow).find("td:last-child").find("a").remove();
            $(newRow).find("td:last-child").append(removeLink);
            $(this).closest("table").find("input").not(".add-row-btn").each(function () {
                $(this).val("");
            });
            $(grid).find("tbody").append(newRow);
            var rowsCounterId = $(grid).attr("rows-count-txt-id");
            var counterInput = $(grid).parents("[id*='matrixGridPanel']").find("[id*='" + rowsCounterId + "']");
            var inputValue = parseInt($(counterInput).val(), 10);
            $(counterInput).attr("value", inputValue + 1);
        });


        $("body").on("click", ".add-column-btn", function () {
            var grid = $("#" + $(this).attr("table-id"));
            var allRows = $(grid).find("tr");
            $(allRows).each(function (index) {
                if ($(this).attr('class') === "HeaderRow") {
                    var lastThElement = $(this).find("th:last-child");
                    var newThElement = $(lastThElement).clone(false);
                    $(newThElement).find("input").remove();
                    $(newThElement).find("span").text("");
                    var headerInput = document.createElement("input");
                    $(headerInput).css("width", "120px");
                    $(headerInput).attr("name", $(grid).attr("field-name") + "_ColumnHeader" + $(this).find("th").length);
                    $(newThElement).append(headerInput);
                    $(this).append(newThElement);
                } else {
                    var lastTxtBox = $(this).find("td:last-child");
                    var columnCount = $(this).find("td").length;
                    var input = $(lastTxtBox).find("input");
                    var lastIndex = input[0].name.lastIndexOf("$");
                    var uniqueNumber = input[0].name.substring(lastIndex, input[0].name.length).replace("$ctl0", "");
                    uniqueNumber = parseInt(uniqueNumber, 10) + columnCount;
                    var newTxtBox = $(lastTxtBox).clone(false);

                    $(newTxtBox).find("input").val("");
                    $(newTxtBox).find("input").attr("name", input[0].name.substring(0, lastIndex) + "$ctl0" + uniqueNumber);
                    $(this).append(newTxtBox);
                }
            });
            var columnsCounterId = $(grid).attr("columns-count-txt-id");
            var counterInput = $(grid).parents("[id*='matrixGridPanel']").find("[id*='" + columnsCounterId + "']");
            var inputValue = parseInt($(counterInput).val(), 10);
            var newColumnCount = inputValue + 1;
            $(counterInput).attr("value", newColumnCount);

            if (newColumnCount > 1) {
                $(this).prev().removeAttr("disabled");
            }
        });


        $("body").on("click", ".remove-column-btn", function () {
            var grid = $("#" + $(this).attr("table-id"));
            var allRows = $(grid).find("tr");

            $(allRows).each(function (index) {
                if ($(this).attr('class') === "HeaderRow") {
                    $(this).find("th:last-child").remove();
                }

                var lastInput = $(this).find("td:last-child");
                $(lastInput).remove();
            });

            var columnsCounterId = $(grid).attr("columns-count-txt-id");
            var counterInput = $(grid).parents("[id*='matrixGridPanel']").find("[id*='" + columnsCounterId + "']");
            var inputValue = parseInt($(counterInput).val(), 10);
            var newColumntCount = inputValue - 1;
            $(counterInput).attr("value", newColumntCount);

            if (newColumntCount === 1) {
                $(this).attr("disabled", "disabled");
            }
        });

        $("body").on("click", ".remove-row-btn", function () {
            $(this).closest("tr").remove();

            var tableId = $(this).attr("table-id");
            var grid = $("#" + tableId);
            var rowsCounterId = $(grid).attr("rows-count-txt-id");
            var counterInput = $(grid).parents("[id*='matrixGridPanel']").find("[id*='" + rowsCounterId + "']");
            var inputValue = parseInt($(counterInput).val(), 10);
            $(counterInput).attr("value", inputValue - 1);
        });

        $('body').on("click", "[id$='_okBtn']", function () {
            console.log("on validation");
            return validateMatrices();
        });

        function getInputName(lastRow, grid) {
            var firstRow = $(grid).find("tr:last-child"),
                fieldName = $(grid).attr("field-name"),
                hasRowsHeaders = $(grid).attr("has-rows-headers");

            var rowTds;

            if (hasRowsHeaders != 'False') {
                rowTds = $(firstRow).find("td").not(":first");
            } else {
                rowTds = $(firstRow).find("td");
            }

            var name = $(rowTds).first().find("input")[0].name;

            var index = name.indexOf(fieldName + "$") + fieldName.length;

            return name.substring(0, index);
        }

        function validateMatrices() {
            $("input[name*='_ColumnHeader']").each(function () {
                var missing = $(this).val() === "";
                $(this).toggleClass('account-profile-input-required', missing);
            });

            if ($(".account-profile-input-required").length !== 0) {
                //scrolling to first item with error

                var groupsPage = $("[id*='groupsPropertyEditor']").length > 0 ? true : false;

                if (groupsPage) {
                    var firstValidationInput = $('.account-profile-input-required').first(),
                        matrixTable = $(firstValidationInput).parents("[id*='matrixGridPanel']");;

                    var top = $("#aspnetForm").scrollTop() + $(matrixTable).offset().top - $(matrixTable).height() / 2;

                    $("#aspnetForm").scrollTop(top);
                } else {
                    var container = $('.dialogSubContainer'),
                        scrollTo = $('.account-profile-input-required').first();
                    container.scrollTop(
                        scrollTo.offset().top - container.offset().top + container.scrollTop() - 70
                    );

                }

                if ($('.account-profile-input-required').first()) {
                    setTimeout(function() {
                            $('.account-profile-input-required').first()
                                .removeClass('account-profile-input-required');
                        },
                        3000);
                }

                return false;
            }

            return true;
        }

        
    });

    


    $(function () {
        $("#aspnetForm").validate({
            rules: {
                email: {
                    email: true,
                    emailValidator: true
                }
            }
        });

        $.validator.methods.email = function (value, element) {
            return this.optional(element) || /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(value);
        }

        $("input[type=email]").on("change textInput input", function () {
            var saveButtons = $("[id$='__okBtn'], [id$='__nextButton']");
            if ($("#aspnetForm").valid()) {
                $(saveButtons).removeClass("disabled");
                $(saveButtons).css("pointer-events", "auto");
                $("#validationError").css("display", "none");
            } else {
                $(saveButtons).addClass("disabled");
                $(saveButtons).css("pointer-events", "none");
                $("#validationError").css("display", "block");
            }
        });
    });

    //// validates radio button options input. It's called from server side
    //function radioButtonValidation() {
    //    var selectedOption = $("[name='_radioFieldOptionSelect'] span.checked");
    //    if (selectedOption.length > 0)
    //        return false;
    //    var options = $("input[id&='_radioFieldOption']");
    //    for (var i = 0; i > options.length - 1; i++) {
    //        if (!options[i].val())
    //            return false;
    //    }
    //    return true;
    //}

</script>