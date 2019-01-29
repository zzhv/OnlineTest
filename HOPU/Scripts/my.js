//点击清空跳转输入框
$(function () {
    $("#tiaozhuan").on("click", function () {
        $("#tiaozhuan").val("");
    })
})

//指定跳转
$(function () {
    $("#tiaozhuan").on("keyup", function () {
        $("input[name='answerIteam']").removeAttr("disabled");
        $("#answerIsRight").css("display", "none");
        $("#isAutoNextTopic").val(0);
        if ($("#tiaozhuan").val() <= 0 * 1) {
            $("#tiaozhuan").val("1")
        }
        $.ajax({
            type: "post",
            url: "AllBrowse",
            data:
            {
                Tid: $("#tiaozhuan").val()
            },
            datatype: "json",
            success: function (data) {
                for (var i = 0, length = data.length; i < length; i++) {
                    $("#topicId").text(data[i].TopicID);
                    $("#thisTopicId").val(data[i].TopicID);
                    switch (data[i].TopicID * 1) {
                        case $("#topicCount").text() * 1:
                            $("#nextpage").attr("disabled", "disabled")
                            break;
                        case 1:
                            $("#lastpage").attr("disabled", "disabled")
                            break;
                        default:
                            $("button[name='lastOrnextBtn']").removeAttr("disabled");
                    }
                    $("#tittle").text(data[i].Title);
                    if (data[i].Answer.length < 2) {//如果不是多选题
                        $("input[name='answerIteam']").attr("type", "radio");
                        $("#topicType").text("单选题");
                    } else {
                        $("input[name='answerIteam']").attr("type", "checkbox");
                        $("#topicType").text("多选题");
                    }
                    $("#labelanswer1").text("A：" + data[i].AnswerA);
                    $("#labelanswer2").text("B：" + data[i].AnswerB);
                    $("#labelanswer3").text("C：" + data[i].AnswerC);
                    $("#labelanswer4").text("D：" + data[i].AnswerD);
                    $("#answer").text(data[i].Answer);
                }
                $("input[name='answerIteam']").prop("checked", false);
            }
        });

    })
});



//上、下一题
$(function () {
    $("button[name='lastOrnextBtn']").on('click', function () {
        var checkedNum = $(this).val();
        $("input[name='answerIteam']").removeAttr("disabled");
        $("#answerIsRight").css("display", "none");
        $("#isAutoNextTopic").val(0);
        //if ($("#tiaozhuan").val() == "") {
        //    /* $("#showDiv").empty();*///清空标记类容
        //    return;
        //} else {
        $.ajax({
            type: "post",
            url: "AllBrowse",
            data:
            {
                Tid: $("#thisTopicId").val() * 1 + (checkedNum * 1)
            },
            datatype: "json",
            success: function (data) {
                for (var i = 0, length = data.length; i < length; i++) {
                    $("#topicId").text(data[i].TopicID);
                    switch (data[i].TopicID * 1) {
                        case $("#topicCount").text() * 1:
                            $("#nextpage").attr("disabled", "disabled")
                            break;
                        case 1:
                            $("#lastpage").attr("disabled", "disabled")
                            break;
                        default:
                            $("button[name='lastOrnextBtn']").removeAttr("disabled");
                    }
                    $("#tittle").text(data[i].Title);
                    if (data[i].Answer.length < 2) {//如果不是多选题
                        $("input[name='answerIteam']").attr("type", "radio");
                        $("#topicType").text("单选题");
                    } else {
                        $("input[name='answerIteam']").attr("type", "checkbox");
                        $("#topicType").text("多选题");
                    }
                    $("#labelanswer1").text("A：" + data[i].AnswerA);
                    $("#labelanswer2").text("B：" + data[i].AnswerB);
                    $("#labelanswer3").text("C：" + data[i].AnswerC);
                    $("#labelanswer4").text("D：" + data[i].AnswerD);
                    $("#answer").text(data[i].Answer);
                    $("#tiaozhuan").val(data[i].TopicID);
                    $("#thisTopicId").val(data[i].TopicID);
                }
                $("input[name='answerIteam']").prop("checked", false);//清除所有按钮的选中
            }
        });
    })
})


