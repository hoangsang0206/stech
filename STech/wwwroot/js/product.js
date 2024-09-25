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

const activeStars = () => {
    const selected_stars = $('.rating-star-group-large .fa-star.active').toArray();
    const selected_stars_count = selected_stars.length;

    $('.rating-star-group-large .fa-star').removeClass('active');
    $('.rating-star-group-large').toArray().map((item) => {
        $(item).find('.fa-star').slice(0, selected_stars_count).addClass('active');
    })
}   

$('.rating-star-group-large .fa-star').hover(function () {
    $(this).prevAll().addBack().addClass('hover');
    $(this).nextAll().removeClass('hover');
}, function () {
    $('.rating-star-group-large .fa-star').removeClass('hover');
})

$('.rating-star-group-large .fa-star').click(function () {
    $('.rating-star-group-large .fa-star').removeClass('hover');
    $('.rating-star-group-large .fa-star').removeClass('active');
    $(this).prevAll().addBack().addClass('active');
    $(this).nextAll().removeClass('active');

    activeStars();
})

$('.reviewer-select-star .fa-star').click(() => {
    $('.post-review').addClass('show');
})


$('.click-upload-review-images').click(() => {
    $('#click-upload-images').click();
})

$('#click-upload-images').change(function () {
    const files = $(this)[0].files;

    if (files.length > 3) {
        alert('Chỉ được chọn tối đa 3 ảnh');
        $(this).val(null);
        return;
    }

    const dataTransfer = new DataTransfer();
    Array.from($('#review-images')[0].files).map((file) => {
        dataTransfer.items.add(file);
    });

    Array.from(files).map((file) => {
        dataTransfer.items.add(file);
    })

    while (dataTransfer.items.length > 3) {
        dataTransfer.items.remove(0);
    }

    $('#review-images')[0].files = dataTransfer.files;
    $('#review-images').trigger('change');
})

$('#review-images').change(function () {
    const files = $(this)[0].files;

    const preview_images = $('.post-review__images')
    preview_images.empty();

    Array.from(files).map((file) => {
        if (file.type.startsWith('image/')) {
            const reader = new FileReader();

            reader.onload = function (e) {
                preview_images.append(`
                    <div><img src="${e.target.result}" /></div>
                `)
            };

            reader.readAsDataURL(file);
        }
    })
})


$('.post-review form').submit(function (e) {
    e.preventDefault();

    const product_id = $(this).data('product');
    const rating = $(this).find('.rating-star-group-large .fa-star.active').length;
    const content = $(this).find('#review-content').val();
    const images = $(this).find('#review-images')[0].files || null;

    const reviewer_name = $(this).find('#reviewer-name').val() || null;
    const reviewer_email = $(this).find('#reviewer-email').val() || null;
    const reviewer_phone = $(this).find('#reviewer-phone').val() || null;

    const reviewerInfo = reviewer_name != null && reviewer_email != null ? {
        ReviewerName: reviewer_name,
        ReviewerEmail: reviewer_email,
        ReviewerPhone: reviewer_phone
    } : null;


    const formData = new FormData();

    formData.append('ProductId', product_id);
    formData.append('Rating', rating);
    formData.append('Content', content);

    if (reviewerInfo != null) {
        formData.append('ReviewerInfo.ReviewerName', reviewerInfo.ReviewerName);
        formData.append('ReviewerInfo.ReviewerEmail', reviewerInfo.ReviewerEmail);
        formData.append('ReviewerInfo.ReviewerPhone', reviewerInfo.ReviewerPhone);
    }


    if (images != null) {
        Array.from(images).map((file) => {
            formData.append('files', file);
        })
    }

    //for (const [key, value] of formData.entries()) {
    //    console.log(key, value);
    //}

    const submit_btn = $(this).find('button[type="submit"]');
    const btn_element = showButtonLoader(submit_btn, '23px', '4px');
    $.ajax({
        url: '/api/reviews/post-review',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: (response) => {
            if (response.status) {
                hideButtonLoader(submit_btn, btn_element);
                showDialog('info', 'Gửi đánh giá thành công', 'Cảm ơn bạn đã gửi đánh giá cho sản phẩm này');
                $(this)[0].reset();
                $('.post-review__images').empty();
            } else {
                showDialog('error', 'Gửi đánh giá thất bại', response.message);
            }
        },
        error: () => {
            hideButtonLoader(submit_btn, btn_element);
        }
    })
})

const showReplyForm = (reply_button) => {
    $(reply_button).closest('.review-item').find('.review-reply-form').addClass('show');
}

const hideReplyForm = (form) => {
    $(form).removeClass('show');
}

$('.reply-review').not('.not-logged-in').click(function () {
    showReplyForm(this);
})

$('.cancel-review-reply').click(function () {
    const form = $(this).closest('.review-reply-form');
    hideReplyForm(form);
    form.find('form')[0].reset();
})

$('.review-reply-form form').submit(function (e) {
    e.preventDefault();

    const review_id = $(this).data('review');
    const content = $(this).find('.review-reply-input').val();

    if (!content) {
        return;
    }
    if (content.trim().length <= 0) {
        showDialog('warning', 'Vui lòng nhập nội dung', null);
        return;
    }

    const submit_btn = $(this).find('button[type="submit"]');
    const btn_element = showButtonLoader(submit_btn, '15px', '4px');

    $.ajax({
        type: 'POST',
        url: '/api/reviews/post-review-reply',
        contentType: 'application/json',
        data: JSON.stringify({
            ReviewId: review_id,
            Content: content
        }),
        success: (response) => {
            hideButtonLoader(submit_btn, btn_element);
            if (response.status) {
                showDialog('info', 'Phản hồi đã được gửi', 'Cảm ơn bạn đã trả lời đánh giá này');
                $(this)[0].reset();
                hideReplyForm($(this).closest('.review-reply-form'));
            } else {
                showDialog('error', 'Gửi phản hồi thất bại', response.message);
            }
        },
        error: () => {
            hideButtonLoader(submit_btn, btn_element);
        }
    })
})

const renderReviews = (reviews) => { 

}

const loadReviews = (sort_by, page) => { 

}

$(document).ready(() => { 

})