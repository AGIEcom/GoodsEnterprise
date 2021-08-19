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