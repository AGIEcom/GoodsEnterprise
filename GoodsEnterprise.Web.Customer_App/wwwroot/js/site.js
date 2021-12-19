// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(document).ready(function ($) {
    $('#txtPassWordRegister').passtrength({
        minChars: 8,
        passwordToggle: true,
        tooltip: true
    });
   
    $('#btRegister').click(function () {
        if ($("#txtFirstName").val() == "") {
            $("#txtFirstName").focus();
            alert("Please enter first name");
            return false;
        }
        else if ($("#txtEmailRegister").val() == "") {
            $("#txtEmailRegister").focus();
            alert("Please enter email address");
            return false;
        }
        else if (!validateEmail($("#txtEmailRegister").val())) {
            $("#txtEmailRegister").focus();
            alert("Please enter valid email address");
            return false;
        }
        else if ($("#txtPassWordRegister").val() == "") {
            $("#txtPassWordRegister").focus();
            alert("Please enter Password");
            return false;
        }
        else if ($("#txtConfirmPassWordRegister").val() == "") {
            $("#txtConfirmPassWordRegister").focus();
            alert("Please enter confirm Password");
            return false;
        }
        else if ($("#txtPassWordRegister").val() !== $("#txtConfirmPassWordRegister").val()) {
            $("#txtConfirmPassWordRegister").focus();
            alert("The password and confirmation password do not match.");
            return false;
        }
        else if (!$('div.passtrengthMeter').hasClass('very-strong')) {
            $("#txtPassWordRegister").focus();
            alert("password does not meet the criteria. It should be Minimum 8 characters which contain at least one numeric digit and a special character");
            return false;
        }
        else {
            return true;
        }
    });
    function validateEmail($email) {
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        return emailReg.test($email);
    }

});