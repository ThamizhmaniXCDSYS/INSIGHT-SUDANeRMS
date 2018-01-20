$(function () {
    $('#InvoiceDate').datepicker().datepicker('setDate', $("#InvoiceDate").val());
    $('#DCDate').datepicker().datepicker('setDate', $("#DCDate").val());
    $('#ReceivedDate').datepicker().datepicker('setDate', $("#ReceivedDate").val());
    $('#InvoiceDate').attr('readonly', true);
    $('#DCDate').attr('readonly', true);
    $('#ReceivedDate').attr('readonly', true);

    $("#btnComplete").click(function () {
        
        var Id = $("#Id").val();
        if (Id == 0) {
            ErrMsg("Please Save");
            return false;
        }
        if (skuValidation() == false) {
            return false;
        }
        else {
            if (confirm("Are you sure to Complete?")) {
                var objMatInward1 = {
                    Id: $("#Id").val()
                };
                $.ajax({
                    url: '/Store/CompleteMaterialInward',
                    type: 'POST',
                    dataType: 'json',
                    data: objMatInward1,
                    traditional: true,
                    success: function (data) {
                        if (data != "") {
                            InfoMsg(data + " is completed successfully", function () { window.location.href = '/Store/MaterialInwardList'; });
                        }
                    },
                    error: function (xhr, status, error) {
                        ErrMsg($.parseJSON(xhr.responseText).Message);
                    }
                });
            }
        }
    });
    $.getJSON("/Store/FillStoreUnits",
             function (fillig) {
                 var ddlUnits = $("#ddlUnits");
                 ddlUnits.empty();
                 ddlUnits.append($('<option/>',
                {
                    value: "",
                    text: "Select One"

                }));

                 $.each(fillig, function (index, itemdata) {
                     ddlUnits.append($('<option/>',
                         {
                             value: itemdata.Value,
                             text: itemdata.Text
                         }));
                 });
             });

    $("#SupplierSearch").click(function () {

        LoadPopupDynamicaly("/Store/StoreSupplier", $('#DivSupplierSearch'),
            function () {
                LoadSetGridParam($('#StoreSupplierList'), "/Store/StoreSupplierListJqGrid")
            });
    });
    $("#MaterialSearch").click(function () {
        var Id = $("#Id").val();
        if (Id == 0) {
            ErrMsg("Please Save");
            return false;
        }
        $("#StoreMaterialsList").jqGrid('resetSelection');
        LoadMaterialPopupDynamicaly("/Store/MaterialSearch", $('#DivMaterialSearch'),
            function () {
                LoadSetGridParam1($('#StoreMaterialsList'), "/Store/StoreSKUListJqGridForMaterialInward")
            },"",1000);
    });
    var clbPupGrdSel = null;
    function LoadPopupDynamicaly(dynURL, ModalId, loadCalBack, onSelcalbck, width) {

        if (width == undefined) { width = 700; }
        // if (ModalId.html().length == 0) {
        $.ajax({
            url: dynURL,
            type: 'GET',
            async: false,
            dataType: 'html', // <-- to expect an html response
            success: function (data) {
                ModalId.dialog({
                    height: 370,
                    width: width,
                    modal: true,
                    title: 'Supplier Details',
                    buttons: {}
                });
                ModalId.html(data);
            }
        });
        //   }
        clbPupGrdSel = onSelcalbck;
        ModalId.dialog('open');
        CallBackFunction(loadCalBack);
    }
    function LoadMaterialPopupDynamicaly(dynURL1, ModalId1, loadCalBack1, onSelcalbck1, width) {

        if (width == undefined) { width = 910; }
        if (ModalId1.html().length == 0) {
            $.ajax({
                url: dynURL1,
                type: 'GET',
                async: false,
                dataType: 'html', // <-- to expect an html response
                success: function (data) {
                    ModalId1.dialog({
                        height: 700,
                        width: width,
                        modal: true,
                        title: 'Material Details',
                        buttons: {}
                    });
                    ModalId1.html(data);
                }
            });
        }
        clbPupGrdSel = onSelcalbck1;
        ModalId1.dialog('open');
        CallBackFunction(loadCalBack1);
    }
    function LoadSetGridParam(GridId, brUrl) {

        GridId.setGridParam({
            url: brUrl,
            datatype: 'json',
            mtype: 'GET'
        }).trigger("reloadGrid");
    }
    function LoadSetGridParam1(GridId1, brUrl1) {

        GridId1.setGridParam({
            url: brUrl1,
            datatype: 'json',
            mtype: 'GET'
        }).trigger("reloadGrid");
    }

    $("#txtOrderQuantity").keyup(function () {
        NumbersOnly($(this).val());
    });
    $("#txtOrderQuantity").keyup(function () {
        NumbersOnly($(this).val());
    });
    $("#txtReceivedQty").keyup(function () {
        NumbersOnly($(this).val());
    });
    $("#txtDamagedQty").keyup(function () {
        NumbersOnly($(this).val());
    });

    function NumbersOnly(value) {
        if (isNaN(value)) {
            ErrMsg("Numbers only allowed.");
            return false;
        }
    }

});

