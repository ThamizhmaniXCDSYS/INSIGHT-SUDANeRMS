/// <reference path="../jquery-1.5.1.js" />
String.prototype.trim = function () {
    return this.replace(/^\s*/, "").replace(/\s*$/, "");
}

function isEmptyorNull(myText) { //typeof(myText) === "undefined"
    var re = /^\s{1,}$/g; //match any white space including space, tab, form-feed, etc.
    if (myText == null || myText.length == 0 || ((myText.search(re)) > -1)) { return true; }
    else { return false; }
}

function loadUserControl(Url, UserControlId) {
    $(".popup").dialog("destroy").remove();
    UserControlId.html("");
    $.ajax({
        url: Url,
        type: 'GET',
        dataType: 'html',
        success: function (data) {
            UserControlId.html(data);
        }
    });
}

function CallBackFunction(CallBackfn) {
    if (CallBackfn != undefined && CallBackfn) { CallBackfn(); }
}

function returnplanText(textwtTag) {
    var plnTxt = "", html = textwtTag;
    var div = document.createElement("div");
    div.innerHTML = html;
    if (div.innerText) { plnTxt = div.innerText; } else { plnTxt = div.textContent; }
    return plnTxt;
}


function LoadSetGridParam(GridId, brUrl) {
     
    GridId.setGridParam({
        url: brUrl,
        datatype: 'json',
        mtype: 'GET'
    }).trigger("reloadGrid");
}
function BindingValonBlur(BlurTxtId, blrUrl, postData, srchFlag, callBackSucessFn, callBackErrorFn) {
    if (BlurTxtId.val() == "" || srchFlag) {
        return false;
    }
    $.ajax({
        url: blrUrl,
        type: 'GET',
        dataType: 'json',
        data: postData,
        success: function (data) {
            CallBackFunction(callBackSucessFn(data));
        },
        error: function (xhr, status, error) {
            msgError = $.parseJSON(xhr.responseText).Message;
            if (msgError == 'More Results found!') { CallBackFunction(callBackErrorFn()); } else {
                ErrMsg(msgError, function () { BlurTxtId.val(""); BlurTxtId.focus(); });
            }
        }
    });
}

//Assses 360 module related code date 13-jan-2013 //
// version: beta
// created: 2005-08-30
// updated: 2005-08-31
// mredkj.com
// site name mredkj.com/tutorials/validate2.html
function extractNumber(obj, decimalPlaces, allowNegative) {
    var temp = obj.value;

    // avoid changing things if already formatted correctly
    var reg0Str = '[0-9]*';
    if (decimalPlaces > 0) {
        reg0Str += '\\.?[0-9]{0,' + decimalPlaces + '}';
    } else if (decimalPlaces < 0) {
        reg0Str += '\\.?[0-9]*';
    }
    reg0Str = allowNegative ? '^-?' + reg0Str : '^' + reg0Str;
    reg0Str = reg0Str + '$';
    var reg0 = new RegExp(reg0Str);
    if (reg0.test(temp)) return true;

    // first replace all non numbers
    var reg1Str = '[^0-9' + (decimalPlaces != 0 ? '.' : '') + (allowNegative ? '-' : '') + ']';
    var reg1 = new RegExp(reg1Str, 'g');
    temp = temp.replace(reg1, '');

    if (allowNegative) {
        // replace extra negative
        var hasNegative = temp.length > 0 && temp.charAt(0) == '-';
        var reg2 = /-/g;
        temp = temp.replace(reg2, '');
        if (hasNegative) temp = '-' + temp;
    }

    if (decimalPlaces != 0) {
        var reg3 = /\./g;
        var reg3Array = reg3.exec(temp);
        if (reg3Array != null) {
            // keep only first occurrence of .
            //  and the number of places specified by decimalPlaces or the entire string if decimalPlaces < 0
            var reg3Right = temp.substring(reg3Array.index + reg3Array[0].length);
            reg3Right = reg3Right.replace(reg3, '');
            reg3Right = decimalPlaces > 0 ? reg3Right.substring(0, decimalPlaces) : reg3Right;
            temp = temp.substring(0, reg3Array.index) + '.' + reg3Right;
        }
    }

    obj.value = temp;
}
function blockNonNumbers(obj, e, allowDecimal, allowNegative) {
    var key;
    var isCtrl = false;
    var keychar;
    var reg;

    if (window.event) {
        key = e.keyCode;
        isCtrl = window.event.ctrlKey
    }
    else if (e.which) {
        key = e.which;
        isCtrl = e.ctrlKey;
    }

    if (isNaN(key)) return true;

    keychar = String.fromCharCode(key);

    // check for backspace or delete, or if Ctrl was pressed
    if (key == 8 || isCtrl) {
        return true;
    }

    reg = /\d/;
    var isFirstN = allowNegative ? keychar == '-' && obj.value.indexOf('-') == -1 : false;
    var isFirstD = allowDecimal ? keychar == '.' && obj.value.indexOf('.') == -1 : false;

    return isFirstN || isFirstD || reg.test(keychar);
}
function showYesOrNo(cellvalue, options, rowObject) {
    return (cellvalue == 'True') ? 'Yes' : 'No';
}

function InitStudentGrid() {
    $("#StudentList").jqGrid({
        datatype: 'local',
        colNames: ['Id', 'Id No', 'Name', 'Section', 'Campus Name', 'Grade', 'Academic Year', 'Is Hosteller'],
        colModel: [
              { name: 'Id', index: 'Id', key: true, hidden: true },
              { name: 'IdNo', index: 'IdNo' },
              { name: 'Name', index: 'Name', width: 150 },
              { name: 'Section', index: 'Section' },
              { name: 'Campus', index: 'Campus' },
              { name: 'Grade', index: 'Grade' },
              { name: 'AcademicYear', index: 'AcademicYear', sortable: false },
              { name: 'IsHosteller', index: 'IsHosteller', formatter: showYesOrNo }
              ],
        pager: $("#StudentPager"),
        rowNum: 10,
        rowList: [10, 15, 20, 50],
        sortname: 'Name',
        sortorder: 'asc',
        viewrecords: true,
        height: 'auto',
        shrinkToFit: true,
        width: $("#StudentList").parent().width(), //430,
        onSelectRow: function (rowid, status) {
            rowData = $('#StudentList').getRowData(rowid);
            if (clbPupGrdSel != undefined && clbPupGrdSel) { clbPupGrdSel(rowData); }
            clbPupGrdSel = null;
            $('#DivStudentSearch').dialog('close');
        }
    });
}
function InitStaffMasterGrid() {
    $("#StaffMasterList").jqGrid({
        datatype: 'local',
        colNames: ['Id', 'Date of Joined', 'Grade', 'Staff Name', 'Subject Teaching'],
        colModel: [
              { name: 'Id', index: 'Id', key: true, hidden: true },
              { name: 'DateJoined', index: 'DateJoined', hidden: true },
              { name: 'Grade', index: 'Grade', width: 150, hidden: true },
              { name: 'StaffName', index: 'StaffName' },
              { name: 'SubjectTeaching', index: 'SubjectTeaching', hidden: true }
              ],
        pager: $("#StaffMasterPager"),
        rowNum: 10,
        rowList: [10, 15, 20, 50],
        sortname: 'StaffName',
        sortorder: 'asc',
        viewrecords: true,
        height: 'auto',
        shrinkToFit: true,
        width: 380,//$("#StaffMasterList").parent().width(), //430,
        onSelectRow: function (rowid, status) {
            rowData = $('#StaffMasterList').getRowData(rowid);
            if (clbPupGrdSel != undefined && clbPupGrdSel) { clbPupGrdSel(rowData); }
            clbPupGrdSel = null;
            $('#DivStaffMasterSearch').dialog('close');
        }
    });
}
//Assess 360 module related code date 13-jan-2013 //

var clbPupGrdSel = null;
function LoadPopupDynamicaly(dynURL, ModalId, loadCalBack, onSelcalbck, width, height,title) {
    if (width == undefined) { width = 800; }
    if (height == undefined) { height = 800; }
    if (title == undefined) { title = "List" }
    // if (ModalId.html().length == 0) {
    $.ajax({
        url: dynURL,
        type: 'GET',
        async: false,
        dataType: 'html', // <-- to expect an html response
        success: function (data) {
            ModalId.dialog({
                width: width,
                height: height,
                position: 'absolute',
                modal: true,
                title:title,
                buttons: {}
            });
            ModalId.html(data);
        }
    });
    // }
    clbPupGrdSel = onSelcalbck;
    ModalId.dialog('open');
    CallBackFunction(loadCalBack);
}

//Added by kingston for validating integer and decimal values (includes ".,-")
function AllowNumericValue(evt) {
    $("#" + evt.id).keyup(function (event) {
        
        if ($.inArray(event.keyCode, [45, 46, 8, 9, 27, 13, 190, 173,110]) !== -1 ||
(event.keyCode == 65 && event.ctrlKey === true) ||
(event.keyCode >= 35 && event.keyCode <= 39)) {
            return;
        }

        else {
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
                ErrMsg('Enter the numeric value');
                // var test = $("#" + evt.id).val();
                // test = test.slice(0, -1);
                $("#" + evt.id).val('');
            }
        }
    });
    return true;
}

//Convert the default datetime value into dd-mm-yyyy format
//example: convert 2014-03-27 7:25:50 PM ==> 27-03-2014
var actualdt;
var dt;
function Dateonly(date) {
    debugger;
    actualdt = "";
    var stdt = date.substring(0, 10);
    var stdtarray = stdt.split('-');
    if (stdtarray.length < 2) {
        stdtarray = stdt.split('/');
        actualdt = stdtarray[1] + '/' + stdtarray[0] + '/' + stdtarray[2];
    }
    else
        actualdt = stdtarray[2] + '-' + stdtarray[1] + '-' + stdtarray[0];

    return actualdt;

}
