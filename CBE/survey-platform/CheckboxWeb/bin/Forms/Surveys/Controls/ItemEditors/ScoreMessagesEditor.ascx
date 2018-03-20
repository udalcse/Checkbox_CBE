<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ScoreMessagesEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.ScoreMessagesEditor" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Common" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('.numeric').numeric({ decimal: true, negative: true });
        $('#messagesGrid input').css('width', '90%');
        $('.hidden').hide();
        
        $(document).on('click', '#messagesGrid .editBtn', function () {
            var id = $(this).attr('messageId');
            showInputs(id);
            return false;
        });
        
        $(document).on('click', '#messagesGrid .cancelBtn', function () {
            var id = $(this).attr('messageId');
            hideInputs(id);
            return false;
        });
        
        $(document).on('click', '#messagesGrid .deleteBtn', function () {
            var id = $(this).attr('messageId');
            var row = $('#messagesGrid tr[messageId="' + id + '"]');
            row.detach();
            if($('#messagesGrid tr').length <= 1) {
                $('#messagesGrid').hide();
            }
            postOptions();
        });
        
        $(document).on('click', '#messagesGrid .updateBtn', function () {
            var id = $(this).attr('messageId');
            var row = $('#messagesGrid tr[messageId="' + id + '"]');
            var high = row.find('.message-high input').val();
            var low = row.find('.message-low input').val();
            row.find('.message-high span').html(high == '' ? '0' : high);
            row.find('.message-low span').html(low == '' ? '0' : low);
            var getEncodedText = htmlEncode(row.find('.message-body input').val());
            row.find('.message-body span').html(getEncodedText);
            hideInputs(id);
            postOptions();
        });
        
        if ($('#messagesGrid tr').length <= 1) {
            $('#messagesGrid').hide();
        }
    });

    function postOptions() {
        collectMessages();
        $('#<%= _postMessagesBtn.ClientID %>').click();
    }

    function showInputs(messageId) {
        var row = $('#messagesGrid tr[messageId="' + messageId + '"]');
        row.find('.update-cancel').show();
        row.find('.edit-delete').hide();

        row.find('.message-low input, .message-high input, .message-body input').show();
        row.find('.message-low span, .message-high span, .message-body span').hide();
        return false;
    }
    
    function hideInputs(messageId) {
        var row = $('#messagesGrid tr[messageId="' + messageId + '"]');
        row.find('.update-cancel').hide();
        row.find('.edit-delete').show();

        row.find('.message-low input, .message-high input, .message-body input').hide();
        row.find('.message-low span, .message-high span, .message-body span').show();
        return false;
    }

    function collectMessages() {
        var str = '<xml><messages>';
        $.each($('#messagesGrid tr').not('.HeaderRow'), function (index, elem) {
            str += '<message id="' + $(elem).find('input[type="hidden"]').val()
                + '" low="' + $(elem).find('.message-low span').text()
                + '" high="' + $(elem).find('.message-high span').text() + '">';
            str += $(elem).find('.message-body span').html();
            str += '</message>';
        });
        str += '</messages></xml>';
        $('#normalEntryOptionOrder').val(str);
    }
</script>

<asp:Button ID="_postMessagesBtn" CssClass="hidden" runat="server"/>
<input type="hidden" id="normalEntryOptionOrder" name="normalEntryOptionOrder" />
<div style="max-height: 225px; overflow-y: auto;">
<table id="messagesGrid" class="DefaultGrid border999 shadow999" style="width: 600px;">
    <tbody>
    <tr class="HeaderRow">
        <th scope="col" style="display: none;width: 0px;"> ID </th>
        <th scope="col"> Low Score </th>
        <th scope="col"> High Score </th>
        <th scope="col" style="text-align: left;"> Message </th>
        <th scope="col"></th> 
    </tr>
<% for(int i=0; i<Messages.Count(); i++) { %>
    <tr class="<%= (i % 2 == 0 ? "EvenRow" : "OddRow") %>" messageId="<%= Messages[i].MessageId %>">
        <td class="hidden" style="width: 0px;">
            <input type="hidden" value="<%= Messages[i].MessageId %>"/> 
        </td>
        <td class="message-low" style="width:70px;text-align: center;">
            <input maxlength="5" class="hidden numeric" type="text" value="<%= Messages[i].LowScore %>" /> 
            <span><%= Messages[i].LowScore %></span> 
        </td>
        <td class="message-high" style="width:80px;text-align: center;">
            <input maxlength="5" class="hidden numeric" type="text" value="<%= Messages[i].HighScore %>" /> 
            <span><%= Messages[i].HighScore %></span> 
        </td>
        <td class="message-body">
            <input class="hidden" type="text" value="<%= Utilities.AdvancedHtmlEncode(Messages[i].MessageText) %>" /> 
            <span><%= Utilities.AdvancedHtmlEncode(Messages[i].MessageText) %></span> 
        </td>
        <td style="width:130px;text-align: center;">
            <div class="edit-delete">
                <a messageId="<%= Messages[i].MessageId %>" class="editBtn ckbxButton roundedCorners shadow999 border999 orangeButton smallButton" style="float: left;" >Edit</a>
                <a messageId="<%= Messages[i].MessageId %>" class="deleteBtn ckbxButton roundedCorners shadow999 border999 redButton smallButton" style="float: left;" >Delete</a>                    
            </div>
            <div class="hidden update-cancel">
                <a messageId="<%= Messages[i].MessageId %>" class="updateBtn ckbxButton roundedCorners shadow999 border999 orangeButton smallButton" style="float: left;" >Update</a>
                <a messageId="<%= Messages[i].MessageId %>" class="cancelBtn ckbxButton roundedCorners shadow999 border999 redButton smallButton" style="float: left;" >Cancel</a>
            </div>
        </td>
    </tr>
<% } %>
    </tbody>
</table>
</div>


<div class="spacing"></div>
<div class="dialogSubTitle">
    <%=WebTextManager.GetText("/controlText/scoreMessageItemEditor/newMessage")%>
</div>
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_minScoreTxt" ID="_minScoreLbl" runat="server" TextId="/controlText/scoreMessageItemEditor/minimumScore" /></p>
    <asp:TextBox ID="_minScoreTxt" CssClass="numeric" MaxLength="5" runat="server" />

    <p><ckbx:MultiLanguageLabel AssociatedControlID="_maxScoreTxt" ID="_maxScoreLbl" runat="server" TextId="/controlText/scoreMessageItemEditor/maximumScore" /></p>
    <asp:TextBox ID="_maxScoreTxt" CssClass="numeric" MaxLength="5" runat="server" />

    <p><ckbx:MultiLanguageLabel AssociatedControlID="_messageText" ID="_messageLbl" runat="server" TextId="/controlText/scoreMessageItemEditor/message" /></p>
    <asp:TextBox ID="_messageText" runat="server" Width="400" />
    <btn:CheckboxButton runat="server" ID="_addMessageBtn" TextId="/controlText/scoreMessageItemEditor/addMessage" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" />
</div>