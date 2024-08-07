$('.sidebar-arrow').click(function () {
    $(this).closest('li').find('.sidebar-expandation').toggleClass('show');
    $(this).toggleClass('active');
})

$('.show-user-action').click(function () {
    $('.user-action').toggleClass('show');
    $('.notification-content').removeClass('show');
    $(this).toggleClass('active');
})

$('.show-notification').click(function () {
    $('.notification-content').toggleClass('show');
    $('.user-action').removeClass('show');
    $(this).toggleClass('active');
})

$('.page-btn-nav-item').click(function () {
    $('.page-btn-nav-item').removeClass('active');
    $(this).addClass('active');
})

$('.click-logout').click(() => {
    Swal.fire({
        title: "Đăng xuất?",
        text: "Bạn chắc chắn muốn đăng xuất?",
        icon: "question",
        showCancelButton: true,
        showConfirmButton: true,
        focusConfirm: false,
        cancelButtonText: "Hủy",
        confirmButtonText: "Đăng xuất",
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = '/admin/logout'
        }
    });
})