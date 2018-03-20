/////////////////////////////////////////////////////////////////////////////
// RadUploadHandler.js                                                     //
//   Utility methods for working with rad file upload control.             //
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
// validateSelectedFileExtension                                           //
//   Validate that selected file has one of the allowed file extensions.   //
/////////////////////////////////////////////////////////////////////////////
function validateSelectedFileExtension(radUpload, eventArgs)
{
    var input = eventArgs.get_fileInputField();

    //Make sure input could be found
    if (input == null) {
        return false;
    }
    
    //If input is not valid, clear selection and return false
    if (!radUpload.isExtensionValid(input.value)) {
        var inputs = radUpload.getFileInputs();
        
        for (i = 0; i < inputs.length; i++) {
            if (inputs[i] == input) {
                radUpload.clearFileInputAt(i);
                break;
            }
        }

        return false;
    }

    return true;
}

/////////////////////////////////////////////////////////////////////////////
// clearUploadedFiles                                                      //
//   Clear uploaded files in rad control.                                  //
/////////////////////////////////////////////////////////////////////////////
function clearUploadedFiles(radUploadClientId) {
    var radUpload = getRadUpload(radUploadClientId);

    if (radUpload == null) {
        return;
    }

    var inputs = radUpload.getFileInputs();

    for (i = 0; i < inputs.length; i++) {
        if (inputs[i] == input) {
            radUpload.clearFileInputAt(i);
        }
    }
}
