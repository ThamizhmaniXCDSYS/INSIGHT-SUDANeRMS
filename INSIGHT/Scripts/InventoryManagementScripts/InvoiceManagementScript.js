$(document).ready(function () {
    debugger;
    var lastsel;
    $('#hiddenInvoiceItemLineId').val(0);

    $('#dvLoading').hide() // hide it initially.
.ajaxStart(function () {

    $('#dvLoading').show(); // show on any Ajax event.
})

.ajaxStop(function () {
    $('#dvLoading').hide(); // hide it when it is done.
});
    $("#ddlPONumber").select2({
    });
    $(".datepicker").datepicker();
    //// Autocomplete Search for InvoiceNumber....
    var cache = {};
    $('#txtInvoiceNumberautosuggest').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: ROOT + '/InventoryManagement/GetInvoiceNumber/',
                type: "POST",
                dataType: "json",
                data: { term: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Text, value: item.Value };
                    }))
                }
            })
        },
        minLength: 1,
        delay: 100,
        select: function (event, ui) {
            event.preventDefault();
            $('#hiddenInvoiceId').val(ui.item.value);
            $('#hiddenAutosuggestInvoiceId').val(ui.item.value);
            $('#txtInvoiceNumberautosuggest').val(ui.item.label);
        },
        focus: function (event, ui) {
            event.preventDefault();
            $('#txtInvoiceNumberautosuggest').val(ui.item.label);
        },
        messages: {
            noResults: '', results: ''
        }
    });

    $('#txtGLDate').datepicker()
    .on("input change", function (e) {
        $.ajax({
            url: ROOT + '/InventoryManagement/GetExchangeRateByGLDateandCurrency?GLDate=' + $('#txtGLDate').val() + '&Currency=' + $('#txtInvCurrency').val(),
            type: 'GET',
            dataType: "json",
            success: function (data) {
                $('#txtExchangeRate').val(data);
                $('#txtInvAmtUSD').val(parseInt($('#txtExchangeRate').val()) * parseInt($('#txtTotInvoiceAmount').val()));
            }
        });
    });
    GetPODetailsList();
    FillPORequest();
    $('#btnGetPO').click(function () {
        PurchOrderIds = $('#ddlPONumber').val();
        POInvoiceId = $('#hiddenAutosuggestInvoiceId').val();
        if (POInvoiceId == '' || POInvoiceId == undefined || POInvoiceId == 0) { POInvoiceId = 0; }
        if (PurchOrderIds == '' || PurchOrderIds == undefined && POInvoiceId == '' || POInvoiceId == undefined) {
            ErrMsg("Select PO / Invoice Number");
            return false;
        }
       
        else {
            resetInvoicePanel();
            $.ajax({
                type: 'GET',
                dataType: "json",
                url: ROOT + '/InventoryManagement/GetPODetailsbyPO/?PurchOrderIds=' + $('#ddlPONumber').val() + '&POInvoiceId=' + POInvoiceId,
                success: function (data) {
                    debugger;
                    $('#dvLoading').hide();
                    if (data.length > 1) {
                        ErrMsg('Multiple Supplier details available for selected PO!!');
                        return false;
                    }
                    else {
                        GetPODetailsList();
                        $('#txtSupplierName').val(data[0].Supplier);
                        $('#txtSupplierNumber').val(data[0].SupplierNumber);
                        $('#txtInvCurrency').val(data[0].POCurrency);
                        $('#txtInvoiceNumber').val(data[0].InvoiceNumber);
                        $('#txtBLNo').val(data[0].BillOfLading);
                        $('#txtContainerNumber').val(data[0].ContainerNumber);
                        $('#txtTotInvoiceAmount').val(data[0].InvoiceAmount);
                        $('#txtInvoiceDate').val(data[0].strInvDate);
                        $('#txtGLDate').val(data[0].strGLDate);
                        $('#txtVoucherNo').val(data[0].VoucherNumber);
                    }
                }
            });
        }
    });
    $('#btnSave').click(function () {

        if ($('#ddlPONumber').val() == '' || $('#ddlPONumber').val() == undefined) {
            ErrMsg("Select PO Number");
            return false;
        }
        if ($('#txtInvoiceNumber').val() == '' || $('#txtInvoiceNumber').val() == undefined) {
            ErrMsg("Enter Invoice Number");
            return false;
        }
        if ($('#txtInvoiceDate').val() == '' || $('#txtInvoiceDate').val() == undefined) {
            ErrMsg("Enter Invoice Date");
            return false;
        }
        if ($('#txtTotInvoiceAmount').val() == '' || $('#txtTotInvoiceAmount').val() == undefined) {
            ErrMsg("Enter Invoice Amount");
            return false;
        }
        if ($('#txtBLNo').val() == '' || $('#txtBLNo').val() == undefined) {
            ErrMsg("Enter Bill of Lading Number");
            return false;
        }
        if ($('#txtContainerNumber').val() == '' || $('#txtContainerNumber').val() == undefined) {
            ErrMsg("Enter Container Number");
            return false;
        }
        $.ajax({
            type: 'GET',
            dataType: "json",
            data: { POInvoiceId: $('#hiddenInvoiceId').val(), InvoiceNumber: $('#txtInvoiceNumber').val(), BillOfLading: $('#txtBLNo').val(), ContainerNumber: $('#txtContainerNumber').val(), InvoiceAmount: $('#txtTotInvoiceAmount').val() },
            //url: ROOT + '/InventoryManagement/SaveInvoiceDetailsforPO/?PurchOrderId=' + $('#ddlPONumber').val() + '&InvNumber=' + $('#txtInvoiceNumber').val() + '&InvDate=' + $('#txtInvoiceDate').val() + '&BLNo=' + $('#txtBLNo').val() + '&ContainerNo=' + $('#txtContainerNumber').val() + '&POInvoiceId=' + $('#hiddenInvoiceId').val(),
            url: ROOT + '/InventoryManagement/SaveInvoiceDetailsforPO/?PurchOrderId=' + $('#ddlPONumber').val() + '&InvDate=' + $('#txtInvoiceDate').val(),
            success: function (data) {
                $('#dvLoading').hide();
                if (data.InvoiceId > '0') {
                    GetPODetailsList();
                    $('#hiddenInvoiceId').val(data.InvoiceId);
                    $('#txtInvoiceNumber').prop('readonly', true);
                    $('#txtInvoiceDate').prop('readonly', true);
                    $('#txtBLNo').prop('readonly', true);
                    $('#txtContainerNumber').prop('readonly', true);
                    $('#txtTotInvoiceAmount').prop('readonly', true);
                }
                else {
                    ErrMsg(data.msg);
                }
            }
        });

    });
    $('#btnComplete').click(function () {
        if ($('#ddlPONumber').val() == '' || $('#ddlPONumber').val() == undefined) {
            ErrMsg("Select PO Number");
            return false;
        }
        if ($('#txtInvoiceNumber').val() == '' || $('#txtInvoiceNumber').val() == undefined) {
            ErrMsg("Enter Invoice Number");
            return false;
        }
        if ($('#txtInvoiceDate').val() == '' || $('#txtInvoiceDate').val() == undefined) {
            ErrMsg("Enter Invoice Date");
            return false;
        }
        if ($('#txtTotInvoiceAmount').val() == '' || $('#txtTotInvoiceAmount').val() == undefined) {
            ErrMsg("Enter Invoice Amount");
            return false;
        }
        if ($('#txtBLNo').val() == '' || $('#txtBLNo').val() == undefined) {
            ErrMsg("Enter Bill of Lading Number");
            return false;
        }
        if ($('#txtContainerNumber').val() == '' || $('#txtContainerNumber').val() == undefined) {
            ErrMsg("Enter Container Number");
            return false;
        }
        if ($('#txtGLDate').val() == '' || $('#txtGLDate').val() == undefined) {
            ErrMsg("Enter GL Date");
            return false;
        }
        if ($('#txtVoucherNo').val() == '' || $('#txtVoucherNo').val() == undefined) {
            ErrMsg("Enter Voucher No");
            return false;
        }
        if ($('#txtExchangeRate').val() == '' || $('#txtExchangeRate').val() == undefined || parseInt($('#txtExchangeRate').val()) == 0) {
            ErrMsg("Enter Exchange Rate");
            return false;
        }
        if ($('#txtInvDifference').val() > 0 || parseInt($('#txtInvDifference').val()) < 0) {
            ErrMsg("Invoiced amount not matching with Invoice amount");
            return false;
        }
        if ($('#hiddenInvoiceId').val() == "") {
            ErrMsg('Save Invoice details before complete!!');
            return false;
        }
        else {
            $.ajax({
                url: ROOT + '/InventoryManagement/CompletePurchaseOrderInvoice?GLDate=' + $('#txtGLDate').val(),
                type: 'POST',
                data: { POInvoiceId: $('#hiddenInvoiceId').val(), VoucherNumber: $('#txtVoucherNo').val(), ExchangeRate: $('#txtExchangeRate').val() },
                dataType: 'json',
                traditional: true,
                success: function (data) {
                    $('#dvLoading').hide();
                },
                loadError: function (xhr, status, error) {
                    msgError = $.parseJSON(xhr.responseText).Message;
                    ErrMsg(msgError, function () { });
                }
            });
        }
    });
});
function GetPODetailsList() {
    debugger;
    if ($('#hiddenInvoiceId').val() == '' || $('#hiddenInvoiceId').val() == undefined || $('#hiddenInvoiceId').val() == 0) { $('#hiddenInvoiceId').val(0); }
    $('#PODetailsList').jqGrid('GridUnload');
    $('#PODetailsList').jqGrid({
        datatype: 'Json',
        type: 'GET',
        shrinkToFit: false,
        url: ROOT + '/InventoryManagement/PurchaseOrderDetailsListJQGrid?PurchOrderId=' + $('#ddlPONumber').val() + '&POInvoiceId=' + $('#hiddenInvoiceId').val(),
        height: 245,
        width: 1220,
        colNames: ['Id', 'PurchOrderId', 'PO #', 'POLineId', 'UNCode', 'Commodity', 'Ordered Qty', 'PO UnitPrice', 'PO Value', 'Invoiced Qty', 'Remaining Qty', 'Invoice UnitPrice', 'Invoice UnitPrice', 'Invoice Qty', 'Remarks'],
        colModel: [
            { name: 'Id', index: 'Id', hidden: true, editable: true, key: true },
            { name: 'PurchOrderId', index: 'PurchOrderId', editable: true, editoptions: { disabled: true }, hidden: true },
            { name: 'PO', index: 'PO', width: 50 },
            { name: 'POLineId', index: 'POLineId', hidden: true, key: true, editable: true },
            { name: 'UNCode', index: 'UNCode', width: 60, search: true },
            { name: 'Commodity', index: 'Commodity', width: 350 },
            { name: 'OrderedQty', index: 'OrderedQty', width: 90, sortable: false, editable: true, editoptions: { readOnly: true } },
            { name: 'POUnitPrice', index: 'POUnitPrice', width: 80, editable: true, editoptions: { readOnly: true } },
            { name: 'POValue', index: 'POValue', width: 70, editable: true, editoptions: { readOnly: true } },
            { name: 'InvoicedQty', index: 'InvoicedQty', search: false, editable: true, editoptions: { disabled: true, class: 'NoBorder', style: " background-color:#FFFFFF; border-color:#FFFFFF;font-size:10px;" }, width: 90 },
            { name: 'RemainingQty', index: 'RemainingQty', search: false, editable: true, editoptions: { disabled: true, class: 'NoBorder', style: " background-color:#FFFFFF; border-color:#FFFFFF;font-size:10px;" }, width: 90 },
            { name: 'InvoiceUnitPrice', index: 'InvoiceUnitPrice', hidden: true },
            { name: 'InvoiceUnitPrice', index: 'InvoiceUnitPrice', sortable: false, search: false, width: 90, formatter: createtxtboxInvoiceunitprice },
            { name: 'InvoiceQty', index: 'InvoiceQty', sortable: false, width: 90, search: false, formatter: createtxtboxqty },
            { name: 'Remarks', index: 'Remarks', sortable: false, search: false, width: 130, formatter: createtxtboxRemarks }
        ],
        multiselect: true,
        viewrecords: true,
        rowNum: 500,
        rowList: [500, 1000, 2500, 5000],
        sortorder: 'Asc',
        sortname: 'POLineId',
        caption: '&nbsp;<i class="icon-th-list"></i>&nbsp;PO Item List',
        pager: 'PODetailsListPager',
        loadComplete: function () {
            var $this = $(this), rows = this.rows, l = rows.length, i, row;
            $(this).hide();
            for (i = 1; i < l; i++) {
                row = rows[i];
                if ($(row).hasClass("jqgrow")) {
                    $this.jqGrid('editRow', row.id);
                }
            }
            $(this).show();
            ID = $('#PODetailsList').jqGrid('getDataIDs');
        },
    });
    $('#PODetailsList').jqGrid('filterToolbar', { searchOnEnter: true, enableClear: false, afterSearch: function () { $('#PODetailsList').clearGridData(); } });
    $("#PODetailsList").navGrid('#PODetailsListPager', { add: false, edit: false, del: false, search: false, refresh: false })
    .navButtonAdd('#PODetailsListPager', {
        caption: "Disconnect",
        onClickButton: function () {
            //if ($('#hiddenInvoiceId').val() == "") {
            //    ErrMsg('Fill Invoice and Container details!!');
            //    return false;
            //}
            debugger;
            var arrayObj = [];
            var Ids = $("#PODetailsList").jqGrid('getGridParam', 'selarrrow');
            if (Ids.length > 0) {
                for (var i = 0; i < Ids.length; i++) {
                    if ($('#' + Ids[i] + '_Remarks').val() == "" || $('#' + Ids[i] + '_Remarks').val() == undefined) {
                        ErrMsg('Remarks should not be empty!!');
                        return false;
                    }
                    else {
                        arrayObj.push({ PurchOrderId: $('#' + Ids[i] + '_PurchOrderId').val(), POLineId: $('#' + Ids[i] + '_POLineId').val(), POInvoiceId: $('#hiddenInvoiceId').val() });
                    }
                }
            }
            if (Ids.length > 0) {
                $.ajax({
                    url: ROOT + '/InventoryManagement/DisconnectInvoiceItem',
                    type: 'POST',
                    data: JSON.stringify({ DataObj: arrayObj }),
                    dataType: 'json',
                    traditional: true,
                    contentType: 'application/json; charset=utf-8',
                    success: function (data) {
                        $('#dvLoading').hide();
                        InfoMsg(data.Msg);
                    },
                    loadError: function (xhr, status, error) {
                        msgError = $.parseJSON(xhr.responseText).Message;
                        ErrMsg(msgError, function () { });
                    }
                });
                //InfoMsg('Disconnected!!');
            }
            else {
                ErrMsg('Select Items to disconnect!!');
                return false;
            }
        }
    });

    $("#PODetailsList").trigger("reloadGrid");
    ///Invoice unitprice
    function createtxtboxInvoiceunitprice(cellvalue, options, rowObject) {
        return "<input id='" + rowObject[0] + "_InvoiceUnitPrice'  onkeypress=' AllowNumericValue(this);' onblur='CheckBox(" + rowObject[0] + ");VaidateInvoiceUnitPrice(" + rowObject[0] + "," + rowObject[3] + "," + rowObject[7] + ");' value='" + rowObject[11] + "'  style='width: 55px;font-size:10px' type='text'/>";
    }
    ///Invoice Qty
    function createtxtboxqty(cellvalue, options, rowObject) {
        return "<input id='InvoicedQty_" + rowObject[0] + "'  onkeypress=' AllowNumericValue(this);' onblur='CheckBox(" + rowObject[0] + ");CalculateRemainingQty(" + rowObject[0] + "," + rowObject[3] + "," + rowObject[6] + ");'   style='width: 55px;font-size:10px' type='text'/>";
    }
    ////Remarks
    function createtxtboxRemarks(cellvalue, options, rowObject) {
        return "<input id='" + rowObject[0] + "_Remarks'  onblur='CheckBox(" + rowObject[0] + ");updateRemarks(" + rowObject[0] + "," + rowObject[3] + ");'   style='width: 95px;font-size:10px' type='text'/>";
    }
}
function FillPORequest() {
    $.getJSON(ROOT + '/InventoryManagement/FillPONumber/',
function (modelData) {
    var select = $("#ddlPONumber");
    select.empty();
    select.append($('<option/>',
{
    value: "",
    text: "Select PO"
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
function checkvalid(value, column) {
    if (value == '') {
        return [false, column + ": Field is Required"];
    }
    else if (value == 0) {
        return [false, column + ": Field is must be greater then zero"];
    }
    else if (!$.isNumeric(value)) {
        return [false, column + 'Should be numeric'];
    }
    else {
        return [true];
    }
}
function VaidateInvoiceUnitPrice(Id, lineid, pounitPrice) {
    if ($('#hiddenInvoiceId').val() == "") {
        ErrMsg('Fill Invoice and Container details!!');
        return false;
    }
    if ($('#' + Id + '_InvoiceUnitPrice').val() == "") {
        $('#' + Id + '_InvoiceUnitPrice').val(0);
    }
    if ($('#' + Id + '_InvoiceUnitPrice').val() > 0 && parseFloat($('#' + Id + '_InvoiceUnitPrice').val()) > parseFloat($('#' + Id + '_POUnitPrice').val())) {
        ErrMsg('Invoice unitprice should not exceed PO unitprice');
        $('#' + Id + '_InvoiceUnitPrice').val($('#' + Id + '_POUnitPrice').val());
        return false;
    }
    if ($('#' + Id + '_InvoiceUnitPrice').val() > 0 && $('#' + Id + '_InvoiceUnitPrice').val() != 0.00 && $('#hiddenInvoiceId').val() > 0) {
        $.ajax({
            url: ROOT + '/InventoryManagement/UpdateInvoiceUnitPriceCell?PurchOrderId=' + $('#' + Id + '_PurchOrderId').val() + '&POInvoiceId=' + $('#hiddenInvoiceId').val() + '&POLineId=' + lineid + '&InvoiceItemLineId=' + $('#hiddenInvoiceItemLineId').val() + '&InvoiceUnitPrice=' + $('#' + Id + '_InvoiceUnitPrice').val(),
            type: 'POST',
            dataType: 'json',
            traditional: true,
            success: function (data) {
                $('#dvLoading').hide();
                if (data != null)
                    $('#hiddenInvoiceItemLineId').val(data.POInvoiceItemId);
            },
            loadError: function (xhr, status, error) {
                msgError = $.parseJSON(xhr.responseText).Message;
                ErrMsg(msgError, function () { });
            }
        });
    }

}
function CalculateRemainingQty(Id, lineid, OrdQty) {
    if ($('#hiddenInvoiceId').val() == "") {
        ErrMsg('Fill Invoice and Container details!!');
        return false;
    }
    if ($('#InvoicedQty_' + Id).val() == "") {
        $('#InvoicedQty_' + Id).val(0);
    }
    if ($('#InvoicedQty_' + Id).val() > 0 && parseInt($('#InvoicedQty_' + Id).val()) > parseInt($('#' + Id + '_OrderedQty').val())) {
        ErrMsg('Invoice Qty should not exceed Ordered Qty');
        $('#InvoicedQty_' + Id).val(0);
        return false;
    }
    if ($('#InvoicedQty_' + Id).val() > 0 && $('#' + Id + '_RemainingQty').val() != 0.00 && $('#hiddenInvoiceId').val() > 0) {
        var RemainingQty = $('#' + Id + '_RemainingQty').val() - $('#InvoicedQty_' + Id).val()
        $('#' + Id + '_RemainingQty').val(RemainingQty.toFixed(3));
        $.ajax({
            url: ROOT + '/InventoryManagement/UpdateInvoicedQtyCell?POLineId=' + lineid + '&InvoiceQty=' + $('#InvoicedQty_' + Id).val() + '&RemainingQty=' + $('#' + Id + '_RemainingQty').val() + '&POInvoiceId=' + $('#hiddenInvoiceId').val() + '&PurchOrderId=' + $('#' + Id + '_PurchOrderId').val() + '&InvoiceItemLineId=' + $('#hiddenInvoiceItemLineId').val(),
            type: 'POST',
            dataType: 'json',
            traditional: true,
            success: function (data) {
                $('#dvLoading').hide();
                $('#' + Id + '_InvoicedQty').val(data.InvoicedQty);
                $('#' + Id + '_RemainingQty').val(data.RemainingQty);
                $('#txtInvDifference').val(data.TotalInvoicedValue - parseInt($('#txtTotInvoiceAmount').val()));
                $('#hiddenInvoiceItemLineId').val(data.POInvoiceItemId);
            },
            loadError: function (xhr, status, error) {
                msgError = $.parseJSON(xhr.responseText).Message;
                ErrMsg(msgError, function () { });
            }
        });
    }
}
function updateRemarks(Id, lineid) {
    if ($('#hiddenInvoiceId').val() == "") {
        ErrMsg('Fill Invoice and Container details!!');
        return false;
    }
    if ($('#' + Id + '_Remarks').val() != "" && $('#' + Id + '_Remarks').val() != undefined && $('#hiddenInvoiceId').val() > 0) {
        $.ajax({
            url: ROOT + '/InventoryManagement/UpdateInvoiceRemarksCell?PurchOrderId=' + $('#' + Id + '_PurchOrderId').val() + '&POInvoiceId=' + $('#hiddenInvoiceId').val() + '&POLineId=' + lineid + '&InvoiceItemLineId=' + $('#hiddenInvoiceItemLineId').val() + '&Remarks=' + $('#' + Id + '_Remarks').val(),
            type: 'POST',
            dataType: 'json',
            traditional: true,
            success: function (data) {
                $('#dvLoading').hide();
                $('#' + Id + '_InvoicedQty').val(data.InvoicedQty);
                $('#' + Id + '_RemainingQty').val(data.RemainingQty);
                $('#' + Id + '_Remarks').val(data.Remarks)
                $('#hiddenInvoiceItemLineId').val(data.POInvoiceItemId);
            },
            loadError: function (xhr, status, error) {
                msgError = $.parseJSON(xhr.responseText).Message;
                ErrMsg(msgError, function () { });
            }
        });
    }

}
function CheckBox(Id) {
    jQuery("#PODetailsList").setSelection(Id, true);
}
function resetInvoicePanel() {
    //$('#hiddenInvoiceId').val('');
    $('#txtInvoiceNumber').val('');
    $('#txtInvoiceDate').val('');
    $('#txtBLNo').val('');
    $('#txtContainerNumber').val('');
    $('#txtTotInvoiceAmount').val('');
    $('#txtInvoiceNumber').prop('readonly', false);
    $('#txtBLNo').prop('readonly', false);
    $('#txtContainerNumber').prop('readonly', false);
    $('#txtTotInvoiceAmount').prop('readonly', false);
}