$(function () {
//    if ($("#SubmitSuccessMsg").val() != null & $("#SubmitSuccessMsg").val() != "") {
//        alert($("#SubmitSuccessMsg").val());
//        InfoMsg("Issue created Successfully.\n Issue Number is:" + $("#SubmitSuccessMsg").val(), function () { $("#SubmitSuccessMsg").val(""); });
    //    }
    $("#ddlSearchBy").change(function () {
        if ($("#ddlSearchBy").val() == "") {
            $("#txtSearch").val("");
            $("#txtSearch").attr("disabled", true);
        }
        else {
            $("#txtSearch").attr("disabled", false);
        }
    });
    
});

function statusimage(cellvalue, options, rowObject) {
    var i;
    var cellValueInt = parseInt(cellvalue);
    var cml = $("#StaffManagementList").jqGrid();
    for (i = 0; i < cml.length; i++) {
       
            if (cellValueInt <= 24) {
                return '<img src="../../Images/yellow.jpg" height="10px" width="10px" alt=' + cellvalue + ' title=' + cellvalue + ' />'
            }
            else if (cellValueInt > 24 && cellValueInt <= 48) {
                return '<img src="../../Images/orange.jpg" height="10px" width="10px"  alt=' + cellvalue + ' title=' + cellvalue + ' />'
            }
            else if (cellValueInt > 48) {
                return '<img src="../../Images/redblink3.gif" height="10px" width="10px" alt=' + cellvalue + ' title=' + cellvalue + ' />'
            }
            else if (cellvalue == 'Completed') {
                return '<img src="../../Images/green.jpg" height="12px" width="12px" alt=' + cellvalue + ' title=' + cellvalue + ' />'
            }
    }
}

function ShowComments(ActivityId) {
    modalid = $('#Activities');
    $('#ActivitiesList').clearGridData();
    LoadPopupDynamicaly("/StaffIssues/LoadUserControl/Activities", modalid, function () {
        LoadSetGridParam($('#ActivitiesList'), "/StaffIssues/ActivitiesListJqGrid?Id=" + ActivityId);
    });
}
function LoadSetGridParam(GridId, brUrl) {
    GridId.setGridParam({
        url: brUrl,
        datatype: 'json',
        mtype: 'GET',
        page: 1
    }).trigger("reloadGrid");
}
var clbPupGrdSel = null;
function LoadPopupDynamicaly(dynURL, ModalId, loadCalBack, onSelcalbck, width) {
    if (width == undefined) { width = 800; }
    if (ModalId.html().length == 0) {
        $.ajax({
            url: dynURL,
            type: 'GET',
            async: false,
            dataType: 'html', // <-- to expect an html response
            success: function (data) {
                ModalId.dialog({
                    height: 'auto',
                    width: width,
                    modal: true,
                    title: 'History',
                    buttons: {}
                });
                ModalId.html(data);
            }
        });
    }
    clbPupGrdSel = onSelcalbck;
    ModalId.dialog('open');
    CallBackFunction(loadCalBack);
}