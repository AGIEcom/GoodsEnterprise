// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
//datatable initilaze
$(document).ready(function () {
    $('#tblBrandMaster').DataTable({
        'columnDefs': [
            {'targets': [3], 'orderable': false},
            {'searchable': false, "targets": [1, 2, 3]}
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });

    $('#tblCategoryMaster').DataTable({
        'columnDefs': [
            { 'targets': [3], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });

    $('#tblSubCategoryMaster').DataTable({
        'columnDefs': [
            { 'targets': [3], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });

    $('#tblProductMaster').DataTable({
        'columnDefs': [
            { 'targets': [3], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });

    $('#tblTaxMaster').DataTable({
        'columnDefs': [
            { 'targets': [3], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });

    $('#tblSupplierMaster').DataTable({
        'columnDefs': [
            { 'targets': [3], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });

    $('#tblRoleMaster').DataTable({
        'columnDefs': [
            { 'targets': [2], 'orderable': false },
            { 'searchable': false, "targets": [1, 2] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });

    $('#tblAdminMaster').DataTable({
        'columnDefs': [
            { 'targets': [5], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3, 4, 5] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });

    $('#tblCustomerMaster').DataTable({
        'columnDefs': [
            { 'targets': [5], 'orderable': false },
            { 'searchable': false, "targets": [1, 2, 3, 4, 5] }
        ],
        "order": [],
        lengthMenu: [5, 10, 20, 50]
    });
});

//end

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
$(".btn-brand-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
});

function fileValidation() {
    var fileInput =
        document.getElementById('fileUpload');

    var filePath = fileInput.value;
    if (filePath == '')
    {
        return true;
    }
    // Allowing file type
    var allowedExtensions =
        /(\.jpg|\.jpeg|\.png)$/i;

    if (!allowedExtensions.exec(filePath)) {
        alert('Invalid file type, Only jpg, jpeg, png are allowed');
        fileInput.value = '';
        return false;
    }
    else
    {
        $("#imgPath").css("display", "none");
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
    var brandId = $('#selectBrand').val();
    var error = false;
    $(".text-danger").remove();

    if (categoryName.length < 1) {
        $('#txtCategory').after('<span class="text-danger">Category name is required</span>');
        error = true;
    }
    if (brandId < 1) {
        $('#divSelectBrand').after('<div class="select-margin"><span class="text-danger">Brand is required</span></div>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
$(".btn-category-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
});
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
        $('#selectCategory').after('<span class="text-danger">Category is required</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
$(".btn-subCategory-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
});
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
    if (brandId < 1) {
        $('#selectProductBrand').after('<span class="text-danger">Brand is required</span>');
        error = true;
    }
    if (categoryId < 1) {
        $('#selectProductCategory').after('<span class="text-danger">Category is required</span>');
        error = true;
    }
    if (subCategoryId < 1) {
        $('#selectProductSubCategory').after('<span class="text-danger">SubCategory is required</span>');
        error = true;
    }
    if (error == true) {
        return false;
    }
    else {
        return true;
    }

});

$(".btn-product-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
});
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
$(".btn-tax-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
});
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
$(".btn-supplier-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
});
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
$(".btn-role-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
});
//End

//admin master
$("#lnkCreateAdmin").click(function () {
    $("#divCreateUpdateAdmin").css("display", "block");
    $("#adminlist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
    $("#txtIsEmailSubscribed").prop('checked', true);
});

$(".admin-submit").click(function () {
    var firstName = $('#txtAdminFirstName').val();
    var LastName = $('#txtAdminLastName').val();
    var password = $('#txtAdminPassword').val();
    var email = $('#txtAdminEmail').val();
    var roleId = $('#selectRole').val();
    var error = false;
    $(".text-danger").remove();

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

    if (!password.match(passwordReg)) {
        $('#txtAdminPassword').after('<span class="text-danger">Password must contain </br> 5 to 25 characters which contain at least one numeric digit and a special character</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
$(".btn-admin-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
});
//End

//customer master
$("#lnkCreateCustomer").click(function () {
    $("#divCreateUpdateCustomer").css("display", "block");
    $("#customerlist").css("display", "none");
    $("#txtIsActive").prop('checked', true);
    $("#txtEmailSubscribed").prop('checked', true);
});

$(".customer-submit").click(function () {
    var firstName = $('#txtCustomerFirstName').val();
    var LastName = $('#txtCustomerLastName').val();
    var password = $('#txtCustomerPassword').val();
    var email = $('#txtCustomerEmail').val();
    var roleId = $('#selectRole').val();
    var error = false;
    $(".text-danger").remove();

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

    if (!password.match(passwordReg)) {
        $('#txtCustomerPassword').after('<span class="text-danger">Password must contain </br> 5 to 25 characters which contain at least one numeric digit and a special character</span>');
        error = true;
    }

    if (error == true) {
        return false;
    }
    else {
        return true;
    }
});
$(".btn-customer-delete").click(function () {
    var result = confirm("Want to delete?");
    if (result) {
        return true;
    }
    else
        return false;
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
var getMenuItem = function (itemData) {
    var item = $("<li>", {
        class: 'nav-item',
        id: itemData.id
    }).append(
        $("<a>", {
            href: itemData.link,
            class: 'nav-link text-dark',
            html: itemData.name,
            id: itemData.id + '-links',
        }));
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
});
//End

//Cascade dropdown
$(function () {
    $("#selectProductBrand").on("change", function () {
        var brandId = $(this).val();
        $("#selectProductCategory").empty();
        $("#selectProductSubCategory").empty();
        $("#selectProductCategory").append("<option value=''>-- Select Category --</option>");
        $("#selectProductSubCategory").append("<option value=''>-- Select SubCategory --</option>");
        $.getJSON(`?handler=Categories&brandId=${brandId}`, (data) => {
            $.each(data, function (i, item) {
                $("#selectProductCategory").append(`<option value="${item.id}">${item.name}</option>`);
            });
        });
    });
});

$(function () {
    $("#selectProductCategory").on("change", function () {
        var categoryId = $(this).val();
        $("#selectProductSubCategory").empty();
        $("#selectProductSubCategory").append("<option value=''>-- Select SubCategory --</option>");
        $.getJSON(`?handler=SubCategories&categoryId=${categoryId}`, (data) => {
            $.each(data, function (i, item) {
                $("#selectProductSubCategory").append(`<option value="${item.id}">${item.name}</option>`);
            });
        });
    });
});
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
//End
