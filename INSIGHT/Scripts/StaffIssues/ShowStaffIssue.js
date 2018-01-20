$(function () {
    $("#txtDescription").attr("readonly", true).css("background-color", "#F5F5F5");
    $("#txtResolution").attr("readonly", true).css("background-color", "#F5F5F5");
    var Id = $("#Id").val();

    jQuery("#DescriptionForSelectedId").jqGrid({

        url: '/StaffIssues/DescriptionForSelectedIdJqGrid?Id=' + $("#Id").val(),
        datatype: 'json',
        mtype: 'GET',
        height: '130',
        width: '1160',
        autowidth: true,
        colNames: ['Commented By', 'Commented On', 'Rejection Comments', 'Resolution Comments'],
        colModel: [
        // { name: 'EntityRefId', index: 'EntityRefId', width: 80, align: 'left'  },
              {name: 'CommentedBy', index: 'UploadedBy', width: 80, align: 'left'  },
              { name: 'CommentedOn', index: 'UploadedOn', width: 140, align: 'left'  },
              { name: 'RejectionComments', index: 'RejectionComments', sortable: false },
              { name: 'ResolutionComments', index: 'ResolutionComments', sortable: false }
              ],
        rowNum: '10',
        rowList: [5, 10, 20, 50, 100, 150, 200 ],
        sortname: '',
        sortorder: "",
        viewrecords: true,
        multiselect: false,
        caption: 'Discussion Forum'
    });


    jQuery("#Uploadedfileslist").jqGrid({
        mtype: 'GET',
        url: '/StaffIssues/StaffDocumentsjqgrid?Id=' + Id,
        datatype: 'json',
        height: '50',
        width: '1250',
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
});

function uploaddat(id) {
    window.location.href = "/StaffIssues/uploaddisplay?Id=" + id;
    processBusy.dialog('close');
}



