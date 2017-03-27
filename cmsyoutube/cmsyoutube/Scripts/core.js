'use strict';

(function () {

    $('.item_video').on('click',function (e) {
        e.preventDefault();
        var $code = $(this).data('video');
        var myVideo = '<iframe class="embed-responsive-item" src="//www.youtube.com/embed/'
            + $code + '?autoplay=1" frameborder="0" allowfullscreen></iframe>';
        
        $('#video_view').empty().append(myVideo);
    })

})();

function getId(url) {
    var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
    var match = url.match(regExp);

    if (match && match[2].length == 11) {
        return match[2];
    } else {
        return 'error';
    }
}

function hex2a(hexx) {
    var hex = hexx.toString();//force conversion
    var str = '';
    for (var i = 0; i < hex.length; i += 2)
        str += String.fromCharCode(parseInt(hex.substr(i, 2), 16));
    return str;
}

