// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// DataTable resize functionality has been removed
// Tables now use native responsive behavior


//datatable initialization
$(document).ready(function () {
    $('#tblBrandMaster').DataTable({
        "dom": "t<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>", // Show table with pagination and info at bottom
        'columnDefs': [
            { 'targets': [3], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3] },
            { 'className': 'text-center', 'targets': [2, 3] },
            { 'width': '35%', 'targets': [0] },
            { 'width': '50%', 'targets': [1] },
            { 'width': '10%', 'targets': [2] },
            { 'width': '5%', 'targets': [3] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50],
        "pageLength": 10,
        "responsive": true,
        "scrollX": true,
        "autoWidth": false,
        "initComplete": function(settings, json) {
            // Wait for DOM to be ready before connecting controls
            setTimeout(function() {
                var table = $('#tblBrandMaster').DataTable();

                // Handle custom length select
                $('#brandLengthSelect').on('change', function() {
                    var newLength = parseInt($(this).val());
                    table.page.len(newLength).draw();
                });

                // Handle custom search input
                $('#brandSearchInput').on('keyup change', function() {
                    table.search($(this).val()).draw();
                });

                // Update the custom controls when table changes
                table.on('draw', function() {
                    $('#brandLengthSelect').val(table.page.len());
                    $('#brandSearchInput').val(table.search());
                });

                // Set initial values
                $('#brandLengthSelect').val(table.page.len());
                $('#brandSearchInput').val(table.search());
            }, 100); // Small delay to ensure DOM is ready
        },
        "drawCallback": function(settings) {
            // Fallback: Ensure controls work even if initComplete doesn't run
            try {
                var table = $('#tblBrandMaster').DataTable();
                $('#brandLengthSelect').val(table.page.len());
                $('#brandSearchInput').val(table.search());
            } catch (e) {
                console.log('DataTable not ready yet, will retry...');
            }
        }
    });
    $('#tblCategoryMaster').DataTable({
        "dom": "t<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>", // Show table with pagination and info at bottom
        'columnDefs': [
            { 'targets': [3], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3] },
            { 'className': 'text-center', 'targets': [2, 3] },
            { 'width': '35%', 'targets': [0] },
            { 'width': '50%', 'targets': [1] },
            { 'width': '10%', 'targets': [2] },
            { 'width': '5%', 'targets': [3] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50],
        "pageLength": 10,
        "responsive": true,
        "scrollX": true,
        "autoWidth": false,
        "initComplete": function(settings, json) {
            // Wait for DOM to be ready before connecting controls
            setTimeout(function() {
                var table = $('#tblCategoryMaster').DataTable();

                // Handle custom length select
                $('#categoryLengthSelect').on('change', function() {
                    var newLength = parseInt($(this).val());
                    table.page.len(newLength).draw();
                });

                // Handle custom search input
                $('#categorySearchInput').on('keyup change', function() {
                    table.search($(this).val()).draw();
                });

                // Update the custom controls when table changes
                table.on('draw', function() {
                    $('#categoryLengthSelect').val(table.page.len());
                    $('#categorySearchInput').val(table.search());
                });

                // Set initial values
                $('#categoryLengthSelect').val(table.page.len());
                $('#categorySearchInput').val(table.search());
            }, 100); // Small delay to ensure DOM is ready
        },
        "drawCallback": function(settings) {
            // Fallback: Ensure controls work even if initComplete doesn't run
            try {
                var table = $('#tblCategoryMaster').DataTable();
                $('#categoryLengthSelect').val(table.page.len());
                $('#categorySearchInput').val(table.search());
            } catch (e) {
                console.log('DataTable not ready yet, will retry...');
            }
        }
    });

    $('#tblSubCategoryMaster').DataTable({
        "dom": "t<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>", // Show table with pagination and info at bottom
        'columnDefs': [
            { 'targets': [3], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3] },
            { 'className': 'text-center', 'targets': [2, 3] },
            { 'width': '35%', 'targets': [0] },
            { 'width': '50%', 'targets': [1] },
            { 'width': '10%', 'targets': [2] },
            { 'width': '5%', 'targets': [3] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50],
        "pageLength": 10,
        "responsive": true,
        "scrollX": true,
        "autoWidth": false,
        "initComplete": function(settings, json) {
            // Wait for DOM to be ready before connecting controls
            setTimeout(function() {
                var table = $('#tblSubCategoryMaster').DataTable();

                // Handle custom length select
                $('#subCategoryLengthSelect').on('change', function() {
                    var newLength = parseInt($(this).val());
                    table.page.len(newLength).draw();
                });

                // Handle custom search input
                $('#subCategorySearchInput').on('keyup change', function() {
                    table.search($(this).val()).draw();
                });

                // Update the custom controls when table changes
                table.on('draw', function() {
                    $('#subCategoryLengthSelect').val(table.page.len());
                    $('#subCategorySearchInput').val(table.search());
                });

                // Set initial values
                $('#subCategoryLengthSelect').val(table.page.len());
                $('#subCategorySearchInput').val(table.search());
            }, 100); // Small delay to ensure DOM is ready
        },
        "drawCallback": function(settings) {
            // Fallback: Ensure controls work even if initComplete doesn't run
            try {
                var table = $('#tblSubCategoryMaster').DataTable();
                $('#subCategoryLengthSelect').val(table.page.len());
                $('#subCategorySearchInput').val(table.search());
            } catch (e) {
                console.log('DataTable not ready yet, will retry...');
            }
        }
    });

    //$('#tblProductMaster').DataTable({
    //    'columnDefs': [
    //        { 'targets': [3], 'orderable': false },
    //        { 'searchable': false, "targets": [1, 2, 3] }
    //    ],
    //    "order": [],
    //    lengthMenu: [5, 10, 20, 50]
    //});

    $('#tblTaxMaster').DataTable({
        "dom": "t<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>", // Show table with pagination and info at bottom
        'columnDefs': [
            { 'targets': [4], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3, 4] },
            { 'className': 'text-center', 'targets': [2, 3, 4] },
            { 'width': '25%', 'targets': [0] },
            { 'width': '35%', 'targets': [1] },
            { 'width': '15%', 'targets': [2] },
            { 'width': '15%', 'targets': [3] },
            { 'width': '10%', 'targets': [4] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50],
        "pageLength": 10,
        "responsive": true,
        "scrollX": true,
        "autoWidth": false,
        "initComplete": function(settings, json) {
            // Wait for DOM to be ready before connecting controls
            setTimeout(function() {
                var table = $('#tblTaxMaster').DataTable();

                // Handle custom length select
                $('#taxLengthSelect').on('change', function() {
                    var newLength = parseInt($(this).val());
                    table.page.len(newLength).draw();
                });

                // Handle custom search input
                $('#taxSearchInput').on('keyup change', function() {
                    table.search($(this).val()).draw();
                });

                // Update the custom controls when table changes
                table.on('draw', function() {
                    $('#taxLengthSelect').val(table.page.len());
                    $('#taxSearchInput').val(table.search());
                });

                // Set initial values
                $('#taxLengthSelect').val(table.page.len());
                $('#taxSearchInput').val(table.search());
            }, 100); // Small delay to ensure DOM is ready
        },
        "drawCallback": function(settings) {
            // Fallback: Ensure controls work even if initComplete doesn't run
            try {
                var table = $('#tblTaxMaster').DataTable();
                $('#taxLengthSelect').val(table.page.len());
                $('#taxSearchInput').val(table.search());
            } catch (e) {
                console.log('DataTable not ready yet, will retry...');
            }
        }
    });

 

    $('#tblRoleMaster').DataTable({
        'columnDefs': [
            { 'targets': [2], 'orderable': false },
            { 'searchable': false, "targets": [1, 2] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50],
        "pageLength": 5
    });

    $('#tblAdminMaster').DataTable({
        'columnDefs': [
            { 'targets': [5], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3, 4, 5] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50],
        "pageLength": 5
    });

    $('#tblCustomerMaster').DataTable({
        'columnDefs': [
            { 'targets': [5], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3, 4, 5] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50],
        "pageLength": 5
    });
});
//end

 
// End of file

var emailReg = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
var passwordReg = /^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{5,25}$/;

// Write your Javascript code.
//brand master
$("#lnkCreateBrand").click(function () {
    $("#divCreateUpdateBrand").css("display", "block");
    $("#brandlist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
});

$(".brand-submit").click(function () {
    var brandName = $('#txtBrand').val();
    var error = false;
    $(".text-danger").remove();

    if (brandName.length < 1) {
        $('#txtBrand').after('<span class="text-danger">Brand name is required</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//$(".btn-brand-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});

function fileValidation() {
    var fileInput =
        document.getElementById('fileUpload');
    var selectedFileName = document.getElementById('selectedFileName');

    var filePath = fileInput.value;
    if (filePath == '') {
        // Hide file name display when no file is selected
        if (selectedFileName) {
            selectedFileName.style.display = 'none';
        }
        return true;
    }
    // Allowing file type
    var allowedExtensions =
        /(\.jpg|\.jpeg|\.png)$/i;

    if (!allowedExtensions.exec(filePath)) {
        alert('Invalid file type, Only jpg, jpeg, png are allowed');
        fileInput.value = '';
        // Hide file name display when file is cleared
        if (selectedFileName) {
            selectedFileName.style.display = 'none';
        }
        return false;
    }
    else {
        $("#imgPath").css("display", "none");
        
        // Show selected file name
        if (selectedFileName && fileInput.files && fileInput.files[0]) {
            var fileNameText = selectedFileName.querySelector('.file-name-text');
            if (fileNameText) {
                fileNameText.textContent = fileInput.files[0].name;
                selectedFileName.style.display = 'flex';
            }
        }
    }
}

function productFileValidation() {
    var fileInput =
        document.getElementById('productFileUpload');

    var filePath = fileInput.value;
    if (filePath == '') {
        return true;
    }
    // Allowing file type
    var allowedExtensions =
        /(\.xls|\.xlsx)$/i;

    if (!allowedExtensions.exec(filePath)) {
        alert('Invalid file type, Only xls, xlsx are allowed');
        fileInput.value = '';
        return false;
    }
    else {
        return true;
    }
}
//End

//category master
$("#lnkCreateCategory").click(function () {
    $("#divCreateUpdateCategory").css("display", "block");
    $("#categorylist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
});

$(".category-submit").click(function () {
    var categoryName = $('#txtCategory').val();
    var error = false;
    $(".text-danger").remove();

    if (categoryName.length < 1) {
        $('#txtCategory').after('<span class="text-danger">Category name is required</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//$(".btn-category-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});
//End

//sub category master
$("#lnkCreateSubCategory").click(function () {
    $("#divCreateUpdateSubCategory").css("display", "block");
    $("#subCategorylist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
});

$(".subCategory-submit").click(function () {

    var categoryName = $('#txtSubCategory').val();
    var categoryId = $('#selectCategory').val();
    var error = false;
    $(".text-danger").remove();

    if (categoryName.length < 1) {
        $('#txtSubCategory').after('<span class="text-danger">SubCategory name is required</span>');
        error = true;
    }
    if (categoryId < 1) {
        $('#divSelectCategory').after('<div class="select-margin"><span class="text-danger">Category is required</span></div>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//$(".btn-subCategory-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});
//End

//product master
$("#lnkCreateProduct").click(function () {
    $("#divCreateUpdateProduct").css("display", "block");
    $("#productlist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
});

$(".product-submit").click(function () {

    var product = $('#txtProduct').val();
    var brandId = $('#selectProductBrand').val();
    var categoryId = $('#selectProductCategory').val();
    var subCategoryId = $('#selectProductSubCategory').val();
    var error = false;
    $(".text-danger").remove();

    if (product.length < 1) {
        $('#txtProduct').after('<span class="text-danger">Product Code is required</span>');
        error = true;
    }
    //if (brandId < 1) {
    //    $('#selectProductBrand').after('<span class="text-danger">Brand is required</span>');
    //    error = true;
    //}
    //if (categoryId < 1) {
    //    $('#selectProductCategory').after('<span class="text-danger">Category is required</span>');
    //    error = true;
    //}
    //if (subCategoryId < 1) {
    //    $('#selectProductSubCategory').after('<span class="text-danger">SubCategory is required</span>');
    //    error = true;
    //}
    if (error == true) {
        return false;
    }
    else {
        return true;
    }

});

//$(".btn-product-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});
//End


//tax master
$("#lnkCreateTax").click(function () {
    $("#divCreateUpdateTax").css("display", "block");
    $("#taxlist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
});

$(".tax-submit").click(function () {
    var taxName = $('#txtTax').val();
    var error = false;
    $(".text-danger").remove();

    if (taxName.length < 1) {
        $('#txtTax').after('<span class="text-danger">Tax name is required</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//$(".btn-tax-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});
//End

//supplier master
$("#lnkCreateSupplier").click(function () {
    $("#divCreateUpdateSupplier").css("display", "block");
    $("#supplierlist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
});

$(".supplier-submit").click(function () {
    var supplierName = $('#txtSupplier').val();
    var email = $('#txtEmail').val();
    var error = false;
    $(".text-danger").remove();

    if (supplierName.length < 1) {
        $('#txtSupplier').after('<span class="text-danger">Supplier name is required</span>');
        error = true;
    }

    if (!email.match(emailReg)) {
        $('#txtEmail').after('<span class="text-danger">Enter valid Email address</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//$(".btn-supplier-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});
//End

//role master
$("#lnkCreateRole").click(function () {
    $("#divCreateUpdateRole").css("display", "block");
    $("#rolelist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
});

$(".role-submit").click(function () {
    var roleName = $('#txtRole').val();
    var error = false;
    $(".text-danger").remove();

    if (roleName.length < 1) {
        $('#txtRole').after('<span class="text-danger">Role name is required</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//$(".btn-role-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});
//End

//admin master
$("#lnkCreateAdmin").click(function () {
    $("#divCreateUpdateAdmin").css("display", "block");
    $("#adminlist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
    $("#txtIsEmailSubscribed").prop('checked', true);
});

$(".admin-submit").click(function (e) {
    var firstName = $('#txtAdminFirstName').val();
    var LastName = $('#txtAdminLastName').val();
    var password = $('#txtAdminPassword').val();
    var confirmPassword = $('#txtAdminConfirmPassword').val();
    var email = $('#txtAdminEmail').val();
    var roleId = $('#selectRole').val();
    var error = false;
    var isEditMode = $('#hdnAdminID').length > 0 && $('#hdnAdminID').val() > 0;
    var isPasswordChangeEnabled = $('#chkChangePassword').length > 0 && $('#chkChangePassword').is(':checked');
    
    $(".text-danger").remove();
    
    // If in edit mode and password change is not enabled, clear the password field to prevent null submission
    if (isEditMode && !isPasswordChangeEnabled) {
        $('#txtAdminPassword').removeAttr('name');
        $('#txtAdminConfirmPassword').removeAttr('name');
    } else {
        $('#txtAdminPassword').attr('name', 'objAdmin.Password');
    }

    if (!email.match(emailReg)) {
        $('#txtAdminEmail').after('<span class="text-danger">Enter valid Email address</span>');
        error = true;
    }

    if (firstName.length < 1) {
        $('#txtAdminFirstName').after('<span class="text-danger">First name is required</span>');
        error = true;
    }

    if (LastName.length < 1) {
        $('#txtAdminLastName').after('<span class="text-danger">Last name is required</span>');
        error = true;
    }

    if (roleId < 1) {
        $('#selectRole').after('<span class="text-danger">Role is required</span>');
        error = true;
    }

    // Password validation - only validate if creating new admin or changing password in edit mode
    if (!isEditMode || isPasswordChangeEnabled) {
        if (password.length < 6) {
            $('#txtAdminPassword').after('<span class="text-danger">Password must be at least 6 characters long</span>');
            error = true;
        }
        
        if (password !== confirmPassword) {
            $('#txtAdminConfirmPassword').after('<span class="text-danger">Passwords do not match</span>');
            error = true;
        }
        
        if (password.length > 0 && !password.match(passwordReg)) {
            $('#txtAdminPassword').after('<span class="text-danger">Password must contain 5 to 25 characters with at least one numeric digit and a special character</span>');
            error = true;
        }
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//$(".btn-admin-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});
//End

//customer master
$("#lnkCreateCustomer").click(function () {
    $("#divCreateUpdateCustomer").css("display", "block");
    $("#customerlist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
    $("#txtEmailSubscribed").prop('checked', true);
});

$(".customer-submit").click(function (e) {
    var firstName = $('#txtCustomerFirstName').val();
    var LastName = $('#txtCustomerLastName').val();
    var password = $('#txtCustomerPassword').val();
    var confirmPassword = $('#txtCustomerConfirmPassword').val();
    var email = $('#txtCustomerEmail').val();
    var roleId = $('#selectRole').val();
    var error = false;
    var isEditMode = $('#hdnCustomerID').length > 0 && $('#hdnCustomerID').val() > 0;
    var isPasswordChangeEnabled = $('#chkChangeCustomerPassword').length > 0 && $('#chkChangeCustomerPassword').is(':checked');
    
    $(".text-danger").remove();
    
    // If in edit mode and password change is not enabled, clear the password field to prevent null submission
    if (isEditMode && !isPasswordChangeEnabled) {
        $('#txtCustomerPassword').removeAttr('name');
        $('#txtCustomerConfirmPassword').removeAttr('name');
    } else {
        $('#txtCustomerPassword').attr('name', 'objCustomer.Password');
    }

    if (!email.match(emailReg)) {
        $('#txtCustomerEmail').after('<span class="text-danger">Enter valid Email address</span>');
        error = true;
    }

    if (firstName.length < 1) {
        $('#txtCustomerFirstName').after('<span class="text-danger">First name is required</span>');
        error = true;
    }

    if (LastName.length < 1) {
        $('#txtCustomerLastName').after('<span class="text-danger">Last name is required</span>');
        error = true;
    }

    if (roleId < 1) {
        $('#selectRole').after('<span class="text-danger">Role is required</span>');
        error = true;
    }

    // Password validation - only validate if creating new customer or changing password in edit mode
    if (!isEditMode || isPasswordChangeEnabled) {
        if (password.length < 6) {
            $('#txtCustomerPassword').after('<span class="text-danger">Password must be at least 6 characters long</span>');
            error = true;
        }
        
        if (password !== confirmPassword) {
            $('#txtCustomerConfirmPassword').after('<span class="text-danger">Passwords do not match</span>');
            error = true;
        }
        
        if (password.length > 0 && !password.match(passwordReg)) {
            $('#txtCustomerPassword').after('<span class="text-danger">Password must contain 5 to 25 characters with at least one numeric digit and a special character</span>');
            error = true;
        }
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//$(".btn-customer-delete").click(function () {
//    var result = confirm("Want to delete?");
//    if (result) {
//        return true;
//    }
//    else
//        return false;
//});
//End
//UploadDownload
$(".UploadDownload-submit").click(function () {
    var fileInput = $('#productFileUpload').val();
    var error = false;
    $(".text-danger").remove();

    if (fileInput.length < 1) {
        $('#productFileUpload').after('<span class="text-danger">Select the file to upload, Only xls, xlsx are allowed</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
//End
//Logout
$('ul li.dropdown').hover(function () {
    $(this).find('.dropdown-menu').stop(true, true).delay(100).fadeIn(200);
}, function () {
    $(this).find('.dropdown-menu').stop(true, true).delay(100).fadeOut(200);
});
//End

//Load Menu
// Icon mapping for menu items
var menuIcons = {
    'Brand': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z" clip-rule="evenodd"></path></svg>',
    'Category': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path d="M7 3a1 1 0 000 2h6a1 1 0 100-2H7zM4 7a1 1 0 011-1h10a1 1 0 110 2H5a1 1 0 01-1-1zM2 11a2 2 0 012-2h12a2 2 0 012 2v4a2 2 0 01-2 2H4a2 2 0 01-2-2v-4z"></path></svg>',
    'SubCategory': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path d="M3 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h6a1 1 0 110 2H4a1 1 0 01-1-1zM3 16a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"></path></svg>',
    'Product': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 2L3 7v11a1 1 0 001 1h12a1 1 0 001-1V7l-7-5zM6 9a1 1 0 112 0v6a1 1 0 11-2 0V9zm6 0a1 1 0 112 0v6a1 1 0 11-2 0V9z" clip-rule="evenodd"></path></svg>',
    'Tax': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4 4a2 2 0 00-2 2v4a2 2 0 002 2V6h10a2 2 0 00-2-2H4zm2 6a2 2 0 012-2h8a2 2 0 012 2v4a2 2 0 01-2 2H8a2 2 0 01-2-2v-4zm6 4a2 2 0 100-4 2 2 0 000 4z" clip-rule="evenodd"></path></svg>',
    'Supplier': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>',
    'Role': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M6 6V5a3 3 0 013-3h2a3 3 0 013 3v1h2a2 2 0 012 2v3.57A22.952 22.952 0 0110 13a22.95 22.95 0 01-8-1.43V8a2 2 0 012-2h2zm2-1a1 1 0 011-1h2a1 1 0 011 1v1H8V5zm1 5a1 1 0 011-1h.01a1 1 0 110 2H10a1 1 0 01-1-1z" clip-rule="evenodd"></path><path d="M2 13.692V16a2 2 0 002 2h12a2 2 0 002-2v-2.308A24.974 24.974 0 0110 15c-2.796 0-5.487-.46-8-1.308z"></path></svg>',
    'Admin': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-6-3a2 2 0 11-4 0 2 2 0 014 0zm-2 4a5 5 0 00-4.546 2.916A5.986 5.986 0 0010 16a5.986 5.986 0 004.546-2.084A5 5 0 0010 11z" clip-rule="evenodd"></path></svg>',
    'Customer': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd"></path></svg>',
    'Import': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M3 4a1 1 0 011-1h4a1 1 0 010 2H6.414l2.293 2.293a1 1 0 11-1.414 1.414L5 6.414V8a1 1 0 01-2 0V4zm9 1a1 1 0 010-2h4a1 1 0 011 1v4a1 1 0 01-2 0V6.414l-2.293 2.293a1 1 0 11-1.414-1.414L13.586 5H12zm-9 7a1 1 0 012 0v1.586l2.293-2.293a1 1 0 111.414 1.414L6.414 15H8a1 1 0 010 2H4a1 1 0 01-1-1v-4zm13-1a1 1 0 011 1v4a1 1 0 01-1 1h-4a1 1 0 010-2h1.586l-2.293-2.293a1 1 0 111.414-1.414L15 13.586V12a1 1 0 011-1z" clip-rule="evenodd"></path></svg>',
    'Export': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm3.293-7.707a1 1 0 011.414 0L9 10.586V3a1 1 0 112 0v7.586l1.293-1.293a1 1 0 111.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z" clip-rule="evenodd"></path></svg>',
    'Base Cost': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z"></path><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd"></path></svg>',
    'Promotion Cost': '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M12.395 2.553a1 1 0 00-1.45-.385c-.345.23-.614.558-.822.88-.214.33-.403.713-.57 1.116-.334.804-.614 1.768-.84 2.734a31.365 31.365 0 00-.613 3.58 2.64 2.64 0 01-.945-1.067c-.328-.68-.398-1.534-.398-2.654A1 1 0 005.05 6.05 6.981 6.981 0 003 11a7 7 0 1011.95-4.95c-.592-.591-.98-.985-1.348-1.467-.363-.476-.724-1.063-1.207-2.03zM12.12 15.12A3 3 0 017 13s.879.5 2.5.5c0-1 .5-4 1.25-4.5.5 1 .786 1.293 1.371 1.879A2.99 2.99 0 0113 13a2.99 2.99 0 01-.879 2.121z" clip-rule="evenodd"></path></svg>'
};

var getMenuItem = function (itemData) {
    var menuIcon = menuIcons[itemData.name] || '<svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20"><path d="M10 12a2 2 0 100-4 2 2 0 000 4z"></path><path fill-rule="evenodd" d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z" clip-rule="evenodd"></path></svg>';

    var item = "";
    if (itemData.SubMenu) {
        // Parent menu item with submenu
        item = $("<li>", {
            class: 'menu-item',
            id: itemData.id
        }).append(
            $("<a>", {
                href: itemData.link,
                class: 'menu-link dropdown-toggle',
                id: itemData.id + '-links',
                "data-toggle": "collapse",
                "aria-expanded": "false"
            }).append(
                $("<div>", {
                    class: 'menu-icon',
                    html: menuIcon
                })
            ).append(
                $("<span>", {
                    class: 'menu-text',
                    html: itemData.name
                })
            ).append(
                $("<svg>", {
                    class: 'dropdown-arrow',
                    width: '16',
                    height: '16',
                    fill: 'currentColor',
                    viewBox: '0 0 20 20',
                    html: '<path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd"></path>'
                })
            )
        );
    } else {
        // Regular menu item
        item = $("<li>", {
            class: 'menu-item',
            id: itemData.id
        }).append(
            $("<a>", {
                href: itemData.link,
                class: 'menu-link',
                id: itemData.id + '-links'
            }).append(
                $("<div>", {
                    class: 'menu-icon',
                    html: menuIcon
                })
            ).append(
                $("<span>", {
                    class: 'menu-text',
                    html: itemData.name
                })
            )
        );
    }

    // Add submenu if exists
    if (itemData.SubMenu) {
        var subList = $("<ul>", {
            class: 'collapse list-unstyled submenu',
            id: itemData.name.replace(/\s+/g, '') + "SubMenu"
        });
        $.each(itemData.SubMenu, function () {
            var subItem = getMenuItem(this);
            subItem.find('.menu-link').addClass('submenu-link');
            subList.append(subItem);
        });
        item.append(subList);
    }

    return item;
};



var $menu = $("#mainmenu");
$.getJSON("Menu/Menu.json", function (data) {
    $.each(data.mainmenu, function (index, data) {
        if (data.roleid == $("#hdnRoleId").val()) {
            $.each(data.menu, function (index, data) {
                $menu.append(getMenuItem(data));
            });
        }
    });

    // Submenu toggle handlers are now handled in modern-interactions.js
    // Removed duplicate event handlers to prevent conflicts

    // Load active menu after menu is built
    setTimeout(function () {
        loadActiveMenu();
    }, 500);
});
//End
$(document).ready(function () {
    loadActiveMenu();
    $(window).trigger('resize');

});
// Sidebar toggle is handled in modern-interactions.js
// No duplicate event handlers needed here

function loadActiveMenu() {
    var currentPath = window.location.pathname;
    var url = currentPath.split('/').pop() || currentPath;

    // Remove active class from all menu items
    $('.menu-link').removeClass('active');
    $('.menu-item').removeClass('active');

    // Handle special cases for submenu items
    if (url === 'all-promotion-cost' || url === 'all-product' || url === 'all-base-cost') {
        // Activate parent Product menu and expand submenu
        var productMenu = $('.menu-link[data-toggle="collapse"]');
        if (productMenu.length) {
            productMenu.addClass('active');
            productMenu.attr('aria-expanded', 'true');

            // Find and show the submenu
            var submenu = productMenu.parent().find('.submenu, .collapse');
            submenu.addClass('show').show();

            // Also activate the specific submenu item
            // setTimeout(function () {
            $('.submenu-link[href*="' + url + '"]').addClass('active');
            //  }, 100);

        }
        // Update page title based on active menu
        const submenuLink = document.querySelector(`.submenu-link[href*="${url}"]`);
        var activeMenu = $('.submenu-link.active .menu-text').first();
        if (submenuLink && $('#currentPageTitle').length) {
            $('#currentPageTitle').text(submenuLink.textContent);
        }
    } else {
        // Find and activate the matching menu item
        var matchingLink = $('.menu-link[href*="' + url + '"]').first();
        if (matchingLink.length) {
            matchingLink.addClass('active');
        } else {
            // Special handling for UploadDownload page (default after login)
            if (url.toLowerCase() === 'import' || url.toLowerCase() === '') {
                var importMenu = $('.menu-text:contains("Import")').parent();
                if (importMenu.length) {
                    importMenu.addClass('active');
                }
            } else {
                // Try to match by page name patterns
                var pagePatterns = {
                    'Brand': ['brand', 'all-brand'],
                    'Category': ['category', 'all-category'],
                    'SubCategory': ['subcategory', 'all-subcategory'],
                    'Product': ['product', 'all-product'],
                    'Base Cost': ['basecost', 'all-base-cost'],
                    'Promotion Cost': ['promotioncost', 'all-promotion-cost'],
                    'Tax': ['tax', 'all-tax'],
                    'Supplier': ['supplier', 'all-supplier'],
                    'Role': ['role', 'all-role'],
                    'Admin': ['admin', 'all-admin'],
                    'Customer': ['customer', 'all-customer'],
                    'Import': ['import', 'upload', 'import'],
                    'Export': ['export', 'export']
                };

                for (var menuName in pagePatterns) {
                    var patterns = pagePatterns[menuName];
                    for (var i = 0; i < patterns.length; i++) {
                        if (url.toLowerCase() == patterns[i].toLowerCase()) {
                            $('.menu-text').filter(function() {
                                return $(this).text().trim() === menuName;
                            }).parent().addClass('active');
                            break;
                        }
                    }
                }
            }
        }
        // Update page title based on active menu
        var activeMenu = $('.menu-link.active .menu-text').first();
        if (activeMenu.length && $('#currentPageTitle').length) {
            $('#currentPageTitle').text(activeMenu.text());
        }
    }


}

//Cascade dropdown
//$(function () {
//    $("#selectProductBrand").on("change", function () {
//        var brandId = $(this).val();
//        $("#selectProductCategory").empty();
//        $("#selectProductSubCategory").empty();
//        $("#selectProductCategory").append("<option value=''>-- Select Category --</option>");
//        $("#selectProductSubCategory").append("<option value=''>-- Select SubCategory --</option>");
//        $.getJSON(`?handler=Categories&brandId=${brandId}`, (data) => {
//            $.each(data, function (i, item) {
//                $("#selectProductCategory").append(`<option value="${item.id}">${item.name}</option>`);
//            });
//        });
//    });
//});

//$(function () {
//    $("#selectProductCategory").on("change", function () {
//        var categoryId = $(this).val();
//        $("#selectProductSubCategory").empty();
//        $("#selectProductSubCategory").append("<option value=''>-- Select SubCategory --</option>");
//        $.getJSON(`?handler=SubCategories&categoryId=${categoryId}`, (data) => {
//            $.each(data, function (i, item) {
//                $("#selectProductSubCategory").append(`<option value="${item.id}">${item.name}</option>`);
//            });
//        });
//    });
//});
//End

//Number only
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
//End

//Decimal Only
function isDecimal(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46) {
        return false;
    }
    return true;
}

// Function to validate positive decimal numbers with up to 2 decimal places
function validatePositiveDecimal(input) {
    // Get cursor position before any changes
    const cursorPosition = input.selectionStart;
    let value = input.value;
    
    // Remove any non-numeric characters except the first decimal point
    let newValue = value.replace(/[^0-9.]/g, '');
    
    // Handle multiple decimal points
    const parts = newValue.split('.');
    if (parts.length > 2) {
        // If more than one decimal point, keep only the first one
        newValue = parts[0] + '.' + parts.slice(1).join('');
    }
    
    // Limit to 2 decimal places
    if (parts.length === 2 && parts[1].length > 2) {
        newValue = parts[0] + '.' + parts[1].substring(0, 2);
    }
    
    // Prevent leading zeros (e.g., 001.23 becomes 1.23)
    if (parts[0] && parts[0].length > 1 && parts[0].startsWith('0') && !parts[0].startsWith('0.')) {
        newValue = parseFloat(newValue).toString();
    }
    
    // Update the input value
    input.value = newValue;
    
    // Maintain cursor position
    const diff = newValue.length - value.length;
    const newCursorPosition = Math.max(0, Math.min(cursorPosition + diff, newValue.length));
    
    // Use setTimeout to ensure the cursor position is set after the value is updated
    setTimeout(() => {
        input.setSelectionRange(newCursorPosition, newCursorPosition);
    }, 0);
    
    // Validate the final value
    const numericValue = parseFloat(newValue);
    if (isNaN(numericValue) || numericValue < 0) {
        input.value = '';
    }
    
    return true;
}

// Add event listener for decimal input fields
document.addEventListener('DOMContentLoaded', function() {
    const decimalInputs = document.querySelectorAll('input[type="number"][onkeypress*="validatePositiveDecimal"]');
    decimalInputs.forEach(input => {
        // Replace onkeypress with oninput for better real-time validation
        input.removeAttribute('onkeypress');
        input.addEventListener('input', function(e) {
            validatePositiveDecimal(this);
        });
    });
});

//End

// Initialize date picker
document.addEventListener('DOMContentLoaded', function () {
    const expiryDateInput = document.querySelector(".datepicker"); //document.getElementById('datepicker');

    // Format the date for display when the page loads
    if (expiryDateInput && expiryDateInput.value) {
        const date = new Date(expiryDateInput.value);
        if (!isNaN(date.getTime())) {
            expiryDateInput.value = date.toISOString().split('T')[0];
        }
    } else {
        // Set min date to today if no value is set
        expiryDateInput.min = new Date().toISOString().split('T')[0];
    }

    // Add input validation
    if (expiryDateInput) {
        expiryDateInput.addEventListener('change', function () {
            const selectedDate = new Date(this.value);
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            if (selectedDate < today) {
                alert('Expiry date cannot be in the past');
                this.value = '';
            }
        });
    }
});

function toggleTaxSlab() {
    var checkBox = document.getElementById("txtIsTaxable");
    var dropdown = document.querySelector(".divTax");
    var selectElement = document.getElementById("selectProductTax");

    if (checkBox.checked) {
        dropdown.style.display = "block";
        if (selectElement) {
            selectElement.disabled = false;
        }
    } else {
        dropdown.style.display = "none";
        if (selectElement) {
            selectElement.disabled = true;
            selectElement.value = "";
        }
    }
}

// Promotion Cost file validation function
function promotionCostFileValidation() {
    var fileInput = document.getElementById('productFileUpload');
    var selectedFileName = document.getElementById('selectedPromotionFileName');

    var filePath = fileInput.value;
    if (filePath == '') {
        // Hide file name display when no file is selected
        if (selectedFileName) {
            selectedFileName.style.display = 'none';
        }
        return true;
    }
    // Allowing file type
    var allowedExtensions = /(\.xls|\.xlsx)$/i;

    if (!allowedExtensions.exec(filePath)) {
        alert('Invalid file type, Only xls, xlsx are allowed');
        fileInput.value = '';
        // Hide file name display when file is cleared
        if (selectedFileName) {
            selectedFileName.style.display = 'none';
        }
        return false;
    }
    else {
        // Show selected file name
        if (selectedFileName && fileInput.files && fileInput.files[0]) {
            var fileNameText = selectedFileName.querySelector('.file-name-text');
            if (fileNameText) {
                fileNameText.textContent = fileInput.files[0].name;
                selectedFileName.style.display = 'flex';
            }
        }
        return true;
    }
}

// Upload Download file validation function
function uploadDownloadFileValidation() {
    var fileInput = document.getElementById('productFileUpload');
    var selectedFileName = document.getElementById('selectedUploadFileName');

    var filePath = fileInput.value;
    if (filePath == '') {
        // Hide file name display when no file is selected
        if (selectedFileName) {
            selectedFileName.style.display = 'none';
        }
        return true;
    }
    // Allowing file type
    var allowedExtensions = /(\.xls|\.xlsx)$/i;

    if (!allowedExtensions.exec(filePath)) {
        alert('Invalid file type, Only xls, xlsx are allowed');
        fileInput.value = '';
        // Hide file name display when file is cleared
        if (selectedFileName) {
            selectedFileName.style.display = 'none';
        }
        return false;
    }
    else {
        // Show selected file name
        if (selectedFileName && fileInput.files && fileInput.files[0]) {
            var fileNameText = selectedFileName.querySelector('.file-name-text');
            if (fileNameText) {
                fileNameText.textContent = fileInput.files[0].name;
                selectedFileName.style.display = 'flex';
            }
        }
        return true;
    }
}

// Clear selected file functions
function clearPromotionSelectedFile() {
    var fileInput = document.getElementById('productFileUpload');
    var selectedFileName = document.getElementById('selectedPromotionFileName');
    
    fileInput.value = '';
    selectedFileName.style.display = 'none';
}

function clearUploadSelectedFile() {
    var fileInput = document.getElementById('productFileUpload');
    var selectedFileName = document.getElementById('selectedUploadFileName');
    
    fileInput.value = '';
    selectedFileName.style.display = 'none';
}

