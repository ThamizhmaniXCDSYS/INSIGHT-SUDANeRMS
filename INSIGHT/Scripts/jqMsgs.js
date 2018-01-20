(function ($) {
    $.msgs = {
        okBtn: ' OK ',
        okCalBck: null,
        dlgAlrt: null,
        icnSpan: '<span class="ui-icon" style="float:left; margin:0 7px 50px 1em;"></span>',
        errIcn: "ui-icon-circle-close",
        infoIcn: "ui-icon-info",
        sucsIcn: "ui-icon-circle-check",
        info: function (message, calBck, title) {
            if (title == null) title = 'Information';
            this.show(message, calBck, title);
            this.dlgAlrt.find('div').removeClass('ui-state-error').addClass('ui-state-highlight');
            this.dlgAlrt.find('span').addClass(this.infoIcn);
        },
        error: function (message, calBck, title) {
            if (title == null) title = 'Error';
            this.show(message, calBck, title);
            this.dlgAlrt.find('div').removeClass('ui-state-highlight').addClass('ui-state-error');
            this.dlgAlrt.find('span').addClass(this.errIcn);
        },
        success: function (message, calBck, title) {
            if (title == null) title = 'Success';
            this.show(message, calBck, title);
            this.dlgAlrt.find('div').removeClass('ui-state-highlight').addClass('ui-icon-check');
            this.dlgAlrt.find('span').addClass(this.sucsIcn);
        },
        _init: function (dtitle) {
            this.dlgAlrt = $('<div id="alrtdialog"><div class="ui-corner-all"><p id="alrtMsg"></p></div></div>');
            $("BODY").append(this.dlgAlrt);
            this.dlgAlrt.dialog({
                modal: true,
                title: dtitle,
                widht: 'auto',
                resizable: true,
                close: function (event, ui) { $.msgs._clbf(); },
                buttons:
                    [{
                        text: this.okBtn,
                        click: function () { $(this).dialog("close"); $.msgs._clbf(); }
                    }]
            });
        },
        _clbf: function () {
            var clbf = $.msgs.okCalBck; if (clbf != undefined && clbf) { clbf(); }
        },
        show: function (message, calBck, title) {
            if (calBck) { this.okCalBck = calBck; }

            if (!this.dlgAlrt) {
                this._init(title);
            }
            else {
                this.dlgAlrt.dialog("option", "title", title);
                this.dlgAlrt.dialog("open");
            }
            $("#alrtMsg").html($.msgs.icnSpan + message);
        }
    };

    // Shortuct functions
    InfoMsg = function (message, callback, title) { $.msgs.info(message, callback, title); };

    ErrMsg = function (message, callback, title) { $.msgs.error(message, callback, title); };

    SucessMsg = function (message, callback, title) { $.msgs.success(message, callback, title); };

    MsgcalBack = function (clb) { $.msgs.okCalBck = clb; };

})(jQuery);
