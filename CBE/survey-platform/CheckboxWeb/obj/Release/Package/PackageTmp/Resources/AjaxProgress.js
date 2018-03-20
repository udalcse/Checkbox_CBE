
var ajaxProgress = new progressObj();

function progressObj() {
    this.progressKey = '';
    this.progressProviderType = '';
    this.progressBarContainerId = '';
    this.appRootUrl = '';
    this.updateCallback = null;
    this.errorCallback = null;
    this.completeCallback = null;
    this.progressBarOptions = null;
    this.errorThreshold = 1;
    this.errorCount = 0;

    this.spinnerIndex = 0;
    this.spinner = new Array(' .', ' ..', ' ...');

    //Start the progress function
    this.startProgress = function(
        progressKey,
        progressBarContainerId,
        appRootUrl,
        updateCallback,
        errorCallback,
        completeCallback,
        progressProviderType
    ) {

        ajaxProgress.errorCount = 0;
        ajaxProgress.progressKey = progressKey;
        ajaxProgress.progressBarContainerId = progressBarContainerId;
        ajaxProgress.appRootUrl = appRootUrl;
        ajaxProgress.updateCallback = updateCallback;
        ajaxProgress.errorCallback = errorCallback;
        ajaxProgress.completeCallback = completeCallback;
        
        if (typeof progressProviderType != "undefined")
            ajaxProgress.progressProviderType = progressProviderType;
        
        //Start progress
        ajaxProgress.progressBarOptions = {
            steps: 100,
            stepDuration: 1,
            max: 100,
            showText: true,
            textFormat: 'percentage',
            width: 120,
            height: 12,
            boxImage: appRootUrl + 'App_Themes/CheckboxTheme/Images/progressbar.gif',
            barImage:
                {
                    0: appRootUrl + 'App_Themes/CheckboxTheme/Images/progressbg_green.gif'
                }
        };

        if (ajaxProgress.progressBarContainerId != null
            && ajaxProgress.progressBarContainerId != '') {
            //Turn on progress display
            $('#' + progressBarContainerId).progressBar(0, ajaxProgress.progressBarOptions);
        }

        //Start polling
        setTimeout(ajaxProgress.checkProgress, 4000);
    };

    //
    this.getSpinner = function() {
        var retVal = ajaxProgress.spinner[ajaxProgress.spinnerIndex % 3];
        ajaxProgress.spinnerIndex++;

        return retVal;
    };

    //Check progress
    this.checkProgress = function() {
        $.ajax({
                type: "GET",
                url: ajaxProgress.appRootUrl + 'Services/progressreportingservice.svc/json/getprogressstatus',
                async: true,
                cache: false,
                contentType: "application/json; charset=utf-8",
                data: { progressKey: ajaxProgress.progressKey, provider: ajaxProgress.progressProviderType },
                dataType: "json",
                success: function(data) {
                    if (data == null
                        || data.d == null) {
                        ajaxProgress.handleProgressError('NULL data returned by progress status page.');
                        return;
                    }

                    if (data.d.Success == null) {
                        ajaxProgress.handleProgressError('Data returned by progress status page had NULL success.');
                        return;
                    }

                    if (data.d.Success == false) {
                        ajaxProgress.handleProgressError(data.d.ErrorMessage);
                        return;
                    }

                    if (data.d.Success == true) {
                        ajaxProgress.handleProgressSuccess(data.d.ProgressData);
                        return;
                    }
                },
                error: function(xhr, ajaxOptions, thrownError) {
                    ajaxProgress.handleProgressError(xhr.status + ':  ' + thrownError);
                }
            });
    };

    //Handle an error getting progress
    this.handleProgressError = function(errorMessage) {
        if (ajaxProgress.errorCallback != null) {
            ajaxProgress.errorCallback(errorMessage);
        }
    };

    //Handle success getting progress
    this.handleProgressSuccess = function(progressData) {
        if (progressData == null) {
            ajaxProgress.handleProgressError('Progress get call succeeded, but data was null.');
            return;
        }

        if (progressData.Status == null) {
            ajaxProgress.handleProgressError('Progress get call succeeded, but data.Status was null.');
            return;
        }


        //Check for progress error
        // Status Codes:
        //  0 - Pending
        //  1 - Running
        //  2 - Completed
        //  3 - Error
        if (progressData.Status == 'Error') {
            //Check error threshold
            ajaxProgress.errorCount++;

            //If we haven't reached our threshold, try again
            if (ajaxProgress.errorCount < ajaxProgress.errorThreshold) {
                setTimeout(ajaxProgress.checkProgress, 4000);
                return;
            }

            if (progressData.ErrorMessage != null) {
                ajaxProgress.handleProgressError(progressData.ErrorMessage);
            }
            else if (progressData.StatusMessage != null) {
                ajaxProgress.handleProgressError(progressData.StatusMessage);
            }
            else {
                ajaxProgress.handleProgressError('No error specified.');
            }
            return;
        }

        //Check for completion
        if (progressData.Status == 'Completed') {
            ajaxProgress.setProgress(progressData.PercentComplete);
            if (ajaxProgress.completeCallback != null) {
                ajaxProgress.completeCallback(progressData);
            }

            return;
        }

        //Otherwise, call update callback and keep running
        if (ajaxProgress.updateCallback != null) {
            progressData.StatusMessage = progressData.StatusMessage + ajaxProgress.getSpinner();
            ajaxProgress.updateCallback(progressData);
        }

        //Update progress bar
        //Turn on progress display
        ajaxProgress.setProgress(progressData.PercentComplete);

        setTimeout(ajaxProgress.checkProgress, 4000);
    };

    this.setProgress = function(progressPercent) {
        if (ajaxProgress.progressBarContainerId != null
            && ajaxProgress.progressBarContainerId != '') {
            $('#' + ajaxProgress.progressBarContainerId).progressBar(progressPercent, ajaxProgress.progressBarOptions);
        }
    };
}       
