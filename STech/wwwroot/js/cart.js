
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
    const cartFormInput = $('.cart-form input').toArray();
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
    const idFromUrl = window.location.hash.substring(1);
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
    const cartInfoList = $('.cart-info').toArray();
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
const updateCartQty = (id, type, qty, input_element) => {
    showWebLoader();
    $.ajax({
        type: 'PUT',
        url: '/cart/updatequantity',
        data: {
            id: id,
            type: type,
            qty: qty
        },
        success: (response) => {
            hideWebLoader()
            if (response.status) {
                input_element.val(response.data.quantity);
                $('.total-price').html(response.data.totalPrice.toLocaleString("vi-VN") + 'đ');

                if (response.message) {
                    setTimeout(() => {
                        showHtmlErrorDialog(response.message)
                    }, 500);
                }
            } else {
                setTimeout(() => {
                    showHtmlErrorDialog(response.message)
                }, 500);
            }
        },
        error: () => {
            hideWebLoader();
            setTimeout(showErrorDialog, 500);
        }
    })
}

$('.update-quantity').click(function() {
    const productID = $(this).data('product');
    const updateType = $(this).data('update');

    if (productID.length > 0 && updateType.length > 0) {
        updateCartQty(productID, updateType, 0, $(this).siblings('input[name="quantity"]'));
    }
})

//--------
$('input[name="quantity"]').focus(function() {
    const currentQty = $(this).val();

    $(this).blur(() => {
        const newQty = $(this).val();
        const productID = $(this).data('product');
        if (newQty != currentQty) {
            updateCartQty(productID, "", newQty, $(this));
        }
    })
})