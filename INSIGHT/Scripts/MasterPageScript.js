var processBusy;

$(function () {
//    processBusy = $('<div><div id="processDiv" class="processing"></div>Processing...</div>').appendTo($('body')).dialog({
//        autoOpen: false,
//        modal: true,
//        height: 'auto',
//        width: 'auto',
//        align: 'center',
//        resizable: false,
//        draggable: false,
//        minWidth: 40,
//        minHeight: 35,
//        cache: true,
//        closeOnEscape: true
//    }).ajaxStart(function () {
//        $(this).dialog('open');
//    })
//               .ajaxStop(function () { $(this).dialog('close'); });

//    window.onbeforeunload = function (e) { processBusy.dialog('open'); };
//    $(".ui-dialog-titlebar").hide();

//    if ($("#processDiv").closest('.ui-dialog').is(':visible')) {
//        processBusy.dialog('close');
//    }

    // Searching by pressing ENTER  Key
    var defFunc = function defaultFunc(e) {
         
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            $("#btnsearch").click();
            return false;
        }
        return true;
    };

    $('input:text', '#srchKeyPress').keypress(defFunc);
});

function emailValidate(email) {
    return /^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$/.test(email);
}

function validateEmail(sEmail) {
     
    var filter = "/^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/";

    // var filter = "^[A-Za-z0-9_\-\.]+@@(([A-Za-z0-9\-])+\.)+([A-Za-z\-])+$";
    if (filter.test(sEmail)) {
        return true;
    }
    else {
        ErrMsg("Please type valid Email Address.");
        return false;
    }

}

//function validate(email) {
//    alert(email);

//    var delimiterChars =';' ;
//    alert(delimiterChars);
//    var ToArr = [];
//    var i = 1;
//    ToArr = email.split(delimiterChars);

//    for (i; i <= ToArr.length; i++) {
//         
//        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
//        var address = ToArr[i];
//        if (reg.test(address) == false) 
//        {
//            return false;
//        }
//        else {
//            return true;
//        }
//    }
//}

function validate(email) {
        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        var address = email;
        if (reg.test(address) == false) {
            return false;
        }
        else {
            return true;
        }
   
}