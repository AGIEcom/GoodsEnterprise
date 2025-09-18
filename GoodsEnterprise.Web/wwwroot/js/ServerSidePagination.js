// Make sure initDataTable is available globally
if (typeof initDataTable !== 'function') {
    //console.error('initDataTable function is not available. Make sure site.js is loaded first.');
}

$(document).ready(function () {
    // Initialize tables
    ProductGridDataLoading();
    PromotionPriceGridDataLoading();
   
    // Reload table when search filter changes
    //$('#searchByDropdown').on('change', function() {
    //    const table = $('#tblProductMaster').DataTable();
    //    if (table) {
    //        table.ajax.reload(null, false); // false = don't reset paging
            
    //        // Refresh the table after data is loaded
    //        table.on('draw', function() {
    //            if (typeof resizeDataTables === 'function') {
    //                resizeDataTables();
    //            }
    //        });
    //    }
    //});
    
    // Reload Product table when custom search input changes
    $('#customSearchInputProduct').on('keyup', function () {
        if (this.value.length >= 5) {
            const table = $('#tblProductMaster').DataTable();
            if (table) {
                table.ajax.reload(null, false); // false = don't reset paging
            }
        }
    });
    
    // Reload PromotionCost table when search filter changes
    //$('#searchByDropdownPromotionCost').on('change', function() {
    //    const table = $('#tblPromotionCost').DataTable();
    //    if (table) {
    //        table.ajax.reload(null, false); // false = don't reset paging
            
    //        // Refresh the table after data is loaded
    //        table.on('draw', function() {
    //            if (typeof resizeDataTables === 'function') {
    //                resizeDataTables();
    //            }
    //        });
    //    }
    //});
    
    // Reload PromotionCost table when custom search input changes
    $('#customSearchInput').on('keyup', function () {
        if (this.value.length >= 5) {
            const table = $('#tblPromotionCost').DataTable();
            if (table) {
                table.ajax.reload(null, false); // false = don't reset paging
            }
        }
    });
    
    // Handle window resize for DataTables in this file
    let resizeTimer;
    $(window).on('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function() {
            if (typeof resizeDataTables === 'function') {
                resizeDataTables();
            }
        }, 250);
    });

    function ProductGridDataLoading() {
        // Destroy existing DataTable if it exists
        if ($.fn.DataTable.isDataTable('#tblProductMaster')) {
            $('#tblProductMaster').DataTable().destroy();
        }

        // Initialize with our custom function
        tbl_barangay = initDataTable('#tblProductMaster', {
            processing: true,
            serverSide: true,
            responsive: true,
            autoWidth: true,
            scrollX: true,
            scrollCollapse: true,
            lengthMenu: [5, 10, 20, 50],
            pageLength: 5,
            searching: false, // Disable default search to use our custom search
            order: [[6, "desc"]],
            deferRender: true,
            //drawCallback: function() {
            //    // Handle resizing after table draw
            //    if (typeof resizeDataTables === 'function') {
            //        resizeDataTables();
            //    }
            //},
            columnDefs: [{
                targets: [7], /* column index */
                orderable: false, /* true or false */

            }],
            initComplete: function() {
                // Using custom search controls, no need to move DataTables search input
                // The default search is disabled and we use our custom search input
            },
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
                    additionalValues[0] = $("#searchByDropdown").val() || "All";
                    data.AdditionalValues = additionalValues;
                    
                    // Add custom search text for Product
                    var customSearchText = $('#customSearchInputProduct').val() || '';
                    data.search = { value: customSearchText };
                    
                    // Force modifiedDate sorting on initial load
                    if (data.start === 0 && (!data.search || !data.search.value)) {
                        data.SortOrder = "modifiedDate DESC";
                    }
                    
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
                    data: "code",
                    name: "Code"
                },
                {
                    data: "productName",
                    name: "ProductName"
                },
                {
                    data: "categoryName",
                    name: "CategoryName"
                },
                {
                    data: "brandName",
                    name: "BrandName"
                },
                {
                    data: "outerEan",
                    name: "OuterEan"
                },
                {
                    data: "status",
                    name: "Status"
                },  
                {
                    data: "modifiedDate",
                    name: "ModifiedDate",
                    render: function (data, type, row) {
                        if (type === 'display' && data) {
                            return new Date(data).toLocaleDateString('en-GB');
                        }
                        return data;
                    }
                },
                {
                    data: "id",
                    name: "Id",
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return '<a class="modern-btn modern-btn-primary modern-btn-sm" href="/all-product?productId=' + row.id + '&amp;handler=Edit">Edit</a> ' +
                                '<a href="/all-product?productId=' + row.id + '&amp;handler=DeleteProduct" class="modern-btn modern-btn-danger modern-btn-sm btn-product-delete">Delete</a>';
                        }
                        return data;
                    }
                }


            ]
        });
    }

    //function PromotionPriceGridDataLoading() {

    //    tbl_barangay = $('#tblPromotionCost').dataTable({

    //        processing: true,
    //        serverSide: true,
    //        responsive: true,
    //        lengthMenu: [10, 20, 50],

    //        "order": [[1, "desc"]],
    //        "deferRender": true,
    //        'columnDefs': [{

    //            'targets': [6], /* column index */

    //            'orderable': false, /* true or false */

    //        }],
    //        ajax: {
    //            type: "POST",
    //            url: './api/DataBasePagination/getpromotioncostdata',
    //            contentType: "application/json; charset=utf-8",
    //            dataType: "json",
    //            headers: {
    //                "XSRF-TOKEN": document.querySelector('[name="__RequestVerificationToken"]').value
    //            },
               
    //            async: true,
    //            data: function (data) {
    //                let additionalValues = [];
    //                //additionalValues[0] = $("#txtfromDate").val();
    //                //additionalValues[1] = $("#txtToDate").val();
    //                //var Appic = GetApplication();
    //                //additionalValues[2] = Appic;
    //                //data.AdditionalValues = additionalValues;
    //                return JSON.stringify(data);
    //            },
    //            error: function (jqXHR, exception) {
    //                var msg = '';
    //                if (jqXHR.status === 0) {
    //                    msg = 'Not connect.\n Verify Network.';
    //                } else if (jqXHR.status == 404) {
    //                    msg = 'Requested page not found. [404]';
    //                } else if (jqXHR.status == 500) {
    //                    msg = 'Internal Server Error [500].';
    //                } else if (exception === 'parsererror') {
    //                    msg = 'Requested JSON parse failed.';
    //                } else if (exception === 'timeout') {
    //                    msg = 'Time out error.';
    //                } else if (exception === 'abort') {
    //                    msg = 'Ajax request aborted.';
    //                } else {
    //                    msg = 'Uncaught Error.\n' + jqXHR.responseText;
    //                }

    //            }
    //        },
    //        columns: [                           
    //            {
    //                data: "SupplierName"
    //            },
    //            {
    //                data: "ProductName"
    //            },
    //            {
    //                data: "PromotionCost"
    //            },
    //            {
    //                data: "StartDate"
    //            },
    //            {
    //                data: "EndDate"
    //            },
    //            {
    //                data: "Status"
    //            },
    //            {
    //                data: "PromotionCostID",
    //                render: function (data, type, row) {
    //                    if (type === 'display') {
    //                        return '<a class="btn btn-primary" href="/all-promotion-cost?PromotionCostId=' + row.PromotionCostID + '&amp;handler=Edit">Edit</a> | ' +
    //                            '<a href="/all-promotion-cost?PromotionCostId=' + row.PromotionCostID + '&amp;handler=DeleteProduct" class="btn btn-primary btn-PromotionCost-delete">Delete</a>';
    //                    }
    //                    return data;
    //                }
    //            }
                 

    //        ]
    //    });
    //}

    function PromotionPriceGridDataLoading() {
        if (!$('#tblPromotionCost').length) {
            //console.error('#tblPromotionCost not found');
            return;
        }

        // Initialize with our custom function
        tbl_barangay = initDataTable('#tblPromotionCost', {
            processing: true,
            serverSide: true,
            responsive: true,
            autoWidth: true,
            scrollX: true,
            scrollCollapse: true,
            lengthMenu: [5, 10, 20, 50],
            pageLength: 5,
            order: [[1, "desc"]],
            deferRender: true,
            searching: false, // Disable default search to use our custom search
            drawCallback: function() {
                // Handle resizing after table draw
                if (typeof resizeDataTables === 'function') {
                    resizeDataTables();
                }
            },
            columnDefs: [
                { 
                    targets: [6], // action column is index 6 (0..6)
                    orderable: false,
                    className: 'text-nowrap' // Prevent action buttons from wrapping
                }
            ],
            ajax: {
                type: "POST",
                url: './api/DataBasePagination/getpromotioncostdata',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: {
                    "XSRF-TOKEN": document.querySelector('[name="__RequestVerificationToken"]').value
                },
                data: function (data) {
                    // Add SearchBy parameter and custom search text for PromotionCost
                    var searchBy = $('#searchByDropdownPromotionCost').val() || 'All';
                    var customSearchText = $('#customSearchInput').val() || '';
                    data.additionalValues = [searchBy];
                    data.search = { value: customSearchText };
                    return JSON.stringify(data);
                },
                dataSrc: function (json) {
                    console.log("DT server response:", json); // check shape in console
                    // defensive: if server returns top-level array, return it; if returns object with data, return that
                    if (!json) return [];
                    return json.data || json;
                },
                error: function (xhr, status, error) {
                    console.error("DataTables AJAX error:", status, error);
                    console.log("Response text:", xhr && xhr.responseText);
                }
            },
            columns: [
                // Use functions to handle PascalCase OR camelCase JSON keys
                { data: function (row) { return row.SupplierName || row.supplierName || ''; } },
                { data: function (row) { return row.ProductName || row.productName || ''; } },
                { data: function (row) { return row.PromotionCost || row.promotionCost || ''; } },
                { data: function (row) { 
                    var startDate = row.StartDate || row.startDate || '';
                    return startDate ? new Date(startDate).toLocaleDateString() : '';
                } },
                { data: function (row) { 
                    var endDate = row.EndDate || row.endDate || '';
                    return endDate ? new Date(endDate).toLocaleDateString() : '';
                } },
                { data: function (row) { return row.Status || row.status || ''; } },
                {
                    data: function (row) {
                        var id = row.PromotionCostID || row.promotionCostID || '';
                        return '<a class="btn btn-primary" href="/all-promotion-cost?PromotionCostId=' + id + '&amp;handler=Edit">Edit</a> | ' +
                            '<a href="/all-promotion-cost?PromotionCostId=' + id + '&amp;handler=Delete" class="btn btn-primary btn-PromotionCost-delete">Delete</a>';
                    }
                }
            ],
            initComplete: function() {
                // Using custom search controls, no need to move DataTables search input
                // The default search is disabled and we use our custom search input
            }
        });
    }
});

