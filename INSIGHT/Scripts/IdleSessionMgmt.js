(function ($) {
    $.fn.extend({
        autologout: function (refreshUrl, logoutUrl, options) {
            options = $.extend({}, $.Autologout.defaults, options);

            this.each(function () {
                new $.Autologout(this, refreshUrl, logoutUrl, options);
                return false;
            });
            return;
        }
    });

    $.Autologout = function (prompt, refreshUrl, logoutUrl, options) {
        
        var logoutTimer = null;
        var sessionTimer = null;
        var dialogWait = Math.max(0, Number(options.sessionDialogWait * 60000));
        var timeout = Math.max(30000, (Number(options.sessionTimeout) * 60000) - dialogWait) - 30000;

        $(prompt).dialog({
            autoOpen: false,
            bgiframe: options.bgiframe,
            modal: true,
            buttons: {
                OK: function () {
                    $(this).dialog('close');
                    $.get(refreshUrl, resetTimers, 'html');
                },
                Cancel: sessionExpired
            },
            open: function () {
                if (options.pageSelector) {
                    var height = $(options.pageSelector).outerHeight();
                    $('.ui-widget-overlay').animate({ 'height': height }, 'fast');
                }
            }
        }).ajaxStart(function () { resetTimers(); });
        resetTimers();
        function resetTimers() {
            if (logoutTimer) clearTimeout(logoutTimer);
            if (sessionTimer) clearTimeout(sessionTimer);
            sessionTimer = setTimeout(sessionExpiring, timeout);
        }
        function sessionExpiring() {
            logoutTimer = setTimeout(sessionExpired, dialogWait);
            $(prompt).dialog('open');
        }
        function sessionExpired() {
            window.location.href = logoutUrl;
        }
    };
    $.Autologout.defaults = {
        sessionTimeout: 1,
        sessionDialogWait: 1.5,
        bgiframe: true,
        pageSelector: null
    };
})(jQuery);