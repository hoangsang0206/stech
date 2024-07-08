
function updateCartCount() {
    $.ajax({
        type: 'GET',
        url: '/cart/getcartcount',
        success: (response) => {
            $('.cart-count').html(response.data);
        },
        error: () => {
            $('.cart-count').html(0);
        }
    })
}

$(document).ready(() => {
    updateCartCount();
})

//-Add product to cart ------------------------------------
$('.add-to-cart-btn, .btn-add-to-cart, .buy-action-btn').click(function() {
    const productID = $(this).data('product');

    if (productID) {
        showWebLoader();
        $.ajax({
            type: 'POST',
            url: '/cart/addtocart',
            data: {
                id: productID
            },
            success: (respone) => {
                if (respone.status) {
                    updateCartCount();
                }
            }
        })

        hideWebLoader()
    }   
})

$('.buy-action-btn').click(() => {
    setTimeout(() => { window.location.href = '/cart' }, 510)
})

//-------------------------------
$('.not-logged-in').click(() => {
    $('.login').addClass('show');
})

//---------------------------------
$(document).ready(() => {
    var cartFormInput = $('.cart-form input').toArray();
    cartFormInput.forEach((input) => {
        checkInputValid($(input));
    })

    if ($('#cod-method').is(':checked')) {
        $('.input-ship-info').show();
    }

    $('input[name="shipmethod"]').on('change', () => {
        if ($('#cod-method').is(':checked')) {
            $('.input-ship-info').show();
        }
        else {
            $('.input-ship-info').hide();
        }
    })
})

//---------------------------------
function activeCartStep(step1, step2, step3, step4) {
    $(step1).addClass('step-active');
    $(step2).addClass('step-active');
    $(step3).addClass('step-active');
    $(step4).addClass('step-active');
}

function disAciveStep(step2, step3, step4) {
    $(step2).removeClass('step-active');
    $(step3).removeClass('step-active');
    $(step4).removeClass('step-active');
}

function showCartInfo() {
    var idFromUrl = window.location.hash.substring(1);
    if (idFromUrl.length > 0) {
        hideCartInfo();
        $('#' + idFromUrl).addClass('form-current');

        disAciveStep('.step-2', '.step-3', '.step-4');

        if (idFromUrl == 'cart-order-box') {
            activeCartStep('.step-1', '.step-2');
        }
    }
}

function hideCartInfo() {
    var cartInfoList = $('.cart-info').toArray();
    cartInfoList.forEach((item) => {
        $(item).removeClass('form-current');
    })
}

$(document).ready(() => {
    showCartInfo();

    $(window).on('hashchange', () => {
        showCartInfo();
    })
})

//--Update cart item quantity
$('.update-quantity').click(function() {
    var productID = $(this).data('product');
    var updateType = $(this).data('update');
    var parentOfBtn = $(this).parent('.cart-product-quantity');
    var inputQuantity = parentOfBtn.children('input[name="quantity"]');

    if (productID.length > 0 && updateType.length > 0) {
        showWebLoader();
        $.ajax({
            type: 'PUT',
            url: '/cart/updatequantity',
            data: {
                maSP: productID,
                updateType: updateType
            },
            success: (res) => {
                hideWebLoader()
                inputQuantity.val(res.qty);

                var total = res.total.toLocaleString("vi-VN") + 'đ';
                $('.total-price').empty();
                $('.total-price').text(total);

                if (res.error.length > 0) {
                    $('.cart-error').empty();
                    $('.cart-error').show();
                    var str = `<span><i class="fa-solid fa-circle-exclamation"></i>
                    ${res.error}</span>`;
                    $('.cart-error').append(str);

                    var timeout = setTimeout(() => {
                        $('.cart-error').hide()
                        clearTimeout(timeout);
                    }, 7000)
                }
            },
            error: () => { hideWebLoader(); }
        })
    }
})

//--------
$('input[name="quantity"]').focus((e) => {
    var currentVal = $(e.target).val();

    $(e.target).blur(() => {
        var newVal = $(e.target).val();
        var productID = $(e.target).data('product');
        if (newVal != currentVal) {
            showWebLoader();    
            $.ajax({
                type: 'PUT',
                url: '/cart/updatequantity',
                data: {
                    maSP: productID,
                    sluong: newVal
                },
                success: (res) => {
                    hideWebLoader()
                    $(e.target).val(res.qty);

                    var total = res.total.toLocaleString("vi-VN") + 'đ';
                    $('.total-price').empty();
                    $('.total-price').text(total);

                    if (res.error.length > 0) {
                        $('.cart-error').empty();
                        $('.cart-error').show();
                        var str = `<span><i class="fa-solid fa-circle-exclamation"></i>
                    ${res.error}</span>`;
                        $('.cart-error').append(str);

                        var timeout = setTimeout(() => {
                            $('.cart-error').hide()
                            clearTimeout(timeout);
                        }, 7000)
                    }
                },
                error: () => { hideWebLoader(); }
            })
        }
    })
})