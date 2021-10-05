
//var users = [];
//document.getElementById('btn_create').onclick = function () {
//    var GroupName = document.getElementById('GroupName').value;
//    var userCheckbox = document.getElementsByName('userCheckBox');
//    for (var user of userCheckbox) {
//        if (user.checked)
//            users.push(user.value);
//    }
//    $.ajax({
//        type: "POST",
//        url: "CreateGroup",
//        data: { Name: GroupName, UsersIDs: users },
//        success: function (data) {
//            $('#myModal').modal('hide');
//            $('.modal-backdrop').remove();
//            resetInput();
//        }
//    });
//}



function confirmGroup() {
    var unsaved = false;
    if ($("#GroupName").val() != "")
        unsaved = true;
    if (unsaved) {
        var flag = confirm("Create Group Not Saved. Are you Sure you want to leave with out saving the data?");
        if (flag) {
            $('#myModal').modal('hide');
            $('.modal-backdrop').remove();
            resetInput();
        }

    } else {
        $('#myModal').modal('hide');
        $('.modal-backdrop').remove();
        resetInput();
    }
};
function resetInput() {
    $("#GroupName").val("");
    $("#Search").val("");
    $("input:checkbox[name='UsersIDs']").prop('checked', false);
}