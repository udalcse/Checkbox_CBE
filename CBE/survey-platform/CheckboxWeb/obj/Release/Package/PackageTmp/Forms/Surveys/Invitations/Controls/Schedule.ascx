<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Schedule.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.Schedule" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript">
    <%-- Ensure service initialized --%>
    $(document).ready(function() {
        // Remove
         $('#removeScheduleButton').click(function(){
            if($('.selectScheduleItem:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/schedule.ascx/removeConfirmation") %>', 
                    onScheduleRemoveConfirm,
                    390,
                    240,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/schedule.ascx/removeTitle") %>'
                );
            }
        });
        $(document).on('click', '#_selectAllSchedule', function(){
            if ($(this).attr('checked'))
                $('.selectScheduleItem').attr('checked', 'checked');
            else
                 $('.selectScheduleItem').removeAttr('checked');
        });
        $(document).on('click', '.selectScheduleItem', function(event){
                event.stopPropagation();
        });

        $(document).on('click', '.cancelScheduleImg', function(e)
            {
                e = e || window.event;
                e.stopPropagation();
                var scheduleId = $(e.target).attr("scheduleItemId");
                $('input.dateTimePickerInput[scheduleItemId="' + scheduleId + '"]').val($('input.dateTimePickerInput[scheduleItemId="' + scheduleId + '"]').attr('oldvalue'));
                $('.saveScheduleImg[scheduleItemId="' + scheduleId + '"]').hide();
                $('.cancelScheduleImg[scheduleItemId="' + scheduleId + '"]').hide();
            }
        );

        $(document).on('click', '.saveScheduleImg', function(e)
            {
                e = e || window.event;

                e.stopPropagation();

                var scheduleId = $(e.target).attr("scheduleItemId");

                var input = $('input.dateTimePickerInput[scheduleItemId="' + scheduleId + '"]');
                if (input.val() == "") {
                    return;
                }

                if (input.val() == input.attr('oldvalue'))
                    return;

                updateScheduleItemDate(scheduleId,  $(input).datepicker("getDate"), errorFunction);

                input.attr('oldvalue', input.val());
                $('.saveScheduleImg[scheduleItemId="' + scheduleId + '"]').hide();
                $('.cancelScheduleImg[scheduleItemId="' + scheduleId + '"]').hide();
            }
        );

    });
     
    function renderCompleteScheduleList()
    {
        if (!$('.selectScheduleItem').length)
        {
            $('#_selectAllSchedule').hide();
        }
        else
        {
            $('#removeScheduleButton').show();
        }

        $('.dateTimePickerInput').datetimepicker({
                    numberOfMonths: 2,
                    minDate: new Date(),
                    defaultDate: new Date(),
                    onClose: function() {                    
                        var scheduleId = $(this).attr("scheduleItemId");

                        var input = $('input.dateTimePickerInput[scheduleItemId="' + scheduleId + '"]');
                        if (input.val() == "") {
                            return;
                        }

                        if (input.val() == input.attr('oldvalue'))
                            return;

                        $('.cancelScheduleImg[scheduleItemId="' + scheduleId + '"]').show();
                        $('.saveScheduleImg[scheduleItemId="' + scheduleId + '"]').show();
                    }
                });
        $(document).on('blur', '.dateTimePickerInput',
            function() {
                    //if user has left date empty then set the date as current date plus an hour
                    if($(this).val() == "") {
                        var date = new Date();
                        date.setHours(date.getHours() + 1);
                        $(this).datepicker( "setDate" , date);
                    }
                });
          $(document).on('click', '.dateTimePickerInput', function (event) { event.stopPropagation(); });
    }
     
    //Set new schedule item date for invitaton  
    function   updateScheduleItemDate(scheduleId, newDate, serviceErrorCallback) {
        svcInvitationManagement.setScheduledDate(
        _at,
        <%=InvitationId %>,
        scheduleId,
        newDate,
        onScheduleUpdated,
        {oldScheduleId : scheduleId},
        serviceErrorCallback
        );
    }

    function onScheduleUpdated(scheduleId, params)
    {
        $('[scheduleitemid="' + params.oldScheduleId +'"]').attr('scheduleitemid', scheduleId);
    }

    //if error occurs while updating
    function errorFunction(data) {
        if (typeof(data) != 'undefined')
            alert(data.FailureMessage);
        else
            alert("Error while new schedule item date updating!");
        reloadScheduleGrid();
    }
    //
    function reloadScheduleGrid() {
        //Reload grid
        <%=_scheduleGrid.ReloadGridHandler%>(true);
    }

    <%-- Load Recipient list --%>
    function loadInvitationScheduleList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs ){
        svcInvitationManagement.listInvitationSchedule(
            _at,
            <%=InvitationId %>,
            sortAscending,
            currentPage,
            <%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
            loadCompleteCallback,
            loadCompleteArgs
        );
    }
    //
    function getSelectedScheduleItemArray(){
        var scheduleItemIdArray = new Array();
        
        $('.selectScheduleItem:checked').each(function(index){
            scheduleItemIdArray.push($(this).attr('scheduleItemId'));
        });

        return scheduleItemIdArray;
    }


    function onScheduleRemoveConfirm(args){
        if(args.success){
            var ScheduleItems = getSelectedScheduleItemArray();
      
            svcInvitationManagement.deleteScheduleItems(
                _at,
                <%=InvitationId %>,
                ScheduleItems,
                function(result){
                    if (result == 'false') {                               
                        statusControl.initialize('ScheduleStatusContainer');
                        statusControl.showStatusMessage(
                            '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/schedule.ascx/removeSuccessful") %>'.replace('{0}', ScheduleItems.ids.length),
                            StatusMessageType.success);
                    }
                    reloadScheduleGrid();

                    <%if (!String.IsNullOrEmpty(OnScheduleItemsDeleted))
                        {%>
                        <%=OnScheduleItemsDeleted %>();
                    <%
                        }%>
                }
            );
        }
    }

    //
  	function onReminderClick(reminder){
  	    if(reminder.InvitationActivityType == 'Reminder') {
  	        if(reminder.ProcessingFinished == null) {
		          location.href = '<%=ResolveUrl("~/Forms/Surveys/Invitations/Message.aspx?i={0}&reminder=true&scheduleEdit={1}") %>'.replace('{0}', reminder.InvitationID).replace('{1}', 'true');
              } else {
		          location.href = '<%=ResolveUrl("~/Forms/Surveys/Invitations/ReminderDetails.aspx?i={0}&scheduleEdit={1}") %>'.replace('{0}', reminder.InvitationID).replace('{1}', 'true');
              }
  	    }
	}

</script>

<div class="dashStatsWrapper border999 shadow999">
    <ckbx:Grid ID="_scheduleGrid" runat="server" GridCssClass="ckbxRecipientsGrid" />
</div>

<div class="padding10 left" style="width:40%">
    <a class="ckbxButton roundedCorners border999 shadow999 redButton" id="removeScheduleButton" style="display:none"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/schedule.ascx/deleteInvitation")%></a>
</div>

<%--<div class="padding10 right" style="width:40%; visibility: hidden;">
    <btn:CheckboxButton ID="_sendReminderBtn" OnClick="_sendReminderBtn_Click" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton right" TextID="/controlText/invitationDashboard.ascx/sendReminderNow" uframeignore="true" style="color:white;" /> 
</div>--%>

<script type="text/C#" runat="server">

    /// <summary>
    /// Get/set callback for schedule items deleted event
    /// </summary>
    public String OnScheduleItemsDeleted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _scheduleGrid.InitialSortField = "Scheduled";
        _scheduleGrid.ListTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/scheduleListTemplate.html");
        _scheduleGrid.ListItemTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/scheduleListItemTemplate.html");
        _scheduleGrid.LoadDataCallback = "loadInvitationScheduleList";
        _scheduleGrid.EmptyGridText = WebTextManager.GetText("/pageText/forms/surveys/invitations/controls/schedule.ascx/noActivitiesScheduled");
        _scheduleGrid.RenderCompleteCallback = "renderCompleteScheduleList";
        _scheduleGrid.ItemClickCallback = "onReminderClick";
    }

    protected void _sendReminderBtn_Click(object sender, EventArgs e)
    {
        if (!Checkbox.Messaging.Email.EmailGateway.ProviderSupportsBatches)
            Response.Redirect("SendReminder.aspx?i=" + InvitationId + "&onClose=onDialogClosed");
        else
            Response.Redirect("SendScheduledReminder.aspx?i=" + InvitationId + "&onClose=onDialogClosed");
    }
</script>
