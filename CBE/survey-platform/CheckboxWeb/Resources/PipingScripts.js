/****************************************************************************
 * Attach click, keyup and select events for rad editor to store selection  *
 ****************************************************************************/
function AttachRadEditorEvents(targetControlId)
{
    var targetControl = window[targetControlId];
    
    if(targetControl)
    {
        targetControl.AttachEventHandler('onClick', function(e){StoreCursorPosition(targetControl)});
        targetControl.AttachEventHandler('onSelect', function(e){StoreCursorPosition(targetControl)});
        targetControl.AttachEventHandler('onKeyUp', function(e){StoreCursorPosition(targetControl)});
    }
}

/****************************************************************************
 * Method to store the cursor position of a text input.                     *
 ****************************************************************************/
function StoreCursorPosition(targetControl)
{
    if(targetControl.Document && targetControl.Document.selection)
    {
        targetControl.cursorPos = targetControl.Document.selection.createRange().duplicate();
    }
    else if (targetControl.createTextRange) 
    {
        targetControl.cursorPos = document.selection.createRange().duplicate();
    }
}

/****************************************************************************
 * Update the target control and set the pipe text at the current cursor    *
 *  position.  Requires rad window scripts to be included in calling page.  *
 ****************************************************************************/
function UpdatePipeTargetAndClose(targetControlId, pipeText)
{
    //Find the target controls
    var oWnd = GetRadWindow();

    //Check for rad editor, which is actually an iframe so it is in the 
    // windows collection of the parent.
    var targetControl = oWnd.BrowserWindow[targetControlId];
        
    //Check for a text box
    if(targetControl == null)
    {
        targetControl = oWnd.BrowserWindow.document.getElementById(targetControlId);
    }
    
    //Now attempt to insert the text
    if(targetControl != null)
    {
        //Call method on opener, which may store cursor position
        oWnd.BrowserWindow.InsertAtCursor(targetControl, pipeText);
    }
        
    //Close the callling window
    oWnd.Close();   
}

/****************************************************************************
* Update the target control and set the pipe text at the current cursor    *
*  position.                                                               *
****************************************************************************/
function UpdatePipeTarget(targetControlId, pipeText) {

    //Check for rad editor, which is actually an iframe so it is in the 
    // windows collection of the parent.
    var targetControl = window[targetControlId];

    //Check for a text box
    if (targetControl == null) {
        targetControl = window.document.getElementById(targetControlId);
    }

    //Now attempt to insert the text
    if (targetControl != null) {
        //Call method on opener, which may store cursor position
        window.InsertAtCursor(targetControl, pipeText);
    }
}
/****************************************************************************
 * Insert text at a given point in a text field.                            *
 
 ****************************************************************************/
function InsertAtCursor(targetField, textValue)
{
    //Rad Editor
    if(targetField.Document)
    {
        var selection;
        
        //Try to use stored cursor position
        if(targetField.cursorPos)
        {
            selection = targetField.cursorPos;
        }
        else if(targetField.Document.selection)
        {
            selection = targetField.Document.selection.createRange();
        }
        else
        {
            targetField.PasteHtml(textValue);
        }
        
        if(selection)
        {
            selection.text = ' ' + textValue;
            
            //IE automatically creates links out of things with @ in them (like email 
            // addresses or pipes) so give IE the unlink command.
            //Commenting out since it doesn't seem to work.
            //targetField.Document.execCommand('Unlink', false, null);
            
            selection.moveStart('character', -textValue.length);
            selection.select();
        }
    }
    //IE Text box
    else if (document.selection) 
    {
        var selection;
        
        //Try to use stored cursor position
        if(targetField.cursorPos)
        {
            selection = targetField.cursorPos;
        }
        else
        {
            targetField.focus();
            selection = document.selection.createRange();
        }
        
        selection.text = ' ' + textValue;
        selection.moveStart('character', -textValue.length);
        selection.select();
    }
    //Mozilla
    else if (targetField.selectionStart || targetField.selectionStart == '0') 
    {
        targetField.focus();
        var startPos = targetField.selectionStart;
        var endPos = targetField.selectionEnd;
        
        targetField.value = targetField.value.substring(0, startPos) + ' ' + textValue + targetField.value.substring(endPos, targetField.value.length);

        targetField.selectionStart = startPos + 1;
        targetField.selectionEnd = startPos + textValue.length + 1;
    } 
    else 
    {
        //Otherwise, insert text at the end
        targetField.value += ' ' + textValue;
    }
}