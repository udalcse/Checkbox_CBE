/****************************************************************************
 * searchResults.js                                                         *
 * Base client methods for search results page.                             *
 ****************************************************************************/
  
//Object definition
function searchResultsObj() {
    this.resultsContainers = new Array();
    this.gridLoadHandlers = new Array();
    this.noResultsContainer = '';
    this.loadingContainer = '';
    this.searchTermContainer = '';
    this.searchMethod = null;
    this.term;
    this.storedResults = new Object();
    this.resultsContainerPrefix = '';
    this.resultsLoadedCallback = null;

    /////////////////////////////////////////////////////////////////////////
    //Initialize
    this.initialize = function (resultsContainers, gridLoadHandlers, noResultsContainer, loadingContainer, searchTermContainer, searchMethod, resultContainerPrefix) {
        this.resultsContainers = resultsContainers;
        this.gridLoadHandlers = gridLoadHandlers;
        this.noResultsContainer = noResultsContainer;
        this.loadingContainer = loadingContainer;
        this.searchTermContainer = searchTermContainer;
        this.searchMethod = searchMethod;
        this.resultsContainerPrefix = resultContainerPrefix;
    }

    this.truncString = function (src, len) {
        if (src == null || typeof (src) == 'undefined')
            return src;
        var words, res, i;
        res = '';
        words = src.split(' ');
        for (i = 0; i < words.length; i++) {
            if (typeof (words[i]) != 'undefined') {
                if (words[i].length > len) {
                    words[i] = words[i].substr(0, len - 1) + '&hellip;';
                }
                res = res + ' ' + words[i];
            }
        }

        return res;
    }

    /////////////////////////////////////////////////////////////////////////
    //Start search process
    this.startSearch = function (searchTerm, onResultsLoadedCallback) {
        this.resultsLoadedCallback = onResultsLoadedCallback;

        if (searchTerm == null || searchTerm.replace(' ', '') == '') {
            return;
        }

        if (this.searchMethod == null) {
            alert('Unable to search.  Search method is null');
            return;
        }

        //Hide various containers
        $('#' + this.noResultsContainer).hide();

        for (var i = 0; i < this.resultsContainers.length; i++) {
            $('#' + this.resultsContainers[i]).hide();
        }

        //Set title
        $('#' + this.searchTermContainer).html(this.truncString(searchTerm, 30));

        //Store term
        this.term = searchTerm;

        //Starting loading dialogs
        $('#' + this.loadingContainer).show();

        //Reset results
        this.storedResults = new Object();

        //Call load method
        this.searchMethod(_at, searchTerm, this.onResultsLoaded, this);
    }    

    /////////////////////////////////////////////////////////////////////////
    // Handle loaded results
    this.onResultsLoaded = function (results, searchResultHelper) {
        var noResults = true;
        var resultCount = 0;

        for (var i = 0; i < results.length; i++) {
            //Store results
            searchResultHelper.storedResults[results[i].GroupKey] = results[i].GroupResults;
            resultCount += results[i].GroupResults.length;

            //Show/hide results containers
            if (results[i].GroupResults.length > 0) {
                noResults = false;
                $('#' + searchResultHelper.resultsContainerPrefix + results[i].GroupKey + 'Container').show();
            }
            else {
                $('#' + searchResultHelper.resultsContainerPrefix + results[i].GroupKey + 'Container').hide();
            }
        }

        //Show/hide empty results message
        if (noResults) {
            $('#' + searchResultHelper.noResultsContainer).show();
        }
        else {
            $('#' + searchResultHelper.noResultsContainer).hide();
        }

        //Call grid load handlers
        for (var i = 0; i < searchResultHelper.gridLoadHandlers.length; i++) {
            searchResultHelper.gridLoadHandlers[i](true);
        }

        //Hide loading container
        $('#' + searchResultHelper.loadingContainer).hide();

        //Callback
        if (searchResultHelper.resultsLoadedCallback != null) {
            searchResultHelper.resultsLoadedCallback(resultCount);
        }
    }

    /////////////////////////////////////////////////////////////////////////
    // Get stored results
    this.getStoredResults = function (resultsKey, currentPage, pageSize, callback, callbackArgs) {
        var resultArray = this.storedResults[resultsKey];

        var result = {};
        result.TotalItemCount = 0;
        result.ResultPage = new Array();

        if (resultArray != null) {
            result.TotalItemCount = resultArray.length;
            result.ResultPage = this.getResultsPage(resultArray, currentPage, pageSize);
        };

        if (callback != null) {
            callback(result, callbackArgs);
        }
    }

    /////////////////////////////////////////////////////////////////////////
    // Get page of search results
    this.getResultsPage = function (itemArray, pageNumber, pageSize) {
        if (pageNumber <= 0 || pageSize <= 0) {
            return itemArray;
        }

        var start = (pageNumber - 1) * pageSize
        var end = start + pageSize;

        if (start > itemArray.length - 1) {
            return new Array();
        }

        if (end > itemArray.length - 1) {
            end = itemArray.length;
        }

        if (start == 0 && end == 0) {
            return [itemArray[0]];
        }

        return itemArray.slice(start, end);
    }
}
