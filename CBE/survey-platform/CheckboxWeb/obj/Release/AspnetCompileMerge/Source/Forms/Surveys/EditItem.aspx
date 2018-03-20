<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="EditItem.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.EditItem" MasterPageFile="~/Dialog.Master" EnableEventValidation="false" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="Controls/SurveyItemEditor.ascx" TagName="ItemEditor" TagPrefix="ckbx" %>

<asp:Content ID="_script" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/jquery-ui-timepicker-addon.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/globalHelper.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/itemEditor.js" />
    
    <script type="text/javascript">
        var _choiceTabIsOld = false;
        var setDefaultReadOnly = false;

        //Show message about empty item before the continuation of the wizard
        function ShowConfirmMessage(message) {
            showConfirmDialogWithCallback(message, confirmationApproved);
        }
        
        //Confirmation approved handler
        function confirmationApproved() {           
            $("#<%= _isConfirmed.ClientID %>").val('true');

            var okAction = $('#<%= Master.OkBtn.ClientID %>').attr('data-href');
            if (typeof okAction != "undefined")
                window.location = okAction;
            else
                window.location =  $('#<%= Master.OkBtn.ClientID %>').attr('href');
        }

        function onItemAdded(itemId, pageId) {
                var args = { op: 'addItem', result: 'ok', itemId: itemId, pageId: pageId };
                executeStringCallback('templateEditor.onItemAdded', args);
        }

        function okClick(itemId, pageId, isNew) {
            var args;

            if (isNew) {
                args = { op: 'addItem', result: 'ok', itemId: itemId, pageId: pageId };
                closeWindow('templateEditor.onItemAdded', args);
            }
            else {
                args = { op: 'editItem', result: 'ok', itemId: itemId, pageId: pageId };
                closeWindow('templateEditor.onItemEdited', args);
            }
        }
        

        function ToggleTabs(command) {
            //manager on tabs show\hide
            var tabs = $('#itemEditorTabs');
            //tab indexes to hide\show
            var tabsIndexes = [3, 4];

            for (var i = 0; i < tabsIndexes.length; i++) {
                tabs.ckbxTabs(command, tabsIndexes[i]);
            }
        };


        $(document).ready(function ()
        {
            $(window).keypress(function (event) {
                if (event.keyCode == 13 &&
                    !$(event.target).hasClass('quickEntryText') &&
                    !$(event.target).hasClass('questionText') &&
                    !$(event.target).hasClass('ace_text-input')) {
                        event.preventDefault();
                        return false;
                }
            });

            $('iframe[src*="youtube"]').each(function() 
            {
                var url = $(this).attr("src");
                $(this).attr("src", url + "?wmode=transparent");
                });

            //remove previously initialized editors
            if (typeof (tinymce) != 'undefined') {
                var i = 0;
                tinymce.editors = [];
                while (tinymce.editors.length > 0 && i <= tinymce.editors.length) {
                    tinyMCE.execCommand("mceRemoveEditor", false, tinymce.editors[tinymce.editors.length - 1].id);
                    i++;
                }               
            }

            // if matrix type and binding value is selected , hide tabs
            //TODO : code should be refactored 
            $(document).on("change", "select[id*='questionBindingSelector']", function (event) {
                event.preventDefault();
                event.stopPropagation();
                var isMatrix = "<%= IsMatrix %>";

                if (isMatrix === "True" && this.value !== "0" ) {
                    ToggleTabs("hideTab");
                } else {
                    ToggleTabs("showTab");
                }

                var itemTypeName = "<%= ItemTypeName %>";
                var questionBindingValue = $(this).find("option:selected").text().trim();
                if (questionBindingValue != "None" && (itemTypeName == "RadioButton" || itemTypeName == "SingleLineText" || itemTypeName == "MultiLineText")) {
                    debugger;
                    if (itemTypeName == "MultiLineText") {
                        $("#uniform-_behaviorEditor__pipeSelectorHtml_sourceList").parent().css("display", "none");
                        $("[id*='_defaultHtml_ifr']").contents().find("body").attr("contenteditable", "false");
                        $("[id$='_defaultText']").prop("readonly", true);
                        setDefaultReadOnly = true;
                    }
                    else {
                        $("#uniform-_behaviorEditor__pipeSelector_sourceList").parent().css("display", "none");
                        $("[id$='_defaultTextTxt']").prop("readonly", true);
                    }
                }
                else if (questionBindingValue == "None") {
                    if (itemTypeName == "MultiLineText") {
                        $("#uniform-_behaviorEditor__pipeSelectorHtml_sourceList").parent().css("display", "inline-block");
                        $("[id*='_defaultHtml_ifr']").contents().find("body").attr("contenteditable", "true");
                        $("[id$='_defaultText']").prop("readonly", false);
                        setDefaultReadOnly = false;
                    }
                    else {
                        $("#uniform-_behaviorEditor__pipeSelector_sourceList").parent().css("display", "inline-block");
                        $("[id$='_defaultTextTxt']").prop("readonly", false);
                    }
                }
            });

            // wait till tinymce is loaded
            setTimeout(function() {
                $("select[id*='questionBindingSelector']").trigger("change");
            }, 1500);


        });
        
    </script>
</asp:Content>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_pageContent">
    <input type="hidden" id="currentItemId" value="<%= ItemId %>" />

    <div style="display:none;">
        <asp:TextBox ID="_isConfirmed" runat="server" Text="false" />
    </div>

    <ckbx:ItemEditor ID="_itemEditor" runat="server" />
</asp:Content>
