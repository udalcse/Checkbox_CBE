<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="false" CodeBehind="EditInfo.aspx.cs" Inherits="CheckboxWeb.Users.EditInfo" %>

<%@ MasterType VirtualPath="~/Admin.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Register Src="../Controls/Status/StatusControl.ascx" TagName="StatusControl" TagPrefix="status" %>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">

    <style>
        #footer {
            position: fixed;
            background: #eeeeee;
            border-top: 1px solid #dddddd;
            bottom: 0;
            color: #666666;
            font-size: 0.85em;
            height: 100px;
        }

        .overflow-auto {
            overflow: auto;
        }
    </style>

    <script>
        var resizeIntervalId;
        var originalContentHeight;



   <%if (ApplicationManager.AppSettings.UseHTMLEditor)
  {%>
        $(document).ready(function () {

            $('.multiLineProperty').tinymce({
                // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js")%>',
            height: 150,
            //cleanup_on_startup:true,
            remove_redundant_brs: true,
            forced_root_block: false,
            width: 550,
            relative_urls: false,
            remove_script_host: false,
            plugins: [
                 "image charmap textcolor code upload advlist link  table paste"
            ],
            toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist indent outdent link |  table | styleselect fontselect fontsizeselect ",
            table_default_attributes: {
                cellpadding: '5px',
                border: '1px'
            },
            paste_as_text: true,
            menubar: false,
            //Cause contents to be written back to base textbox on change

            gecko_spellcheck: true,
            fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
        });
    });

    <%
  }%>


        $("body").on("click", ".remove-row-btn", function () {
            $(this).closest("tr").remove();

            var tableId = $(this).attr("table-id");
            var grid = $("#" + tableId);
            var rowsCounterId = $(grid).attr("rows-count-txt-id");
            var counterInput = $(grid).parents("[id*='matrixGridPanel']").find("[id*='" + rowsCounterId + "']");
            var inputValue = parseInt($(counterInput).val(), 10);
            $(counterInput).attr("value", inputValue - 1);
        });


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
                    $(headerInput).toggleClass('account-profile-input-required-row', true);
                    return;
                } else {
                    $(headerInput).toggleClass('account-profile-input-required-row', false);
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


        $(function() {
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

            $("input[type=email]").change(function() {
                var saveButton = $("#<%= _profileChangeButton.ClientID %>");
                if ($("#aspnetForm").valid()) {
                    $(saveButton).removeClass("disabled");
                    $(saveButton).css("pointer-events", "auto");
                } else {
                    $(saveButton).addClass("disabled");
                    $(saveButton).css("pointer-events", "none");
                }
            });

            renderIconsForEmails();
        });

        function renderIconsForEmails() {
            var emails = $("input[type=email]");
            emails.prev(".email-icon").css("display", "inline-block");
        }

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
            $("input[name*='_ColumnHeader']").each(function() {
                var missing = $(this).val() === "";
                $(this).toggleClass('account-profile-input-required', missing);
            });


            var errors = $(".matrix-profile-properties").find(".account-profile-input-required");

            if (errors.length > 0) {
                var position = $(errors).first().position().top;
                $("html body").animate({ scrollTop: position - 50 }, 0);
            }

            return $(errors).length == 0;
        }


    </script>

    <div class="grid_12">
        <status:StatusControl ID="_statusControl" runat="server" />
    </div>
    <div class="grid_12 overflow-auto">
        <div class="viewport">
            <div class="padding15">
                <div class="left padding15" class="loginPanel">
                    <asp:Panel ID="_passwordPanel" runat="server">
                        <ckbx:MultiLanguageLabel ID="_passwordTitle" runat="server" TextId="/pageText/users/editInfo.aspx/passwordTitle" CssClass="dialogSubTitle sectionTitle" />
                        <div class="formInput">
                            <p>
                                <ckbx:MultiLanguageLabel ID="_passwordLabel" runat="server" AssociatedControlID="_password" TextId="/pageText/users/editInfo.aspx/password" CssClass="loginInfoLabel" />
                            </p>
                            <asp:TextBox ID="_password" runat="server" TextMode="Password" CssClass="loginInfo" MaxLength="255" />
                            <div style="margin-top: 5px">
                                <asp:RequiredFieldValidator ID="_passwordRequired" runat="server" ControlToValidate="_password" Display="Dynamic" ValidationGroup="Password" CssClass="error message"><%= WebTextManager.GetText("/pageText/users/editInfo.aspx/passwordRequired")%></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="formInput">
                            <p>
                                <ckbx:MultiLanguageLabel ID="_newPasswordLabel" runat="server" AssociatedControlID="_newPassword" TextId="/pageText/users/editInfo.aspx/newPassword" CssClass="loginInfoLabel" />
                            </p>
                            <asp:TextBox ID="_newPassword" runat="server" TextMode="Password" CssClass="loginInfo" MaxLength="255" />
                            <div style="margin-top: 5px">
                                <asp:RequiredFieldValidator ID="_newPasswordRequired" runat="server" ControlToValidate="_newPassword" Display="Dynamic" ValidationGroup="Password" CssClass="error message"><%= WebTextManager.GetText("/pageText/users/editInfo.aspx/newPasswordRequired")%></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ValidationGroup="UserInfo" ID="_passwordLength" runat="server" Display="Dynamic" ControlToValidate="_password" CssClass="error message" ValidationExpression="[\w\s]{1,255}"><%= WebTextManager.GetText("/pageText/users/editInfo.aspx/passwordLength") %></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <div class="formInput" style="padding-bottom: 5px;">
                            <p>
                                <ckbx:MultiLanguageLabel ID="_passwordConfirmLabel" runat="server" AssociatedControlID="_passwordConfirm" TextId="/pageText/users/editInfo.aspx/confirmPassword" CssClass="loginInfoLabel" />
                            </p>
                            <asp:TextBox ID="_passwordConfirm" runat="server" TextMode="Password" CssClass="loginInfo" MaxLength="255" /><ckbx:MultiLanguageLabel ID="_confirmPasswordError" runat="server" CssClass="error message" Visible="false" />
                            <div style="margin-top: 5px">
                                <asp:RequiredFieldValidator ID="_passwordConfirmRequired" runat="server" ControlToValidate="_passwordConfirm" Display="Dynamic" ValidationGroup="Password" CssClass="error message"><%= WebTextManager.GetText("/pageText/users/editInfo.aspx/confirmPasswordRequired")%></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="_compareValidator" runat="server" ControlToValidate="_passwordConfirm" ControlToCompare="_newPassword" Display="Dynamic" ValidationGroup="Password" CssClass="error message"><%= WebTextManager.GetText("/pageText/users/editInfo.aspx/passwordMismatch")%></asp:CompareValidator>
                            </div>
                        </div>

                        <div style="margin-top: 30px;">
                            <btn:CheckboxButton ID="_passwordChangeButton" runat="server" TextId="/pageText/users/editInfo.aspx/changePasswordButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" ValidationGroup="Password" CausesValidation="true" OnClick="ChangePasswordButtton_Click" />
                        </div>
                        <div class="clear"></div>
                    </asp:Panel>
                </div>
                <div class="left padding15 profilePanel">
                    <asp:Panel ID="_profilePanel" runat="server">
                        <ckbx:MultiLanguageLabel ID="_profileTitle" runat="server" TextId="/pageText/users/editInfo.aspx/profileInfoTitle" CssClass="dialogSubTitle sectionTitle emailSection" />
                        <div class="profileFrame">
                            <div class="profileContent">
                                <div class="single-line-profile-properties">
                                    <div class="formInput">
                                        <p>
                                            <ckbx:MultiLanguageLabel ID="_emailLabel" runat="server" AssociatedControlID="_email" TextId="/pageText/users/editInfo.aspx/email" CssClass="loginInfoLabel" />
                                        </p>
                                        <asp:TextBox ID="_email" runat="server" CssClass="loginInfo fixed_300" MaxLength="255" /><asp:RequiredFieldValidator ID="_emailRequired" runat="server" ControlToValidate="_email" ValidationGroup="Profile" Display="Dynamic" CssClass="error message" Enabled="false"><%= WebTextManager.GetText("/pageText/users/editInfo.aspx/emailRequired")%></asp:RequiredFieldValidator>
                                        <ckbx:MultiLanguageLabel ID="_emailFormatInvalidLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/editInfo.aspx/emailInvalid" />
                                    </div>


                                    <asp:ListView ID="_profileList" runat="server" OnItemDataBound="ProfileList_ItemDataBound" OnDataBound="ProfileList_DataBound">
                                        <LayoutTemplate>
                                            <div id="properties">
                                                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                                            </div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <div class="formInput">
                                                <p>
                                                    <asp:Label ID="_propertyName" runat="server" Text="<%# Container.DataItem %>" CssClass="loginInfoLabel" AssociatedControlID="_propertyValue" />
                                                </p>
                                                <img src="<%= ResolveUrl("~/Resources/images/envelop.png") %>" class="email-icon" style="display: none"/>
                                                <asp:TextBox ID="_propertyValue" runat="server" CssClass="loginInfo" /><br />
                                                <ckbx:MultiLanguageLabel ID="_emailInvalidLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/editInfo.aspx/emailInvalid" />
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
                                            <div class="formInput">
                                                <p>
                                                    <asp:Label ID="_propertyName" runat="server" Text="<%# Container.DataItem %>" CssClass="loginInfoLabel" AssociatedControlID="_radioButtonPanel" />
                                                </p>
                                                <asp:Panel runat="server" ID="_radioButtonPanel" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:ListView>

                                </div>
                                <div class="multi-line-profile-properties">
                                    <asp:ListView ID="_profileMultiLineList" runat="server" OnItemDataBound="ProfileMultiLIneList_ItemDataBound" OnDataBound="ProfileMultiLIneList_DataBound">
                                        <LayoutTemplate>
                                            <div id="multiLineProperties">
                                                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                                            </div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <div class="formInput">
                                                <p>
                                                    <asp:Label ID="_propertyName" runat="server" Text="<%# Container.DataItem %>" CssClass="loginInfoLabel" AssociatedControlID="_multiLinePropertyValue" />
                                                </p>
                                                <asp:TextBox ID="_multiLinePropertyValue" runat="server" CssClass="loginInfo multiLineProperty" /><br />
                                            </div>
                                        </ItemTemplate>
                                    </asp:ListView>

                                    <div class="clear"></div>
                                </div>

                                <div class="clear"></div>


                            </div>

                        </div>
                    </asp:Panel>
                </div>
                <div class="clear"></div>
                <div class="matrix-profile-properties">
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
                                <asp:Panel runat="server" ID="_matrixGridPanel" />
                            </div>
                        </ItemTemplate>
                    </asp:ListView>

                    <div class="clear"></div>
                </div>
            </div>
            <div class="save-profile-fields">
                <btn:CheckboxButton ID="_profileChangeButton" runat="server" TextId="/pageText/users/editInfo.aspx/changeProfileButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" ValidationGroup="Profile" CausesValidation="true" OnClick="EditProfileButtton_Click" />
            </div>
        </div>
    </div>
</asp:Content>
