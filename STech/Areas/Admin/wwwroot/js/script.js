$('.sidebar-arrow').click(function () {
    $(this).closest('li').find('.sidebar-expandation').toggleClass('show');
    $(this).toggleClass('active');
})

$('.show-user-action').click(function () {
    $('.user-action').toggleClass('show');
    $(this).toggleClass('active');
})

$('.show-notification').click(function () {
    $('.notification-content').toggleClass('show');
    $(this).toggleClass('active');
})