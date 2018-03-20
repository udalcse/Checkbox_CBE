
function FeatureTour() {
    var self = this;

  //  var isHome = window.location.href.indexOf('Forms/Manage.aspx') >= 0;
  //  var hideTour = readCookie('hide-feature-tour') != null;
    var stepsInfo = [];
    stepsInfo.push({
        key: 'welcome',
        position: 'center-center',
        itemSelector: '',
        noArrow: true,
        title: 'Welcome to Checkbox Survey',
        description: 'This product tour will highlight the main areas and features of Checkbox Survey. Click Next to continue or “X” to cancel. Visit our <a target="_blank" href="http://www.checkbox.com/support/overview">support page</a> for more help getting started.'
    });
    stepsInfo.push({
        key: 'add-survey',
        position: 'top-left',
        itemSelector: '#surveymanager_addsurvey',
        title: 'Create a Survey',
        description: 'Click the +Survey button to create your first survey. View our <a href="https://www.checkbox.com/checkbox-6-quick-start-guide-2/" target="_blank">Quick Start Guide</a> if you need help getting started.'
    });
    stepsInfo.push({
        key: 'surveys',
        position: 'top-right',
        itemSelector: '.leftPanel',
        title: 'View & Organize Surveys',
        description: 'The Survey Dashboard lists your surveys and folders, which you can filter using the Search box. Click the edit link to edit your survey and survey settings.'
    });
    stepsInfo.push({
        key: 'users',
        position: 'top-center',
        itemSelector: '#header .headerMenu li:eq(1)',
        title: 'Users',
        description: 'Click the Users link to create and edit both administrative users and respondents. See Checkbox’s <a target="_blank" href="https://www.checkbox.com/checkbox-6-user-roles-guide/">User Role Guide</a> for more information on roles and permissions.'
    });
    stepsInfo.push({
        key: 'styles',
        position: 'top-center',
        itemSelector: '#header .headerMenu li:eq(2)',
        title: 'Styles',
        description: 'Use one of Checkbox’s pre-loaded style templates or create your own using your organization’s branding and colors. See our <a href="https://www.checkbox.com/checkbox-6-style-guide/" target="_blank">Style Guide</a> for more information.'
    });
    stepsInfo.push({
        key: 'libraries',
        position: 'top-center',
        itemSelector: '#header .headerMenu li:eq(3)',
        title: 'Libraries',
        description: 'You can use Checkbox Libraries to save commonly used questions and surveys for later use. Libraries can be private or shared with other users.'
    });
    stepsInfo.push({
        key: 'search',
        position: 'top-center',
        itemSelector: '#header .header-search',
        title: 'Universal Search',
        description: 'Use the Universal Search feature to search for a term across all survey, report, user, group, email list or invitation names.'
    });
    stepsInfo.push({
        key: 'video',
        position: 'center-center',
        noArrow: true,
        itemSelector: '',
        width: 460,
        title: 'More Resources',
        description: 'For more help getting started with Checkbox, view our <a href="http://www.checkbox.com/support/checkbox-documentation" target="_blank">documentation</a>, <a href="http://www.checkbox.com/support/ticket" target="_blank">contact our friendly support team</a>, or arrange for <a href="mailto:sales@checkbox.com">personal training.</a>'
    });
    var defaultStepWidth = 360;
    var container;
    var stepTitle;
    var description;
    var stepContainer;
    var stepIndicators;
    var nextButton;
    var finishButton;
    var closeButton;

    self.step = 0;
    self.numSteps = stepsInfo.length;

    self.goToNext = function() {
        var nextStep = parseInt(self.step) + 1;
        self.goToStep(nextStep);
    };

    self.goToStep = function (idx) {
        var stepInfo = stepsInfo[idx];

        // Special step functionality
        switch (stepInfo.key) {
        case 'details':
            $('#listPlace .groupContent:first').click();
            break;
        }

        // Set content
        stepTitle.html(stepInfo.title);
        description.html(stepInfo.description);

        // Position tour window
        var stepWidth = (typeof stepInfo.width != 'undefined') ? stepInfo.width : defaultStepWidth;
        container.css('width', stepWidth + 'px');
        var hideArrow = (typeof stepInfo.noArrow != 'undefined' && stepInfo.noArrow == true) ? true : false;
        var stepPos = stepInfo.position;
        var posSplit = stepPos.split('-');
        var yPos = posSplit[0];
        var xPos = posSplit[1];
        container.attr('class', 'feature-tour-wrapper');
        container.addClass('step-' + stepInfo.key);
        if (!hideArrow) {
            container.addClass('arrow-' + stepPos);
        }

        var tourTop = 0;
        var tourLeft = 0;
        if (stepInfo.itemSelector != '') {
            var $stepPageItem = $(stepInfo.itemSelector);
            var itemPos = $stepPageItem.offset();
            tourLeft = itemPos.left;
            if (xPos == 'center') {
                tourLeft += (($stepPageItem.width() / 2) - (stepWidth / 2));
            } else if (xPos == 'right') {
                tourLeft += $stepPageItem.width();
            }
            tourTop = itemPos.top;
            if (typeof stepInfo.extraTopOffset != 'undefined') {
                tourTop += stepInfo.extraTopOffset;
            }
            if (yPos == 'top') {
                tourTop += 20 + parseInt($stepPageItem.css('padding-top'));
            }
        } else { // center it
            tourTop = 200; // (($(window).height() / 2) - (container.height() / 2));
            tourLeft = (($(window).width() / 2) - (stepWidth / 2));
        }
        container.css({ left: tourLeft + 'px', top: tourTop + 'px' });

        // Highlight newly-active step's indicator
        stepIndicators.removeClass('active');
        stepIndicators.eq(idx).addClass('active');
        self.step = idx;

        // Toggle next & finish buttons
        if ((parseInt(self.step) + 1) == parseInt(self.numSteps)) { // last step
            nextButton.fadeOut('fast', function() {
                finishButton.fadeIn('fast');
            });
        } else {
            finishButton.fadeOut('fast', function() {
                nextButton.fadeIn('fast');
            });
        }
    };

    function finishTour() {
        container.fadeOut('fast');

        // Set cookie so tour isn't shown on page load anymore
        createCookie('hide-feature-tour', '1');
    };

    self.launchTour = function() {
        finishButton = stepContainer.find('.finish');
        finishButton.hide();

        container.fadeIn();

        self.step = 0;
        self.goToStep(0);
    };

    self.initFeatureTour = function() {
        var tourHTML = '<div id="feature_tour" class="feature-tour-wrapper" style="display: none;"><div class="feature-tour">';
        tourHTML += '<div class="arrow"></div><a href="#" class="close"></a>';
        tourHTML += '<h4 class="step-title">Title</h4><p class="step-description">Description</p>';
        tourHTML += '<div class="steps"><a class="next ckbxButton blueButton" href="#">Next</a><a class="finish ckbxButton blueButton" href="#">Finish</a><ul>';
        for (var i = 0; i < self.numSteps; i++) {
            tourHTML += '<li><a href="#" data-step-index="' + i + '">' + (i + 1) + '</a></li>';
        }
        tourHTML += '</ul></div>';
        tourHTML += '</div></div>';
        $('body').append(tourHTML);
        container = $('#feature_tour');
        defaultStepWidth = container.width();
        stepTitle = container.find('.step-title');
        description = container.find('.step-description');
        stepContainer = container.find('.steps');
        stepIndicators = stepContainer.find('li');
        nextButton = stepContainer.find('.next');
        finishButton = stepContainer.find('.finish');
        closeButton = container.find('.close');

        nextButton.click(function(e) {
            e.preventDefault();
            self.goToNext();
        });
        finishButton.add(closeButton).click(function(e) {
            e.preventDefault();
            finishTour();
        });
        stepIndicators.children('a').click(function(e) {
            e.preventDefault();
            var idx = $(this).attr('data-step-index');
            self.goToStep(idx);
        });

        // Add "show tour" button if this is the homepage
        /*if (isHome) {
            $('<a id="show_feature_tour" class="show-feature-tour" href="#">?</a>').insertBefore('#header .settings-gear');
            $('#show_feature_tour').click(function (e) {
                self.launchTour();
            });
        }

        // If this is the homepage, and the user hasn't already designated 
        // to hide the tour, launch it immediately
        if (isHome && !hideTour) {
            self.launchTour();
        }*/
    };
}
