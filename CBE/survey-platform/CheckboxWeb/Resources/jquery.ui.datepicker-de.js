/* German initialisation for the jQuery UI date picker plugin. */
/* Written by Milian Wolff (mail@milianw.de). */
jQuery(function($){
	$.datepicker.regional['de'] = {
		closeText: 'schließen',
		prevText: '&#x3c;zurück',
		nextText: 'Vor&#x3e;',
		currentText: 'heute',
		monthNames: ['Januar','Februar','März','April','Mai','Juni',
		'Juli','August','September','Oktober','November','Dezember'],
		monthNamesShort: ['Jan','Feb','Mär','Apr','Mai','Jun',
		'Jul','Aug','Sep','Okt','Nov','Dez'],
		dayNames: ['Sonntag','Montag','Dienstag','Mittwoch','Donnerstag','Freitag','Samstag'],
		dayNamesShort: ['So','Mo','Di','Mi','Do','Fr','Sa'],
		dayNamesMin: ['So','Mo','Di','Mi','Do','Fr','Sa'],
		weekHeader: 'Wo',
		dateFormat: 'dd.mm.yy',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ''};
	$.datepicker.setDefaults($.datepicker.regional['de']);

	$.timepicker.regional['de'] = {
	    timeOnlyTitle: 'Zeit Wählen',
	    timeText: 'Zeit',
	    hourText: 'Stunde',
	    minuteText: 'Minute',
	    secondText: 'Sekunde',
	    millisecText: 'Millisekunde',
	    timezoneText: 'Zeitzone',
	    currentText: 'Jetzt',
	    closeText: 'Fertig',
	    timeFormat: 'hh:mm',
	    amNames: ['vorm.', 'AM', 'A'],
	    pmNames: ['nachm.', 'PM', 'P'],
	    ampm: false,
	    isRTL: false
	};
	$.timepicker.setDefaults($.timepicker.regional['de']);
});
