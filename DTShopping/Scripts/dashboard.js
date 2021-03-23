﻿$(document).ready(function () {
    $(".preloader").hide();

        $('#addToCart').click(function (e) {
        var prodId = $("#product_id").val();
        var quantity = $("#quantity").val();
        var size = 0;
        if ($("#sizeDropDown").length > 0) {
            size = $("#sizeDropDown").val();
        }
        var color = 0;
        if ($("#colorDropDown").length > 0) {
            color = $("#colorDropDown").val();
        }

        AddProductInCart(prodId, quantity, true, size, color);
    });
});

function checkPoints() {
    var chkId = "";
    var checkBoxIdsList = $("#checkBoxIds").val();

    if (checkBoxIdsList != "" || checkBoxIdsList != undefined) {
        $("input:checkbox[name=pointsCheckBox]").each(function () {

            //alert("Id: " + $(this).attr("id") + " Value: " + $(this).val());
            chkId = $(this).attr("id");
            if (checkBoxIdsList.indexOf(chkId) > -1) {
                $(this).attr('checked', true);
            }
        });
    }
}
function LoginRegisterCalls() {
    Login_Account();
}

function GetCityByState() {
    var id = $("#stateList").val();
    $(".preloader").show();
    $.ajax({
        url: '/Account/GetCityListByState',
        type: 'Post',
        datatype: 'Json',
        data: { Id: id }
    }).done(function (result) {
        if (result == null || result == undefined || result == "") {
            $("#error").html(result);
        }
        else {
            $("#cityList").html("");
            $.each(result, function (key, value) {
                $("#cityList").append($("<option></option>").val(value.cityID).html(value.cityName));
            });
        }

        $(".preloader").hide();
    });
}

function SaveDetailForm() {
    $("#loginError").html("");
    var loginDetail = $('#addForm').serialize();
    $(".preloader").show();
    $.ajax({
        url: '/Account/SaveDetail',
        type: 'Post',
        datatype: 'Json',
        data: loginDetail
    }).done(function (result) {

        $("#loginError").html(result);
        $("#loginError1").html(result);
        $(".preloader").hide();
        $("#closeError").show();

    }).fail(function (error) {
        $("#loginError").html(error.statusText);
        $(".preloader").hide();
        $("#closeError").show();
        $(".preloader").hide();
    });

    return false;
}

function UpdateAccount() {
    $("#loginError").html("");
    var oldPassword = $("#oldPassword").val();
    var password_str = $("#password_str").val();
    var validateOldPassword = true;
    $("#ErrorMessage").html("");
    if (oldPassword != password_str) {
        validateOldPassword = false;
        $("#ErrorMessage").html("<br/><br/>Old password is not correct.");
    }
    if (validateOldPassword == true) {
        var validateConfirm = true;
        var newPassword = $("#new_password_str").val();
        var ConfirmPassword = $("#ConfirmPassword").val();
        if (newPassword != ConfirmPassword) {
            validateConfirm = false;
            $("#ErrorMessage").html("<br/><br/>New password and Confirm password do not match.");
        }
        else {

            $("#password_str").val(newPassword);
            var loginDetail = $('#update_account').serialize();
            $(".preloader").show();
            $.ajax({
                url: '/Home/UpdateAccount',
                type: 'Post',
                datatype: 'Json',
                data: loginDetail
            }).done(function (result) {
                alert(result);
                $(".preloader").hide();
            }).fail(function (error) {
                alert(error.statusText);
                $(".preloader").hide();
                $(".preloader").hide();
            });
        }
    }

    return false;
}

function Login_Account() {
    $("#loginError").html("");
    var loginDetail = $('#addForm').serialize();
    $(".preloader").show();
    $.ajax({
        url: '/Account/Login',
        type: 'Post',
        datatype: 'Json',
        data: loginDetail
    }).done(function (result) {
        if (result == "Success") {
            window.location.href = "/Home/Index";
        }
        else {
            $("#loginError").html(result);
            $("#loginError2").html(result);
        }
        $(".preloader").hide();
        $("#closeError").show();
    }).fail(function (error) {
        $("#loginError").html(error.statusText);
        $(".preloader").hide();
        $("#closeError").show();
        $(".preloader").hide();
    });

    return false;
}



function Other_Register() {
    debugger;
    $("#loginError").html("");
    var registerDetail = $('#addForm').serialize();
    $(".preloader").show();
    $.ajax({
        url: '/Account/SaveOtherRegister',
        type: 'Post',
        datatype: 'Json',
        data: registerDetail
    }).done(function (result) {
        debugger;
        var obj = $.parseJSON(result);
        if (obj.response == "OK") {
            alert("Registered Successfully!! your ID No. is  " + obj.idno + " and password " + obj.password + "");
        }
        else {
            alert("invalid pincode");
        }
        //if (result == "Success") {
        //    window.location.href = "/Home/Index";
        //}
        //else {
        //    $("#loginError").html(result);
        //    $("#loginError2").html(result);
        //}
        $(".preloader").hide();
        $("#closeError").show();
    }).fail(function (error) {
        $("#loginError").html(error.statusText);
        $(".preloader").hide();
        $("#closeError").show();
        $(".preloader").hide();
    });

    return false;
}

function callDetailPage(productID) {
    window.location.href = "/Home/GetProductDetail?prodId=" + productID;
}