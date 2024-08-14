const updateUrlPath = (newPath) => {
    const url = window.location.protocol + '//' + window.location.host + newPath;
    history.pushState({}, '', url);
}

const updateParams = (params) => {
    const url = new URL(window.location.href);
    const _params = new URLSearchParams(url.search);

    Object.entries(params).forEach(([key, value]) => { 
        if (!value) {
            _params.delete(key);
        } else {
            _params.set(key, value);
        }
    })

    url.search = _params.toString();
    history.pushState({}, '', url);
}

const showWebLoader = () => {
    $('.web-loader-wrapper').addClass('show');
}

const hideWebLoader = (timeout) => {
    setTimeout(() => {
        $('.web-loader-wrapper').removeClass('show');
    }, timeout);
}

const showButtonLoader = (element, loaderSize, strokeWidth) => {
    const elementHtml = $(element).html();
    const elementLoader = `<div class="loader-box">
            <svg class="loader-container" style="width: ${loaderSize}; height: ${loaderSize}" viewBox="0 0 40 40" height="40" width="40">
                <circle class="track" cx="20" cy="20" r="17.5" pathlength="100" stroke-width="${strokeWidth}" fill="none" />
                <circle class="car" cx="20" cy="20" r="17.5" pathlength="100" stroke-width="${strokeWidth}" fill="none" />
            </svg></div>`;

    const elementWidth = $(element).width();
    const elementHeight = $(element).height();

    $(element).html(elementLoader);
    $(element).find('.loader-box').css('height', elementHeight);
    $(element).find('.loader-box').css('width', elementWidth);

    $(element).prop('disabled', true);

    return elementHtml;
}

const hideButtonLoader = (element, elementHtml) => {
    const timeout = setTimeout(() => {
        $(element).html(elementHtml);
        $(element).prop('disabled', false);
        clearTimeout(timeout);
    }, 1000);
}

const showFormError = (form, message) => {
    $(form).find('.form-error').show();
    $(form).find('.form-error').html(message);
}

const closeFormError = (form) => {
    $(form).find('.form-error').hide();
    $(form).find('.form-error').empty();
}

const closeFormErrorWithTimeout = (form) => {
    const timeout = setTimeout(() => {
        $(form).find('.form-error').hide();
        $(form).find('.form-error').empty();
        clearTimeout(timeout);
    }, 5000)
}

const clearFormInput = (form) => {
    form.find('input').not('[type="radio"], [type="checkbox"]').val('').trigger('change');
    form.find('select').val('').trigger('change');
    form.find('textarea').val('').trigger('change');
}

const showDialog = (type, title, message) => {
    Swal.fire({
        title: title,
        text: message,
        icon: type,
    })
}

const showHtmlDialog = (type, title, html) => {
    Swal.fire({
        title: title,
        html: html,
        icon: type,
    })
}

const showErrorDialog = () => {
    Swal.fire({
        title: "Đã xảy ra lỗi",
        text: "Đã xảy ra lỗi không xác định",
        icon: "error",
    })
}

const showConfirmDialog = (title, message, callback) => {
    Swal.fire({
        title: title,
        text: message,
        icon: "question",
        showCancelButton: true,
        showConfirmButton: true,
        focusConfirm: false,
        cancelButtonText: "Hủy",
        confirmButtonText: "Xác nhận",
    }).then((result) => {
        if (result.isConfirmed) {
            callback();
        }
    });
}

const showForm = (form_container) => {
    $(form_container).addClass('show');

    $(form_container).find('.close-form').click(() => {
        closeForm(form_container);
    })
}

const closeForm = (form_container) => {
    $(form_container).removeClass('show');
}


const loadPagination = (totalPages, currentPage) => {
    $('.pagination-box').empty();

    if (totalPages < 1) {
        return;
    }

    const maxPagesToShow = 4;

    let startPage = Math.max(1, currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(totalPages, startPage + maxPagesToShow - 1);

    if ((endPage - startPage + 1) < maxPagesToShow)
    {
        startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }

    if (currentPage > 1)
    {
        $('.pagination-box').append(`
            <a href="javascript:void(0)" class="pagination-item" data-page="previous">
                <i class="fa-solid fa-chevron-left"></i>
            </a>
        `);
    }

    for (let i = startPage; i <= endPage; i++)
    {
        $('.pagination-box').append(`
            <a href="javascript:void(0)" class="pagination-item ${i === currentPage ? 'current' : ''}" data-page="${i}">${i}</a>
        `);
    }

    if (currentPage < totalPages)
    {
        $('.pagination-box').append(`
            <a href="javascript:void(0)" class="pagination-item" data-page="next">
                <i class="fa-solid fa-chevron-right"></i>
            </a>
        `);
    }
}

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

$('.click-logout, .sidebar-logout').click(() => {
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

$(document).click((e) => {
    if ($(e.target).closest('.filter-contents, .filter-btn').length <= 0) {
        $('.filter-contents').removeClass('show');
    }
})

$('.filter-btn').click(function () {
    $('.filter-contents').removeClass('show');
    $(this).siblings('.filter-contents').toggleClass('show');
})


$('.page-dropdown-btn').click(function () {
    $(this).siblings('.page-dropdown-content').toggleClass('show');
})

$('.page-dropdown-item').click(function () {
    const value = $(this).data('value');
    const text = $(this).data('text');

    $('.page-dropdown-item').removeClass('selected');
    $(this).addClass('selected');

    const dropdown_btn = $(this).closest('.page-dropdown').find('.page-dropdown-btn');
    dropdown_btn.data('selected', value);
    dropdown_btn.find('span').html(text);

    dropdown_btn.siblings('.page-dropdown-content').removeClass('show');
})


function checkInputValid(_input) {
    if (_input) {
        if (_input.val().length > 0) {
            _input.addClass('input-valid');
        }
        else {
            _input.removeClass('input-valid');
        }
    }
}

$('.form-input').not('select').toArray().forEach((input) => {
    const _input = $(input);

    _input.on({
        focus: () => { checkInputValid(_input) },
        change: () => { checkInputValid(_input) }
    });
})
