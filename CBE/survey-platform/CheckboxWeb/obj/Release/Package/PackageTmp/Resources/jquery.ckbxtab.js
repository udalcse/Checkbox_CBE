(function ($) {
    var methods = {
        //Hide tab
        hideTab: function (tabIndex) {
            var $this = $(this);

            var theTab = $this.children('li:eq(' + tabIndex + ')');

            if (theTab.length == 0) {
                theTab = $this.children('ul').children('li:eq(' + tabIndex + ')');
            }

            theTab.hide();
        },

        showTab: function(tabIndex) {
            var $this = $(this);

            var theTab = $this.children('li:eq(' + tabIndex + ')');

            if (theTab.length == 0) {
                theTab = $this.children('ul').children('li:eq(' + tabIndex + ')');
            }

            theTab.show();
        },

        deSelect: function () {
            $(this).children('li').removeClass('active');
        },

        //Init tabs
        ckbxTabs: function (options) {

            //Supported setting default values
            var settings = {
                tabName: 'tabs',
                tabStyle: 'default',
                tabContentContainer: '',
                initialTabIndex: 0,
                onTabClick: null,
                onTabsLoaded: null
            };

            return this.each(function () {
                try {
                    //Merge options with settings
                    if (options) {
                        $.extend(settings, options);
                    }

                    var $this = $(this);

                    //Get tab style
                    var tabStyle = 'default';
                    var activeTabClass = 'active';

                    if (typeof (settings.tabStyle) != 'undefined') {
                        tabStyle = settings.tabStyle.toLowerCase();
                    }

                  //  var containerCssClass = 'tabsMenu';

                    //Update page structure depending on tab type
                    if (tabStyle == 'inverted') {
                        activeTabClass = 'current';
                        var tabContainer = $('<div class="invertedshiftdown"></div>');

                        //Insert new div
                        tabContainer.insertBefore($this);

                        //Move current UL into div
                        $this.appendTo(tabContainer);
                    }
                    else {
                        //Class for horizontal tabs
                        $this.addClass('tabsMenu');
                    }

                    //Select children
                    var tabs = $this.children('li');

                    //Add links to list
                    tabs.each(function (index) {
                        var anchor = $('<a uframeIgnore="true" href="javascript:void(0);">' + $(this).html() + '</a>');
                        $(this).empty();//.html('');
                        anchor.data('tabIndex', index);

                        //Set active class
                        if (index == settings.initialTabIndex) {
                            $('#' + settings.tabName + '-' + index + '-tabContent').show();
                            $(this).addClass(activeTabClass);
                        }
                        else {
                            $('#' + settings.tabName + '-' + index + '-tabContent').hide();
                            $(this).removeClass(activeTabClass);
                        }

                        //Ensure click not bound
                        anchor.unbind('click');

                        //Bind click
                        anchor.bind('click', function () {
                            tabs.each(function (index) {
                                $('#' + settings.tabName + '-' + index + '-tabContent').hide();
                                $(this).removeClass(activeTabClass);
                            });
                            $('#' + settings.tabName + '-' + $(this).data('tabIndex') + '-tabContent').show();
                            $(this).parent().addClass(activeTabClass);

                            if (settings.onTabClick != null) {
                                settings.onTabClick(index);
                            }
                        });

                        //Append
                        $(this).append(anchor);
                    });

                    if (settings.onTabsLoaded != null) {
                        settings.onTabsLoaded();
                    }
                }
                catch (err) {
                    alert('jquery.ckbxttab.js error.');
                }
            });
        }
    }

    $.fn.ckbxTabs = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.ckbxTabs.apply(this, arguments);
        }
    };
})(jQuery);