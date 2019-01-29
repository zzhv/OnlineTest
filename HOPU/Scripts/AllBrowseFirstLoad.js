
$(window).on('load', function () {
    //$("input").on('load', function () {
    $.ajax({
        type: "post",
        url: "AllBrowse",
        data:
        {
            Tid: "1"
        },
        datatype: "json",
        success: function (data) {
            for (var i = 0, length = data.length; i < length; i++) {
                $("#topicId").text(data[i].TopicID);
                $("#lastpage").attr("disabled", "disabled")//默认第一题禁止上一页
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
        }
    });
    //});
});

//校验答案
$(function () {
    $("input[name='answerIteam']").on('click', function () {
        var answer = "";
        if ($("input[name='answerIteam']").prop("type") == "radio") {
            var checkedAnswer = $("input[name='answerIteam']:checked").val();
            $.ajax({
                type: "post",
                url: "AllBrowse",
                data:
                {
                    Tid: $("#thisTopicId").val()
                },
                datatype: "json",
                success: function (data) {
                    for (var i = 0, length = data.length; i < length; i++) {
                        if (checkedAnswer === data[i].Answer) {
                            answerIsTrue();
                        }
                        else {
                            answerIsFalse();
                        }
                    }
                }
            })
        } else if ($("input[name='answerIteam']").prop("type") == "checkbox") {
            $.each($("input[name='answerIteam']:checked"), function () {
                answer += $(this).val();
            })
            var answerLenth = 0;
            if ($("#answer").text().length == answer.length) {
                answerLenth = $("#answer").text().length;
            }
            switch (answerLenth) {
                case 2:
                    answerIsTrueOrFalse(answer)
                    break;
                case 3:
                    answerIsTrueOrFalse(answer)
                    break;
                case 4:
                    answerIsTrueOrFalse(answer)
                    break;
            }
        }
    })
})

function answerIsTrueOrFalse(answer) {
    if (answer == $("#answer").text()) {
        answerIsTrue();
    } else {
        answerIsFalse();
    }
}

function answerIsTrue() {
    $("#answerIsRight").css("background", "#cce7c9");
    $("#answerIsRight").css("color", "#53bb48");
    $("#answerIsRight").html("恭喜你答对了");
    $("#isAutoNextTopic").val(1);//答对自动下一题
    $("input[name='answerIteam']").attr("disabled", "disabled");
    answerTrueNum(1)
    $("#answerIsRight").css("display", "block");
}

function answerIsFalse() {
    $("#answerIsRight").css("background", "#fcbabc");
    $("#answerIsRight").css("color", "#fd0000");
    $("#answerIsRight").html("很遗憾,你答错了，答案是：" + "<b>" + $("#answer").text() + "</b>");
    $("#isAutoNextTopic").val(0);
    $("input[name='answerIteam']").attr("disabled", "disabled");
    answerFalseNum(1);
    $("#answerIsRight").css("display", "block");
}

//答题信息
var TrueSum = 0;
var FalseSum = 0;
function answerTrueNum(num) {
    TrueSum += num;
    TruePercent(TrueSum, FalseSum)
    $("#TrueNum").text(TrueSum);
}

function answerFalseNum(num) {
    FalseSum += num;
    TruePercent(TrueSum, FalseSum)
    $("#FalseNum").text(FalseSum);
}

function TruePercent(TrueSum, FalseSum) {
    $("#TruePre").text(GetPercent(TrueSum, (TrueSum + FalseSum)));
}

function GetPercent(num, total) {
    /// <summary>
    /// 求百分比
    /// </summary>
    /// <param name="num">当前数</param>
    /// <param name="total">总数</param>
    num = parseFloat(num);
    total = parseFloat(total);
    if (isNaN(num) || isNaN(total)) {
        return "-";
    }
    return total <= 0 ? "0%" : (Math.round(num / total * 10000) / 100.00) + "%";
}



//是否显示答案按钮
$(function () {
    $("#showOrHidennAnswer").on('click', function () {
        if ($("#showOrHidennAnswer").val() * 1 == 1) {
            $("#showOrHidennAnswer").html("隐藏答案");
            $("#showOrHidennAnswer").removeClass("btn-danger");
            $("#showOrHidennAnswer").addClass("btn-warning");
            $("#showOrHidennAnswer").val(0);
            $("#answerBox").css("display", "block");
        } else {
            $("#showOrHidennAnswer").html("显示答案");
            $("#showOrHidennAnswer").addClass("btn-danger");
            $("#showOrHidennAnswer").removeClass("btn-warning");
            $("#showOrHidennAnswer").val(1);
            $("#answerBox").css("display", "none");
        }
    })
})

//方向键下一题
$(this).keydown(function (event) { //监听键盘按下时的事件
    //console.log(event.keyCode); //按下不同的按键，对应的event.keyCode也不同
    $("input[name='answerIteam']").removeAttr("disabled");
    $("#answerIsRight").css("display", "none");
    $("#isAutoNextTopic").val(0);
    var checkedNum = "";
    switch (event.keyCode) {
        case 37:
            var checkedNum = -1;
            break;
        case 39:
            var checkedNum = 1;
            break;
    }
    if (checkedNum * 1 == -1 || checkedNum * 1 == 1) {
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
                    switch (data[i].TopicID) {
                        case $("#topicCount").text():
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
                $("input[name='answerIteam']").prop("checked", false);
            }
        });
    }
});


//label hover
//$(function () {
//    $("label[name='labelanswer']").hover(function () {
//        $(this).css("background", "#70db93");
//        $(this).css("cursor", "pointer");
//    }, function () {
//        $(this).css("background", "white");
//    });
//});


$(function () {
    $("label[name='labelanswer']").hover(function () {
        $(this).css("background", "#0088cc");
        $(this).css("color", "#fff");
        $(this).css("cursor", "pointer");
    }, function () {
        $(this).css("color", "#000");
        $(this).css("background", "white");
    });
});


$(function () {
    $("#AutoNextTmpLabel").hover(function () {
        $(this).css("cursor", "pointer");
    });
});

//自动下一题
$(function () {
    $("#AutoNextTmp").on("click", function () {
        if ($("#AutoNextTmp").prop("checked")) {
            datijieshu();
        } else {
            clearTimeout(i);
        }
    })
})

function datijieshu() {
    if ($("#isAutoNextTopic").val() * 1 == 1) {
        $("#isAutoNextTopic").val(0);
        setTimeout(function () {
            $("input[name='answerIteam']").removeAttr("disabled");
            $("#answerIsRight").css("display", "none");
            $.ajax({
                type: "post",
                url: "AllBrowse",
                data:
                {
                    Tid: $("#thisTopicId").val() * 1 + 1
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
        }, 2000);
    }
    i = setTimeout("datijieshu()", 1000);
}

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


$("#answerBox").ready(function () {
    $('pre code').each(function (i, block) {
        hljs.highlightBlock(block);
    });
});
