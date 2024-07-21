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


$('.form-update-user').submit(function (e) {
    e.preventDefault();

    const fullname = $(this).find('#update_fullname').val();
    const dob = $(this).find('#update_dob').val();
    const phonenumber = $(this).find('#update_phonenumber').val();
    const email = $(this).find('#update_email').val();
    const gender = $(this).find('input[name="update_gender"]:checked').val();

    if (fullname && phonenumber && email) {
        const submitBtn = $(e.target).find('.form-submit-btn');
        const btnHtml = showButtonLoader(submitBtn, '23px', '4px')

        $.ajax({
            type: 'POST',
            url: '/api/account/update',
            contentType: 'application/json',
            data: JSON.stringify({
                FullName: fullname,
                Email: email,
                PhoneNumber: phonenumber,
                DOB: dob || null,
                Gender: gender || null
            }),
            success: (response) => {
                if (response.status) {
                    $('.user-fullname').text(fullname);
                    $('.user_email').text(email);
                    closeFormError(this);
                    showDialog('info', 'Cập nhật thành công', null);
                } else {
                    const str = `<span>
                <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.message + `</span>`;
                    showFormError(this, str);
                    closeFormErrorWithTimeout(this);
                }

                hideButtonLoader(submitBtn, btnHtml);
            },
            error: () => {
                showErrorDialog();
                hideButtonLoader(submitBtn, btnHtml);
            }
        })
    }
})

let cropper;
const showCropper = (image) => {
    $('#user-image').val(null);
    if (cropper) {
        cropper.destroy();
    }

    const reader = new FileReader();
    reader.onload = function (e) {
        $('#image-to-edit').attr('src', e.target.result);
        cropper = new Cropper($('#image-to-edit')[0], {
            aspectRatio: 1,
            zoomable: false,
            scaleable: false,
        })
    }

    reader.readAsDataURL(image);

    $('.upload-image').removeClass('show');
    $('.edit-image').addClass('show');
}

const hideCropper = () => {
    cropper.destroy();
    $('#image-to-edit').removeAttr('src');
    $('.edit-image').removeClass('show');
    $('.upload-image').addClass('show');
}

const hideFormUploadImage = () => {
    $('.drag-and-click-upload-image').removeClass('hide');
    $('.preview-image-wrapper').removeClass('show');
    $('#preview-image').removeAttr('src');
    $('.upload-image').removeClass('show');
    $('#user-image').val(null);
    $('.form-upload-image').find('.form-submit-btn').prop('disabled', true);
}

$('.form-edit-image').submit(function (e) {
    e.preventDefault();
    const canvas = cropper.getCroppedCanvas();
    if (canvas) {
        canvas.toBlob((blob) => {
            const croppedImage = new File([blob], 'user-image.png', { type: 'image/png' });
            const inputFiles = new DataTransfer();
            inputFiles.items.add(croppedImage);
            $('#user-image')[0].files = inputFiles.files;
            $('.drag-and-click-upload-image').addClass('hide');
            $('.preview-image-wrapper').addClass('show');
            $('#preview-image').attr('src', URL.createObjectURL(croppedImage))
            $('.form-upload-image').find('.form-submit-btn').removeAttr('disabled');
            hideCropper();
        })
    }
})

$('.form-edit-image').on('reset', function () {
    hideCropper();
})


$('.click-change-image').click(() => {
    $('.upload-image').addClass('show');
})

$('.drag-and-click-upload-image').on('dragover dragenter', function (e) {
    $(this).addClass('active');
    e.preventDefault();
})

$('.drag-and-click-upload-image').on('dragleave dragend drop', function () {
    $(this).removeClass('active');
})

$('.drag-and-click-upload-image, .click-choose-image').click(() => {
    $('#user-image').click();
})

$('.drag-and-click-upload-image').on('drop', (e) => {
    e.preventDefault();
    const file = e.originalEvent.dataTransfer.files[0];
    if (file) {
        showCropper(file);
    }
})

$('#user-image').change(function (e) {
    const file = e.target.files[0];
    if (file) {
        showCropper(file);
    }
})

$('.form-upload-image').submit(function (e) {
    e.preventDefault();
    const formData = new FormData();
    const file = $(this).find('#user-image').prop('files')[0];
    formData.append('file', file);

    if (formData) {
        const submitBtn = $(this).find('.form-submit-btn');
        const btnHtml = showButtonLoader(submitBtn, '23px', '4px');
        $.ajax({
            type: 'POST',
            url: '/api/account/upload',
            data: formData,
            contentType: false,
            processData: false,

            success: (response) => {
                if (response.status) {
                    $('.user-image img').attr('src', response.data);
                    showDialog('info', 'Cập nhật ảnh đại diện thành công', null);
                    closeFormError(this);
                    hideFormUploadImage();
                } else {
                    const str = `<span>
                        <i class="fa-solid fa-circle-exclamation"></i>`
                                + response.message + `</span>`;
                    showFormError(this, str);
                    closeFormErrorWithTimeout(this);
                }

                hideButtonLoader(submitBtn, btnHtml);
            },
            error: () => {
                showErrorDialog();
                hideButtonLoader(submitBtn, btnHtml);
            }
        });
    }
})

$('.form-upload-image').on('reset', function() {
    hideFormUploadImage();
})


$('.btn-add-address').click(() => {
    $('.add-address').addClass('show');
})