$(document).ready(function () {
    ProductGridDataLoading();
    PromotionPriceGridDataLoading();

    function ProductGridDataLoading() {

        tbl_barangay = $('#tblProductMaster').dataTable({

            processing: true,
            serverSide: true,
            responsive: true,
            lengthMenu: [10, 20, 50],

            "order": [[1, "desc"]],
            "deferRender": true,
            'columnDefs': [{

                'targets': [3], /* column index */

                'orderable': false, /* true or false */

            }],
            ajax: {
                type: "POST",
                url: './api/DataBasePagination/getproductdata',
                contentType: "application/json; charset=utf-8",
                headers: {
                    "XSRF-TOKEN": document.querySelector('[name="__RequestVerificationToken"]').value
                },

                async: true,
                data: function (data) {
                    let additionalValues = [];
                    //additionalValues[0] = $("#txtfromDate").val();
                    //additionalValues[1] = $("#txtToDate").val();
                    //var Appic = GetApplication();
                    //additionalValues[2] = Appic;
                    //data.AdditionalValues = additionalValues;
                    return JSON.stringify(data);
                },
                error: function (jqXHR, exception) {
                    var msg = '';
                    if (jqXHR.status === 0) {
                        msg = 'Not connect.\n Verify Network.';
                    } else if (jqXHR.status == 404) {
                        msg = 'Requested page not found. [404]';
                    } else if (jqXHR.status == 500) {
                        msg = 'Internal Server Error [500].';
                    } else if (exception === 'parsererror') {
                        msg = 'Requested JSON parse failed.';
                    } else if (exception === 'timeout') {
                        msg = 'Time out error.';
                    } else if (exception === 'abort') {
                        msg = 'Ajax request aborted.';
                    } else {
                        msg = 'Uncaught Error.\n' + jqXHR.responseText;
                    }

                }
            },
            columns: [
                {
                    data: "code"
                },
                {
                    data: "outerEan"
                },
                {
                    data: "status"
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return '<a class="btn btn-primary" href="/all-product?productId=' + row.id + '&amp;handler=Edit">Edit</a> | ' +
                                '<a href="/all-product?productId=' + row.id + '&amp;handler=DeleteProduct" class="btn btn-primary btn-product-delete">Delete</a>';
                        }
                        return data;
                    }
                }


            ]
        });
    }

    function PromotionPriceGridDataLoading() {

        tbl_barangay = $('#tblPromotionCost').dataTable({

            processing: true,
            serverSide: true,
            responsive: true,
            lengthMenu: [10, 20, 50],

            "order": [[1, "desc"]],
            "deferRender": true,
            'columnDefs': [{

                'targets': [3], /* column index */

                'orderable': false, /* true or false */

            }],
            ajax: {
                type: "POST",
                url: './api/DataBasePagination/getpromotioncostdata',
                contentType: "application/json; charset=utf-8",
                headers: {
                    "XSRF-TOKEN": document.querySelector('[name="__RequestVerificationToken"]').value
                },

                async: true,
                data: function (data) {
                    let additionalValues = [];
                    //additionalValues[0] = $("#txtfromDate").val();
                    //additionalValues[1] = $("#txtToDate").val();
                    //var Appic = GetApplication();
                    //additionalValues[2] = Appic;
                    //data.AdditionalValues = additionalValues;
                    return JSON.stringify(data);
                },
                error: function (jqXHR, exception) {
                    var msg = '';
                    if (jqXHR.status === 0) {
                        msg = 'Not connect.\n Verify Network.';
                    } else if (jqXHR.status == 404) {
                        msg = 'Requested page not found. [404]';
                    } else if (jqXHR.status == 500) {
                        msg = 'Internal Server Error [500].';
                    } else if (exception === 'parsererror') {
                        msg = 'Requested JSON parse failed.';
                    } else if (exception === 'timeout') {
                        msg = 'Time out error.';
                    } else if (exception === 'abort') {
                        msg = 'Ajax request aborted.';
                    } else {
                        msg = 'Uncaught Error.\n' + jqXHR.responseText;
                    }

                }
            },
            columns: [
                {
                    data: "code"
                },
                {
                    data: "outerEan"
                },
                {
                    data: "status"
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return '<a class="btn btn-primary" href="/all-product?productId=' + row.id + '&amp;handler=Edit">Edit</a> | ' +
                                '<a href="/all-product?productId=' + row.id + '&amp;handler=DeleteProduct" class="btn btn-primary btn-product-delete">Delete</a>';
                        }
                        return data;
                    }
                }


            ]
        });
    }
});

