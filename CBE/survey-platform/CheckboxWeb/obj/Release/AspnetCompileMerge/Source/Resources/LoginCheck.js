/////////////////////////////////////////////////////////////////////////////
// LoginCheck.js                                                           //
//    Ensures login page is not shown in rad window dialog.  Relies on     //
//    functions in DialogHandler.js                                        //
/////////////////////////////////////////////////////////////////////////////

//Run script on start
$(document).ready(ensureLoginInMainWindow);

var _applicationRoot = null;

//Store application root path
function setApplicationRoot(applicationRoot) {
    _applicationRoot = applicationRoot;
}

//Ensure this page is show in main window.  If not, redirect to login w/reference
// to parent page.
function ensureLoginInMainWindow() {
    if (top != self) {
        redirectParentPage(_applicationRoot + '/Login.aspx?returnUrl=' + UrlEncode(window.parent.location.href));
    }
    
    if (typeof (UFrameManager) == 'undefined') {
        return;
    }

    //UFrame
    if (UFrameManager
        && UFrameManager._uFrames
        && _frameContainer
        && UFrameManager._uFrames[_frameContainer]) {

        window.location.href = _applicationRoot + '/Login.aspx?returnUrl=' + UrlEncode(window.location.href);
    }      
}

///UrlEncode
function UrlEncode(c){var o='';var x=0;c=c.toString();var r=/(^[a-zA-Z0-9_.]*)/;
  while(x<c.length){var m=r.exec(c.substr(x));
    if(m!=null && m.length>1 && m[1]!=''){o+=m[1];x+=m[1].length;
    }else{if(c[x]==' ')o+='+';else{var d=c.charCodeAt(x);var h=d.toString(16);
    o+='%'+(h.length<2?'0':'')+h.toUpperCase();}x++;}}return o;} 
