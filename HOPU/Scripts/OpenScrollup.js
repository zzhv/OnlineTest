$(function () {
    $.scrollUp({
        animation: 'slide',
        activeOverlay: false,
        scrollName: 'scrollUp',
        topDistance: '500',
        topSpeed: 300,
        animationInSpeed: 200,
        animationOutSpeed: 200,
        scrollText: '回到顶部',// 元素文本
        scrollImg: {
            active: true,
            type: 'background',
            src: '../../Content/image/top.png'
        }
    });
});