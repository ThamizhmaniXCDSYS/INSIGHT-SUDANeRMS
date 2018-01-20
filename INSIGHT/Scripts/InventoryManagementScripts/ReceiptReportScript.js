$(document).ready(function () {
    $('#ReceiptReportList').jqGrid({
        url: ROOT + '/InventoryManagement/ReceiptReportJQGrid',
        datatype: 'Json',
        type: 'GET',
        shrinkToFit: false,
        height: 245,
        width: 1200,
        colNames: ['Receipt_Id', 'Receipt Key', 'Extern PO Key', 'Storer Key', 'Company', 'SKU', 'Description', 'Qty Expected', 'Qty Received', 'Date Received', 'Stock Type', 'Batch', 'Man Date', 'Receipt Date', 'Expiry Date'
        , 'Cost', 'UnitCost In USD', 'Total Cost', 'TotalCost In USD', 'PO Key', 'Supplier Code', 'Supplier Name', 'Supplier DeliveryNote No', 'InvoiceNumber', 'Container Key', 'Bill of Loading', 'Notes', 'Damage Qty', 'Condition Code', 'LOT'
        , 'Retail SKU', 'ASN Price', 'Currency', 'Portal Price', 'ShelfLife At Receipt', 'ShelfLife', 'ShelfLife Percentage At Receipt', 'Date Start', 'Date End', 'ASN Start', 'ASN End', 'SKU Start', 'SKU End', 'Lottable01', 'Lottable09'
        , 'Container Qty', 'Total Qty Received', 'CreatedBy', 'CreatedDate', 'ModifiedBy', 'ModifiedDate', 'RequestId', 'IsValid'],
        colModel: [
            { name: 'Receipt_Id', index: 'Receipt_Id', hidden: true, key: true },
            { name: 'ReceiptKey', index: 'ReceiptKey' },
            { name: 'ExternPOKey', index: 'ExternPOKey' },
            { name: 'StorerKey', index: 'StorerKey' },
            { name: 'Company', index: 'Company', search: false },
            { name: 'SKU', index: 'SKU', search: false },
            { name: 'Description', index: 'Description', search: false },
            { name: 'QtyExpected', index: 'QtyExpected', search: false },
            { name: 'QtyReceived', index: 'QtyReceived', search: false },
            { name: 'DateReceived', index: 'DateReceived', search: false },
            { name: 'StockType', index: 'StockType', search: false },
            { name: 'Batch', index: 'Batch', search: false },
            { name: 'ManDate', index: 'ManDate', search: false },
            { name: 'ReceiptDate', index: 'ReceiptDate', search: false },
            { name: 'ExpiryDate', index: 'ExpiryDate', search: false },
            { name: 'Cost', index: 'Cost', search: false },
            { name: 'UnitCostInUSD', index: 'UnitCostInUSD', search: false },
            { name: 'TotalCost', index: 'TotalCost', search: false },
            { name: 'TotalCostInUSD', index: 'TotalCostInUSD', search: false },
            { name: 'POKey', index: 'POKey' },
            { name: 'SupplierCode', index: 'SupplierCode', search: false },
            { name: 'SupplierName', index: 'SupplierName', search: false },
            { name: 'SupplierDeliveryNoteNo', index: 'SupplierDeliveryNoteNo', search: false },
            { name: 'InvoiceNumber', index: 'InvoiceNumber' },
            { name: 'ContainerKey', index: 'ContainerKey' },
            { name: 'BillofLoading', index: 'BillofLoading', search: false },
            { name: 'Notes', index: 'Notes', search: false },
            { name: 'DamageQty', index: 'DamageQty', search: false },
            { name: 'ConditionCode', index: 'ConditionCode', search: false },
            { name: 'LOT', index: 'LOT', search: false },
            { name: 'RetailSKU', index: 'RetailSKU', search: false },
            { name: 'ASNPrice', index: 'ASNPrice', search: false },
            { name: 'Currency', index: 'Currency', search: false },
            { name: 'PortalPrice', index: 'PortalPrice', search: false },
            { name: 'ShelfLifeAtReceipt', index: 'ShelfLifeAtReceipt', search: false },
            { name: 'ShelfLife', index: 'ShelfLife', search: false },
            { name: 'ShelfLifePercentageAtReceipt', index: 'ShelfLifePercentageAtReceipt', search: false },
            { name: 'DateStart', index: 'DateStart', search: false },
            { name: 'DateEnd', index: 'DateEnd', search: false },
            { name: 'ASNStart', index: 'ASNStart', search: false },
            { name: 'ASNEnd', index: 'ASNEnd', search: false },
            { name: 'SKUStart', index: 'SKUStart', search: false },
            { name: 'SKUEnd', index: 'SKUEnd', search: false },
            { name: 'Lottable01', index: 'Lottable01', search: false },
            { name: 'Lottable09', index: 'Lottable09', search: false },
            { name: 'ContainerQty', index: 'ContainerQty', search: false },
            { name: 'TotalQtyReceived', index: 'TotalQtyReceived', search: false },
            { name: 'CreatedBy', index: 'CreatedBy', hidden: true },
            { name: 'CreatedDate', index: 'CreatedDate', hidden: true },
            { name: 'ModifiedBy', index: 'ModifiedBy', hidden: true },
            { name: 'ModifiedDate', index: 'ModifiedDate', hidden: true },
            { name: 'RequestId', index: 'RequestId', hidden: true },
            { name: 'IsValid', index: 'IsValid', hidden: true }
        ],
        rowNum: 1000,
        viewrecords: true,
        rowList: [1000, 2500, 5000],
        sortorder: 'Asc',
        sortname: 'Receipt_Id',
        caption: '&nbsp;<i class="icon-th-list"></i>&nbsp;Receipt Report Details List',
        pager: 'ReceiptReportListPager',
    });
    //$('#ReceiptReportList').jqGrid('filterToolbar', { searchOnEnter: true, enableClear: false, afterSearch: function () { $('#ReceiptReportList').clearGridData(); } });
    $("#ReceiptReportList").navGrid('#ReceiptReportListPager', { add: false, edit: false, del: false, search: false, refresh: false });
    //$("#ReceiptReportList").jqGrid('navButtonAdd', '#ReceiptReportListPager', {
    //    caption: "Export To Excel",
    //    onClickButton: function () {
    //        var ExptType = 'Excel';
    //        var searchItems = $('#Sector').val() + ',' + $('#contingentType').val() + ',' + $('#Contingent').val() + ',' + $('#Period').val() + ',' + $('#PeriodYear').val();
    //        window.open("OrderitemsMisMatchJqGrid" + '?searchItems=' + searchItems + '&ExptType=' + ExptType + '&rows=9999' + '&sidx=' + '&sord=desc' + '&page=1');
    //    }
    //});
});
function Search() {
    $("#ReceiptReportList").setGridParam(
        {
            datatype: "json",
            url: ROOT + '/InventoryManagement/ReceiptReportJQGrid',
            postData: { ReceiptKey: $('#txtReceiptKey').val(), ExternPOKey: $('#txtExternPOKey').val(), StorerKey: $('#txtStorerKey').val(), POKey: $('#txtPOKey').val(), InvoiceNumber: $('#txtInvoiceNumber').val(), ContainerKey: $('#txtContainerKey').val() },
            page: 0
        }).trigger("reloadGrid");
}
function ResetSearch() {
    $("input[type=text], textarea, select").val("");
    $('#ReceiptReportList').setGridParam({
        datatype: "json",
        url: ROOT + '/InventoryManagement/ReceiptReportJQGrid',
        postData: { ReceiptKey: "", ExternPOKey: "", StorerKey: "", POKey: "", InvoiceNumber: "", ContainerKey: "" },
        page: 1
    }).trigger("reloadGrid");
}