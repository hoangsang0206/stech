$('.register form').submit(function (e) {
    e.preventDefault();
    const userName = $(this).find('#RegUserName').val();
    const password = $(this).find('#RegPassword').val();
    const confirmPassword = $(this).find('#ConfirmPassword').val();
    const email = $(this).find('#Email').val();

    const submitBtn = $(e.target).find('.form-submit-btn');
    const btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/register',
        data: {
            RegUserName: userName,
            RegPassword: password,
            ConfirmPassword: confirmPassword,
            Email: email
        },
        success: (response) => {
            if (!response.status) {
                const str = `<span>
                <i class="fa-solid fa-circle-exclamation"></i>`
                    + response.message + `</span>`;
                showFormError(this, str);
                closeFormErrorWithTimeout(this);
                resetBtn(submitBtn, btnText);
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
    const btnText = showBtnLoading(submitBtn);

    $.ajax({
        type: 'POST',
        url: '/account/login',
        data: {
            UserName: userName,
            Password: password
        },
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
                resetBtn(submitBtn, btnText);
            }
        },
        error: (jqXHR) => { }
    })
})


$('.login-info-logout, .account-logout').click(() => {
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