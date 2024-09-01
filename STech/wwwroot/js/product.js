$('.view-specification').click(() => {
    $('.product-specs-hidden').addClass('show');
})

$('.close-product-specs').click(() => {
    $('.product-specs-hidden').removeClass('show');
})

$('.view-contents').click(function () {
    $('.product-contents-container').toggleClass('opened');
    $(this).toggleClass('opened');


    if ($('.product-contents-container').hasClass('opened')) {
        $(this).find('a').html(`
            <span>Thu gọn</span>
            <i class="fa-solid fa-chevron-up"></i>
        `)
    } else {
        $(this).find('a').html(`
            <span>Xem tất cả</span>
            <i class="fa-solid fa-chevron-down"></i>
        `)
    }
})