$(document).ready(function () {
    $('pre code').each(function (i, block) {
        hljs.highlightBlock(block);
    });
});

//倒计时
var kaishi = document.getElementById("startTime").value;  // $("#startTime").val();
var jieshu = document.getElementById("endTime").value;// $("#endTime").val();
var time_now_server, time_now_client, time_end, time_server_client, timerID;
time_end = new Date(jieshu);//结束的时间
time_end = time_end.getTime();
time_now_server = new Date(kaishi);//开始的时间
time_now_server = time_now_server.getTime();

time_now_client = new Date();
time_now_client = time_now_client.getTime();

time_server_client = time_now_server - time_now_client;

setTimeout("show_time()", 1000);

function show_time() {
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
    if (time_distance > 0) {
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

        timer.innerText = "考试结束";
        clearTimeout(timerID)
    }
}

//已答题改变边框
//$(function () {
//    $("input[name='answerIteam']").on('click', function () {

//    })
//})

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
            success: function (data, SumScore) {
                for (var i = 0; i < data.length; i++) {
                    console.log("所选答案" + data[i].UserAnswer + "正确答案" + data[i].RealAnswer + "结果" + data[i].IsTrue);
                    if (data[i].IsTrue.toString() == "true") {
                        sumScore += itemScore;
                    }
                }
                console.log("总分"+sumScore);
            }
        })
    })
})


$(function () {
    $("#btnTijiao2").on('click', function () {
        var AnswerArray = [];
        var lstInt = "";
        var index = 0;
        for (i = 0; i < document.all.length; i++) {
            if (document.all(i).type == 'radio' && document.all(i).checked == true) {
                AnswerArray[index] = document.all(i).value
                index++;
            } else if (document.all(i).type == 'checkbox' && document.all(i).checked == true) {
                lstInt = document.all(i).value + lstInt;
                var b = lstInt.split("");       //分割字符串a为数组b
                b.sort();              //数组b升序排序（系统自带的方法）
                AnswerArray[index] = b.join("");        //把数组b每个元素连接成字符串c
                index++;
                lstInt = "";
            }
        }
        for (var i = 0; i < AnswerArray.length; i++) {
            console.log(AnswerArray[i]);
        }
        $.ajax({
            type: "post",
            url: "Test",
            data: { Answer: AnswerArray },
            datatype: "json",
            //success: function (data) {
            //    alert(12312312);
            //}
        })
        ////radio
        //var TopicCount = $("#topicCount").val() * 1;
        ////alert(TopicCount);
        //for (var i = 1; i <= TopicCount; i++) {
        //    var flag = $("input[name='answerIteam-" + i + "']:checked").val();
        //    AnswerArray[i - 1] = flag;
        //    if (flag == null) {
        //        alert("您还有未填项，无法提交!");
        //        return false;
        //    }

        //    var lstInt = "";
        //    var items = document.getElementsByName("answerIteam-" + i);
        //    for (var i = 0; i < items.length; i++) {
        //        if (items[i].checked) {
        //            lstInt = items[i].value + lstInt;
        //        }
        //    }
        //    alert(lstInt);
        //    alert(AnswerArray[2]);
        //    if (lstInt.length <= 0) {
        //        alert("您还有未填项，无法提交!");
        //        return false;
        //    }
        //}
    })
})