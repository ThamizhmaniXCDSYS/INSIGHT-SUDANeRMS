$(function () {
 
    $("#MaterialSearch1").click(function () {
        var Id = $("#Id").val();
        var BranchCode = $("#ddlReqForCamp").val();

        if (BranchCode == "") {
            ErrMsg("Please select Required for Campus");
            return false;
        }
        if (Id == 0) {
            ErrMsg("Please Save");
            return false;
        }
        LoadMaterialPopupDynamicaly("/Store/MaterialRequestMaterialSearch?Campus=" + BranchCode, $('#DivMaterialSearch'),
            function () {
                LoadSetGridParam1($('#StoreMaterialsList'), "/Store/StoreSKUListJqGrid")
            });
    });
    function LoadMaterialPopupDynamicaly(dynURL1, ModalId1, loadCalBack1, onSelcalbck1, width) {
        if (width == undefined) { width = 960; }
        if (ModalId1.html().length == 0) {
            $.ajax({
                url: dynURL1,
                type: 'GET',
                async: false,
                dataType: 'html', // <-- to expect an html response
                success: function (data) {
                    ModalId1.dialog({
                        height: 700,
                        width: 980,
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
    function LoadSetGridParam1(GridId1, brUrl1) {

        GridId1.setGridParam({
            url: brUrl1,
            datatype: 'json',
            mtype: 'GET'
        }).trigger("reloadGrid");
    }
    if ($("#Id").val() != 0) {
        $("#btnSave").hide();
    }

    $("#txtQuantity").keyup(function () {
        var qty = $("#txtQuantity").val();
        if (isNaN(qty)) {
            ErrMsg("Numbers only allowed for Quantity");
            return false;
        }
        else if (qty > 100) {
            ErrMsg("Quantity should not exceed 100.");
            return false;
        }
    });

    var clbPupGrdSel = null;
    function LoadPopupDynamicaly(dynURL, ModalId, loadCalBack, onSelcalbck, width) {
        if (width == undefined) { width = 597; }
        if (ModalId.html().length == 0) {
            $.ajax({
                url: dynURL,
                type: 'GET',
                async: false,
                dataType: 'html', // <-- to expect an html response
                success: function (data) {
                    ModalId.dialog({
                        height: 553,
                        width: width,
                        modal: true,
                        title: 'Student Details',
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
});

function LoadSetGridParam(GridId, brUrl) {
    GridId.setGridParam({
        url: brUrl,
        datatype: 'json',
        mtype: 'GET'
    }).trigger("reloadGrid");
}

