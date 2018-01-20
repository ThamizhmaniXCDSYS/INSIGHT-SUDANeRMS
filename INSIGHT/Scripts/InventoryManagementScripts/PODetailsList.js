$(document).ready(function () {
    var PurchOrderId = $('#PurchOrderId').val();
    $('#PODetailsList').jqGrid({
        url: ROOT + '/InventoryManagement/POItemDetailsListJQGrid?PurchOrderId=' + PurchOrderId,
        datatype: 'Json',
        type: 'GET',
        shrinkToFit: false,
        height: 245,
        width: 1200,
        colNames: ['POLineId', 'PurchOrderId', 'UNCode', 'Commodity', 'OrderQty', 'POUnitPrice', 'POValue', 'InvoicedQty', 'RemainingQty', 'CreatedDate', 'CreatedBy', 'ModifiedDate', 'ModifiedBy'],
        colModel: [
            { name: 'POLineId', index: 'POLineId', hidden: true, key: true },
            { name: 'Inventory_PurchaseOrder.PurchOrderId', index: 'Inventory_PurchaseOrder.PurchOrderId', hidden: true },
            { name: 'UNCode', index: 'UNCode', align: 'center' },
            { name: 'Commodity', index: 'Commodity',width:300 },
            { name: 'OrderQty', index: 'OrderQty' ,width:90},
            { name: 'POUnitPrice', index: 'POUnitPrice', align: 'center', width: 90 },
            { name: 'POValue', index: 'POValue', align: 'center', width: 90 },
            { name: 'InvoicedQty', index: 'InvoicedQty', width:90},
            { name: 'RemainingQty', index: 'RemainingQty', align: 'right', width: 90 },
            { name: 'CreatedDate', index: 'CreatedDate',  width: 90 },
            { name: 'CreatedBy', index: 'CreatedBy', width: 90 },
            { name: 'ModifiedDate', index: 'ModifiedDate', hidden: true },
            { name: 'ModifiedBy', index: 'ModifiedBy', hidden: true },
        ],
        multiselect:true,
        rowNum: 1000,
        viewrecords: true,
        rowList: [1000, 2500, 5000],
        sortorder: 'Desc',
        sortname: 'POLineId',
        caption: '&nbsp;<i class="icon-th-list"></i>&nbsp;PO Details List',
        pager: 'PODetailsListPager',
    });
    $("#PODetailsList").navGrid('#PODetailsListPager', { add: false, edit: false, del: false, search: false, refresh: false });
});
function Search() {
    $("#PODetailsList").setGridParam(
        {
            datatype: "json",
            url: ROOT + '/InventoryManagement/PODetailsListJQGrid',
            postData: { PONumber: $('#txtPONumber').val(), POStatus: $('#txtPOStatus').val(), SupplierNo: $('#txtSupplierNo').val(), ContainerNo: $('#txtContainerNo').val(), InvoiceNo: $('#txtInvoiceNo').val(), OracleVoucherNo: $('#txtOracleVoucherNo').val() },
            page: 0
        }).trigger("reloadGrid");
}
function ResetSearch() {
    $("input[type=text], textarea, select").val("");
    $('#PODetailsList').setGridParam({
        datatype: "json",
        url: ROOT + '/InventoryManagement/PODetailsListJQGrid',
        postData: { PONumber: "", POStatus: "", SupplierNo: "", ContainerNo: "", InvoiceNo: "", OracleVoucherNo: "" },
        page: 1
    }).trigger("reloadGrid");
}
function BackToList() {
    window.location.href = ROOT + '/InventoryManagement/PurchaseOrder';
}