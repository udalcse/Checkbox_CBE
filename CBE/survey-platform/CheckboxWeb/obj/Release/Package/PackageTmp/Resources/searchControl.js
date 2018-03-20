

//Instantiate object
var searchControl = new searchControlObj();

///Search control object
function searchControlObj() {
    //Bind search button to controls
    this.bindSearch = function(searchInputId, searchLinkButtonName){
        //Bind key press
        $('#' + searchInputId).keypress(function(e){
            //Check for enter key
            if(e.which == 13) {
                //Submit search button - Have to do some work since search button is actually
                // a link button and there is no way to simulate a link click in jQuery. We'll
                // instead call __doPostback using passed-in name of underlying link button
                                
                __doPostBack(searchLinkButtonName, '');
                return true;
            }
        });
    }
}
