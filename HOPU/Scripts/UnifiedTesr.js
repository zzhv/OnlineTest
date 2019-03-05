$(document).ready(function () {
    $('pre code').each(function (i, block) {
        hljs.highlightBlock(block);
    });
});

//倒计时
var kaishi = document.getElementById("startTime").value;  // $("#startTime").val();
var jieshu = document.getElementById("endTime").value;// $("#endTime").val();
var time_now_server, time_now_client, time_end, time_server_client, timerID, isTrue = true, score;
time_end = new Date(jieshu);//结束的时间
time_end = time_end.getTime();
time_now_server = new Date(kaishi);//开始的时间
time_now_server = time_now_server.getTime();

time_now_client = new Date();
time_now_client = time_now_client.getTime();

time_server_client = time_now_server - time_now_client;

var timerID = setTimeout("show_time()", 1000);

function show_time(Stop) {
    if (Stop == 1) {
        isTrue = false;
    }
    var timer = document.getElementById("timer");
    if (!timer) {
        return;
    }
    timer.innerHTML = time_server_client;

    var time_now, time_distance, str_time;
    var int_day, int_hour, int_minute, int_second;
    var time_now = new Date();
    time_now = time_now.getTime() + time_server_client;
    time_distance = time_end - time_now;
    if (time_distance > 0 && isTrue) {
        int_day = Math.floor(time_distance / 86400000)
        time_distance -= int_day * 86400000;
        int_hour = Math.floor(time_distance / 3600000)
        time_distance -= int_hour * 3600000;
        int_minute = Math.floor(time_distance / 60000)
        time_distance -= int_minute * 60000;
        int_second = Math.floor(time_distance / 1000)

        if (int_hour < 10)
            int_hour = "0" + int_hour;
        if (int_minute < 10)
            int_minute = "0" + int_minute;
        if (int_second < 10)
            int_second = "0" + int_second;
        str_time = int_hour + ":" + int_minute + ":" + int_second;
        timer.innerText = str_time;
        setTimeout("show_time()", 1000);
    }
    else {
        //时间结束，自动提交
        clearTimeout(timerID)
        timer.innerText = "结束";
        var AnswerArray = [];
        var TopicCount = $("#topicCount").val() * 1;
        var index = 0;
        if (isTrue) {

            for (var i = 1; i <= TopicCount; i++) {
                var lstInt = "";
                if ($("input[name='answerIteam-" + i + "']").attr('type') == 'checkbox') {
                    var items = document.getElementsByName("answerIteam-" + i);
                    for (var j = 0; j < items.length; j++) {
                        if (items[j].checked) {
                            lstInt = items[j].value + lstInt;
                        }
                    }
                    if (lstInt.length <= 0) {
                        alert("您还有未填项，无法提交!");
                        return false;
                    } else {
                        var newlstInt = lstInt.split("");       //分割字符串a为数组b
                        newlstInt.sort();
                        AnswerArray[index] = newlstInt.join("");
                        index++;
                    }
                } else {
                    var flag = $("input[name='answerIteam-" + i + "']:checked").val();
                    if (flag == null) {
                        alert("您还有未填项，无法提交!");
                        return false;
                    } else {
                        AnswerArray[index] = flag;
                        index++;
                    }
                }
            }
            //for (var i = 0; i < AnswerArray.length; i++) {
            //    console.log(AnswerArray[i]);
            //}
            var itemScore = 100 / AnswerArray.length;
            var sumScore = 0;
            $.ajax({
                type: "post",
                url: "UnifiedTest",
                data: {
                    Answer: AnswerArray,
                    UtId: $("#UtId").val() * 1
                },
                datatype: "json",
                success: function (data) {
                    for (var i = 0; i < data.length; i++) {
                        console.log("所选答案" + data[i].UserAnswer + "正确答案" + data[i].RealAnswer + "结果" + data[i].IsTrue);
                        if (data[i].IsTrue.toString() == "true") {
                            sumScore += itemScore;
                        }
                    }
                    console.log("总分" + Math.round(sumScore));
                }
            })
        }

    }
}

//改变边框
function liBorder(id) {
    var flag = $("input[name='answerIteam-" + id + "']:checked").val();
    //alert(flag);
    if (true != null) {
        document.getElementById("S_" + id).style.border = "1px solid #0088cc";
    } else {
        document.getElementById("S_" + id).style.border = "1px solid #000";
    }
}

//label hover
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
    $("input[name='answerIteam']").hover(function () {
        $(this).css("cursor", "pointer");
    });
});


//提交试卷
$(function () {
    $("#btnTijiao").on('click', function () {
        var AnswerArray = [];
        var TopicCount = $("#topicCount").val() * 1;
        var index = 0;
        for (var i = 1; i <= TopicCount; i++) {
            var lstInt = "";
            if ($("input[name='answerIteam-" + i + "']").attr('type') == 'checkbox') {
                var items = document.getElementsByName("answerIteam-" + i);
                for (var j = 0; j < items.length; j++) {
                    if (items[j].checked) {
                        lstInt = items[j].value + lstInt;
                    }
                }
                if (lstInt.length <= 0) {
                    alert("您还有未填项，无法提交!");
                    return false;
                } else {
                    var newlstInt = lstInt.split("");       //分割字符串a为数组b
                    newlstInt.sort();
                    AnswerArray[index] = newlstInt.join("");
                    index++;
                }
            } else {
                var flag = $("input[name='answerIteam-" + i + "']:checked").val();
                if (flag == null) {
                    alert("您还有未填项，无法提交!");
                    return false;
                } else {
                    AnswerArray[index] = flag;
                    index++;
                }
            }
        }
        //for (var i = 0; i < AnswerArray.length; i++) {
        //    console.log(AnswerArray[i]);
        //}
        //答案收集完成
        var itemScore = 100 / AnswerArray.length;//计算单题分数
        var sumScore = 0;//总分
        //开始提交
        show_time(1);//关闭倒计时
        $.ajax({
            type: "post",
            url: "UnifiedTest",
            data: {
                Answer: AnswerArray,
                UtId: $("#UtId").val() * 1
            },
            datatype: "json",
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    console.log("所选答案" + data[i].UserAnswer + "正确答案" + data[i].RealAnswer + "结果" + data[i].IsTrue);
                    if (data[i].IsTrue.toString() == "true") {
                        sumScore += itemScore;
                    }

                }
                $("#timer").attr('id', 'score');
                //$("#Score").css('display', 'block');
                $("#score").text(Math.round(sumScore) + " 分");
                console.log("总分" + Math.round(sumScore));
            }
        })
    })
})