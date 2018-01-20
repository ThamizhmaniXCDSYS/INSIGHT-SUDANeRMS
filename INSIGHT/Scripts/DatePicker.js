
$(function () {
    $(".datepicker").datepicker({
        showOn: "button",
        dateFormat: "dd/mm/yy",
        buttonImage: "../../Images/date.gif",
        buttonImageOnly: true,
        changeMonth: true,
        timeFormat: 'hh:mm:ss',
        autowidth: true,
        changeYear: true
    });

    $(".staffdatepicker").datepicker({
        showOn: "button",
        dateFormat: "dd/mm/yy",
        buttonImage: "../../Images/date.gif",
        buttonImageOnly: true,
        changeMonth: true,
        timeFormat: 'hh:mm:ss',
        autowidth: true,
        changeYear: true
    });

    $(".discontinue_datepicker").datepicker({
        showOn: "button",
        dateFormat: "dd/mm/yy",
        buttonImage: "../../Images/date.gif",
        buttonImageOnly: true,
        changeMonth: true,
        timeFormat: 'hh:mm:ss',
        autowidth: true,
        changeYear: true
    });

    $(".discontinue_datepicker1").datepicker({
        showOn: "button",
        dateFormat: "dd/mm/yy",
        buttonImage: "../../Images/date.gif",
        buttonImageOnly: true,
        changeMonth: true,
        timeFormat: 'hh:mm:ss',
        autowidth: true,
        changeYear: true
    });

    $(".datepickerMDY").datepicker({
        showOn: "button",
        dateFormat: "mm/dd/yy",
        buttonImage: "../../Images/date.gif",
        buttonImageOnly: true,
        changeMonth: true,
       // timeFormat: 'hh:mm:ss',
        autowidth: true,
        changeYear: true,
        maxDate: '+0d'
    });
    $(".datetimepicker").datetimepicker({
        showOn: "button",
        dateFormat: "dd/mm/yy",
        buttonImage: "../../Images/date.gif",
        buttonImageOnly: true,
        changeMonth: true,
        timeFormat: 'hh:mm TT',
        autowidth: true,
        changeYear: true,
        maxDate: '+0d'

    });
    $(".datetimepickerMDY").datetimepicker({
        showOn: "button",
        dateFormat: "mm/dd/yy",
        buttonImage: "../../Images/date.gif",
        buttonImageOnly: true,
        changeMonth: true,
        timeFormat: 'hh:mm:ss TT',
        autowidth: true,
        changeYear: true,
        maxDate: '+0d'
    });
    $(".timepicker").timepicker({       
        changeMonth: true,
        timeFormat: 'hh:mm TT',
        autowidth: true,
        changeYear: true
    });
});