$(document).on("change", "#MakeID", function () {
    var _value = $(this).val();
    if (_value != null && _value != "") {
        $("#ModelID option").each(function (i, obj) {
            var _optionValue = $(obj).attr("data-parentID");
            if (_optionValue != null && _optionValue != "" && _value == _optionValue)
                $(obj).removeClass("hide");
            else if (!$(obj).hasClass("hide") && !$(obj).hasClass("dontHide"))
                $(obj).addClass("hide");
        })
    } else {
        $("#ModelID option").addClass("hide");
        $("#ModelID option.dontHide").removeClass("hide");
    }
    $("#ModelID option[value='']").prop("selected", true);
    $("#VehicleTrimID option[value='']").prop("selected", true);
});

$(document).on("change", "#ModelID", function () {
    var _value = $(this).val();
    if (_value != null && _value != "") {
        $("#VehicleTrimID option").each(function (i, obj) {
            var _optionValue = $(obj).attr("data-parentID");
            if (_optionValue != null && _optionValue != "" && _value == _optionValue)
                $(obj).removeClass("hide");
            else if (!$(obj).hasClass("hide") && !$(obj).hasClass("dontHide"))
                $(obj).addClass("hide");
        })
    } else {
        $("#VehicleTrimID option").addClass("hide");
        $("#VehicleTrimID option.dontHide").removeClass("hide");
    }
    $("#VehicleTrimID option[value='']").prop("selected", true);
});

$(document).on("click", "#list", function (event) {
    event.preventDefault();
    $('#products .item').addClass('list-group-item');
});

$(document).on("click", "#grid", function (event) {
    event.preventDefault();
    $('#products .item').removeClass('list-group-item');
    $('#products .item').addClass('grid-group-item');
});