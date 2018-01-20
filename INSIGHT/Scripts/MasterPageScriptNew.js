function toProperCase(str) {
    return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
}

function isEmptyorNull(myText) { //typeof(myText) === "undefined"
    var re = /^\s{1,}$/g; //match any white space including space, tab, form-feed, etc.
    if (myText == null || myText.length == 0 || ((myText.search(re)) > -1)) { return true; }
    else { return false; }
}

function vallidateRequiredFields(requiredFieldsArray, alertMsg, ignoreDisabled, ignoreVisible) {
    $('span:contains(*):visible').hide();
    var Msg = false;
    for (var i = 0; i < requiredFieldsArray.length; i++) {
        var currentFiled = $('#' + requiredFieldsArray[i]);
        if (currentFiled.length > 0 && (ignoreVisible || currentFiled.is(":visible") || currentFiled.attr("type").toLowerCase() == "hidden") && (!((currentFiled.attr('disabled'))
                                || currentFiled.attr('readonly')) || ignoreDisabled) && isEmptyorNull(currentFiled.val()))
        { Msg = true; $('#S' + requiredFieldsArray[i]).show(); } else { $('#S' + requiredFieldsArray[i]).hide(); }
    }
    if (Msg) { if (alertMsg && alertMsg.length > 0) { ErrMsg(alertMsg) }; return false; } else { return true; }
}

function OpenPopUpWindow(PopUpURL, PopWindowName) {
    var popupWindow = window.open(PopUpURL, PopWindowName, 'height=500,width=1100,left=50,top=150,resizable=no,scrollbars=yes,toolbar=no,menubar=no,location=no,directories=yes,addressbar=yes,status=no')
    if (PopWindowName != undefined && !isEmptyorNull(PopWindowName)) { PopWindowName.focus(); }
    return false;
}

$(function () {
    var processBusy = $('<div><div class="processing"></div></div>')
    $('body').append(processBusy);
    window.onbeforeunload = function (e) { processBusy.dialog('open'); };
    processBusy.dialog({
        autoOpen: false,
        modal: true,
        height: 'auto',
        width: 'auto',
        resizable: false,
        draggable: false,
        minWidth: 100,
        minHeight: 30,
        closeOnEscape: false
    }).ajaxStart(function () { $(this).dialog('open'); }).ajaxStop(function () { $(this).dialog('close'); });
    $(".ui-dialog-titlebar").hide();

    $("span.spnCollapse").click(function () {
        var content = $(this).parent().parent().parent().next();
        if (content.is(':visible')) {
            content.hide();
            try { $(content[0].firstChild).hide(); } catch (err) { }
            $(this).addClass("spnExpand").removeClass("spnCollapse");
        }
        else {
            content.show();
            try { $(content[0].firstChild).show(); } catch (err) { }
            $(this).addClass("spnCollapse").removeClass("spnExpand");
        }
    });
    $("input:submit, a, button").button();
    $("input:text[readonly!='false'][readonly!='']").keydown(function (e) {
        var evt = e || window.event;
        if (evt) {
            var keyCode = evt.charCode || evt.keyCode;
            if (keyCode === 8) {
                if (evt.preventDefault) {
                    evt.preventDefault();
                } else {
                    evt.returnValue = false;
                }
            }
        }
    });
    $('#DDLTheme').change(function () {
        $('#uicss').attr('href', $(this).val());
        $.cookie("css", $(this).val(), { expires: 365, path: '/' });
    });
    if ($.cookie("css")) {
        $('#DDLTheme').val($.cookie("css")).change();
    }
    $("#btnSignOut").click(function () {
        {
            $.ajax({
                async: false,
                type: 'POST',
                url: '/Base/SignOut',
                dataType: "json"
            });
            try {
                document.execCommand("ClearAuthenticationCache");
            }
            catch (e) { }
            window.close();
        }
    });

    $.fn.extend(
        {
            outerHtml: function () {
                if ($(this).attr('outerHTML'))
                    return $(this).attr('outerHTML');
                else {
                    var content = $(this).wrap('<div></div>').parent().html();
                    $(this).unwrap();
                    return content;
                }
            }
        });
    setInterval("det_time()", 1000);
});

function det_time() {
    var now = new Date();
    var dateString = now.getDate() + "-" + (now.getMonth() + 1) + "-" + now.getFullYear() + " "
                        + now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();
    $("#IdDateTime").html(dateString);
}

$.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        // CAUTION: Needed to parenthesize options.path and options.domain
        // in the following expressions, otherwise they evaluate to undefined
        // in the packed version for some reason...
        var path = options.path ? '; path=' + (options.path) : '';
        var domain = options.domain ? '; domain=' + (options.domain) : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = $.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};

function emailValidate(email) {
    return /^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$/.test(email);
}
