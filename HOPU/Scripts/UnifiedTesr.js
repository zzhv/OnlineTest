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
    //$("S_" + id).css("border", "1px solid red");
    //$("S_" + id).addClass("borderr")
    document.getElementById("S_" + id).style.border = "1px solid #0088cc";
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