// Make sure initDataTable is available globally
if (typeof initDataTable !== 'function') {
    //console.error('initDataTable function is not available. Make sure site.js is loaded first.');
}

// Global variables to prevent multiple initializations
var productTableInitialized = false;
var promotionTableInitialized = false;
var baseCostTableInitialized = false;

var supplierTableInitialized = false;

$(document).ready(function () {
    // Initialize tables
    if ($('#ProductTypehtn').val() == 'List') {
        ProductGridDataLoading();
    }
    if ($('#PromotionTypehtn').val() == 'List') {
        PromotionPriceGridDataLoading();
    }
    if ($('#BasePageTypehtn').val() == 'List') {
        BaseCostGridDataLoading();
    }
    if ($('#SupplierTypehtn').val() == 'List') {
        SupplierGridDataLoading();
    }
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

    // Prevent table reinitialization on window focus
    $(window).on('focus', function () {
        console.log('Window focused - tables should remain stable');
    });

    // Reload table when search filter changes
    // Search dropdown functionality removed - using custom search instead

    // Reload Product table when custom search input changes
    $('#customSearchInputProduct').on('keyup', function () {
        if (this.value.length >= 5) {
            const table = $('#tblProductMaster').DataTable();
            if (table) {
                table.ajax.reload(null, false); // false = don't reset paging
            }
        }
    });
    $('#refreshProductTable').on('click', function () {
        $('#customSearchInputProduct').val("");
        $('#searchByDropdown').val("All");
        const table = $('#tblProductMaster').DataTable();
        if (table) {
            table.ajax.reload(null, false); // false = don't reset paging
        }
    });
    $('#clearProductSearch').on('click', function () {
        $('#customSearchInputProduct').val("");
        const table = $('#tblProductMaster').DataTable();
        if (table) {
            table.ajax.reload(null, false); // false = don't reset paging
        }
    });

    // Supplier search functionality
    $('#customSearchInputSupplier').on('keyup', function () {
        if (this.value.length >= 3) {
            const table = $('#tblSupplierMaster').DataTable();
            if (table) {
                table.ajax.reload(null, false); // false = don't reset paging
            }
        }
    });
    $('#refreshSupplierTable').on('click', function () {
        $('#customSearchInputSupplier').val("");
        $('#searchByDropdownSupplier').val("All");
        const table = $('#tblSupplierMaster').DataTable();
        if (table) {
            table.ajax.reload(null, false); // false = don't reset paging
        }
    });
    $('#clearSupplierSearch').on('click', function () {
        $('#customSearchInputSupplier').val("");        
        const table = $('#tblSupplierMaster').DataTable();
        if (table) {
            table.ajax.reload(null, false); // false = don't reset paging
        }
    });
    

    // PromotionCost search dropdown removed - using custom search instead

    // Reload PromotionCost table when custom search input changes
    $('#customSearchInput').on('keyup', function () {
        if (this.value.length >= 5) {
            const table = $('#tblPromotionCost').DataTable();
            if (table) {
                table.ajax.reload(null, false); // false = don't reset paging
            }
        }
    });


    // Window resize handler to fix column alignment issues
    $(window).on('resize', function () {
        // Add a small delay to ensure the resize is complete
        setTimeout(function () {
            // Adjust columns for all initialized DataTables
            if (productTableInitialized && $.fn.DataTable.isDataTable('#tblProductMaster')) {
                $('#tblProductMaster').DataTable().columns.adjust();
            }
            if (promotionTableInitialized && $.fn.DataTable.isDataTable('#tblPromotionCost')) {
                $('#tblPromotionCost').DataTable().columns.adjust();
            }
            if (baseCostTableInitialized && $.fn.DataTable.isDataTable('#tblBaseCost')) {
                $('#tblBaseCost').DataTable().columns.adjust();
            }
            if (supplierTableInitialized && $.fn.DataTable.isDataTable('#tblSupplierMaster')) {
                $('#tblSupplierMaster').DataTable().columns.adjust();
            }
        }, 250);
    });

    function ProductGridDataLoading() {
        // Check if already initialized using global variable
        if (productTableInitialized) {
            console.log('ProductGrid DataTable already initialized, skipping...');
            return $('#tblProductMaster').DataTable();
        }

        // Check if DataTable already exists and is properly initialized
        if ($.fn.DataTable.isDataTable('#tblProductMaster')) {
            console.log('ProductGrid DataTable already exists, skipping initialization');
            productTableInitialized = true;
            return $('#tblProductMaster').DataTable();
        }

        console.log('Initializing ProductGrid DataTable...');
        productTableInitialized = true;

        // Initialize with our custom function (now returns a Promise)
        return $('#tblProductMaster').DataTable({
            // const table = initDataTable('#tblProductMaster', {
            processing: true,
            serverSide: true,
            responsive: false, // Disable responsive to prevent collapse
            autoWidth: false,
            scrollX: true,
            scrollCollapse: false,
            lengthMenu: [5, 10, 20, 50],
            pageLength: 5,
            searching: false, // Disable default search to use our custom search
            order: [], // No default client-side ordering, let server handle default sort
            deferRender: true,
            // drawCallback removed - no resize functionality needed
            columnDefs: [{
                targets: [6], /* column index - Actions column (0-based: Code, ProductName, CategoryName, BrandName, OuterEan, Status, Actions) */
                orderable: false, /* true or false */

            }],
            initComplete: function () {
                $('#tblProductMaster_wrapper .dataTables_length').appendTo('#product-length-menu-placeholder');
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
                    data: "id",
                    name: "Id",
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return '<a class="modern-btn modern-btn-primary modern-btn-sm" href="/all-product?productId=' + row.id + '&amp;handler=Edit" title="Edit Product">' +
                                '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
                                '<path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>' +
                                '<path d="m18.5 2.5 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>' +
                                '</svg></a> ' +
                                '<a href="/all-product?productId=' + row.id + '&amp;handler=DeleteProduct" class="modern-btn modern-btn-sm modern-btn-danger btn-product-delete" onclick="return deleteConfirm(event, \'Product\', this.href)" title="Delete Product">' +
                                '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
                                '<polyline points="3,6 5,6 21,6"></polyline>' +
                                '<path d="m19,6v14a2,2 0 0,1 -2,2H7a2,2 0 0,1 -2,-2V6m3,0V4a2,2 0 0,1 2,-2h4a2,2 0 0,1 2,2v2"></path>' +
                                '<line x1="10" y1="11" x2="10" y2="17"></line>' +
                                '<line x1="14" y1="11" x2="14" y2="17"></line>' +
                                '</svg></a>';
                        }
                        return data;
                    }
                }


            ]
        });
    }
    function deleteConfirm(event, entityName, deleteUrl) {
        event.preventDefault(); // Prevent default link behavior

        showModernConfirm(
            'Delete Confirmation',
            `Are you sure you want to delete this ${entityName}? This action cannot be undone.`,
            'Delete',
            'Cancel',
            () => {
                // Proceed with deletion
                window.location.href = deleteUrl;
            }
        );

        return false; // Prevent default link behavior
    }

    // Make deleteConfirm globally available for other DataTables
    window.deleteConfirm = deleteConfirm;


    function PromotionPriceGridDataLoading() {
        if (!$('#tblPromotionCost').length) {
            //console.error('#tblPromotionCost not found');
            return;
        }

        // Check if already initialized using global variable
        if (promotionTableInitialized) {
            console.log('PromotionCost DataTable already initialized, skipping...');
            return $('#tblPromotionCost').DataTable();
        }

        // Check if DataTable already exists and is properly initialized
        if ($.fn.DataTable.isDataTable('#tblPromotionCost')) {
            console.log('PromotionCost DataTable already exists, skipping initialization');
            promotionTableInitialized = true;
            return $('#tblPromotionCost').DataTable();
        }

        console.log('Initializing PromotionCost DataTable...');
        promotionTableInitialized = true;

        // Initialize with our custom function
        return $('#tblPromotionCost').DataTable({
            // const table = initDataTable('#tblPromotionCost', {
            processing: true,
            serverSide: true,
            responsive: false, // Disable responsive to prevent collapse
            autoWidth: false,
            scrollX: true,
            scrollCollapse: false,
            lengthMenu: [5, 10, 20, 50],
            pageLength: 5,
            order: [], // No default client-side ordering, let server handle default sort
            deferRender: true,
            searching: false, // Disable default search to use our custom search
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
                {
                    data: function (row) { return row.SupplierName || row.supplierName || ''; },
                    name: "SupplierName"
                },
                {
                    data: function (row) { return row.ProductName || row.productName || ''; },
                    name: "ProductName"
                },
                {
                    data: function (row) { return row.PromotionCost || row.promotionCost || ''; },
                    name: "PromotionCost"
                },
                {
                    data: function (row) {
                        var startDate = row.StartDate || row.startDate || '';
                        return startDate ? new Date(startDate).toLocaleDateString() : '';
                    },
                    name: "StartDate"
                },
                {
                    data: function (row) {
                        var endDate = row.EndDate || row.endDate || '';
                        return endDate ? new Date(endDate).toLocaleDateString() : '';
                    },
                    name: "EndDate"
                },
                {
                    data: function (row) { return row.Status || row.status || ''; },
                    name: "Status"
                },
                {
                    data: function (row) {
                        var id = row.PromotionCostID || row.promotionCostID || '';
                        return '<a class="modern-btn modern-btn-primary modern-btn-sm" href="/all-promotion-cost?PromotionCostId=' + id + '&amp;handler=Edit" title="Edit Promotion Cost">' +
                            '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
                            '<path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>' +
                            '<path d="m18.5 2.5 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>' +
                            '</svg></a> ' +
                            '<a href="/all-promotion-cost?PromotionCostId=' + id + '&amp;handler=Delete" class="modern-btn modern-btn-sm modern-btn-danger btn-PromotionCost-delete" onclick="return deleteConfirm(event, \'PromotionCost\', this.href)" title="Delete Promotion Cost">' +
                            '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
                            '<polyline points="3,6 5,6 21,6"></polyline>' +
                            '<path d="m19,6v14a2,2 0 0,1 -2,2H7a2,2 0 0,1 -2,-2V6m3,0V4a2,2 0 0,1 2,-2h4a2,2 0 0,1 2,2v2"></path>' +
                            '<line x1="10" y1="11" x2="10" y2="17"></line>' +
                            '<line x1="14" y1="11" x2="14" y2="17"></line>' +
                            '</svg></a>';
                    }
                }
            ],
            initComplete: function () {
                $('#tblPromotionCost_wrapper .dataTables_length').appendTo('#promotion-cost-length-menu-placeholder');
                // Using custom search controls, no need to move DataTables search input
                // The default search is disabled and we use our custom search input
            }
        });
    }


    function BaseCostGridDataLoading() {
        // Check if already initialized using global variable
        if (baseCostTableInitialized) {
            console.log('BaseCost DataTable already initialized, skipping...');
            return $('#tblBaseCost').DataTable();
        }

        if (!$('#tblBaseCost').length) {
            console.error('#tblBaseCost not found');
            return;
        }

        // Check if DataTable already exists and is properly initialized
        if ($.fn.DataTable.isDataTable('#tblBaseCost')) {
            console.log('BaseCost DataTable already exists, skipping initialization');
            baseCostTableInitialized = true;
            return $('#tblBaseCost').DataTable();
        }

        console.log('Initializing BaseCost DataTable...');
        baseCostTableInitialized = true;

        // Initialize with our custom function
        return $('#tblBaseCost').DataTable({
            processing: true,
            serverSide: true,
            responsive: true,
            autoWidth: true,
            scrollX: true,
            scrollCollapse: true,
            lengthMenu: [5, 10, 20, 50],
            pageLength: 10,
            order: [], // No default client-side ordering, let server handle default sort
            deferRender: true,
            searching: false, // Disable default search to use our custom search
            columnDefs: [
                {
                    targets: [6], // action column is index 6 (0..6)
                    orderable: false,
                    className: 'text-nowrap' // Prevent action buttons from wrapping
                }
            ],
            ajax: {
                type: "POST",
                url: './api/DataBasePagination/getbasecostdata',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: {
                    "XSRF-TOKEN": document.querySelector('[name="__RequestVerificationToken"]').value
                },
                data: function (data) {
                    // Add SearchBy parameter and custom search text for BaseCost
                    var searchBy = $('#searchByDropdownBaseCost').val() || 'All';
                    var customSearchText = $('#customSearchInputBaseCost').val() || '';
                    data.additionalValues = [searchBy];
                    data.search = { value: customSearchText };
                    return JSON.stringify(data);
                },
                dataSrc: function (json) {
                    console.log("BaseCost DT server response:", json);
                    if (!json) return [];
                    return json.data || json;
                },
                error: function (xhr, status, error) {
                    console.error("BaseCost DataTables AJAX error:", status, error);
                    console.log("Response text:", xhr && xhr.responseText);
                }
            },
            columns: [
                // Use functions to handle PascalCase OR camelCase JSON keys
                {
                    data: function (row) { return row.SupplierName || row.supplierName || ''; },
                    name: "SupplierName"
                },
                {
                    data: function (row) { return row.ProductName || row.productName || ''; },
                    name: "ProductName"
                },
                {
                    data: function (row) {
                        var baseCost = row.BaseCost1 || row.baseCost1 || row.BaseCost || row.baseCost || '';
                        return baseCost ? parseFloat(baseCost).toFixed(2) : '0.00';
                    },
                    name: "BaseCost"
                },
                {
                    data: function (row) {
                        var startDate = row.StartDate || row.startDate || '';
                        return startDate ? new Date(startDate).toLocaleDateString() : '';
                    },
                    name: "StartDate"
                },
                {
                    data: function (row) {
                        var endDate = row.EndDate || row.endDate || '';
                        return endDate ? new Date(endDate).toLocaleDateString() : 'N/A';
                    },
                    name: "EndDate"
                },
                //{
                //    data: function (row) {
                //        var isActive = row.IsActive || row.isActive;
                //        return isActive === 'Active' ? '<span class="badge badge-success">Active</span>' : '<span class="badge badge-secondary">Inactive</span>';
                //    }
                //},
                {
                    data: function (row) { return row.Status || row.status || ''; },
                    name: "Status"
                },
                {
                    data: function (row) {
                        var id = row.BaseCostId || row.baseCostId || '';
                        return '<a class="modern-btn modern-btn-primary modern-btn-sm" href="/all-base-cost?BaseCostId=' + id + '&amp;handler=Edit" title="Edit Base Cost">' +
                            '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
                            '<path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>' +
                            '<path d="m18.5 2.5 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>' +
                            '</svg></a> ' +
                            '<a href="/all-base-cost?BaseCostId=' + id + '&amp;handler=Delete" class="modern-btn modern-btn-sm modern-btn-danger btn-BaseCost-delete" onclick="return deleteConfirm(event, \'BaseCost\', this.href)" title="Delete Base Cost">' +
                            '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
                            '<polyline points="3,6 5,6 21,6"></polyline>' +
                            '<path d="m19,6v14a2,2 0 0,1 -2,2H7a2,2 0 0,1 -2,-2V6m3,0V4a2,2 0 0,1 2,-2h4a2,2 0 0,1 2,2v2"></path>' +
                            '<line x1="10" y1="11" x2="10" y2="17"></line>' +
                            '<line x1="14" y1="11" x2="14" y2="17"></line>' +
                            '</svg></a>';
                    }
                }
            ],

            initComplete: function () {
                // Move the length menu to our custom placeholder
                $('#tblBaseCost_wrapper .dataTables_length').appendTo('#base-cost-length-menu-placeholder');
                console.log('BaseCost DataTable initialized successfully');
            }
        });

        // Delete confirmation for BaseCost - now handled by deleteConfirm function
        // The onclick="return deleteConfirm(event, 'BaseCost', this.href)" handles this
    }

    function SupplierGridDataLoading() {
        // Check if already initialized using global variable
        if (supplierTableInitialized) {
            console.log('Supplier DataTable already initialized, skipping...');
            return $('#tblSupplierMaster').DataTable();
        }
        
        // Check if DataTable already exists and is properly initialized
        if ($.fn.DataTable.isDataTable('#tblSupplierMaster')) {
            console.log('Supplier DataTable already exists, skipping initialization');
            supplierTableInitialized = true;
            return $('#tblSupplierMaster').DataTable();
        }

        console.log('Initializing Supplier DataTable...');
        supplierTableInitialized = true;
        
        // Initialize with our custom function
        return $('#tblSupplierMaster').DataTable({
            processing: true,
            serverSide: true,
            responsive: false, // Disable responsive to prevent collapse
            autoWidth: true, // Enable automatic column width calculation
            scrollX: true,
            scrollCollapse: false,
            lengthMenu: [5, 10, 20, 50],
            pageLength: 10,
            searching: false, // Disable default search to use our custom search
            order: [], // No default client-side ordering, let server handle default sort
            deferRender: true,
            columnDefs: [{
                targets: [5], /* column index - Actions column (0-based: SupplierName, SKUCode, Email, Description, Status, Actions) */
                orderable: false, /* true or false */
            }],
            dom: 'lrtip', // Ensure the length menu is created
            initComplete: function () {
                // Move the length menu to our custom placeholder
                $('#tblSupplierMaster_wrapper .dataTables_length').appendTo('#supplier-length-menu-placeholder');
            },
            ajax: {
                type: "POST",
                url: './api/DataBasePagination/getsupplierdata',
                contentType: "application/json; charset=utf-8",
                headers: {
                    "XSRF-TOKEN": document.querySelector('[name="__RequestVerificationToken"]').value
                },
                async: true,
                data: function (data) {
                    let additionalValues = [];
                    additionalValues[0] = $("#searchByDropdownSupplier").val() || "All";
                    data.AdditionalValues = additionalValues;

                    // Add custom search text for Supplier
                    var customSearchText = $('#customSearchInputSupplier').val() || '';
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
                    data: "supplierName",
                    name: "SupplierName"
                },
                {
                    data: "skuCode",
                    name: "SKUCode"
                },
                {
                    data: "email",
                    name: "Email"
                },
                {
                    data: "description",
                    name: "Description"
                },
                {
                    data: "status",
                    name: "Status"
                },
                {
                    data: "id",
                    name: "Id",
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return '<div class="action-buttons d-flex gap-1">' +
                                '<a class="modern-btn modern-btn-primary modern-btn-xs" href="/all-supplier?supplierId=' + row.id + '&amp;handler=Edit" title="Edit Supplier">' +
                                '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
                                '<path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>' +
                                '<path d="m18.5 2.5 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>' +
                                '</svg></a>' +
                                '<a href="/all-supplier?supplierId=' + row.id + '&amp;handler=DeleteSupplier" class="modern-btn modern-btn-xs modern-btn-danger btn-supplier-delete" onclick="return deleteConfirm(event, \'Supplier\', this.href)" title="Delete Supplier">' +
                                '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
                                '<polyline points="3,6 5,6 21,6"></polyline>' +
                                '<path d="m19,6v14a2,2 0 0,1 -2,2H7a2,2 0 0,1 -2,-2V6m3,0V4a2,2 0 0,1 2,-2h4a2,2 0 0,1 2,2v2"></path>' +
                                '<line x1="10" y1="11" x2="10" y2="17"></line>' +
                                '<line x1="14" y1="11" x2="14" y2="17"></line>' +
                                '</svg></a>' +
                                '</div>';
                        }
                        return data;
                    }
                }
            ]
        });
    }


});

