function formSuccess(result) {
    if (result.Success) {
        location.reload();
        //window.location.href = "@Url.Action("Index", "Home")";
    } else {
        $("#failure").css("display", "none")
        $("#error").html('发布失败！' + result + '!');
        $("#error").css('display', 'block');
        //alert('添加失败！' + result);
    }
}

function formFailure() {
    $("#error").css('display', 'none');
    $("#failure").css("display", "block")
}



function deleteTest(result) {
    if (result.Success) {
        location.reload();
    } else {
    alert(result);
    }
}