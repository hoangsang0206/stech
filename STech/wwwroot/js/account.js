$('.register form').submit(function (e) {
    e.preventDefault();
    const userName = $(this).find('#RegUserName').val();
    const password = $(this).find('#RegPassword').val();
    const confirmPassword = $(this).find('#ConfirmPassword').val();
    const email = $(this).find('#Email').val();

    if (!userName || !password || !confirmPassword || !email) {
        return;
    }

    const submitBtn = $(e.target).find('.form-submit-btn');
    const btnHtml = showButtonLoader(submitBtn, '23px', '4px')
    $.ajax({
        type: 'POST',
        url: '/api/account/register',
        contentType: 'application/json',
        data: JSON.stringify({
            RegUserName: userName,
            RegPassword: password,
            ConfirmPassword: confirmPassword,
            Email: email,
            ReturnUrl: window.location.href
        }),
        success: (response) => {
            if (!response.status) {
                const str = `<span>
                <i class="fa-solid fa-circle-exclamation"></i>`
                    + response.message + `</span>`;
                showFormError(this, str);
                closeFormErrorWithTimeout(this);
                hideButtonLoader(submitBtn, btnHtml);
            } else {
                closeFormError(this);
                window.location.href = response.data;
            }
        },
        error: (jqXHR) => { }
    })
})


$('.login form').submit(function (e) {
    e.preventDefault();
    const userName = $(this).find('#UserName').val();
    const password = $(this).find('#Password').val();

    if (!userName || !password) {
        return
    }

    const submitBtn = $(this).find('.form-submit-btn');
    const btnHtml = showButtonLoader(submitBtn, '23px', '4px');

    $.ajax({
        type: 'POST',
        url: '/api/account/login',
        contentType: 'application/json',
        data: JSON.stringify({
            UserName: userName,
            Password: password,
            ReturnUrl: window.location.href
        }),
        success: (response) => {
            if (response.status) {
                closeFormError(this);
                window.location.href = response.data;
            } else {
                const str = `<span>
                <i class="fa-solid fa-circle-exclamation"></i>`
                    + response.message + `</span>`;
                showFormError(this, str);
                closeFormErrorWithTimeout(this);
                hideButtonLoader(submitBtn, btnHtml);
            }
        },
        error: (jqXHR) => { }
    })
})


$('.login-info-logout, .account-sidebar-logout').click(() => {
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
            window.location.href = '/account/logout'
        }
    });
})


$('.account-sidebar-item').click(function () {
    const sidebar = $(this).data('sidebar');

    $('.account-content').removeClass('current');
    $(`[data-sidebar="${sidebar}"]`).addClass('current');

    $('.account-sidebar-item').removeClass('active');
    $(this).addClass('active');
})