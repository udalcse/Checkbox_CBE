﻿/* Inicialización en español para la extensión 'UI date picker' para jQuery. */
/* Traducido por Vester (xvester@gmail.com). */
(function($) {
    $.datepicker.regional['es'] = {
            renderer: $.ui.datepicker.defaultRenderer,
            monthNames: ['Enero','Febrero','Marzo','Abril','Mayo','Junio',
            'Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'],
            monthNamesShort: ['Ene','Feb','Mar','Abr','May','Jun',
            'Jul','Ago','Sep','Oct','Nov','Dic'],
            dayNames: ['Domingo','Lunes','Martes','Mi&eacute;rcoles','Jueves','Viernes','S&aacute;bado'],
            dayNamesShort: ['Dom','Lun','Mar','Mi&eacute;','Juv','Vie','S&aacute;b'],
            dayNamesMin: ['Do','Lu','Ma','Mi','Ju','Vi','S&aacute;'],
            dateFormat: 'dd/mm/yyyy',
            firstDay: 1,
            prevText: '&#x3c;Ant', prevStatus: '',
            prevJumpText: '&#x3c;&#x3c;', prevJumpStatus: '',
            nextText: 'Sig&#x3e;', nextStatus: '',
            nextJumpText: '&#x3e;&#x3e;', nextJumpStatus: '',
            currentText: 'Hoy', currentStatus: '',
            todayText: 'Hoy', todayStatus: '',
            clearText: '-', clearStatus: '',
            closeText: 'Cerrar', closeStatus: '',
            yearStatus: '', monthStatus: '',
            weekText: 'Sm', weekStatus: '',
            dayStatus: 'DD d MM',
            defaultStatus: '',
            isRTL: false
    };
    $.extend($.datepicker.defaults, $.datepicker.regional['es']);

    $.timepicker.regional['es'] = {
        timeOnlyTitle: 'Elegir una hora',
        timeText: 'Hora',
        hourText: 'Horas',
        minuteText: 'Minutos',
        secondText: 'Segundos',
        millisecText: 'Milisegundos',
        timezoneText: 'Huso horario',
        currentText: 'Ahora',
        closeText: 'Cerrar',
        timeFormat: 'hh:mm',
        amNames: ['a.m.', 'AM', 'A'],
        pmNames: ['p.m.', 'PM', 'P'],
        ampm: false,
        isRTL: false
    };
    $.timepicker.setDefaults($.timepicker.regional['es']);
})(jQuery);
