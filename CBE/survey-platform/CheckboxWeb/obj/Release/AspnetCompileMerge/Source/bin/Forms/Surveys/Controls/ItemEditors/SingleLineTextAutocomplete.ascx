<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SingleLineTextAutocomplete.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.SingleLineTextAutocomplete" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    <% if(_sourceList.Visible) { %>
    var updateSourceVisibility = function () {
        var val = $('[name="<%=_sourceList.UniqueID%>"]:checked').val();
        if (val === 'List') {
            $('#<%=_remotePanel.ClientID%>').hide();
            $('#<%=_listPanel.ClientID%>').show();
        } else {
            $('#<%=_listPanel.ClientID%>').hide();
            $('#<%=_remotePanel.ClientID%>').show();
        }
    }
    <% } %>

    var updateListData = function () {
        var val = $('#<%=_autocompleteLists.ClientID%>').val();
        if (val === 'empty') {
            $('.autocomplete-list-editor').hide();
        } else {
            $('.autocomplete-list-editor').show();
            toggleInputs(false);

            if (val === 'new') {
                $('#<%=_listName.ClientID%>').val('');
                $('#<%=_listData.ClientID%>').val('');
            } else {
                var listId = parseInt(val);
                if (listId >=0) {
                    svcSurveyEditor.getAutocompleteListData(_at, listId).then(function(data) {
                        $('#<%=_listData.ClientID%>').val(data);
                    });
                    if (listId < 1000) 
                        toggleInputs(true);

                    $('#<%=_listName.ClientID%>').val($('#<%=_autocompleteLists.ClientID%> option:selected').text());
                }
            }
        }
    }

    var toggleInputs = function (disable) {
        $('#<%=_listData.ClientID%>').prop('disabled', disable);
        $('#<%=_listName.ClientID%>').prop('disabled', disable);        
    }

    $(function () {
        <% if(_sourceList.Visible) { %>
            updateSourceVisibility();
            $('#<%=_sourceList.ClientID%>').on('change', function () {
                updateSourceVisibility();
            });
        <% } %>

        updateListData();

        $('#<%=_autocompleteLists.ClientID%>').on('change', function () {
            updateListData();
        });
        $('#<%=_listData.ClientID%>').on('keypress', function (e) {
            if (e.which === 13) {
                e.preventDefault();
                $(this).val($(this).val() + "\n");
            }
        });
    });
</script>


<div class="formInput radioList">
    <ckbx:MultiLanguageRadioButtonList ID="_sourceList" runat="server" RepeatDirection="Vertical" AutoPostBack="False">
        <asp:ListItem Text="" TextId="/controlText/singleLineTextEditor/autocompleteList" Value="List" Selected="True"/>
        <asp:ListItem Text="" TextId="/controlText/singleLineTextEditor/remoteServer" Value="Remote" />
    </ckbx:MultiLanguageRadioButtonList>
</div>
<asp:Panel ID="_listPanel" runat="server">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_autocompleteLists" runat="server" TextId="/controlText/singleLineTextEditor/selectList" /></p>
    </div>
    <div class="formInput left">
        <ckbx:MultiLanguageDropDownList ID="_autocompleteLists" uniformignore="true" runat="server" AutoPostBack="false"/>
    </div>
    <br class="clear"/>
    <div class="autocomplete-list-editor">
        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_listName" runat="server" TextId="/controlText/singleLineTextEditor/listName" /></p>
        </div>
        <div class="formInput left">
            <asp:TextBox ID="_listName" MaxLength="30" runat="server" />
        </div>
        <br class="clear"/>            
    
        <div>
            <%=WebTextManager.GetText("/controlText/singleLineTextEditor/textEntryInstructions") %>
        </div>
        <asp:Label ID="_textEntryError" runat="server" CssClass="error message" Visible="false" style="margin-left: 0; margin-bottom: 5px;" />
        <asp:TextBox ID="_listData" runat="server" Rows="5" Columns="60" TextMode="MultiLine" />
        <br class="clear"/>
    </div>
</asp:Panel>

<asp:Panel ID="_remotePanel" runat="server">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_remoteServer" runat="server" TextId="/controlText/singleLineTextEditor/serverUrl" /></p>
    </div>

    <asp:TextBox runat="server" ID="_remoteServer" Width="400px" />
    <br class="clear"/>
</asp:Panel>




