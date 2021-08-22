// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

//datatable initilaze
$(document).ready(function () {
    $('#tblbrandMaster').DataTable({
        'columnDefs': [{
            'targets': [3], /* column index */
            'orderable': false, /* true or false */ 
           
        }],
        "order": [], 
        lengthMenu: [5, 10, 20, 50],


    }); 
});

//end

// Write your Javascript code.
//brand master
$("#lnkCreateBrand").click(function () {
    $("#divCreateUpdateBrand").css("display", "block");
    $("#brandlist").css("display", "none");
});

$(".brand-submit").click(function () {
    if ($("#txtBrand").val() == "") {
        alert("The Name field is required.");
        $("#txtBrand").focus();
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

//category master
$("#lnkCreateCategory").click(function () {
    $("#divCreateUpdateCategory").css("display", "block");
    $("#categorylist").css("display", "none");
});

$(".category-submit").click(function () {
    if ($("#txtCategory").val() == "") {
        alert("The Name field is required.");
        $("#txtCategory").focus();
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

//sub category master
$("#lnkCreateSubCategory").click(function () {
    $("#divCreateUpdateSubCategory").css("display", "block");
    $("#subCategorylist").css("display", "none");
});

$(".subCategory-submit").click(function () {
    if ($("#txtSubCategory").val() == "") {
        alert("The Name field is required.");
        $("#txtSUBCategory").focus();
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
 
//brand master end