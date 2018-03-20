/*
    Timeline display control
*/
var timeline = new Timeline();


function Timeline() {
    this.STATUS_None = 0;
    this.STATUS_Created = 1;
    this.STATUS_Pending = 2;
    this.STATUS_Succeeded = 3;
    this.STATUS_Error = 4;
    this._interval = 0;
    this._lastTimelineRequestDate = new Date() - 1000 * 300;
    this._lastGraphRequestDate = new Date() - 1000 * 300;
    this._nextSearchSeconds = 1;
    this._appRoot = '';
    this._status = 0;
    this._oldGraphData = null;

    this.initialize = function (config, appRoot) {
        this.config = config;
        this._appRoot = appRoot;
        this._lastGraphRequestDate = new Date() - 1000 * 300;

        if (!this._interval)
        {
            this._interval = setInterval(function(){
                timeline.load.call(timeline);                    
            }, 1000);
        }
    };

    this.load = function (force) {
        if (timeline._status == this.STATUS_Succeeded)
            return;
        var currentTime = new Date();
        if (typeof (force) == 'undefined')
            force = false;

        this.config.force = force;

        if (force || (currentTime - this._lastTimelineRequestDate > 1000 * this._nextSearchSeconds)) {
            this._lastTimelineRequestDate = new Date();
            this._nextSearchSeconds = 3;
            svcTimeline.getTimeline(this.config.at, this.config.manager, this.config.requestId,
                typeof (this.config.parentObjectID) == 'undefined' || this.config.parentObjectID < 0 ? 0 : this.config.parentObjectID,
                this.config.parentObjectType, this.onTimelineRecieved, this);
        }
    };

    this.onTimelineRecieved = function (data) {
        timeline._status = data.Status;
        if (data.RequestID)
            timeline.config.requestId = data.RequestID;
        if (data.Status == timeline.STATUS_Succeeded) {
            timeline._nextSearchSeconds = timeline.config.expiration;
            timeline.prevResults = timeline.results;
            timeline.results = data.Results;
            timeline.dateRecieved = new Date();
            //reload data when expired
            timeline.config.requestId = 0; //force to build a new request
            //display recieved data
            timeline.displayResults(timeline.config.id, data.Results);
            if (typeof (timeline.config.onload) == 'function') {
                timeline.config.onload();
            }
        } else if (data.Status == timeline.STATUS_Error) {
         
            console.log('Timeline error occured. ' + data.Message);
        } else {
            this._nextSearchSeconds = 1;
        }

        var currentTime = new Date();
        if (timeline.config.showGraph) {
            if (typeof (svcResponseData) == 'undefined') {
                console.log('svcResponseData is undefined.');
                return;
            }

            //collect responses for all available surveys
            if (typeof (timeline.config.parentObjectID) == 'undefined' || timeline.config.parentObjectID == null || timeline.config.parentObjectID <= 0 || timeline.config.parentObjectID == '') {
                timeline.config.parentObjectID = -1;
            }

            //if (timeline.config.force || (currentTime - timeline._lastGraphRequestDate > 1000 * (timeline.config.expiration + 1))) {
            timeline._lastGraphRequestDate = new Date();
            svcResponseData.getLifecycleAggregatedResponseDataInDays(_at, timeline.config.parentObjectID, 7, 7, timeline.onLifeCycleRecieved, timeline);
            //}
        }
    };

    this.oldGraphDataValid = function(data)
    {
        if (timeline._oldGraphData == null)
            return false;
        //compare data
        if (timeline._oldGraphData.length != data.length)
            return false;
        for (var i = 0; i < data.length; i++)
        {
            if (data[i].ResultText != timeline._oldGraphData[i].ResultText || 
                data[i].AnswerCount != timeline._oldGraphData[i].AnswerCount)
            return false;
        }
        return true;
    };

    this.onLifeCycleRecieved = function (data) {
        if (data == null) {
            $('#' + timeline.config.graphContainerId).hide();
            return;
        }
        var preparedData = new Array();
        var preparedCategories = new Array();
        var totalAnswerCount = 0;
        for (var i = 0; i < data.AggregateResults.length; i++) {
            totalAnswerCount += data.AggregateResults[i].AnswerCount;
            preparedData.push([data.AggregateResults[i].ResultText, data.AggregateResults[i].AnswerCount]);
            preparedCategories.push(data.AggregateResults[i].ResultText);
        }

        if (!totalAnswerCount) {
            $('#' + timeline.config.graphContainerId).hide();
            return;
        }

        if (timeline.oldGraphDataValid(data.AggregateResults)) {
            $('#' + timeline.config.graphContainerId).show();
            return;
        }
        timeline._oldGraphData = data.AggregateResults;

        //calculate graph width (60% of area)
        var graphWidth = $('.dashPadding').width() * 0.6;

        var chart = new Highcharts.Chart({
            chart: {
                animation: true,
                renderTo: timeline.config.graphContainerId,
                //graph width with padding
                width: graphWidth - graphWidth * 0.05,
                height: 200,
                events: {
                    load: function (a) {
                        $(this.container).css("margin", "0 auto");
                    }
                }
            },
            credits:
            {
                enabled: false
            },
            title: {
                text: textHelper.getTextValue("/controlText/surveyDashboard/statistics", "Survey Responses") + ' (' + data.CompletedResponseCount + ')',
                style: {
                    fontFamily: "proxima-nova"
                }

            },
            xAxis: { categories: preparedCategories },
            yAxis: {
                min: 0,
                title: ''
            },
            tooltip: {
                pointFormat: '',
                formatter: function () {
                    return this.y;
                },
                percentageDecimals: 0
            },
            plotOptions: {
                line: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    },
                    showInLegend: false
                }
            },
            series: [
		        { type: 'line', data: preparedData }
		    ],
            legend: {
                enabled: false
            }

        });
        //make left align for highchart
        $('.highcharts-container').css('margin-left', 0);

        $('#' + timeline.config.graphContainerId).show();
    };

    this.getText = function (Count, Date, EventName, ObjectGUID, ObjectID, Period, UserID, ObjectParentName, Url, ObjectParentID){
        var textTemplate = textHelper.getTextValue("/timeline/event/description/" + EventName.toLowerCase() + "/" + Period);

        //replace date if any
        if (Date) {
            textTemplate = textTemplate.replace("{Date}", dateUtils.extractDate(dateUtils.jsonDateToInvariantDateString(Date))).
            replace("{Time}", dateUtils.extractTime(dateUtils.jsonDateToInvariantDateString(Date)));
        }

        //replace ObjectParentName if any
        if (ObjectParentName && Period==1) {
            textTemplate = textTemplate.replace("{ObjectParentName}", "'" + ObjectParentName + "'");
        }        

        //embed link
        if (Url != null && Url != '#')
        {
            var url = timeline.getURL(timeline._appRoot, Url, ObjectID, ObjectGUID, ObjectParentID, Period, EventName);
            textTemplate = textTemplate.replace("<{LINK}>", "<a href='" + url.replace("'", "''") + "'>");
            textTemplate = textTemplate.replace("</{LINK}>", "</a>");
        }
        else
        {
            textTemplate = textTemplate.replace("<{LINK}>", "");
            textTemplate = textTemplate.replace("</{LINK}>", "");
        }

        return textTemplate.
            replace("{Count}", Count).
            replace("{EventName}", EventName).
            replace("{ObjectGUID}", ObjectGUID).
            replace("{ObjectID}", ObjectID).
            replace("{UserID}", UserID);
    };

    this.pushEvent = function (Period, EventName, FilterEventType) {
        $("#left_column_search_text").val('');
        $('#leftPanel_search').trigger('searchExecuted', ['', FilterEventType, Period, EventName]);
    };

    this.getURL = function (root, url, ObjectID, ObjectGUID, ObjectParentID, Period, EventName) {
        url = url.replace("{ObjectID}", ObjectID).replace("{ObjectGUID}", ObjectGUID).replace("{ObjectParentID}", ObjectParentID).
            replace("{Period}", Period).replace("{EventName}", EventName);
        if (typeof(timeline.config.parentObjectID) != 'undefined' && timeline.config.parentObjectID > 0)
            url = url.replace("{selectedParentObjectID}", timeline.config.parentObjectID);
        else
            url = url.replace("{selectedParentObjectID}", -1);
        return url.indexOf('javascript:') == 0 ? url : root + url;
    };

    this.getLinkText = function (EventName, Period) {
        return textHelper.getTextValue("/timeline/event/link/" + EventName.toLowerCase() + "/" + Period);
    };

    this.displayResults = function (containerId, results) {
        if (typeof (results) == 'undefined' || results == null || results.length == 0) {
            $('#' + containerId).hide();
        }

        //filter by event name if needed
        var l = timeline;
        if (typeof (l.config.visibleEvents) != 'undefined') {
            var filteredResults = new Array();
            for (var i = 0; i < results.length; i++) {
                for (var j = 0; j < l.config.visibleEvents.length; j++) {
                    if (results[i].EventName == l.config.visibleEvents[j]) {
                        filteredResults.push(results[i]);
                        break;
                    }
                }
            }
            results = filteredResults;
            var l = timeline;
            l.results = filteredResults;
        };

        $(results).each(function (index) {
            this.getText = function (Count, Date, EventName, ObjectGUID, ObjectID, Period, UserID, ObjectParentName, Url, ObjectParentID) {
                return timeline.getText(Count, Date, EventName, ObjectGUID, ObjectID, Period, UserID, ObjectParentName, Url, ObjectParentID);
            };
        }
        );

        if (typeof (templateHelper) != 'undefined') {
            templateHelper.loadAndApplyTemplate(
                'timelineListItemTemplate.html',
                timeline._appRoot + '/Forms/jqtmpl/timelineListItemTemplate.html',
                results,
                null,
                containerId,
                true,
                function () {
                    timeline.doInitialPaging(containerId);
                });
        }
    };

    this.doInitialPaging = function (id) {
        var l = timeline;

        if (typeof (l.prevResults) != 'undefined') {
            $('#' + id).children().each(function (idx) {
                var found = false;
                var prevDate = null;
                for (var j = 0; j < l.prevResults.length; j++) {
                    if (l.prevResults[j].EventID == l.results[idx].EventID &&
                        l.prevResults[j].UserID == l.results[idx].UserID &&
                        l.prevResults[j].ObjectID == l.results[idx].ObjectID &&
                        l.prevResults[j].ObjectGUID == l.results[idx].ObjectGUID &&
                        l.prevResults[j].Count == l.results[idx].Count &&
                        l.prevResults[j].Date == l.results[idx].Date &&
                        l.prevResults[j].Period == l.results[idx].Period &&
                        l.prevResults[j].ObjectParentName == l.results[idx].ObjectParentName &&
                        l.prevResults[j].ObjectParentID == l.results[idx].ObjectParentID) {
                        l.results[idx].received = l.prevResults[j].received;
                        prevDate = l.results[idx].received;
                        found = true;
                        break;
                    }
                }

                var now = new Date();
                if (!found || ((prevDate != null) && (typeof (prevDate) != 'undefined')) && (now.getTime() - prevDate.getTime() < 60000 * 3)) {
                    $(this).attr("class", $(this).attr("class") + " new");
                    l.results[idx].received = new Date();
                }

                //hide all other that don't fit into the first page
                if (idx >= l.config.recordsPerPage && !l.expanded)
                    $(this).hide();
            });
        }
        else {
            $('#' + id).children().each(function (idx) {
                //mark 3 top items as new
                if (idx < 3) {
                    l.results[idx].received = new Date();
                    $(this).attr("class", $(this).attr("class") + " new");
                }
                //hide all other that don't fit into the first page
                if (idx >= l.config.recordsPerPage && !l.expanded)
                    $(this).hide();
            });
        };

        if (l.results.length > l.config.recordsPerPage && !l.expanded) {
            $('#' + id + '_show').show();
        }
        else {
            $('#' + id + '_show').hide();
        }
        if (l.results.length > 0) {
            $('#' + id.replace("_list", "_container")).show();
            $('#' + id).show();
        }
        else {
            //$('#' + id.replace("_list", "_container")).hide();
            $('#' + id).hide();
        }
    };

    this.showAll = function (id) {
        var l = timeline;
        l.expanded = true;
        $('#' + id).children().show();
        $('#' + id + '_show').hide();
    };

    this.setParentObject = function (objectId, manager, parentObjectType) {
        timeline._status = 0;
        timeline.config.requestId = 0;
        timeline.config.parentObjectID = objectId;
        timeline.config.parentObjectType = parentObjectType;
        timeline._lastTimelineRequestDate = new Date() - 1000 * 300;
        timeline._lastGraphRequestDate = new Date() - 1000 * 300;
        $('#' + timeline.config.id).html('<img id="ctl00_ctl00__pageContent__leftContent__surveyList__progressSpinner" class="ProgressSpinner" src="../App_Themes/CheckboxTheme/Images/ProgressSpinner.gif" style="height:31px;width:31px;border-width:0px;">');
        $('#' + timeline.config.graphContainerId).hide('');
        timeline.load(true);
    };

    this.loadSurvey = function (ObjectID) {
        if (typeof (loadSurveyData) == 'function') {
            loadSurveyData(ObjectID);
        }
    };

    this.loadUser = function (ObjectID) {
        loadUserData(ObjectID);
        $('.gridContent').removeClass('gridActive');
        $('#user_' + escString(ObjectID)).addClass('gridActive');
    };

    this.loadGroup = function (ObjectID) {
        loadGroupData(ObjectID);
        $('.gridContent').removeClass('gridActive');
        $('#group_' + ObjectID).addClass('gridActive');
    };

    this.loadEmailList = function (ObjectID) {
        loadEmailListPanelData(ObjectID);
        $('.gridContent').removeClass('gridActive');
        $('#emailList_' + ObjectID).addClass('gridActive');
    };

}
