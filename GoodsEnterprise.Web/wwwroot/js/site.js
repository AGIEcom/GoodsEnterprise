// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

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

//document.getElementById('editButton').onClick = function () {
//    deliveryServiceClick();
//};

//function deliveryServiceClick() {
//    var dialogDiv = $('#dialogDiv');

//    if (dialogDiv.length == 0) {
//        dialogDiv = $("<div id='dialogDiv'><div/>").appendTo('body');
//        $('#divCreateUpdateBrand').appendTo(dialogDiv).removeClass('hide')
//        dialogDiv.attr("Title", "Please select your chosen delivery service.");


//    } else {
//        dialogDiv.dialog("open");
//    }
//}

//$(".brand-clear").click(function () {
//    $("#txtBrand").val("");
//    $("#txtDescription").val("");
//    $("#txtIsActive").prop("checked", "false");
//});

//brand master end