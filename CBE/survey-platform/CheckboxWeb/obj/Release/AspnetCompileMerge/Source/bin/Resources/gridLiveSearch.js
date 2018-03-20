function LiveSearchObj(liveSearchDivId, filterName) {
    var _self = this;

    var _timeout;

    var _filterName = filterName;

    var _container = $('#' + liveSearchDivId);
    var _applyBtn = _container.find('.liveSearch-applyBtn');
    var _cancelBtn = _container.find('.liveSearch-cancelBtn');
    var _loadIcon = _container.find('.liveSearch-loadIcon');
    var _textInput = _container.find('input[type="text"]');

    //private member
    var init = function () {
        _cancelBtn.hide();
        _textInput.unbind().keyup(function (e) {
            _self.doSearch();
        });

        //bind search button
        _applyBtn.unbind().click(function (event) {
            event.preventDefault();

            if (_textInput.val() != '')
                _self.doSearch();
        });

        //bind cancel button
        _cancelBtn.unbind().click(function (event) {
            event.preventDefault();

            _textInput.val('').blur();
            _self.doSearch();
            
            $(this).fadeOut('fast');
        });
        
        _textInput.on('focus', function () {
            _container.addClass('active');
        });

        _textInput.on('blur', function () {
            _container.removeClass('active');
            if ($.trim($(this).val()) != '' && !_loadIcon.is(':visible')) {
                if ($(this).hasClass('init')) { // initial - don't animate
                    $(this).removeClass('init');
                    _cancelBtn.show();
                }
                else {
                    _applyBtn.fadeOut('fast', function () {
                        _cancelBtn.fadeIn('fast');
                    });
                }
            }
        });
    };

    //public member
    this.doSearch = function () {
        var filter = _textInput.val();

        //if there is load icon, show it
        if (_loadIcon.length) {
            _applyBtn.hide();
            _cancelBtn.hide();
            _loadIcon.hide().fadeIn('fast');
        } else {
            showButton(filter);
        }

        clearTimeout(_timeout);
        _timeout = setTimeout(function () {
            _textInput.trigger('searchExecuted', [filter, _filterName]);
            
            //hide load icon and show right button
            if (_loadIcon.length) {
                _loadIcon.fadeOut('fast', function () {
                    showButton(filter);
                });
            } else {
                showButton(filter);
            }
        }, 500);
    };

    //public member
    this.changeFilterName = function(newFilterName) {
        _filterName = newFilterName;
    };

    //private member
    var showButton = function (filter) {
        if (filter == '') {
            _applyBtn.show();
            _cancelBtn.hide();
        } else {
            _cancelBtn.show();
            _applyBtn.hide();
        }
    };

    //init in constructor
    init();
}