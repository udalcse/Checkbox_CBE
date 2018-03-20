// JScript File

    //Get a reference to the rad window a dialog is contained in
    function GetRadWindow()   
    {   
        var oWindow = null; 
          
        if (window.RadWindow) 
        {
            oWindow = window.RadWindow; //Will work in Moz in all cases, including clasic dialog   
        }
        else if (window.frameElement.radWindow) 
        {
            oWindow = window.frameElement.radWindow;//IE (and Moz az well)   
        }
        
                  
        return oWindow;   
    }   
    
    //Close a rad window
    function Close()
    {
        GetRadWindow().close();
    }

    //Close a dialog and cause the opener to reload
    function CloseAndReload()   
    {   
        var oWnd = GetRadWindow();   
        
        CloseAndRedirectParent(oWnd.BrowserWindow.location);
    }   
    
    //Close a dialog and sent the parent to the new location
    function CloseAndRedirectParent(newParentUrl)
    {
        var oWnd = GetRadWindow();

        oWnd.BrowserWindow.location = newParentUrl;
        oWnd.close();   
    }
    
   
   //Show the splash screen
    function ShowSplash()   
    {  
        radsplash();   
    }   
    
    //Hide the splash screen
    function HideSplash()   
    {   
        radsplash(false);   
    }   




