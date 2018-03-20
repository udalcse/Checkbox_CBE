/*
Source adapted from: http://www.webappers.com/progressBar/
*/

Type.registerNamespace('Checkbox.Web.Ajax.Controls.ProgressBehavior');

Checkbox.Web.Ajax.Controls.ProgressBehavior = function(element) {

    //  the DOM element whose background image
    //  contains the indicator image
    this._indicator = null;
    this._mode = null;
    //  keep track of the current percentage.
    //  when the continuous animation is running,
    //  this value will be -1
    this._percentage = null;

    //  Members used by the continuous animation
    //  resizing secquence
    this._sequenceAnimation = null;
    //  manually change the percentage
    this._percentageAnimation = null;
    //  handle the animation started event
    this._percentAnimationStartedHandler = null;
    //  handle the animation ended event
    this._percentAnimationEndedHandler = null;
    //  handle the percent animation property changed event
    this._percentAnimationPropertyChangedHandler = null;

    //  Members used by the percentage animation
    //  not running
    this._isPercentQueueRunning = null;
    //  create the empty request queue
    this._percentQueue = null;
    //  the DOM element that contains the percent status message
    this._info = null;

    //Message to prepend to the progress status
    this._message = '';

    Checkbox.Web.Ajax.Controls.ProgressBehavior.initializeBase(this, [element]);
}

Checkbox.Web.Ajax.Controls.ProgressBehavior.prototype = {
    initialize: function() {
        Checkbox.Web.Ajax.Controls.ProgressBehavior.callBaseMethod(this, 'initialize');

        //  get the DOM element that contains the indicator
        this._indicator = $get(this.get_id() + '_indicator');
        this._info = $get(this.get_id() + '_info');

        if (this.get_Mode() == Checkbox.Web.Ajax.Controls.ProgressBarMode.Manual) {
            this._percentQueue = new Array();
            this._isPercentQueueRunning = false;

            //  create the animation that is used to change the percentage values
            this._percentageAnimation = new $AA.LengthAnimation(this._indicator, null, 50, 'style', 'width', null, null, '%');

            //  attach
            this._percentAnimationEndedHandler = Function.createDelegate(this, this._onPercentAnimationEnd);
            this._percentageAnimation.add_ended(this._percentAnimationEndedHandler);
            //  attach
            this._percentAnimationStartedHandler = Function.createDelegate(this, this._onPercentAnimationStart);
            this._percentageAnimation.add_started(this._percentAnimationStartedHandler);
            //  attach
            this._percentAnimationPropertyChangedHandler = Function.createDelegate(this, this._onPercentAnimationPropertyChanged);
            this._percentageAnimation.add_propertyChanged(this._percentAnimationPropertyChangedHandler);

            //  set the percentage
            this.set_percentage(0);
        }

        if (this.get_Mode() == Checkbox.Web.Ajax.Controls.ProgressBarMode.Continuous) {
            //  create the resize animation
            var resizeAnimation = new $AA.LengthAnimation(this._indicator, 1, 50, 'style', 'width', 0, 100, '%');
            var noOpAnimation = new $AA.ScriptAction(null, .1, 25, '');
            //  setup the sequence
            this._sequenceAnimation = new $AA.SequenceAnimation(null, null, null, [resizeAnimation, noOpAnimation], -1);

            this._percentage = -1;
            this._info.style.display = 'none';
        }
    },

    dispose: function() {
        Checkbox.Web.Ajax.Controls.ProgressBehavior.callBaseMethod(this, 'dispose');
    },

    show: function() {
        this.get_element().style.display = '';
    },

    hide: function() {
        this.get_element().style.display = 'none';
    },

    play: function() {
        if (this.get_Mode() == Checkbox.Web.Ajax.Controls.ProgressBarMode.Continuous) {
            //  kick off the animation        
            this._sequenceAnimation.play();
        }
    },

    stop: function() {
        if (this.get_Mode() == Checkbox.Web.Ajax.Controls.ProgressBarMode.Continuous) {
            //  kick off the animation        
            this._sequenceAnimation.stop();
        }
    },

    set_percentage: function(percentage) {

    //DEBUG
    alert("set_percentage");
    
        if (this.get_Mode() == Checkbox.Web.Ajax.Controls.ProgressBarMode.Manual) {

            
            
            
            //  add the request to the queue
            this._percentQueue.push(percentage);

            //  if we are not already processing,
            //  start processing the queue
            if (!this._isPercentQueueRunning) {
                this._processPercentQueue();
            }
        }
    },

    set_message: function(message) {
        this._message = message;
    },

    _processPercentQueue: function() {
      
        //  if there are items in the queue start
        //  processing them
        if (this._percentQueue.length > 0) {
            //  update the status to running
            this._isPercentQueueRunning = true;

            var fromPercent = this._percentage;
            var toPercent = this._percentQueue[0];

            // define the new percentage
            if ((toPercent.toString().substring(0, 1) == "+") || (toPercent.toString().substring(0, 1) == "-")) {
                toPercent = fromPercent + parseInt(toPercent);
            }

            //  make sure we don't go above or below
            //  the 0 - 100 range
            if (toPercent < 0) {
                toPercent = 0;
            }
            if (toPercent > 100) {
                toPercent = 100;
            }

            //  keep the actual value in sync        
            this._percentQueue[0] = toPercent;

            //  determine how long the animation should run
            var duration = 0.1;

            //  update the animation values
            this._percentageAnimation.set_duration(duration);
            this._percentageAnimation.set_startValue(fromPercent);
            this._percentageAnimation.set_endValue(toPercent);

            //  kick off the animation
            this._percentageAnimation.play();
        }
    },

    _onPercentAnimationStart: function() {
        //  add the updating class
        Sys.UI.DomElement.addCssClass(this.get_element(), 'updating');
    },

    _onPercentAnimationEnd: function() {
        //  remove the updating class
        Sys.UI.DomElement.removeCssClass(this.get_element(), 'updating');

        this._updatePercentage(this._percentQueue[0]);
        // remove the entry from the queue
        this._percentQueue.splice(0, 1);
        //  we are not running any more
        this._isPercentQueueRunning = false;
        //  process any other items in the queue
        this._processPercentQueue();
    },

    _onPercentAnimationPropertyChanged: function(sender, args) {
        if (args.get_propertyName() == 'percentComplete') {
            //  get the width of the element, thats the percentage
            var width = sender.get_target().style.width;
            if (width != '') {
                this._updatePercentage($common.parseUnit(width).size);
            }
        }
    },

    _updatePercentage: function(value) {
        this._percentage = parseInt(value);
        this._info.innerText = this._message + ' ' + value + '%';
        this.get_element().title = this._info.innerText;
    },

    get_Mode: function() {
        return this._mode;
    },
    set_Mode: function(value) {
        if (this._mode != value) {
            this._mode = value;
            this.raisePropertyChanged('Mode');
        }
    }
}
Checkbox.Web.Ajax.Controls.ProgressBehavior.registerClass('Checkbox.Web.Ajax.Controls.ProgressBehavior', Sys.UI.Control);

Checkbox.Web.Ajax.Controls.ProgressBarMode = function() {
    throw Error.invalidOperation();
}
Checkbox.Web.Ajax.Controls.ProgressBarMode.prototype = {
    Manual: 0,
    Continuous: 1
}
Checkbox.Web.Ajax.Controls.ProgressBarMode.registerEnum('Checkbox.Web.Ajax.Controls.ProgressBarMode');