$(document).ready(function () {
    function formateadorLink(cellvalue, options, rowObject) {
        return '<a href=' + ROOT + '/InventoryManagement/PODetailsList?PurchOrderId=' + rowObject[0] + '>' + cellvalue + '</a>';
    }
    $('#POMasterList').jqGrid({
        url: ROOT + '/InventoryManagement/PurchaseOrderListJQGrid',
        datatype: 'Json',
        type: 'GET',
        //shrinkToFit: false,
        height: 245,
        width: 1200,
        colNames: ['PurchOrderId', 'PO Number', 'PO Issued date', 'PO Currency', 'Supplier', 'Supplier No', 'CreatedDate', 'CreatedBy', 'ModifiedDate', 'ModifiedBy', 'RequestId'],
        colModel: [
            { name: 'PurchOrderId', index: 'PurchOrderId', hidden: true },
            { name: 'PO', index: 'PO', formatter: formateadorLink, sortable: true, align: 'center', width: 40 },
            { name: 'POIssuedDate', index: 'POIssuedDate', align: 'center', width: 40 },
            { name: 'POCurrency', index: 'POCurrency', align: 'center', width: 40 },
            { name: 'Supplier', index: 'Supplier', width: 100 },
            { name: 'SupplierNumber', index: 'SupplierNumber', align: 'center', width: 40 },
            { name: 'CreatedDate', index: 'CreatedDate', hidden: true },
            { name: 'CreatedBy', index: 'CreatedBy', hidden: true },
            { name: 'ModifiedDate', index: 'ModifiedDate', hidden: true },
            { name: 'ModifiedBy', index: 'ModifiedBy', hidden: true },
            { name: 'RequestId', index: 'RequestId', hidden: true }
        ],
        multiselect:true,
        rowNum: 1000,
        viewrecords: true,
        rowList: [500, 1000, 2500, 5000],
        sortorder: 'desc',
        sortname: 'PurchOrderId',
        caption: '&nbsp;<i class="icon-th-list"></i>&nbsp;Purchase Order List',
        pager: 'POMasterListPager',
    });
    $("#POMasterList").navGrid('#POMasterListPager', { add: false, edit: false, del: false, search: false, refresh: false });
});
function Search() {
    $("#POMasterList").setGridParam(
        {
            datatype: "json",
            url: ROOT + '/InventoryManagement/PurchaseOrderListJQGrid',
            postData: { PO: $('#txtPONumber').val() },
            page: 0
        }).trigger("reloadGrid");
}
function ResetSearch() {
    $("input[type=text], textarea, select").val("");
    $('#POMasterList').setGridParam({
        datatype: "json",
        url: ROOT + '/InventoryManagement/PurchaseOrderListJQGrid',
        postData: { PO: "" },
        page: 1
    }).trigger("reloadGrid");
}