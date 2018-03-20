/*
Universal search control
*/
var search = new Search();

function Search() {
    this.initialized = false;
    this.config = { searchInputId: '', at: '', resultRequestPeriod: 500, minSearchTermLength: 3 };
    this.requestID = '';
    this.prevTerm = '';
    this.term = '';
    this.linkMap = [
        ['Survey', 'Forms/Manage.aspx?term={term}', 'Forms/Manage.aspx?s={ObjectID}'],
        ['Response', 'Forms/Surveys/Responses/Manage.aspx?term={term}', 'Forms/Surveys/Responses/View.aspx?responseGuid={ObjectGUID}'],
        ['EMailList', 'Users/Manage.aspx?m=e&term={term}', 'Users/Manage.aspx?m=e&selected={ObjectID}'],
        ['Group', 'Users/Manage.aspx?m=g&term={term}', 'Users/Manage.aspx?m=g&selected={ObjectID}'],
        ['User', 'Users/Manage.aspx?m=u&term={term}', 'Users/Manage.aspx?m=u&selected={ObjectIDString}'],
        ['Invitation', 'Forms/Surveys/Invitations/Manage.aspx?term={term}', 'Forms/Surveys/Invitations/Manage.aspx?i={ObjectID}'],
        ['Report', 'Forms/Surveys/Reports/Manage.aspx?term={term}', 'Forms/Surveys/Reports/Manage.aspx?reportID={ObjectID}'],
        ['Item', '#', 'Forms/Surveys/Edit.aspx?item={ObjectID}'],
    ];


    //config sample : { needInit: true, at: _at}
    //
    this.initialize = function (config) {
        this.config = config;

        $('#' + this.config.searchInputId).focus(function (e) {
            $('#header_messages').hide();
            if (!search.initialized)
                search.doInit();

            if (search.results != null && search.results.length > 0) {
                search.fixPosition();
                $('#header_search_results').show();
            }
        });

        $('#' + this.config.searchInputId).focusout(function (e) {
            $('#header_search_results').delay(500).hide('slow');
        });

        setInterval(function () {
            var term = $('#' + search.config.searchInputId).val().replace(/'/g, "&#39;");

            if (term !== search.prevTerm) {
                if (term.length >= search.config.minSearchTermLength) {
                    this.lastSearchDate = new Date();
                }
                if (term.indexOf(search.prevTerm) != 0) {
                    //totally new term -- clear the request ID
                    search.requestID = '';
                }
                search.doSearch(term);
                search.prevTerm = term;
            }
        }, 1000);
    };

    this.fixPosition = function() {
        var left = window.innerWidth - $('.search-input-container').offset().left - 200;
        var top = $('.search-input-container').offset().top - 4;
        $('#header_search_results').css('right', left).css('top', top);
    };

    this.doInit = function () {
        if (!this.initialized) {
            search.fixPosition();

            svcSearch.Initialize(this.config.at,
            function (res, searchObj) {
                searchObj.initialized = res;
            },
            this);
        }
    };

    this.lastSearchDate = new Date();

    //avoids javascript injections, replaces html tags
    this.escapeInjections = function(htmlString) {
        return htmlString
                        .replace(/&/g, "&amp;")
                        .replace(/</g, "&lt;")
                        .replace(/>/g, "&gt;")
                        .replace(/"/g, "&quot;")
                        .replace(/'/g, "&#39;");
    }

    this.doSearch = function (term) {
        this.term = term;
        if (term.length >= this.config.minSearchTermLength) {
            $('#universal_search_loading').show();
            //console.log('Search requested ' + this.requestID + ' [' + term + ']');
            svcSearch.Search(this.config.at, search.escapeInjections(term), this.requestID,
                    function (res) {
                        //console.log('Search reсieved ' + search.requestID + ' [' + term + ']');
                        //console.log(res);
                        if (term != search.term) {
                            //console.log('Ignore results, because search.term is ' + search.term);
                            return;
                        }
                        search.requestID = res.RequestID;
                        if (!res.Completed && res.Pending) {
                            var currentTime = new Date();
                            setTimeout(function () {
                                if ((currentTime - lastSearchDate < 15000))
                                    search.doSearch(term);
                                else
                                    $('#universal_search_loading').hide();
                            }, search.config.resultRequestPeriod);
                        }
                        search.render(res);
                    },
            this);
        }
        else {
            $('#header_search_results').hide();
        }
    };

    this.render = function (res) {
        search.fixPosition();

        var results = res.Results;
        if (res.Pending)
            $('#universal_search_loading').show();
        else
            $('#universal_search_loading').hide();

        this.results = results;
        prevType = '';
        $('#header_search_results ul').html('');
        if (results != null) {
            for (i = 0; i < results.length; i++) {
                var linksData = ['', '#', '#'];
                for (j = 0; j < this.linkMap.length; j++) {
                    if (this.linkMap[j][0] == results[i].ObjectType) {
                        linksData = this.linkMap[j];
                        break;
                    }
                }
                if (prevType != results[i].ObjectType) {
                    if (linksData[1] != '#')
                        $('#header_search_results ul').append('<li class="section-header"><a class="view-all" href="' + this.prepareLink(linksData[1], results[i], this.term) + '">View All &raquo;</a>' + results[i].ObjectType + '</li>');
                    else
                        $('#header_search_results ul').append('<li class="section-header">' + results[i].ObjectType + '</li>');
                }
                prevType = results[i].ObjectType;
                $('#header_search_results ul').append('            <li><a href="' + this.prepareLink(linksData[2], results[i], this.term) + '" title="' + results[i].MatchedField + " = " + results[i].MatchedText.replace('"', '') + '">' + results[i].Title + '</a></li>');
            }
        }

        if (results != null && results.length == 0) {
            $('#header_search_results ul').html('<li><a href="#" onclick="return false;">No results have been found</a></li>');
        }

        if ($('#' + this.config.searchInputId).val().length >= this.config.minSearchTermLength) {
            $('#header_search_results').show();
        }
        else {
            $('#header_search_results').hide();
        }
    };

    this.prepareLink = function (tmpl, data, term) {
        if (typeof (data.ObjectID) != 'undefined' && data.ObjectID) {
            tmpl = tmpl.replace("{ObjectID}", data.ObjectID);
        }
        if (typeof (data.ObjectGUID) != 'undefined' && data.ObjectGUID) {
            tmpl = tmpl.replace("{ObjectGUID}", data.ObjectGUID);
        }
        if (typeof (data.ObjectIDString) != 'undefined' && data.ObjectIDString) {
            tmpl = tmpl.replace("{ObjectIDString}", data.ObjectIDString);
        }
        if (typeof (term) != 'undefined' && term) {
            tmpl = tmpl.replace("{term}", term);
        }
        return this.config.appPath + tmpl;
    };
}
