$(function () {
    $.scrollUp({
        animation: 'fade',
        activeOverlay: false,
        scrollName: 'scrollUp',
        topDistance: '500',
        topSpeed: 300,
        animationInSpeed: 200,
        animationOutSpeed: 200,
        scrollImg: {
            active: true,
            type: 'background',
            src: '../../Content/image/top.png'
        }
    });
});