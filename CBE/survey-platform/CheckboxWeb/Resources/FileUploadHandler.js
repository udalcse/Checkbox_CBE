/////////////////////////////////////////////////////////////////////////////
//                   FileUploadHandler.js                                  //
//   Utility methods for working with file upload control.                 //
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
// validateSelectedFileExtension                                           //
//   Validate that selected file has one of the allowed file extensions.   //
/////////////////////////////////////////////////////////////////////////////
function validateSelectedFileExtension(fileUpload, validExtensions) {
    var input = fileUpload.value;

    //Make sure input and validExtension aren't empty
    if (input == null  || validExtensions == null || validExtensions.length == 0) {
        return false;
    }

    var regStr = '(';
    for(i=0; i<validExtensions.length; i++){
        regStr+=validExtensions[i]+"|";
    }
    regStr = regStr.substring(0,regStr.length-1) + ")$";

    var regEx = new RegExp(regStr);

    return regEx.test(input);
}

function submitFile(id, url, toId, loadCompleted) {
    var uFrame = $('<iframe id="usrFrame' + id + '" name="usrFrame' + id + '" src="javascript:false" style="position:absolute;top:-1000px;left:-1000px" uframeignore="true"/>');
    var uForm = $('<form id="usrForm' + id + '" target="usrFrame' + id + '" action="' + url + '" method="POST" enctype="multipart/form-data" style="position:absolute;top:-1000px;left:-1000px" uframeignore="true"></form>');
    var iFile = $('input[type = "file"]');
    $(iFile).appendTo(uForm);
    $(uForm).appendTo(uFrame);
    $(uFrame).appendTo(document.body);

    $(uFrame).bind("load", function () {
        setTimeout(function () {
            $(iFile).remove();
            $(uFrame).remove();
        }, 500);

        if (loadCompleted) {
            loadCompleted();
            return;
        }
    });

    $(uForm).submit();
    return false;
}


function startFileUploading(fileID, postbackButtonID, url) {
    var uFrame = $('<iframe id="usrFrame' + fileID + '" name="usrFrame' + fileID + '" src="javascript:false" style="position:absolute;top:-1000px;left:-1000px" uframeignore="true" onerror="alert(111);"/>');
    var uForm = $('<form id="usrForm' + fileID + '" target="usrFrame' + fileID + '" action="' + url + '" method="POST" enctype="multipart/form-data" style="position:absolute;top:-1000px;left:-1000px" uframeignore="true" onerror=""></form>');
    var iFile = $('#' + fileID);
    $(iFile).appendTo(uForm);
    $(uForm).appendTo(uFrame);
    $(uFrame).appendTo(document.body);

    $(uFrame).bind("load", function () {

        setTimeout(function () {
            $(iFile).remove();
            $(uFrame).remove();
        }, 500);

        $('#' + postbackButtonID).click();
    });

    $(uForm).submit();
    return false;
}