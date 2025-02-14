﻿const showButtonLoader = (element, loaderSize, strokeWidth) => {
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

const showDialogWithCallback = (type, title, message, callback) => {
    Swal.fire({
        title: title,
        text: message,
        icon: type,
    }).then(() => {
        callback();
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

const updateQueryParams = (params) => {
    const url = new URL(window.location.href);
    params.map(param => {
        url.searchParams.set(param.key, param.value);
    })
    window.history.pushState({}, '', url);
}


let isShow = false;
$(".categories-btn").click(() => {
    if (!isShow) {
        isShow = true;
        $(".hidden-menu").addClass("showHiddenMenu");
        $(".overlay").addClass("showOverlay");
    } else {
        isShow = false;
        $('.hidden-menu').removeClass("showHiddenMenu");
        $(".overlay").removeClass("showOverlay");
    }
});

$(".hidden-menu").mousedown((e) => {
    if ($(e.target).closest('.sidebar').length <= 0) {
        $('.hidden-menu').removeClass("showHiddenMenu");
        $(".overlay").removeClass("showOverlay");
    }
});

$(".overlay").mousedown(() => {
    $(".hidden-menu").removeClass("showHiddenMenu");
    $(".mobile-sidebar").removeClass("showMobileSidebar");
    $(".overlay").removeClass("showOverlay");
});


//Show change theme
$('.toogle-theme-btn').click(() => {
    $('.change-theme').toggleClass('show');
})

//Change theme -----------------------------
$('input[name="theme"]').on('change', function() {
    const theme = $(this).val();
    const color = $(this).data('color');
    
    $(this).siblings('.theme-label-box .theme-label').css('background-color', color);
    
    $.ajax({
        type: 'POST',
        url: `/api/themes?theme=${theme}`,
        success: (response) => {
            location.reload();
        }
    })
})

//Show/hide web loader
function showWebLoader() {
    $('.webloading').css('display', 'grid');
}

function hideWebLoader() {
    const timeout = setTimeout(() => {
        $('.webloading').hide();
        clearTimeout(timeout);
    }, 1000);
}

const showPageContentLoader = (container, loader_height) => {
    const element = `
        <div class="page-content-loader" style="height: ${loader_height}rem">
            <div class="loader-box">
                <svg class="loader-container" style="width: 30px; height: 30px; --uib-color: #000" viewBox="0 0 40 40" height="40" width="40">
                    <circle class="track" cx="20" cy="20" r="17.5" pathlength="100" stroke-width="4px" fill="none" />
                    <circle class="car" cx="20" cy="20" r="17.5" pathlength="100" stroke-width="4px" fill="none" />
                </svg>
            </div>
        </div>  
    `;

    $(container).html(element);
}

const hidePageContentLoader = (container, timeout_ms) => {
    const timeout = setTimeout(() => {
        $(container).empty();
        clearTimeout(timeout);
    }, timeout_ms)
}

//Show scroll to top button ----------------------------------------------------------
const scrollTopBtn = document.querySelector(".to-top-btn");

window.onscroll = function () {
    if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
        scrollTopBtn.classList.add("showToTopBtn");
    } else {
        scrollTopBtn.classList.remove("showToTopBtn");
    }
}

scrollTopBtn.onclick = function () {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
};

//Show sidebar menu ------------------------------------------------------------------------
function showSidebar() {
    $(".mobile-sidebar").addClass("showMobileSidebar");
    $(".overlay").addClass("showOverlay");
    $(".overlay").click(function () {
        $(".mobile-sidebar").removeClass("showMobileSidebar");
    });
};

$(".mobile-categories-btn").click(showSidebar);
$(".bottom-nav-show-sidebar").click(showSidebar);
$(".sidebar-close-btn").click(function () {
    $(".mobile-sidebar").removeClass("showMobileSidebar");
    $(".overlay").removeClass("showOverlay");
});

//Show mobile sidebar menu level 2, 3
var sidebarContents = $('.mobile-sidebar .megamenu-item').toArray();
sidebarContents.forEach(item => {
    var _item = $(item);

    var sidebarChervon = _item.find('.megamenu-item-box .megamenu-chevron');
    sidebarChervon.click(() => {
        _item.children('.megamenu-content').toggleClass('showSidebarContent');
        sidebarChervon.toggleClass('chevronRotate');

        var sidebarContentsLvl2 = _item.children('.megamenu-content').find('.megamenu-content-item').toArray();

        sidebarContentsLvl2.forEach(item1 => {
            var _item1 = $(item1);
            var sidebarChevronLevel2 = _item1.find('.sub-megamenu-box .megamenu-chevron')

            sidebarChevronLevel2.click(() => {
                _item1.children('.megamenu-item-list').toggleClass('showSidebarContent');
                sidebarChevronLevel2.toggleClass('chevronRotate');
            })
        })
    })


})

//Show bottom navigation --------------------------------------------------------------
$(document).ready(function () {
    let preScrollPos = window.scrollY;
    $(window).scroll(() => {
        const currentScrollPos = window.scrollY;
        if (currentScrollPos > preScrollPos) {
            $(".bottom-nav-mobile").addClass("showBottomNav");
        }
        else {
            $(".bottom-nav-mobile").removeClass("showBottomNav");
        }
        preScrollPos = currentScrollPos;
    })
});

//Slick Slider ------------------------------------------------------------------------
$(".sale-slick-slider").slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    infinite: true,
    autoplay: true,
    autoplaySpeed: 2000,
    arrows: true,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
    dots: true,
    swipeToSlide: true,
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 4,
                arrows: false
            }
        },
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 2.2,
                autoplay: false,
                infinite: false,
                arrows: false
            }
        }
    ]
});

//Slick Slider for Collection in main page
$(".collection-slick-slider").slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    infinite: false,
    arrows: true,
    swipeToSlide: true,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 4,
                arrows: false
            }
        },
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 2.2,
                arrows: false
            }
        }
    ]
})

//Slick Slider for Brands Collection
$(".brand-collections").slick({
    slidesToShow: 7,
    slidesToScroll: 1,
    dots: false,
    infinite: true,
    arrows: false,
    autoplay: true,
    autoplaySpeed: 1500,
    swipeToSlide: true,
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 5,
            }
        },
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 3
            }
        }
    ]
})

//Slick slider in product detail page -----------------------
$('.slider-main').slick({
    slidesToShow: 1,
    slidesToScroll: 1,
    arrows: true,
    fade: false,
    infinite: false,
    useTransform: true,
    speed: 400,
    cssEase: 'cubic-bezier(0.77, 0, 0.18, 1)',
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-arrow-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-arrow-right"></i></button>`,
})

$('.slider-nav').on('init', function (event, slick) {
    $('.slider-nav .slick-slide.slick-current').addClass('is-active');
}).slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    dots: false,
    focusOnSelect: false,
    infinite: false,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-arrow-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-arrow-right"></i></button>`,
})

$('.slider-main').on('afterChange', function (event, slick, currentSlide) {
    $('.slider-nav').slick('slickGoTo', currentSlide);
    var currrentNavSlideElem = '.slider-nav .slick-slide[data-slick-index="' + currentSlide + '"]';
    $('.slider-nav .slick-slide.is-active').removeClass('is-active');
    $(currrentNavSlideElem).addClass('is-active');
});

$('.slider-nav').on('click', '.slick-slide', function (event) {
    event.preventDefault();
    var goToSingleSlide = $(this).data('slick-index');

    $('.slider-main').slick('slickGoTo', goToSingleSlide);
});


//------
//Slick Slider for Collection in product detail page
$(".order-product-slick-slider").slick({
    slidesToShow: 6,
    slidesToScroll: 1,
    infinite: false,
    arrows: true,
    swipeToSlide: true,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 4,
                arrows: false
            }
        },
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 2.2,
                arrows: false
            }
        }
    ]
})

//Show footer column content -----------------------------------------------------------
var footerColArr = $(".footer-col").toArray();
footerColArr.forEach(item => {
    var _item = $(item);
    _item.children(".footer-title").click(() => {
        _item.toggleClass("activeFooter");
    });
});

//Lazy loading image
function loadImg(img) {
    const url = img.getAttribute('lazy-src');

    img.removeAttribute('lazy-src');
    img.setAttribute('src', url);  
    img.classList.add('img-loaded');
    //img.parentNode.classList.remove('lazy-loading');
}

function lazyLoading() {
    if ('IntersectionObserver' in window) {
        var lazyImages = document.querySelectorAll('[lazy-src]');
        let observer = new IntersectionObserver((entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !entry.target.classList.contains('img-loaded')) {
                    loadImg(entry.target);
                }   
            })
        }));

        lazyImages.forEach(img => {
            observer.observe(img);
        });
    }
}

$(document).ready(lazyLoading);


//$(document).click(function (e) {
//    var target = e.target;
//    while (target && target.tagName !== 'A') {
//        target = target.parentNode;
//    }
//    if (target.getAttribute('href') === '#') {
//        e.preventDefault();
//        window.location.href = "/error/notfound";
//    }
//})


 //---Show order dropdown content---------------------------
$(".sort-dropdown-btn").click(() => {
    $(".sort-dropdown-content").toggleClass("showDropdown");
    $(".sort-dropdown-content").click(() => {
        $(".sort-dropdown-content").removeClass("showDropdown");
    })
})

//----------------------------------------------------------
//---Save search history to Local Storage-------------------
$(document).ready(function () {
    $('.search-form').submit(() => {
        var searchText = $('#search').val();

        var searchHistory = JSON.parse(localStorage.getItem('searchHistory')) || [];

        if (searchHistory.length > 13) {
            searchHistory.shift();
        }

        if (searchText.length > 0 || searchText != null) {
            searchHistory.push(searchText);
        }

        localStorage.setItem('searchHistory', JSON.stringify(searchHistory));
    })
})

//Show search width 100% in mobile
$(document).ready(function () {
    $('#search').on('click', () => {
        const windowWidth = $(window).width();
        if (windowWidth < 768) {
            $('.header-box').hide();
            $('.header-btn').hide();
            $('body').css('overflow', 'hidden');
            $('.search').css('width', '100%');
            $('.search').css('transition', 'var(--trans-all-03)');
            $('.close-search').show();
            $('.search-history').show();
        }

        $('.close-search').click(() => {
            $('.header-box').css('display', 'flex');
            $('.header-btn').show();
            $('body').css('overflow', 'scroll');
            $('.search').css('width', '60%');
            $('.close-search').hide();
            $('.ajax-search-autocomplete').hide();
            $('.search-history').hide();
        })

        var searchHistory = JSON.parse(localStorage.getItem('searchHistory')) || [];

        $('.search-history-list').empty();
        if (searchHistory.length > 0) {
            $('.search-history').css('padding', '0 0 1rem 0');
            $('.search-history').children('h4').hide();

            for (var i = searchHistory.length - 1; i >= 0; i--) {
                $('.search-history-list').append(`<a href="/search?q=${searchHistory[i]}">`
                    + `<li class="search-history-list-item">`
                    + searchHistory[i] + '</li>' + '</a>');
            }
        }
        else {
            $('.search-history').children('h4').show();
        }

        $('.clear-search-history').click(() => {
            $('.search-history-list').empty();
            localStorage.removeItem('searchHistory');
            $('.search-history').css('padding', '1rem');
            $('.search-history').children('h4').show();
        })
    })

})



let typingTimeOut;
$("#search").keyup(function () {
    clearTimeout(typingTimeOut);

    typingTimeOut = setTimeout(() => {
        const searchText = $(this).val();
        if (searchText.length > 0) {
            $.ajax({
                url: '/api/products',
                type: 'GET',
                data: {
                    q: searchText
                },
                success: function (response) {
                    const maxItems = window.innerWidth < 768 ? 25 : 6

                    $('.ajax-search-autocomplete').show();
                    $('.ajax-search-items').empty();

                    if (!response.status || !response.data.length) {
                        $('.ajax-search-empty').css('display', 'grid');
                    }
                    else {
                        $('.ajax-search-empty').hide();

                        const products = response.data.slice(0, maxItems);
                        products.map(item => {
                            const src = item.productImages[0]?.imageSrc || '/images/no-image.jpg';
                            const itemPrice = item.saleProducts[0]?.salePrice || item.price;

                            $('.ajax-search-items').append(`<a href="/product/${item.productId}"> 
                                <div class="ajax-search-item d-flex justify-content-between align-items-center">
                                    <div class="ajax-search-item-info">
                                        <div class="ajax-search-item-name d-flex align-items-center">
                                            <h3>${item.productName}</h3>
                                        </div>
                                        <div class="ajax-search-item-price d-flex align-items-center">
                                            <h3>${itemPrice.toLocaleString('vi-VN') + 'đ'}</h3>
                                            <h4>${item.originalPrice > itemPrice ? item.originalPrice.toLocaleString('vi-VN') + 'đ' : ''}</h4>
                                        </div>
                                    </div>
                                    <div class="ajax-search-item-image">
                                        <img src="${src}" alt="" />
                                    </div>
                                </div>
                            </a>`)
                        })
                    }
                },
                error: () => {
                    $('.ajax-search-autocomplete').hide();
                }
            })
        }
        else {
            $('.ajax-search-autocomplete').hide();
        }
    }, 500)

    $(document).on('click', (e) => {
        if (window.innerWidth > 768) {
            var ajaxSearch = $('.ajax-search-autocomplete');
            if (!$(e.target).closest('.ajax-search-autocomplete').length) {
                ajaxSearch.hide();
            }
        }
    })
})

//-----------------------------------------------------------------------
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

const inputArr = $('.form-input').not('select').toArray();
inputArr.forEach((input) => {
    const _input = $(input);

    _input.on({
        focus: () => { checkInputValid(_input) },
        change: () => { checkInputValid(_input) }
    });
})

//--Show form ----------------------------------------------------------------
$('.action-login-btn').click(() => {
    $('.login').addClass('show');
})

$('.action-register-btn').click(() => {
    $('.register').addClass('show');
})

$('.login-btn .login-link').click(() => {
    $('.login').addClass('show');
})

$('.bottom-nav-account').click(() => {
    $('.login').addClass('show');
})

//---

$('.close-form').mousedown(function () {
    $(this).closest('.form-container').removeClass('show');
    clearFormInput($(this).closest('.form-container').find('form'));
    closeFormError($(this).closest('.form-container').find('form'));
})

$('.form-container').mousedown(function (e) {
    if ($(this).hasClass('edit-image') || $(this).hasClass('upload-image')) {
        return;
    }

    if ($(e.target).closest('.form-box').length <= 0) {
        $(this).removeClass('show');
        clearFormInput($(this).find('form'))
    }
})

$('.form-container form').on('reset', function (e) {
    e.preventDefault();

    $(this).closest('.form-container').removeClass('show');
    clearFormInput($(this));
})

$('.to-login').mousedown(() => {
    $('.register').removeClass('show');
    $('.login').addClass('show');
    clearFormInput($('.register'))
})

$('.to-register').mousedown(() => {
    $('.login').removeClass('show');
    $('.register').addClass('show');
    clearFormInput($('.login'))
})

 //------------------------

$(document).on('focus', '.page-input > input', function () {
    $(this).parent().addClass('focused');
})

$(document).on('blur', '.page-input > input', function () {
    $(this).parent().removeClass('focused');
})

$(document).on('change', '.page-radio-input input', function () {
    const name = $(this).attr('name');
    $(`.page-radio-input input[name="${name}"]`).closest('label').removeClass('checked');
    $(this).closest('label').addClass('checked')
})


$('.register form').submit(function (e) {
    e.preventDefault();
    const userName = $(this).find('#RegUserName').val();
    const password = $(this).find('#RegPassword').val();
    const confirmPassword = $(this).find('#ConfirmPassword').val();
    const email = $(this).find('#Email').val();
    const captchaResponse = $(this).find('input[name="cf-turnstile-response"]').val();

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
            ReturnUrl: window.location.href,
            CaptchaResponse: captchaResponse
        }),
        success: (response) => {
            if (!response.status) {
                const str = `<span>
                <i class="fa-solid fa-circle-exclamation"></i>`
                    + response.message + `</span>`;
                showFormError(this, str);
                closeFormErrorWithTimeout(this);
                hideButtonLoader(submitBtn, btnHtml);
                renderTurnstile('.register');
            } else {
                closeFormError(this);
                window.location.href = response.data;
            }
        },
        error: (jqXHR) => {
            renderTurnstile('.register');
        }
    })
})


$('.login form').submit(function (e) {
    e.preventDefault();
    const userName = $(this).find('#UserName').val();
    const password = $(this).find('#Password').val();
    const captchaResponse = $(this).find('input[name="cf-turnstile-response"]').val();

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
            ReturnUrl: window.location.href,
            CaptchaResponse: captchaResponse
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
                renderTurnstile('.login');
            }
        },
        error: (jqXHR) => {
            renderTurnstile('.login');
        }
    })
})

$('.external-login-btn').click(function () {
    const provider = $(this).data('provider');
    const returnUrl = window.location.href;

    if (provider) {
        window.location.href = `/account/externallogin?provider=${provider}&returnUrl=${returnUrl}`;
    }
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


$(document).on('click', '.not-logged-in', () => {
    $('.login').addClass('show');
})



function renderTurnstile(container) {
    const element = `${container} .cf-turnstile`;
    $(element).empty();

    if (typeof turnstile !== 'undefined') {
        turnstile.render(element, {
            siteKey: $(element).data('sitekey')
        })

        $(`${container} .form-submit-btn`).prop('disabled', true);
    }
}

//turnstile callback
function loginCaptchaCompleted(token) {
    $('.login .form-submit-btn').prop('disabled', false);
}

function registerCaptchaCompleted(token) {
    $('.register .form-submit-btn').prop('disabled', false);
}


// count down
const updateCountDown = (element, endDate) => {
    const interval = setInterval(() => {
        const now = new Date();
        const diff = endDate - now;

        if (diff <= 0) {
            clearInterval(interval);
            return;
        }

        const days = Math.floor(diff / (1000 * 60 * 60 * 24));
        const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((diff % (1000 * 60)) / 1000);

        const formatNum = (num) => num.toString().padStart(2, '0');

        const countDownElement = $(element).find('.sale-count-down');
        countDownElement.find('.days').text(formatNum(days));
        countDownElement.find('.hours').text(formatNum(hours));
        countDownElement.find('.minutes').text(formatNum(minutes));
        countDownElement.find('.seconds').text(formatNum(seconds));
    }, 1000);
}


//Chatbot
const appendChatbotMessage = (message, isUser) => {
    const messageClass = isUser ? 'from-user' : 'from-bot';

    $('.chatbot-messages').append(`
        <div class="chatbot-message ${messageClass}">
            <div class="chatbot-message-box">
                ${message}
            </div>
        </div>
    `);

    $('.chatbot-messages').scrollTop($('.chatbot-messages').prop("scrollHeight"));
}

$('.chatbot-float-icon').click(() => {
    $('.chatbot').addClass('show')
})

$('.close-chatbot').click(() => {
    $('.chatbot').removeClass('show')
})

$('.chatbot-input input').on('keyup', function (e) {
    if ($(this).val().length > 0)
    {
        $(this).removeClass('error');
    }
})

$('.chatbot-input').submit(function (e) {
    e.preventDefault();

    const message = $(this).find('input').val();

    if (!message) {
        $(this).find('input').addClass('error').focus();
        return;
    } else {
        $(this).find('input').removeClass('error');
    }

    appendChatbotMessage(`<div class="chatbot-message-text"><span>${message}</span></div>`, true);
    $(this).find('input').val('');
    const element = showButtonLoader($('.chatbot-send'), '21px', '4px');

    $.ajax({
        type: 'POST',
        url: '/api/chat',
        contentType: 'application/json',

        data: JSON.stringify({
            message
        }),
        success: (response) => {
            const receivedMessage = response.message;
            appendChatbotMessage(receivedMessage, false);
            hideButtonLoader($('.chatbot-send'), element);
        }
    })
})

$('select').toArray().map(select => {
    $(select).select2();
})

$('.filter-button').click(function() {
    const filterContent = $(this).siblings('.filter-content');
    $('.filter-content').not(filterContent).removeClass('show');
    filterContent.toggleClass('show');
    
    updateFilterContentPosition();
})

$(document).click((e) => {
    if ($(e.target).closest('.filter-content, .filter-button').length <= 0) {
        $('.filter-content').removeClass('show');
    }
})

const updateFilterContentPosition = () => {
    $(".filter-content").each(function () {
        const filterMaxWidth = $('.filter-box').outerWidth();
        $(this).css('max-width', filterMaxWidth);

        const container = $(".page-container");
        const containerOffset = container.offset();
        const containerWidth = container.outerWidth();

        const $element = $(this);
        const elementOffset = $element.offset();
        const elementWidth = $element.outerWidth();

        const originalLeft = parseFloat($element.css("left")) || 0;
        
        if (elementOffset.left + elementWidth > containerOffset.left + containerWidth) {
            const overflow = (elementOffset.left + elementWidth) - (containerOffset.left + containerWidth);
            $element.css("left", originalLeft - overflow + "px");
        }
        
        if (elementOffset.left < containerOffset.left) {
            const overflow = containerOffset.left - elementOffset.left;
            $element.css("left", originalLeft + overflow + "px");
        }
    });
}

$(document).ready(() => {
    updateFilterContentPosition();
})

$(window).resize(() => {
    updateFilterContentPosition();
})
