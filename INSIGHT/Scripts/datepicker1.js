$(function () {
    $(".datepicker1").datepicker({
     //showOn: "button",
        //dateFormat: "m/d/yy",
        dateFormat: "d/m/yy",
        //buttonImage: "../../Images/date.gif",
       // buttonImageOnly: true,
        changeMonth: true,
        timeFormat: 'hh:mm:ss',
        autowidth: true,
        changeYear: true
    });
});