$(document).ready(function () {
    var RequestId = $('#RequestId').val();
    var PORequestId = $('#PORequest').val();

    $('#dvLoading').hide() // hide it initially.
.ajaxStart(function () {

    $(this).show(); // show on any Ajax event.
})

 .ajaxStop(function () {
     $(this).hide(); // hide it when it is done.
 });
    function formateadorLink(cellvalue, options, rowObject) {
        return '<a href=' + ROOT + '/Orders/BulkOrderUploadDetails?RequestId=' + rowObject[0] + '>' + cellvalue + '</a>';
    }


    jQuery("#ReceiptsReportBulkUploadRequestJQGrid").jqGrid({
        mtype: 'GET',
        url: ROOT + '/Orders/OrdersBulkUploadRequestJQGrid?Category=RECEIPTSREPORTUPLOAD',
        datatype: 'json',
        height: '205',
        width: '1200',
        shrinkToFit: true,
        colNames: ['RequestId', 'RequestName', 'RequestNo', 'Category', 'Status', 'CreatedBy', 'CreatedDate', 'ModifiedBy', 'UploadStatus'],
        colModel: [
                  { name: 'RequestId', index: 'RequestId', key: true, hidden: true },
                  { name: 'RequestName', index: 'RequestName' },
                  { name: 'RequestNo', index: 'RequestNo', formatter: formateadorLink, sortable: true, width: 50 },
                  { name: 'Category', index: 'Category', sortable: true, width: 70 },
                  { name: 'Status', index: 'Status', width: 50, hidden: true },
                  { name: 'CreatedBy', index: 'CreatedBy', width: 70 },
                  { name: 'CreatedDate', index: 'CreatedDate', width: 50 },
                  { name: 'ModifiedBy', index: 'ModifiedBy', width: 50, hidden: true },
                  { name: 'UploadStatus', index: 'UploadStatus' }
        ],
        pager: '#ReceiptsReportBulkUploadRequestJQGridPager',
        rowNum: '1000',
        rowList: [100, 200, 300, 400],
        sortname: 'RequestId',
        sortorder: "Desc",
        viewrecords: true,
        caption: '&nbsp;<i class="icon-th-list"></i>&nbsp;Receipts Report Request List',
        loadComplete: function () {
            var ids = jQuery("#ReceiptsReportBulkUploadRequestJQGrid").jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                rowData = jQuery("#ReceiptsReportBulkUploadRequestJQGrid").jqGrid('getRowData', ids[i]);
                if (rowData.UploadStatus == "Completed Successfully") {
                    $("#ReceiptsReportBulkUploadRequestJQGrid").setCell(ids[i], "UploadStatus", "", { "color": "#1A7F03", 'font-weight': 'bold' });
                }
                if (rowData.UploadStatus == "InProgress") {
                    $("#ReceiptsReportBulkUploadRequestJQGrid").setCell(ids[i], "UploadStatus", "", { "color": "#3104B4", 'font-weight': 'bold' });
                }
                if (rowData.UploadStatus == "Partially Completed") {
                    $("#ReceiptsReportBulkUploadRequestJQGrid").setCell(ids[i], "UploadStatus", "", { "color": "#FF8000", 'font-weight': 'bold' });
                }
                if (rowData.UploadStatus == "Completed with Errors") {
                    $("#ReceiptsReportBulkUploadRequestJQGrid").setCell(ids[i], "UploadStatus", "", { "color": "#FF0000", 'font-weight': 'bold' });
                }
                if (rowData.UploadStatus == "Alread Exist") {
                    $("#ReceiptsReportBulkUploadRequestJQGrid").setCell(ids[i], "UploadStatus", "", { "color": "#A0522D", 'font-weight': 'bold' });
                }
            }
        }
    });

    if ($('#txtRequestName').val() == "") {
        $('#lnkupload').hide();
        $('#test').hide();
    }
    $('#dvLoading').hide()
    $('#file_upload').uploadify({
        'auto': false,
        'method': 'post',
        'formData': { 'RequestId': RequestId, 'Sector': PORequestId },
        'swf': ROOT + '/Content/BootStrap/uploadify.swf',
        ////this is where the file posts when it uploads.
        'uploader': ROOT + '/InventoryManagement/ReceiptsReportBulkUpload',

        //To send the RequestNo for the post method
        'onUploadSuccess': function (file, data, response) {
            //data is whatever you return from the server
            //we're sending the URL from the server so we append this as an image to the #uploaded div
            $('#dvLoading').show();
            $("#uploaded").append("<img src='" + data + "' alt=''/>");

            $('#txtRequestName').val('');
            $('#lnkupload').hide();
        },
        'onQueueComplete': function (queueData) {
            InfoMsg(queueData.uploadsSuccessful + ' files were successfully uploaded.');
            $('#test').hide();
            $('#dvLoading').hide();
            $.ajax({
                url: ROOT + '/InventoryManagement/callReceiptReportParallel?RequestId=' + RequestId ,
                type: 'POST',
                dataType: 'json',
                traditional: true,
                success: function (data) {
                    window.location.href = ROOT + '/InventoryManagement/ReceiptsReportUploadRequestCreation';
                },
                loadError: function (xhr, status, error) {
                    msgError = $.parseJSON(xhr.responseText).Message;
                    ErrMsg(msgError, function () { });
                },
            });
        }
    });
    FillPORequest();
});
function FillPORequest() {
    $.getJSON(ROOT + '/InventoryManagement/FillPORequest/',
function (modelData) {
    var select = $("#PORequest");
    select.empty();
    select.append($('<option/>',
{
    value: "",
    text: "Select PO Request"
}));
    $.each(modelData, function (index, itemData) {

        select.append($('<option/>',
{
    value: itemData.Value,
    text: itemData.Text
}));
    });
});
}
function Validation() {
    if ($('#PORequest').val() == "") {
        ErrMsg("Select PO Request");
        return false;
    }
    if ($('#txtRequestName').val() == "") {
        ErrMsg("Request Name can not be empty");
        return false;
    }
}