$(function () {
    var Id = $("#Id").val();
    $("#Save").click(function () {
        ValidateIssueDescription();
        if (ValidateIssueDescription() == false) {
            ErrMsg("Issue Description should not exceed 4000 characters.");
            return false;
        }
    });

    $("#CompleteLogIssue").click(function () {
        if (ValidateIssueDescription() == false) {
            ErrMsg("Issue Description should not exceed 4000 characters.");
            return false;
        }
        var issgrp = $("#ddlIssueGroup").val();
        var isstyp = $("#ddlIssueType").val();
        var issdesc = $("#txtDescription").val();
        if (issgrp == "") {
            ErrMsg("Issue Group is Mandatory.");
            return false;
        }
        else if (isstyp == "") {
            ErrMsg("Issue Type is Mandatory.");
            return false;
        }
        else if (issdesc == "") {
            ErrMsg("Issue Description is Mandatory.");
            return false;
        }
        else {
            return true;
        }
    });
    $("#CommentList").jqGrid({
        url: '/StaffIssues/DescriptionForSelectedIdJqGrid?Id=' + $('#Id').val(),
        datatype: 'json',
        mtype: 'GET',
        colNames: ['Commented By', 'Commented On', 'Rejection Comments', 'Resolution Comments'],
        colModel: [
        // { name: 'Issue Number', index: 'EntityRefId', sortable: false },
              {name: 'CommentedBy', index: 'CommentedBy', sortable: false },
              { name: 'CommentedOn', index: 'CommentedOn', sortable: false },
              { name: 'RejectionComments', index: 'RejectionComments', sortable: false },
              { name: 'ResolutionComments', index: 'ResolutionComments', sortable: false }
             ],
        rowNum: -1,
        //width: 1160,
        autowidth: true,
        height: 150,
        sortname: 'EntityRefId',
        sortorder: "desc",
        viewrecords: true,
        caption: 'Discussion Forum'
    });


    jQuery("#Uploadedfileslist").jqGrid({
        mtype: 'GET',
        url: '/StaffIssues/StaffDocumentsjqgrid?Id=' + Id,
        datatype: 'json',
        height: '50',
        width: '650',
        shrinkToFit: true,
        colNames: ['Uploaded By', 'Uploaded On', 'File Name'],
        colModel: [
                          { name: 'UploadedBy', index: 'UploadedBy', width: '30%', align: 'left', sortable: false },
                          { name: 'UploadedOn', index: 'UploadedOn', width: '30%', align: 'left', sortable: false },
                          { name: 'FileName', index: 'FileName', width: '30%', align: 'left', sortable: false }
                          ],
        pager: '#uploadedfilesgridpager',
        rowNum: '10',
        rowList: [5, 10, 20, 50, 100, 150, 200 ],
        //           sortname: 'IssueNumber',
        //           sortorder: "Desc",
        multiselect: true,
        viewrecords: true,
        caption: 'Uploaded Documents'
    });

    $(".flip").click(function () {
         
        var icon = $('.icon', this);
        $(".panel").slideToggle("slow");
        icon.attr("src", this.attr("src") == up ? down : up);
    });

    $("#file1").click(function () {
        var Id = $("#Id").val();
        if (Id == 0) {
            ErrMsg("Please create Issue.");
            return false;
        }
    });

    $('#btnIdRejectIssue').click(function () {
        return rejectionValidation("Please enter the comments to reject.");
    });
    $('#btnReply').click(function () {
        return rejectionValidation("Please enter the comments to reply.");
    });
    $('#btnIdResolveIssue').click(function () {
        if ($('#txtResolution').val() == null || $('#txtResolution').val() == "") {
            ErrMsg("Please enter the Resolution Comments", function () { $('#txtResolution').focus(); });
            return false;
        } else {
            return true;
        }
        return false;
    });

});

function uploaddat(id) {
    window.location.href = "/StaffIssues/uploaddisplay?Id=" + id;
    processBusy.dialog('close');
}

function rejectionValidation(msg) {

    if ($('#txtRejCommentsArea').val() == null || $('#txtRejCommentsArea').val() == "") {
        ErrMsg(msg, function () { $('#txtRejCommentsArea').focus(); });
        $('#txtRejCommentsArea').attr("readonly", false).css("background-color", "white");
        return false;
    } else {
        return true;
    }
    return false;
}

function ValidateIssueDescription() {
    var Issdesc = $("#txtDescription").val();
    // alert(Issdesc.length);
    if (Issdesc.length > 4000)
        return false;
}


